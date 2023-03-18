﻿using Cosmos.HAL;
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
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.System.Network.IPv4.UDP;
using System.Diagnostics;
using GoOS;
using Cosmos.HAL.BlockDevice.Registers;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using Cosmos.System;
using Console = System.Console;
using System.Linq;
using Cosmos.HAL.BlockDevice;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;

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

/** 
TODO:
 - Remove GoOS Networking
**/


namespace GoOS
{



    public class Kernel : Sys.Kernel
    {
        //Vars for OS
        public string version = "1.5";
        public string BuildType = "Beta";
        public bool cmdm = true;
        public bool root = false;

        //GoOS Core
        public void print(string str)
        {
            Console.WriteLine(str);
        }
        public void log(System.ConsoleColor colour, string str)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(str);
        }
        public void clog(System.ConsoleColor colour, string str)
        {
            Console.ForegroundColor = colour;
            CP737Console.Write(str + "\n");
        }
        public void cwrite(string str)
        {
            CP737Console.Write(str);

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



        public static string olddir = @"0:\";
        private Boolean adminconsoledisk = false;



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
        public static string username = null;
        public static string computername = null;

        protected override void BeforeRun()
        {
            try
            {
                FS = new Sys.FileSystem.CosmosVFS(); Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS); FS.Initialize(true);
                var total_space = FS.GetTotalSize(@"0:\");
                adminconsoledisk = true;
            }
            catch (Exception e)
            {
                log(ConsoleColor.Red, "GoOS Admin could not detect a disk. system will not support any apps that require a HDD to write/read from.");
                log(ConsoleColor.Red, "GoOS Needs a HDD installed to use some of the cool features");
                log(ConsoleColor.Red, "The GitHub releases page usually includes a disk built for GoOS");
                log(ConsoleColor.Red, "Disks aren't required but they're highly reccomended.");
                log(ConsoleColor.White, "Press any key to continue.");
                Console.ReadKey();
                adminconsoledisk = false;
            }
            try
            {
                if (!File.Exists(@"0:\content\sys\setup.gms"))
                {
                    Sys.FileSystem.VFS.VFSManager.CreateFile(@"0:\content\sys\setup.gms");
                    var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\setup.gms");
                    var setupstream = setupcontent.GetFileStream();
                    if (setupstream.CanWrite)
                    {
                        Console.Clear();
                        log(ConsoleColor.Green, "Welcome to GoOS!");
                        log(ConsoleColor.Green, "Before we continue, you need to set up your computer.");
                        write("Enter a username (default: User): ");
                        String usrn = Console.ReadLine();
                        write("Name your computer (default: GoOS): ");
                        String cprn = Console.ReadLine();

                        byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {usrn}\ncomputername: {cprn}");
                        setupstream.Write(textToWrite, 0, textToWrite.Length);
                    }


                }
                var systemsetup = File.ReadAllLines(@"0:\content\sys\setup.gms");
                foreach (string line in systemsetup)
                {
                    if (line.StartsWith("username: "))
                    {
                        username = line.Replace("username: ", "");
                    }
                    if (line.StartsWith("computername: "))
                    {
                        computername = line.Replace("computername: ", "");
                    }
                }



            }
            catch (Exception e)
            {

            }
            if (username == null)
            {
                username = "user";

            }
            if (computername == null)
            {
                computername = "GoOS";

            }
            //Console.BackgroundColor = ConsoleColor.DarkBlue;

            Console.Clear();
            clog(ConsoleColor.DarkMagenta, "╔═════════════════════════════════════════════════════════════════════════════╗");
            clog(ConsoleColor.Red, "║═══════════════════████████████══════════════════════════════════════════════║");
            clog(ConsoleColor.DarkRed, "║══════════════██████████████████████═════════════════════════════════════════║");
            clog(ConsoleColor.Magenta, "║═██████████═██████████████████████████═══════════════════════════════════════║");
            clog(ConsoleColor.DarkMagenta, "║═████████═══█████████════════██████══════════════════════════════════════════║");
            clog(ConsoleColor.Red, "║═███████════█████════════════════════════════════════════════════════════════║");
            clog(ConsoleColor.DarkRed, "║═██████═════███══════════════════════════════════════════════════════════════║");
            //Do NOT change owen.
            textcolour(ConsoleColor.Magenta);
            cwrite("║═█████══════██══════════════════════════════════");
            textcolour(ConsoleColor.White);
            cwrite("Goplex Studios GoOS.");
            textcolour(ConsoleColor.Magenta);
            cwrite("═════════║");
            clog(ConsoleColor.Green, "");
            textcolour(ConsoleColor.Red);
            cwrite("║═█████══════█════════════████████████████████═══");
            textcolour(ConsoleColor.White);
            cwrite("Copyright 2023 (c) Owen2k6.");
            textcolour(ConsoleColor.Red);
            cwrite("══║");
            clog(ConsoleColor.Green, "");
            textcolour(ConsoleColor.DarkRed);
            cwrite("║═█████══════██═══════════███████████████████════");
            textcolour(ConsoleColor.White);
            cwrite("Version " + version);
            textcolour(ConsoleColor.DarkRed);
            cwrite(" ═════════════════║");
            clog(ConsoleColor.Green, "");
            textcolour(ConsoleColor.Magenta);
            cwrite("║═█████══════██═══════════███████████████████════");
            textcolour(ConsoleColor.White);
            cwrite("Welcome to GoOS");
            textcolour(ConsoleColor.Magenta);
            cwrite("══════════════║");
            clog(ConsoleColor.Green, "");
            //Ok now continue
            clog(ConsoleColor.DarkMagenta, "║═██████═════████═════════██████████████████══════════════════════════════════║");
            clog(ConsoleColor.Red, "║═███████════██████══════════════██████████═══════════════════════════════════║");
            clog(ConsoleColor.DarkRed, "║═█████████══████████████████████████████═════════════════════════════════════║");
            clog(ConsoleColor.Magenta, "║═███████████═════════════════════════════════════════════════════════════════║");
            clog(ConsoleColor.DarkMagenta, "║═███████████████══════════════════████═══════════════════════════════════════║");
            clog(ConsoleColor.Red, "║═█████████████████████████████████████═══════════════════════════════════════║");
            clog(ConsoleColor.DarkRed, "║═█████████████████████████████████████═══════════════════════════════════════║");
            clog(ConsoleColor.Magenta, "╚═════════════════════════════════════════════════════════════════════════════╝");

            string roota = @"0:\";
            Directory.SetCurrentDirectory(roota);
        }

        protected override void Run()
        {
            textcolour(ConsoleColor.Cyan);
            string currentdir = Directory.GetCurrentDirectory() + @"\";
            string currentdirfix = @"0:\";
            if (currentdir.Contains(@"0:\\"))
            {
                currentdirfix = currentdir.Replace(@"0:\\", @"0:\");
            }
            else if (currentdir.Contains(@"0:\\\"))
            {
                currentdirfix = currentdir.Replace(@"0:\\\", @"0:\");
            }
            write($"{username}");
            textcolour(ConsoleColor.Yellow);
            write("@");
            textcolour(ConsoleColor.Cyan);
            write($"{computername} ");
            textcolour(ConsoleColor.Green);
            write(currentdirfix);
            textcolour(ConsoleColor.White);
            String input = Console.ReadLine();
            String[] args = input.Split(' ');
            

            //codein tiem
            switch (args[0])
            {
                case "help":
                    if (args.Length > 1)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    GoOS.Commands.Help.Main();
                    break;
                case "run":
                    if (args.Length > 2)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    if (args.Length == 1)
                    {
                        log(ConsoleColor.Red, "Missing arguments");
                        break;
                    }
                    GoOS.Commands.Run.Main(args[1]);
                    break;
                case "mkdir":
                    if (args.Length > 2)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    if (args.Length == 1)
                    {
                        log(ConsoleColor.Red, "Missing arguments");
                        break;
                    }
                    GoOS.Commands.Make.MakeDirectory(args[1]);
                    break;
                case "mkfile":
                    if (args.Length > 2)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    if (args.Length == 1)
                    {
                        log(ConsoleColor.Red, "Missing arguments");
                        break;
                    }
                    GoOS.Commands.Make.MakeFile(args[1]);
                    break;
                case "deldir":
                    if (args.Length > 2)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    if (args.Length == 1)
                    {
                        log(ConsoleColor.Red, "Missing arguments");
                        break;
                    }
                    GoOS.Commands.Delete.DeleteDirectory(args[1]);
                    break;
                case "delfile":
                    if (args.Length > 2)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    if (args.Length == 1)
                    {
                        log(ConsoleColor.Red, "Missing arguments");
                        break;
                    }
                    GoOS.Commands.Delete.DeleteFile(args[1]);   
                    break;
                case "cd":
                        if (args.Length > 2)
                        {
                            log(ConsoleColor.Red, "Too many arguments");
                            break;
                        }
                        if (args.Length == 1)
                        {
                            log(ConsoleColor.Red, "Missing arguments");
                            break;
                        }
                    GoOS.Commands.Cd.Run(args[1]);
                    break;
                case "cd..":
                    if (args.Length > 1)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    Directory.SetCurrentDirectory(olddir);
                    break;
                case "cdr":
                    if (args.Length > 1)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    string roota = @"0:\";
                    Directory.SetCurrentDirectory(roota);
                    break;
                case "dir":
                    if (args.Length > 1)
                    {
                        log(ConsoleColor.Red, "Too many arguments");
                        break;
                    }
                    GoOS.Commands.Dir.Run();
                    break;
                case "notepad":
                    textcolour(ConsoleColor.White);
                    MIV.StartMIV();
                    break;
                case "settings":
                    GoOS.ControlPanel.ControlPanel.Open();
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    break;

            }


            textcolour(ConsoleColor.Green);
        }

        /// 
        ///  ANYTHING BELOW IS NOT IN USE. DO NOT TOUCH!
        /// 
        protected void CommandMode()
        {
            textcolour(ConsoleColor.Green);
            write("0:\\");
            textcolour(ConsoleColor.Gray);
            String input = Console.ReadLine();
            //And so it begins...
            //Commands Section
            //^ commands are a living nightmare -ekeleze
            if (input.Equals("cinfo", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Magenta, "Goplex Operating System");
                log(ConsoleColor.Blue, "GoOS is owned by Goplex Studios.");
                log(ConsoleColor.Red, "SYSTEM INFOMATION:");
                log(ConsoleColor.Red, "GoOS Version " + version);
                log(ConsoleColor.Red, "Build Type: " + BuildType);
                log(ConsoleColor.White, "Copyright 2022 (c) Owen2k6");
            }
            else if (input.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Magenta, "Goplex Operating System");
                log(ConsoleColor.Blue, "HELP - Shows system commands");
                log(ConsoleColor.Blue, "CINFO - Shows system infomation");
                log(ConsoleColor.Blue, "SUPPORT - Shows how to get support");
                log(ConsoleColor.Blue, "CORE - Displays GoOS Core infomation");
                log(ConsoleColor.Blue, "CALC - Shows a list of possible calculation commands");
                log(ConsoleColor.Blue, "CREDITS - Shows the GoOS Developers");
                log(ConsoleColor.Blue, "DISKCHECK - Check Disk Information");
                log(ConsoleColor.Blue, "LS - List all files on the disk");
                log(ConsoleColor.Blue, "NOTEPAD - MIV Notepad (Looks like VIM)");
                log(ConsoleColor.Blue, "DEL - Delete a file");
                log(ConsoleColor.Blue, "LD - ReLabel a disk (Rename disk)");
                log(ConsoleColor.Blue, "FTP - Will not work - File Transfer Protocol");
                log(ConsoleColor.Blue, "IPCONF - List all networking information");
                log(ConsoleColor.Blue, "GUI - See a cool lil test!");
            }
            else if (input.Equals("credits", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Cyan, "Goplex Studios - GoOS");
                log(ConsoleColor.Cyan, "Discord Link: https://discord.owen2k6.com/");
                log(ConsoleColor.Red, "Contributors:");
                log(ConsoleColor.Red, "Owen2k6 - Main Developer and creator");
                log(ConsoleColor.Red, "Zulo - Helped create the command system");
                log(ConsoleColor.Red, "moderator_man - Helped with my .gitignore issue and knows code fr");
                log(ConsoleColor.Red, "atmo - GUI Libs");
            }
            else if (input.Equals("support", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Cyan, "Goplex Studios Support");
                log(ConsoleColor.Red, "== For OS Support");
                log(ConsoleColor.Red, "To get support, you must be in the Goplex Studios Discord Server.");
                log(ConsoleColor.Red, "Discord Link: https://discord.owen2k6.com/");
                log(ConsoleColor.Red, "Open support tickets in #get-staff-help");
                log(ConsoleColor.Red, "== To report a bug");
                log(ConsoleColor.Red, "Go to the issues tab on the Owen2k6/GoOS Github page");
                log(ConsoleColor.Red, "and submit an issue with the bug tag.");
            }
            else if (input.Equals("core", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Magenta, "GoOS Core Ver 0.3");
                log(ConsoleColor.Magenta, "The Main backend to GoOS.");
                log(ConsoleColor.Magenta, "==========================");
                log(ConsoleColor.Magenta, "==Developed using Cosmos==");
                log(ConsoleColor.Magenta, "==========================");
                log(ConsoleColor.Red, "GoOS Core Is still in early development.");
                log(ConsoleColor.Red, "there are a lot of issues known and we are working on it! ");
            }

            //Games Section

            //Calculator Area

            else if (input.Equals("calc", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Magenta, "GoCalc Commands");
                log(ConsoleColor.Blue, "ADD - Add 2 numbers");
                log(ConsoleColor.Blue, "SUBTRACT - Subtract 2 numbers");
                log(ConsoleColor.Blue, "DIVIDE - Divide 2 numbers");
                log(ConsoleColor.Blue, "MULTIPLY - Multiply 2 numbers");
                log(ConsoleColor.Blue, "SQUARE - Square a number");
                log(ConsoleColor.Blue, "CUBE - Cube a number");
                log(ConsoleColor.Blue, "POWER10 - Make a number to the power of 10");
            }
            else if (input.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Green, "GoCalc - Addition");
                log(ConsoleColor.Green, "Whole numbers only !!");
                log(ConsoleColor.Green, "Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Adding up to");
                int ans = no1 + no2;
                log(ConsoleColor.Yellow, "Answer: " + ans);
            }
            else if (input.Equals("subtract", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Green, "GoCalc - Subtraction");
                log(ConsoleColor.Green, "Whole numbers only !!");
                log(ConsoleColor.Green, "Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Adding up to");
                int ans = no1 - no2;
                log(ConsoleColor.Yellow, "Answer: " + ans);
            }
            else if (input.Equals("divide", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Green, "GoCalc - Division");
                log(ConsoleColor.Green, "Whole numbers only !!");
                log(ConsoleColor.Green, "Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Adding up to");
                int ans = no1 / no2;
                log(ConsoleColor.Yellow, "Answer: " + ans);
            }
            else if (input.Equals("multiply", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Green, "GoCalc - Multiplication");
                log(ConsoleColor.Green, "Whole numbers only !!");
                log(ConsoleColor.Green, "Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                log(ConsoleColor.Green, "Adding up to");
                int ans = no1 * no2;
                log(ConsoleColor.Yellow, "Answer: " + ans);
            }
            else if (input.Equals("square", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Green, "GoCalc - Squaring");
                log(ConsoleColor.Green, "Whole numbers only !!");
                log(ConsoleColor.Green, "Enter number to square: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1;
                log(ConsoleColor.Yellow, "Answer: " + ans);
            }
            else if (input.Equals("cube", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Green, "GoCalc - Cubing");
                log(ConsoleColor.Green, "Whole numbers only !!");
                log(ConsoleColor.Green, "Enter number to cube: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1 * no1;
                log(ConsoleColor.Yellow, "Answer: " + ans);
            }
            else if (input.Equals("power10", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Green, "GoCalc - To the power of 10");
                log(ConsoleColor.Green, "Whole numbers only !!");
                log(ConsoleColor.Green, "Enter number to p10: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1;
                log(ConsoleColor.Yellow, "Answer: " + ans);
            }

            // GoOS Admin

            //Disk Only stuff
            else if (input.Equals("diskcheck", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    try
                    {
                        log(ConsoleColor.Red, "GoOS Admin: Showing Disk Information for 0:\\");
                        var available_space = FS.GetAvailableFreeSpace(@"0:\");
                        var total_space = FS.GetTotalSize(@"0:\");
                        var label = FS.GetFileSystemLabel(@"0:\");
                        var fs_type = FS.GetFileSystemType(@"0:\");
                        log(ConsoleColor.Red, "Available Free Space: " + available_space + "(" + (available_space / 1e+9) + "GiB)");
                        log(ConsoleColor.Red, "Total Space on disk: " + total_space + "(" + (total_space / 1e+9) + "GiB)");
                        log(ConsoleColor.Red, "Disk Label: " + label);
                        log(ConsoleColor.Red, "File System Type: " + fs_type);
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                        log(ConsoleColor.Red, "GoOS Admin: For system security, we have disabled all Drive functions.");
                        adminconsoledisk = false;
                    }
                }
            }
            else if (input.Equals("ls", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    try
                    {
                        var directory_list = Directory.GetFiles(@"0:\");
                        foreach (var file in directory_list)
                        {
                            log(ConsoleColor.Red, file);
                        }
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                        log(ConsoleColor.Red, "GoOS Admin: For system security, we have disabled all Drive functions.");
                        adminconsoledisk = false;
                    }
                }
            }
            else if (input.Equals("notepad", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    textcolour(ConsoleColor.White);
                    MIV.StartMIV();
                }
            }
            else if (input.Equals("gostudio", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    textcolour(ConsoleColor.White);
                    //GOSStudio.StartGSS();
                }
            }
            else if (input.Equals("del", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: Enter file name");
                    textcolour(ConsoleColor.Yellow);
                    write("FilePath: 0:\\");
                    String inputaman = Console.ReadLine();
                    try
                    {
                        File.Delete(@"0:\" + inputaman);
                        log(ConsoleColor.Blue, "GoOS Admin: File Deleted!");
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "Please send the following to GoOS Developers");
                        log(ConsoleColor.Red, e.ToString());
                    }
                }
            }
            else if (input.Equals("run", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: Enter file name");
                    textcolour(ConsoleColor.Yellow);
                    write("FilePath: 0:\\");
                    String inputaman = Console.ReadLine();
                    try
                    {
                        log(ConsoleColor.Blue, "GoOS Admin: Attempting to run " + inputaman);
                        if (!inputaman.EndsWith(".goexe"))
                        {
                            log(ConsoleColor.Red, "GoOS Admin: Incompatible format.");
                            log(ConsoleColor.Red, "GoOS Admin: File must be .goexe");
                        }
                        if (inputaman.EndsWith(".goexe"))
                        {
                            log(ConsoleColor.Yellow, "Application.Start");
                            var content = File.ReadAllLines(@"0:\" + inputaman);
                            string theysaid = null;
                            ConsoleKey keypressed = ConsoleKey.O;
                            int count = 1;
                            String a = null;
                            String b = null;
                            String c = null;
                            String d = null;
                            String e = null;
                            String f = null;
                            String g = null;
                            String h = null;
                            String i = null;
                            String j = null;
                            foreach (string line in content)
                            {
                                count = count + 1;
                                //log(ConsoleColor.Magenta, "LINE FOUND: CONTENT: " + line);
                                if (line.StartsWith("#"))
                                {

                                }
                                if (line.StartsWith(""))
                                {

                                }
                                if (line.StartsWith("print"))
                                {
                                    string thingtosay = line.Replace("print=", "");
                                    thingtosay = thingtosay.Replace("{getInput}", theysaid);
                                    thingtosay = thingtosay.Replace("{1}", a);
                                    thingtosay = thingtosay.Replace("{2}", b);
                                    thingtosay = thingtosay.Replace("{3}", c);
                                    thingtosay = thingtosay.Replace("{4}", d);
                                    thingtosay = thingtosay.Replace("{5}", e);
                                    thingtosay = thingtosay.Replace("{6}", f);
                                    thingtosay = thingtosay.Replace("{7}", g);
                                    thingtosay = thingtosay.Replace("{8}", h);
                                    thingtosay = thingtosay.Replace("{9}", i);
                                    thingtosay = thingtosay.Replace("{10}", j);
                                    log(ConsoleColor.Magenta, thingtosay);
                                }
                                if (line.StartsWith("sleep"))
                                {
                                    String howlong = line.Split("=")[1];
                                    int potato = Convert.ToInt32(howlong);
                                    sleep(potato);
                                }
                                if (line.StartsWith("input"))
                                {
                                    if (line == "input=")
                                    {
                                        textcolour(ConsoleColor.Blue);
                                        theysaid = Console.ReadLine();
                                    }
                                    else
                                    {
                                        String addon = line.Replace("input=", "");
                                        write(addon);
                                        textcolour(ConsoleColor.Blue);
                                        theysaid = Console.ReadLine();
                                    }

                                }
                                if (line.StartsWith("stop"))
                                {
                                    if (line == "stop=")
                                    {
                                        textcolour(ConsoleColor.Blue);
                                        log(ConsoleColor.Green, "Press any key to continue...");
                                        Console.ReadKey();
                                        Console.WriteLine();
                                    }
                                    else
                                    {
                                        String addon = line.Replace("stop=", "");
                                        textcolour(ConsoleColor.DarkRed);
                                        write(addon);
                                        textcolour(ConsoleColor.Blue);
                                        Console.ReadKey();
                                        Console.WriteLine();
                                    }

                                }
                                if (line.StartsWith("if"))
                                {
                                    //version by GoOS God
                                    //i actually dont know
                                    if (line.Split("=")[1] == "1" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "2" || line.Split("=")[1] == "2" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "1")
                                    {
                                        if (a == b)
                                        {
                                            log(ConsoleColor.Magenta, line.Split("=")[4]);
                                        }
                                        if (a != b)
                                        {

                                            if (line.Split("=")[5].Equals("end"))
                                            {

                                                break;
                                            }
                                            log(ConsoleColor.Magenta, line.Split("=")[5]);

                                        }

                                    }
                                    if (line.Split("=")[1] == "3" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "4" || line.Split("=")[1] == "4" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "3")
                                    {
                                        if (c == d)
                                        {
                                            log(ConsoleColor.Magenta, line.Split("=")[4]);
                                        }
                                        if (c != d)
                                        {

                                            if (line.Split("=")[5].Equals("end"))
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                log(ConsoleColor.Magenta, line.Split("=")[5]);
                                            }
                                        }

                                    }
                                    if (line.Split("=")[1] == "5" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "6" || line.Split("=")[1] == "6" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "5")
                                    {
                                        if (e == f)
                                        {
                                            log(ConsoleColor.Magenta, line.Split("=")[4]);
                                        }
                                        if (e != f)
                                        {

                                            if (line.Split("=")[5].Equals("end"))
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                log(ConsoleColor.Magenta, line.Split("=")[5]);
                                            }
                                        }

                                    }
                                    if (line.Split("=")[1] == "7" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "8" || line.Split("=")[1] == "8" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "7")
                                    {
                                        if (g == h)
                                        {
                                            log(ConsoleColor.Magenta, line.Split("=")[4]);
                                        }
                                        if (g != h)
                                        {

                                            if (line.Split("=")[5].Equals("end"))
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                log(ConsoleColor.Magenta, line.Split("=")[5]);
                                            }
                                        }

                                    }
                                    if (line.Split("=")[1] == "9" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "10" || line.Split("=")[1] == "10" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "9")
                                    {
                                        if (i == j)
                                        {
                                            log(ConsoleColor.Magenta, line.Split("=")[4]);
                                        }
                                        if (i != j)
                                        {

                                            if (line.Split("=")[5].Equals("end"))
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                log(ConsoleColor.Magenta, line.Split("=")[5]);
                                            }
                                        }

                                    }

                                }
                                if (line.StartsWith("variable"))
                                {
                                    if (line.Split("=")[1] == "1")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            a = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "2")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            b = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "3")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            c = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "4")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            d = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "5")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            e = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "6")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            f = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "7")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            g = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "8")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            h = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "9")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            i = gethandled;
                                        }
                                    }
                                    if (line.Split("=")[1] == "10")
                                    {
                                        if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                        {
                                            log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                            log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                            break;
                                        }
                                        else
                                        {
                                            String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                            j = gethandled;
                                        }
                                    }

                                }
                                if (line.StartsWith("randomnum"))
                                {
                                    String Num1 = line.Split("=")[1];
                                    String Num2 = line.Split("=")[2];
                                    String varstore = line.Split("=")[3];
                                    int Num1int = Convert.ToInt32(Num1);
                                    int Num2int = Convert.ToInt32(Num2);
                                    if (varstore == "1")
                                    {
                                        a = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "2")
                                    {
                                        b = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "3")
                                    {
                                        c = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "4")
                                    {
                                        d = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "5")
                                    {
                                        e = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "6")
                                    {
                                        f = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "7")
                                    {
                                        g = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "8")
                                    {
                                        h = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "9")
                                    {
                                        i = rnd.Next(Num1int, Num2int).ToString();
                                    }
                                    if (varstore == "10")
                                    {
                                        j = rnd.Next(Num1int, Num2int).ToString();
                                    }

                                }
                                if (line.StartsWith("clear"))
                                {
                                    Console.Clear();
                                }

                            }
                            log(ConsoleColor.Yellow, "Application.exit");

                        }
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "GoOS Admin has killed this program as an error has occoured");
                        log(ConsoleColor.Red, "Report this to the app developer or the GoOS Devs for assistance.");
                        log(ConsoleColor.Red, "Screenshot this stack trace:");
                        log(ConsoleColor.Red, e.ToString());
                    }
                }
            }
            else if (input.Equals("ld", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    var label = FS.GetFileSystemLabel(@"0:\");
                    log(ConsoleColor.Red, "GoOS Admin: Relabel disk");
                    log(ConsoleColor.Red, "GoOS Admin: Press ENTER to leave the label as \"" + label + "\"");
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
                        log(ConsoleColor.Blue, "GoOS Admin: Drive Label modified from " + label + " to " + inputamana);
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "Please send the following to GoOS Developers");
                        log(ConsoleColor.Red, e.ToString());
                    }
                }
            }
            else if (input.Equals("ftp", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    //using (var xServer = new FtpServer(FS, "0:\\"))
                    //{
                    /** Listen for new FTP client connections **/
                    // this does not work
                    //     log(ConsoleColor.Blue, "GoOS Admin: Listening on " + NetworkConfiguration.CurrentAddress.ToString() + ":21");
                    //     log(ConsoleColor.Blue, "Use PLAIN configurations with no login information.");
                    //     log(ConsoleColor.Blue, "FTP MODE ENABLED. REBOOT TO DISABLE");
                    //     xServer.Listen();
                    //}
                }
            }
            else if (input.Equals("ipconf", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Red, "GoOS Admin: Showing Internet Information");
                log(ConsoleColor.Red, NetworkConfiguration.CurrentAddress.ToString());
            }

            //smth cool bro


            else if (input.Equals("root", StringComparison.OrdinalIgnoreCase))
            {
                //Root function activator. do not disable modes without enabling other modes.
                print("");
                write("Root Password:");
                String passinpt = Console.ReadLine();
                //if (passinpt == rootpassword)
                //{
                //    cmdm = false;
                //    root = true;
                //}
                //else
                //{
                ///    Console.Clear();
                //    log(ConsoleColor.Red, "GoOS Admin: Password incorrect.");
                //}
            }

            else if (input.Equals("gogetAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", StringComparison.OrdinalIgnoreCase))
            {
                textcolour(ConsoleColor.Yellow);
                write("What app do you want to install: ");
                textcolour(ConsoleColor.White);
                String filetoget = Console.ReadLine();
                textcolour(ConsoleColor.Green);
                using (var xClient = new TcpClient(39482))
                {
                    xClient.Connect(new Address(135, 125, 172, 225), 80);
                    //135.125.172.225

                    /** Send data **/
                    xClient.Send(Encoding.ASCII.GetBytes("GET /" + filetoget + ".goexe HTTP/1.1\nHost: ubnserver.owen2k6.com\n\n"));

                    /** Receive data **/
                    var endpoint = new EndPoint(Address.Zero, 0);
                    var data = xClient.Receive(ref endpoint);  //set endpoint to remote machine IP:port
                    var data2 = xClient.NonBlockingReceive(ref endpoint); //retrieve receive buffer without waiting
                    string bitString = BitConverter.ToString(data2);
                    File.Create(@"0:\" + filetoget + ".goexe");
                    File.WriteAllText(@"0:\" + filetoget + ".goexe", bitString);
                    print(bitString);


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
                log(ConsoleColor.Green, "");
                log(ConsoleColor.Magenta, "Type HELP for a list of commands");
            }
            textcolour(ConsoleColor.Green);
        }


        protected void Root()
        {
            textcolour(ConsoleColor.Red);
            write("GoOS Admin:");
            textcolour(ConsoleColor.Gray);
            String input = Console.ReadLine();
            if (input.Equals("goos.cmd.list", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.DarkRed, "GoOS Root Commands:");
                log(ConsoleColor.DarkRed, "WHITE = SAFE");
                log(ConsoleColor.DarkRed, "RED = DANGEROUS");
                log(ConsoleColor.Yellow, "Password Security -");
                log(ConsoleColor.White, "GOOS.ROOT.SECURITY.PASSWORD.CHANGE");
                log(ConsoleColor.White, "GOOS.ROOT.SECURITY.PASSWORD");
                log(ConsoleColor.White, "GOOS.SECURITY.PASSWORD.CHANGE");
                log(ConsoleColor.White, "GOOS.SECURITY.PASSWORD.REMOVE");
                log(ConsoleColor.White, "GOOS.SECURITY.PASSWORD");
                log(ConsoleColor.Yellow, "Disk Commands -");
                log(ConsoleColor.White, "GOOS.DISK.RELABEL");
                log(ConsoleColor.White, "GOOS.DISK.SCAN");
                log(ConsoleColor.White, "GOOS.DISK.LISTALL");
                log(ConsoleColor.Red, "GOOS.DISK.FORMAT");
                log(ConsoleColor.Yellow, "Root Settings -");
                log(ConsoleColor.White, "GOOS.ROOT.EXIT");
            }
            else if (input.Equals("goos.root.security.password.change", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Red, "This feature is unavailable at the current time.");
                log(ConsoleColor.Red, "edit passwordsystem.goplexsecure in to change passwords for now.");

            }
            else if (input.Equals("goos.root.security.password.remove", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Red, "This feature is unavailable at the current time.");
                log(ConsoleColor.Red, "edit passwordsystem.goplexsecure in to change passwords for now.");
            }
            else if (input.Equals("goos.security.password.change", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Red, "This feature is unavailable at the current time.");
                log(ConsoleColor.Red, "edit passwordsystem.goplexsecure in to change passwords for now.");
            }
            else if (input.Equals("goos.security.password.remove", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Red, "This feature is unavailable at the current time.");
                log(ConsoleColor.Red, "edit passwordsystem.goplexsecure in to change passwords for now.");
            }
            else if (input.Equals("goos.security.password", StringComparison.OrdinalIgnoreCase))
            {
                log(ConsoleColor.Red, "Requesting data.");
                //var grabpass = password;
                log(ConsoleColor.Red, "Data Recieved.");
                //print(grabpass);
            }
            else if (input.Equals("goos.disk.relabel", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    var label = FS.GetFileSystemLabel(@"0:\");
                    log(ConsoleColor.Red, "GoOS Admin: Relabel disk");
                    log(ConsoleColor.Red, "GoOS Admin: Press ENTER to leave the label as \"" + label + "\"");
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
                        log(ConsoleColor.Blue, "GoOS Admin: Drive Label modified from " + label + " to " + inputamana);
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "Please send the following to GoOS Developers");
                        log(ConsoleColor.Red, e.ToString());
                    }
                }
            }
            else if (input.Equals("goos.disk.scan", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    try
                    {
                        log(ConsoleColor.Red, "GoOS Admin: Showing Disk Information for 0:\\");
                        var available_space = FS.GetAvailableFreeSpace(@"0:\");
                        var total_space = FS.GetTotalSize(@"0:\");
                        var label = FS.GetFileSystemLabel(@"0:\");
                        var fs_type = FS.GetFileSystemType(@"0:\");
                        log(ConsoleColor.Red, "Available Free Space: " + available_space + "(" + (available_space / 1e+9) + "GiB)");
                        log(ConsoleColor.Red, "Total Space on disk: " + total_space + "(" + (total_space / 1e+9) + "GiB)");
                        log(ConsoleColor.Red, "Disk Label: " + label);
                        log(ConsoleColor.Red, "File System Type: " + fs_type);
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                        log(ConsoleColor.Red, "GoOS Admin: For system security, we have disabled all Drive functions.");
                        adminconsoledisk = false;
                    }
                }
            }
            else if (input.Equals("goos.disk.listall", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    try
                    {
                        var directory_list_directories = Directory.GetDirectories(@"0:\");
                        foreach (var folder in directory_list_directories)
                        {
                            log(ConsoleColor.Red, folder);
                        }
                        var directory_list = Directory.GetFiles(@"0:\");
                        foreach (var file in directory_list)
                        {
                            log(ConsoleColor.Red, file);
                        }
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                        log(ConsoleColor.Red, "GoOS Admin: For system security, we have disabled all Drive functions.");
                        adminconsoledisk = false;
                    }
                }
            }
            else if (input.Equals("goos.disk.format", StringComparison.OrdinalIgnoreCase))
            {
                if (!adminconsoledisk)
                {
                    log(ConsoleColor.Red, "GoOS Admin: There is currently no disk loaded to the system.");
                }
                if (adminconsoledisk)
                {
                    try
                    {
                        bool proceed = false;
                        bool hasentered = false;
                        while (!hasentered)
                        {
                            write("Are you sure you want to format drive 0:\\?");
                            string inputer = Console.ReadLine();
                            if (inputer.Equals("yes", StringComparison.OrdinalIgnoreCase))
                            {
                                log(ConsoleColor.Red, "ARE YOU SURE?");
                                string inputer2 = Console.ReadLine();
                                if (inputer2.Equals("yes", StringComparison.OrdinalIgnoreCase))
                                {
                                    proceed = true;
                                    hasentered = true;
                                }
                                if (inputer2.Equals("no", StringComparison.OrdinalIgnoreCase))
                                {
                                    print("aborted.");
                                    hasentered = true;
                                }
                            }
                            if (inputer.Equals("no", StringComparison.OrdinalIgnoreCase))
                            {
                                print("aborted.");
                                hasentered = true;

                            }
                        }
                        if (proceed)
                        {
                            try
                            {
                                var directory_list = Directory.GetFiles(@"0:\");
                                foreach (var file in directory_list)
                                {
                                    File.Delete(@"0:\" + file);
                                }
                            }
                            catch (Exception e)
                            {
                                log(ConsoleColor.Red, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                                log(ConsoleColor.Red, "GoOS Admin: For system security, we have disabled all Drive functions.");
                                adminconsoledisk = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log(ConsoleColor.Red, "GoOS Admin: Error Loading disk! You might have disconnected the drive!");
                        log(ConsoleColor.Red, "GoOS Admin: For system security, we have disabled all Drive functions.");
                        adminconsoledisk = false;
                    }
                }
            }
            else if (input.Equals("goos.root.exit", StringComparison.OrdinalIgnoreCase))
            {
                root = false;
                cmdm = true;
            }
            else if (input.Equals("goos.test.idk", StringComparison.OrdinalIgnoreCase))
            {

            }
        }

    }


}


