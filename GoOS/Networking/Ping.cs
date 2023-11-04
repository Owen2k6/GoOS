using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;
using System.Text;
using GoOS.Themes;
using GoOS.Commands;
using Console = BetterConsole;
using ConsoleColor = PrismAPI.Graphics.Color;
using static GoOS.Core;
using System.Threading;
using PrismAPI.Graphics;
using IL2CPU.API.Attribs;
using PrismAPI.Hardware.GPU;
using GoOS.GUI;
using GoOS.GUI.Apps;

namespace GoOS.Networking;

public class Ping
{
    public static void Run()
    {
        try
        {
            log(ConsoleColor.Red, "1");
            using (var xClient = new DHCPClient())
            {
                /** Send a DHCP Discover packet **/
                //This will automatically set the IP config after DHCP response
                xClient.SendDiscoverPacket();
                log(ConsoleColor.Blue, NetworkConfiguration.CurrentAddress.ToString());
            }

            using (var xClient = new TcpClient())
            {
                log(ConsoleColor.Red, "2");
                try
                {
                    Address ip = new Address( byte.Parse("216"), byte.Parse("14"), byte.Parse("148"),byte.Parse("33")); 
                    log(ConsoleColor.Blue, ip.ToString());
                    xClient.Connect(ip, 12000, 1000);
                    //5.39.84.58
                }
                catch (Exception e)
                {
                    log(ConsoleColor.Red, e.Message);
                }

                log(ConsoleColor.Red, "3");
                
                xClient.Send(Encoding.UTF8.GetBytes("TEST"));
                
            }
        }
        catch (Exception e)
        {
            log(ConsoleColor.Red, "Internal Error:");
            log(ConsoleColor.White, e.ToString());
        }
    }
}