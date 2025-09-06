using System;
using System.IO;
using Cosmos.System;

namespace GoOS.GUI.Apps;

using SVGAIITerminal;

public class NewConsole : Window
{
    public SVGAIITerminal terminal;

    public NewConsole()
    {
        terminal = new SVGAIITerminal(640, 400, Resources.TerminalFont, null, null);
        Contents = terminal.Contents;
        Title = "SVGAIITerminal";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        terminal.Clear();
        terminal.WriteLine(File.ReadAllText(@"0:\content\motd.txt"));
        DrawPrompt();
    }

    private string CWD = @"0:\";

    private void DrawPrompt()
    {
        terminal.Write(Kernel.username, ConsoleColor.Cyan);
        terminal.Write("@", ConsoleColor.Yellow);
        terminal.Write(Kernel.computername, ConsoleColor.Cyan);
        terminal.Write(" ");
        terminal.Write(CWD, ConsoleColor.Green);
        terminal.Write(" # ");
        startX = terminal.CursorLeft;
        startY = terminal.CursorTop;
    }

    private void ParseInput()
    {
        string[] args = input.Trim().Split(' ');

        try
        {
            switch (args[0])
            {
                case "":
                    break;
                case "help":
                    terminal.Write(
                        "GoOS Terminal v1.6\n" +
                        "These shell commands are defined internally. Type `help' to see this list.\n" +
                        "\n" +
                        "A star (*) next to a name means that the command is disabled.\n" +
                        "\n" +
                        "about                                   mkdir DIR\n" +
                        "cat FILE                                notepad [FILE]\n" +
                        "cd DIR                                  paint | paintbrush\n" +
                        "clock                                   ping\n" +
                        "cmd | terminal                          rm FILE\n" +
                        "control | settings                      rmdir DIR\n" +
                        "echo [ARGS...]                          store | gostore\n" +
                        "exit                                    taskmgr | taskman | sysmon\n" +
                        "fm | explorer | gosplorer [DIR]         touch FILE\n" +
                        "gimviewer FILE                          goide [FILE]\n" +
                        "goweb                                   ls\n" +
                        "help                                    clear\n"
                        );
                    break;
                case "ping":
                    terminal.WriteLine("Pong!");
                    break;
                case "echo":
                    for (int i = 1; i < args.Length; i++)
                    {
                        terminal.Write($"{args[i]} ");
                    }
                    terminal.WriteLine();
                    break;
                case "exit":
                    Dispose();
                    break;
                case "ls":
                    if (Directory.GetDirectories(CWD).Length > 0)
                        terminal.Write(string.Join("  ", Directory.GetDirectories(CWD)).Trim() + "  ", ConsoleColor.Blue);
                    if (Directory.GetFiles(CWD).Length > 0)
                        terminal.Write(string.Join("  ", Directory.GetFiles(CWD)).Trim(), ConsoleColor.Red);
                    terminal.WriteLine();
                    break;
                case "cd":
                    {
                        string target = CWD;
                        if (args[1] == ".")
                        {
                            break;
                        }
                        else if (args[1] == "..")
                        {
                            if (CWD != @"0:\")
                            {
                                try
                                {
                                    target = Path.GetDirectoryName(CWD.TrimEnd('\\')) ?? @"0:\";
                                }
                                catch
                                {
                                    target = @"0:\";
                                }
                            }
                        }
                        else
                        {
                            target = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        }
                        if (!Directory.Exists(target))
                        {
                            terminal.WriteLine($"cd: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        CWD = target;
                    }
                    break;
                case "cat":
                    {
                        string path = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        if (!File.Exists(path))
                        {
                            terminal.WriteLine($"cat: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        terminal.WriteLine(File.ReadAllText(path));
                    }
                    break;
                case "touch":
                    File.Create(args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]));
                    break;
                case "mkdir":
                    Directory.CreateDirectory(args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]));
                    break;
                case "rm":
                    {
                        string path = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        if (!File.Exists(path))
                        {
                            terminal.WriteLine($"rm: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        File.Delete(path);
                    }
                    break;
                case "rmdir":
                    {
                        string path = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        if (!Directory.Exists(path))
                        {
                            terminal.WriteLine($"rmdir: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        Directory.Delete(path);
                    }
                    break;
                case "about":
                    WindowManager.AddWindow(new About());
                    break;
                case "clock":
                    WindowManager.AddWindow(new Clock());
                    break;
                case "gimviewer:":
                    {
                        string path = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        if (!File.Exists(path))
                        {
                            terminal.WriteLine($"gimviewer: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        int type = 0;
                        switch (args[1].Split('.')[1])
                        {
                            case "bmp": type = 0; break;
                            case "png": type = 1; break;
                            case "ppm": type = 2; break;
                            case "tga": type = 3; break;
                        }
                        WindowManager.AddWindow(new Gimviewer(File.ReadAllBytes(path), type));
                    }
                    break;
                case "cmd":
                case "terminal":
                    WindowManager.AddWindow(new NewConsole());
                    break;
                case "notepad":
                    if (args.Length < 2)
                    {
                        WindowManager.AddWindow(new Notepad());
                        break;
                    }
                    {
                        string path = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        if (!File.Exists(path))
                        {
                            terminal.WriteLine($"notepad: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        WindowManager.AddWindow(new Notepad(path));
                    }
                    break;
                case "paint":
                case "paintbrush":
                    WindowManager.AddWindow(new Paintbrush());
                    break;
                case "taskmgr":
                case "taskman":
                case "sysmon":
                    WindowManager.AddWindow(new TaskManager());
                    break;
                case "goide":
                    if (args.Length < 2)
                    {
                        WindowManager.AddWindow(new GoIDE.ProjectsFrame());
                        break;
                    }
                    {
                        string path = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        if (!File.Exists(path))
                        {
                            terminal.WriteLine($"goide: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        WindowManager.AddWindow(new GoIDE.IDEFrame(path.Remove(path.LastIndexOf(".")).Substring(1), path, path.EndsWith(".9xc")));
                    }
                    break;
                case "control":
                case "settings":
                    WindowManager.AddWindow(new Settings.Frame());
                    break;
                case "goweb":
                    WindowManager.AddWindow(new GoWeb.GoWebWindow());
                    break;
                case "store":
                case "gostore":
                    WindowManager.AddWindow(new GoStore.MainFrame());
                    break;
                case "fm":
                case "explorer":
                case "gosplorer":
                    if (args.Length < 2)
                    {
                        WindowManager.AddWindow(new Gosplorer.MainFrame());
                        break;
                    }
                    {
                        string path = args[1].Contains(':') ? args[1] : Path.Combine(CWD, args[1]);
                        if (!Directory.Exists(path))
                        {
                            terminal.WriteLine($"gosplorer: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                            break;
                        }
                        WindowManager.AddWindow(new Gosplorer.MainFrame(path));
                    }
                    break;
                case "clear":
                    terminal.Clear();
                    break;
                default:
                    terminal.WriteLine($"{args[0]}: not found", ConsoleColor.Red);
                    break;
            }
        }
        catch (Exception ex)
        {
            terminal.WriteLine($"{args[0]}: ${ex.Message}", ConsoleColor.Red);
        }
    }

    private string input = string.Empty;
    private int startX = 0, startY = 0;

    public override void HandleRun()
    {
        if (Closing)
        {
            terminal.KeyBuffer.Enqueue(new KeyEvent() { Key = ConsoleKeyEx.Enter });
        }

        terminal.ForceDrawCursor();
        terminal.TryDrawCursor();

        if (!terminal.KeyBuffer.TryDequeue(out var key))
        {
            return;
        }

        switch (key.Key)
        {
            case ConsoleKeyEx.Enter:
                terminal.ForceDrawCursor(true);
                terminal.TryScroll();

                terminal.CursorLeft = 0;
                terminal.CursorTop++;
                startX = terminal.CursorLeft;
                startY = terminal.CursorTop;

                ParseInput();
                input = string.Empty;
                DrawPrompt();
                break;

            case ConsoleKeyEx.Backspace:
                if (!(terminal.CursorLeft == startX && terminal.CursorTop == startY))
                {
                    Contents.DrawFilledRectangle(terminal._fontWidth * terminal.CursorLeft, terminal.Font.GetHeight() * terminal.CursorTop, terminal._fontWidth, (ushort)terminal.Font.GetHeight(), 0, terminal.BackgroundColor);
                    terminal.CursorTop -= terminal.CursorLeft == 0 ? 1 : 0;
                    terminal.CursorLeft -= terminal.CursorLeft == 0 ? terminal.Width - 1 : 1;
                    Contents.DrawFilledRectangle(terminal._fontWidth * terminal.CursorLeft, terminal.Font.GetHeight() * terminal.CursorTop, terminal._fontWidth, (ushort)terminal.Font.GetHeight(), 0, terminal.BackgroundColor);

                    input = input.Remove(input.Length - 1);
                }

                terminal.ForceDrawCursor();
                break;

            case ConsoleKeyEx.Tab:
                terminal.Write('\t');
                input += SVGAIITerminal.TabIndentation;

                terminal.ForceDrawCursor();
                break;

            default:
                if (KeyboardManager.ControlPressed && key.Key == ConsoleKeyEx.L)
                {
                    terminal.Clear();
                    input = string.Empty;
                }

                if (key.KeyChar >= 32 && key.KeyChar < 128)
                {
                    terminal.Write(key.KeyChar.ToString());
                    input += key.KeyChar;
                }

                terminal.ForceDrawCursor();
                break;
        }
    }

    public override void HandleKey(KeyEvent key)
    {
        terminal.KeyBuffer.Enqueue(key);
    }
}