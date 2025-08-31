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

public class Menubar : Window
{
    private const int SidePadding = 3;
    private const int ItemGap = 6;
    private const int RightMargin = 8;
    private const int CharcoalHeight = 10;
    private const ushort ContextWidth = 155;
    
    // Simple static menu storage
    private static List<MenuModel> _activeMenus = null;
    private static Window _menuOwner = null;

    // Hard-coded root menu items
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

    private readonly int BarHeight;
    private int lastCanvasWidth;
    private byte lastSecond = RTC.Second;
    private Button menuButton;
    private int menusRightEdge;
    
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
        
        // Create GoOS menu button with correct 15x15 size
        menuButton = new Button(this,
            SidePadding,
            2,
            15,
            15,
            "")
        {
            UseSystemStyle = false,
            BackgroundColour = Color.Transparent,
            Image = menuicon,
            TextColour = Color.Black,
            RenderWithAlpha = true
        };
        
        menuButton.Clicked = () =>
        {
            // Show menu at proper position
            ContextMenu.ShowAt(menuButton.X, Contents.Height, RootMenuItems, ContextWidth, RootMenu_Handle);
            
            // Redraw immediately to prevent darkening
            DrawBackground();
            menuButton.Render();
            RenderClock();
        };
        
        DrawBackground();
        menuButton.Render();
        RenderClock();
    }

    public static Menubar Instance { get; private set; }
    
    // Menu registration
    public static void RegisterMenus(Window window, List<MenuModel> menus)
    {
        _activeMenus = menus;
        _menuOwner = window;
        
        if (Instance != null)
        {
            Instance.DrawMenus();
        }
    }

    // Clear menus
    public static void ClearMenus(Window window)
    {
        if (_menuOwner == window)
        {
            _activeMenus = null;
            _menuOwner = null;
            
            if (Instance != null)
            {
                Instance.DrawMenus();
            }
        }
    }

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
                Dialogue.Show("GoOS", "Are you sure you want to restart your computer?",
                    new List<DialogueButton> { new() { Text = "Reboot", Callback = () => Power.Reboot() } },
                    question);
                break;
            case " Shutdown Computer ":
                Dialogue.Show("GoOS", "Are you sure you want to shut down your computer?",
                    new List<DialogueButton> { new() { Text = "Shut Down", Callback = () => Power.Shutdown() } },
                    question);
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
        
        // Redraw menubar after handling menu action
        DrawBackground();
        menuButton.Render();
        DrawMenus();
    }
    
    private void DrawBackground()
    {
        var tile = menubarBackground;
        if (tile != null && tile.Width == 1 && tile.Height == BarHeight)
            for (var x = 0; x < Contents.Width; x++)
                Contents.DrawImage(x, 0, tile, false);
    }
    
    // Draw menu buttons
    private void DrawMenus()
    {
        // Remove all existing menu buttons except GoOS button
        var toRemove = new List<Control>();
        foreach (var c in Controls)
            if (c is Button b && b != menuButton)
                toRemove.Add(c);
                
        foreach (var c in toRemove) 
            Controls.Remove(c);
        
        // Draw background again
        DrawBackground();
        menuButton.Render();
        
        // No active menus? Just draw the clock
        if (_activeMenus == null)
        {
            RenderClock();
            return;
        }
        
        // Draw menu buttons
        var x = menuButton.X + menuButton.Contents.Width + ItemGap;
        var btnH = 15; // Match height to menu icon
        var btnY = 2;
        
        for (int i = 0; i < _activeMenus.Count; i++)
        {
            var menu = _activeMenus[i];
            
            // Skip empty menus
            if (string.IsNullOrEmpty(menu.Title))
                continue;
            
            // Calculate button width
            var width = Charcoal.MeasureString(menu.Title) + 12;
            if (x > Contents.Width - 180) 
                break;
            
            // Create standard button with transparent background
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
            
            // Simple click handler
            int menuIndex = i;
            
            btn.Clicked = () => {
                ShowMenuItems(menuIndex);
                
                // Force redraw immediately after click
                DrawBackground();
                menuButton.Render();
                DrawMenus();
            };
            
            btn.Render();
            x += width + ItemGap;
        }
        
        menusRightEdge = x;
        RenderClock();
    }
    
    // Show menu items when a menu button is clicked
    private void ShowMenuItems(int menuIndex)
    {
        // Validation
        if (_activeMenus == null || menuIndex < 0 || menuIndex >= _activeMenus.Count)
            return;
            
        var menu = _activeMenus[menuIndex];
        
        // No items? Skip
        if (menu.Items == null || menu.Items.Count == 0)
            return;
            
        // Convert menu items to strings
        string[] items = new string[menu.Items.Count];
        for (int i = 0; i < menu.Items.Count; i++)
        {
            items[i] = menu.Items[i].IsSeparator ? "----" : menu.Items[i].Text;
        }
        
        // Get the button that was clicked
        Button menuButton = null;
        foreach (var control in Controls)
        {
            if (control is Button btn && btn != this.menuButton)
            {
                if (btn.Title == menu.Title)
                {
                    menuButton = btn;
                    break;
                }
            }
        }
        
        // Show the menu at the proper position
        if (menuButton != null)
        {
            ContextMenu.ShowAt(menuButton.X, Contents.Height, items, ContextWidth, selectedItem => 
            {
                // Find and execute the action
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    if (!menu.Items[i].IsSeparator && menu.Items[i].Text == selectedItem)
                    {
                        menu.Items[i].OnClick?.Invoke();
                        break;
                    }
                }
                
                // Force complete redraw to prevent darkening
                DrawBackground();
                this.menuButton.Render();
                DrawMenus();
            });
        }
    }

    private void RenderClock()
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
        base.HandleRun();
        
        // Update clock once per second
        var currentSecond = RTC.Second;
        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            RenderClock();
        }
        
        // Handle canvas resize
        if (WindowManager.Canvas.Width != lastCanvasWidth)
        {
            lastCanvasWidth = WindowManager.Canvas.Width;
            Contents = new Canvas(WindowManager.Canvas.Width, (ushort)BarHeight);
            DrawMenus();
        }
    }

    public static void CheckForUpdates()
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
                            "The next GoOS has been released.\nWe don't want to force you to update but at least check out what's new in GoOS " +
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

    public readonly struct MenuModel
    {
        public readonly string Title;
        public readonly List<MenuItem> Items;

        public MenuModel(string title, List<MenuItem> items)
        {
            Title = title ?? string.Empty;
            Items = items ?? new List<MenuItem>();
        }
    }

    public readonly struct MenuItem
    {
        public readonly string Text;
        public readonly Action OnClick;
        public readonly bool IsSeparator;
        public readonly string Shortcut;
        public readonly bool IsDisabled;

        public MenuItem(string text, Action onClick, string shortcut = null, bool isDisabled = false)
        {
            Text = text ?? string.Empty;
            OnClick = onClick;
            IsSeparator = false;
            Shortcut = shortcut ?? string.Empty;
            IsDisabled = isDisabled;
        }

        private MenuItem(bool _sep)
        {
            Text = string.Empty;
            OnClick = null;
            IsSeparator = true;
            Shortcut = string.Empty;
            IsDisabled = false;
        }

        public static MenuItem Separator()
        {
            return new MenuItem(true);
        }
    }
}