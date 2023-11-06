using System;
using System.Collections.Generic;
using System.Linq;
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
using PrismAPI.Graphics;
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
            ContextMenu.Show(contextMenuButtons, 155, ContextMenu_Handle);
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
                        var dnsClient = new DnsClient();
                        var tcpClient = new TcpClient();
                        dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                        dnsClient.SendAsk("apps.goos.owen2k6.com");
                        Address address = dnsClient.Receive();
                        dnsClient.Close();
                        tcpClient.Connect(address, 80);
                        string httpget = "GET /GoOS/"+Kernel.edition+".goos HTTP/1.1\r\n" +
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
                            string headers = responseParts[0];
                            string content = responseParts[1];
                            if (content != Kernel.version && content != Kernel.editionnext)
                            {
                                Dialogue.Show("GoOS Update",
                                    "A newer version of GoOS is available on Github.\nWe recommend you update to the latest version for stability and security reasons.\nhttps://github.com/Owen2k6/GoOS/releases\nCurrent Version: " +
                                    Kernel.version + "\nLatest Version: " + content);
                            }
                            else if (content == Kernel.editionnext)
                            {
                                Dialogue.Show("GoOS Update",
                                    "The next GoOS has been released.\nWe don't want to force you to update but at least check out whats new in GoOS " +
                                    Kernel.editionnext + "!\nhttps://github.com/Owen2k6/GoOS/releases/tag/" + Kernel.editionnext);
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
            string line1 = "GoOS " + Kernel.BuildType + " " + Kernel.version;
            string line2 =
                "Shh! Let's try our best to not leak our hard work! May be open sourced but listing all the features to everyone ruins the surprises!";
            string line3 =
                "I want to make this new version of GoOS so that it surprises people. Sharing every little change really does spoil it.";
            string line4 = "We're all looking at xrc in this statement :)";

            Contents.DrawString(Contents.Width - Font_1x.MeasureString(line1) - 1, Contents.Height - 53, line1, Font_1x,
                Color.White);
            Contents.DrawString(Contents.Width - Font_1x.MeasureString(line2) - 1, Contents.Height - 41, line2, Font_1x,
                Color.White);
            Contents.DrawString(Contents.Width - Font_1x.MeasureString(line3) - 1, Contents.Height - 29, line3, Font_1x,
                Color.White);
            Contents.DrawString(Contents.Width - Font_1x.MeasureString(line4) - 1, Contents.Height - 17, line4, Font_1x,
                Color.White);
        }

        private void AppsFolderButton_Click()
        {
            WindowManager.AddWindow(new AppManager());
        }
    }
}