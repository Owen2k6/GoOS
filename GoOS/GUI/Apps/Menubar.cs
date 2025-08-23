using System;
using System.Collections.Generic;
using Cosmos.HAL.Drivers.Video;
using GoGL.Graphics;
using GoOS.GUI;
using System.Net.Sockets;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.Config;
using Cosmos.System;
using static GoOS.Resources;
using GoOS.GUI.Apps.Settings;
using GoOS.GUI.Apps.GoWeb;
using GoOS.GUI.Apps.Gosplorer;
using GoOS.GUI.Apps.GoIDE;
using GoOS.GUI.Apps.GoStore;

namespace GoOS.GUI.Apps
{
    /// <summary>
    /// Minimal, safe OS 9–style menubar:
    ///  - Background: tiles 1×H Resources.menubarBackground
    ///  - Left: flat “Menu” button (dropdown under button)
    ///  - Left area: per-app menu labels (dropdown under each label)
    ///  - Right: date then time (black text), small offsets
    ///  - No focused-window title
    /// Safety:
    ///  - No immediate renders from hooks
    ///  - Render re-entrancy guard
    ///  - Deferred menu registration processed in HandleRun
    /// </summary>
    public class Menubar : Window
    {
        // public surface
        public static Menubar Instance { get; private set; }
        public static bool IsRenderingNow => Instance != null && Instance._isRendering;

        // layout
        private readonly int BarHeight;
        private const int SidePadding = 6;
        private const int ItemGap = 6;
        private const int RightMargin = 8;
        private const int Font1xHeight = 10; // your 1x font height
        private const ushort ContextWidth = 155;

        // state
        private Button menuButton;
        private static readonly Dictionary<Window, List<MenuModel>> menusByWindow = new();
        private int menusRightEdge;
        private byte lastSecond = Cosmos.HAL.RTC.Second;
        private int lastCanvasWidth;
        private bool needsRedraw = true;
        private bool _isRendering;

        // deferred menu registration (avoid re-entrancy)
        private struct PendingMenus { public Window Win; public List<MenuModel> Models; }
        private readonly Queue<PendingMenus> _pendingMenus = new();
        private bool _hasPendingMenus;

        // root menu items (match ContextMenu labels)
        private static readonly string[] RootMenuItems =
        {
            " About GoOS ",
            " Check for Updates ",
            " ----------------- ",
            " Clock App ",
            " GoStore ",
            " GoIDE ",
            " Gosplorer ",
            " GoWeb ",
            " Notepad ",
            " Paint ",
            " System Monitor ",
            " Settings ",
            " Terminal ",
            " GTerm (OLD) ",
            " ----------------- ",
            " Restart Computer ",
            " Shutdown Computer "
        };

        public Menubar()
        {
            Instance = this;

            var tile = Resources.menubarBackground;
            BarHeight = (tile != null && tile.Width == 1 && tile.Height > 0) ? tile.Height : 19;

            X = 0;
            Y = 0;
            Contents = new Canvas((ushort)WindowManager.Canvas.Width, (ushort)BarHeight);
            lastCanvasWidth = WindowManager.Canvas.Width;

            Title = nameof(Menubar);
            Visible = true;
            Closable = false;
            HasTitlebar = false;
            Unkillable = true;

            InitialiseMenuButton();

            // focus hook: mark dirty only (never render here)
            WindowManager.TaskbarFocusChangedHook = () => { if (!_isRendering) needsRedraw = true; };

            // first frame
            RenderWindow();
        }

        private void InitialiseMenuButton()
        {
            int btnH = Math.Max(1, BarHeight - 4);
            int btnY = 2;
            int btnW = 40;

            menuButton = new Button(this,
                                    (ushort)SidePadding,
                                    (ushort)btnY,
                                    (ushort)btnW,
                                    (ushort)btnH,
                                    "Menu")
            {
                UseSystemStyle = false,               // flat label
                BackgroundColour = Color.Transparent,
                TextColour = Color.Black,
                RenderWithAlpha = true,
            };

            // dropdown anchored under the button
            menuButton.Clicked = () =>
            {
                int anchorX = X + menuButton.X;
                int anchorY = Y + Contents.Height;
                ContextMenu.ShowAt(anchorX, anchorY, RootMenuItems, ContextWidth, RootMenu_Handle);
            };

            menuButton.Render();
        }

        // ===== external API for apps (deferred) =====
        public static void RegisterMenus(Window window, IEnumerable<MenuModel> menus)
        {
            if (window == null || menus == null || Instance == null) return;

            var copy = new List<MenuModel>();
            foreach (var m in menus) copy.Add(m);

            Instance._pendingMenus.Enqueue(new PendingMenus { Win = window, Models = copy });
            Instance._hasPendingMenus = true;
        }

        public static void ClearMenus(Window window)
        {
            if (window == null || Instance == null) return;
            menusByWindow.Remove(window);
            Instance.needsRedraw = true;
        }

        // ===== root dropdown =====
        private void RootMenu_Handle(string item)
        {
            switch (item)
            {
                case " About GoOS ":
                    WindowManager.AddWindow(new About());
                    break;

                case " Check for Updates ":
                    CheckForUpdates();
                    break;

                case " Restart Computer ":
                    Dialogue.Show(
                        "GoOS",
                        "Are you sure you want to restart your computer?",
                        new() { new() { Text = "Reboot", Callback = () => Cosmos.System.Power.Reboot() } },
                        question
                    );
                    break;

                case " Shutdown Computer ":
                    Dialogue.Show(
                        "GoOS",
                        "Are you sure you want to shut down your computer?",
                        new() { new() { Text = "Shut Down", Callback = () => Cosmos.System.Power.Shutdown() } },
                        question
                    );
                    break;

                case " Clock App ":
                    WindowManager.AddWindow(new Clock()); break;
                case " GoStore ":
                    WindowManager.AddWindow(new GoStore.MainFrame()); break;
                case " GoIDE ":
                    WindowManager.AddWindow(new WelcomeFrame()); break;
                case " Gosplorer ":
                    WindowManager.AddWindow(new Gosplorer.MainFrame()); break;
                case " GoWeb ":
                    WindowManager.AddWindow(new GoWebWindow()); break;
                case " Notepad ":
                    WindowManager.AddWindow(new Notepad(false, null)); break;
                case " Paint ":
                    WindowManager.AddWindow(new Paintbrush()); break;
                case " System Monitor ":
                    WindowManager.AddWindow(new TaskManager()); break;
                case " Settings ":
                    WindowManager.AddWindow(new Frame()); break;
                case " Terminal ":
                    WindowManager.AddWindow(new Terminal()); break;
                case " GTerm (OLD) ":
                    WindowManager.AddWindow(new GTerm(false)); break;
            }
        }

        // ===== rendering =====
        private void RenderWindow()
        {
            if (_isRendering) return;     // re-entrancy guard
            _isRendering = true;
            try
            {
                if (WindowManager.Canvas.Width != lastCanvasWidth || Contents.Width != WindowManager.Canvas.Width)
                {
                    lastCanvasWidth = WindowManager.Canvas.Width;
                    Contents = new Canvas((ushort)WindowManager.Canvas.Width, (ushort)BarHeight);
                    needsRedraw = true;
                }

                if (!needsRedraw) return;

                DrawBackgroundTiled();
                RenderControls();
                RenderAppMenusLeft();
                RenderRightClockAndDate();

                needsRedraw = false;
            }
            finally
            {
                _isRendering = false;
            }
        }

        private void DrawBackgroundTiled()
        {
            var tile = Resources.menubarBackground;
            if (tile != null && tile.Width == 1 && tile.Height == BarHeight)
            {
                for (int x = 0; x < Contents.Width; x++)
                    Contents.DrawImage(x, 0, tile, false);
            }
            // no fallback fill: keep previous frame if resource missing
        }

        private void RenderAppMenusLeft()
        {
            // remove previous label buttons (keep Menu)
            var toRemove = new List<Control>();
            foreach (var c in Controls)
            {
                if (c is Button b && b != menuButton)
                    toRemove.Add(c);
            }
            foreach (var c in toRemove) Controls.Remove(c);

            int x = menuButton.X + menuButton.Contents.Width + ItemGap;
            int btnH = Math.Max(1, BarHeight - 4);
            int btnY = 2;

            var focused = WindowManager.FocusedWindow;
            if (focused != null && menusByWindow.TryGetValue(focused, out var menus) && menus != null)
            {
                foreach (var menu in menus)
                {
                    int width = Resources.Font_1x.MeasureString(menu.Title) + 12;
                    if (x > Contents.Width - 180) break;

                    var btn = new Button(this,
                                         (ushort)x,
                                         (ushort)btnY,
                                         (ushort)width,
                                         (ushort)btnH,
                                         menu.Title)
                    {
                        UseSystemStyle = false,
                        BackgroundColour = Color.Transparent,
                        TextColour = Color.Black,
                        RenderWithAlpha = true,
                    };

                    var entries = menu.Items; // capture
                    btn.Clicked = () =>
                    {
                        // build a local dispatch map to avoid shared state
                        var clickMap = new Dictionary<string, Action>(entries.Count);
                        var labels = new string[entries.Count];

                        for (int i = 0; i < entries.Count; i++)
                        {
                            if (entries[i].IsSeparator)
                            {
                                labels[i] = "----";
                            }
                            else
                            {
                                string text = entries[i].Text ?? string.Empty;
                                labels[i] = text;
                                if (!clickMap.ContainsKey(text) && entries[i].OnClick != null)
                                    clickMap[text] = entries[i].OnClick;
                            }
                        }

                        int anchorX = X + btn.X;
                        int anchorY = Y + Contents.Height;

                        ContextMenu.ShowAt(anchorX, anchorY, labels, ContextWidth, label =>
                        {
                            if (label == "----" || string.IsNullOrEmpty(label)) return;
                            if (clickMap.TryGetValue(label, out var act) && act != null) act();
                        });
                    };

                    btn.Render();
                    x += width + ItemGap;
                }
            }

            menusRightEdge = x;
            RenderControls();
        }

        // offsets: baseline +5; small right shifts (stable)
        private void RenderRightClockAndDate()
        {
            string timeString = DateTime.Now.ToString("HH:mm");
            string dateString = DateTime.Now.ToString("dd/MM/yyyy");

            int timeW = Resources.Font_1x.MeasureString(timeString);
            int dateW = Resources.Font_1x.MeasureString(dateString);

            int baselineY = ((BarHeight - Font1xHeight) / 2) + 5;

            int timeX = Contents.Width - RightMargin - timeW + 20;
            int dateX = (timeX - 6 - dateW) + 24;

            if (dateX <= menusRightEdge + ItemGap)
            {
                Contents.DrawString(timeX, baselineY, timeString, Resources.Font_1x, Color.Black, true);
                return;
            }

            Contents.DrawString(dateX, baselineY, dateString, Resources.Font_1x, Color.Black, true);
            Contents.DrawString(timeX, baselineY, timeString, Resources.Font_1x, Color.Black, true);
        }

        public override void HandleRun()
        {
            // apply deferred menu registrations safely here
            if (_hasPendingMenus)
            {
                while (_pendingMenus.Count > 0)
                {
                    var p = _pendingMenus.Dequeue();
                    menusByWindow[p.Win] = p.Models; // replace for that window
                }
                _hasPendingMenus = false;
                needsRedraw = true;
            }

            base.HandleRun();

            bool widthChanged = (WindowManager.Canvas.Width != lastCanvasWidth);
            byte currentSecond = Cosmos.HAL.RTC.Second;

            if (currentSecond != lastSecond || widthChanged || needsRedraw)
            {
                lastSecond = currentSecond;
                RenderWindow();
            }
        }

        // ===== “Check for Updates” (same behaviour as Desktop) =====
        private static void CheckForUpdates()
        {
            try
            {
                string apiHost = "api.goos.owen2k6.com";
                string serverIP;

                using (var dnsClient = new DnsClient())
                {
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk(apiHost);
                    Address address = dnsClient.Receive();
                    dnsClient.Close();

                    serverIP = address?.ToString();
                    if (string.IsNullOrEmpty(serverIP))
                    {
                        Dialogue.Show("GoOS Update", "DNS resolution failed.");
                        return;
                    }
                }

                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect(serverIP, 80);
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        string path = "/GoOS/" + Kernel.edition + ".goos";
                        string http =
                            "GET " + path + " HTTP/1.1\r\n" +
                            "Host: " + apiHost + "\r\n" +
                            "User-Agent: GoOS\r\n" +
                            "Accept: */*\r\n" +
                            "Accept-Encoding: identity\r\n" +
                            "Connection: close\r\n\r\n";

                        byte[] req = System.Text.Encoding.ASCII.GetBytes(http);
                        stream.Write(req, 0, req.Length);

                        byte[] buf = new byte[tcpClient.ReceiveBufferSize];
                        int read = stream.Read(buf, 0, buf.Length);
                        if (read <= 0)
                        {
                            Dialogue.Show("GoOS Update", "Empty HTTP response.");
                            return;
                        }

                        string resp = System.Text.Encoding.ASCII.GetString(buf, 0, read);
                        int sep = resp.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                        if (sep < 0)
                        {
                            Dialogue.Show("GoOS Update", "Invalid HTTP response!");
                            return;
                        }

                        string body = resp.Substring(sep + 4).Trim();

                        if (body != Kernel.version && body != Kernel.editionnext && Kernel.BuildType != "INTERNAL TEST BUILD")
                        {
                            Dialogue.Show("GoOS Update",
                                "A newer version of GoOS is available on Github.\nWe recommend you update to the latest version for stability and security reasons.\nhttps://github.com/Owen2k6/GoOS/releases\nCurrent Version: " +
                                Kernel.version + "\nLatest Version: " + body);
                        }
                        if (body != Kernel.version && body != Kernel.editionnext && Kernel.BuildType == "INTERNAL TEST BUILD")
                        {
                            Dialogue.Show("It's time to move on...",
                                "The Internal Test Version for this edition of GoOS has ended\nThis build of GoOS can no longer access GoOS Online Services.\nPlease check with your INTERNAL TEST Group to see if a new version has been issued.");
                        }
                        else if (body == Kernel.editionnext)
                        {
                            Dialogue.Show("GoOS Update",
                                "The next GoOS has been released.\nWe don't want to force you to update but at least check out whats new in GoOS " +
                                Kernel.editionnext + "!\nhttps://github.com/Owen2k6/GoOS/releases/tag/" +
                                Kernel.editionnext);
                        }
                        else if (body == "404")
                        {
                            Dialogue.Show("GoOS Update", "Your Version of GoOS does not support GoOS Update.");
                        }
                        else
                        {
                            Dialogue.Show("GoOS Update",
                                "You are running the latest version of GoOS.\nVersion: " + Kernel.version +
                                "\nLatest Version: " + body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("GoOS Update", "Failed to connect to Owen2k6 Api. Error: " + ex);
            }
        }

        // ===== simple models for per-app menus =====
        public readonly struct MenuModel
        {
            public readonly string Title;
            public readonly List<MenuItem> Items;

            public MenuModel(string title, IEnumerable<MenuItem> items)
            {
                Title = title ?? string.Empty;
                Items = new List<MenuItem>(items ?? Array.Empty<MenuItem>());
            }
        }

        public readonly struct MenuItem
        {
            public readonly string Text;
            public readonly Action OnClick;
            public readonly bool IsSeparator;

            public MenuItem(string text, Action onClick)
            {
                Text = text ?? string.Empty;
                OnClick = onClick;
                IsSeparator = false;
            }

            private MenuItem(bool _sep)
            {
                Text = string.Empty;
                OnClick = null;
                IsSeparator = true;
            }

            public static MenuItem Separator() => new MenuItem(true);
        }
    }
}
