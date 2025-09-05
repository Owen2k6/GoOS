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
        terminal = new SVGAIITerminal(720, 400, Resources.TerminalFont, null, null);
        Contents = terminal.Contents;
        Title = "SVGAIITerminal";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        terminal.WriteLine("GoOS Terminal v1.6\n");
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
    }

    private void ParseInput()
    {
        string[] args = input.Trim().Split(' ');

        switch (args[0])
        {
            case "":
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
                    target = Path.Combine(CWD, args[1]);
                }
                if (!Directory.Exists(target))
                {
                    terminal.WriteLine($"cd: No such file or directory \"{args[1]}\"", ConsoleColor.Red);
                    break;
                }
                CWD = target;
                break;
            default:
                terminal.WriteLine($"{args[0]}: not found", ConsoleColor.Red);
                break;
        }
    }

    private string input = string.Empty;
    private int startX = -1, startY = -1;

    public override void HandleRun()
    {
        if (Closing)
        {
            terminal.KeyBuffer.Enqueue(new KeyEvent() { Key = ConsoleKeyEx.Enter });
        }

        terminal.ForceDrawCursor();

        if (startX == -1 || startY == -1)
        {
            startX = terminal.CursorLeft;
            startY = terminal.CursorTop;
        }

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