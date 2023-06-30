using System;
using System.IO;
using System.Threading;
using Sys = Cosmos.System;
using static GoOS.Themes.ThemeManager;
using Console = BetterConsole;

namespace GoOS
{
    public static class OOBE
    {
        static string usrn, cprn;

        public static void Open()
        {
            DrawFrame();
            MessageBox(1);
            DrawPage(0);
            DrawPage(1);
            DrawPage(2);
            MessageBox(0);
            try
            {
                Directory.CreateDirectory(@"0:\content");
                Directory.CreateDirectory(@"0:\content\sys");
                Directory.CreateDirectory(@"0:\content\themes");
                File.Create(@"0:\content\sys\setup.gms");
                File.WriteAllText(@"0:\content\sys\version.gms", $"System.Version is set to {Kernel.version} \n Note to users reading this: DO NOT ALTER. IMPORTANT IF USER DATA NEEDS CONVERTING.");
                File.WriteAllText(@"0:\content\sys\user.gms", $"username: {usrn}\ncomputername: {cprn}");
                File.WriteAllBytes(@"0:\content\sys\resolution.gms", new byte[] { 2 }); // Video mode 2: 1280x720
                File.Create(@"0:\content\sys\option-showprotectedfiles.gms");
                File.Create(@"0:\content\sys\option-editprotectedfiles.gms");
                File.Create(@"0:\content\sys\option-deleteprotectedfiles.gms");
                File.WriteAllText(@"0:\content\themes\default.gtheme", "Default = White\nBackground = Black\nStartup = DarkMagenta,Red,DarkRed\nWindowText = Cyan\nWindowBorder = Green\nErrorText = Red\nOther1 = Yellow");
                File.WriteAllText(@"0:\content\themes\mono.gtheme", "Default = White\nBackground = Black\nStartup = White,White,White\nWindowText = White\nWindowBorder = White\nErrorText = White\nOther1 = White");
                File.WriteAllText(@"0:\content\themes\dark.gtheme", "Default = Gray\nBackground = Black\nStartup = DarkGray,Gray,DarkGray\nWindowText = Gray\nWindowBorder = DarkGray\nErrorText = DarkGray\nOther1 = DarkGray");
                File.WriteAllText(@"0:\content\themes\light.gtheme", "Default = Black\nBackground = White\nStartup = Black,Black,Black\nWindowText = Black\nWindowBorder = Black\nErrorText = Black\nOther1 = Black");
                File.WriteAllText(@"0:\content\sys\theme.gms", @"ThemeFile = 0:\content\themes\default.gtheme");
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        /// <summary>
        /// Draws the frame.
        /// </summary>
        private static void DrawFrame()
        {
            // Do not touch. I know what I'm doing.
            // Sorry i touched it -Owen2k6
            Console.Clear();
            try
            {
                Console.BackgroundColor = Background;
                Console.ForegroundColor = WindowBorder;
                     Console.Write("╔═══════════════════════════════════════════════════════════════════════════════════════╗\n" + //1
                                   "║                                                                                       ║\n" + //2
                                   "║                                                                                       ║\n" + //3
                                   "║                                                                                       ║\n" + //   4
                                   "║                                                                                       ║\n" + //5
                                   "║                                                                                       ║\n" + //6
                                   "║                                                                                       ║\n" + //7
                                   "║                                                                                       ║\n" + //8
                                   "║                                                                                       ║\n" +//9
                                   "║                                                                                       ║\n" + //10
                                   "║                                                                                       ║\n" + //11
                                   "║                                                                                       ║\n" +  //12
                                   "║                                                                                       ║\n" +  //13
                                   "║                                                                                       ║\n" +      //14
                                   "║                                                                                       ║\n" +  //15
                                   "║                                                                                       ║\n" +  //16
                                   "║                                                                                       ║\n" +  //17
                                   "║                                                                                       ║\n" +  //18
                                   "║                                                                                       ║\n" +  //19
                                   "║                                                                                       ║\n" +  //20
                                   "║                                                                                       ║\n" +      //21
                                   "║                                                                                       ║\n" +  //22
                                   "║                                                                                       ║\n" +  //23
                                   "║                                                                                       ║\n" +  //24
                                   "║                                                                                       ║\n" +  //25
                                   "║                                                                                       ║\n" +  //26
                                   "║                                                                                       ║\n" +  //27
                                   "║                                                                                       ║\n" +  //28
                                   "╚═══════════════════════════════════════════════════════════════════════════════════════╝");    //29

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
            Console.ForegroundColor = WindowText;
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
                DrawControls("[ENTER - Continue]");
                string welcomeText = "Welcome to GoOS";
                string setupText = "We have some things to set up and get sorted!";
                string continueText = "Press enter to continue...";
                string broDidntPressEnter = "Bro I said enter!";

                int welcomePosition = 40 - (welcomeText.Length / 2);
                int setupPosition = 40 - (setupText.Length / 2);
                int continuePosition = 40 - (continueText.Length / 2);
                int broDidntPressEnterPosition = 40 - (broDidntPressEnter.Length / 2);

                Console.SetCursorPosition(welcomePosition, 2);
                Console.Write(welcomeText);

                Console.SetCursorPosition(setupPosition, 3);
                Console.Write(setupText);

                Console.SetCursorPosition(continuePosition, 22);
                Console.Write(continueText);

            readagain:
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(broDidntPressEnterPosition, 20);
                    Console.Write(broDidntPressEnter);
                    Thread.Sleep(500);
                    Console.SetCursorPosition(broDidntPressEnterPosition, 20);
                    Console.Write("                 ");
                    goto readagain;
                }
            }
            else if (page == 1)
            {
                DrawFrame();
                DrawTitle(" Usage Agreements - GoOS Setup ", 0);
                DrawControls("[F8 - Agree]═══[ESC - Shut down]");
                string title = "Usage Agreements";
                string q = "While nobody reads them, we want you to know some basic guidelines.";
                string w = "Please refrain from creating Viruses in the GoCode engine.";
                string e = "We do not submit any data or files to Goplex Studios or Owen2k6";
                string r = "You are in full control and nothing is sent over the net.";
                string t = "We will grant support to users based on SUPPORT.MD";
                string y = "on our Github page. Forks of our OS will not be supported";
                string u = "by us. Keep your OS up to date to keep getting support!";
                string i = "Press F8 to accept UA, otherwise press Escape to shut down...";
                string broDidntPressEnter = "Bro I said F8 or escape!";

                int titlePos = 40 - (title.Length / 2);
                int qPos = 40 - (q.Length / 2);
                int wPos = 40 - (w.Length / 2);
                int ePos = 40 - (e.Length / 2);
                int rPos = 40 - (r.Length / 2);
                int tPos = 40 - (t.Length / 2);
                int yPos = 40 - (y.Length / 2);
                int uPos = 40 - (u.Length / 2);
                int iPos = 40 - (i.Length / 2);
                int broDidntPressEnterPosition = 40 - (broDidntPressEnter.Length / 2);

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


            readagain:
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.F8 && key.Key != ConsoleKey.Escape)
                {
                    Console.SetCursorPosition(broDidntPressEnterPosition, 20);
                    Console.Write(broDidntPressEnter);
                    Thread.Sleep(500);
                    Console.SetCursorPosition(broDidntPressEnterPosition, 20);
                    Console.Write("                        ");
                    goto readagain;
                }
                else if (key.Key == ConsoleKey.Escape) { Sys.Power.Shutdown(); }
            }
            else if (page == 2)
            {
                DrawFrame();
                DrawTitle(" Accounts - GoOS Setup ", 0);
                string title = "Your Account";
                string q = "There is more planned in the future, however we only";
                string w = "need 2 things from you. Your name and your computer's name.";
                string e = "Username: ";
                string r = "Computer Name: ";
                string t = "";
                string y = "";
                string u = "";
                string i = "";

                int titlePos = 40 - (title.Length / 2);
                int qPos = 40 - (q.Length / 2);
                int wPos = 40 - (w.Length / 2);
                int ePos = 40 - (e.Length / 2);
                int rPos = 40 - (r.Length / 2);
                int tPos = 40 - (t.Length / 2);
                int yPos = 40 - (y.Length / 2);
                int uPos = 40 - (u.Length / 2);
                int iPos = 40 - (i.Length / 2);

                Console.SetCursorPosition(titlePos, 2);
                Console.Write(title);
                Console.SetCursorPosition (qPos, 3);
                Console.Write(q);
                Console.SetCursorPosition (wPos, 4);
                Console.Write(w);

                Console.SetCursorPosition (wPos, 6);
                Console.Write(e);
                Console.SetCursorPosition(wPos, 8);
                Console.Write(r);

                Console.ForegroundColor = Default;
                Console.SetCursorPosition(wPos + 10, 6);
                usrn = Console.ReadLine();
                Console.SetCursorPosition(wPos + 15, 8);
                cprn = Console.ReadLine();
            }
        }

        private static void MessageBox(int message)
        {
            Console.ForegroundColor = WindowBorder;

            if (message == 0)
            {
                /*Console.Write("╔════════════════════╗", 29, 10);
                Console.Write("║                    ║", 29, 11);
                Console.Write("║                    ║", 29, 12);
                Console.Write("║                    ║", 29, 13);
                Console.Write("╚════════════════════╝", 29, 14);*/

                Console.ForegroundColor = WindowText;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(31, 12);
                Console.Write("Saving settings...");
            }
            else if (message == 1)
            {
                /*Console.Write("╔════════════════════╗", 29, 10);
                Console.Write("║                    ║", 29, 11);
                Console.Write("║                    ║", 29, 12);
                Console.Write("║                    ║", 29, 13);
                Console.Write("╚════════════════════╝", 29, 14);*/

                Console.ForegroundColor = WindowText;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(31, 12);
                Console.Write("Preparing Setup...");
            }
            else if (message == 2)
            {
                /*Console.Write("╔══════════════════════════════════════════════════════╗", 12, 10);
                Console.Write("║                                                      ║", 12, 11);
                Console.Write("║                                                      ║", 12, 12);
                Console.Write("║                                                      ║", 12, 13);
                Console.Write("╚══════════════════════════════════════════════════════╝", 12, 14);*/

                Console.ForegroundColor = WindowText;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(14, 12);
                Console.Write("A serious error has occurred, setup cannot continue.");
                while (true) { Console.ReadKey(true); } // Lock up
            }
        }
    }
}