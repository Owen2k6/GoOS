using System;
using System.IO;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using TcpClient = Cosmos.System.Network.IPv4.TCP.TcpClient;
using GoOS.Themes;
using System.Text;
using System.Collections.Generic;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.GoStore
{
    public class MainFrame : Window
    {
        Button[] RepoFilesButtons;
        Canvas DescriptionCanvas;
        List<(string, string, string, string, string)> repoFiles = new List<(string, string, string, string, string)>();

        public MainFrame()
        {
            try
            {
                // Get the file list from every repo
                string[] infoFiles =
                {
                    GetInfoFile("apps.goos.owen2k6.com"),
                    //GetInfoFile("dev.apps.goos.owen2k6.com"),
                    GetInfoFile("repo.mobren.net")
                };

                foreach (string file in infoFiles)
                {
                    string[] metadata = file.Split('|');
                    repoFiles.Add((metadata[0], metadata[1], metadata[2], metadata[3], metadata[4]));
                }

                // Generate the fonts.
                Fonts.Generate();

                // Create the window.
                Contents = new Canvas(400, 300);
                Title = "GoStore";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize the controls.
                RepoFilesButtons = new Button[repoFiles.Count];

                for (int i = 0; i < repoFiles.Count; i++)
                {
                    RepoFilesButtons[i] = new Button(this, Convert.ToUInt16(10 + i / 10 * 185), Convert.ToUInt16(52 + (i * 20 - i / 10 * 200)), Convert.ToUInt16(repoFiles[i].Item1.Length * 8), 20, repoFiles[i].Item1)
                    {
                        Name = repoFiles[i].Item1,
                        UseSystemStyle = false,
                        BackgroundColour = Color.LightGray,
                        SelectionColour = new Color(100, 100, 100),
                        HasSelectionColour = true,
                        ClickedAlt = repoFiles_Click
                    };
                }

                DescriptionCanvas = new Canvas(300, 250);

                // Paint the window.
                Contents.Clear(Color.LightGray);
                RenderSystemStyleBorder();
                Contents.DrawString(10, 10, "GoStore", Fonts.Font_2x, Color.White);
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
            DescriptionCanvas.Clear(Color.LightGray);
            DescriptionCanvas.DrawLine(0, 0, DescriptionCanvas.Width - 1, 0, new Color(80, 80, 80));
            DescriptionCanvas.DrawLine(0, 0, 0, DescriptionCanvas.Height - 1, new Color(80, 80, 80));
            DescriptionCanvas.DrawLine(1, DescriptionCanvas.Height - 2, DescriptionCanvas.Width - 2, DescriptionCanvas.Height - 2, new Color(89, 89, 89));
            DescriptionCanvas.DrawLine(DescriptionCanvas.Width - 2, 1, DescriptionCanvas.Width - 2, DescriptionCanvas.Height - 1, new Color(89, 89, 89));
            DescriptionCanvas.DrawLine(0, DescriptionCanvas.Height - 1, DescriptionCanvas.Width, DescriptionCanvas.Height - 1, Color.Black);
            DescriptionCanvas.DrawLine(DescriptionCanvas.Width - 1, 0, DescriptionCanvas.Width - 1, DescriptionCanvas.Height - 1, Color.Black);
            DescriptionCanvas.DrawString(10, 10, i, Fonts.Font_2x, Color.White);
            DescriptionCanvas.DrawString(10, 52, "Version: " + repoFiles[GetIndexByTitle(i)].Item3, Fonts.Font_1x, Color.White);
            DescriptionCanvas.DrawString(10, 64, "Author: " + repoFiles[GetIndexByTitle(i)].Item3, Fonts.Font_1x, Color.White);
            DescriptionCanvas.DrawString(10, 76, "Description: " + repoFiles[GetIndexByTitle(i)].Item4, Fonts.Font_1x, Color.White);
            DescriptionCanvas.DrawString(10, 88, "Language: " + repoFiles[GetIndexByTitle(i)].Item1, Fonts.Font_1x, Color.White);

            for (int y = 0; y < 300; y++)
            {
                for (int x = 0; x < 400; x++)
                {
                    Contents[x, y] = new Color(Contents[x, y].R - 100, Contents[x, y].G - 100, Contents[x, y].B - 100);
                }
            }

            Contents.DrawImage(50, 25, DescriptionCanvas, false);
        }
    }
}
