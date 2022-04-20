using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.Core;
using Cosmos.System.Graphics;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.Debug.Kernel;
using Cosmos.HAL.Drivers.PCI.Video;
using Cosmos.System.Network;
using System.IO;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.Config;
using Cosmos.HAL;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.Network.IPv4.TCP.FTP;
using Cosmos;
using Cosmos.HAL.Drivers.USB;
using Cosmos.HAL.Drivers.PCI;
using Cosmos.HAL.Drivers;
using Cosmos.HAL.Network;
using Cosmos.Common.Extensions;
using Cosmos.Common;
using Cosmos.Core.Memory;
using Cosmos.Core.IOGroup;

namespace GoOS
{
    public class Kernel : Sys.Kernel
    {
        private static string request = string.Empty;
        private static TcpClient tcpc = new TcpClient(80);
        private static Address dns = new Address(8, 8, 8, 8);
        private static EndPoint endPoint = new EndPoint(dns, 80);
        public static bool ParseHeader()
        {
            return false;
        }
        bool isenabled = true;
        public static VGAScreen VScreen = new VGAScreen();

        protected override void BeforeRun()
        {
            try
            {
                NetworkDevice nic = NetworkDevice.GetDeviceByName("eth0"); //get network device by name
                IPConfig.Enable(nic, new Address(192, 168, 1, 69), new Address(255, 255, 255, 0), new Address(192, 168, 1, 254)); //enable IPv4 configuration
                using (var xClient = new DHCPClient())
                {
                    /** Send a DHCP Discover packet **/
                    //This will automatically set the IP config after DHCP response
                    xClient.SendDiscoverPacket();
                }
                using (var xClient = new DnsClient())
                {
                    xClient.Connect(new Address(192, 168, 1, 254)); //DNS Server address

                    /** Send DNS ask for a single domain name **/
                    xClient.SendAsk("github.com");

                    /** Receive DNS Response **/
                    Address destination = xClient.Receive(); //can set a timeout value


                }
            } catch
            {
                Console.WriteLine("Error starting Goplex Web Interface.");
                Console.WriteLine("The system will proceed to boot without networking.");
                Console.WriteLine("Press ENTER to continue (and yes it has to be ENTER)");
                Console.ReadLine();
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Goplex Studios GoOS");
            Console.WriteLine("Copyright 2022 (c) Owen2k6");
            Console.ForegroundColor = ConsoleColor.Yellow;
             Console.WriteLine("This is a PRIVATE DEVELOPMENT BUILD. DO NOT REDISTRIBUTE");
            // Console.Writeline("This is a PRIVATE BETA BUILD. DO NOT REDISTRIBUTE");
            // Console.Writeline("This is a Public Beta Build.");
            // Console.Writeline("This is a Public Development Build.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("For more info on GoOS, type 'cinfo'.");
            Console.WriteLine("Support Status for this build could not be found.");
            Console.WriteLine("Type 'HELP' for a list of working commands");
            Console.ForegroundColor = ConsoleColor.Green;
        }

        protected override void Run()
        {
            Console.Write("0:\\");
            String input = Console.ReadLine();
            //And so it begins...
            if (input == "cinfo")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Goplex Operating System");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("GoOS is owned by Goplex Studios.");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("SYSTEM INFOMATION:");
                Console.WriteLine("GoOS Version 1.3.5.20");
                Console.WriteLine("Owen2k6 Api version: 0.12");
                Console.WriteLine("Branch: Development");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Copyright 2022 (c) Owen2k6");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (input == "help")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Goplex Operating System");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("HELP - Shows system commands");
                Console.WriteLine("CINFO - Shows system infomation");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (input == "support")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Goplex Studios Support");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("To get support, you must be in the Goplex Studios Discord Server.");
                Console.WriteLine("Discord Link: https://discord.gg/3tex5G8Grp");
                Console.WriteLine("Open support tickets in #get-staff-help");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Provide the following infomation when opening \n a support ticket.");
                Console.WriteLine("Support Code: 019x2910b11");
                Console.WriteLine("GOOSE: 1.0000.0");
                Console.WriteLine("O2K6API: 0.12");
            }
            else if (input == "testapp")  
            {
            
            }










            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry, but `" + input + "` is not a command");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Type HELP for a list of commands");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
