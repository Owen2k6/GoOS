using System;
using System.Collections.Generic;
using System.Text;
using static System.ConsoleColor;
using Sys = Cosmos.System;
using Console = System.Console;

namespace GoOS.ControlPanel
{
    public static class ControlPanel
    {
        // Welcome to the most commented file in GoOS
        //agreed
        // GoOS Core
        public static void print(string str)
        {
            Console.WriteLine(str);
        }
        public static void log(System.ConsoleColor colour, string str)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(str);
        }
        public static void write(string str)
        {
            Console.Write(str);
        }
        public static void textcolour(System.ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
        }
        public static void highlightcolour(System.ConsoleColor colour)
        {
            Console.BackgroundColor = colour;
        }
        public static void sleep(int time)
        {
            System.Threading.Thread.Sleep(time);
        }
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
<<<<<<< Updated upstream
                Console.ForegroundColor = Red;
                CP737Console.Write("╔══════════════════════════════════ Settings ══════════════════════════════════╗\n" +
=======
                Console.ForegroundColor = Green;
                CP737Console.Write("╔══════════════════════════════════════════════════════════════════════════════╗\n" +
>>>>>>> Stashed changes
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
                                   "╚═════[TAB - Selection]═══[ESC - Exit]═════════════════════════════════════════");
                if (CP737Console.unicodeToCP737.TryGetValue('╝', out byte mapped))
                {
                    CP737Console.console.mText[79, 24] = mapped;
                }
            }
            catch { }
        }

<<<<<<< Updated upstream
        private static void DrawMainText()
        {
            int mem = (int)(Kernel.FS.GetTotalSize(@"0:\") / 1000000);
            int screenWidth = 80;
            string title = "System information";
            string q = $"Total Storage (bytes): {mem}";
            string w = $"Total Memory (megabytes): {Cosmos.Core.CPU.GetAmountOfRAM()}";

            int titlePos = (screenWidth / 2) - (title.Length / 2);
            int qPos = (screenWidth / 2) - (q.Length / 2);
            int wPos = (screenWidth / 2) - (w.Length / 2);

            Console.SetCursorPosition(titlePos, 2);
=======
        /// <summary>
        /// Writes a title to the top of the frame.
        /// </summary>
        /// <param name="Title">The title to be written.</param>
        private static void DrawTitle(string Title, int Y)
        {
            int OldX = Console.CursorLeft; int OldY = Console.CursorTop;

            Console.SetCursorPosition(40 - (Title.Length / 2), Y);
            Console.ForegroundColor = Cyan;
            Console.Write(Title);
            Console.SetCursorPosition(OldX, OldY);
        }

        /// <summary>
        /// Write some controls to the bottom of the screen.
        /// </summary>
        /// <param name="Controls">The controls to be written.</param>
        private static void DrawControls(string Controls)
        {
            Console.ForegroundColor = Cyan;
            int OldX = Console.CursorLeft; int OldY = Console.CursorTop;
            Console.SetCursorPosition(6, 24);
            foreach (char c in Controls)
            {
                if (c == '═')
                {
                    Console.CursorLeft++;
                }
                else
                {
                    Console.Write(c);
                }
            }
        }

        /// <summary>
        /// Draws the text of the main screen.
        /// </summary>
        private static void DrawMainText()
        {
            Console.ForegroundColor = Green;
            string title = "System information:";
            string q = "Total Storage (mb): " + (int)Kernel.FS.GetTotalSize(@"0:\") / 1e6;
            string w = "Total Memory (mb): " + Cosmos.Core.CPU.GetAmountOfRAM();

            // We already know that screenWidth is going to be 40, so we set it directly so its faster.
            Console.SetCursorPosition(40 - (title.Length / 2), 2);
            Console.ForegroundColor = Cyan;
>>>>>>> Stashed changes
            Console.Write(title);
            Console.SetCursorPosition(qPos, 3);
            Console.Write(q);
            Console.SetCursorPosition(wPos, 4);
            Console.Write(w);
            Console.ForegroundColor = Green;

<<<<<<< Updated upstream

            setButton("Reset System", 3, 18, 14, Black, Red);
            setButton("Change Username", 2, 46, 11, Black, Red);
            setButton("Change Computer Name", 1, 18, 11, Black, Red);
=======
            MkButton("Change Computer Name", 15, 11, Cyan, Black);
            MkButton("Change Username", 44, 11, Black, Cyan);
            MkButton("Reset System", 33, 14, Black, Cyan);
>>>>>>> Stashed changes
        }

        private static void MessageBox()
        {
<<<<<<< Updated upstream
            CP737Console.Write("╔════════════ Info ════════════╗", 24, 10);
=======
            // TODO: Apply new style
            Console.ForegroundColor = Green;
            CP737Console.Write("╔══════════════════════════════╗", 24, 10);
>>>>>>> Stashed changes
            CP737Console.Write("║                              ║", 24, 11);
            CP737Console.Write("║ Contents saved successfully. ║", 24, 12);
            CP737Console.Write("║                              ║", 24, 13);
            CP737Console.Write("╚══════════════════════════════╝", 24, 14);
<<<<<<< Updated upstream
=======
            DrawTitle("Info", 10);

            Console.ForegroundColor = Cyan;
            Console.SetCursorPosition(26, 12);
            Console.Write("Contents saved successfully.");

>>>>>>> Stashed changes
            Console.ReadKey();
        }

        private static void setButton(string name, int id, int x, int y, ConsoleColor highlight, ConsoleColor colour)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = highlight; 
            Console.ForegroundColor = colour;
            Console.Write(name);
        }

        private static void Run()
        {
            bool running = true;
            string menu = "main";
            int selected = 1;

            int mem = (int)(Kernel.FS.GetTotalSize(@"0:\") / 1000000);
            DrawMainText();

            while (running)
            {
                if (menu == "main")
                {
<<<<<<< Updated upstream
=======
                    Console.ForegroundColor = Cyan;
                    DrawTitle(" Settings ", 0); // Do not remove spaces!
                    DrawControls("═══[ ARROWS - Selection ]═══[ ESC - Exit ]═══[ ENTER - Continue ]");

>>>>>>> Stashed changes
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch (key.Key)
                    {
<<<<<<< Updated upstream
                        case ConsoleKey.Tab:
                            Console.BackgroundColor = Black;
                            Console.ForegroundColor = White;

                            if (selected == 1)
                            {
                                setButton("Change Username", 2, 46, 11, Black, Red);
                                setButton("Change Computer Name", 1, 18, 11, Black, Red);
                                selected = 2;
                            }
                            else if (selected == 2)
                            {
                                setButton("Change Computer Name", 1, 18, 11, Black, Red);
                                setButton("Reset System", 3, 18, 14, Black, Red);
                                selected = 3;
                            }
                            else if (selected == 3)
                            {
                                setButton("Reset System", 3, 18, 14, Black, Red);
                                setButton("Change Username", 2, 46, 11, Black, Red);
                                selected = 1;
=======
                        case ConsoleKey.LeftArrow:
                            if (selected == 2)
                            {
                                MkButton("Change Computer Name", 15, 11, Cyan, Black); // Select button
                                MkButton("Change Username", 44, 11, Black, Cyan); // Deselect button
                                selected = 1;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (selected == 3)
                            {
                                MkButton("Change Computer Name", 15, 11, Cyan, Black); // Select button
                                MkButton("Reset System", 33, 14, Black, Cyan); // Deselect button
                                selected = 1;
                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (selected == 1)
                            {
                                MkButton("Reset System", 33, 14, Cyan, Black); // Select button
                                MkButton("Change Computer Name", 15, 11, Black, Cyan); // Deselect button
                                selected = 3;
                            }
                            else if (selected == 2)
                            {
                                MkButton("Reset System", 33, 14, Cyan, Black); // Select button
                                MkButton("Change Username", 44, 11, Black, Cyan); // Deselect button
                                selected = 3;
                            }
                            break;

                        case ConsoleKey.RightArrow:
                            if (selected == 1)
                            {
                                MkButton("Change Username", 44, 11, Cyan, Black); // Select button
                                MkButton("Change Computer Name", 15, 11, Black, Cyan); // Deselect button
                                selected = 2;
                            }
                            else if (selected == 3)
                            {
                                MkButton("Change Username", 44, 11, Cyan, Black); // Select button
                                MkButton("Reset System", 33, 14, Black, Cyan); // Deselect button
                                selected = 2;
>>>>>>> Stashed changes
                            }
                            break;
                        case ConsoleKey.Enter:
                            if (selected == 1)
                            {
                                menu = "username";
                            }
                            else if (selected == 2)
                            {
                                menu = "computer name";
                            }
                            else if (selected == 3)
                            {
                                menu = "reset system";
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

                    Console.ForegroundColor = Cyan;
                    Console.Write("New Username: ");
                    Console.ForegroundColor = Green;
                    string thingtosave = Console.ReadLine();

                    System.IO.File.Delete(@"0:\content\sys\setup.gms");
                    System.IO.File.Create(@"0:\content\sys\setup.gms");
                    var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\setup.gms");
                    var setupstream = setupcontent.GetFileStream();
                    byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {thingtosave}\ncomputername: {Kernel.computername}");
                    setupstream.Write(textToWrite, 0, textToWrite.Length);
                    Kernel.username = thingtosave;

                    MessageBox();
                    menu = "main";
                    selected = 2;
                    DrawFrame();
                    DrawMainText();
                }

                else if (menu == "computer name")
                {
                    DrawFrame();
                    Console.SetCursorPosition(2, 11);
                    Console.ForegroundColor = Cyan;
                    Console.Write("New Computer Name: ");
                    Console.ForegroundColor = Green;
                    string thingtosave = Console.ReadLine();

                    System.IO.File.Delete(@"0:\content\sys\setup.gms");
                    System.IO.File.Create(@"0:\content\sys\setup.gms");
                    var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\setup.gms");
                    var setupstream = setupcontent.GetFileStream();
                    byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {Kernel.username}\ncomputername: {thingtosave}");
                    setupstream.Write(textToWrite, 0, textToWrite.Length);
                    Kernel.computername = thingtosave;

                    MessageBox();
                    menu = "main";
                    selected = 2;
                    DrawFrame();
                    DrawMainText();
                }

                else if (menu == "reset system")
                {
                    DrawFrame();
                    Console.SetCursorPosition(2, 11);
                    Console.ForegroundColor = Cyan;
                    Console.Write("Are you sure? (Y/N)");
<<<<<<< Updated upstream
                    string thingtosave = Console.ReadLine();
                    if (thingtosave == @"Y")
                    {
                        thingtosave = @"y";
                    }
                    if (thingtosave == @"N")
                    {
                        thingtosave = @"n";
                    }
=======
                    Console.ForegroundColor = Green;
                    string thingtosave = Console.ReadLine().ToLower();
>>>>>>> Stashed changes

                    if (thingtosave == @"y")
                    {
                        // note that that wont do shit

                        try
                        {
                            //DONT ALTER. SOMEHOW IT WORKS
                            //YOU ARE FRENCH IF YOU TOUCH IT
                            System.IO.File.Delete(@"0:\content\sys\setup.gms");
                            System.IO.Directory.Delete(@"0:\content\prf");
                            var directory_list = System.IO.Directory.GetFiles(@"0:\");
                            foreach (var file in directory_list)
                            {
                                System.IO.File.Delete(@"0:\" + file);
                            }
                            var directory_list3 = System.IO.Directory.GetDirectories(@"0:\");
                            foreach (var file in directory_list3)
                            {
                                System.IO.File.Delete(@"0:\" + file);
                            }
                            Kernel.FS.Initialize(false);
                        }
                        catch (Exception monkeyballs)
                        {
                            DrawFrame();
                            int screenWidth = 80;
                            string title = "System Reset";
                            string q = "GoOS is now back to factory default settings.";
                            string w = "The system will no longer operate until restarted.";
                            string e = "";
                            string r = "Once restarted, the GoOS setup will launch instantly.";
                            string t = "";
                            string y = "";
                            string u = "";
                            string i = "";

                            int titlePos = (screenWidth / 2) - (title.Length / 2);
                            int qPos = (screenWidth / 2) - (q.Length / 2);
                            int wPos = (screenWidth / 2) - (w.Length / 2);
                            int ePos = (screenWidth / 2) - (e.Length / 2);
                            int rPos = (screenWidth / 2) - (r.Length / 2);
                            int tPos = (screenWidth / 2) - (t.Length / 2);
                            int yPos = (screenWidth / 2) - (y.Length / 2);
                            int uPos = (screenWidth / 2) - (u.Length / 2);
                            int iPos = (screenWidth / 2) - (i.Length / 2);

                            Console.SetCursorPosition(titlePos, 2);
                            Console.Write(title);
                            Console.SetCursorPosition(qPos, 3);
                            Console.Write(q);
                            Console.SetCursorPosition(wPos, 4);
                            Console.Write(w);
                            Console.SetCursorPosition(rPos, 10);
                            Console.Write(r);
                            bool pool = true;
                            while (pool)
                            {
                                //The system will forever hang. this code will never end.
                                //What why?
                                // so the user has to restart to complete the reset
                                // Ohhh
                                // building test now
                                // aight
                                // IT WORKS
                                // wasdfghjkloipqwerty
                                // i take that as a pogchamp
                                // you should add the ability to add custom commands and features to your bot somehow
                            }

                        }

                    }
                    else // you dont need an elif for no. literally anything else and kick out
                    {
                        MessageBox();
                        menu = "main";
                        selected = 2;
                        DrawFrame();
                        DrawMainText();
                    }


                    MessageBox();
                    menu = "main";
                    selected = 2;
                    DrawFrame();
                    DrawMainText();
                }
            }
            Console.BackgroundColor = Black;
            Console.Clear();
        }
    }
}