using System;
using System.Collections.Generic;
using System.Text;
using static System.ConsoleColor;
using Sys = Cosmos.System;

namespace GoOS
{
    public static class OOBE
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

        static bool running = true;
        static string usrn, cprn;

        public static void Open()
        {
            while (running)
            {
                DrawPage(0);
                DrawPage(1);
                DrawPage(2);
                Sys.FileSystem.VFS.VFSManager.CreateFile(@"0:\content\sys\setup.gms");
                var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\setup.gms");
                var setupstream = setupcontent.GetFileStream();
                if (setupstream.CanWrite)
                {
                    MessageBox(0);
                    byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {usrn}\ncomputername: {cprn}");
                    setupstream.Write(textToWrite, 0, textToWrite.Length);
                    MessageBox(1);
                    MessageBox(2);
                }
                else
                {
                    MessageBox(2);
                }
            }
        }

        /// <summary>
        /// Draws the frame.
        /// </summary>
        private static void DrawFrame()
        {
            // Do not touch. I know what I'm doing.
            Console.Clear();
            try
            {
                Console.BackgroundColor = Black;
                Console.ForegroundColor = DarkRed;
                CP737Console.Write("╔══════════════════════════════════════════════════════════════════════════════╗\n" +
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
            catch { }
        }

        /// <summary>
        /// Writes a title to the top of the frame.
        /// </summary>
        /// <param name="Title">The title to be written.</param>
        private static void DrawTitle(string Title, int Y)
        {
            int OldX = Console.CursorLeft; int OldY = Console.CursorTop;

            Console.SetCursorPosition(40 - (Title.Length / 2), Y);
            Console.ForegroundColor = Red;
            Console.Write(Title);
            Console.SetCursorPosition(OldX, OldY);
        }

        /// <summary>
        /// Write some controls to the bottom of the screen.
        /// </summary>
        /// <param name="Controls">The controls to be written.</param>
        private static void DrawControls(string Controls)
        {
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

        private static void DrawPage(int page)
        {
            if (page == 0)
            {
                DrawFrame();
                DrawTitle(" GoOS Setup ", 0);
                DrawControls("[ENTER - Continue]═══[ESC - Exit]");
                int screenWidth = 80;
                string welcomeText = "Welcome to GoOS";
                string setupText = "We have some things to set up and get sorted!";
                string continueText = "Press enter to continue, otherwise press escape to exit setup.";

                int welcomePosition = (screenWidth / 2) - (welcomeText.Length / 2);
                int setupPosition = (screenWidth / 2) - (setupText.Length / 2);
                int continuePosition = (screenWidth / 2) - (continueText.Length / 2);

                Console.SetCursorPosition(welcomePosition, 2);
                Console.Write(welcomeText);

                Console.SetCursorPosition(setupPosition, 3);
                Console.Write(setupText);

                Console.Write(continueText);

                ConsoleKeyInfo key = Console.ReadKey(true);

                while (key.Key != ConsoleKey.Enter)
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            running = false;
                            Console.Clear();
                            break;
                    }
                }
            }
            else if (page == 1)
            {
                DrawFrame();
                DrawTitle(" Usage Agreements - GoOS Setup ", 0);
                int screenWidth = 80;
                string title = "Usage Agreements";
                string q = "While nobody reads them, we want you to know some basic guidelines.";
                string w = "Please refrain from creating Viruses in the GoCode engine.";
                string e = "We do not submit any data or files to Goplex Studios or Owen2k6";
                string r = "You are in full control and nothing is sent over the net.";
                string t = "We will grant support to users based on SUPPORT.MD";
                string y = "on our Github page. Forks of our OS will not be supported";
                string u = "by us. Keep your OS up to date to keep getting support!";
                string i = "Press any key to accept this UA. Terminate OS if declined.";

                int titlePos = (screenWidth / 2) - (title.Length / 2);
                int qPos = (screenWidth / 2) - (q.Length / 2);
                int wPos = (screenWidth / 2) - (w.Length / 2);
                int ePos = (screenWidth /2) - (e.Length / 2);
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
                Console.SetCursorPosition(ePos, 5);
                Console.Write(e);
                Console.SetCursorPosition(rPos, 6);
                Console.Write(r);
                Console.SetCursorPosition(tPos, 7);
                Console.Write(t);
                Console.SetCursorPosition(uPos, 8);
                Console.Write(u);
                Console.SetCursorPosition(iPos, 13);
                Console.Write(i);
                Console.ReadKey();


            }
            else if (page == 2)
            {
                DrawFrame();
                DrawTitle(" Accounts - GoOS Setup ", 0);
                int screenWidth = 80;
                string title = "Your Account";
                string q = "There is more planned in the future, however we only";
                string w = "need 2 things from you. Your name and your computer's name.";
                string e = "";
                string r = "";
                string t = "";
                string y = "";
                string u = "Username: ";
                string i = "Computer Name: ";

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
                Console.SetCursorPosition (qPos, 3);
                Console.Write(q);
                Console.SetCursorPosition (wPos, 4);
                Console.Write(w);


                Console.SetCursorPosition (wPos, 6);
                Console.Write(u);
                Console.SetCursorPosition(wPos, 8);
                Console.Write(i);

                Console.ForegroundColor = Gray;
                Console.SetCursorPosition(wPos + 10, 6);
                usrn = Console.ReadLine();
                Console.SetCursorPosition(wPos + 15, 8);
                cprn = Console.ReadLine();
            }
        }

        private static void MessageBox(int message)
        {
            Console.ForegroundColor = DarkRed;

            if (message == 0)
            {
                CP737Console.Write("╔════════════════════╗", 29, 10);
                CP737Console.Write("║                    ║", 29, 11);
                CP737Console.Write("║                    ║", 29, 12);
                CP737Console.Write("║                    ║", 29, 13);
                CP737Console.Write("╚════════════════════╝", 29, 14);

                Console.ForegroundColor = Red;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(31, 12);
                Console.Write("Saving settings...");
            }
            else if (message == 1)
            {
                CP737Console.Write("╔════════════════════════════════════════╗", 19, 10);
                CP737Console.Write("║                                        ║", 19, 11);
                CP737Console.Write("║                                        ║", 19, 12);
                CP737Console.Write("║                                        ║", 19, 13);
                CP737Console.Write("╚════════════════════════════════════════╝", 19, 14);

                Console.ForegroundColor = Red;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(21, 12);
                Console.Write("Settings have been saved successfully!");
                Console.ReadKey();
                Console.Clear();
            }
            else if (message == 2)
            {
                CP737Console.Write("╔══════════════════════════════════════════════════════╗", 12, 10);
                CP737Console.Write("║                                                      ║", 12, 11);
                CP737Console.Write("║                                                      ║", 12, 12);
                CP737Console.Write("║                                                      ║", 12, 13);
                CP737Console.Write("╚══════════════════════════════════════════════════════╝", 12, 14);

                Console.ForegroundColor = Red;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(14, 12);
                Console.Write("A serious error has occoured, setup cannot continue.");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}