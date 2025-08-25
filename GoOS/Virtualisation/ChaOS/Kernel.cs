using System;
using System.IO;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using Gold.Graphics;
using GoOS.GUI.Apps;
using GoOS.GUI.Apps.Terminal;
using GoOS.Virtualisation.ChaOS.Core;
using GoOS.Virtualisation.ChaOS.Misc.Apps;
using static GoOS.Kernel;
using static Gold.Graphics.Color;

namespace GoOS.Virtualisation.ChaOS
{
    public class Kernel : VM
    {
        public new string name = "ChaOS";

        public const string ver = "Version 1.4";
        public const string copyright = "Copyright (c) 2023 imperium";

        public string username = "usr";

        public string input;
        public string inputBeforeLower;
        public string inputCapitalized;

        public static string rootdir = @"0:\content\vrt\ChaOS\";

        public Shell Terminal;
        public SVGAIITerminal Console;

        public ChaOS.Core.Core core;
        private ChaOS.Core.DiskManager diskMan;

        public Kernel(Shell terminal) : base(terminal)
        {
            Terminal = terminal;
            Console = terminal._terminal;

            core = new Core.Core(this);
            diskMan = new DiskManager(this);
        }

        protected override void BeforeRun()
        {
            try
            {
                Console.Clear();
                core.log("Welcome to...\n");
                core.clog("  ______   __                   ______    ______  \n /      \\ |  \\                 /      \\  /      \\ \n|  $$$$$$\\| $$____    ______  |  $$$$$$\\|  $$$$$$\\\n| $$   \\$$| $$    \\  |      \\ | $$  | $$| $$___\\$$\n| $$      | $$$$$$$\\  \\$$$$$$\\| $$  | $$ \\$$    \\ \n| $$   __ | $$  | $$ /      $$| $$  | $$ _\\$$$$$$\\\n| $$__/  \\| $$  | $$|  $$$$$$$| $$__/ $$|  \\__| $$\n \\$$    $$| $$  | $$ \\$$    $$ \\$$    $$ \\$$    $$\n  \\$$$$$$  \\$$   \\$$  \\$$$$$$$  \\$$$$$$   \\$$$$$$ ", Green);
                core.log("\n" + ver + "\n" + copyright + "\nType \"help\" to get started!");
                core.log();
            }
            catch (Exception ex) { core.Crash(ex, true); }
        }

        protected override void Run()
        {
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
                if (diskMan.disk) Console.Write(username + " (" + falsedir + "): ");
                else Console.Write(username + " > ");

                inputBeforeLower = Console.ReadLine();
                inputCapitalized = inputBeforeLower.ToUpper(); // Input converted to uppercase
                input = inputBeforeLower.ToLower().Trim();     // Input converted to lowercase

                core.log();

                if (input.StartsWith("help"))
                {
                    var us = string.Empty;
                    var color = Console.ForegroundColor;
                    if (!diskMan.disk) { us = " (unavailable)"; color = LightGray; }

                    core.clog("Functions:", Green);
                    core.log(" help - Shows all functions, do \"help (page)\" for more commands");
                    core.log(" credits - Shows all of the wonderful people that make ChaOS work");
                    core.log(" cls/clear - Clears the screen");
                    core.log(" time - Tells you the time");
                    core.log(" echo - Echoes what you say");
                    core.log(" calc - Allows you to do simple math");
                    core.log(" sysinfo - Gives info about the system");
                    core.log(" username - Username related functions");
                    core.log(" color - Allows you to change the color scheme");
                    core.log(" sd/shutdown/stop - Shuts down ChaOS");
                    core.log(" rb/reboot - Reboots the system");
                    core.clog(" diskinfo - Gives info about the disk" + us, color);
                    core.clog(" cd - Opens directory" + us, color);
                    core.clog(" cd.. - Opens last directory" + us, color);
                    core.clog(" dir - Lists files in the current directory" + us, color);
                    core.clog(" mkdir - Creates a directory" + us, color);
                    core.clog(" mkfile - Creates a file" + us, color);
                    core.clog(" deldir - Deletes a directory" + us, color);
                    core.clog(" delfile - Deletes a file" + us, color);
                    core.clog(" lb - Relabels the disk" + us, color);
                    core.clog(" notepad - Launches MIV (Minimalistic Vi)" + us + "\n", color);
                }

                #region Username functions
                else if (input.StartsWith("username"))
                {
                    if (input.Contains("set"))
                    {
                        try { username = input.Split("set ")[1].Trim(); } catch { core.clog("No arguments\n", Red); }
                        core.clog("Done! (" + username + ")\n", Yellow);
                    }
                    else if (input.EndsWith("current")) core.clog("Current username: " + username + "\n", Yellow);
                    else
                    {
                        core.clog("Username subfunctions:", Green);
                        core.log(" username set (username) - Changes the username");
                        core.log(" username current - Displays current username\n");
                    }
                }
                #endregion

                #region Color functions
                else if (input.Contains("color"))
                {
                    if (input.EndsWith("list"))
                    {
                        var OldBack = Console.BackgroundColor; var OldFore = Console.ForegroundColor;
                        core.clog("Color list:", Green);
                        Console.Write(" "); core.SetScreenColor(White, Black, false); Console.Write("black - Pure light mode, will make you blind"); core.SetScreenColor(OldBack, OldFore, false);
                        core.clog("\n dark blue - Dark blue with black background", DeepBlue);
                        core.clog(" dark magenta - Dark magenta with black background", Magenta);
                        core.clog(" dark yellow - Dark yellow/orange with black background", Yellow);
                        core.clog(" gray - Gray with black background", LightGray);
                        core.clog(" red - Red with black background", Red);
                        core.clog(" magenta - Magenta with black background", Magenta);
                        core.clog(" yellow - Light yellow with black background", Yellow);
                        core.clog(" white - Pure white with black background\n", White);
                    } // "Cosmos is built on else if blocks"
                    else if (input.EndsWith("black")) core.SetScreenColor(White, Black);
                    else if (!input.Contains("dark") && input.EndsWith("blue")) core.SetScreenColor(Black, Blue);
                    else if (!input.Contains("dark") && input.EndsWith("green")) core.SetScreenColor(Black, Green);
                    else if (!input.Contains("dark") && input.EndsWith("cyan")) core.SetScreenColor(Black, Cyan);
                    else if (!input.Contains("dark") && input.EndsWith("gray")) core.SetScreenColor(Black, LightGray);
                    else if (!input.Contains("dark") && input.EndsWith("red")) core.SetScreenColor(Black, Red);
                    else if (!input.Contains("dark") && input.EndsWith("magenta")) core.SetScreenColor(Black, Magenta);
                    else if (!input.Contains("dark") && input.EndsWith("yellow")) core.SetScreenColor(Black, Yellow);
                    else if (input.EndsWith("white")) core.SetScreenColor(Black, White);
                    else core.clog("Please list colors by doing \"opt color list\" or set a color by doing \"opt color (color)\"\n", LightGray);
                }
                #endregion

                else if (input == "credits")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 1);
                    core.cwrite("  ______   __                   ______    ______  \n /      \\ |  \\                 /      \\  /      \\ \n|  $$$$$$\\| $$____    ______  |  $$$$$$\\|  $$$$$$\\\n| $$   \\$$| $$    \\  |      \\ | $$  | $$| $$___\\$$\n| $$      | $$$$$$$\\  \\$$$$$$\\| $$  | $$ \\$$    \\ \n| $$   __ | $$  | $$ /      $$| $$  | $$ _\\$$$$$$\\\n| $$__/  \\| $$  | $$|  $$$$$$$| $$__/ $$|  \\__| $$\n \\$$    $$| $$  | $$ \\$$    $$ \\$$    $$ \\$$    $$\n  \\$$$$$$  \\$$   \\$$  \\$$$$$$$  \\$$$$$$   \\$$$$$$ ", Green);
                    Console.SetCursorPosition(55, 3);
                    core.cwrite("Version 1.4", Yellow);
                    Console.SetCursorPosition(62, 5);
                    core.cwrite("Credits", Yellow);
                    Console.SetCursorPosition(57, 6);
                    Console.Write("ekeleze - Creator");
                    Console.SetCursorPosition(60, 7);
                    Console.Write("xrc2 - Contributor");
                    Console.SetCursorPosition(20, 11);
                    Console.Write("Special thanks:");
                    Console.SetCursorPosition(1, 13);
                    Console.Write("Owen2k6 - For allowing ChaOS to live on");
                    Console.SetCursorPosition(0, 24);
                }

                else if (input == "clear" || input == "cls")
                    Console.Clear();

                else if (input == "time")
                {
                    string Hour = Cosmos.HAL.RTC.Hour.ToString(); string Minute = Cosmos.HAL.RTC.Minute.ToString();
                    if (Minute.Length < 2) Minute = "0" + Minute;
                    core.clog("Current time is " + Hour + ":" + Minute + "\n", Yellow);
                }

                else if (input.StartsWith("notepad") && diskMan.disk)
                    new MIV(this).StartMIV(input.Split("notepad ")[1]);

                else if (input == "shutdown" || input == "sd")
                {
                    if (diskMan.disk) diskMan.SaveChangesToDisk();
                    core.clog("Shutting down...", LightGray);
                    Stop();
                }

                else if (input == "reboot" || input == "rb")
                {
                    if (diskMan.disk) diskMan.SaveChangesToDisk();
                    core.clog("Rebooting...", LightGray);
                    Sys.Power.Reboot();
                }

                else if (input.StartsWith("echo"))
                    core.clog(inputBeforeLower.Split("echo ")[1] + "\n", LightGray);

                else if (input.StartsWith("calc"))
                {
                    int result = 0; input = input.Remove(0, 5); input = input.Trim();
                    if (input.Contains('+')) result = Convert.ToInt32(input.Split('+')[0]) + Convert.ToInt32(input.Split('+')[1]);
                    else if (input.Contains('-')) result = Convert.ToInt32(input.Split('-')[0]) - Convert.ToInt32(input.Split('-')[1]);
                    else if (input.Contains('*')) result = Convert.ToInt32(input.Split('*')[0]) * Convert.ToInt32(input.Split('*')[1]);
                    else if (input.Contains('/')) result = Convert.ToInt32(input.Split('/')[0]) / Convert.ToInt32(input.Split('/')[1]);
                    else if (input.EndsWith("calc"))
                    {
                        core.clog("Calculator subfunctions", Green);
                        core.log(" + - Adds numbers");
                        core.log(" - - Subtracts numbers");
                        core.log(" * - Multiplies numbers");
                        core.log(" / - Divides numbers");
                        core.log("\nSyntax: calc (num)(func)(num)\n");
                    }
                    if (result != 0)core. clog("Result: " + result + "\n", LightGray);
                }

                else if (input == "sysinfo")
                {
                    core.clog("System info:", Green);
                    core.log(" CPU: " + Cosmos.Core.CPU.GetCPUBrandString());
                    core.log(" CPU speed: " + Cosmos.Core.CPU.GetCPUCycleSpeed() / 1e6 + "MHz");
                    core.log(" System RAM: " + Cosmos.Core.GCImplementation.GetUsedRAM() + "MB used out of " + Cosmos.Core.CPU.GetAmountOfRAM() + "MB\n");
                }

                else if (input == "diskinfo")
                {
                    long availableSpace = VFSManager.GetAvailableFreeSpace(@"0:\");
                    long diskSpace = VFSManager.GetTotalSize(@"0:\");
                    string fsType = VFSManager.GetFileSystemType("0:\\");
                    core.clog("Disk info for " + FS.GetFileSystemLabel(rootdir), Green);
                    if (diskSpace < 1e6) core.log(" Disk space: " + availableSpace / 1e3 + " KB free out of " + diskSpace / 1000 + " KB total");
                    else if (diskSpace > 1e6) core.log(" Disk space: " + availableSpace / 1e6 + " MB free out of " + diskSpace / 1e+6 + " MB total");
                    else if (diskSpace > 1e9) core.log(" Disk space: " + availableSpace / 1e9 + " GB free out of " + diskSpace / 1e+9 + " GB total");
                    core.log(" Filesystem type: " + fsType + "\n");
                }

                #region Disk commands

                else if (input.StartsWith("mkdir"))
                {
                    try { inputBeforeLower = inputBeforeLower.Split("mkdir ")[1]; } catch { core.clog("No arguments\n", Red); CanContinue = false; }
                    if (inputBeforeLower.Contains("0:\\")) { inputBeforeLower.Replace("0:\\", ""); }
                    if (inputBeforeLower.Contains(" ")) { core.clog("Directory name cannot contain spaces!\n", Red); CanContinue = false; }
                    if (CanContinue)
                    {
                        if (!Directory.Exists(inputCapitalized))
                            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\" + inputCapitalized);
                        else
                            core.clog("Directory already exists!\n", Red);
                    }
                }

                else if (input.StartsWith("mkfile"))
                {
                    try { inputBeforeLower = inputBeforeLower.Split("mkfile ")[1]; } catch { core.clog("No arguments\n", Red); CanContinue = false; }
                    if (inputBeforeLower.Contains("0:\\")) { input.Replace("0:\\", ""); }
                    if (inputBeforeLower.Contains(" ")) { core.clog("Filename cannot contain spaces!\n", Red); CanContinue = false; }
                    if (CanContinue)
                    {
                        if (!File.Exists(inputBeforeLower))
                            File.Create(Directory.GetCurrentDirectory() + @"\" + inputBeforeLower);
                        else
                            core.clog("File already exists!\n", Red);
                    }
                }

                else if (input.StartsWith("deldir"))
                {
                    try { inputBeforeLower = inputBeforeLower.Split("deldir ")[1]; } catch { core.clog("No arguments\n", Red); CanContinue = false; }
                    if (inputBeforeLower.Contains("0:\\")) { input.Replace("0:\\", ""); }
                    if (inputBeforeLower.Contains(" ")) { core.clog("Filename cannot contain spaces!\n", Red); CanContinue = false; }
                    if (CanContinue)
                    {
                        if (Directory.Exists(inputBeforeLower))
                            Directory.Delete(Directory.GetCurrentDirectory() + @"\" + inputBeforeLower, true);
                        else
                            core.clog("Directory not found!\n", Red);
                    }
                }

                else if (input.StartsWith("delfile"))
                {
                    try { inputBeforeLower = inputBeforeLower.Split("delfile ")[1]; } catch { core.clog("No arguments\n", Red); CanContinue = false; }
                    if (inputBeforeLower.Contains("0:\\")) { input.Replace("0:\\", ""); }
                    if (inputBeforeLower.Contains(" ")) { core.clog("Filename cannot contain spaces!\n", Red); CanContinue = false; }
                    if (CanContinue)
                    {
                        if (File.Exists(inputBeforeLower))
                            File.Delete(Directory.GetCurrentDirectory() + @"\" + inputBeforeLower);
                        else
                            core.clog("File not found!\n", Red);
                    }
                }

                else if (input.StartsWith("cd") && diskMan.disk)
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
                        try { inputBeforeLower = inputBeforeLower.Split("cd ")[1]; } catch { core.clog("No arguments\n", Red); CanContinue = false; }
                        if (inputBeforeLower.Trim() != string.Empty) CanContinue = true;
                        if (CanContinue)
                        {
                            if (inputBeforeLower.Contains(@"0:\")) { inputBeforeLower.Replace(@"0:\", ""); }
                            if (Directory.GetCurrentDirectory() != rootdir) { inputBeforeLower = @"\" + inputBeforeLower; }
                            if (Directory.Exists(Directory.GetCurrentDirectory() + inputBeforeLower))
                                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + inputBeforeLower);
                            else core.clog("Directory not found!\n", Red);
                        }
                    }

                    else
                    {
                        core.clog("Cd subfunctions:", Green);
                        core.log(" cd (path) - Browses to directory");
                        core.log(" cd.. - Browses to last directory\n");
                    }
                }

                else if (input == "dir" && diskMan.disk)
                {
                    core.clog("Directory listing at " + falsedir, Yellow);
                    var directoryList = VFSManager.GetDirectoryListing(Directory.GetCurrentDirectory());
                    var files = 0; var dirs = 0;
                    foreach (var directoryEntry in directoryList)
                    {
                        if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + directoryEntry.mName))
                            core.clog("<Dir> " + directoryEntry.mName, LightGray); dirs += 1;
                    }
                    foreach (var directoryEntry in directoryList)
                    {
                        if (File.Exists(Directory.GetCurrentDirectory() + "\\" + directoryEntry.mName))
                            core.clog("<File> " + directoryEntry.mName, LightGray); files += 1;
                    }
                    core.clog("\nFound " + files + " files and " + dirs + " directories.\n", Yellow);
                }

                else if (input.StartsWith("copy"))
                {
                    var potato = string.Empty; var potato1 = string.Empty;
                    try { potato = inputBeforeLower.Split(" ")[1]; potato1 = inputBeforeLower.Split(" ")[2]; } catch { core.clog("No arguments\n", Red); CanContinue = false; }
                    if (CanContinue)
                    {
                        var Contents = File.ReadAllText(potato);
                        File.Create(potato1);
                        File.WriteAllText(potato1, Contents);
                        core.clog("Copy process finished successfully!\n", LightGray);
                    }
                }

                else if (input.StartsWith("lb") && diskMan.disk)
                    FS.SetFileSystemLabel(rootdir, inputBeforeLower.Split("lb ")[1]);

                #endregion

                else
                {
                    Console.Beep();
                    core.clog("Unknown command.\n", Red);
                }
            }
            catch (Exception ex) { core.Crash(ex); }
        }
    }
}
