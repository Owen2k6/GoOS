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
using Cosmos.System.Emulation;
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

        public static VGAScreen VScreen = new VGAScreen();
        private static readonly CosmosVFS cosmosVFS = new Sys.FileSystem.CosmosVFS();
        private readonly Sys.FileSystem.CosmosVFS fs = cosmosVFS;
        Canvas canvas;

        protected override void BeforeRun()
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
            CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Console.Clear();
            Cosmos.HAL.Global.TextScreen.SetColors(ConsoleColor.Black, ConsoleColor.White);
            Console.WriteLine("   Goplex Studios - GoOS");
            Console.WriteLine("   Version 1.3 Dev");
            Console.WriteLine("   ");
            Console.WriteLine("   Type HELP for a list of commands");
            Console.WriteLine("   Type SUPPORT for support links...");
            Console.WriteLine(" ");
            var drive = new DriveInfo("0");
            Console.WriteLine("   Volume in drive 0 is " + $"{drive.VolumeLabel}");
            Console.WriteLine("   Drive " + @"0:\ " + "booted successfuly!");
            Console.WriteLine("   " + $"{drive.TotalSize}" + " bytes");
            Console.WriteLine("   " + $"{drive.AvailableFreeSpace}" + " bytes free");
            Console.Write(@"0:\ ");
        }

        protected override void Run()
        {
            String prefix = @"0:\ ";
            String input = Console.ReadLine();



            if (input.ToLower() == "setup")
            {
                if (!File.Exists(@"0:\setupdone"))
                {
                    Console.WriteLine("Welcome to GoOS!");
                    Console.WriteLine("This is an installer to help you get the essential apps for GoOS! ");
                    Console.WriteLine("Press Any Key to enter the setup");
                    Console.WriteLine("Press Ctrl+C to close GoOS");
                    Console.ReadKey();
                    var file_stream = File.Create(@"0:\setupdone");
                }
                else
                {
                    Console.WriteLine("Looks like GoOS is already installed!");
                }
            }
            else if (input.ToLower() == "dir")
            {
                var directory_list = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing("0:/");
                foreach (var directoryEntry in directory_list)
                {
                    Console.WriteLine(directoryEntry.mName);
                }
            }
            else
            {
                Console.WriteLine("sorry, but `" + input + "` is not a command");
                Console.WriteLine("type HELP for a list of commands");
            }
                Console.Write(prefix);
            }
        }
    } 