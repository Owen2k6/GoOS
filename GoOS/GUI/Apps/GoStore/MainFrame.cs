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
        string[] Catagories = { "ERROR" }; // Hardcoded fallback
        public static string Version = "1.0";

        private int catagory = 0;
        private int page = 0;

        private List<Application> _repoFiles;

        private Button nextButton;
        private Button prevousButton;

        public static readonly List<string> AllowDLFrom = new List<string>
        {
            "1.5"
        };
        private Canvas _infoBoard;
        private string _infoBoardText = "Welcome to GoStore! Please wait while we load application data...";

        // Hard-coded example data to prevent freezing if server is unavailable
        private string[] sampleRepos = { "api.goos.owen2k6.com" };
        private string sampleInfoFile = "GoOS Test|Test application for GoOS|1.0|Owen2k6|Utilities|1.5|test|goexe";

        public MainFrame()
        {
            try
            {
                // Create the window first
                Contents = new Canvas(800, 600);
                Title = "GoStore";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize basic objects
                _repoFiles = new List<Application>();
                _infoBoard = new Canvas(424, 16);
                textX = 424;

                // Try to load data
                LoadData();

                InitialiseButtons();
                Render(Catagories[0]);
            }
            catch (Exception e)
            {
                Dialogue.Show("GoStore", "Failed to connect to GoOS Services", default, WindowManager.errorIcon);
                // Don't close the window, just show a blank screen
                Contents.Clear();
                RenderSystemStyleBorder();
            }
        }

        private void LoadData()
        {
            bool anyDataLoaded = false;

            // Try to get categories from server
            try
            {
                string[] serverCategories = GetCatagoriesFile();
                if (serverCategories != null && serverCategories.Length > 0)
                {
                    Catagories = serverCategories;
                    anyDataLoaded = true;
                }
            }
            catch (Exception e)
            {
                // Use default categories
            }

            // Try to get repositories
            string[] repos;
            try
            {
                repos = GetReposFile();
                if (repos == null || repos.Length == 0)
                {
                    repos = sampleRepos;
                }
            }
            catch (Exception)
            {
                repos = sampleRepos;
            }

            catagoryButtons = new Button[Catagories.Length];
            _repoFilesButtons = new Button[50]; // Pre-allocate reasonable size

            // Try to get infoboard text
            try
            {
                string infoText = GetInfoBoardFile();
                if (!string.IsNullOrEmpty(infoText))
                {
                    _infoBoardText = infoText;
                }
            }
            catch (Exception)
            {
                _infoBoardText = "Welcome to GoStore! Some features may not be available due to connection issues.";
            }

            // Load applications from repos
            foreach (string repo in repos)
            {
                if (string.IsNullOrEmpty(repo)) continue;

                try
                {
                    string infoFileContent = GetInfoFile(repo);
                    if (string.IsNullOrEmpty(infoFileContent))
                        continue;

                    Infofile infoFile = new Infofile(infoFileContent.Split('\n'), repo);

                    foreach (string program in infoFile.Contents)
                    {
                        if (string.IsNullOrEmpty(program)) continue;

                        string[] appData = program.Split('|');
                        if (appData.Length < 5) continue;

                        Application app = new Application(appData, infoFile.URL);
                        app.Downloadable = AllowDLFrom.Contains(app.GoOSVersion);
                        _repoFiles.Add(app);
                        anyDataLoaded = true;
                    }
                }
                catch (Exception)
                {
                    // Continue to next repo
                }
            }

            // If no applications were loaded, throw an exception
            if (_repoFiles.Count == 0)
            {
                // Add sample app as fallback
                Application sampleApp = new Application(sampleInfoFile.Split('|'), "api.goos.owen2k6.com");
                sampleApp.Downloadable = true;
                _repoFiles.Add(sampleApp);

                if (!anyDataLoaded)
                {
                    Dialogue.Show("GoStore", "Failed to load application data from server", default, WindowManager.errorIcon);
                }
            }
        }

        private void InitialiseButtons()
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

            return 0; // Return first category instead of -1 to avoid crashes
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
            {
                if (b != null)
                    Controls.Remove(b);
            }

            for (int i = 0; i < _repoFilesButtons.Length; i++)
            {
                _repoFilesButtons[i] = null;
            }

            Contents.Clear();
            Contents.DrawImage(0, 0, Resources.GoStore, false);

            int Line = 0;
            int Colum = 0;
            int buttonCount = 0;

            for (int i = page * 18; i < Math.Min(_repoFiles.Count, page * 18 + 18); i++)
            {
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

                    int x = 150 + Colum * (207 + 5);
                    int y = 45 + (Line) * (78 + 5);

                    _repoFilesButtons[buttonCount] = new Button(this, Convert.ToUInt16(x),
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

                    buttonCount++;
                    Line++;

                    Contents.DrawImage(x, y, StoreButton);
                    _repoFilesButtons[buttonCount - 1].Render();
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

        // Simple, synchronous HTTP GET without using any async methods
        private string SimpleHttpGet(string host, string path)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                string serverIP = ResolveDNS(host);
                if (string.IsNullOrEmpty(serverIP))
                    throw new Exception("DNS resolution failed");

                // Connect with a simple approach - no async or timeout
                tcpClient.Connect(serverIP, 80);

                NetworkStream stream = tcpClient.GetStream();

                string httpget = "GET " + path + " HTTP/1.1\r\n" +
                                 "User-Agent: GoOS\r\n" +
                                 "Accept: */*\r\n" +
                                 "Accept-Encoding: identity\r\n" +
                                 "Host: " + host + "\r\n" +
                                 "Connection: close\r\n\r\n";

                byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                stream.Write(dataToSend, 0, dataToSend.Length);

                byte[] receivedData = new byte[8192]; // Use a smaller buffer
                int bytesRead = 0;

                bytesRead = stream.Read(receivedData, 0, receivedData.Length);

                string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                int headerEnd = receivedMessage.IndexOf("\r\n\r\n");
                if (headerEnd == -1) throw new Exception("Invalid HTTP response");

                return receivedMessage.Substring(headerEnd + 4);
            }
        }

        // Network methods that handle exceptions internally
        private string GetInfoFile(string repo)
        {
            try
            {
                return SimpleHttpGet(repo, "/info.glist");
            }
            catch
            {
                return "";
            }
        }

        private string GetInfoBoardFile()
        {
            try
            {
                return SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/" + Kernel.edition + "-status.gostore");
            }
            catch
            {
                return "";
            }
        }

        private string[] GetReposFile()
        {
            try
            {
                string result = SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/repos.gostore");
                if (string.IsNullOrEmpty(result))
                    return new string[0];
                return result.Split('\n');
            }
            catch
            {
                return new string[0];
            }
        }

        private string[] GetCatagoriesFile()
        {
            try
            {
                string result = SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/cat.gostore");
                if (string.IsNullOrEmpty(result))
                    return new string[0];
                return result.Split('\n');
            }
            catch
            {
                return new string[0];
            }
        }

        private string ResolveDNS(string host)
        {
            var dnsClient = new DnsClient();
            dnsClient.Connect(DNSConfig.DNSNameservers[0]);
            dnsClient.SendAsk(host);
            Address address = dnsClient.Receive();
            dnsClient.Close();
            return address.ToString();
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
            // Count items in current category
            int itemsInCategory = 0;
            foreach (Application app in _repoFiles)
            {
                if (GetCatagoryIndex(app.Category) == catagory)
                    itemsInCategory++;
            }

            int maxPages = (itemsInCategory + 17) / 18; // Ceiling division

            if (page < maxPages - 1)
            {
                page++;
                Render(Catagories[catagory]);
            }
        }

        private void previousPage()
        {
            if (page > 0)
            {
                page--;
                Render(Catagories[catagory]);
            }
        }
    }
}