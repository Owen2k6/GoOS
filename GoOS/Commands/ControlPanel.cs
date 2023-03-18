using System;
using System.Collections.Generic;
using System.Text;
using static System.ConsoleColor;
using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;
using System.Linq.Expressions;
using Cosmos.Core.Memory;
using System.Drawing;
using IL2CPU.API.Attribs;
using System.Text;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.System.Network.IPv4.UDP;
using System.Diagnostics;
using GoOS;
using Cosmos.HAL.BlockDevice.Registers;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using Cosmos.System;
using Console = System.Console;
using System.Linq;
using Cosmos.HAL.BlockDevice;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;

namespace GoOS.ControlPanel
{
    public static class ControlPanel
    {
        #region CP737

        /// <summary>
        /// Prints characters on the CP737 code-page.
        /// </summary>
        private static class CP737Console
        {
            public static readonly Cosmos.System.Console console = new(null);

            public static readonly Dictionary<char, byte> unicodeToCP737
                = new()
                {
            {  '░', 0xB0 }, {  '▒', 0xB1 },
            {  '▓', 0xB2 }, {  '│', 0xB3 },
            {  '┤', 0xB4 }, {  '╡', 0xB5 },
            {  '╢', 0xB6 }, {  '╖', 0xB7 },
            {  '╕', 0xB8 }, {  '╣', 0xB9 },
            {  '║', 0xBA }, {  '╗', 0xBB },
            {  '╝', 0xBC }, {  '╜', 0xBD },
            {  '╛', 0xBE }, {  '┐', 0xBF },
            {  '└', 0xC0 }, {  '┴', 0xC1 },
            {  '┬', 0xC2 }, {  '├', 0xC3 },
            {  '─', 0xC4 }, {  '┼', 0xC5 },
            {  '╞', 0xC6 }, {  '╟', 0xC7 },
            {  '╚', 0xC8 }, {  '╔', 0xC9 },
            {  '╩', 0xCA }, {  '╦', 0xCB },
            {  '╠', 0xCC }, {  '═', 0xCD },
            {  '╬', 0xCE }, {  '╧', 0xCF },
            {  '╨', 0xD0 }, {  '╤', 0xD1 },
            {  '╥', 0xD2 }, {  '╙', 0xD3 },
            {  '╘', 0xD4 }, {  '╒', 0xD5 },
            {  '╓', 0xD6 }, {  '╫', 0xD7 },
            {  '╪', 0xD8 }, {  '┘', 0xD9 },
            {  '┌', 0xDA }, {  '█', 0xDB },
            {  '▄', 0xDC }, {  '▌', 0xDD },
            {  '▐', 0xDE }, {  '▀', 0xDF },
            {  '■', 0xFE }
                };

            /// <summary>
            /// Writes the given characters at the current position.
            /// </summary>
            /// <param name="line">The line to write.</param>
            /// <param name="x">The X coordinate to write the text to. If set to a negative value, the current cursor position will be used.</param>
            /// <param name="y">The Y coordinate to write the text to. If set to a negative value, the current cursor position will be used.</param>
            public static void Write(string line, int x = -1, int y = -1)
            {
                console.CursorVisible = Console.CursorVisible;
                console.Background = Console.BackgroundColor;
                console.Foreground = Console.ForegroundColor;
                console.X = Console.CursorLeft;
                console.Y = Console.CursorTop;

                if (x < 0) x = Console.CursorLeft;
                if (y < 0) y = Console.CursorTop;

                Span<byte> encodingBuffer = stackalloc byte[1];
                Span<char> inputBuffer = stackalloc char[1];

                for (int i = 0; i < line.Length; i++)
                {
                    console.X = x;
                    console.Y = y;

                    if (line[i] == '\n')
                    {
                        x = 0;
                        y++;
                        continue;
                    }

                    if (unicodeToCP737.TryGetValue(line[i], out byte mapped))
                    {
                        console.Write(mapped);
                        if (console.Y > 24)
                        {
                            console.X = 0;
                            console.Y = 24;
                        }
                    }
                    else
                    {
                        inputBuffer[0] = line[i];
                        Encoding.ASCII.GetBytes(inputBuffer, encodingBuffer);
                        console.Write(encodingBuffer[0]);
                    }

                    x++;
                    if (x > Console.WindowWidth)
                    {
                        x = 0;
                        y++;
                    }

                    if (y > Console.WindowHeight)
                    {
                        // stop character printing
                        break;
                    }
                }
            }
        }

#endregion

        public static void Open()
        {
            Console.CursorVisible = false;
            DrawFrame();
            Run();
        }

        private static void DrawFrame()
        {
            Console.Clear();
            try
            {
                Console.BackgroundColor = Black;
                Console.ForegroundColor = Red;
                CP737Console.Write("╔═══════════════════════════ GoplexOS Control Panel ═══════════════════════════╗\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "║                                                                              ║\n" +
                                   "╚══════════════════════════════════════════════════════════════════════════════");
                if (CP737Console.unicodeToCP737.TryGetValue('╝', out byte mapped))
                {
                    CP737Console.console.mText[79, 24] = mapped;
                }
            }
            catch {}
        }

        private static void DrawMainText()
        {
            Console.SetCursorPosition(46, 11);
            Console.BackgroundColor = Black;
            Console.ForegroundColor = Red;
            Console.Write("Change Computer Name");
            Console.SetCursorPosition(18, 11);
            Console.BackgroundColor = Red;
            Console.ForegroundColor = White;
            Console.Write("Change Username");
        }

        private static void MessageBox()
        {
            CP737Console.Write("╔════════════ Info ════════════╗", 24, 10);
            CP737Console.Write("║                              ║", 24, 11);
            CP737Console.Write("║ Contents saved successfully. ║", 24, 12);
            CP737Console.Write("║                              ║", 24, 13);
            CP737Console.Write("╚══════════════════════════════╝", 24, 14);
            Console.ReadKey();
        }

        private static void Run()
        {
            bool running = true;
            string menu = "main";
            string selected = "change username";

            DrawMainText();

            while (running)
            {
                if (menu == "main")
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Tab:
                           // Console.BackgroundColor = Red;
                            Console.ForegroundColor = White;

                            if (selected == "change username")
                            {
                                Console.SetCursorPosition(18, 11);
                                Console.BackgroundColor = Black;
                                Console.ForegroundColor = Red;
                                Console.Write("Change Username");

                                Console.SetCursorPosition(46, 11);
                                Console.BackgroundColor = Red;
                                Console.ForegroundColor = White;
                                Console.Write("Change Computer Name");
                                selected = "change computer name";
                            }
                            else if (selected == "change computer name")
                            {
                                Console.SetCursorPosition(46, 11);
                                Console.BackgroundColor = Black;
                                Console.ForegroundColor = Red;
                                Console.Write("Change Computer Name");

                                Console.SetCursorPosition(18, 11);
                                Console.BackgroundColor = Red;
                                Console.ForegroundColor = White;
                                Console.Write("Change Username");
                                selected = "change username";
                            }
                            break;
                        case ConsoleKey.Enter:
                            if (selected == "change username")
                            {
                                menu = "username";
                            }
                            else if (selected == "change computer name")
                            {
                                menu = "computer name";
                            }
                            break;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                    }
                }

                else if (menu == "username")
                {
                    DrawFrame();
                    Console.SetCursorPosition(2, 11);
                    Console.Write("New Username: ");
                    string thingtosave = Console.ReadLine();

                    var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\setup.gms");
                    var setupstream = setupcontent.GetFileStream();
                    byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {thingtosave}\ncomputername: {Kernel.computername}");
                    setupstream.Write(textToWrite, 0, textToWrite.Length);
                    Kernel.username = thingtosave;

                    MessageBox();
                    menu = "main";
                    selected = "change username";
                    DrawFrame();
                    DrawMainText();
                }

                else if (menu == "computer name")
                {
                    DrawFrame();
                    Console.SetCursorPosition(2, 11);
                    Console.Write("New Computer Name: ");
                    string thingtosave = Console.ReadLine();

                    var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\setup.gms");
                    var setupstream = setupcontent.GetFileStream();
                    byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {Kernel.username}\ncomputername: {thingtosave}");
                    setupstream.Write(textToWrite, 0, textToWrite.Length);
                    Kernel.computername = thingtosave;

                    MessageBox();
                    menu = "main";
                    selected = "change username";
                    DrawFrame();
                    DrawMainText();
                }
            }
            Console.WriteLine();
            Console.Clear();
        }
    }

    
}
