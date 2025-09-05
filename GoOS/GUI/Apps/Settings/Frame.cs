using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.ScanMaps;
using Gold;
using Gold.Graphics;
using Gold.Hardware.GPU;
using GoOS._9xCode;
using GoOS.GUI.Apps.GoStore;
using GoOS.GUI.Apps.GoWeb;

namespace GoOS.GUI.Apps.Settings;

public class Frame : Window
{
    private const int buttonHeight = 24;
    private readonly List<Button> sButtons = new();
    private int page;

    public Frame()
    {
        // Create the window.
        Contents = new Canvas(307, 400);
        Title = "Settings";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);
        // Paint the window.
        ReDraw();
        foreach (var control in Controls) control.Render();
    }

    public void ReDraw()
    {
        Contents.Clear(Color.LightGray);
        Controls.Clear();
        sButtons.Clear();
        Draw();
        AddSideButtons();
        foreach (var control in Controls) control.Render();
    }

    public void Draw()
    {
        Contents.DrawImage(0, 0, Resources.SBG, false);
        switch (page)
        {
            case 0:
                new Button(this, 109, 4, 185, 24, "About")
                {
                    Clicked = () =>
                    {
                        RenderInternalMenu("About GoOS");
                        Contents.DrawString(5, 33, "GoOS " + Kernel.editiontitle, Resources.Font_2x, Color.Black);
                        Contents.DrawString(5, 78, "Version " + Kernel.version, Resources.Geneva, Color.Black);
                        Contents.DrawString(5, 100, "Copyright (C) Owen2k6 " + Kernel.Copyright, Resources.Geneva,
                            Color.Black);
                        Contents.DrawString(5, 122, "Owen2k6 Open Sourced Licence", Resources.Geneva, Color.Black);
                        Contents.DrawString(5, 144, "Made In England", Resources.Geneva, Color.Black);
                        Contents.DrawString(5, 188, "System Memory: " + CPU.GetAmountOfRAM() + "MB",
                            Resources.Geneva, Color.Black);
                        Contents.DrawString(5, 222, "GoOS Implementations --", Resources.Geneva, Color.Black);
                        Contents.DrawString(5, 234, "Gold Version " + Info.Version, Resources.Geneva,
                            Color.Black);
                        Contents.DrawString(5, 246, "GoCode Version " + GoCode.GoCode.Version, Resources.Geneva,
                            Color.Black);
                        Contents.DrawString(5, 258, "9xCode Version " + Interpreter.Version, Resources.Geneva,
                            Color.Black);
                        Contents.DrawString(5, 270, "GoStore Version " + MainFrame.Version, Resources.Geneva,
                            Color.Black);
                        Contents.DrawString(5, 282, "GoWeb Version " + GoWebWindow.Version, Resources.Geneva,
                            Color.Black);
                        Contents.DrawString(5,294, "Giff Engine " + Giff.Giff.Version, Resources.Geneva, Color.Black);
                    }
                };
                new Button(this, 109, 4 + 24, 185, 24, "Software Update")
                {
                    Clicked = () =>
                    {
                        #region GoOS Update Check

                        try
                        {
                            using (var tcpClient = new TcpClient())
                            {
                                var dnsClient = new DnsClient();

                                // DNS
                                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                                dnsClient.SendAsk("api.goos.owen2k6.com");

                                // Address from IP
                                var address = dnsClient.Receive();
                                dnsClient.Close();
                                var serverIP = address.ToString();

                                tcpClient.Connect(serverIP, 80);
                                var stream = tcpClient.GetStream();
                                var httpget = "GET /GoOS/" + Kernel.edition + ".goos HTTP/1.1\r\n" +
                                              "User-Agent: GoOS\r\n" +
                                              "Accept: */*\r\n" +
                                              "Accept-Encoding: identity\r\n" +
                                              "Host: api.goos.owen2k6.com\r\n" +
                                              "Connection: Keep-Alive\r\n\r\n";
                                var dataToSend = Encoding.ASCII.GetBytes(httpget);
                                stream.Write(dataToSend, 0, dataToSend.Length);

                                // Receive data
                                var receivedData = new byte[tcpClient.ReceiveBufferSize];
                                var bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                                var receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                                var responseParts =
                                    receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                                if (responseParts.Length < 2 || responseParts.Length > 2)
                                    Dialogue.Show("GoOS Update", "Invalid HTTP response!", default,
                                        WindowManager.errorIcon);

                                var content = responseParts[1];

                                if (content != Kernel.version && content != Kernel.editionnext)
                                {
                                    if (Kernel.BuildType == "INTERNAL TEST BUILD")
                                    {
                                        RenderInternalMenu("Software Update");
                                        Contents.DrawString(5, 33, "GoOS " + content, Resources.Font_2x,
                                            Color.Black);
                                        Contents.DrawString(5, 78, "ITB Expired.", Resources.Geneva, Color.Black);
                                        Contents.DrawString(5, 100,
                                            "Check with Owen2k6 for ITB updates or return to GoOS Release.",
                                            Resources.Geneva, Color.Black);
                                    }

                                    RenderInternalMenu("Software Update");
                                    Contents.DrawString(5, 33, "GoOS " + content, Resources.Font_2x,
                                        Color.Black);
                                    Contents.DrawString(5, 78, "A new version of GoOS is available.",
                                        Resources.Geneva, Color.Black);
                                    Contents.DrawString(5, 100,
                                        "To Update, go to \nhttps://github.com/Owen2k6/GoOS/", Resources.Geneva,
                                        Color.Black);
                                }
                                else if (content == Kernel.editionnext)
                                {
                                    if (Kernel.BuildType == "INTERNAL TEST BUILD")
                                    {
                                        RenderInternalMenu("Software Update");
                                        Contents.DrawString(5, 33, "GoOS " + content, Resources.Font_2x,
                                            Color.Black);
                                        Contents.DrawString(5, 78, "ITB Expired.", Resources.Geneva, Color.Black);
                                        Contents.DrawString(5, 100,
                                            "Check with Owen2k6 for ITB updates \nor return to GoOS Release.",
                                            Resources.Geneva, Color.Black);
                                    }

                                    RenderInternalMenu("Software Update");
                                    Contents.DrawString(5, 33, "GoOS " + content, Resources.Font_2x,
                                        Color.Black);
                                    Contents.DrawString(5, 78,
                                        "The next edition of GoOS is here!\nGoOS Update will no longer display \nupdates beyond this version.\n\nDon't worry, you don't have to update\nto receive continued support.\nWhile GoOS Update will no longer \ndisplay updates, this edition may \nstill receive updates.\nCheck https://github.com/Owen2k6/GoOS\nIf you wish to update your edition \nor version.",
                                        Resources.Geneva, Color.Black);
                                }
                                else if (content == "404")
                                {
                                    Kernel.ErrorSound();
                                    Dialogue.Show("GoOS Update", "Your Version of GoOS does not support GoOS Update.");
                                }
                                else
                                {
                                    RenderInternalMenu("Software Update");
                                    Contents.DrawString(5, 33, "GoOS " + content, Resources.Font_2x,
                                        Color.Black);
                                    Contents.DrawString(5, 78, "GoOS is up to date!", Resources.Geneva, Color.Black);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Kernel.ErrorSound();
                            Dialogue.Show("GoOS Update", "Failed to connect to Owen2k6 Api. Error: " + ex);
                        }

                        #endregion

                        #region GoOS Support Checker

                        try
                        {
                            using (var tcpClient = new TcpClient())
                            {
                                var dnsClient = new DnsClient();

                                // DNS
                                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                                dnsClient.SendAsk("api.goos.owen2k6.com");

                                // Address from IP
                                var address = dnsClient.Receive();
                                dnsClient.Close();
                                var serverIP = address.ToString();

                                tcpClient.Connect(serverIP, 80);
                                var stream = tcpClient.GetStream();
                                var httpget = "GET /GoOS/" + Kernel.edition + "-support.goos HTTP/1.1\r\n" +
                                              "User-Agent: GoOS\r\n" +
                                              "Accept: */*\r\n" +
                                              "Accept-Encoding: identity\r\n" +
                                              "Host: api.goos.owen2k6.com\r\n" +
                                              "Connection: Keep-Alive\r\n\r\n";
                                var dataToSend = Encoding.ASCII.GetBytes(httpget);
                                stream.Write(dataToSend, 0, dataToSend.Length);

                                // Receive data
                                var receivedData = new byte[tcpClient.ReceiveBufferSize];
                                var bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                                var receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                                var responseParts =
                                    receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                                if (responseParts.Length < 2 || responseParts.Length > 2)
                                    Dialogue.Show("GoOS Update", "Invalid HTTP response!", default,
                                        WindowManager.errorIcon);

                                var content = responseParts[1];

                                if (content == "true")
                                {
                                }
                                else if (content == "false")
                                {
                                    RenderInternalMenu("Software Update");
                                    Contents.DrawString(5, 33, "GoOS Support has ended.", Resources.Font_2x,
                                        Color.Black);
                                    Contents.DrawString(5, 78,
                                        "Your version of GoOS is no longer \nsupported. It's time to update if you \nwant to continue receiving \nsupport and updates.\n\nDon't fret, GoOS Services will \ncontinue to work, but you will \nno longer receive updates. \n Eventually, GoStore apps will \nno longer be compatible with \nthis version of GoOS.\n\nWe do recommend updating to the \nlatest version of GoOS. \n\nCheck https://github.com/Owen2k6/GoOS\nIf you wish to update your edition.",
                                        Resources.Geneva, Color.Black);
                                }
                                else
                                {
                                    if (Kernel.BuildType == "INTERNAL TEST BUILD")
                                    {
                                        RenderInternalMenu("Software Update");
                                        Contents.DrawString(5, 33, "GoOS " + content, Resources.Font_2x,
                                            Color.Black);
                                        Contents.DrawString(5, 78, "ITB Expired.", Resources.Geneva, Color.Black);
                                        Contents.DrawString(5, 100,
                                            "Check with Owen2k6 for ITB updates \nor return to GoOS Release.",
                                            Resources.Geneva, Color.Black);
                                    }

                                    RenderInternalMenu("Software Update");
                                    Contents.DrawString(5, 33, "Contact Support", Resources.Font_2x,
                                        Color.Black);
                                    Contents.DrawString(5, 78,
                                        "GoOS Authenticity could not be verified.\nPlease contact Owen2k6 for support.",
                                        Resources.Geneva, Color.Black);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Kernel.ErrorSound();
                            Dialogue.Show("GoOS Security", "Failed to connect to Owen2k6 Api. Error: " + ex);
                        }

                        #endregion
                    }
                };
                new Button(this, 109, 4 + 24 * 2, 185, 24, "Storage")
                {
                    Clicked = () =>
                    {
                        var ttype = "Bytes";
                        var tsize = Kernel.FS.GetTotalSize(@"0").ToString();
                        var tftype = "Bytes";
                        var tfsize = Kernel.FS.GetTotalFreeSpace(@"0").ToString();
                        RenderInternalMenu("Storage");
                        Contents.DrawString(5, 33, "Volume " + Kernel.FS.GetFileSystemLabel(@"0"), Resources.Font_2x,
                            Color.Black);
                        if (Kernel.FS.GetTotalSize(@"0") > 1000)
                        {
                            ttype = "KB";
                            tsize = (Kernel.FS.GetTotalSize(@"0") / 1000).ToString();
                        }

                        if (Kernel.FS.GetTotalSize(@"0") > 1000000)
                        {
                            ttype = "MB";
                            tsize = (Kernel.FS.GetTotalSize(@"0") / 1000000).ToString();
                        }

                        if (Kernel.FS.GetTotalSize(@"0") > 1000000000)
                        {
                            ttype = "GB";
                            tsize = (Kernel.FS.GetTotalSize(@"0") / 1000000000).ToString();
                        }

                        Contents.DrawString(5, 78, "Total Storage: " + tsize + ttype, Resources.Geneva, Color.Black);
                        if (Kernel.FS.GetTotalFreeSpace(@"0") > 1000)
                        {
                            tftype = "KB";
                            tfsize = (Kernel.FS.GetTotalFreeSpace(@"0") / 1000).ToString();
                        }

                        if (Kernel.FS.GetTotalFreeSpace(@"0") > 1000000)
                        {
                            tftype = "MB";
                            tfsize = (Kernel.FS.GetTotalFreeSpace(@"0") / 1000000).ToString();
                        }

                        if (Kernel.FS.GetTotalFreeSpace(@"0") > 1000000000)
                        {
                            tftype = "GB";
                            tfsize = (Kernel.FS.GetTotalFreeSpace(@"0") / 1000000000).ToString();
                        }

                        Contents.DrawString(5, 100, "Free Storage: " + tfsize + tftype, Resources.Geneva, Color.Black);
                        Contents.DrawString(5, 122, "Format: " + Kernel.FS.GetFileSystemType(@"0"), Resources.Geneva,
                            Color.Black);
                        Contents.DrawString(5, 144, "Validation: " + Kernel.FS.IsValidDriveId(@"0"),
                            Resources.Geneva, Color.Black);
                    }
                };
                new Button(this, 109, 4 + 24 * 4, 185, 24, "Language and Locale")
                {
                    Clicked = () =>
                    {
                        Dialogue.Show("GoOS Kernel Incompatible",
                            "This feature is planned but at this time, the GoOS Kernel is not able to parse language data.");
                    }
                };
                new Button(this, 109, 4 + 24 * 5, 185, 24, "Keyboard Layout")
                {
                    Clicked = () =>
                    {
                        RenderInternalMenu("Keyboard Layout");
                        new Button(this, 5, 52, 298, 24, "English US (104USQWERTY-US-1.0)")
                        {
                            Clicked = () => { KeyboardManager.SetKeyLayout(new USStandardLayout()); }
                        };
                        new Button(this, 5, 76, 298, 24, "English UK (105GBQWERTY-GB-1.0)")
                        {
                            Clicked = () => { KeyboardManager.SetKeyLayout(new GBStandardLayout()); }
                        };
                        new Button(this, 5, 100, 298, 24, "Spanish (105ESQWERTY-ES-1.0)")
                        {
                            Clicked = () => { KeyboardManager.SetKeyLayout(new ESStandardLayout()); }
                        };
                        new Button(this, 5, 124, 298, 24, "French (105FRQWERTY-FR-1.0)")
                        {
                            Clicked = () => { KeyboardManager.SetKeyLayout(new FRStandardLayout()); }
                        };
                        new Button(this, 5, 148, 298, 24, "German (105DEQWERTY-DE-1.0)")
                        {
                            Clicked = () => { KeyboardManager.SetKeyLayout(new DEStandardLayout()); }
                        };
                        new Button(this, 5, 172, 298, 24, "Turkish (105TRQWERTY-TR-1.0)")
                        {
                            Clicked = () => { KeyboardManager.SetKeyLayout(new TRStandardLayout()); }
                        };
                        Contents.DrawString(5, 33, "Setting does not persist post reboot.", Resources.Geneva,
                            Color.Red);
                        foreach (var control in Controls) control.Render();
                    }
                };
                new Button(this, 109, 4 + 24 * 7, 185, 24, "Reset")
                {
                    Clicked = () => { Dialogue.Show("Unimplemented", "Work In Progress"); }
                };
                break;
            case 1:
                Contents.DrawString(109, 11,
                    "Current: " + WindowManager.Canvas.Width + "x" + WindowManager.Canvas.Height,
                    Resources.Charcoal,
                    Color.Black);
                new Button(this, 109, 28, 185, 24, "Change Resolution")
                {
                    Clicked = () =>
                    {
                        RenderInternalMenu("Change Resolution");
                        new Button(this, 5, 52, 298, 24, "800 x 600")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    800, 600);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 0 });
                            }
                        };
                        new Button(this, 5, 76, 298, 24, "1024 x 768")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1024, 768);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 1 });
                            }
                        };
                        new Button(this, 5, 100, 298, 24, "1280 x 960")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1280, 960);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 2 });
                            }
                        };
                        new Button(this, 5, 124, 298, 24, "1400 x 1050")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1400, 1050);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 3 });
                            }
                        };
                        new Button(this, 5, 148, 298, 24, "1600 x 1200")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1600, 1200);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 4 });
                            }
                        };
                        new Button(this, 5, 172, 298, 24, "1280 x 720")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1280, 720);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 5 });
                            }
                        };
                        new Button(this, 5, 196, 298, 24, "1280 x 800")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1280, 800);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 6 });
                            }
                        };
                        new Button(this, 5, 220, 298, 24, "1366 x 768")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1366, 768);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 7 });
                            }
                        };
                        new Button(this, 5, 244, 298, 24, "1440 x 900")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1440, 900);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 8 });
                            }
                        };
                        new Button(this, 5, 268, 298, 24, "1600 x 900")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1600, 900);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 9 });
                            }
                        };
                        new Button(this, 5, 292, 298, 24, "1680 x 1050")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1680, 1050);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 10 });
                            }
                        };
                        new Button(this, 5, 316, 298, 24, "1920 x 1080")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1920, 1080);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 11 });
                            }
                        };
                        new Button(this, 5, 340, 298, 24, "1920 x 1200")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    1920, 1200);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 12 });
                            }
                        };
                        new Button(this, 5, 364, 298, 24, "2560 x 1440")
                        {
                            Clicked = () =>
                            {
                                
                                WindowManager.Canvas = Display.GetDisplay(
                                    2560, 1440);
                                WindowManager.Update();
                                WindowManager.windows = new List<Window>(10);
                                
                                WindowManager.AddWindow(new Desktop());
                                WindowManager.AddWindow(new Menubar());
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                                    new byte[] { 13 });
                            }
                        };
                        foreach (var control in Controls) control.Render();
                    }
                };

                break;
        }
    }

    public void DrawSideBar(string name, int ID, Action clickedAction)
    {
        if (page == ID)
        {
            Contents.DrawImage(0, (ushort)(sButtons.Count * buttonHeight), Resources.SBGBS, false);
            sButtons.Add(new Button(this, 0,
                (ushort)(sButtons.Count * buttonHeight), 96, buttonHeight, name)
            {
                Clicked = clickedAction,
                AppearPressed = true,
                UseSystemStyle = false,
                RenderWithAlpha = true,
                BackgroundColour = new Color(0, 0, 0, 0)
                //Image = Resources.SBGBS
            });
        }
        else
        {
            sButtons.Add(new Button(this, 0,
                (ushort)(sButtons.Count * buttonHeight), 96, buttonHeight, name)
            {
                Clicked = clickedAction,
                RenderWithAlpha = true,
                UseSystemStyle = false,
                BackgroundColour = new Color(0, 0, 0, 0)
            });
        }
    }

    private void RenderInternalMenu(string Name = "Untitled Settings Pane")
    {
        Contents.Clear();
        Controls.Clear();
        Contents.DrawImage(0, 0, Resources.SBGM, false);
        new Button(this, 2, 2, 24, 24, "<")
        {
            Clicked = () => { ReDraw(); },
            RenderWithAlpha = true,
            UseSystemStyle = false,
            BackgroundColour = new Color(0, 0, 0, 0),
            Image = Resources.SBBB
        };
        Contents.DrawString(33, 6, Name, Resources.Charcoal, Color.Black);
        foreach (var control in Controls) control.Render();
    }

    private void AddSideButtons()
    {
        DrawSideBar("General", 0, () =>
        {
            page = 0;
            ReDraw();
        });
        DrawSideBar("Display", 1, () =>
        {
            page = 1;
            ReDraw();
        });
    }
}