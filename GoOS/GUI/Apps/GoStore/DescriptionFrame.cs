using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Gold.Graphics;
using GoOS._9xCode;
using GoOS.Commands;
// Window, Button
// Control base, if needed
using IO = System.IO;
using static GoOS.Resources;
using Console = BetterConsole;

namespace GoOS.GUI.Apps.GoStore;

public class DescriptionFrame : Window
{
    // Bottom bar metrics (match MainFrame’s footer)
    private const int FOOTER_H = 23;

    // Colours (OS 9 look)
    private static readonly Color PanelFill = new(0xFFFFFFFF); // OS9 white background
    private static readonly Color FooterFill = new(0xFFDADADA); // platinum strip
    private static readonly Color DividerDark = new(0xFFB3B3B3);
    private static readonly Color TextBlack = Color.Black;
    private readonly Application App;
    private readonly List<string> DescriptionLines = new();

    private Button InstallButton;
    private Button OpenButton;

    public DescriptionFrame(Application app)
    {
        App = app;

        // Create the window (transparent base; we paint our own white background)
        Contents = new Canvas(480, 360);
        Contents.Clear(new Color(0x00000000));
        Title = "GoStore";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Center);

        // Prepare description text (using Charcoal when drawn)
        DescriptionLines.AddRange(SpliceText(app.Description.Replace("\\n", "\n") + "\n\n", 55));
        DescriptionLines.AddRange(SpliceText("Version: " + app.Version.Replace("\\n", "\n"), 55));
        DescriptionLines.AddRange(SpliceText("Author: " + app.Author.Replace("\\n", "\n"), 55));
        DescriptionLines.AddRange(SpliceText("Category: " + app.Category.Replace("\\n", "\n"), 55));
        DescriptionLines.AddRange(SpliceText("GoOS Version: " + app.GoOSVersion.Replace("\\n", "\n"), 55));

        // Create buttons once; render in Paint()
        CreateButtons();
    }

    private void CreateButtons()
    {
        // Layout inside the bottom bar
        int btnH = 20, btnW = 120;
        var footerTop = Contents.Height - FOOTER_H;
        var btnY = footerTop + (FOOTER_H - btnH) / 2;

        // Open (left)
        OpenButton = new Button(this,
            8, (ushort)btnY, (ushort)btnW, (ushort)btnH, "Open")
        {
            UseSystemStyle = true,
            RenderWithAlpha = true,
            CenterTitle = true,
            Clicked = OpenButton_Click
        };

        // Install/Uninstall toggle (right)
        InstallButton = new Button(this,
            (ushort)(Contents.Width - 8 - btnW), (ushort)btnY, (ushort)btnW, (ushort)btnH,
            IO.File.Exists(@"0:\go\" + App.Filename) ? "Uninstall" : "Install")
        {
            UseSystemStyle = true,
            RenderWithAlpha = true,
            CenterTitle = true,
            Clicked = InstallButton_Click
        };

        // Reflect installed state in the bevel (pressed look when installed)
        UpdateInstallButtonStateVisual();
    }

    private void UpdateInstallButtonStateVisual()
    {
        var installed = IO.File.Exists(@"0:\go\" + App.Filename);
        InstallButton.Title = installed ? "Uninstall" : "Install";
        InstallButton.AppearPressed = installed; // pressed look = “installed”
    }

    private static string[] SpliceText(string input, int n)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            sb.Append(input[i]);
            if ((i + 1) % n == 0) sb.Append("\n");
        }

        return sb.ToString().Split('\n');
    }

    public override void Paint()
    {
        // Clear, then paint an OS9 white panel instead of a skinned image
        Contents.Clear(new Color(0x00000000));
        var font = Charcoal ?? Font_1x;

        // White background for the whole content area
        Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, PanelFill);

        // Text (black, OS9-style)
        Contents.DrawString(10, 10, App.Name, font, TextBlack);
        for (var i = 0; i < DescriptionLines.Count; i++)
            Contents.DrawString(10, 56 + i * 16, DescriptionLines[i], font, TextBlack);

        // Bottom bar (system-style strip)
        DrawBottomBar();

        // Render buttons on top
        OpenButton.Render();
        InstallButton.Render();

        RenderSystemStyleBorder();
    }

    private void DrawBottomBar()
    {
        var footerTop = Contents.Height - FOOTER_H;

        // Fill bar and top divider
        Contents.DrawFilledRectangle(0, footerTop, Contents.Width, FOOTER_H, 0, FooterFill);
        Contents.DrawLine(0, footerTop, Contents.Width, footerTop, DividerDark);
    }

    private void InstallButton_Click()
    {
        try
        {
            // Toggle install state
            if (!IO.File.Exists(@"0:\go\" + App.Filename))
            {
                using (var tcpClient = new TcpClient())
                {
                    var dnsClient = new DnsClient();

                    // DNS
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk(App.Repository);

                    // Address from IP
                    var address = dnsClient.Receive();
                    dnsClient.Close();
                    var serverIP = address.ToString();

                    tcpClient.Connect(serverIP, 80);
                    var stream = tcpClient.GetStream();
                    var httpget = "GET /" + App.Filename + " HTTP/1.1\r\n" +
                                  "User-Agent: GoOS\r\n" +
                                  "Accept: */*\r\n" +
                                  "Accept-Encoding: identity\r\n" +
                                  "Host: " + App.Repository + "\r\n" +
                                  "Connection: Keep-Alive\r\n\r\n";
                    var dataToSend = Encoding.ASCII.GetBytes(httpget);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    // Receive data
                    var receivedData = new byte[tcpClient.ReceiveBufferSize];
                    var bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                    var receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                    var parts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
                    if (parts.Length != 2)
                    {
                        Dialogue.Show("GoStore", "Invalid HTTP response!", default, WindowManager.errorIcon);
                        return;
                    }

                    if (parts[1] == "404")
                    {
                        Dialogue.Show("Error", "The requested file or resource was not found.", default,
                            WindowManager.errorIcon);
                        return;
                    }

                    if (!IO.Directory.Exists(@"0:\go")) IO.Directory.CreateDirectory(@"0:\go");
                    IO.File.WriteAllText(@"0:\go\" + App.Filename, parts[1]);
                }

                Dialogue.Show("Success", "Application installed successfully.");
            }
            else
            {
                while (IO.File.Exists(@"0:\go\" + App.Filename))
                    IO.File.Delete(@"0:\go\" + App.Filename);

                Dialogue.Show("Success", "Application uninstalled successfully.");
            }

            // Reflect new state on the same button (no re-creation)
            UpdateInstallButtonStateVisual();

            // Repaint once
            Paint();
        }
        catch (Exception ex)
        {
            Dialogue.Show("Error", "An error occurred while installing the app:\n" + ex.Message);
        }
    }

    private void OpenButton_Click()
    {
        if (!IO.File.Exists(@"0:\go\" + App.Filename))
        {
            Dialogue.Show("Error", "You must install the app before you can use it.", default, WindowManager.errorIcon);
            return;
        }

        Console.Clear();
        Console.Title = "Terminal - GoIDE";
        WindowManager.AddWindow(new GTerm(false));
        Console.Clear();

        if (!App.Filename.EndsWith(".9xc"))
            Run.Main(@"0:\go\" + App.Filename, false);
        else
            Interpreter.Run(@"0:\go\" + App.Filename);

        Console.Clear();
        WindowManager.RemoveWindowByTitle("Terminal - GoIDE");
        Console.Title = "GTerm";
    }
}