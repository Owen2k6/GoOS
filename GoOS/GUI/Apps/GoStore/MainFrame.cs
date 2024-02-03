using System;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class MainFrame : Window
    {
        Button[] catagoryButtons;
        Button[] _repoFilesButtons;
        string[] Catagories;

        private int catagory = 0;
        private int page = 0;

        private List<Application> _repoFiles;

        private Button nextButton;
        private Button prevousButton;

        public static readonly List<string> AllowDLFrom = new List<string>
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

        private Canvas _infoBoard;
        private string _infoBoardText;

        public MainFrame()
        {
            try
            {
                // Get the info file from every repo
                string[] repos = GetReposFile();
                Infofile[] infoFiles = new Infofile[repos.Length];
                _repoFiles = new List<Application>();
                Catagories = GetCatagoriesFile();
                catagoryButtons = new Button[Catagories.Length];

                for (int i = 0; i < repos.Length; i++)
                {
                    infoFiles[i] = new Infofile(GetInfoFile(repos[i]).Split('\n'), repos[i]);
                }

                foreach (Infofile file in infoFiles)
                {
                    foreach (string program in file.Contents)
                    {
                        Application app = new Application(program.Split('|'), file.URL);
                        app.Downloadable = AllowDLFrom.Contains(app.GoOSVersion);

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
                _infoBoard = new Canvas(424, 16);
                _infoBoardText = GetInfoBoardFile();
                textX = 424;

                if (HasLaunched() || Kernel.BuildType == "NIFPR")
                {
                    for (int i = 0; i < Catagories.Length; i++)
                    {
                        catagoryButtons[i] = new Button(this, Convert.ToUInt16(5),
                            Convert.ToUInt16(45 + i * 20),
                            Convert.ToUInt16(Catagories[i].Length * 8), 20, Catagories[i].Trim())
                        {
                            Name = Catagories[i].Trim(),
                            UseSystemStyle = false,
                            BackgroundColour = new Color(0, 0, 0, 0),
                            ClickedAlt = CategoryButtonClick,
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

                    Render(Catagories[0]);
                }
                else
                {
                    Contents.DrawImage(0, 0, Resources.GoStoreSoon, false);
                    long unixTime = Convert.ToInt64(1707955200); //1707955200
                    DateTime targetDate = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                    DateTime currentDate = DateTime.UtcNow;

                    TimeSpan difference = targetDate - currentDate;
                    int daysUntil = (int)difference.TotalDays;
                    Contents.DrawString(290, 467, daysUntil+" Days left", Resources.Font_2x, Color.Green);
                    RenderSystemStyleBorder();
                }
            }
            catch (Exception e) { ShowCrashDialogue(e); }
        }

        private void CategoryButtonClick(string cat)
        {
            page = 0;
            Render(cat);
        }

        private int GetCatagoryIndex(string cat)
        {
            for (int i = 0; i < Catagories.Length; i++)
            {
                if (Catagories[i].Trim() == cat)
                {
                    return i;
                }
            }

            return -1;
        }

        private int textX;

        public override void HandleRun()
        {
            base.HandleRun();

            _infoBoard.DrawImage(0, 0, GoStoreinfoboard, false);
            _infoBoard.DrawString(textX, 0, _infoBoardText, Font_1x, Color.Yellow);
            Contents.DrawImage(359, 9, _infoBoard, false);

            if (textX < -Font_1x.MeasureString(_infoBoardText)) textX = 424;

            textX--;
        }

        private void Render(string category)
        {
            catagory = GetCatagoryIndex(category);

            foreach (Button b in _repoFilesButtons)
                Controls.Remove(b);

            _repoFilesButtons = null;

            Contents.Clear();
            Contents.DrawImage(0, 0, Resources.GoStore, false);

            int accountFor = 0;

            _repoFilesButtons = new Button[_repoFiles.Count];

            int Line = 0;
            int Colum = 0;

            int yOffset = 0;

            for (int i = page * 18; i < page * 18 + 18; i++)
            {
                // 207 x 78

                if (i >= _repoFiles.Count) break;

                int appCat = GetCatagoryIndex(_repoFiles[i].Category);

                if (Line >= 6 && appCat == catagory)
                {
                    Line = 0;
                    Colum++;
                }

                if (appCat == catagory)
                {
                    string VersionSpaces = "";

                    for (int ii = 0; ii < 25 - _repoFiles[i].Name.Length - (_repoFiles[i].GoOSVersion.TrimEnd().Length + 6); ii++)
                        VersionSpaces += " ";

                    int x = 150 + Colum * (207 + 5); // 150
                    int y = 45 + (Line) * (78 + 5);

                    _repoFilesButtons[i] = new Button(this, Convert.ToUInt16(x), // 150 
                        Convert.ToUInt16(y),
                        207, 78, _repoFiles[i].Name + VersionSpaces + "GoOS " +
                                 _repoFiles[i].GoOSVersion.TrimEnd() + "+" + "\nBy " + _repoFiles[i].Author + "\n" +
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

                    Contents.DrawImage(x, y, StoreButton);
                    if (_repoFilesButtons[i] != null) _repoFilesButtons[i].Render();
                }
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

            foreach (Button i in catagoryButtons) i.Render();
            prevousButton.Render();
            nextButton.Render();

            Contents.DrawString((144 / 2) - (Font_1x.MeasureString(category.Trim()) / 2), 563, category.Trim(), Font_1x, Color.White);
            Contents.DrawString(pagex, 563, (page + 1).ToString(), Font_1x, Color.White);

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

        private string GetInfoBoardFile()
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
                    string httpget = "GET /GoOS/" + Kernel.edition + "-status.gostore HTTP/1.1\r\n" +
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

        private string[] GetCatagoriesFile()
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
                    string httpget = "GET /GoOS/cat.gostore HTTP/1.1\r\n" +
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
        }

        private void nextPage()
        {
            if (_repoFilesButtons.Length / 18 > page)
            {
                page++;
                Render(Catagories[catagory]);
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
            if (page > 0)
            {
                page--;
                Render(Catagories[catagory]);
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