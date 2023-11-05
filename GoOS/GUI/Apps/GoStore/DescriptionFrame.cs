using System;
using IO = System.IO;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using TcpClient = Cosmos.System.Network.IPv4.TCP.TcpClient;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class DescriptionFrame : Window
    {
        Button IDEOpenButton;
        Button CancelButton;
        Button InstallButton;
        String Name, File, Repo;

        public DescriptionFrame(string name, string version, string author, string description, string language, string repo)
        {
            // Set class variables
            File = name + "." + language;
            Name = name;
            Repo = repo;

            // Create the window.
            Contents = new Canvas(300, 250);
            Title = "GoStore";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Center);

            // Initialize the controls
            IDEOpenButton = new Button(this, 10, Convert.ToUInt16(Contents.Height - 30), 104, 20, "Open in IDE") { Clicked = IDEOpenButton_Click };
            CancelButton = new Button(this, Convert.ToUInt16(Contents.Width - 172), Convert.ToUInt16(Contents.Height - 30), 64, 20, "Cancel") { Clicked = CancelButton_Click };
            InstallButton = new Button(this, Convert.ToUInt16(Contents.Width - 98), Convert.ToUInt16(Contents.Height - 30), 88, 20, !IO.File.Exists(@"0:\go\" + File) ? " Install " : "Uninstall") { Clicked = InstallButton_Click };

            // Paint the window.
            Contents.Clear(Color.LightGray);
            RenderSystemStyleBorder();
            Contents.DrawString(10, 10, name, Font_2x, Color.White);
            Contents.DrawString(10, 56, "Version: " + version.Replace("\\n", "\n"), Font_1x, Color.White);
            Contents.DrawString(10, 72, "Author: " + author.Replace("\\n", "\n"), Font_1x, Color.White);
            Contents.DrawString(10, 86, "Language: " + language.Replace("\\n", "\n"), Font_1x, Color.White);
            Contents.DrawString(10, 100, "Description: " + description.Replace("\\n", "\n"), Font_1x, Color.White);
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray); Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
            IDEOpenButton.Render();
            CancelButton.Render();
            InstallButton.Render();
        }

        private void InstallButton_Click()
        {
            try
            {
                if (!IO.File.Exists(@"0:\go\" + File))
                {
                    var dnsClient = new DnsClient();
                    var tcpClient = new TcpClient();

                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk(Repo);
                    Address address = dnsClient.Receive();
                    dnsClient.Close();

                    tcpClient.Connect(address, 80);

                    string httpget = "GET /" + File + " HTTP/1.1\r\n" +
                                     "User-Agent: GoOS\r\n" +
                                     "Accept: */*\r\n" +
                                     "Accept-Encoding: identity\r\n" +
                                     "Host: " + Repo + "\r\n" +
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
                        if (responseParts[1] == "404")
                        {
                            Dialogue.Show("404", "The requested file or resource was not found.", default, WindowManager.errorIcon);
                            return;
                        }

                        if (!IO.Directory.Exists(@"0:\go")) IO.Directory.CreateDirectory(@"0:\go");

                        IO.File.WriteAllText(@"0:\go\" + File, responseParts[1]);
                    }

                    Dialogue.Show("Success", "Application installed successfully.");
                    InstallButton.Title = "Uninstall";
                    InstallButton.Render();
                }
                else
                {
                    while (IO.File.Exists(@"0:\go\" + File)) IO.File.Delete(@"0:\go\" + File);

                    Dialogue.Show("Success", "Application uninstalled successfully.");
                    InstallButton.Title = " Install ";
                    InstallButton.Render();
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("Error", ex.ToString());
            }
        }

        private void CancelButton_Click() => Dispose();

        private void IDEOpenButton_Click()
        {
            if (!IO.File.Exists(@"0:\go\" + File))
            {
                Dialogue.Show("Error", "You need to install this package before you can use it.", default, WindowManager.errorIcon);
            }
            else
            {
                WindowManager.AddWindow(new GoIDE.IDEFrame(Name, @"0:\go\" + File, File.EndsWith(".9xc")));
            }
        }
    }
}
