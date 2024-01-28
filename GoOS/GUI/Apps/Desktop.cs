using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Core.Memory;
using Cosmos.System;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoOS.GUI.Models;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class Desktop : Window
    {
        Button AppsFolderButton;

        private string[] contextMenuButtons =
        {
            " About GoOS ",
            " Garbage Collect ",
            " Check for Updates "
        };

        public override void ShowContextMenu()
        {
            if (MouseManager.X != 0 && MouseManager.Y != 0)
            {
                ContextMenu.Show(contextMenuButtons, 155, ContextMenu_Handle);
            }
        }

        private void ContextMenu_Handle(string item)
        {
            switch (item)
            {
                case " About GoOS ":
                    WindowManager.AddWindow(new About());
                    break;
                case " Garbage Collect ":
                    Dialogue.Show("Garbage Collection", Heap.Collect() + " bytes freed");
                    break;
                case " Check for Updates ":
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
                            string httpget = "GET /GoOS/" + Kernel.edition + ".goos HTTP/1.1\r\n" +
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

                            string[] responseParts =
                                receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                            if (responseParts.Length < 2 || responseParts.Length > 2)
                                Dialogue.Show("GoOS Update", "Invalid HTTP response!", default,
                                    WindowManager.errorIcon);

                            string content = responseParts[1];

                            if (content != Kernel.version && content != Kernel.editionnext && Kernel.BuildType != "INTERNAL TEST BUILD")
                            {
                                Dialogue.Show("GoOS Update",
                                    "A newer version of GoOS is available on Github.\nWe recommend you update to the latest version for stability and security reasons.\nhttps://github.com/Owen2k6/GoOS/releases\nCurrent Version: " +
                                    Kernel.version + "\nLatest Version: " + content);
                            }
                            if (content != Kernel.version && content != Kernel.editionnext && Kernel.BuildType == "INTERNAL TEST BUILD")
                            {
                                Dialogue.Show("It's time to move on...",
                                    "The Internal Test Version for this edition of GoOS has ended\nThis build of GoOS can no longer access GoOS Online Services.\nPlease check with your INTERNAL TEST Group to see if a new version has been issued.");
                            }
                            else if (content == Kernel.editionnext)
                            {
                                Dialogue.Show("GoOS Update",
                                    "The next GoOS has been released.\nWe don't want to force you to update but at least check out whats new in GoOS " +
                                    Kernel.editionnext + "!\nhttps://github.com/Owen2k6/GoOS/releases/tag/" +
                                    Kernel.editionnext);
                            }
                            else if (content == "404")
                            {
                                Dialogue.Show("GoOS Update", "Your Version of GoOS does not support GoOS Update.");
                            }
                            else
                            {
                                Dialogue.Show("GoOS Update", "You are running the latest version of GoOS.\nVersion: " +
                                                             Kernel.version + "\nLatest Version: " + content);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Dialogue.Show("GoOS Update", "Failed to connect to Owen2k6 Api. Error: " + ex);
                    }

                    break;
            }
        }

        public Desktop()
        {
            Contents = new Canvas(WindowManager.Canvas.Width, Convert.ToUInt16(WindowManager.Canvas.Height - 28));
            //Contents.Clear(Kernel.DesktopColour);
            Contents.DrawImage(0, 0, background, false);
            Title = nameof(Desktop);
            Visible = true;
            Closable = false;
            HasTitlebar = false;
            Unkillable = true;
            SetDock(WindowDock.None);

            AppsFolderButton = new Button(this, 20, 20, 64, 80, "Apps")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.White,
                TextColour = Color.Black,

                Image = folderIcon,
                Clicked = AppsFolderButton_Click
            };

            AppsFolderButton.Render();
            //TODO: xrc if you dare edit this panel you wont exist on this project anymore
            if (Kernel.BuildType != "R")
            {
                if (Kernel.BuildType == "NIFPR")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    string line2 =
                        "Shh lets not leak our hard work.";
                    string line3 =
                        "Despite this being open source, Lets keep the new features as secret as possible.";
                    string line4 = "Thank you to everyone that is actively developing and testing GoOS.";

                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 53, line1,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2) - 1, Contents.Height - 41, line2,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line3) - 1, Contents.Height - 29, line3,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line4) - 1, Contents.Height - 17, line4,
                        Font_1x,
                        Color.White);
                }
                else if (Kernel.BuildType == "PRB")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    string line2 =
                        "This Build is not stable and is not recommended for use by Non-Testers";
                    string line3 =
                        "Official use of this build type is limited to testers only.";
                    string line4 = "GoOS Update and Security are not available for these builds.";

                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 53, line1,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2) - 1, Contents.Height - 41, line2,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line3) - 1, Contents.Height - 29, line3,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line4) - 1, Contents.Height - 17, line4,
                        Font_1x,
                        Color.White);
                }
                else if (Kernel.BuildType == "PRE")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    string line2 =
                        "This is a Pre Release of GoOS";
                    string line3 =
                        "We don't recommend using this build for regular use.";
                    string line4 = "This build sports beta functions that may not be included in the final release.";

                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 53, line1,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2) - 1, Contents.Height - 41, line2,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line3) - 1, Contents.Height - 29, line3,
                        Font_1x,
                        Color.White);
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line4) - 1, Contents.Height - 17, line4,
                        Font_1x,
                        Color.White);
                }
                else if (Kernel.BuildType == "INTERNAL TEST BUILD")
                {
                    string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 17, line1,
                        Font_1x,
                        Color.White);
                }
                else
                {
                    string line = "GoOS " + Kernel.BuildType + " " + Kernel.version;
                    Contents.DrawString(Contents.Width - Font_1x.MeasureString(line) - 1, Contents.Height - 17, line,
                        Font_1x,
                        Color.White);
                }
            }
        }

        private void AppsFolderButton_Click()
        {
            WindowManager.AddWindow(new AppManager());
        }
    }
}