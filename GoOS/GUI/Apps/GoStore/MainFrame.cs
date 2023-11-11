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
using System.Runtime.CompilerServices;
using GoOS.Themes;
using PrismAPI.Graphics;
using static GoOS.Resources;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class MainFrame : Window
    {
        Button[] catagoryButtons = new Button[7];

        readonly List<string> Catagories = new()
        {
            "Utilities",
            "Games",
            "Demos",
            "Development",
            "Updates",
            "Office",
            "Production",
        };

        Button[] RepoFilesButtons;
        private int[] RepoFilesButtonsPageNumbers;

        private int catagory = 0;
        private int page = 0;

        List<(string, string, string, string, string, string, string)> repoFiles =
            new();

        private Dictionary<string, string> GoOSversions = new();

        private Button nextButton;
        private Button prevousButton;

        readonly string[] repos =
        {
            "apps.goos.owen2k6.com",
            "goos.ekeleze.org",
            "repo.mobren.net"
        };

        private readonly string[] allowDLfrom =
        {
            "1.5pre1",
            "1.5pre2",
            "1.5pre3",
            "1.5rc1",
            "1.5rc2",
            "1.5rc3",
            "1.5"
        };

        public MainFrame()
        {
            // Get the file list from every repo
            string[][] infoFiles =
            {
                GetInfoFile(repos[0]).Split('\n'),
                GetInfoFile(repos[1]).Split('\n'),
                GetInfoFile(repos[2]).Split('\n')
            };
            try
            {
                int o = 0;
                foreach (string[] file in infoFiles)
                {
                    try
                    {
                        foreach (string program in file)
                        {
                            string[] metadata = program.Split('|');
                            repoFiles.Add((metadata[0], metadata[1], metadata[2], metadata[3], metadata[4], metadata[5],
                                repos[o]));
                            if (GoOSversions.ContainsKey(metadata[0]))
                                GoOSversions.Remove(metadata[0]);
                            GoOSversions.Add(metadata[0], metadata[6]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Dialogue.Show("GoStore had an issue.", ex.ToString());
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
                RepoFilesButtonsPageNumbers = new int[repoFiles.Count];
                bool GS = HasLaunched();
                if (GS || Kernel.BuildType == "NIFPR")
                {
                    for (int i = 0; i < Catagories.Count; i++)
                    {
                        catagoryButtons[i] = new Button(this, Convert.ToUInt16(5),
                            Convert.ToUInt16(45 + i * 20),
                            Convert.ToUInt16(Catagories[i].Length * 8), 20, Catagories[i])
                        {
                            Name = Catagories[i],
                            UseSystemStyle = false,
                            BackgroundColour = new Color(0, 0, 0, 0),
                            ClickedAlt = CatgoryAction,
                            RenderWithAlpha = true
                        };
                    }
                    
                    nextButton = new Button(this, 685, 556, 109, 35, "Next")
                    {
                        Name = "next",
                        UseSystemStyle = false,
                        BackgroundColour = new Color(0, 0, 0, 0),
                        ClickedAlt = nextPage,
                        RenderWithAlpha = true
                    };
                    
                    prevousButton = new Button(this, 514, 556, 109, 35, "Previous")
                    {
                        Name = "Previous",
                        UseSystemStyle = false,
                        BackgroundColour = new Color(0, 0, 0, 0),
                        ClickedAlt = previousPage,
                        RenderWithAlpha = true
                    };
                    
                    CatgoryAction("Utilities");
                }
                else
                {
                    Contents.DrawImage(0, 0, Resources.GoStore, false);
                    RenderSystemStyleBorder();
                    Contents.DrawString(150, 50,
                        "The GoStore is coming soon...\n\nThis will allow users to install applications\ndirectly to their device!\nWe are working on preparing the GoStore\nand getting apps made for you to enjoy!\n\nSee you on launch day!",
                        Font_1x, Color.White);
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("Error", ex.ToString());
            }
        }

        private void dostuff()
        {
            foreach (Button b in RepoFilesButtons)
                Controls.Remove(b);
            
            RepoFilesButtons = null;
            RepoFilesButtonsPageNumbers = null;

            Contents.Clear();
            Contents.DrawImage(0, 0, Resources.GoStore, false);

            int accountFor = 0;

            RepoFilesButtonsPageNumbers = new int[repoFiles.Count];
            RepoFilesButtons = new Button[repoFiles.Count];

            int Line = 0;
            int Colum = 0;
            int ButtonPage = 0;
            page = 0;

            int yOffset = 0;

            for (int i = 0; i < repoFiles.Count; i++)
            {
                // 207 x 78

                int appCat = 0;

                switch (repoFiles[i].Item6)
                {
                    case "Utilities":
                        appCat = 0;
                        break;
                    case "Games":
                        appCat = 1;
                        break;
                    case "Demos":
                        appCat = 2;
                        break;
                    case "Development":
                        appCat = 3;
                        break;
                    case "Updates":
                        appCat = 4;
                        break;
                    case "Office":
                        appCat = 5;
                        break;
                    case "Production":
                        appCat = 6;
                        break;
                }

                if (Colum >= 3 && Line >= 6 && appCat == catagory)
                {
                    ButtonPage++;
                }
                else if (Line >= 6 && appCat == catagory)
                {
                    Line = 0;
                    Colum++;
                }

                if (appCat == catagory)
                {
                    string GoOSversion = "1.5";
                    string VersionSpaces = "";

                    if (GoOSversions.ContainsKey(repoFiles[i].Item1))
                        GoOSversions.TryGetValue(repoFiles[i].Item1, out GoOSversion);

                    for (int ii = 0; ii < 25 - repoFiles[i].Item1.Length - (GoOSversion.TrimEnd().Length + 6); ii++)
                        VersionSpaces += " ";

                    int x = 150 + Colum * (207 + 5); // 150
                    int y = 45 + (Line) * (78 + 5);

                    RepoFilesButtons[i] = new Button(this, Convert.ToUInt16(x), // 150 
                        Convert.ToUInt16(y),
                        207, 78, repoFiles[i].Item1 + VersionSpaces + "GoOS " +
                                 GoOSversion.TrimEnd() + "+" + "\nBy " + repoFiles[i].Item5 + "\n" +
                                 repoFiles[i].Item3.Replace(@"\n", "\n"))
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

                    Line++;

                    RepoFilesButtonsPageNumbers[i] = ButtonPage;
                }
            }

            RenderSystemStyleBorder();
            int a = 0;
            foreach (Button i in RepoFilesButtons)
            {
                if (page == RepoFilesButtonsPageNumbers[a])
                {
                    int x = i.X;
                    int y = i.Y;
                    Contents.DrawImage(x, y, StoreButton);
                    if (i != null) i.Render();
                }

                a++;
            }
            
            foreach (Button i in catagoryButtons) i.Render();
            
            // y 556
            
            // x 514
            
            // Width 109 x Height 35
            
            prevousButton.Render();
            nextButton.Render();
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

                string httpget = "GET /info.glist HTTP/1.1\r\n" +
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
            catch (Exception ex)
            {
                Dialogue.Show("GoStore Was unable to connect you.",
                    "Some apps may not be available as some servers were uncontactable. \nError: " + ex +
                    "\n\nPlease try again later.");
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

        private void CatgoryAction(string name)
        {
            switch (name)
            {
                case "Utilities":
                    catagory = 0;
                    break;
                case "Games":
                    catagory = 1;
                    break;
                case "Demos":
                    catagory = 2;
                    break;
                case "Development":
                    catagory = 3;
                    break;
                case "Updates":
                    catagory = 4;
                    break;
                case "Office":
                    catagory = 5;
                    break;
                case "Production":
                    catagory = 6;
                    break;
            }

            dostuff(); // 565
            
            switch (name)
            {
                case "Utilities":
                    Contents.DrawString(38, 563, "Utilities", Font_1x, Color.White);
                    break;
                case "Games":
                    Contents.DrawString(52, 563, "Games", Font_1x, Color.White);
                    break;
                case "Demos":
                    Contents.DrawString(52, 563, "Demos", Font_1x, Color.White);
                    break;
                case "Development":
                    Contents.DrawString(29, 563, "Development", Font_1x, Color.White);
                    break;
                case "Updates":
                    Contents.DrawString(43, 563, "Updates", Font_1x, Color.White);
                    break;
                case "Office":
                    Contents.DrawString(47, 563, "Office", Font_1x, Color.White);
                    break;
                case "Production":
                    Contents.DrawString(33, 563, "Production", Font_1x, Color.White);
                    break;
            }

            int pagex = 646 + 4;
            if ((page + 1).ToString().Length == 2)
            {
                pagex -= 4;
            }
            
            if ((page + 1).ToString().Length == 3)
            {
                pagex -= 8;
            }
            
            Contents.DrawString(pagex, 563, (page + 1).ToString(), Font_1x, Color.White);
        }

        private void nextPage(string useless)
        {
            if (RepoFilesButtonsPageNumbers.Contains(page + 1))
            {
                page++;
                dostuff();
                int pagex = 646 + 4;
                if ((page + 1).ToString().Length == 2)
                {
                    pagex -= 4;
                }
                
                if ((page + 1).ToString().Length == 3)
                {
                    pagex -= 8;
                }
            
                Contents.DrawString(pagex, 563, (page + 1).ToString(), Font_1x, Color.White);
            }
        }

        private void previousPage(string useless)
        {
            if (RepoFilesButtonsPageNumbers.Contains(page - 1))
            {
                page--;
                dostuff();
                int pagex = 646 + 4;
                if ((page + 1).ToString().Length == 2)
                {
                    pagex -= 4;
                }

                if ((page + 1).ToString().Length == 3)
                {
                    pagex -= 8;
                }

                Contents.DrawString(pagex, 563, (page + 1).ToString(), Font_1x, Color.White);
            }
        }
    }
}