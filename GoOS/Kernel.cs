using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;
using System.Linq;
using System.Text;
using GoOS.Themes;
using GoOS.Commands;
using Console = BetterConsole;
using ConsoleColor = PrismAPI.Graphics.Color;
using static GoOS.Core;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoOS._9xCode;
using PrismAPI.Graphics;
using IL2CPU.API.Attribs;
using PrismAPI.Hardware.GPU;
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

        public static Dictionary<string, string> InstalledPrograms = new Dictionary<string, string>() { };

        public static bool isGCIenabled = true;
        public static string[] pathPaths = { };

        //Vars for OS
        public const string version = "1.5pr3";
        public const string edition = "1.5pre"; // This is the current edition of GoOS. Used for UPDATER.
        public const string editionnext = "1.5"; // This is the next edition of GoOS. Used for UPDATER.
        public const string BuildType = "NIFPR";
        public static string olddir = @"0:\";

        public static string Notepadtextsavething = "";
        public static string NotepadFileToSaveNameThing = "";

        public static Sys.FileSystem.CosmosVFS FS;

        public static string username = null;
        public static string computername = null;

        public static string cutStatus = "Disabled";

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GoOS_Intro.bmp")]
        public static byte[] rawBootLogo;

        protected override void BeforeRun()
        {
            System.Console.Clear();
            System.Console.ForegroundColor = System.ConsoleColor.Cyan;
            System.Console.WriteLine("GoOS - Starting GoOS...");

            if (Cosmos.Core.CPU.GetAmountOfRAM() < 192)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine("GoOS - Insufficient Memory to initialise GoOS.");
                System.Console.WriteLine("GoOS - GoOS Recommends 1024MiB but the minimum is 172MiB");

                while (true);
            }

            Resources.Generate(ResourceType.Fonts);
            Resources.Generate(ResourceType.Priority);

            WindowManager.Canvas = Display.GetDisplay(1920, 1080);
            WindowManager.Canvas.DrawImage(0, 0, Resources.background, false);
            Console.Init(800, 600);

            WindowManager.AddWindow(new LoadingDialogue("GoOS is starting\nPlease wait..."));
            WindowManager.Update();

            Resources.Generate(ResourceType.Normal);

            ThemeManager.SetTheme(Theme.Fallback);
            log(ThemeManager.WindowText, "GoOS - Starting GoOS...");
            try
            {
                FS = new Sys.FileSystem.CosmosVFS();
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS);
                FS.Initialize(true);
                var total_space = FS.GetTotalSize(@"0:\");
            }
            catch
            {
                WindowManager.AddWindow(new Dialogue("Fatal Error", "Failed to initialize filesystem!\n" +
                                                     "GoOS needs a HDD installed to save user settings, application data and more\n" +
                                                     "Please verify that your hard disk is plugged in correctly",
                                                     default, WindowManager.errorIcon));
                WindowManager.Update();
                while (true);
            }

            if (!File.Exists(@"0:\content\sys\setup.gms"))
            {
                Resources.Generate(ResourceType.OOBE);
                WindowManager.IsInOOBE = true;
                WindowManager.AddWindow(new GUI.Apps.OOBE.MainFrame());
                return;
            }

            if (!File.Exists(@"0:\content\sys\path.ugms"))
            {
                try { File.Create(@"0:\content\sys\path.ugms"); }
                catch (Exception)
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
                    pathPaths.Append(@"0:\content\GCI\");
                }
                catch (Exception) { }
            }

            try
            {
                var systemsetup = File.ReadAllLines(@"0:\content\sys\user.gms");
                foreach (string line in systemsetup)
                {
                    if (line.StartsWith("username: ")) username = line.Replace("username: ", "");
                    if (line.StartsWith("computername: ")) computername = line.Replace("computername: ", "");
                }

                foreach (string line in File.ReadAllLines(@"0:\content\sys\theme.gms"))
                {
                    if (line.StartsWith("ThemeFile = ")) ThemeManager.SetTheme(line.Split("ThemeFile = ")[1]);
                }
            }
            catch
            {
                WindowManager.AddWindow(new Dialogue("Warning", "Failed to load settings!\nContinuing with default values...", default, Resources.warningIcon));
                log(ThemeManager.Other1, "GoOS - Failed to load settings, continuing with default values...");
            }

            if (username == null || username == "")
            {
                username = "user";
            }

            if (computername == null || computername == "")
            {
                computername = "GoOS";
            }

            InitNetwork();

            WindowManager.windows = new List<Window>(10);
            WindowManager.AddWindow(new Taskbar());
            WindowManager.AddWindow(new Desktop());

            #region GoOS Update Check

            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var dnsClient = new DnsClient();

                    // DNS
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk("api.goos.owen2k6.com");

                    // Address from IP
                    Address address = dnsClient.Receive();
                    dnsClient.Close();
                    string serverIP = address.ToString();

                    tcpClient.Connect(serverIP, 80);
                    NetworkStream stream = tcpClient.GetStream();
                    string httpget = "GET /GoOS/" + edition + ".goos HTTP/1.1\r\n" +
                                     "User-Agent: GoOS\r\n" +
                                     "Accept: */*\r\n" +
                                     "Accept-Encoding: identity\r\n" +
                                     "Host: api.goos.owen2k6.com\r\n" +
                                     "Connection: Keep-Alive\r\n\r\n";
                    byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    // Receive data
                    byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                    string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                    string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                    if (responseParts.Length < 2 || responseParts.Length > 2)
                        Dialogue.Show("GoOS Update", "Invalid HTTP response!", default, WindowManager.errorIcon);

                    string content = responseParts[1];

                    if (content != version && content != editionnext)
                    {
                        Dialogue.Show("GoOS Update",
                            "A newer version of GoOS is available on Github.\nWe recommend you update to the latest version for stability and security reasons.\nhttps://github.com/Owen2k6/GoOS/releases\nCurrent Version: " +
                            version + "\nLatest Version: " + content);
                    }
                    else if (content == editionnext)
                    {
                        Dialogue.Show("GoOS Update",
                            "The next GoOS has been released.\nWe don't want to force you to update but at least check out whats new in GoOS " +
                            editionnext + "!\nhttps://github.com/Owen2k6/GoOS/releases/tag/" + editionnext);
                    }
                    else if (content == "404")
                    {
                        Dialogue.Show("GoOS Update", "Your Version of GoOS does not support GoOS Update.");
                    }
                    else
                    {
                        WindowManager.AddWindow(new Welcome());
                    }
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("GoOS Update", "Failed to connect to Owen2k6 Api. Error: " + ex);
            }

            #endregion

            #region GoOS Support Checker

            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var dnsClient = new DnsClient();

                    // DNS
                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                    dnsClient.SendAsk("api.goos.owen2k6.com");

                    // Address from IP
                    Address address = dnsClient.Receive();
                    dnsClient.Close();
                    string serverIP = address.ToString();

                    tcpClient.Connect(serverIP, 80);
                    NetworkStream stream = tcpClient.GetStream();
                    string httpget = "GET /GoOS/" + edition + "-support.goos HTTP/1.1\r\n" +
                                     "User-Agent: GoOS\r\n" +
                                     "Accept: */*\r\n" +
                                     "Accept-Encoding: identity\r\n" +
                                     "Host: api.goos.owen2k6.com\r\n" +
                                     "Connection: Keep-Alive\r\n\r\n";
                    byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                    stream.Write(dataToSend, 0, dataToSend.Length);

                    // Receive data
                    byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                    string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                    string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

                    if (responseParts.Length < 2 || responseParts.Length > 2)
                        Dialogue.Show("GoOS Update", "Invalid HTTP response!", default, WindowManager.errorIcon);

                    string content = responseParts[1];

                    if (content == "true")
                    {
                    }
                    else if (content == "false")
                    {
                        Dialogue.Show("GoOS Security Notice",
                            "GoOS Support for this edition has ended. Please update to the latest version of GoOS for the latest security patches.\nThere will no more security patches released for GoOS " +
                            edition +
                            ".\nDownload the latest edition from \nhttps://github.com/Owen2k6/GoOS/releases/tag/");
                    }
                    else
                    {
                        Dialogue.Show("GoOS Security",
                            "You seem to be sporting some funky version of GoOS. Your edition doesnt exist on our servers (" +
                            edition + ")");
                    }
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("GoOS Security", "Failed to connect to Owen2k6 Api. Error: " + ex);
            }

            #endregion

            Console.Clear();

            Canvas cv = Image.FromBitmap(rawBootLogo, false);
            Console.Canvas.DrawImage(0, 0, cv, false);
            Console.SetCursorPosition(0, 13);

            Directory.SetCurrentDirectory(@"0:\");
        }

        public static string currentdirfix = string.Empty;

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
            textcolour(ThemeManager.WindowText);
            string currentdir = Directory.GetCurrentDirectory() + @"\";
            currentdirfix = @"0:\";
            if (currentdir.Contains(@"0:\\"))
            {
                currentdirfix = currentdir.Replace(@"0:\\", @"0:\");
            }
            else if (currentdir.Contains(@"0:\\\"))
            {
                currentdirfix = currentdir.Replace(@"0:\\\", @"0:\");
            }

            write($"{username}");
            textcolour(ThemeManager.Other1);
            write("@");
            textcolour(ThemeManager.WindowText);
            write($"{computername} ");
            textcolour(ThemeManager.WindowBorder);
            write(currentdirfix);
            textcolour(ThemeManager.Default);
        }

        protected override void Run()
        {
            isGCIenabled = File.Exists(@"0:\content\sys\path.ugms");

            if (isGCIenabled)
            {
                GoCodeInstaller.CheckForInstalledPrograms();
            }

            if (cutStatus == "FULL")
            {
            }

            if (cutStatus == "Single")
            {
            }

            DrawPrompt();

            // Commands section

            string[] args = Console.ReadLine().Trim().Split(' ');

            uint TotalRamUINT = Cosmos.Core.CPU.GetAmountOfRAM();
            int TotalRam = Convert.ToInt32(TotalRamUINT);

            switch (args[0])
            {
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
                    if (args.Length < 2)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments!");
                        break;
                    }

                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments!");
                        break;
                    }

                    GoCodeInstaller.Install(args[1]);
                    break;
                case "uninstall":
                    if (args.Length < 2)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments!");
                        break;
                    }

                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments!");
                        break;
                    }

                    GoCodeInstaller.Uninstall(args[1]);
                    break;
                case "movefile":
                    if (args.Length < 3)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments!");
                        break;
                    }

                    if (args.Length > 3)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments!");
                        break;
                    }

                    try
                    {
                        ExtendedFilesystem.MoveFile(args[1], args[2]);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Error whilst trying to move file: " + e);
                        break;
                    }

                    break;
                case "copyfile":
                    if (args.Length < 3)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments!");
                        break;
                    }

                    if (args.Length > 3)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments!");
                        break;
                    }

                    try
                    {
                        ExtendedFilesystem.CopyFile(args[1], args[2]);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Error whilst trying to copy file: " + e);
                        break;
                    }

                    break;
                case "help":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                    }

                    Commands.Help.Main();
                    break;
                case "go":
                    if (args.Length < 2)
                    {
                        log(ThemeManager.ErrorText, "Insufficient arguments");
                        break;
                    }

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

                            String filetoget = args[3];
                            try
                            {
                                string repo = args[2], type;

                                if (args[4] == "-g")
                                {
                                    type = "goexe";
                                }
                                else if (args[4] == "-9")
                                {
                                    type = "9xc";
                                }
                                else
                                {
                                    log(ThemeManager.ErrorText, "Unknown application type");
                                    break;
                                }

                                log(Color.Red, "Downloading " + filetoget + "." + type + " from " + repo);

                                using (TcpClient tcpClient = new TcpClient())
                                {
                                    var dnsClient = new DnsClient();

                                    // DNS
                                    dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                                    dnsClient.SendAsk(repo);

                                    // Address from IP
                                    Address address = dnsClient.Receive();
                                    dnsClient.Close();
                                    string serverIP = address.ToString();

                                    tcpClient.Connect(serverIP, 80);
                                    NetworkStream stream = tcpClient.GetStream();
                                    string httpget = "GET /" + filetoget + "." + type + " HTTP/1.1\r\n" +
                                                     "User-Agent: GoOS\r\n" +
                                                     "Accept: */*\r\n" +
                                                     "Accept-Encoding: identity\r\n" +
                                                     "Host: " + repo + "\r\n" +
                                                     "Connection: Keep-Alive\r\n\r\n";
                                    byte[] dataToSend = Encoding.ASCII.GetBytes(httpget);
                                    stream.Write(dataToSend, 0, dataToSend.Length);

                                    // Receive data
                                    byte[] receivedData = new byte[tcpClient.ReceiveBufferSize];
                                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);
                                    string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, bytesRead);

                                    string[] responseParts = receivedMessage.Split(new[] { "\r\n\r\n" }, 2,
                                        StringSplitOptions.None);

                                    if (responseParts.Length < 2 || responseParts.Length > 2)
                                        Dialogue.Show("GoOS Update", "Invalid HTTP response!", default,
                                            WindowManager.errorIcon);

                                    string content = responseParts[1];

                                    if (content == "404")
                                    {
                                        log(ThemeManager.ErrorText,
                                            "The requested file was not found on the server. Try another repo?");
                                        break;
                                    }

                                    File.Create(@"0:\" + filetoget + "." + type);
                                    File.WriteAllText(@"0:\" + filetoget + "." + type, content);
                                    log(Color.Green, "Downloaded " + filetoget + "." + type);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }

                            break;
                        default:
                            log(ThemeManager.ErrorText, "Unknown request.");
                            break;
                    }

                    break;
                case "run":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    if (oldCode)
                        Commands.Run.Main(args[1]);
                    else
                        GoCode.GoCode.Run(args[1]);
                    break;
                case "mkdir":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    Commands.Make.MakeDirectory(args[1]);
                    break;
                case "mkfile":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    Commands.Make.MakeFile(args[1]);
                    break;
                case "deldir":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    Commands.Delete.DeleteDirectory(args[1]);
                    break;
                case "delfile":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    Commands.Delete.DeleteFile(args[1]);
                    break;
                case "del":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    Delete.UniversalDelete(args[1]);
                    break;
                case "cd":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    Commands.Cd.Run(args[1]);
                    break;
                case "cd..":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    try
                    {
                        Directory.SetCurrentDirectory(Directory.GetCurrentDirectory().TrimEnd('\\')
                            .Remove(Directory.GetCurrentDirectory().LastIndexOf('\\') + 1));
                        Directory.SetCurrentDirectory(Directory.GetCurrentDirectory()
                            .Remove(Directory.GetCurrentDirectory().Length - 1));
                    }
                    catch
                    {
                        // ignored
                    }

                    if (!Directory.GetCurrentDirectory().StartsWith(@"0:\"))
                    {
                        Directory.SetCurrentDirectory(@"0:\");
                    }

                    break;
                case "cdr":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    string roota = @"0:\";
                    Directory.SetCurrentDirectory(roota);
                    break;
                case "dir":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    Commands.Dir.Run();
                    break;
                case "ls":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    Commands.Dir.Run();
                    break;
                case "notepad":
                    if (TotalRam < 1000)
                    {
                        log(ThemeManager.ErrorText, "This program has been disabled due to low ram.");
                        break;
                    }

                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    if (args[1].EndsWith(".gms") && !devMode)
                    {
                        log(ThemeManager.ErrorText,
                            "Files that end with .gms cannot be opened. they are protected files.");
                        break;
                    }

                    textcolour(ThemeManager.Default);
                    var editor = new TextEditor(Util.Paths.JoinPaths(currentdirfix, args[1]));
                    editor.Start();
                    break;
                case "vm":
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

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
                    if (args.Length > 2)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    if (args.Length == 1)
                    {
                        log(ThemeManager.ErrorText, "Missing arguments");
                        break;
                    }

                    _9xCode.Interpreter.Run(Directory.GetCurrentDirectory() + args[1]);
                    break;
                default:
                    if (isGCIenabled)
                    {
                        GoCodeInstaller.CheckForInstalledPrograms();
                    }

                    if (InstalledPrograms.ContainsKey(args[0]))
                    {
                        string rootass = @"0:\";

                        string currentDIRRRRRR = Directory.GetCurrentDirectory();

                        Directory.SetCurrentDirectory(rootass);

                        InstalledPrograms.TryGetValue(args[0], out string locat);

                        string TrueLocat = locat;

                        if (locat.Contains(@"0:\"))
                        {
                            TrueLocat = TrueLocat.Replace(@"0:\", "");
                        }

                        if (locat.ToLower().EndsWith(".goexe") || locat.ToLower().EndsWith(".gexe"))
                            Commands.Run.Main(TrueLocat);
                        else if (locat.ToLower().EndsWith(".9xc"))
                            Interpreter.Run(TrueLocat);

                        Directory.SetCurrentDirectory(currentDIRRRRRR);
                        break;
                    }

                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
    }
}