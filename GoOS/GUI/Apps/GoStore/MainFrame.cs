using System;
using System.IO;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using TcpClient = Cosmos.System.Network.IPv4.TCP.TcpClient;
using System.Text;
using System.Collections.Generic;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class MainFrame : Window
    {
        Button[] CategoryButtons;
        Button[] RepoFilesButtons;
        List<(string, string, string, string, string, string, string, string)> repoFiles = new List<(string, string, string, string, string, string, string, string)>();
        List<(string, string, string, string, string, string, string, string)> repoFilesFiltered = new List<(string, string, string, string, string, string, string, string)>();

        readonly string[] repos =
        {
            "apps.goos.owen2k6.com",
            //"dev.apps.goos.owen2k6.com",
            "repo.mobren.net"
        };

        public MainFrame()
        {
            try
            {
                // Get the file list from every repo
                string[][] infoFiles =
                {
                    GetInfoFile(repos[0]).Split('\n'),
                    GetInfoFile(repos[1]).Split('\n'),
                    //GetInfoFile(repos[2]).Split('\n')
                };

                int o = 0;
                foreach (string[] file in infoFiles)
                {
                    foreach (string program in file)
                    {
                        string[] metadata = program.Split('|');
                        repoFiles.Add((metadata[0], metadata[1], metadata[2], metadata[3], metadata[4], metadata[5], metadata[6], repos[o]));
                    }
                    o++;
                }

                // Create the window.
                Contents = new Canvas(400, 300);
                Title = "GoStore";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize the controls.
                CategoryButtons = new Button[]
                {
                    new Button(this, 2, 2, 80, 20, "Games")
                    {
                        UseSystemStyle = false,
                        BackgroundColour = Color.DeepGray,
                        SelectionColour = Color.Black,
                        HasSelectionColour = true,
                        Clicked = GamesButton_Click
                    },
                    new Button(this, 2, 22, 80, 20, "Utilities")
                    {
                        UseSystemStyle = false,
                        BackgroundColour = Color.DeepGray,
                        SelectionColour = Color.Black,
                        HasSelectionColour = true,
                        Clicked = UtilitiesButton_Click
                    },
                    new Button(this, 2, 42, 80, 20, "Demos")
                    {
                        UseSystemStyle = false,
                        BackgroundColour = Color.DeepGray,
                        SelectionColour = Color.Black,
                        HasSelectionColour = true,
                        Clicked = DemosButton_Click
                    },
                    new Button(this, 2, 62, 80, 20, "Networking")
                    {
                        UseSystemStyle = false,
                        BackgroundColour = Color.DeepGray,
                        SelectionColour = Color.Black,
                        HasSelectionColour = true,
                        Clicked = NetworkingButton_Click
                    }
                };

                GamesButton_Click(); // Default category

                // Paint the window.
                Contents.Clear(Color.LightGray);
                RenderSystemStyleBorder();
                Contents.DrawFilledRectangle(2, 2, 80, Convert.ToUInt16(Contents.Height - 4), 0, Color.DeepGray);
                foreach (Button u in CategoryButtons) u.Render();
                foreach (Button i in RepoFilesButtons) i.Render();
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
            catch { }

            return string.Empty;
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
            WindowManager.AddWindow(new DescriptionFrame(repoFiles[GetIndexByTitle(i)].Item1,
                repoFiles[GetIndexByTitle(i)].Item4, repoFiles[GetIndexByTitle(i)].Item5,
                repoFiles[GetIndexByTitle(i)].Item3, repoFiles[GetIndexByTitle(i)].Item2,
                repoFiles[GetIndexByTitle(i)].Item6, repoFiles[GetIndexByTitle(i)].Item8));
        }

        private void RenderRepoFilesFromCategory(string category)
        {
            try
            {
                repoFilesFiltered = new List<(string, string, string, string, string, string, string, string)>();

                foreach (var repoFile in repoFiles)
                {
                    if (repoFile.Item6 == category)
                    {
                        repoFilesFiltered.Add(repoFile);
                    }
                }

                RepoFilesButtons = new Button[repoFilesFiltered.Count];

                for (int i = 0; i < repoFilesFiltered.Count; i++)
                {
                    RepoFilesButtons[i] = new Button(this, Convert.ToUInt16(90 + i / 10 * 185), Convert.ToUInt16(10 + (i * 20 - i / 10 * 200)), Convert.ToUInt16(repoFiles[i].Item1.Length * 8), 20, repoFiles[i].Item1)
                    {
                        Name = repoFilesFiltered[i].Item1,
                        UseSystemStyle = false,
                        BackgroundColour = Color.LightGray,
                        SelectionColour = new Color(100, 100, 100),
                        HasSelectionColour = true,
                        ClickedAlt = repoFiles_Click
                    };
                }
            }
            catch { }
        }

        private void GamesButton_Click()
        {
            RenderRepoFilesFromCategory("Games");
        }

        private void UtilitiesButton_Click()
        {
            RenderRepoFilesFromCategory("Utilities");
        }

        private void DemosButton_Click()
        {
            RenderRepoFilesFromCategory("Demos");
        }

        private void NetworkingButton_Click()
        {
            RenderRepoFilesFromCategory("Networking");
        }
    }
}
