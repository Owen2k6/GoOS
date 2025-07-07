using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using GoOS.Themes;
using GoOS.Commands;
using Console = BetterConsole;
using ConsoleColor = GoGL.Graphics.Color;
using static GoOS.Core;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoOS._9xCode;
using GoGL.Graphics;
using IL2CPU.API.Attribs;
using GoGL.Hardware.GPU;
using GoOS.GUI;
using GoOS.GUI.Apps;
using GoOS.Networking;
using LibDotNetParser.CILApi;
using System.Net.Sockets;

// Goplex Studios - GoOS
// Copyright (C) 2024  Owen2k6

namespace GoOS
{
    public class Kernel : Sys.Kernel
    {
        // This enables the user to switch between the old and new GoCode interpreters.
        // This is to be removed as soon as the new one is finished, only being added as the new one needs testing.
        public static bool oldCode = false;
        public static readonly bool devMode = false;

        public static Dictionary<string, string> InstalledPrograms = new Dictionary<string, string>();

        public static bool isGCIenabled = true;
        public static string[] pathPaths = { };

        //Vars for OS
        public const string version = "1.5.4";
        public const string edition = "1.5"; // This is the current edition of GoOS. Used for UPDATER.
        public const string editiontitle = "Scafell"; // This is the current edition name of GoOS.
        public const string editionnext = "1.6"; // This is the next edition of GoOS. Used for UPDATER.
        public const string BuildType = "NIFPR";
        public const string Copyright = "2021-2025";
        public static string olddir = @"0:\";

        public static string Notepadtextsavething = "";
        public static string NotepadFileToSaveNameThing = "";

        public static Sys.FileSystem.CosmosVFS FS;

        public static string username = null;
        public static string computername = null;

        public static string cutStatus = "Disabled";
        public static string currentdirfix = string.Empty;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GoOS_Intro.bmp")]
        public static byte[] rawBootLogo;

        protected override void BeforeRun()
        {
            System.Console.Clear();
            System.Console.ForegroundColor = System.ConsoleColor.Cyan;
            System.Console.WriteLine("GoOS - Starting GoOS...");
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.WriteLine("Initialising Memory.");

            // Check RAM requirements
            uint ramAmount = Cosmos.Core.CPU.GetAmountOfRAM();
            if (ramAmount < 256)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                FATALErrorSound();
                System.Console.WriteLine("GoOS - Insufficient Memory to initialise GoOS.");
                System.Console.WriteLine("GoOS - GoOS Recommends 1024MiB but the minimum is 256MiB");
                while (true) ;
            }

            System.Console.WriteLine(ramAmount + "MB... DONE!");
            System.Console.WriteLine("Initialising Fonts...");
            Resources.Generate(ResourceType.Fonts);
            System.Console.WriteLine("SUCCESSFUL!");
            System.Console.WriteLine("Initialising System Resources...");
            Resources.Generate(ResourceType.Priority);
            System.Console.WriteLine("SUCCESSFUL!");

            InitialiseFileSystem();
            InitialiseGoGL();

            Resources.Generate(ResourceType.Normal);
            ThemeManager.SetTheme(Theme.Fallback);
            log(ThemeManager.WindowText, "GoOS - Starting GoOS...");

            // Handle OOBE setup
            if (!File.Exists(@"0:\content\sys\setup.gms"))
            {
                WindowManager.Canvas.DrawImage(0, 0, Resources.bootbackground, false);
                WindowManager.Update();
                Resources.Generate(ResourceType.OOBE);
                WindowManager.IsInOOBE = true;
                WindowManager.AddWindow(new GUI.Apps.OOBE.MainFrame());
                return;
            }

            SetupPathAndGCI();
            LoadUserSettings();
            InitNetwork();

            WindowManager.windows = new List<Window>(10);
            WindowManager.AddWindow(new Taskbar());
            WindowManager.AddWindow(new Desktop());

            Sys.MouseManager.X = 0;
            Sys.MouseManager.Y = 0;

            Console.Clear();

            Canvas cv = Image.FromBitmap(rawBootLogo, false);
            Console.Canvas.DrawImage(0, 0, cv, false);
            Console.SetCursorPosition(0, 13);

            Directory.SetCurrentDirectory(@"0:\");
        }

        private void InitialiseFileSystem()
        {
            try
            {
                System.Console.WriteLine("Initialising Filesystem...");
                FS = new Sys.FileSystem.CosmosVFS();
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS);
                FS.Initialize(true);
                var total_space = FS.GetTotalSize(@"0:\");
                System.Console.WriteLine("SUCCESSFUL!");
            }
            catch
            {
                FATALErrorSound();
                log(ConsoleColorEx.Red, "Failed to initialize filesystem!");
                log(ConsoleColorEx.Red, "GoOS needs a HDD installed to save user settings, application data and more");
                log(ConsoleColorEx.Red, "Please verify that your hard disk is plugged in correctly");
                while (true) ;
            }
        }

        private void InitialiseGoGL()
        {
            System.Console.WriteLine("Initialising GoGL...");
            byte[] screenRes = File.Exists(@"0:\content\sys\resolution.gms")
                ? File.ReadAllBytes(@"0:\content\sys\resolution.gms")
                : new byte[] { 6 };
            byte[] termRes = File.Exists(@"0:\content\sys\tresolution.gms")
                ? File.ReadAllBytes(@"0:\content\sys\tresolution.gms")
                : new byte[] { 2 };

            var videoMode = ControlPanel.videoModes[screenRes[0]].Item2;
            WindowManager.Canvas = Display.GetDisplay(videoMode.Width, videoMode.Height);

            WindowManager.Canvas.DrawImage(0, 0, Resources.bootbackground, false);
            WindowManager.Canvas.DrawImage(videoMode.Width / 2 - 37,
                videoMode.Height / 2 - 37, Resources.bootlogo, true);
            Console.Init(800, 600);
            Cosmos.System.PCSpeaker.Beep(600, 100);
            WindowManager.Update();
        }

        private void SetupPathAndGCI()
        {
            if (!File.Exists(@"0:\content\sys\path.ugms"))
            {
                try
                {
                    File.Create(@"0:\content\sys\path.ugms");
                }
                catch
                {
                    isGCIenabled = false;
                    pathPaths = null;
                }
            }

            if (!Directory.Exists(@"0:\content\GCI\"))
            {
                try
                {
                    Directory.CreateDirectory(@"0:\content\GCI\");
                    pathPaths = pathPaths.Append(@"0:\content\GCI\").ToArray();
                }
                catch
                {
                    // Silently fail
                }
            }
        }

        private void LoadUserSettings()
        {
            try
            {
                if (File.Exists(@"0:\content\sys\user.gms"))
                {
                    foreach (string line in File.ReadAllLines(@"0:\content\sys\user.gms"))
                    {
                        if (line.StartsWith("username: ")) username = line.Replace("username: ", "");
                        if (line.StartsWith("computername: ")) computername = line.Replace("computername: ", "");
                    }
                }

                if (File.Exists(@"0:\content\sys\theme.gms"))
                {
                    foreach (string line in File.ReadAllLines(@"0:\content\sys\theme.gms"))
                    {
                        if (line.StartsWith("ThemeFile = ")) ThemeManager.SetTheme(line.Split("ThemeFile = ")[1]);
                    }
                }
            }
            catch
            {
                WindowManager.AddWindow(new Dialogue("Warning",
                    "Failed to load settings!\nContinuing with default values...", default, Resources.warningIcon));
                log(ThemeManager.Other1, "GoOS - Failed to load settings, continuing with default values...");
            }

            username = string.IsNullOrEmpty(username) ? "user" : username;
            computername = string.IsNullOrEmpty(computername) ? "GoOS" : computername;
        }

        private static void InitNetwork()
        {
            using (var xClient = new DHCPClient())
            {
                xClient.SendDiscoverPacket();
                log(ConsoleColor.Blue, NetworkConfiguration.CurrentAddress.ToString());
            }
        }

        public static void DrawPrompt()
        {
            string currentdir = Directory.GetCurrentDirectory() + @"\";

            // Fix path formatting issues
            if (currentdir.Contains(@"0:\\"))
                currentdirfix = currentdir.Replace(@"0:\\", @"0:\");
            else if (currentdir.Contains(@"0:\\\"))
                currentdirfix = currentdir.Replace(@"0:\\\", @"0:\");
            else
                currentdirfix = @"0:\";

            textcolour(ThemeManager.WindowText);
            write($"{username}");
            textcolour(ThemeManager.Other1);
            write("@");
            textcolour(ThemeManager.WindowText);
            write($"{computername} ");
            textcolour(ThemeManager.WindowBorder);
            write(currentdirfix);
            textcolour(ThemeManager.Default);
        }

        public static void FATALErrorSound()
        {
            Cosmos.System.PCSpeaker.Beep(600, 400);
            Cosmos.System.PCSpeaker.Beep(500, 400);
            Cosmos.System.PCSpeaker.Beep(600, 400);
            Cosmos.System.PCSpeaker.Beep(500, 400);
        }

        public static void ErrorSound()
        {
            Cosmos.System.PCSpeaker.Beep(600, 20);
            Cosmos.System.PCSpeaker.Beep(400, 20);
            Cosmos.System.PCSpeaker.Beep(200, 20);
        }

        public static void InfoSound()
        {
            Cosmos.System.PCSpeaker.Beep(600, 20);
            Cosmos.System.PCSpeaker.Beep(800, 20);
            Cosmos.System.PCSpeaker.Beep(1200, 20);
        }

        protected override void Run()
        {
            isGCIenabled = File.Exists(@"0:\content\sys\path.ugms");

            if (isGCIenabled)
            {
                GoCodeInstaller.CheckForInstalledPrograms();
            }

            // Placeholder for cut status logic
            if (cutStatus == "FULL" || cutStatus == "Single")
            {
                // Logic would go here
            }

            DrawPrompt();

            // Parse command line
            string[] args = Console.ReadLine().Trim().Split(' ');
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                return;

            int totalRam = Convert.ToInt32(Cosmos.Core.CPU.GetAmountOfRAM());

            // Process command
            ProcessCommand(args, totalRam);
        }

        private void ProcessCommand(string[] args, int totalRam)
        {
            switch (args[0])
            {
                case "gldiag":
                    WindowManager.AddWindow(new GUI.Apps.GoGLDiag());
                    break;

                case "codeswitch":
                    oldCode = !oldCode;
                    break;

                case "fm":
                    WindowManager.AddWindow(new GUI.Apps.Gosplorer.MainFrame());
                    break;

                case "exit":
                    Console.Visible = false;
                    break;

                case "ping":
                    Ping.Run();
                    break;

                case "install":
                    if (!CheckArgCount(args, 2, true)) break;
                    GoCodeInstaller.Install(args[1]);
                    break;

                case "uninstall":
                    if (!CheckArgCount(args, 2, true)) break;
                    GoCodeInstaller.Uninstall(args[1]);
                    break;

                case "movefile":
                    if (!CheckArgCount(args, 3, true)) break;

                    try
                    {
                        ExtendedFilesystem.MoveFile(args[1], args[2]);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Error whilst trying to move file: " + e);
                    }
                    break;

                case "copyfile":
                    if (!CheckArgCount(args, 3, true)) break;

                    try
                    {
                        ExtendedFilesystem.CopyFile(args[1], args[2]);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Error whilst trying to copy file: " + e);
                    }
                    break;

                case "help":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }
                    Commands.Help.Main();
                    break;

                case "go":
                    if (!CheckArgCount(args, 2, true)) break;
                    HandleGoCommand(args);
                    break;

                case "run":
                    if (!CheckArgCount(args, 2, true)) break;

                    if (oldCode)
                        Commands.Run.Main(args[1]);
                    else
                        GoCode.GoCode.Run(args[1]);
                    break;

                case "mkdir":
                    if (!CheckArgCount(args, 2, true)) break;
                    Commands.Make.MakeDirectory(args[1]);
                    break;

                case "mkfile":
                    if (!CheckArgCount(args, 2, true)) break;
                    Commands.Make.MakeFile(args[1]);
                    break;

                case "deldir":
                    if (!CheckArgCount(args, 2, true)) break;
                    Commands.Delete.DeleteDirectory(args[1]);
                    break;

                case "delfile":
                    if (!CheckArgCount(args, 2, true)) break;
                    Commands.Delete.DeleteFile(args[1]);
                    break;

                case "del":
                    if (!CheckArgCount(args, 2, true)) break;
                    Delete.UniversalDelete(args[1]);
                    break;

                case "cd":
                    if (!CheckArgCount(args, 2, true)) break;
                    Commands.Cd.Run(args[1]);
                    break;

                case "cd..":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }
                    NavigateToParentDirectory();
                    break;

                case "cdr":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }
                    Directory.SetCurrentDirectory(@"0:\");
                    break;

                case "dir":
                case "ls":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }
                    Commands.Dir.Run();
                    break;

                case "notepad":
                    if (!CheckRamAndArguments(totalRam, args, 2)) break;

                    if (args[1].EndsWith(".gms") && !devMode)
                    {
                        log(ThemeManager.ErrorText, "Files that end with .gms cannot be opened. they are protected files.");
                        break;
                    }

                    textcolour(ThemeManager.Default);
                    var editor = new TextEditor(Util.Paths.JoinPaths(currentdirfix, args[1]));
                    editor.Start();
                    break;

                case "vm":
                    if (!CheckArgCount(args, 2, true)) break;
                    VM.Run(args[1]);
                    break;

                case "clear":
                    Console.Clear();
                    break;

                case "whoami":
                    log(ThemeManager.ErrorText, "Showing Internet Information");
                    log(ThemeManager.ErrorText, NetworkConfiguration.CurrentAddress.ToString());
                    break;

                case "dotnet":
                    var fl = new DotNetFile(Directory.GetCurrentDirectory() + args[1]);
                    DotNetClr.DotNetClr clr = new DotNetClr.DotNetClr(fl, @"0:\framework");
                    clr.Start();
                    break;

                case "9xcode":
                    if (!CheckArgCount(args, 2, true)) break;
                    _9xCode.Interpreter.Run(Directory.GetCurrentDirectory() + args[1]);
                    break;

                default:
                    HandleDefaultCommand(args);
                    break;
            }
        }

        private bool CheckArgCount(string[] args, int expectedCount, bool exact)
        {
            if (args.Length < expectedCount)
            {
                log(ThemeManager.ErrorText, "Missing arguments");
                return false;
            }

            if (exact && args.Length > expectedCount)
            {
                log(ThemeManager.ErrorText, "Too many arguments");
                return false;
            }

            return true;
        }

        private bool CheckRamAndArguments(int totalRam, string[] args, int expectedCount)
        {
            if (totalRam < 1000)
            {
                log(ThemeManager.ErrorText, "This program has been disabled due to low ram.");
                return false;
            }

            return CheckArgCount(args, expectedCount, true);
        }

        private void NavigateToParentDirectory()
        {
            try
            {
                string currentDir = Directory.GetCurrentDirectory().TrimEnd('\\');
                int lastSlashIndex = currentDir.LastIndexOf('\\');

                if (lastSlashIndex > 0)
                {
                    Directory.SetCurrentDirectory(currentDir.Remove(lastSlashIndex));
                }
            }
            catch
            {
                // Silently fail
            }

            // Ensure we don't navigate above root
            if (!Directory.GetCurrentDirectory().StartsWith(@"0:\"))
            {
                Directory.SetCurrentDirectory(@"0:\");
            }
        }

        private void HandleGoCommand(string[] args)
        {
            switch (args[1])
            {
                case "type":
                    log(Color.Minty, "GoOS - Application Types");
                    log(Color.GoogleYellow, "-g Goexe");
                    log(Color.GoogleYellow, "-9 9xCode");
                    break;

                case "install":
                    if (args.Length != 5)
                    {
                        log(ThemeManager.ErrorText, "X: go install <repo> <appname> -<type>");
                        break;
                    }

                    DownloadApplication(args[2], args[3], args[4]);
                    break;

                default:
                    log(ThemeManager.ErrorText, "Unknown request.");
                    break;
            }
        }

        private void DownloadApplication(string repo, string fileToGet, string typeFlag)
        {
            try
            {
                string type;

                if (typeFlag == "-g")
                {
                    type = "goexe";
                }
                else if (typeFlag == "-9")
                {
                    type = "9xc";
                }
                else
                {
                    log(ThemeManager.ErrorText, "Unknown application type");
                    return;
                }

                log(Color.Red, $"Downloading {fileToGet}.{type} from {repo}");

                using (TcpClient tcpClient = new TcpClient())
                {
                    // DNS lookup
                    var dnsClient = new DnsClient();
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk(repo);
                    Address address = dnsClient.Receive();
                    dnsClient.Close();

                    // Connect and download
                    tcpClient.Connect(address.ToString(), 80);
                    NetworkStream stream = tcpClient.GetStream();

                    string httpget = $"GET /{fileToGet}.{type} HTTP/1.1\r\n" +
                                    "User-Agent: GoOS\r\n" +
                                    "Accept: */*\r\n" +
                                    "Accept-Encoding: identity\r\n" +
                                    $"Host: {repo}\r\n" +
                                    "Connection: Keep-Alive\r\n\r\n";

                    byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    // Receive response
                    byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                    string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                    string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                    if (responseParts.Length != 2)
                    {
                        Dialogue.Show("GoOS Update", "Invalid HTTP response!", default, WindowManager.errorIcon);
                        return;
                    }

                    string content = responseParts[1];

                    if (content == "404")
                    {
                        log(ThemeManager.ErrorText, "The requested file was not found on the server. Try another repo?");
                        return;
                    }

                    string filePath = $@"0:\{fileToGet}.{type}";
                    File.Create(filePath);
                    File.WriteAllText(filePath, content);
                    log(Color.Green, $"Downloaded {fileToGet}.{type}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void HandleDefaultCommand(string[] args)
        {
            if (isGCIenabled)
            {
                GoCodeInstaller.CheckForInstalledPrograms();
            }

            if (InstalledPrograms.ContainsKey(args[0]))
            {
                string currentDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(@"0:\");

                InstalledPrograms.TryGetValue(args[0], out string location);

                if (location != null)
                {
                    string actualLocation = location.Replace(@"0:\", "");

                    if (location.ToLower().EndsWith(".goexe") || location.ToLower().EndsWith(".gexe"))
                        Commands.Run.Main(actualLocation);
                    else if (location.ToLower().EndsWith(".9xc"))
                        Interpreter.Run(actualLocation);
                }

                Directory.SetCurrentDirectory(currentDir);
            }
            else
            {
                Console.WriteLine("Invalid command.");
            }
        }
    }
}