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

        protected override void BeforeRun()
        {
            String prefix = @"0:\ ";
            var textscr = Cosmos.HAL.Global.TextScreen;
            Cosmos.System.Global.Console = new Cosmos.System.Console(textscr);
            Cosmos.HAL.Global.TextScreen = textscr;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.Write(prefix);

            
            Console.WriteLine("Goplex Studios - GoOS Ver. 2.0");
        }

        protected override void Run()
        {
            String prefix = @"0:\ ";
            String input = Console.ReadLine();



            if (input.ToLower() == "help")
            {
                Console.WriteLine("~~~GoOS Help~~~");
            }
            else if (input.ToLower() == "sysinf")
            {
            }
            else
            {
                Console.WriteLine("sorry, but `" + input + "` is not a command");
                Console.WriteLine("type HELP for a list of commands");
                Console.Write(prefix);
            }
                
            }
        }
    } 