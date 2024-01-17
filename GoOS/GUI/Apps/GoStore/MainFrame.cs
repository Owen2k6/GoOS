using System;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using PrismAPI.Graphics;
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
        /*
         * == RC Release Notice.
         * TODO: This is bad practice as the list is stored on the client so as more categories are added the client will be unable to display apps from them.
         * TODO: Prevent Release Candidate or Release builds until this is resolved.
         * - Owen2k6 DO NOT REMOVE.
         */

        Button[] _repoFilesButtons;
        private int[] _repoFilesButtonsPageNumbers;

        private int catagory = 0;
        private int page = 0;

        private List<Application> _repoFiles;

        private Dictionary<string, string> GoOSversions = new();

        private Button nextButton;
        private Button prevousButton;

        readonly string[] repos =
        {
            "apps.goos.owen2k6.com",
            //"goos.ekeleze.org",
            //"repo.mobren.net"
        };

        private readonly string[] _allowDLFrom =
        {
            "1.5pre1",
            "1.5pre2",
            "1.5pre3",
            "1.5rc1",
            "1.5rc2",
            "1.5rc3",
            "1.5"
        };
        /*
         * == RC Release Notice.
         * TODO: This needs to be reset to just 1.5 before release.
         * TODO: Prevent Release Candidate or Release builds until this is resolved.
         * - Owen2k6 DO NOT REMOVE.
         */

        public MainFrame()
        {
            try
            {
                // Get the info file from every repo
                string[] repos = GetReposFile();
                Infofile[] infoFiles = new Infofile[repos.Length];
                _repoFiles = new List<Application>();

                for (int i = 0; i < repos.Length; i++)
                {
                    infoFiles[i] = new Infofile(GetInfoFile(repos[i]).Split('\n'), repos[i]);
                }

                foreach (Infofile file in infoFiles)
                {
                    foreach (string program in file.Contents)
                    {
                        Application app = new Application(program.Split('|'), file.URL);
                        app.Downloadable = _allowDLFrom.Contains(app.GoOSVersion);

                        _repoFiles.Add(app);
                    }
                }

                // Create the window.
                Contents = new Canvas(800, 600);
                Title = "GoStore";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize the controls.
                _repoFilesButtons = new Button[_repoFiles.Count];
                _repoFilesButtonsPageNumbers = new int[_repoFiles.Count];

                if (HasLaunched() || Kernel.BuildType == "NIFPR")
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
                        Clicked = nextPage,
                        RenderWithAlpha = true
                    };

                    prevousButton = new Button(this, 514, 556, 109, 35, "Previous")
                    {
                        Name = "Previous",
                        UseSystemStyle = false,
                        BackgroundColour = new Color(0, 0, 0, 0),
                        Clicked = previousPage,
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
            catch (Exception e) { ShowCrashDialogue(e); }
        }

        private void RenderApplicationsList()
        {
            foreach (Button b in _repoFilesButtons)
                Controls.Remove(b);
            
            _repoFilesButtons = null;
            _repoFilesButtonsPageNumbers = null;

            Contents.Clear();
            Contents.DrawImage(0, 0, Resources.GoStore, false);

            int accountFor = 0;

            _repoFilesButtonsPageNumbers = new int[_repoFiles.Count];
            _repoFilesButtons = new Button[_repoFiles.Count];

            int Line = 0;
            int Colum = 0;
            int ButtonPage = 0;
            page = 0;

            int yOffset = 0;

            for (int i = 0; i < _repoFiles.Count; i++)
            {
                // 207 x 78

                int appCat = 0;

                switch (_repoFiles[i].Category)
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
                    
                    /*
                     * == RC Release Notice.
                     * TODO: This is bad practice as the list is stored on the client so as more categories are added the client will be unable to display apps from them.
                     * TODO: Prevent Release Candidate or Release builds until this is resolved.
                     * - Owen2k6 DO NOT REMOVE.
                     */
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

                    for (int ii = 0; ii < 25 - _repoFiles[i].Name.Length - (GoOSversion.TrimEnd().Length + 6); ii++)
                        VersionSpaces += " ";

                    int x = 150 + Colum * (207 + 5); // 150
                    int y = 45 + (Line) * (78 + 5);

                    _repoFilesButtons[i] = new Button(this, Convert.ToUInt16(x), // 150 
                        Convert.ToUInt16(y),
                        207, 78, _repoFiles[i].Name + VersionSpaces + "GoOS " +
                                 GoOSversion.TrimEnd() + "+" + "\nBy " + _repoFiles[i].Author + "\n" +
                                 _repoFiles[i].Version.Replace(@"\n", "\n"))
                    {
                        Name = _repoFiles[i].Name,
                        UseSystemStyle = false,
                        BackgroundColour = new Color(0, 0, 0, 0),
                        ClickedAlt = _repoFiles_Click,
                        RenderWithAlpha = true,
                        CenterTitle = false,
                        textX = 5,
                        textY = 2
                    };

                    Line++;

                    _repoFilesButtonsPageNumbers[i] = ButtonPage;
                }
            }

            for (int i = 0; i < _repoFilesButtons.Length; i++)
            {
                if (page == _repoFilesButtonsPageNumbers[i])
                {
                    int x = _repoFilesButtons[i].X;
                    int y = _repoFilesButtons[i].Y;
                    Contents.DrawImage(x, y, StoreButton);
                    if (_repoFilesButtons[i] != null) _repoFilesButtons[i].Render();
                }
            }
            
            foreach (Button i in catagoryButtons) i.Render();
            prevousButton.Render();
            nextButton.Render();

            RenderSystemStyleBorder();
        }

        private string GetInfoFile(string repo)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var dnsClient = new DnsClient();

                    // DNS
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk(repo);

                    // Address from IP
                    Address address = dnsClient.Receive();
                    dnsClient.Close();
                    string serverIP = address.ToString();

                    tcpClient.Connect(serverIP, 80);
                    NetworkStream stream = tcpClient.GetStream();
                    string httpget = "GET /info.glist HTTP/1.1\r\n" +
                                     "User-Agent: GoOS\r\n" +
                                     "Accept: */*\r\n" +
                                     "Accept-Encoding: identity\r\n" +
                                     "Host: " + repo + "\r\n" +
                                     "Connection: Keep-Alive\r\n\r\n";
                    byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    // Receive data
                    byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                    string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                    string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                    if (responseParts.Length < 2 || responseParts.Length > 2) Dialogue.Show("GoStore", "Invalid HTTP response!", default, WindowManager.errorIcon);

                    return responseParts[1];
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("Failed to contact servers - GoStore",
                    "Some apps may not be available as some servers were uncontactable\n"
                    + ex + "\n\nPlease try again later");

                return string.Empty;
            }
        }

        private string[] GetReposFile()
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var dnsClient = new DnsClient();

                    // DNS
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk("api.goos.owen2k6.com");

                    // Address from IP
                    Address address = dnsClient.Receive();
                    dnsClient.Close();
                    string serverIP = address.ToString();

                    tcpClient.Connect(serverIP, 80);
                    NetworkStream stream = tcpClient.GetStream();
                    string httpget = "GET /GoOS/repos.gostore HTTP/1.1\r\n" +
                                     "User-Agent: GoOS\r\n" +
                                     "Accept: */*\r\n" +
                                     "Accept-Encoding: identity\r\n" +
                                     "Host: api.goos.owen2k6.com\r\n" +
                                     "Connection: Keep-Alive\r\n\r\n";
                    byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    // Receive data
                    byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                    string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                    string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                    if (responseParts.Length < 2 || responseParts.Length > 2) Dialogue.Show("GoStore", "Invalid HTTP response!", default, WindowManager.errorIcon);

                    return responseParts[1].Split('\n');
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("Failed to contact servers - GoStore",
                    "Some apps may not be available as some servers were uncontactable\n"
                    + ex + "\n\nPlease try again later");

                return Array.Empty<string>();
            }
        }

        private bool HasLaunched()
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var dnsClient = new DnsClient();

                    // DNS
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk("apps.goos.owen2k6.com");

                    // Address from IP
                    Address address = dnsClient.Receive();
                    dnsClient.Close();
                    string serverIP = address.ToString();

                    tcpClient.Connect(serverIP, 80);
                    NetworkStream stream = tcpClient.GetStream();
                    string httpget = "GET /GoOS/gslaunch.goos HTTP/1.1\r\n" + 
                                     "User-Agent: GoOS\r\n" +
                                     "Accept: */*\r\n" +
                                     "Accept-Encoding: identity\r\n" +
                                     "Host: api.goos.owen2k6.com\r\n" +
                                     "Connection: Keep-Alive\r\n\r\n";
                    byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    // Receive data
                    byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                    string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                    string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                    if (responseParts.Length < 2 || responseParts.Length > 2) Dialogue.Show("GoStore", "Invalid HTTP response!", default, WindowManager.errorIcon);

                    return responseParts[1] == "true";
                }
            }
            catch { return false; }
        }

        private int GetIndexByTitle(string title)
        {
            for (int i = 0; i < _repoFiles.Count; i++)
                if (_repoFiles[i].Name == title) return i;
            return 0;
        }

        private void _repoFiles_Click(string i)
        {
            WindowManager.AddWindow(new DescriptionFrame(_repoFiles[GetIndexByTitle(i)]));
            
            /*
             * == RC Release Notice.
             * TODO: This is bad practice. The repository is stored on the end of the metadata. We cant expand the GoStore abilities without breaking earlier clients.
             * TODO: Assigned to @MrDumbrava as they created this.
             * TODO: Prevent Release Candidate or Release builds until this is resolved.
             * - Owen2k6 DO NOT REMOVE.
             */
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
                
                /*
                 * == RC Release Notice.
                 * TODO: This is bad practice as the list is stored on the client so as more categories are added the client will be unable to display apps from them.
                 * TODO: Prevent Release Candidate or Release builds until this is resolved.
                 * - Owen2k6 DO NOT REMOVE.
                 */
            }

            RenderApplicationsList();
            
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
                
                /*
                 * == RC Release Notice.
                 * TODO: This is bad practice as the list is stored on the client so as more categories are added the client will be unable to display apps from them.
                 * TODO: Prevent Release Candidate or Release builds until this is resolved.
                 * - Owen2k6 DO NOT REMOVE.
                 */
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

        private void nextPage()
        {
            if (_repoFilesButtonsPageNumbers.Contains(page + 1))
            {
                page++;
                RenderApplicationsList();
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

        private void previousPage()
        {
            if (_repoFilesButtonsPageNumbers.Contains(page - 1))
            {
                page--;
                RenderApplicationsList();
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