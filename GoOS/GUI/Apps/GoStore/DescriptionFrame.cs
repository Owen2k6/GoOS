using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using PrismAPI.Graphics;
using static GoOS.Resources;
using System.Net.Sockets;
using Console = BetterConsole;
using GoOS.Commands;

namespace GoOS.GUI.Apps.GoStore
{
    public class DescriptionFrame : Window
    {
        List<string> DescriptionLines;
        Button OpenButton;
        Button InstallButton;
        Application App;

        public DescriptionFrame(Application app)
        {
            // Set class variables
            App = app;

            // Create the window.
            Contents = new Canvas(480, 360);
            Title = "GoStore";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Center);

            // Initialize the controls
            OpenButton = new Button(this, 101, 323, 133, 30, "Open")
            {
                Clicked = OpenButton_Click,
                UseSystemStyle = false,
                BackgroundColour = new Color(0, 0, 0, 0),
                RenderWithAlpha = true
            };
            InstallButton = new Button(this, 246, 323, 133, 30, !IO.File.Exists(@"0:\go\" + app.Filename) ? "Install" : "Uninstall")
            {
                Clicked = InstallButton_Click,
                UseSystemStyle = false,
                BackgroundColour = new Color(0, 0, 0, 0),
                RenderWithAlpha = true
            };

            DescriptionLines = new List<string>();
            DescriptionLines.AddRange(SpliceText("Description: " + app.Description.Replace("\\n", "\n"), 59));
            DescriptionLines.AddRange(SpliceText("Version: " + app.Version.Replace("\\n", "\n"), 59));
            DescriptionLines.AddRange(SpliceText("Author: " + app.Author.Replace("\\n", "\n"), 59));
            DescriptionLines.AddRange(SpliceText("Category: " + app.Category.Replace("\\n", "\n"), 59));
            DescriptionLines.AddRange(SpliceText("GoOS Version: " + app.GoOSVersion.Replace("\\n", "\n"), 59));

            Contents.DrawString(10, 10, "test", default, Color.White);
        }

        static string[] SpliceText(string input, int n)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                result.Append(input[i]);

                if ((i + 1) % n == 0)
                {
                    result.Append("\n");
                }
            }

            return result.ToString().Split('\n');
        }

        public override void Paint()
        {
            Contents.DrawImage(0, 0, GoStoreDescFrame, false);
            /*Contents.DrawString(10, 10, App.Name, Font_2x, Color.White);
            Contents.DrawImage(OpenButton.X, OpenButton.Y, GoStoreButtonBlue);
            Contents.DrawImage(InstallButton.X, InstallButton.Y, InstallButton.Title == "Install" ? GoStoreButtonGreen : GoStoreButtonRed);
            for (int i = 0; i < DescriptionLines.Count; i++) Contents.DrawString(10, 56 + i * 16, DescriptionLines[i], Font_1x, Color.White);
            OpenButton.Render();
            InstallButton.Render();
            RenderSystemStyleBorder();*/
        }

        private void InstallButton_Click()
        {
            try
            {
                //Cosmos.System.MouseManager.X = (uint)X + InstallButton.X;
                //Cosmos.System.MouseManager.Y = (uint)Y + InstallButton.Y;

                //if (!MainFrame.AllowDLFrom.Contains(App.GoOSVersion.Trim()))
                //{
                //    Dialogue.Show("GoStore", "This application requires a newer version of GoOS!");
                //    return;
                //}

                if (!IO.File.Exists(@"0:\go\" + App.Filename))
                {
                    using (TcpClient tcpClient = new TcpClient())
                    {
                        var dnsClient = new DnsClient();

                        // DNS
                        dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                        dnsClient.SendAsk(App.Repository);

                        // Address from IP
                        Address address = dnsClient.Receive();
                        dnsClient.Close();
                        string serverIP = address.ToString();

                        tcpClient.Connect(serverIP, 80);
                        NetworkStream stream = tcpClient.GetStream();
                        string httpget = "GET /" + App.Filename + " HTTP/1.1\r\n" +
                                         "User-Agent: GoOS\r\n" +
                                         "Accept: */*\r\n" +
                                         "Accept-Encoding: identity\r\n" +
                                         "Host: " + App.Repository + "\r\n" +
                                         "Connection: Keep-Alive\r\n\r\n";
                        byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                        stream.Write(dataToSend, 0, dataToSend.Length);

                        // Receive data
                        byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                        int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                        string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                        string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                        if (responseParts.Length < 2 || responseParts.Length > 2) Dialogue.Show("GoStore", "Invalid HTTP response!", default, WindowManager.errorIcon);

                        if (responseParts[1] == "404")
                        {
                            Dialogue.Show("Error", "The requested file or resource was not found.", default, WindowManager.errorIcon);
                            return;
                        }
                    
                        if (!IO.Directory.Exists(@"0:\go")) IO.Directory.CreateDirectory(@"0:\go");
                    
                        IO.File.WriteAllText(@"0:\go\" + App.Filename, responseParts[1]);
                    }

                    Dialogue.Show("Success", "Application installed successfully.");
                    InstallButton.Title = "Uninstall";
                }
                else
                {
                    while (IO.File.Exists(@"0:\go\" + App.Filename)) IO.File.Delete(@"0:\go\" + App.Filename);

                    Dialogue.Show("Success", "Application uninstalled successfully.");
                    InstallButton.Title = "Install";
                }

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
            }
            else
            {
                Console.Clear();
                Console.Title = "Terminal - GoIDE";
                WindowManager.AddWindow(new GTerm(false));
                Console.Clear();

                if (!App.Filename.EndsWith(".9xc"))
                    Run.Main(@"0:\go\" + App.Filename, false);
                else
                    _9xCode.Interpreter.Run(@"0:\go\" + App.Filename);

                Console.Clear();
                WindowManager.RemoveWindowByTitle("Terminal - GoIDE");
                Console.Title = "GTerm";
            }
        }
    }
}
