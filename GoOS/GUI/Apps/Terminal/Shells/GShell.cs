namespace GoOS.GUI.Apps.Terminal.Shells;

using System;
using System.IO;
using GoOS.Tasking;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoOS;
using Gold.Graphics;
using Gold.Graphics.Fonts;
using GoOS.Commands;
using GoOS.Themes;
using LibDotNetParser.CILApi;
using System;
using static GoOS.Kernel;

internal sealed class GShell : Shell
{
    private bool _reading = false;

    private string _virtualDir = @"0:\";

    public GShell(SVGAIITerminal Terminal) : base(Terminal)
    {
    }

    internal override void Run()
    {
        if (!_reading)
        {
            _reading = true;

            _terminal.Write("# ");

            string[] args = _terminal.ReadLine().Trim().Split(' ');

            switch (args[0].ToLower())
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
                    Delete.DeleteDirectory(_terminal, args[1]);
                    break;

                case "delfile":
                    if (!CheckArgCount(args, 2, true)) break;
                    Delete.DeleteFile(_terminal, args[1]);
                    break;

                case "del":
                    if (!CheckArgCount(args, 2, true)) break;
                    Delete.UniversalDelete(_terminal, args[1]);
                    break;

                case "cd":
                    if (!CheckArgCount(args, 2, true)) break;
                    Cd.Run(_terminal, olddir, args[1]);
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

                    _terminal.ForegroundColor = ThemeManager.Default;
                    string p = Util.Paths.JoinPaths(currentdirfix, args[1]);
                    var editor = new TextEditor(p);
                    editor.Start();
                    
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

            _reading = false;
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

        _terminal.ForegroundColor = ThemeManager.WindowText;
        _terminal.Write(Kernel.username);
        _terminal.ForegroundColor = ThemeManager.Other1;
        _terminal.Write("@");
        _terminal.ForegroundColor = ThemeManager.WindowText;
        _terminal.Write(Kernel.computername + " ");
        _terminal.ForegroundColor = ThemeManager.WindowBorder;
        _terminal.Write(currentdirfix);
        _terminal.ForegroundColor = ThemeManager.Default;
    }
}