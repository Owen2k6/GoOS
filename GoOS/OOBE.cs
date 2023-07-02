using System;
using System.IO;
using System.Threading;
using Sys = Cosmos.System;
using static GoOS.Themes.ThemeManager;
using Console = BetterConsole;
using GoOS.Themes;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using System.Collections.Generic;

namespace GoOS
{
    public static class OOBE
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupwelcome.bmp")]
        private static byte[] setupWelcomeRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupTOU.bmp")]
        private static byte[] setupTOURaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupua.bmp")]
        private static byte[] setupUARaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupsupport.bmp")]
        private static byte[] setupSupportRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupuser.bmp")]
        private static byte[] setupUserRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupcomputer.bmp")]
        private static byte[] setupComputerRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupthm.bmp")]
        private static byte[] setupThemeRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupres.bmp")]
        private static byte[] setupResRaw;

        [ManifestResourceStream(ResourceName = "GoOS.Resources.OOBE.setupfinal.bmp")]
        private static byte[] setupFinalRaw;

        private static Canvas setupWelcome = Image.FromBitmap(setupWelcomeRaw, false);
        private static Canvas setupTOU = Image.FromBitmap(setupTOURaw, false);
        private static Canvas setupUA = Image.FromBitmap(setupUARaw, false);
        private static Canvas setupSupport = Image.FromBitmap(setupSupportRaw, false);
        private static Canvas setupUser = Image.FromBitmap(setupUserRaw, false);
        private static Canvas setupComputer = Image.FromBitmap(setupComputerRaw, false);
        private static Canvas setupTheme = Image.FromBitmap(setupThemeRaw, false);
        private static Canvas setupRes = Image.FromBitmap(setupResRaw, false);
        private static Canvas setupFinal = Image.FromBitmap(setupFinalRaw, false);

        private static string username, computerName, theme;
        private static byte videoMode;

        public static void Launch()
        {
            for (int i = 0; i < 9; i++)
                ShowPage(i + 1);

            //Sys.Power.Reboot();
        }

        public static void ShowPage(int page)
        {   int tries = 0;
            
            switch (page)
            {
                case 1:
                    Console.Canvas.DrawImage(0, 0, setupWelcome, false);
                    Console.Canvas.Update();

                    ReadAgain1:
                    var key1 = Console.ReadKey(true);
                    if (key1.Key != ConsoleKey.Enter) goto ReadAgain1;

                    break;

                case 2:
                    Console.Canvas.DrawImage(0, 0, setupTOU, false);
                    Console.Canvas.Update();

                    ReadAgain2:
                    var key2 = Console.ReadKey(true);
                    if (key2.Key != ConsoleKey.F8) goto ReadAgain2;
                    break;

                case 3:
                    Console.Canvas.DrawImage(0, 0, setupUA, false);
                    Console.Canvas.Update();

                    ReadAgain3:
                    var key3 = Console.ReadKey(true);
                    if (key3.Key != ConsoleKey.F9) goto ReadAgain3;
                    break;

                case 4:
                    Console.Canvas.DrawImage(0, 0, setupSupport, false);
                    Console.Canvas.Update();

                    ReadAgain4:
                    var key4 = Console.ReadKey(true);
                    if (key4.Key != ConsoleKey.Enter) goto ReadAgain4;
                    break;

                case 5:
                    Console.Canvas.DrawImage(0, 0, setupUser, false);
                    Console.Canvas.Update();

                    Console.SetCursorPosition(14, 8);
                    Console.Write("Username: ");
                    username = Console.ReadLine();
                    break;

                case 6:
                    Console.Canvas.DrawImage(0, 0, setupComputer, false);
                    Console.Canvas.Update();

                    Console.SetCursorPosition(14, 7);
                    Console.Write("Computer Name: ");
                    computerName = Console.ReadLine();
                    break;

                case 7:
                    Console.Canvas.DrawImage(0, 0, setupTheme, false);
                    Console.Canvas.Update();

                    int themeMenuSelectedButton = 0;

                    Refresh7:
                    string[] themes =
                    {
                        "default",
                        "mono",
                        "dark",
                        "light"
                    };

                    if (themeMenuSelectedButton > (themes.Length - 1))
                    {
                        themeMenuSelectedButton = 0;
                    }
                    else if (themeMenuSelectedButton < 0)
                    {
                        themeMenuSelectedButton = themes.Length - 1;
                    }

                    for (int i = 0; i < themes.Length; i++)
                    {
                        ControlPanel.DrawButton(themes[i], 14, 6 + i,
                            i == themeMenuSelectedButton); // Draw button automatically at the correct coordinates
                    }

                    var key7 = Console.ReadKey(true).Key;
                    switch (key7)
                    {
                        case ConsoleKey.Enter:
                            theme = "ThemeFile = " + @"0:\content\themes\" + themes[themeMenuSelectedButton] +
                                    ".gtheme";
                            break;

                        case ConsoleKey.UpArrow:
                            themeMenuSelectedButton--;
                            goto Refresh7;

                        case ConsoleKey.DownArrow:
                            themeMenuSelectedButton++;
                            goto Refresh7;

                        default:
                            goto Refresh7;
                    }

                    break;

                case 8:
                    Console.Canvas.DrawImage(0, 0, setupRes, false);
                    Console.Canvas.Update();

                    List<(string, (ushort Width, ushort Height))> videoModes = new()
                    {
                        ("800 x 600", (800, 600)),
                        ("1280 x 720", (1280, 720)),
                        ("1920 x 1080", (1920, 1080))
                    };

                    int displayMenuSelectedButton = 0;

                    Refresh8:
                    if (displayMenuSelectedButton > videoModes.Count - 1)
                    {
                        displayMenuSelectedButton = 0;
                    }
                    else if (displayMenuSelectedButton < 0)
                    {
                        displayMenuSelectedButton = videoModes.Count - 1;
                    }

                    for (int i = 0; i < videoModes.Count; i++)
                    {
                        ControlPanel.DrawButton(videoModes[i].Item1, 14, 5 + i,
                            i == displayMenuSelectedButton); // Draw button automatically at the correct coordinates
                    }

                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Enter:
                            if (videoModes[displayMenuSelectedButton].Item1 == "800 x 600") videoMode = 2;
                            if (videoModes[displayMenuSelectedButton].Item1 == "1280 x 720") videoMode = 4;
                            if (videoModes[displayMenuSelectedButton].Item1 == "1920 x 1080") videoMode = 6;
                            break;

                        case ConsoleKey.UpArrow:
                            displayMenuSelectedButton--;
                            goto Refresh8;

                        case ConsoleKey.DownArrow:
                            displayMenuSelectedButton++;
                            goto Refresh8;

                        default:
                            goto Refresh8;
                    }

                    break;

                case 9:
                    
                    
                    balls:
                    try
                    {
                        tries++;
                        Directory.CreateDirectory(@"0:\content");
                        Directory.CreateDirectory(@"0:\content\sys");
                        Directory.CreateDirectory(@"0:\content\themes");
                        File.Create(@"0:\content\sys\option-showprotectedfiles.gms");
                        File.Create(@"0:\content\sys\option-editprotectedfiles.gms");
                        File.Create(@"0:\content\sys\option-deleteprotectedfiles.gms");
                        File.Create(@"0:\content\sys\setup.gms");
                        File.WriteAllText(@"0:\content\sys\version.gms",
                            $"System.Version is set to {Kernel.version} \n Note to users reading this: DO NOT ALTER. IMPORTANT IF USER DATA NEEDS CONVERTING.");
                        File.WriteAllText(@"0:\content\sys\user.gms",
                            $"username: {username}\ncomputername: {computerName}");
                        File.WriteAllBytes(@"0:\content\sys\resolution.gms",
                            new byte[] { videoMode }); // Video mode 2: 1280x720
                        File.WriteAllText(@"0:\content\themes\default.gtheme",
                            "Default = White\nBackground = Black\nStartup = DarkMagenta,Red,DarkRed\nWindowText = Cyan\nWindowBorder = Green\nErrorText = Red\nOther1 = Yellow");
                        File.WriteAllText(@"0:\content\themes\mono.gtheme",
                            "Default = White\nBackground = Black\nStartup = White,White,White\nWindowText = White\nWindowBorder = White\nErrorText = White\nOther1 = White");
                        File.WriteAllText(@"0:\content\themes\dark.gtheme",
                            "Default = Gray\nBackground = Black\nStartup = DarkGray,Gray,DarkGray\nWindowText = Gray\nWindowBorder = DarkGray\nErrorText = DarkGray\nOther1 = DarkGray");
                        File.WriteAllText(@"0:\content\themes\light.gtheme",
                            "Default = Black\nBackground = White\nStartup = Black,Black,Black\nWindowText = Black\nWindowBorder = Black\nErrorText = Black\nOther1 = Black");
                        File.WriteAllText(@"0:\content\sys\theme.gms", @"ThemeFile = " + theme);
                    }
                    catch (Exception e)
                    {               
                        Console.SetCursorPosition(0,0);
                        //Console.WriteLine(e);
                        
                        if (tries < 10)
                        {
                            goto balls;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(e);
                            Console.WriteLine(e);
                            Console.WriteLine(e);
                            Console.WriteLine(e);
                            Console.WriteLine(e);
                            Console.WriteLine(e);
                            Console.WriteLine("Press any key to shutdown.");
                            Console.ReadKey(true);
                            Sys.Power.Shutdown();
                        }
                    }

                    Console.Canvas.DrawImage(0, 0, setupFinal, false);
                    Console.Canvas.Update();

                    ReadAgain9:
                    var key9 = Console.ReadKey(true);
                    if (key9.Key != ConsoleKey.Enter) goto ReadAgain9;
                    Sys.Power.Reboot();
                    break;
            }
        }

        /*static string usrn, cprn;

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
            // Draw the frame with GUI instead of TUI
            Console.Clear();
            Console.Canvas.DrawRectangle(3, 7, Convert.ToUInt16(Console.Canvas.Width - 6), Convert.ToUInt16(Console.Canvas.Height - 14), 0, ThemeManager.WindowBorder);
            Console.Canvas.DrawRectangle(4, 8, Convert.ToUInt16(Console.Canvas.Width - 6), Convert.ToUInt16(Console.Canvas.Height - 14), 0, ThemeManager.WindowBorder);
        }

        /// <summary>
        /// Writes a title to the top of the frame.
        /// </summary>
        /// <param name="Title">The title to be written.</param>
        private static void DrawTitle(string Title, int Y)
        {
            int OldX = Console.CursorLeft; int OldY = Console.CursorTop;

            Console.SetCursorPosition(40 - (Title.Length / 2), Y);
            Console.ForegroundColor = ThemeManager.WindowText;
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
                Console.Write("╔════════════════════╗", 29, 10);
                Console.Write("║                    ║", 29, 11);
                Console.Write("║                    ║", 29, 12);
                Console.Write("║                    ║", 29, 13);
                Console.Write("╚════════════════════╝", 29, 14);

                Console.ForegroundColor = WindowText;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(31, 12);
                Console.Write("Saving settings...");
            }
            else if (message == 1)
            {
                Console.Write("╔════════════════════╗", 29, 10);
                Console.Write("║                    ║", 29, 11);
                Console.Write("║                    ║", 29, 12);
                Console.Write("║                    ║", 29, 13);
                Console.Write("╚════════════════════╝", 29, 14);

                Console.ForegroundColor = WindowText;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(31, 12);
                Console.Write("Preparing Setup...");
            }
            else if (message == 2)
            {
                Console.Write("╔══════════════════════════════════════════════════════╗", 12, 10);
                Console.Write("║                                                      ║", 12, 11);
                Console.Write("║                                                      ║", 12, 12);
                Console.Write("║                                                      ║", 12, 13);
                Console.Write("╚══════════════════════════════════════════════════════╝", 12, 14);

                Console.ForegroundColor = WindowText;
                DrawTitle(" Info ", 10);
                Console.SetCursorPosition(14, 12);
                Console.Write("A serious error has occurred, setup cannot continue.");
                while (true) { Console.ReadKey(true); } // Lock up
            }
        }*/
    }
}