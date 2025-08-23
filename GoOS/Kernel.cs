using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;
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
// Copyright (C) 2025  Owen2k6

namespace GoOS
{
    public class Kernel : Sys.Kernel
    {
        // This enables the user to switch between the old and new GoCode interpreters.
        // This is to be removed as soon as the new one is finished, only being added as the new one needs testing.
        public static bool oldCode = false;
        public static readonly bool devMode = false;

        public static readonly Dictionary<string, string> InstalledPrograms = new Dictionary<string, string>();

        public static bool isGCIenabled = true;

        // Keep simple fixed array; avoid LINQ Append.
        public static string[] pathPaths = new string[0];

        // Vars for OS
        public const string version = "1.5.5";
        public const string edition = "1.5"; // This is the current edition of GoOS. Used for UPDATER.
        public const string editiontitle = "Scafell"; // This is the current edition name of GoOS.
        public const string editionnext = "1.6"; // This is the next edition of GoOS. Used for UPDATER.
        public const string BuildType = "R";
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
            uint ramAmount = Cosmos.Core.CPU.GetAmountOfRAM();
            if (ramAmount < 256)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                FATALErrorSound();
                System.Console.WriteLine("GoOS - Insufficient Memory to initialise GoOS.");
                System.Console.WriteLine("GoOS - GoOS Recommends 1024MiB but the minimum is 256MiB");
                while (true) ;
            }
            Resources.Generate(ResourceType.Boot);
            InitialiseFileSystem();
            InitialiseGoGL();
            Resources.Generate(ResourceType.Fonts);
            Resources.Generate(ResourceType.Priority);
            Resources.Generate(ResourceType.Normal);
            ThemeManager.SetTheme(Theme.Fallback);
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
                // Touch total size to ensure driver warms up; no output needed.
                FS.GetTotalSize(@"0:\");
                System.Console.WriteLine("SUCCESSFUL!");
            }
            catch
            {
                FATALErrorSound();
                log(ConsoleColorEx.Red, "Failed to initialise filesystem!");
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
                    using (File.Create(@"0:\content\sys\path.ugms")) { }
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
                    // manual grow (avoid LINQ Append)
                    string[] old = pathPaths;
                    int n = (old == null) ? 0 : old.Length;
                    string[] np = new string[n + 1];
                    for (int i = 0; i < n; i++) np[i] = old[i];
                    np[n] = @"0:\content\GCI\";
                    pathPaths = np;
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
                    string[] lines = File.ReadAllLines(@"0:\content\sys\user.gms");
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (line.StartsWith("username: "))
                            username = line.Replace("username: ", "");
                        else if (line.StartsWith("computername: "))
                            computername = line.Replace("computername: ", "");
                    }
                }

                if (File.Exists(@"0:\content\sys\theme.gms"))
                {
                    string[] lines = File.ReadAllLines(@"0:\content\sys\theme.gms");
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        if (line.StartsWith("ThemeFile = "))
                            ThemeManager.SetTheme(line.Substring("ThemeFile = ".Length));
                    }
                }
            }
            catch
            {
                WindowManager.AddWindow(new Dialogue("Warning",
                    "Failed to load settings!\nContinuing with default values...", default, Resources.warningIcon));
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

        public static void DrawPrompt()
        {
            string currentdir = Directory.GetCurrentDirectory();
            // Always ensure a trailing backslash for display.
            if (!currentdir.EndsWith("\\")) currentdir += "\\";

            // Fix path formatting issues conservatively; if nothing to fix, keep original.
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

            // Read command line safely (allow bare Enter).
            string line = Console.ReadLine();
            if (line == null)
                return;

            line = line.Trim();
            if (line.Length == 0)
                return;

            // Simple splitter without LINQ/regex; collapses multiple spaces.
            string[] args = SplitBySpaces(line);
            if (args == null || args.Length == 0)
                return;

            int totalRam = (int)Cosmos.Core.CPU.GetAmountOfRAM();

            // Process command
            ProcessCommand(args, totalRam);
        }

        private static string[] SplitBySpaces(string input)
        {
            // Avoid StringSplitOptions, LINQ etc.
            int len = input.Length;
            string[] tmp = new string[16];
            int count = 0;

            int i = 0;
            while (i < len)
            {
                // skip spaces
                while (i < len && input[i] == ' ') i++;
                if (i >= len) break;

                // read token
                int start = i;
                while (i < len && input[i] != ' ') i++;
                int tokLen = i - start;
                if (tokLen > 0)
                {
                    if (count == tmp.Length)
                    {
                        string[] grow = new string[tmp.Length * 2];
                        for (int k = 0; k < tmp.Length; k++) grow[k] = tmp[k];
                        tmp = grow;
                    }
                    tmp[count++] = input.Substring(start, tokLen);
                }
            }

            if (count == 0) return new string[0];
            string[] res = new string[count];
            for (int k = 0; k < count; k++) res[k] = tmp[k];
            return res;
        }

        private void ProcessCommand(string[] args, int totalRam)
        {
            string cmd0 = args[0];

            switch (cmd0)
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
                    try { ExtendedFilesystem.MoveFile(args[1], args[2]); }
                    catch (Exception e) { System.Console.WriteLine("Error whilst trying to move file: " + e); }
                    break;

                case "copyfile":
                    if (!CheckArgCount(args, 3, true)) break;
                    try { ExtendedFilesystem.CopyFile(args[1], args[2]); }
                    catch (Exception e) { System.Console.WriteLine("Error whilst trying to copy file: " + e); }
                    break;

                case "help":
                    if (args.Length > 1) { log(ThemeManager.ErrorText, "Too many arguments"); break; }
                    Commands.Help.Main();
                    break;

                case "go":
                    if (!CheckArgCount(args, 2, true)) break;
                    HandleGoCommand(args);
                    break;

                case "run":
                    if (!CheckArgCount(args, 2, true)) break;
                    if (oldCode) Commands.Run.Main(args[1]);
                    else GoCode.GoCode.Run(args[1]);
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
                    if (args.Length > 1) { log(ThemeManager.ErrorText, "Too many arguments"); break; }
                    NavigateToParentDirectory();
                    break;

                case "cdr":
                    if (args.Length > 1) { log(ThemeManager.ErrorText, "Too many arguments"); break; }
                    Directory.SetCurrentDirectory(@"0:\");
                    break;

                case "dir":
                case "ls":
                    if (args.Length > 1) { log(ThemeManager.ErrorText, "Too many arguments"); break; }
                    Commands.Dir.Run();
                    break;

                case "notepad":
                    if (!CheckRamAndArguments(totalRam, args, 2)) break;
                    if (EndsWithIgnoreCase(args[1], ".gms") && !devMode)
                    {
                        log(ThemeManager.ErrorText, "Files that end with .gms cannot be opened. they are protected files.");
                        break;
                    }
                    textcolour(ThemeManager.Default);
                    {
                        string p = Util.Paths.JoinPaths(currentdirfix, args[1]);
                        var editor = new TextEditor(p);
                        editor.Start();
                    }
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
                    {
                        if (!CheckArgCount(args, 2, true)) break;
                        var fl = new DotNetFile(Directory.GetCurrentDirectory() + args[1]);
                        DotNetClr.DotNetClr clr = new DotNetClr.DotNetClr(fl, @"0:\framework");
                        clr.Start();
                    }
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

        private static bool EndsWithIgnoreCase(string text, string suffix)
        {
            int tl = text.Length, sl = suffix.Length;
            if (sl > tl) return false;
            int off = tl - sl;
            for (int i = 0; i < sl; i++)
            {
                char a = text[off + i];
                char b = suffix[i];
                if (a >= 'A' && a <= 'Z') a = (char)(a + 32);
                if (b >= 'A' && b <= 'Z') b = (char)(b + 32);
                if (a != b) return false;
            }
            return true;
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
                log(ThemeManager.ErrorText, "This programme has been disabled due to low RAM.");
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

                if (lastSlashIndex > 2) // keep "0:\"
                {
                    Directory.SetCurrentDirectory(currentDir.Remove(lastSlashIndex));
                }
                else
                {
                    Directory.SetCurrentDirectory(@"0:\");
                }
            }
            catch
            {
                // Silently fail; fall back to root to be safe.
                Directory.SetCurrentDirectory(@"0:\");
            }
        }

        private void HandleGoCommand(string[] args)
        {
            string sub = args[1];
            if (sub == "type")
            {
                log(Color.Minty, "GoOS - Application Types");
                log(Color.GoogleYellow, "-g Goexe");
                log(Color.GoogleYellow, "-9 9xCode");
                return;
            }

            if (sub == "install")
            {
                if (args.Length != 5)
                {
                    log(ThemeManager.ErrorText, "X: go install <repo> <appname> -<type>");
                    return;
                }

                DownloadApplication(args[2], args[3], args[4]);
                return;
            }

            log(ThemeManager.ErrorText, "Unknown request.");
        }

        private void DownloadApplication(string repo, string fileToGet, string typeFlag)
        {
            try
            {
                string type;
                if (typeFlag == "-g") type = "goexe";
                else if (typeFlag == "-9") type = "9xc";
                else
                {
                    log(ThemeManager.ErrorText, "Unknown application type");
                    return;
                }

                log(Color.Red, "Downloading " + fileToGet + "." + type + " from " + repo);

                // Resolve host with Cosmos DNS client
                var dnsClient = new DnsClient();
                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                dnsClient.SendAsk(repo);
                Address address = dnsClient.Receive();
                dnsClient.Close();

                // Open TCP and GET
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(address.ToString(), 80);
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        string request =
                            "GET /" + fileToGet + "." + type + " HTTP/1.1\r\n" +
                            "User-Agent: GoOS\r\n" +
                            "Accept: */*\r\n" +
                            "Accept-Encoding: identity\r\n" +
                            "Host: " + repo + "\r\n" +
                            "Connection: close\r\n\r\n";

                        byte[] dataToSend = Encoding.ASCII.GetBytes(request);
                        stream.Write(dataToSend, 0, dataToSend.Length);

                        string filePath = @"0:\" + fileToGet + "." + type;

                        // Receive with header detection; binary-safe, multi-read
                        byte[] buffer = new byte[4096];
                        int headerEndAt = -1;
                        int bytesRead;
                        int totalBuffered = 0;
                        byte[] headerBuf = new byte[8192]; // should be ample for simple servers
                        int headerLen = 0;

                        // 1) Read until we’ve found \r\n\r\n
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            // while headers not finished, accumulate into headerBuf
                            if (headerEndAt < 0)
                            {
                                int toCopy = bytesRead;
                                if (headerLen + toCopy > headerBuf.Length)
                                {
                                    toCopy = headerBuf.Length - headerLen;
                                }
                                for (int i = 0; i < toCopy; i++) headerBuf[headerLen + i] = buffer[i];
                                headerLen += toCopy;

                                // scan for CRLFCRLF in headerBuf
                                headerEndAt = FindHeaderEnd(headerBuf, headerLen);
                                if (headerEndAt >= 0)
                                {
                                    // parse status line quickly
                                    int firstLineEnd = IndexOfCrlf(headerBuf, 0, headerEndAt + 4);
                                    string statusLine = GetAscii(headerBuf, 0, firstLineEnd);
                                    // Expect "HTTP/1.1 200"
                                    if (statusLine.IndexOf(" 200") < 0)
                                    {
                                        Dialogue.Show("GoOS Update", "HTTP error: " + statusLine, default, WindowManager.errorIcon);
                                        return;
                                    }

                                    // open file and write remainder of this buffer after header
                                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                                    {
                                        int bodyFromHeaderBuf = headerLen - (headerEndAt + 4);
                                        if (bodyFromHeaderBuf > 0)
                                        {
                                            fs.Write(headerBuf, headerEndAt + 4, bodyFromHeaderBuf);
                                        }

                                        // also write remaining bytes from current buffer that were not copied to headerBuf
                                        int remainingFromCurrentRead = bytesRead - toCopy;
                                        if (remainingFromCurrentRead > 0)
                                        {
                                            fs.Write(buffer, toCopy, remainingFromCurrentRead);
                                        }

                                        // 2) stream the rest of the body
                                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            fs.Write(buffer, 0, bytesRead);
                                        }
                                    }

                                    log(Color.Green, "Downloaded " + fileToGet + "." + type);
                                    return;
                                }
                                else
                                {
                                    // headers still not found, continue reading
                                    if (headerLen == headerBuf.Length)
                                    {
                                        Dialogue.Show("GoOS Update", "Invalid HTTP response (headers too large).", default, WindowManager.errorIcon);
                                        return;
                                    }
                                    continue;
                                }
                            }
                        }

                        Dialogue.Show("GoOS Update", "Invalid or empty HTTP response!", default, WindowManager.errorIcon);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static int FindHeaderEnd(byte[] buf, int len)
        {
            // find "\r\n\r\n"
            for (int i = 0; i <= len - 4; i++)
            {
                if (buf[i] == 13 && buf[i + 1] == 10 && buf[i + 2] == 13 && buf[i + 3] == 10)
                    return i;
            }
            return -1;
        }

        private static int IndexOfCrlf(byte[] buf, int start, int max)
        {
            for (int i = start; i < max - 1; i++)
            {
                if (buf[i] == 13 && buf[i + 1] == 10) return i;
            }
            return -1;
        }

        private static string GetAscii(byte[] buf, int start, int endExclusive)
        {
            int n = endExclusive - start;
            if (n <= 0) return "";
            return Encoding.ASCII.GetString(buf, start, n);
        }

        private void HandleDefaultCommand(string[] args)
        {
            if (isGCIenabled)
            {
                GoCodeInstaller.CheckForInstalledPrograms();
            }

            string key = args[0];
            if (InstalledPrograms.ContainsKey(key))
            {
                string currentDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(@"0:\");

                string location;
                if (InstalledPrograms.TryGetValue(key, out location) && location != null)
                {
                    string actualLocation = location.Replace(@"0:\", "");

                    string low = location.ToLower();
                    if (EndsWithIgnoreCase(low, ".goexe") || EndsWithIgnoreCase(low, ".gexe"))
                        Commands.Run.Main(actualLocation);
                    else if (EndsWithIgnoreCase(low, ".9xc"))
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
