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
    public class Help
    {
        public static void help()
        {
            Console.WriteLine("");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("----------------GoOS Help-----------------");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("- here is the list of commands:          -");
            Console.WriteLine("- Help - Shows this page                 -");
            Console.WriteLine("- credits - shows the developers         -");
            Console.WriteLine("- clear - clears the text                -");
            Console.WriteLine("- sysinf - Shows system info             -");
            Console.WriteLine("- shutdown - Shuts down GoOS             -");
            Console.WriteLine("- ipconfig - shows your local ip         -");
            Console.WriteLine("- gocalc - GoOS calculator   (Disabled)  -");
            Console.WriteLine("- dir - Shows files and folders          -");
            Console.WriteLine("- readfile - Allows you to read files    -");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine(" ");
        }
    }
}