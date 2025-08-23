using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.System;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoOS;
using Gold.Graphics;
using Gold.Graphics.Fonts;
using GoOS.Commands;
using GoOS.Themes;
using LibDotNetParser.CILApi;

namespace GoOS.GUI.Apps
{
    public class Terminal : Window
    {
        private bool _reading;

        internal SVGAIITerminal terminal;

        public Terminal()
        {
            Contents = new Canvas(640, 480);

            terminal = new SVGAIITerminal(Contents.Width, Contents.Height, Resources.TerminalFont, UpdateRequest);

            Title = "Terminal";
            Visible = true;
            Closable = true;
            Unkillable = true;
            SetDock(WindowDock.Auto);
        }

        public override void HandleRun()
        {
        }

        public override void HandleKey(KeyEvent key)
        {
            terminal.KeyBuffer.Enqueue(key);

            if (key.Key == ConsoleKeyEx.Enter)
                ProcessCommand(terminal.ReadLine().Trim().Split(' '));
        }

        private void UpdateRequest()
        {
            unsafe
            {
                terminal.Contents.CopyTo(Contents.Internal);
            }
        }

        private void ProcessCommand(string[] args)
        {
            string cmd0 = args[0];

            switch (cmd0)
            {
                case "fm":
                    WindowManager.AddWindow(new GUI.Apps.Gosplorer.MainFrame());
                    break;

                case "exit":
                    //Console.Visible = false;
                    break;

                /*case "ping":
                    Ping.Run();
                    break;*/

                case "install":
                    if (!CheckArgCount(args, 2, true)) break;
                    Commands.GoCodeInstaller.Install(this, args[1]);
                    break;

                case "uninstall":
                    if (!CheckArgCount(args, 2, true)) break;
                    Commands.GoCodeInstaller.Uninstall(args[1]);
                    break;

                case "movefile":
                    if (!CheckArgCount(args, 3, true)) break;
                    try
                    {
                        Commands.ExtendedFilesystem.MoveFile(args[1], args[2]);
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

                    Commands.Help.Main(this);
                    break;

                case "go":
                    if (!CheckArgCount(args, 2, true)) break;
                    HandleGoCommand(args);
                    break;

                case "run":
                    if (!CheckArgCount(args, 2, true)) break;
                    GoCode.GoCode.Run(this, args[1]);
                    break;

                case "mkdir":
                    if (!CheckArgCount(args, 2, true)) break;
                    Make.MakeDirectory(args[1]);
                    break;

                case "mkfile":
                    if (!CheckArgCount(args, 2, true)) break;
                    Make.MakeFile(args[1]);
                    break;

                case "deldir":
                    if (!CheckArgCount(args, 2, true)) break;
                    Delete.DeleteDirectory(terminal, args[1]);
                    break;

                case "delfile":
                    if (!CheckArgCount(args, 2, true)) break;
                    Delete.DeleteFile(terminal, args[1]);
                    break;

                case "del":
                    if (!CheckArgCount(args, 2, true)) break;
                    Delete.UniversalDelete(terminal, args[1]);
                    break;

                case "cd":
                    if (!CheckArgCount(args, 2, true)) break;
                    Cd.Run(terminal, olddir, args[1]);
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

                case "ls" or "dir":
                    if (args.Length > 1)
                    {
                        log(ThemeManager.ErrorText, "Too many arguments");
                        break;
                    }

                    Commands.Dir.Run(this);
                    break;

                case "edit":
                    if (!CheckRamAndArguments((int)Cosmos.Core.CPU.GetAmountOfRAM(), args, 2)) break;
                    if (EndsWithIgnoreCase(args[1], ".gms") && !Kernel.devMode)
                    {
                        log(ThemeManager.ErrorText,
                            "Files that end with .gms cannot be opened. they are protected files.");
                        break;
                    }

                    terminal.ForegroundColor = ThemeManager.Default;
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
                    //Console.Clear();
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
                    break;
                }

                case "9xcode":
                    if (!CheckArgCount(args, 2, true)) break;
                    _9xCode.Interpreter.Run(this, Directory.GetCurrentDirectory() + args[1]);
                    break;

                default:
                    HandleDefaultCommand(args);
                    break;
            }
        }

        private void HandleDefaultCommand(string[] args)
        {
            if (Kernel.isGCIenabled) GoCodeInstaller.CheckForInstalledPrograms(this);

            string key = args[0];
            if (Kernel.InstalledPrograms.ContainsKey(key))
            {
                string currentDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(@"0:\");

                string location;
                if (Kernel.InstalledPrograms.TryGetValue(key, out location) && location != null)
                {
                    // Interpret entries under 0:\ as relative for runners
                    string actualLocation = location.StartsWith(@"0:\")
                        ? location.Substring(3)
                        : location;

                    // Decide once by extension (case-insensitive)
                    string ext = Kernel.GetExtension(actualLocation);

                    if (Kernel.EqualsIgnoreCase(ext, ".goexe") || Kernel.EqualsIgnoreCase(ext, ".gexe"))
                    {
                        Commands.Run.Main(this, actualLocation);
                    }
                    else if (Kernel.EqualsIgnoreCase(ext, ".9xc"))
                    {
                        // Using _9xCode.Interpreter via using GoOS._9xCode
                        _9xCode.Interpreter.Run(this, actualLocation);
                    }
                    else
                    {
                        // Silent unknown type (keeps original semantics)
                        // Console.WriteLine("Unknown application type.");
                    }
                }

                Directory.SetCurrentDirectory(currentDir);
            }
            else
            {
                //Console.WriteLine("Invalid command.");
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

        private void NavigateToParentDirectory()
        {
            try
            {
                string currentDir = Directory.GetCurrentDirectory().TrimEnd('\\');
                int lastSlashIndex = currentDir.LastIndexOf('\\');

                if (lastSlashIndex > 2) // keep "0:\"
                    Directory.SetCurrentDirectory(currentDir.Remove(lastSlashIndex));
                else
                    Directory.SetCurrentDirectory(@"0:\");
            }
            catch
            {
                Directory.SetCurrentDirectory(@"0:\");
            }
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

        public bool CheckArgCount(string[] args, int expectedCount, bool exact)
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

        public void log(Color colour, string str)
        {
            terminal.ForegroundColor = colour;
            terminal.WriteLine(str);
        }

        public void DownloadApplication(string repo, string fileToGet, string typeFlag)
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

                // DNS resolve
                var dnsClient = new DnsClient();
                dnsClient.Connect(DNSConfig.DNSNameservers[0]);
                dnsClient.SendAsk(repo);
                Address address = dnsClient.Receive();
                dnsClient.Close();

                // TCP GET
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

                        byte[] buffer = new byte[4096];
                        int headerEndAt = -1;
                        int bytesRead;
                        int headerLen = 0;
                        byte[] headerBuf = new byte[8192];

                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (headerEndAt < 0)
                            {
                                int toCopy = bytesRead;
                                if (headerLen + toCopy > headerBuf.Length)
                                    toCopy = headerBuf.Length - headerLen;

                                for (int i = 0; i < toCopy; i++)
                                    headerBuf[headerLen + i] = buffer[i];
                                headerLen += toCopy;

                                headerEndAt = FindHeaderEnd(headerBuf, headerLen);
                                if (headerEndAt >= 0)
                                {
                                    int firstLineEnd = IndexOfCrlf(headerBuf, 0, headerEndAt + 4);
                                    string statusLine = GetAscii(headerBuf, 0, firstLineEnd);
                                    if (statusLine.IndexOf(" 200") < 0)
                                    {
                                        Dialogue.Show("GoOS Update", "HTTP error: " + statusLine, default,
                                            WindowManager.errorIcon);
                                        return;
                                    }

                                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                                    {
                                        // body bytes already in headerBuf
                                        int bodyFromHeaderBuf = headerLen - (headerEndAt + 4);
                                        if (bodyFromHeaderBuf > 0)
                                            fs.Write(headerBuf, headerEndAt + 4, bodyFromHeaderBuf);

                                        // any unread portion of current buffer (not copied into headerBuf)
                                        int remainingFromCurrentRead = bytesRead - toCopy;
                                        if (remainingFromCurrentRead > 0)
                                            fs.Write(buffer, toCopy, remainingFromCurrentRead);

                                        // stream remaining body
                                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                            fs.Write(buffer, 0, bytesRead);
                                    }

                                    log(Color.Green, "Downloaded " + fileToGet + "." + type);
                                    return;
                                }

                                if (headerLen == headerBuf.Length)
                                {
                                    Dialogue.Show("GoOS Update", "Invalid HTTP response (headers too large).", default,
                                        WindowManager.errorIcon);
                                    return;
                                }

                                continue;
                            }
                        }

                        Dialogue.Show("GoOS Update", "Invalid or empty HTTP response!", default,
                            WindowManager.errorIcon);
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }
        }

        private static int FindHeaderEnd(byte[] buf, int len)
        {
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
                if (buf[i] == 13 && buf[i + 1] == 10)
                    return i;
            return -1;
        }

        private static string GetAscii(byte[] buf, int start, int endExclusive)
        {
            int n = endExclusive - start;
            if (n <= 0) return "";
            return Encoding.ASCII.GetString(buf, start, n);
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

        public string olddir = @"0:\";
        public string currentdirfix = string.Empty;
        
        public void DrawPrompt()
        {
            string currentdir = Directory.GetCurrentDirectory();
            if (!currentdir.EndsWith("\\")) currentdir += "\\";

            // normalise 0:\ path artefacts
            if (currentdir.IndexOf(@"0:\\\") >= 0)
                currentdirfix = currentdir.Replace(@"0:\\\", @"0:\");
            else if (currentdir.IndexOf(@"0:\\") >= 0)
                currentdirfix = currentdir.Replace(@"0:\\", @"0:\");
            else
                currentdirfix = currentdir;

            terminal.ForegroundColor = ThemeManager.WindowText;
            terminal.Write(Kernel.username);
            terminal.ForegroundColor = ThemeManager.Other1;
            terminal.Write("@");
            terminal.ForegroundColor = ThemeManager.WindowText;
            terminal.Write(Kernel.computername + " ");
            terminal.ForegroundColor = ThemeManager.WindowBorder;
            terminal.Write(currentdirfix);
            terminal.ForegroundColor = ThemeManager.Default;
        }
    }
}