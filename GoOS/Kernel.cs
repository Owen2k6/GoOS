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

            var fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            try
            {
                var file_stream = File.Create(@"0:\GoOS.sys");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                var file_stream = File.Create(@"0:\Networking.sys");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                File.WriteAllText(@"0:\GoOS.sys", "01110010 01100001 01110010 01100111 01101111 01101001 01101010 01101010 01101110 01100001 01100101 01101111 01110010 01101010 01100111 01101110 01101111 01100001 01110010 01100101 01101010 01100111 01101110 01101111 01101010 01100001 01110010 01101110 01100111 01101111 01101010 01110010 01100101 01100111 01101111 01101001 01101010 01100101 01100001 01110010 01101110 01100111 01101111 01101010 01100001 01110010 01101110 01100101 01100111 01101111 01101010 01100101 01110010 01100001 01101111 01101010 01100001 01110010 01101110 01100111 01101111 01100101 01101010 01110010 01101110 01100111 01101111 01101001 01101010 01100101 01100001 01110010 01101110 01100111 01101111 01101001 01101010 01100001 01110010 01100101 01101110 01100111 01101111 01101001 01101010 01100001 01110010 01100101 01101111 01101110 01100111 01100101 01101111 01101010 01101110 01100111 01101111 01100001 01101010 01100101 01110010 01101110 01100111 01101111 01101001 01101010 01100001 01100101 01110010 01101110 01100111 01101111 01101010 01100001 01110010 01100101 01101110 01100111 01101111 01101010 01110010 01100101 01100001 01101000 01101111 01110100 01101010 01101000 01110010 01100101 01100001 01101111 01110100 01101010 01101000 01110100 01101010 01110010 01101110 01110100 01100111 01101010 01101111 01110010 01100111 01101000 01101111 01110010 01101000 01100111 01100001 01101111 01101001 01101010 01101000 01100111 01101111 01101010 01100001 01110010 01100101 01101110 01100111 01101111 01101010 01101001 01100001 01110010 01100101 01101110 01100111 01101111 01100001 01110010 01101010 01100111 01101000 01101110 01101111 01100001 01101001 01101010 01100101 01110010 01100111 01101110 01101111 01100001 01101001 01101010 01110010 01101110 01100111 01101111 01100001 01110010 01101010 01101001 01101110 01100111 01101111 01101010 01101001 01100001 01110010 01101110 01100111 01101111 01101001 01101010 01100001 01110010 01101110 01100111 01101111 01101010 01100001 01110010 01110100 01101000 01101111 01101010 01110010 01110100 01101000 01110010 01101111 01100101 01100001 01101010 01101000 01110100 01101111 01101001 01100101 01101010 01110111 01101000 01101111 01110111 01101010 01100101 01101000 01110100 01101111 01101010 01101001 01100101 01110111 01110010 01110100 01101000 01101111 01100101 01110111 01101001 01110010 01101010 01110100 01101000 01101111 01110111 01100101 01110010 01101010 01101000 01110100 01101111 01101010 01110111 01100101 01110010 01110100 01101000 01101111 01110111 01101010 01100101 01110010 01110100 01101000 01101111 01101010 01110111 01100101 01101001 01110010 01101000 01110010 01100101 01110111 01101111 01101001 01110100 01101000 01110111 01100101 01110010 01101111 01101001 01101010 01110100 01101000 01110111 01100101 01101111 01110010 01101010 01110100 01101000 01110111 01101111 01100101 01110010 01101010 01110100 01101000 01110111 01101111 01100101 01110010 01101010 01101000 01110100 01101111 01101010 01100101 01110111 01110010 01101001 01110100 01101000 01110111 01101111 01101001 01110010 01101010 01110100 01101000 01101111 01110111 01100101 01110010 01101010 01101000 01110100 01101111 01101001 01101010 01110111 01110010 01100101 01101110 01100111 01101111 01101001 01101010 01110010 01100101 01110111 01101110 01101111 01110111 01101010 01100101 01110010 01110100 01101000 01110111 01101111 01110010 01100101 01101010 01101110 01100111 01101111 01101001 01110111 01110010 01101010 01100101 01110100 01101000 01101111 01101001 01110111 01101010 01100101 01110010 01101000 01110100 01101111 01101001 01101010 01110111 01100101 01110010 01101000 01110100 01101111 01101001 01101010 01110111 01100101 01110010 01101111 01110111 01101001 01101010 01100101 01110010 01101000 01110100 01101111 01101010 01110111 01100101 01110010 01101000 01110100 01110111 01100101 01101111 01101010 01110010 01101000 01110100 01101111 01110111 01100101 01110010 01101010 01101000 01110100 01110111 01100101 01101111 01110010 01101010 01110100 01101000 01101111 01101010 01110111 01100101 01110010 01101000 01110100 01101111 01100101 01110111 01101001 01110010 01101010 01110100 01101000 01101010 01100101 01110111 01101111 01110010 01101001 01110100 01101000 01101111 01110111 01100101 01101001 01110010 01101010 01110100 01101000 01101111 01100101 01110010 01101010 01110111 01101001 01101000 01110100 01101111 01110111 01101010 01100101 01110010 01101000 01110100 01101111 01101010 01110111 01101001 01100101 01110010 01101000 01110100 01101111 01110111 01100101 01101010 01110010 01101000 01110100 01110111 01101111 01110010 01100101 01101010 01110100 01101000 01100101 01110111 01110010 01101111 01101010 01101001 01101111 01100101 01110100 01101001 01101000 01101010 01100101 01110010 01110111 01101111 01101010 01101000 01110100 01110111 01101111 01101010 01100101 01110010 01110100 01101000 01110111");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                File.WriteAllText(@"0:\Networking.sys", "01000111 01101111 01001111 01010011 00100000 01101110 01100101 01110100 01110111 01101111 01110010 01101011 01101001 01101110 01100111 00100000 01101101 01100001 01101110 01100001 01100111 01100101 01101101 01100101 01101110 01110100 00100000 01101001 01110011 00100000 01100001 00100000 01110011 01111001 01110011 01110100 01100101 01101101 00100000 01100110 01101001 01101100 01100101 00101110 00100000 01110011 01111001 01110011 01110100 01100101 01101101 00100000 01100110 01101001 01101100 01100101 01110011 00100000 01101000 01100001 01110110 01100101 00100000 01101110 01101111 00100000 01110101 01110011 01100101 00100000 01100010 01110101 01110100 00101110 00100000 01101101 01101001 01100111 01101000 01110100 00100000 01100001 01110011 00100000 01110111 01100101 01101100 01101100 00100000 01101000 01100001 01110110 01100101 00100000 01110011 01101111 01101101 01100101 00100000 01110010 01100101 01100001 01101100 01101001 01110011 01101101 00100000 01110100 01101111 00100000 01110100 01101000 01100101 00100000 01100110 01101001 01101100 01100101 01110011 01111001 01110011 01110100 01100101 01101101 00100000 00111011 00101001");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.Clear();
            Cosmos.HAL.Global.TextScreen.SetColors(ConsoleColor.Black, ConsoleColor.White);
            Console.WriteLine("   Goplex Studios - GoOS");
            Console.WriteLine("   Version 1.2");
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
            try
            {
                Sys.FileSystem.VFS.VFSManager.CreateFile(@"0:\BootSuccess.txt");
                Sys.FileSystem.VFS.VFSManager.DeleteFile(@"0:\BootSuccess.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        protected override void Run()
        {
            String prefix = @"0:\ ";
            String input = Console.ReadLine();



            if (input.ToLower() == "help")
            {
                //
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
            else if (input.ToLower() == "credits")
            {
                Console.Clear();
                Console.WriteLine("----------------------------------");
                Console.WriteLine("-------------credits--------------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("---------Created by Owen----------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("---------Helped by Zulo-----------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("-----Others that have helped!-----");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("---------British Geek Guy---------");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("Thank you for trying out GoOS!");
                Console.WriteLine(" ");
                Console.WriteLine("Returned to Command centre");
                Console.WriteLine(" ");
            }
            else if (input.ToLower() == "clear")
            {
                Console.Clear();
            }
            else if (input.ToLower() == "sysinf")
            {
                Console.WriteLine(" ");
                Console.WriteLine("Goplex Studios GoOS 1.2");
                Console.WriteLine("Build type: Release");
                Console.WriteLine("Build number: 2000");
                Console.WriteLine("Build Support key: 9364758789");
                Console.WriteLine(" ");
            }
            else if (input.ToLower() == "shutdown")
            {
                Console.WriteLine("System shutting down...");
                Console.WriteLine("Goodbye!");
                Cosmos.System.Power.Shutdown();
            }
            else if (input.ToLower() == "support")
            {
                Console.WriteLine("Discord: https://dsc.gg/goplex");
                Console.WriteLine("type the link into the VM host webbrowser");
                Console.WriteLine("if your running the os as your active OS, please stick to using VMs until we have a stable mainframe.");
            }
            else if (input.ToLower() == "gocalc")
            {
                float firstNum;
                float secondNum;                   //Variables for equation
                string operation;
                float answer;

                Console.WriteLine("GoCalc 1.5");
                Console.WriteLine("Note: can only do simple math");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();

                Console.Write("Enter the first number in your basic equation: ");
                firstNum = float.Parse(Console.ReadLine());

                //User input for equation

                Console.Write("Ok now enter your operation ( x , / , +, -) ");
                operation = Console.ReadLine();

                Console.Write("Now enter your second number in the basic equation: ");
                secondNum = float.Parse(Console.ReadLine());
                if (operation == "x")
                {
                    answer = firstNum * secondNum;
                    Console.WriteLine(firstNum + " x " + secondNum + " = " + answer);
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }
                else if (operation == "/")
                {
                    answer = firstNum / secondNum;
                    Console.WriteLine(firstNum + " / " + secondNum + " = " + answer);
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }
                //Getting answers
                else if (operation == "+")
                {
                    answer = firstNum + secondNum;
                    Console.WriteLine(firstNum + " + " + secondNum + " = " + answer);
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }
                else if (operation == "-")
                {
                    answer = firstNum - secondNum;
                    Console.WriteLine(firstNum + " - " + secondNum + " = " + answer);
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Sorry that is not correct format! Please restart!");     //Catch
                    Console.WriteLine("Press ENTER to continue");
                    Console.ReadLine();
                }

            }
            else if (input.ToLower() == "dir")
            {
                string[] filePaths = Directory.GetFiles(@"0:\");
                for (int i = 0; i < filePaths.Length; ++i)
                {
                    string path = filePaths[i];
                    Console.WriteLine(System.IO.Path.GetFileName(path));
                }
                foreach (var d in System.IO.Directory.GetDirectories(@"0:\"))
                {
                    var dir = new DirectoryInfo(d);
                    var dirName = dir.Name;

                    Console.WriteLine(dirName + " <DIR>");
                }

            }
            else if (input.ToLower() == "ipconfig")
            {
                Console.WriteLine(NetworkConfig.CurrentConfig.Value.IPAddress.ToString());
            }
            else if (input.ToLower() == "readfile")
            {
                Console.WriteLine("Path to file");
                var FTR = Console.ReadLine();
                try
                {
                    Console.WriteLine(File.ReadAllText(FTR));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else if (input.ToLower() == "createfile")
            {
                var FTR = Console.ReadLine();
                Console.WriteLine("Path (Path must exist. )");
                try
                {
                    Console.WriteLine(File.ReadAllText(FTR));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else if (input.ToLower() == "graphic.systest.gapp")
            {
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(640, 480, ColorDepth.ColorDepth32));
                canvas.Clear(System.Drawing.Color.Blue);

                try
                {
                    Pen pen = new Pen(System.Drawing.Color.Red);


                    // A GreenYellow horizontal line
                    pen.Color = System.Drawing.Color.White;
                    canvas.DrawLine(pen, 0, 10, 640, 10);
                    canvas.DrawLine(pen, 0, 9, 640, 9);
                    canvas.DrawLine(pen, 0, 8, 640, 8);
                    canvas.DrawLine(pen, 0, 7, 640, 7);
                    canvas.DrawLine(pen, 0, 6, 640, 6);
                    canvas.DrawLine(pen, 0, 5, 640, 5);
                    canvas.DrawLine(pen, 0, 4, 640, 4);
                    canvas.DrawLine(pen, 0, 3, 640, 3);
                    canvas.DrawLine(pen, 0, 2, 640, 2);
                    canvas.DrawLine(pen, 0, 1, 640, 1);
                    canvas.DrawLine(pen, 0, 0, 640, 0);
                    canvas.DrawLine(pen, 0, 0, 640, 0);

                    Console.ReadKey();
                    Sys.Power.Shutdown();
                }
                catch (Exception e)
                {
                    mDebugger.Send("Exception occurred: " + e.Message);
                    Sys.Power.Shutdown();
                }
            }
            else if (input.ToLower() == "update.systest.gapp")
            {
                Console.WriteLine("System Version: 1.2");
                NetworkDevice nic = NetworkDevice.GetDeviceByName("eth0"); //get network device by name
                IPConfig.Enable(nic, new Address(192, 168, 1, 69), new Address(255, 255, 255, 0), new Address(192, 168, 1, 254)); //enable IPv4 configuration
                using (var xClient = new DHCPClient())
                {
                    xClient.SendDiscoverPacket();
                }
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
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