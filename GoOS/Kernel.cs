using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;
using System.Linq.Expressions;
using Cosmos.Core.Memory;
using System.Drawing;
using IL2CPU.API.Attribs;
using System.Text;
using CosmosTTF;
using TechOS.System;
using TechOS.GUI;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.Core.Memory;
using CosmosFtpServer;
using Cosmos.System.Network.IPv4.UDP;
using System.Diagnostics;
using GoOS;
using Cosmos.HAL.BlockDevice.Registers;
using System.Threading;

//Goplex Studios - GoOS
//Copyright (C) 2022  Owen2k6
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace GoOS
{



    public class Kernel : Sys.Kernel
    {
        //GoOS Core
        public void print(string str)
        {
            Console.WriteLine(str);
        }
        public void write(string str)
        {
            Console.Write(str);
        }
        public void textcolour(System.ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
        }
        public void highlightcolour(System.ConsoleColor colour)
        {
            Console.BackgroundColor = colour;
        }
        public void sleep(int time)
        {
            Thread.Sleep(time);
        }
        //Core end




        //[ManifestResourceStream(ResourceName = "Wallpaper.bmp")]
        //public static byte[] Wallpaper;
        //public static Bitmap wallpaper = new Bitmap(Wallpaper);
        public static Canvas canvas;


        private Boolean adminconsoledisk = false;


        //Login Data
        private Boolean loginsystemenabled = false;
        private String username = null;
        private String password = null;



        private static Sys.FileSystem.CosmosVFS FS;
        public static string file;

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
        /* SNAKE VARS */
        public int[] matrix;
        public List<int[]> commands;
        public List<int[]> snake;
        public List<int> food;
        public int randomNumber;
        public int snakeCount;
        Random rnd = new Random();

        protected override void BeforeRun()
        {

            try
            {
                NetworkDevice nic = NetworkDevice.GetDeviceByName("eth0"); //get network device by name
                IPConfig.Enable(nic, new Address(192, 168, 1, 32), new Address(255, 255, 255, 0), new Address(192, 168, 1, 254)); //enable IPv4 configuration
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

            }
            catch
            {
                print("Error starting Goplex Web Interface.");
                print("The system will proceed to boot without networking.");
                print("Press ENTER to continue (and yes it has to be ENTER)");
                Console.ReadLine();
            }

            Console.Clear();
            textcolour(ConsoleColor.Green);
            textcolour(ConsoleColor.Red);
            print("                    GGGGGGGGGGGG                   ");
            textcolour(ConsoleColor.DarkRed);
            print("               GGGGGGGGGGGGGGGGGGGGGG              ");
            textcolour(ConsoleColor.Magenta);
            print("  GGGGGGGGGG GGGGGGGGGGGGGGGGGGGGGGGGGG            ");
            textcolour(ConsoleColor.DarkMagenta);
            print("  GGGGGGGG   GGGGGGGGG        GGGGGG               ");
            textcolour(ConsoleColor.Red);
            print("  GGGGGGG    GGGGG                                 ");
            textcolour(ConsoleColor.DarkRed);
            print("  GGGGGG     GGG                                   ");
            textcolour(ConsoleColor.Magenta);
            write("  GGGGG      GG                                    ");
            textcolour(ConsoleColor.White);
            write("Goplex Studios GoOS.");
            print("");
            textcolour(ConsoleColor.Red);
            write("  GGGGG      G            GGGGGGGGGGGGGGGGGGGG     ");
            textcolour(ConsoleColor.White);
            write("Copyright 2022 (c) Owen2k6.");
            print("");
            textcolour(ConsoleColor.DarkRed);
            write("  GGGGG      GG           GGGGGGGGGGGGGGGGGGG      ");
            textcolour(ConsoleColor.White);
            write("Version 1.4");
            print("");
            textcolour(ConsoleColor.Magenta);
            write("  GGGGG      GG           GGGGGGGGGGGGGGGGGGG      ");
            textcolour(ConsoleColor.White);
            write("Private Development Build");
            print("");
            textcolour(ConsoleColor.DarkMagenta);
            print("  GGGGGG     GGGG         GGGGGGGGGGGGGGGGGG       ");
            textcolour(ConsoleColor.Red);
            print("  GGGGGGG    GGGGGG              GGGGGGGGGG        ");
            textcolour(ConsoleColor.DarkRed);
            print("  GGGGGGGGG  GGGGGGGGGGGGGGGGGGGGGGGGGGGG          ");
            textcolour(ConsoleColor.Magenta);
            print("  GGGGGGGGGGG                                      ");
            textcolour(ConsoleColor.DarkMagenta);
            print("  GGGGGGGGGGGGGGG                  GGGG            ");
            textcolour(ConsoleColor.Red);
            print("  GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG            ");
            textcolour(ConsoleColor.DarkRed);
            print("  GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG            ");
            try
            {
                FS = new Sys.FileSystem.CosmosVFS(); Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS); FS.Initialize(true);
                var total_space = FS.GetTotalSize(@"0:\");
                adminconsoledisk = true;
            }
            catch (Exception e)
            {
                textcolour(ConsoleColor.Red);
                print("GoOS Admin could not detect a disk. system will not support any apps that require a HDD to write/read from.");
                print("GoOS Needs a HDD installed to use some of the cool features");
                print("The GitHub releases page usually includes a disk built for GoOS");
                print("Disks aren't required but they're highly reccomended.");
                adminconsoledisk = false;
            }
            textcolour(ConsoleColor.Green);
            if (loginsystemenabled)
            {
                textcolour(ConsoleColor.Magenta);
                //Login System 0.1 Primitive edition
                print("Hello, " + username + "!");
                print("In order to proceed into GoOS, you must login with your password.");
                textcolour(ConsoleColor.Yellow);
                String input = Console.ReadLine();
                if (input == password)
                {
                    textcolour(ConsoleColor.Cyan);
                    Console.Clear();
                    print("Welcome back to GoOS.");
                }
                else
                {
                    print("Incorrect password.");
                    print("Press any key to retry");
                    Console.ReadKey();
                    Cosmos.System.Power.Reboot();
                }
            }
        }

        protected override void Run()
        {
            write("0:\\");
            String input = Console.ReadLine();
            //And so it begins...
            //Commands Section
            if (input == "cinfo")
            {
                textcolour(ConsoleColor.Magenta);
                print("Goplex Operating System");
                textcolour(ConsoleColor.Blue);
                print("GoOS is owned by Goplex Studios.");
                textcolour(ConsoleColor.DarkYellow);
                print("SYSTEM INFOMATION:");
                print("GoOS Version 1.4");
                print("Owen2k6 Api version: 0.15");
                print("Branch: Development");
                textcolour(ConsoleColor.Red);
                print("Copyright 2022 (c) Owen2k6");
                textcolour(ConsoleColor.Green);
            }
            else if (input == "help")
            {
                textcolour(ConsoleColor.Magenta);
                print("Goplex Operating System");
                textcolour(ConsoleColor.Blue);
                print("HELP - Shows system commands");
                print("CINFO - Shows system infomation");
                print("SUPPORT - Shows how to get support");
                print("GAMES - Shows the list of GoOS Games");
                print("CORE - Displays GoOS Core infomation");
                print("CALC - Shows a list of possible calculation commands");
                print("CREDITS - Shows the GoOS Developers");
                textcolour(ConsoleColor.Green);
            }
            else if (input == "credits")
            {
                textcolour(ConsoleColor.Cyan);
                print("Goplex Studios - GoOS");
                print("Discord Link: https://discord.owen2k6.com/");
                textcolour(ConsoleColor.Red);
                print("Contributors:");
                print("Owen2k6 - Main Developer and creator");
                print("Zulo - Helped create the command system");
                print("moderator_man - Helped with my .gitignore issue and knows code fr");
                print("");
            }
            else if (input == "support")
            {
                textcolour(ConsoleColor.Cyan);
                print("Goplex Studios Support");
                textcolour(ConsoleColor.Red);
                print("== For OS Support");
                print("To get support, you must be in the Goplex Studios Discord Server.");
                print("Discord Link: https://discord.owen2k6.com/");
                print("Open support tickets in #get-staff-help");
                print("== To report a bug");
                print("Go to the issues tab on the Owen2k6/GoOS Github page");
                print("and submit an issue with the bug tag.");
            }
            else if (input == "games")
            {
                textcolour(ConsoleColor.Magenta);
                print("Goplex Games List");
                textcolour(ConsoleColor.Blue);
                print("TEXTADVENTURES - Text based adventure game because why not");
                textcolour(ConsoleColor.Green);
            }
            else if (input == "core")
            {
                textcolour(ConsoleColor.Magenta);
                print("GoOS Core Ver 0.3");
                print("The Main backend to GoOS.");
                print("==========================");
                print("==Developed using Cosmos==");
                print("==========================");
                textcolour(ConsoleColor.Red);
                print("GoOS Core Is still in early development.");
                print("there are a lot of issues known and we are working on it! ");
                textcolour(ConsoleColor.Green);
            }

            //Games Section

            else if (input == "textadventures")
            {
                textcolour(ConsoleColor.Magenta);
                print("Goplex Studios - Text Adventures");
                print("Developed using GoOS Core");
                textcolour(ConsoleColor.Yellow);
                print("????: Hello there, what's your name?");
                textcolour(ConsoleColor.Blue);
                write("Enter a name: ");
                String name = Console.ReadLine();
                textcolour(ConsoleColor.Yellow);
                print("????: Ah. Hello there, " + name);
                print("????: When there are Convos, press ENTER to move on to the next message :)");
                Console.ReadKey();
                print("????: You probably dont know me, but its better that way...");
                Console.ReadKey();
                print("????: Anyways, There are 1 stories we can enter.");
                print("????: Yes i know wrong plural, but there will be more written in the future!");
                Console.ReadKey();
                print("????: The first one i'll say is \"Temple Run\" ");
                print("????: - You are a criminal planning the heist of a lifetime");
                print("????: This heist is set on robbing the great temple.");
                Console.ReadKey();
                print("????: For now, Temple Run is the only available story.");
                Console.ReadKey();
                print("????: So what will it be?");
                print("????: Selection Options: TEMPLERUN");
                textcolour(ConsoleColor.Blue);
                write("Choose One of the Options: ");
                String selection = Console.ReadLine();
                textcolour(ConsoleColor.Yellow);
                if (selection == "templerun")
                {
                    textcolour(ConsoleColor.Red);
                    print("\"Temple Run\" Selected.");
                    textcolour(ConsoleColor.Blue);
                    print("You wake up... it's 2:45AM and you can't get to sleep...");
                    Console.ReadKey();
                    print("You look at your calendar...");
                    Console.ReadKey();
                    print("It's August 4th 2023. 3 days before the heist.");
                    Console.ReadKey();
                    print(name + ": Damn we need to get planning if we're gonna pull this off... ");
                    Console.ReadKey();
                    print("You pick up your phone and call Joe the Fixer...");
                    Console.ReadKey();
                    print(name + ": Joe! How have you been man...");
                    Console.ReadKey();
                    textcolour(ConsoleColor.DarkYellow);
                    print("Joe: Hello... things are not so good...");
                    Console.ReadKey();
                    textcolour(ConsoleColor.Blue);
                    print(name + ": What? Why?");
                    Console.ReadKey();
                    textcolour(ConsoleColor.DarkYellow);
                    print("Joe: Because our plans aren't really in the best ways. How would we survive a 100+ Meter fall into Stone?");
                    Console.ReadKey();
                    textcolour(ConsoleColor.Blue);
                    print(name + "That was Bob's idea... Not mine");
                    Console.ReadKey();
                    textcolour(ConsoleColor.DarkYellow);
                    print("Joe: God. Bob really... Right im adding him to the call.");
                    Console.ReadKey();
                    textcolour(ConsoleColor.Cyan);
                    print("Bob has been added to the call.");
                    Console.ReadKey();
                    print("Bob: What do you want Joe?");
                    Console.ReadKey();
                    textcolour(ConsoleColor.DarkYellow);
                    print("Joe: You know what i want... This plan was pulled out of your-");
                    textcolour(ConsoleColor.Cyan);
                    print("Bob: OK OK. Fine. but jumping in is the best option");
                    Console.ReadKey();
                    textcolour(ConsoleColor.DarkYellow);
                    print("Joe: Ok. well we gotta head down to the planning table before we \n can really think of anything else to do.");
                    Console.ReadKey();
                    textcolour(ConsoleColor.Cyan);
                    print("Bob: Alright. i'll meet you down there.");
                    print("Bob Left the call");
                    Console.ReadKey();
                    textcolour(ConsoleColor.DarkYellow);
                    print("Joe: Got that " + name + "? We'll meet you down there.");
                    Console.ReadKey();
                    textcolour(ConsoleColor.Blue);
                    print(name + ": Got some things i want to do before heading down. see you there.");
                    print(name + " Left the call");
                    Console.ReadKey();
                    textcolour(ConsoleColor.Red);
                    print("This Game is still under development. You have reached the end of this game so far!");
                    print("Keep your OS Up to date to recieve updates for this game!");

                }
            }

            //Calculator Area

            else if (input == "calc")
            {
                textcolour(ConsoleColor.Magenta);
                print("GoCalc Commands");
                textcolour(ConsoleColor.Blue);
                print("ADD - Add 2 numbers");
                print("SUBTRACT - Subtract 2 numbers");
                print("DIVIDE - Divide 2 numbers");
                print("MULTIPLY - Multiply 2 numbers");
                print("SQUARE - Square a number");
                print("CUBE - Cube a number");
                print("POWER10 - Make a number to the power of 10");
                textcolour(ConsoleColor.Green);
            }
            else if (input == "add")
            {
                print("GoCalc - Addition");
                print("Whole numbers only !!");
                print("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                print("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                print("Adding up to");
                int ans = no1 + no2;
            }
            else if (input == "subtract")
            {
                print("GoCalc - Subtraction");
                print("Whole numbers only !!");
                print("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                print("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                print("Adding up to");
                int ans = no1 - no2;
            }
            else if (input == "divide")
            {
                print("GoCalc - Division");
                print("Whole numbers only !!");
                print("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                print("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                print("Adding up to");
                int ans = no1 / no2;
            }
            else if (input == "multiply")
            {
                print("GoCalc - Multiplication");
                print("Whole numbers only !!");
                print("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                print("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                print("Adding up to");
                int ans = no1 * no2;
            }
            else if (input == "square")
            {
                print("GoCalc - Squaring");
                print("Whole numbers only !!");
                print("Enter number to square: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1;
            }
            else if (input == "cube")
            {
                print("GoCalc - Cubing");
                print("Whole numbers only !!");
                print("Enter number to cube: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1 * no1;
            }
            else if (input == "power10")
            {
                print("GoCalc - To the power of 10");
                print("Whole numbers only !!");
                print("Enter number to p10: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1;
            }

            // GoOS Admin

            //Disk Only stuff
            //FS = new Sys.FileSystem.CosmosVFS(); Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS); FS.Initialize();
            //else if (input == "loaddisk")
            //{
            //if (!adminconsoledisk)
            //{
            //    textcolour(ConsoleColor.Red;
            //    print("GoOS Admin: Ensure you are using the iso file provided by Owen2k6 on release.");
            //    print("GoOS Admin: The system will crash if a disk can not be located. ");
            //    print("GoOS Admin: Press any key to continue");
            //    Console.ReadKey();
            //    FS = new Sys.FileSystem.CosmosVFS(); Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS); FS.Initialize(true);
            //    print("GoOS Admin: HardDisk enabled for session");
            //    adminconsoledisk = true;
            //    textcolour(ConsoleColor.Green;
            //}
            //if (adminconsoledisk)
            //{
            //    textcolour(ConsoleColor.Red;
            //    print("GoOS Admin: System Already has hard disk loaded");
            //    textcolour(ConsoleColor.Green;
            //}
            //}
            else if (input == "diskcheck")
            {
                if (!adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    print("GoOS Admin: There is currently no disk loaded to the system.");
                    textcolour(ConsoleColor.Green);
                }
                if (adminconsoledisk)
                {
                    try
                    {
                        textcolour(ConsoleColor.Red);
                        print("GoOS Admin: Showing Disk Information for 0:\\");
                        var available_space = FS.GetAvailableFreeSpace(@"0:\");
                        var total_space = FS.GetTotalSize(@"0:\");
                        var label = FS.GetFileSystemLabel(@"0:\");
                        var fs_type = FS.GetFileSystemType(@"0:\");
                        print("Available Free Space: " + available_space + "(" + (available_space / 1e+9) + "GiB)");
                        print("Total Space on disk: " + total_space + "(" + (total_space / 1e+9) + "GiB)");
                        print("Disk Label: " + label);
                        print("File System Type: " + fs_type);
                        textcolour(ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        print("GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                        print("GoOS Admin: For system security, we have disabled all Drive functions.");
                        adminconsoledisk = false;
                        textcolour(ConsoleColor.Green);
                    }
                }
            }
            else if (input == "ls")
            {
                if (!adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    print("GoOS Admin: There is currently no disk loaded to the system.");
                    textcolour(ConsoleColor.Green);
                }
                if (adminconsoledisk)
                {
                    try
                    {
                        textcolour(ConsoleColor.Red);
                        var directory_list = Directory.GetFiles(@"0:\");
                        foreach (var file in directory_list)
                        {
                            print(file);
                        }
                        textcolour(ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        print("GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                        print("GoOS Admin: For system security, we have disabled all Drive functions.");
                        adminconsoledisk = false;
                        textcolour(ConsoleColor.Green);
                    }
                }
            }
            else if (input == "notepad")
            {
                if (!adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    print("GoOS Admin: There is currently no disk loaded to the system.");
                    textcolour(ConsoleColor.Green);
                }
                if (adminconsoledisk)
                {
                    textcolour(ConsoleColor.White);
                    MIV.StartMIV();
                    textcolour(ConsoleColor.Green);
                }
            }
            else if (input == "del")
            {
                if (!adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    print("GoOS Admin: There is currently no disk loaded to the system.");
                    textcolour(ConsoleColor.Green);
                }
                if (adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    print("GoOS Admin: Enter file name");
                    textcolour(ConsoleColor.Yellow);
                    write("FilePath: 0:\\");
                    String inputaman = Console.ReadLine();
                    try
                    {
                        File.Delete(@"0:\" + inputaman);
                        textcolour(ConsoleColor.Blue);
                        print("GoOS Admin: File Deleted!");
                        textcolour(ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        print("Please send the following to GoOS Developers");
                        print(e.ToString());
                        textcolour(ConsoleColor.Green);
                    }
                }
            }
            else if (input == "ld")
            {
                if (!adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    print("GoOS Admin: There is currently no disk loaded to the system.");
                    textcolour(ConsoleColor.Green);
                }
                if (adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    var label = FS.GetFileSystemLabel(@"0:\");
                    print("GoOS Admin: Relabel disk");
                    print("GoOS Admin: Press ENTER to leave the label as \"" + label + "\"");
                    textcolour(ConsoleColor.Yellow);
                    write("New Label for 0:\\: ");
                    String inputamana = Console.ReadLine();
                    if (inputamana == string.Empty)
                    {
                        inputamana = label;
                    }
                    try
                    {
                        FS.SetFileSystemLabel(@"0:\", inputamana);
                        textcolour(ConsoleColor.Blue);
                        print("GoOS Admin: Drive Label modified from " + label + " to " + inputamana);
                        textcolour(ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        print("Please send the following to GoOS Developers");
                        print(e.ToString());
                        textcolour(ConsoleColor.Green);
                    }
                }
            }
            else if (input == "ftp")
            {
                if (!adminconsoledisk)
                {
                    textcolour(ConsoleColor.Red);
                    print("GoOS Admin: There is currently no disk loaded to the system.");
                    textcolour(ConsoleColor.Green);
                }
                if (adminconsoledisk)
                {
                    using (var xServer = new FtpServer(FS, "0:\\"))
                    {
                        /** Listen for new FTP client connections **/
                        print("GoOS Admin: Listening on " + NetworkConfiguration.CurrentAddress.ToString() + ":21");
                        print("Use PLAIN configurations with no login information.");
                        print("FTP MODE ENABLED. REBOOT TO DISABLE");
                        xServer.Listen();
                    }
                }
            }
            else if (input == "ipconf")
            {

                textcolour(ConsoleColor.Red);
                print("GoOS Admin: Showing Internet Information");
                print(NetworkConfiguration.CurrentAddress.ToString());
                textcolour(ConsoleColor.Green);
            }
            else if (input == "sleeper")
            {
                print("shhhh wait");
                sleep(2000);
                print("2000 ticks wasted :)");
            }

            //smth cool bro

            else if (input == "gui") {
            if (!adminconsoledisk)
            {
                textcolour(ConsoleColor.Red);
                print("GoOS Admin: There is currently no disk loaded to the system.");
                textcolour(ConsoleColor.Green);
            }
            if (adminconsoledisk)
            {
            // backup canvases
            //
            // canvas = new VBECanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
            // canvas = new SVGAIICanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                 canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
                 Sys.MouseManager.ScreenWidth = 1012;
                 Sys.MouseManager.ScreenHeight = 768;
                 while (true)
                 {
                     Heap.Collect();
                     canvas.Clear(Color.Green);
                     //guicanvas.DrawImage(wallpaper, 0, 0);
                     TextBox.textbox(canvas, 4, 4, 0, 0, "GoOS GUI");
                     TextBox.textbox(canvas, 12, 12, 0, 0, "24092022");
                     TextBox.textbox(canvas, 40, 40, 60, 24, "24092022");
                     Cursor.DrawCursor(canvas, Sys.MouseManager.X, Sys.MouseManager.Y);
                     
                     canvas.Display();
                 }
             }


            }



            else
            {
                textcolour(ConsoleColor.Red);
                write("sorry, but ");
                textcolour(ConsoleColor.Yellow);
                write("`" + input + "` ");
                textcolour(ConsoleColor.Red);
                write("is not a command");
                textcolour(ConsoleColor.Magenta);
                print("");
                print("Type HELP for a list of commands");
                textcolour(ConsoleColor.Green);
            }
            textcolour(ConsoleColor.Green);
        }
    }


}


