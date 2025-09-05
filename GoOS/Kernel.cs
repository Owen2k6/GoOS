using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Gold.Hardware.GPU;
using GoOS._9xCode;
using GoOS.Commands;
using GoOS.GUI;
using GoOS.GUI.Apps;
using GoOS.GUI.Apps.OOBE;
using GoOS.Themes;
using GoOS.Util;
using Console = BetterConsole;
using ConsoleColor = Gold.Graphics.Color;
using static GoOS.Core;

// Goplex Studios - GoOS
// Copyright (C) 2025
// Owen2k6

namespace GoOS;

public class Kernel : Cosmos.System.Kernel
{
    // =========================================================
    //                      CONSTANTS / META
    // =========================================================
    public const string version = "1.6 Developer Beta 1";
    public const string edition = "1.6dev"; // Used by Updater
    public const string editiontitle = "Scafell Pike"; // Display name
    public const string editionnext = "1.6"; // Used by Updater
    public const string BuildType = "DB";
    public const string Copyright = "2021-2025";

    // =========================================================
    //                      GLOBAL STATE
    // =========================================================

    public static bool oldCode;
    public static readonly bool devMode = false;
    public static bool isGCIenabled = true;
    public static string olddir = @"0:\";
    public static string currentdirfix = string.Empty;
    public static string username;
    public static string computername;
    public static readonly Dictionary<string, string> InstalledPrograms = new(16);
    public static string[] pathPaths = new string[0];
    public static string cutStatus = "Disabled";

    public static CosmosVFS FS;

    // =========================================================
    //                          BOOT
    // =========================================================
    protected override void BeforeRun()
    {
        System.Console.Clear();
        var ramAmount = CPU.GetAmountOfRAM();
        if (ramAmount < 256)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            FATALErrorSound();
            System.Console.WriteLine("GoOS - Insufficient memory to initialise GoOS.");
            System.Console.WriteLine("GoOS - Recommended: 1024 MiB; minimum: 256 MiB");
            while (true)
            {
            }
        }

        Resources.Generate(ResourceType.Boot);
        InitialiseFileSystem();
        InitialiseGold();
        Resources.Generate(ResourceType.Fonts);
        Resources.Generate(ResourceType.Priority);
        Resources.Generate(ResourceType.Normal);
        ThemeManager.SetTheme(Theme.Fallback);
        if (!File.Exists(@"0:\content\sys\setup.gms"))
        {
            WindowManager.Canvas.DrawImage(0, 0, Resources.bootbackground, false);
            WindowManager.Update();
            Resources.Generate(ResourceType.OOBE);
            WindowManager.IsInOOBE = true;
            WindowManager.AddWindow(new MainFrame());
            return;
        }

        SetupPathAndGCI();
        LoadUserSettings();
        InitNetwork();
        WindowManager.windows = new List<Window>(10);
        WindowManager.AddWindow(new Desktop());
        WindowManager.AddWindow(new Menubar());
        MouseManager.X = 0;
        MouseManager.Y = 0;
        Console.Clear();
        Console.WriteLine("Welcome to the GoOS Terminal");
        Console.WriteLine("Version " + version);
        Console.WriteLine("Type HELP for a list of commands.");
        Directory.SetCurrentDirectory(@"0:\");
    }

    // =========================================================
    //                      INITIALISERS
    // =========================================================
    private void InitialiseFileSystem()
    {
        try
        {
            FS = new CosmosVFS();
            VFSManager.RegisterVFS(FS);
            FS.Initialize(true);
            FS.GetTotalSize(@"0:\"); // warm-up
        }
        catch
        {
            FATALErrorSound();
            log(ConsoleColorEx.Red, "Failed to initialise filesystem!");
            log(ConsoleColorEx.Red, "GoOS needs a HDD installed to save user settings, application data and more.");
            log(ConsoleColorEx.Red, "Please verify that your hard disk is plugged in correctly.");
            while (true)
            {
            }
        }
    }

    private void InitialiseGold()
    {
        var screenRes = File.Exists(@"0:\content\sys\resolution.gms")
            ? File.ReadAllBytes(@"0:\content\sys\resolution.gms")
            : new byte[] { 6 };
        var videoMode = ControlPanel.videoModes[screenRes[0]].Item2;
        WindowManager.Canvas = Display.GetDisplay(videoMode.Width, videoMode.Height);
        WindowManager.Canvas.DrawImage(0, 0, Resources.bootbackground, false);
        WindowManager.Canvas.DrawImage(videoMode.Width / 2 - 37, videoMode.Height / 2 - 37, Resources.bootlogo);
        Console.Init(417, 346);
        PCSpeaker.Beep(600, 100);
        WindowManager.Update();
    }

    private void SetupPathAndGCI()
    {
        if (!File.Exists(@"0:\content\sys\path.ugms"))
            try
            {
                using (File.Create(@"0:\content\sys\path.ugms"))
                {
                }
            }
            catch
            {
                isGCIenabled = false;
                pathPaths = null;
            }

        if (!Directory.Exists(@"0:\content\GCI\"))
            try
            {
                Directory.CreateDirectory(@"0:\content\GCI\");
                var old = pathPaths;
                var n = old == null ? 0 : old.Length;
                var np = new string[n + 1];
                for (var i = 0; i < n; i++) np[i] = old[i];
                np[n] = @"0:\content\GCI\";
                pathPaths = np;
            }
            catch
            {
                /* Ignored */
            }
    }

    private void LoadUserSettings()
    {
        try
        {
            {
                var p = @"0:\content\sys\user.gms";
                if (File.Exists(p))
                    using (var sr = new StreamReader(p, Encoding.UTF8, false))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                            if (line.StartsWith("username: "))
                                username = line.Substring(10);
                            else if (line.StartsWith("computername: "))
                                computername = line.Substring(14);
                    }
            }
            {
                var p = @"0:\content\sys\theme.gms";
                if (File.Exists(p))
                {
                    const string key = "ThemeFile = ";
                    using (var sr = new StreamReader(p, Encoding.UTF8, false))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                            if (line.StartsWith(key))
                                ThemeManager.SetTheme(line.Substring(key.Length));
                    }
                }
            }
        }
        catch
        {
            WindowManager.AddWindow(new Dialogue(
                "Warning",
                "Failed to load settings!\nContinuing with default values...",
                default,
                Resources.warningIcon));
            log(ThemeManager.Other1, "GoOS - Failed to load settings, continuing with default values...");
        }

        if (string.IsNullOrEmpty(username)) username = "user";
        if (string.IsNullOrEmpty(computername)) computername = "GoOS";
    }

    private static void InitNetwork()
    {
        using (var xClient = new DHCPClient())
        {
            xClient.SendDiscoverPacket();
            log(ConsoleColor.Blue, NetworkConfiguration.CurrentAddress.ToString());
        }
    }

    // =========================================================
    //                     PROMPT / SOUNDS
    // =========================================================
    public static void DrawPrompt()
    {
        var currentdir = Directory.GetCurrentDirectory();
        if (!currentdir.EndsWith("\\")) currentdir += "\\";
        if (currentdir.IndexOf(@"0:\\\") >= 0)
            currentdirfix = currentdir.Replace(@"0:\\\", @"0:\");
        else if (currentdir.IndexOf(@"0:\\") >= 0)
            currentdirfix = currentdir.Replace(@"0:\\", @"0:\");
        else
            currentdirfix = currentdir;
        textcolour(ThemeManager.WindowText);
        write(username);
        textcolour(ThemeManager.Other1);
        write("@");
        textcolour(ThemeManager.WindowText);
        write(computername + " ");
        textcolour(ThemeManager.WindowBorder);
        write(currentdirfix);
        textcolour(ThemeManager.Default);
    }

    public static void FATALErrorSound()
    {
        PCSpeaker.Beep(600, 400);
        PCSpeaker.Beep(500, 400);
        PCSpeaker.Beep(600, 400);
        PCSpeaker.Beep(500, 400);
    }

    public static void ErrorSound()
    {
        PCSpeaker.Beep(600, 20);
        PCSpeaker.Beep(400, 20);
        PCSpeaker.Beep(200, 20);
    }

    public static void InfoSound()
    {
        PCSpeaker.Beep(600, 20);
        PCSpeaker.Beep(800, 20);
        PCSpeaker.Beep(1200, 20);
    }

    // =========================================================
    //                        MAIN LOOP
    // =========================================================
    protected override void Run()
    {
        Pump();
        // isGCIenabled = File.Exists(@"0:\content\sys\path.ugms");
        // if (isGCIenabled) GoCodeInstaller.CheckForInstalledPrograms();
        // if (cutStatus == "FULL" || cutStatus == "Single")
        // {
        // }
        // DrawPrompt();
        //var line = Console.ReadLine();
        // if (line == null) return;
        // int start = 0, end = line.Length - 1;
        // while (start <= end && line[start] == ' ') start++;
        // while (end >= start && line[end] == ' ') end--;
        // if (start > end) return;
        // var args = SplitBySpaces(line, start, end);
        // if (args == null || args.Length == 0) return;
        // var totalRam = (int)CPU.GetAmountOfRAM();
        // ProcessCommand(args, totalRam);
    }

    // =========================================================
    //                 COMMAND DISPATCHER
    // =========================================================
    // private void ProcessCommand(string[] args, int totalRam)
    // {
    //     var cmd0 = args[0];
    //     switch (cmd0)
    //     {
    //         // case "run":
    //         //     if (!CheckArgCount(args, 2, true)) break;
    //         //     if (oldCode) Commands.Run.Main(args[1]);
    //         //     else GoCode.GoCode.Run(args[1]);
    //         //     break;
    //         // case "notepad":
    //         //     if (!CheckRamAndArguments(totalRam, args, 2)) break;
    //         //     if (EndsWithIgnoreCase(args[1], ".gms") && !devMode)
    //         //     {
    //         //         log(ThemeManager.ErrorText, "Files that end with .gms cannot be opened. they are protected files.");
    //         //         break;
    //         //     }
    //         //
    //         //     textcolour(ThemeManager.Default);
    //         // {
    //         //     var p = Paths.JoinPaths(currentdirfix, args[1]);
    //         //     var editor = new TextEditor(p);
    //         //     editor.Start();
    //         // }
    //         //     break;
    //         // case "vm":
    //         //     if (!CheckArgCount(args, 2, true)) break;
    //         //     VM.Run(args[1]);
    //         //     break;
    //         // case "clear":
    //         //     Console.Clear();
    //         //     break;
    //     }
    // }
    
    
    public static void Pump()
    {
        if (BetterConsole.CursorVisible)
            BetterConsole.SmartCursor(true);
        BetterConsole.Render();
        BetterConsole.TrimKeyBuffer();
    }

}