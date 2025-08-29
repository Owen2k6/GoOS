using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Gold.Graphics;
using GoOS.GUI.Apps.GoIDE;
using GoOS.GUI.Apps.GoStore;
using GoOS.GUI.Apps.GoWeb;
using GoOS.GUI.Apps.Settings;
using static GoOS.Resources;
using Power = Cosmos.System.Power;

namespace GoOS.GUI.Apps;

/// <summary>
///     Minimal, safe OS 9–style menubar:
///     - Background: tiles 1×H Resources.menubarBackground
///     - Left: flat “Menu” button (dropdown under button)
///     - Left area: per-app menu labels (dropdown under each label)
///     - Right: date then time (black text), small offsets
///     - No focused-window title
///     Safety:
///     - No immediate renders from hooks
///     - Render re-entrancy guard
///     - Deferred menu registration processed in HandleRun
/// </summary>
public class Menubar : Window
{
    private const int SidePadding = 3;
    private const int ItemGap = 6;
    private const int RightMargin = 8;
    private const int CharcoalHeight = 10;
    private const ushort ContextWidth = 155;
    private static readonly Dictionary<Window, List<MenuModel>> menusByWindow = new();

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
        " ----------------- ",
        " Restart Computer ",
        " Shutdown Computer "
    };

    private readonly Queue<PendingMenus> _pendingMenus = new();

    // layout
    private readonly int BarHeight;
    private bool _hasPendingMenus;
    private bool _isRendering;
    private int lastCanvasWidth;
    private byte lastSecond = RTC.Second;

    // state
    private Button menuButton;
    private int menusRightEdge;
    private bool needsRedraw = true;

    public Menubar()
    {
        Instance = this;

        var tile = menubarBackground;
        BarHeight = tile != null && tile.Width == 1 && tile.Height > 0 ? tile.Height : 19;

        X = 0;
        Y = 0;
        Contents = new Canvas(WindowManager.Canvas.Width, (ushort)BarHeight);
        lastCanvasWidth = WindowManager.Canvas.Width;

        Title = nameof(Menubar);
        Visible = true;
        Closable = false;
        HasTitlebar = false;
        Unkillable = true;

        InitialiseMenuButton();

        // focus hook: mark dirty only (never render here)
        WindowManager.TaskbarFocusChangedHook = () =>
        {
            if (!_isRendering) needsRedraw = true;
        };

        // first frame
        RenderWindow();
    }

    // public surface
    public static Menubar Instance { get; private set; }
    public static bool IsRenderingNow => Instance != null && Instance._isRendering;

    private void InitialiseMenuButton()
    {
        var btnH = Math.Max(1, BarHeight - 4);
        var btnY = 2;
        var btnW = 16;

        menuButton = new Button(this,
            SidePadding,
            (ushort)btnY,
            (ushort)btnW,
            (ushort)btnH,
            "")
        {
            UseSystemStyle = false, // flat label
            BackgroundColour = Color.Transparent,
            Image = menuicon,
            TextColour = Color.Black,
            RenderWithAlpha = true
        };

        // dropdown anchored under the button
        menuButton.Clicked = () =>
        {
            var anchorX = X + menuButton.X;
            var anchorY = Y + Contents.Height;
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
                    new List<DialogueButton> { new() { Text = "Reboot", Callback = () => Power.Reboot() } },
                    question
                );
                break;

            case " Shutdown Computer ":
                Dialogue.Show(
                    "GoOS",
                    "Are you sure you want to shut down your computer?",
                    new List<DialogueButton> { new() { Text = "Shut Down", Callback = () => Power.Shutdown() } },
                    question
                );
                break;

            case " Clock App ":
                WindowManager.AddWindow(new Clock()); break;
            case " GoStore ":
                WindowManager.AddWindow(new MainFrame()); break;
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
                WindowManager.AddWindow(new GTerm()); break;
        }
    }

    // ===== rendering =====
    private void RenderWindow()
    {
        if (_isRendering) return; // re-entrancy guard
        _isRendering = true;
        try
        {
            if (WindowManager.Canvas.Width != lastCanvasWidth || Contents.Width != WindowManager.Canvas.Width)
            {
                lastCanvasWidth = WindowManager.Canvas.Width;
                Contents = new Canvas(WindowManager.Canvas.Width, (ushort)BarHeight);
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
        var tile = menubarBackground;
        if (tile != null && tile.Width == 1 && tile.Height == BarHeight)
            for (var x = 0; x < Contents.Width; x++)
                Contents.DrawImage(x, 0, tile, false);
        // no fallback fill: keep previous frame if resource missing
    }

    private void RenderAppMenusLeft()
    {
        // remove previous label buttons (keep Menu)
        var toRemove = new List<Control>();
        foreach (var c in Controls)
            if (c is Button b && b != menuButton)
                toRemove.Add(c);
        foreach (var c in toRemove) Controls.Remove(c);

        var x = menuButton.X + menuButton.Contents.Width + ItemGap;
        var btnH = Math.Max(1, BarHeight - 4);
        var btnY = 2;

        var focused = WindowManager.FocusedWindow;
        if (focused != null && menusByWindow.TryGetValue(focused, out var menus) && menus != null)
            foreach (var menu in menus)
            {
                var width = Charcoal.MeasureString(menu.Title) + 12;
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
                    RenderWithAlpha = true
                };

                var entries = menu.Items; // capture
                btn.Clicked = () =>
                {
                    // build a local dispatch map to avoid shared state
                    var clickMap = new Dictionary<string, Action>(entries.Count);
                    var labels = new string[entries.Count];

                    for (var i = 0; i < entries.Count; i++)
                        if (entries[i].IsSeparator)
                        {
                            labels[i] = "----";
                        }
                        else
                        {
                            var text = entries[i].Text ?? string.Empty;
                            labels[i] = text;
                            if (!clickMap.ContainsKey(text) && entries[i].OnClick != null)
                                clickMap[text] = entries[i].OnClick;
                        }

                    var anchorX = X + btn.X;
                    var anchorY = Y + Contents.Height;

                    ContextMenu.ShowAt(anchorX, anchorY, labels, ContextWidth, label =>
                    {
                        if (label == "----" || string.IsNullOrEmpty(label)) return;
                        if (clickMap.TryGetValue(label, out var act) && act != null) act();
                    });
                };

                btn.Render();
                x += width + ItemGap;
            }

        menusRightEdge = x;
        RenderControls();
    }

    // offsets: baseline +5; small right shifts (stable)
    private void RenderRightClockAndDate()
    {
        var timeString = DateTime.Now.ToString("HH:mm");
        var dateString = DateTime.Now.ToString("dd/MM/yyyy");

        int timeW = Charcoal.MeasureString(timeString);
        int dateW = Charcoal.MeasureString(dateString);

        var baselineY = (BarHeight - CharcoalHeight) / 2 + 5;

        var timeX = Contents.Width - RightMargin - timeW + 20;
        var dateX = timeX - 6 - dateW + 24;

        if (dateX <= menusRightEdge + ItemGap)
        {
            Contents.DrawString(timeX, baselineY, timeString, Charcoal, Color.Black, true);
            return;
        }

        Contents.DrawString(dateX, baselineY, dateString, Charcoal, Color.Black, true);
        Contents.DrawString(timeX, baselineY, timeString, Charcoal, Color.Black, true);
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

        var widthChanged = WindowManager.Canvas.Width != lastCanvasWidth;
        var currentSecond = RTC.Second;

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
            var apiHost = "api.goos.owen2k6.com";
            string serverIP;

            using (var dnsClient = new DnsClient())
            {
                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                dnsClient.SendAsk(apiHost);
                var address = dnsClient.Receive();
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
                using (var stream = tcpClient.GetStream())
                {
                    var path = "/GoOS/" + Kernel.edition + ".goos";
                    var http =
                        "GET " + path + " HTTP/1.1\r\n" +
                        "Host: " + apiHost + "\r\n" +
                        "User-Agent: GoOS\r\n" +
                        "Accept: */*\r\n" +
                        "Accept-Encoding: identity\r\n" +
                        "Connection: close\r\n\r\n";

                    var req = Encoding.ASCII.GetBytes(http);
                    stream.Write(req, 0, req.Length);

                    var buf = new byte[tcpClient.ReceiveBufferSize];
                    var read = stream.Read(buf, 0, buf.Length);
                    if (read <= 0)
                    {
                        Dialogue.Show("GoOS Update", "Empty HTTP response.");
                        return;
                    }

                    var resp = Encoding.ASCII.GetString(buf, 0, read);
                    var sep = resp.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                    if (sep < 0)
                    {
                        Dialogue.Show("GoOS Update", "Invalid HTTP response!");
                        return;
                    }

                    var body = resp.Substring(sep + 4).Trim();

                    if (body != Kernel.version && body != Kernel.editionnext &&
                        Kernel.BuildType != "INTERNAL TEST BUILD")
                        Dialogue.Show("GoOS Update",
                            "A newer version of GoOS is available on Github.\nWe recommend you update to the latest version for stability and security reasons.\nhttps://github.com/Owen2k6/GoOS/releases\nCurrent Version: " +
                            Kernel.version + "\nLatest Version: " + body);
                    if (body != Kernel.version && body != Kernel.editionnext &&
                        Kernel.BuildType == "INTERNAL TEST BUILD")
                        Dialogue.Show("It's time to move on...",
                            "The Internal Test Version for this edition of GoOS has ended\nThis build of GoOS can no longer access GoOS Online Services.\nPlease check with your INTERNAL TEST Group to see if a new version has been issued.");
                    else if (body == Kernel.editionnext)
                        Dialogue.Show("GoOS Update",
                            "The next GoOS has been released.\nWe don't want to force you to update but at least check out whats new in GoOS " +
                            Kernel.editionnext + "!\nhttps://github.com/Owen2k6/GoOS/releases/tag/" +
                            Kernel.editionnext);
                    else if (body == "404")
                        Dialogue.Show("GoOS Update", "Your Version of GoOS does not support GoOS Update.");
                    else
                        Dialogue.Show("GoOS Update",
                            "You are running the latest version of GoOS.\nVersion: " + Kernel.version +
                            "\nLatest Version: " + body);
                }
            }
        }
        catch (Exception ex)
        {
            Dialogue.Show("GoOS Update", "Failed to connect to Owen2k6 Api. Error: " + ex);
        }
    }

    // deferred menu registration (avoid re-entrancy)
    private struct PendingMenus
    {
        public Window Win;
        public List<MenuModel> Models;
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

        public static MenuItem Separator()
        {
            return new MenuItem(true);
        }
    }
}