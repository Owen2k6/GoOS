using System;
using System.Collections;
using System.IO;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using TcpClient = Cosmos.System.Network.IPv4.TCP.TcpClient;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GoOS.Themes;
using PrismAPI.Graphics;
using static GoOS.Resources;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class MainFrame : Window
    {
        Button[] catagoryButtons = new Button[7];
        
        readonly List<(string, int)> Catagories = new()
        {
            ("Utilities", 0),
            ("Games", 1),
            ("Demos", 2),
            ("Development", 3),
            ("Updates", 4),
            ("Office", 5),
            ("Production", 6)
        };
        
        Button[] RepoFilesButtons;
        
        List<(string, string, string, string, string, string, string)> repoFiles =
            new ();

        private Dictionary<string, string> GoOSversions = new ();
        
        readonly string[] repos =
        {
            "apps.goos.owen2k6.com",
            //"goos.ekeleze.org",
            //"repo.mobren.net"
        };

        public MainFrame()
        {
            try
            {
                // Get the file list from every repo
                string[][] infoFiles =
                {
                    GetInfoFile(repos[0]).Split('\n'),
                    //GetInfoFile(repos[1]).Split('\n'),
                    //GetInfoFile(repos[2]).Split('\n'),
                };

                int o = 0;
                foreach (string[] file in infoFiles)
                {
                    foreach (string program in file)
                    {
                        string[] metadata = program.Split('|');
                        repoFiles.Add((metadata[0], metadata[1], metadata[2], metadata[3], metadata[4], metadata[5], repos[o]));
                        if (GoOSversions.ContainsKey(metadata[0]))
                            GoOSversions.Remove(metadata[0]);
                        GoOSversions.Add(metadata[0], metadata[6]);
                    }

                    o++;
                }

                // Generate the fonts.
                Generate(ResourceType.Fonts);

                // Create the window.
                Contents = new Canvas(800, 600);
                Title = "GoStore";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize the controls.
                RepoFilesButtons = new Button[repoFiles.Count];
                bool GS = HasLaunched();
                if (GS || Kernel.BuildType == "NIFPR")
                {
                    for (int i = 0; i < Catagories.Count; i++)
                    {
                        catagoryButtons[i] = new Button(this, Convert.ToUInt16(5),
                            Convert.ToUInt16(45 + i * 20),
                            Convert.ToUInt16(Catagories[i].Item1.Length * 8), 20, Catagories[i].Item1)
                        {
                            Name = Catagories[1].Item1,
                            UseSystemStyle = false,
                            BackgroundColour = new Color(0, 0, 0, 0),
                            ClickedAlt = repoFiles_Click,
                            RenderWithAlpha = true
                        };
                    }
                    
                    // Paint the window.
                    Contents.DrawImage(0, 0, Resources.GoStore, false);
                    
                    for (int i = 0; i < repoFiles.Count; i++)
                    { // 207 x 78

                        string GoOSversion = "1.5";
                        string VersionSpaces = "";

                        if (GoOSversions.ContainsKey(repoFiles[i].Item1))
                            GoOSversions.TryGetValue(repoFiles[i].Item1, out GoOSversion);
                        
                        for (int ii = 0; ii < 25 - repoFiles[i].Item1.Length - (GoOSversion.TrimEnd().Length + 6); ii++)
                            VersionSpaces += " ";
                        
                        Contents.DrawImage(150, 45 + i * (78 + 5), StoreButton);
                        
                        RepoFilesButtons[i] = new Button(this, Convert.ToUInt16(150),
                            Convert.ToUInt16(45 + i * (78 + 5)),
                            207, 78, repoFiles[i].Item1 + VersionSpaces + "GoOS " + 
                                     GoOSversion.TrimEnd() + "+" + "\nBy " + repoFiles[i].Item5 + "\n" + 
                                     repoFiles[i].Item3)
                        {
                            Name = repoFiles[i].Item1,
                            UseSystemStyle = false,
                            BackgroundColour = new Color(0, 0, 0, 0),
                            ClickedAlt = repoFiles_Click,
                            RenderWithAlpha = true,
                            CenterTitle = false,
                            textX = 5,
                            textY = 2
                        };
                    }
                    
                    RenderSystemStyleBorder();
                    foreach (Button i in RepoFilesButtons) i.Render();
                    foreach (Button i in catagoryButtons) i.Render();
                }
                else
                {
                    Contents.DrawImage(0, 0, Resources.GoStore, false);
                    RenderSystemStyleBorder();
                    Contents.DrawString(150, 50, "The GoStore is coming soon...\n\nThis will allow users to install applications\ndirectly to their device!\nWe are working on preparing the GoStore\nand getting apps made for you to enjoy!\n\nSee you on launch day!",Font_1x, Color.White);
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("Error", ex.ToString());
            }
        }

        private string GetInfoFile(string repo)
        {
            try
            {
                var dnsClient = new DnsClient();
                var tcpClient = new TcpClient();

                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                dnsClient.SendAsk(repo);
                Address address = dnsClient.Receive();
                dnsClient.Close();

                tcpClient.Connect(address, 80);

                string httpget = "GET /" + "info.glist" + " HTTP/1.1\r\n" +
                                 "User-Agent: GoOS\r\n" +
                                 "Accept: */*\r\n" +
                                 "Accept-Encoding: identity\r\n" +
                                 "Host: " + repo + "\r\n" +
                                 "Connection: Keep-Alive\r\n\r\n";

                tcpClient.Send(Encoding.ASCII.GetBytes(httpget));

                var ep = new EndPoint(Address.Zero, 0);
                var data = tcpClient.Receive(ref ep);
                tcpClient.Close();

                string httpresponse = Encoding.ASCII.GetString(data);

                string[] responseParts =
                    httpresponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                if (responseParts.Length == 2)
                {
                    return responseParts[1];
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        private Boolean HasLaunched()
        {
            try
            {
                var dnsClient = new DnsClient();
                var tcpClient = new TcpClient();

                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                dnsClient.SendAsk("apps.goos.owen2k6.com");
                Address address = dnsClient.Receive();
                dnsClient.Close();

                tcpClient.Connect(address, 80);

                string httpget = "GET /GoOS/gslaunch.goos HTTP/1.1\r\n" +
                                 "User-Agent: GoOS\r\n" +
                                 "Accept: */*\r\n" +
                                 "Accept-Encoding: identity\r\n" +
                                 "Host: api.owen2k6.com\r\n" +
                                 "Connection: Keep-Alive\r\n\r\n";

                tcpClient.Send(Encoding.ASCII.GetBytes(httpget));

                var ep = new EndPoint(Address.Zero, 0);
                var data = tcpClient.Receive(ref ep);
                tcpClient.Close();

                string httpresponse = Encoding.ASCII.GetString(data);

                string[] responseParts =
                    httpresponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                if (responseParts.Length == 2)
                {
                    if (responseParts[1] == "true")
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        private int GetIndexByTitle(string title)
        {
            for (int i = 0; i < repoFiles.Count; i++)
            {
                if (repoFiles[i].Item1 == title) return i;
            }

            return 0;
        }

        private void repoFiles_Click(string i)
        {
            WindowManager.AddWindow(new DescriptionFrame(
                repoFiles[GetIndexByTitle(i)].Item1, repoFiles[GetIndexByTitle(i)].Item4,
                repoFiles[GetIndexByTitle(i)].Item5, repoFiles[GetIndexByTitle(i)].Item3,
                repoFiles[GetIndexByTitle(i)].Item2, repoFiles[GetIndexByTitle(i)].Item6));
        }
    }
}