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
        public static VGAScreen VScreen = new VGAScreen();
        private static readonly CosmosVFS cosmosVFS = new Sys.FileSystem.CosmosVFS();
        private readonly Sys.FileSystem.CosmosVFS fs = cosmosVFS;


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
                var file_stream = File.Create(@"0:\GOOSE.GOOSE");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                var file_stream = File.Create(@"0:\Networking.GOOSE");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            try
            {
                File.WriteAllText(@"0:\GOOSE.GOOSE", "GoOS Enviorment has not been implimented yet... we are working to make an Operating Enviorment, but its not our main focus.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.Clear();
            Cosmos.HAL.Global.TextScreen.SetColors(ConsoleColor.Black, ConsoleColor.White);
            Console.WriteLine("   Goplex Studios - GoOS");
            Console.WriteLine("   Version 1.1.1");
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
                Console.WriteLine("Goplex Studios GoOS 1.1.1");
                Console.WriteLine("Build type: Release");
                Console.WriteLine("Build number: 1512");
                Console.WriteLine("Build Support key: 0x9364758789");
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

                Console.WriteLine("GoCalc 1.1");
                Console.WriteLine("Note: can only do simple math");
                Console.WriteLine("WARNING FROM DEVELOPERS: Decimals will crash the entire OS! be careful");
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
            else if (input.ToLower().IndexOf("add ") != -1) //Addition
            {
                string[] firststring = input.ToLower().Split("add ");
                string[] secondstring = firststring[1].Split('+');
                int finalresult = Int16.Parse(secondstring[0]) + Int16.Parse(secondstring[1]);
                Console.WriteLine(finalresult);
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