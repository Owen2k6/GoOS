using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.ConsoleColor;
using Sys = Cosmos.System;
using GoOS.Virtualisation;
using static ChaOS.Core;
using static ChaOS.DiskManager;
using Cosmos.System.FileSystem.VFS;
using System.Runtime.ConstrainedExecution;
using static GoOS.Kernel;
using GoOS.Commands;
using GoOS.Themes;
using GoOS.Virtualisation.ChaOS;
using static GoOS.Commands.Run;
namespace GoOS.Virtualisation.ChaOS
{
    public class ChaOS
    {
        public static bool runmode = false;

        public const string ver = "Version 1.3 GoOS VM";
        public const string copyright = "Copyright (c) 2022 Goplex Studios";

        public static string username = "usr";

        public static string input;
        public static string inputBeforeLower;
        public static string inputCapitalized;
        public static string input1;
        public static string inputBeforeLower1;
        public static string inputCapitalized1;

        public static string rootdir = @"0:\content\vrt\ChaOS\";


        public static void boot(string rootpath)
        {
            //BeforeRun bullshit goes here

            try
            {
                //log("Starting up ChaOS...");

                Console.Clear();
                log("Welcome to...\n");
                clog("  ______   __                   ______    ______  \n /      \\ |  \\                 /      \\  /      \\ \n|  $$$$$$\\| $$____    ______  |  $$$$$$\\|  $$$$$$\\\n| $$   \\$$| $$    \\  |      \\ | $$  | $$| $$___\\$$\n| $$      | $$$$$$$\\  \\$$$$$$\\| $$  | $$ \\$$    \\ \n| $$   __ | $$  | $$ /      $$| $$  | $$ _\\$$$$$$\\\n| $$__/  \\| $$  | $$|  $$$$$$$| $$__/ $$|  \\__| $$\n \\$$    $$| $$  | $$ \\$$    $$ \\$$    $$ \\$$    $$\n  \\$$$$$$  \\$$   \\$$  \\$$$$$$$  \\$$$$$$   \\$$$$$$ ", DarkGreen);
                log("\n" + ver + "\n" + copyright + "\nType \"help\" to get started!");
                //if (!disk) log("No hard drive detected, ChaOS will continue without disk support.");
                log();
            }
            catch (Exception ex) { Crash(ex, true); }

            runmode = true;
            run(rootpath);
        }
        public static void run(string rtp)
        {
            runmode = true;

            while (runmode == true)
            {
                //run contents in here

                var CanContinue = true;

                try
                {
                    if (!Directory.GetCurrentDirectory().StartsWith(rootdir)) Directory.SetCurrentDirectory(rootdir); // Directory error correction
                    string truedir = Directory.GetCurrentDirectory();
                    string falsedir = @"0:\";
                    if (truedir.Contains(@"0:\content\vrt\ChaOS\"))
                    {
                        falsedir = truedir.Replace(@"0:\content\vrt\ChaOS\", @"0:\");
                    }
                    if (disk) Console.Write(username + " (" + falsedir + "): ");
                    else Console.Write(username + " > ");

                    inputBeforeLower = Console.ReadLine();
                    /*(if (inputBeforeLower1.Contains(@"0:\"))
                    {
                        inputBeforeLower = inputBeforeLower1.Replace(@"0:\", @"0:\content\vrt\ChaOS\");
                    }*/
                    inputCapitalized = inputBeforeLower.ToUpper(); // Input converted to uppercase
                    input = inputBeforeLower.ToLower().Trim();     // Input converted to lowercase

                    String[] args = inputBeforeLower.Split(' ');
                    log();


                    if (input.StartsWith("help"))
                    {
                        var us = string.Empty;
                        var color = Console.ForegroundColor;
                        if (!disk) { us = " (unavailable)"; color = Gray; }

                        clog("Functions:", DarkGreen);
                        log(" help - Shows all functions, do \"help (page)\" for more commands");
                        log(" credits - Shows all of the wonderful people that make ChaOS work");
                        log(" cls/clear - Clears the screen");
                        log(" time - Tells you the time");
                        log(" echo - Echoes what you say");
                        log(" calc - Allows you to do simple math");
                        log(" sysinfo - Gives info about the system");
                        log(" username - Username related functions");
                        log(" color - Allows you to change the color scheme");
                        log(" sd/shutdown/stop - Shuts down ChaOS");
                        log(" rb/reboot - Reboots the system");
                        clog(" diskinfo - Gives info about the disk" + us, color);
                        clog(" cd - Opens directory" + us, color);
                        clog(" cd.. - Opens last directory" + us, color);
                        clog(" dir - Lists files in the current directory" + us, color);
                        clog(" mkdir - Creates a directory" + us, color);
                        clog(" mkfile - Creates a file" + us, color);
                        clog(" deldir - Deletes a directory" + us, color);
                        clog(" delfile - Deletes a file" + us, color);
                        clog(" lb - Relabels the disk" + us, color);
                        clog(" notepad - Launches MIV (Minimalistic Vi)" + us + "\n", color);
                    }

                    #region Username functions
                    else if (input.StartsWith("username"))
                    {
                        if (input.Contains("set"))
                        {
                            try { username = input.Split("set ")[1].Trim(); } catch { clog("No arguments\n", Red); }
                            clog("Done! (" + username + ")\n", Yellow);
                        }
                        else if (input.EndsWith("current")) clog("Current username: " + username + "\n", Yellow);
                        else
                        {
                            clog("Username subfunctions:", DarkGreen);
                            log(" username set (username) - Changes the username");
                            log(" username current - Displays current username\n");
                        }
                    }
                    #endregion

                    #region Color functions
                    else if (input.Contains("color"))
                    {
                        if (input.EndsWith("list"))
                        {
                            var OldBack = Console.BackgroundColor; var OldFore = Console.ForegroundColor;
                            clog("Color list:", Green);
                            Console.Write(" "); SetScreenColor(White, Black, false); Console.Write("black - Pure light mode, will make you blind"); SetScreenColor(OldBack, OldFore, false);
                            clog("\n dark blue - Dark blue with black background", DarkBlue);
                            clog(" dark green - Dark green with black background", DarkGreen);
                            clog(" dark cyan - Dark cyan with black background", DarkCyan);
                            clog(" dark gray - Dark gray with black background", DarkGray);
                            clog(" blue - Normal blue with black background", Blue);
                            clog(" green - Green with black background", Green);
                            clog(" cyan - Cyan with black background", Cyan);
                            clog(" dark red - Dark red with black background", DarkRed);
                            clog(" dark magenta - Dark magenta with black background", DarkMagenta);
                            clog(" dark yellow - Dark yellow/orange with black background", DarkYellow);
                            clog(" gray - Gray with black background", Gray);
                            clog(" red - Red with black background", Red);
                            clog(" magenta - Magenta with black background", Magenta);
                            clog(" yellow - Light yellow with black background", Yellow);
                            clog(" white - Pure white with black background\n", White);
                        } // "Cosmos is built on else if blocks"
                        else if (input.EndsWith("black")) SetScreenColor(White, Black);
                        else if (input.EndsWith("dark blue")) SetScreenColor(Black, DarkBlue);
                        else if (input.EndsWith("dark green")) SetScreenColor(Black, DarkGreen);
                        else if (input.EndsWith("dark cyan")) SetScreenColor(Black, DarkCyan);
                        else if (input.EndsWith("dark gray")) SetScreenColor(Black, DarkGray);
                        else if (!input.Contains("dark") && input.EndsWith("blue")) SetScreenColor(Black, Blue);
                        else if (!input.Contains("dark") && input.EndsWith("green")) SetScreenColor(Black, Green);
                        else if (!input.Contains("dark") && input.EndsWith("cyan")) SetScreenColor(Black, Cyan);
                        else if (input.EndsWith("dark red")) SetScreenColor(Black, DarkRed);
                        else if (input.EndsWith("dark magenta")) SetScreenColor(Black, DarkMagenta);
                        else if (input.EndsWith("dark yellow")) SetScreenColor(Black, DarkYellow);
                        else if (!input.Contains("dark") && input.EndsWith("gray")) SetScreenColor(Black, Gray);
                        else if (!input.Contains("dark") && input.EndsWith("red")) SetScreenColor(Black, Red);
                        else if (!input.Contains("dark") && input.EndsWith("magenta")) SetScreenColor(Black, Magenta);
                        else if (!input.Contains("dark") && input.EndsWith("yellow")) SetScreenColor(Black, Yellow);
                        else if (input.EndsWith("white")) SetScreenColor(Black, White);
                        else clog("Please list colors by doing \"opt color list\" or set a color by doing \"opt color (color)\"\n", Gray);
                    }
                    #endregion

                    else if (input == "credits")
                    {
                        Console.Clear();
                        Console.SetCursorPosition(0, 1);
                        cwrite("  ______   __                   ______    ______  \n /      \\ |  \\                 /      \\  /      \\ \n|  $$$$$$\\| $$____    ______  |  $$$$$$\\|  $$$$$$\\\n| $$   \\$$| $$    \\  |      \\ | $$  | $$| $$___\\$$\n| $$      | $$$$$$$\\  \\$$$$$$\\| $$  | $$ \\$$    \\ \n| $$   __ | $$  | $$ /      $$| $$  | $$ _\\$$$$$$\\\n| $$__/  \\| $$  | $$|  $$$$$$$| $$__/ $$|  \\__| $$\n \\$$    $$| $$  | $$ \\$$    $$ \\$$    $$ \\$$    $$\n  \\$$$$$$  \\$$   \\$$  \\$$$$$$$  \\$$$$$$   \\$$$$$$ ", DarkGreen);
                        Console.SetCursorPosition(55, 3);
                        cwrite("Version 1.3 GoOS VM", Yellow);
                        Console.SetCursorPosition(62, 5);
                        cwrite("Credits", Yellow);
                        Console.SetCursorPosition(57, 6);
                        Console.Write("ekeleze - Creator");
                        Console.SetCursorPosition(60, 7);
                        Console.Write("xrc2 - Contributor");
                        Console.SetCursorPosition(20, 11);
                        Console.Write("Special thanks:");
                        Console.SetCursorPosition(1, 13);
                        Console.Write("Owen2k6 - For allowing ChaOS to live on through GoOS VM");
                        Console.SetCursorPosition(0, 24);
                    }

                    else if (input == "clear" || input == "cls")
                        Console.Clear();

                    else if (input == "time")
                    {
                        string Hour = Cosmos.HAL.RTC.Hour.ToString(); string Minute = Cosmos.HAL.RTC.Minute.ToString();
                        if (Minute.Length < 2) Minute = "0" + Minute;
                        clog("Current time is " + Hour + ":" + Minute + "\n", Yellow);
                    }

                    else if (input == "notepad" && disk)
                        MIV.StartMIV(args[1]);

                    else if (input == "shutdown" || input == "sd" || input == "stop")
                    {
                        if (disk) SaveChangesToDisk();
                        clog("Shutting down...", Gray);
                        runmode = false;
                    }

                    else if (input.StartsWith("echo"))
                        clog(inputBeforeLower.Split("echo ")[1] + "\n", Gray);

                    else if (input.StartsWith("calc"))
                    {
                        int result = 0; input = input.Remove(0, 5); input = input.Trim();
                        if (input.Contains('+')) result = Convert.ToInt32(input.Split('+')[0]) + Convert.ToInt32(input.Split('+')[1]);
                        else if (input.Contains('-')) result = Convert.ToInt32(input.Split('-')[0]) - Convert.ToInt32(input.Split('-')[1]);
                        else if (input.Contains('*')) result = Convert.ToInt32(input.Split('*')[0]) * Convert.ToInt32(input.Split('*')[1]);
                        else if (input.Contains('/')) result = Convert.ToInt32(input.Split('/')[0]) / Convert.ToInt32(input.Split('/')[1]);
                        else if (input.EndsWith("calc"))
                        {
                            clog("Calculator subfunctions", DarkGreen);
                            log(" + - Adds numbers");
                            log(" - - Subtracts numbers");
                            log(" * - Multiplies numbers");
                            log(" / - Divides numbers");
                            log("\nSyntax: calc (num)(func)(num)\n");
                        }
                        if (result != 0) clog("Result: " + result + "\n", Gray);
                    }

                    else if (input == "sysinfo")
                    {
                        clog("System info:", DarkGreen);
                        log(" CPU: " + Cosmos.Core.CPU.GetCPUBrandString());
                        log(" CPU speed: " + Cosmos.Core.CPU.GetCPUCycleSpeed() / 1e6 + "MHz");
                        log(" System RAM: " + Cosmos.Core.CPU.GetAmountOfRAM() + "MiB used out of " + Cosmos.Core.GCImplementation.GetAvailableRAM() + "\n");
                    }

                    else if (input == "diskinfo")
                    {
                        long availableSpace = VFSManager.GetAvailableFreeSpace(@"0:\");
                        long diskSpace = VFSManager.GetTotalSize(@"0:\");
                        string fsType = VFSManager.GetFileSystemType("0:\\");
                        clog("Disk info for " + FS.GetFileSystemLabel(rootdir), DarkGreen);
                        if (diskSpace < 1e6) log(" Disk space: " + availableSpace / 1e3 + " KB free out of " + diskSpace / 1000 + " KB total");
                        else if (diskSpace > 1e6) log(" Disk space: " + availableSpace / 1e6 + " MB free out of " + diskSpace / 1e+6 + " MB total");
                        else if (diskSpace > 1e9) log(" Disk space: " + availableSpace / 1e9 + " GB free out of " + diskSpace / 1e+9 + " GB total");
                        log(" Filesystem type: " + fsType + "\n");
                    }

                    #region Disk commands

                    else if (input.StartsWith("mkdir"))
                    {
                        try { inputBeforeLower = inputBeforeLower.Split("mkdir ")[1]; } catch { clog("No arguments\n", Red); CanContinue = false; }
                        if (inputBeforeLower.Contains("0:\\")) { inputBeforeLower.Replace("0:\\", ""); }
                        if (inputBeforeLower.Contains(" ")) { clog("Directory name cannot contain spaces!\n", Red); CanContinue = false; }
                        if (CanContinue)
                        {
                            if (!Directory.Exists(inputCapitalized))
                                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\" + inputCapitalized);
                            else
                                clog("Directory already exists!\n", Red);
                        }
                    }

                    else if (input.StartsWith("mkfile"))
                    {
                        try { inputBeforeLower = inputBeforeLower.Split("mkfile ")[1]; } catch { clog("No arguments\n", Red); CanContinue = false; }
                        if (inputBeforeLower.Contains("0:\\")) { input.Replace("0:\\", ""); }
                        if (inputBeforeLower.Contains(" ")) { clog("Filename cannot contain spaces!\n", Red); CanContinue = false; }
                        if (CanContinue)
                        {
                            if (!File.Exists(inputBeforeLower))
                                File.Create(Directory.GetCurrentDirectory() + @"\" + inputBeforeLower);
                            else
                                clog("File already exists!\n", Red);
                        }
                    }

                    else if (input.StartsWith("deldir"))
                    {
                        try { inputBeforeLower = inputBeforeLower.Split("deldir ")[1]; } catch { clog("No arguments\n", Red); CanContinue = false; }
                        if (inputBeforeLower.Contains("0:\\")) { input.Replace("0:\\", ""); }
                        if (inputBeforeLower.Contains(" ")) { clog("Filename cannot contain spaces!\n", Red); CanContinue = false; }
                        if (CanContinue)
                        {
                            if (Directory.Exists(inputBeforeLower))
                                Directory.Delete(Directory.GetCurrentDirectory() + @"\" + inputBeforeLower, true);
                            else
                                clog("Directory not found!\n", Red);
                        }
                    }

                    else if (input.StartsWith("delfile"))
                    {
                        try { inputBeforeLower = inputBeforeLower.Split("delfile ")[1]; } catch { clog("No arguments\n", Red); CanContinue = false; }
                        if (inputBeforeLower.Contains("0:\\")) { input.Replace("0:\\", ""); }
                        if (inputBeforeLower.Contains(" ")) { clog("Filename cannot contain spaces!\n", Red); CanContinue = false; }
                        if (CanContinue)
                        {
                            if (File.Exists(inputBeforeLower))
                                File.Delete(Directory.GetCurrentDirectory() + @"\" + inputBeforeLower);
                            else
                                clog("File not found!\n", Red);
                        }
                    }

                    else if (input.StartsWith("cd") && disk)
                    {
                        if (input == "cd..")
                        {
                            try
                            {
                                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory().TrimEnd('\\').Remove(Directory.GetCurrentDirectory().LastIndexOf('\\') + 1));
                                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 1));
                            }
                            catch { }
                        }

                        else if (input.StartsWith("cd "))
                        {
                            try { inputBeforeLower = inputBeforeLower.Split("cd ")[1]; } catch { clog("No arguments\n", Red); CanContinue = false; }
                            if (inputBeforeLower.Trim() != string.Empty) CanContinue = true;
                            if (CanContinue)
                            {
                                if (inputBeforeLower.Contains(@"0:\")) { inputBeforeLower.Replace(@"0:\", ""); }
                                if (Directory.GetCurrentDirectory() != rootdir) { inputBeforeLower = @"\" + inputBeforeLower; }
                                if (Directory.Exists(Directory.GetCurrentDirectory() + inputBeforeLower))
                                    Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + inputBeforeLower);
                                else clog("Directory not found!\n", Red);
                            }
                        }

                        else
                        {
                            clog("Cd subfunctions:", DarkGreen);
                            log(" cd (path) - Browses to directory");
                            log(" cd.. - Browses to last directory\n");
                        }
                    }

                    else if (input == "dir" && disk)
                    {
                        clog("Directory listing at " + falsedir, Yellow);
                        var directoryList = VFSManager.GetDirectoryListing(Directory.GetCurrentDirectory());
                        var files = 0; var dirs = 0;
                        foreach (var directoryEntry in directoryList)
                        {
                            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + directoryEntry.mName))
                                clog("<Dir> " + directoryEntry.mName, Gray); dirs += 1;
                        }
                        foreach (var directoryEntry in directoryList)
                        {
                            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + directoryEntry.mName))
                                clog("<File> " + directoryEntry.mName, Gray); files += 1;
                        }
                        clog("\nFound " + files + " files and " + dirs + " directories.\n", Yellow);
                    }

                    else if (input.StartsWith("copy"))
                    {
                        var potato = string.Empty; var potato1 = string.Empty;
                        try { potato = inputBeforeLower.Split(" ")[1]; potato1 = inputBeforeLower.Split(" ")[2]; } catch { clog("No arguments\n", Red); CanContinue = false; }
                        if (CanContinue)
                        {
                            var Contents = File.ReadAllText(potato);
                            File.Create(potato1);
                            File.WriteAllText(potato1, Contents);
                            clog("Copy process finished successfully!\n", Gray);
                        }
                    }

                    else if (input.StartsWith("lb") && disk)
                        FS.SetFileSystemLabel(rootdir, inputBeforeLower.Split("lb ")[1]);

                    #endregion

                    else
                    {
                        Console.Beep();
                        clog("Unknown command.\n", Red);
                    }
                }
                catch (Exception ex) { Crash(ex); }
            }

        }
    }
}
