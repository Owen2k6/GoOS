using System;
using System.Collections.Generic;
using System.Text;
using static System.ConsoleColor;
using Sys = Cosmos.System;
using Console = System.Console;
using GoOS.Themes;
using static GoOS.Themes.ThemeManager;
using Cosmos.System;
using Cosmos.System.ScanMaps;
using System.Drawing;

namespace GoOS.Settings
{
    public static class ControlPanel
    {
        // Welcome to the worst file in GoOS
        //agreed
        // GoOS Core

        #region GoOS Core
        public static void print(string str)
        {
            Console.WriteLine(str);
        }
        public static void log(ConsoleColor colour, string str)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(str);
        }
        public static void write(string str)
        {
            Console.Write(str);
        }
        public static void textcolour(ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
        }
        public static void highlightcolour(ConsoleColor colour)
        {
            Console.BackgroundColor = colour;
        }
        public static void sleep(int time)
        {
            System.Threading.Thread.Sleep(time);
        }
        #endregion
        

        /// <summary>
        /// Opens the settings.
        /// </summary>
        public static void Open()
        {
            Console.CursorVisible = false;
            Console.CursorVisible = false;
            DrawFrame();
            Run();
        }

        public static bool DontTouch = true;
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
                Console.ForegroundColor = WindowBorder;
                Console.Write("╔════════════╦══════════════════════════════════════════════════════════════════════════╗\n" + //1
                                   "║            ║                                                                          ║\n" + //2
                                   "║            ║                                                                          ║\n" + //3
                                   "║            ║                                                                          ║\n" + //   4
                                   "║            ║                                                                          ║\n" + //5
                                   "║            ║                                                                          ║\n" + //6
                                   "║            ║                                                                          ║\n" + //7
                                   "║            ║                                                                          ║\n" + //8
                                   "║            ║                                                                          ║\n" +//9
                                   "║            ║                                                                          ║\n" + //10
                                   "║            ║                                                                          ║\n" + //11
                                   "║            ║                                                                          ║\n" +  //12
                                   "║            ║                                                                          ║\n" +  //13
                                   "║            ║                                                                          ║\n" +      //14
                                   "║            ║                                                                          ║\n" +  //15
                                   "║            ║                                                                          ║\n" +  //16
                                   "║            ║                                                                          ║\n" +  //17
                                   "║            ║                                                                          ║\n" +  //18
                                   "║            ║                                                                          ║\n" +  //19
                                   "║            ║                                                                          ║\n" +  //20
                                   "║            ║                                                                          ║\n" +      //21
                                   "║            ║                                                                          ║\n" +  //22
                                   "║            ║                                                                          ║\n" +  //23
                                   "║            ║                                                                          ║\n" +  //24
                                   "╠════════════╩══════════════════════════════════════════════════════════════════════════╣\n" +  //25
                                   "║                                                                                       ║\n" +  //26
                                   "║                                                                                       ║\n" +  //27
                                   "║                                                                                       ║\n" +  //28
                                   "╚═══════════════════════════════════════════════════════════════════════════════════════╝");    //29
                Console.SetCursorPosition(2, 19);
                Console.ForegroundColor = Yellow;
                //Console.Write("Arrow keys");
                Console.SetCursorPosition(2, 22);
                if (DontTouch)
                {
                    MkButton("General", 2, 22, WindowText, Black);
                    MkButton("Personalisation", 10, 22, Black, WindowText);
                    MkButton("Advanced", 26, 22, Black, WindowText);
                    MkButton("About", 2, 1, WindowText, Black);
                    MkButton("Support", 2, 2, Black, WindowText);
                    DontTouch = false;
                }
                //Console.Write("Tab");
                
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

            Console.SetCursorPosition(40 - Title.Length / 2, Y);
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


        /// <summary>
        /// Makes a button
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <param name="x">The X location of the button.</param>
        /// <param name="y">The Y location of the button.</param>
        /// <param name="highlight">The background color of the button</param>
        /// <param name="colour">The foreground color of the button</param>
        private static void MkButton(string name, int x, int y, ConsoleColor highlight, ConsoleColor colour)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = highlight;
            Console.ForegroundColor = colour;
            Console.Write(name);
        }

        /// <summary>
        /// Show a dialogue for changing the keyboard layout.
        /// </summary>
        private static void KeyboardLayoutDialogue()
        {
            List<(string, ScanMapBase)> scanMaps = new()
            {
                ("Deutsch (German)", new DEStandardLayout()),
                ("Espanol (Spanish)", new ESStandardLayout()),
                ("Francais (French)", new ESStandardLayout()),
                ("British English", new GBStandardLayout()),
                ("Turkce (Turkish)", new TRStandardLayout()),
                ("US English", new USStandardLayout())
            };

            Console.BackgroundColor = Black;
            Console.ForegroundColor = WindowBorder;
            int y = 7;
            Console.Write("╔════════════════════╗", 29, y);
            for (int i = 0; i < scanMaps.Count; i++)
            {
                Console.Write("║                    ║", 29, y + i + 1);
            }
            Console.Write("╚════════════════════╝", 29, y + scanMaps.Count + 1);

            int selectedScanMap = 0;
            while (true)
            {
                for (int j = 0; j < scanMaps.Count; j++)
                {
                    Console.SetCursorPosition(30, y + j + 1);
                    if (j == selectedScanMap)
                    {
                        textcolour(Black);
                        highlightcolour(White);
                    }
                    else
                    {
                        textcolour(WindowText);
                        highlightcolour(Black);
                    }

                    string label = scanMaps[j].Item1;
                    Console.Write(label + new string(' ', 20 - label.Length));
                }
                var input = Console.ReadKey();
                switch (input.Key)
                {
                    case ConsoleKey.Enter:
                        KeyboardManager.SetKeyLayout(scanMaps[selectedScanMap].Item2);
                        
                        return;
                    case ConsoleKey.UpArrow:
                        selectedScanMap--;
                        if (selectedScanMap < 0) selectedScanMap = scanMaps.Count - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedScanMap++;
                        if (selectedScanMap >= scanMaps.Count) selectedScanMap = 0;
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        /// <summary>
        /// Something.
        /// </summary>
        private static void Run()
        {
            bool running = true;
            string menu = "General";
            int selected = 1;
            int selected2 = 1;

            Console.ForegroundColor = WindowText;

            while (running)
            {
                if (menu == "General")
                {
                    // ID 1: General
                    // ID 2: Personalisation
                    // ID 3: Advanced

                    if (selected2 == 1)
                    {
                        MkButton("About", 2, 1, WindowText, Black);
                        MkButton("Support", 2, 2, Black, WindowText);
                        MkButton("Keyboard", 2, 3, Black, WindowText);
                    }
                    else if (selected2 == 2)
                    {
                        MkButton("About", 2, 1, Black, WindowText);
                        MkButton("Support", 2, 2, WindowText, Black);
                        MkButton("Keyboard", 2, 3, Black, WindowText);
                    }
                    else if (selected2 == 3)
                    {
                        MkButton("About", 2, 1, Black, WindowText);
                        MkButton("Support", 2, 2, Black, WindowText);
                        MkButton("Keyboard", 2, 3, WindowText, Black);
                    }
                    Console.ForegroundColor = Black; Console.BackgroundColor = Black;
                    Console.SetCursorPosition(1, 4);
                    Console.Write("            ");
                    Console.SetCursorPosition(1, 5);
                    Console.Write("            ");
                    Console.ForegroundColor = WindowBorder; Console.BackgroundColor = Black;
                    ConsoleKeyInfo key = Console.ReadKey(true); 
                    if (selected == 0)
                    {
                        selected = 1;
                    }
                    if (selected > 3)
                    {
                        selected = 3;
                    }
                    if (selected2 == 0)
                    {
                        selected2 = 1;
                    }
                    if (selected2 > 3)
                    {
                        selected2 = 3;
                    }
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            if (selected2 == 3)
                            {
                                KeyboardLayoutDialogue();
                            }
                            break;
                        case ConsoleKey.Tab:
                            // xrc2. This comment is for you.
                            // DO NOT TOUCH THE CODE DIRECLY BELOW THIS COMMENT, IF YOU DO IT WILL BREAK THE SETINGS MENU. YOU HAVE BEEN WARNED.
                            if (selected == 1) { selected = 2; }
                            else if (selected == 2) { selected = 3; }
                            else if (selected == 3) { selected = 1; }

                            if (selected == 1)
                            {
                                MkButton("General", 2, 22, WindowText, Black);
                                MkButton("Personalisation", 10, 22, Black, WindowText);
                                MkButton("Advanced", 26, 22, Black, WindowText);
                                menu = "General";
                            }
                            if (selected == 2)
                            {
                                MkButton("General", 2, 22, Black, WindowText);
                                MkButton("Personalisation", 10, 22, WindowText, Black);
                                MkButton("Advanced", 26, 22, Black, WindowText);
                                menu = "Personalisation";
                            }
                            if (selected == 3)
                            {
                                MkButton("General", 2, 22, Black, WindowText);
                                MkButton("Personalisation", 10, 22, Black, WindowText);
                                MkButton("Advanced", 26, 22, WindowText, Black);
                                menu = "Advanced";
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            selected2--;
                            if (selected2 < 1) selected2 = 3;
                            break;
                        case ConsoleKey.DownArrow:
                            selected2++;
                            if (selected2 > 3) selected2 = 1;
                            break;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                    }

                    if (selected2 == 1)
                    {
                        MkButton("About", 2, 2, WindowText, Black);
                        MkButton("Support", 2, 3, Black, WindowText);
                        MkButton("Keyboard", 2, 3, Black, WindowText);

                        textcolour(WindowText);
                        highlightcolour(Black);
                        Console.SetCursorPosition(15, 2);
                        Console.Write("GoOS                                                            ");
                        Console.SetCursorPosition(15, 3);
                        Console.Write("Version " + Kernel.version + "                                                     ");
                        Console.SetCursorPosition(15, 4);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 5);
                        Console.Write("GoOS is a free and open source software designed with Cosmos.   ");
                        Console.SetCursorPosition(15, 6);
                        Console.Write("If you paid for this software, you should issue a refound       ");
                        Console.SetCursorPosition(15, 7);
                        Console.Write("request or report to proper authorities.                        ");
                    }
                    else if (selected2 == 2)
                    {
                        MkButton("About", 2, 2, Black, WindowText);
                        MkButton("Support", 2, 3, WindowText, Black);
                        MkButton("Keyboard", 2, 3, Black, WindowText);

                        textcolour(WindowText);
                        highlightcolour(Black);
                        Console.SetCursorPosition(15, 2);
                        Console.Write("GoOS Support                                                    ");
                        Console.SetCursorPosition(15, 3);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 4);
                        Console.Write("For support, please open a ticket in the Discord server.        ");
                        Console.SetCursorPosition(15, 5);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 6);
                        Console.Write("For reporting an issue, please report the issue in the issues   ");
                        Console.SetCursorPosition(15, 7);
                        Console.Write("tab in the Github repository.                                   ");
                    }
                    else if (selected2 == 3)
                    {
                        textcolour(WindowText);
                        highlightcolour(Black);
                        Console.SetCursorPosition(15, 2);
                        Console.Write("Keyboard                                                        ");

                        for (int y = 3; y < 15; y++)
                        {
                            Console.SetCursorPosition(15, y);
                            Console.Write("                                                                ");
                        }

                        MkButton("Change Layout", 15, 4, WindowText, Black);
                    }
                }
                else if (menu == "Personalisation")
                {
                    // ID 4: General
                    // ID 5: Personalisation
                    // ID 6: Advanced

                    

                    if (selected == 0)
                    {
                        selected = +1;
                    }
                    if (selected > 3)
                    {
                        selected = selected - 1;
                    }

                    //MkButton("General", 2, 22, Black, WindowText);
                    //MkButton("Personalisation", 10, 22, WindowText, Black);
                    //MkButton("Advanced", 26, 22, Black, WindowText);

                    Console.SetCursorPosition(1, 1);
                    Console.ForegroundColor = Black; Console.BackgroundColor = Black;
                    Console.Write("           ");
                    Console.SetCursorPosition(1, 2);
                    Console.Write("           ");
                    Console.SetCursorPosition(1, 3);
                    Console.Write("           ");
                    Console.SetCursorPosition(1, 4);
                    Console.Write("           ");
                    Console.SetCursorPosition(1, 5);
                    Console.Write("           ");
                    Console.ForegroundColor = WindowBorder;
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Tab:
                            // xrc2. This comment is for you (again).
                            // DO NOT TOUCH THE CODE DIRECLY BELOW THIS COMMENT, IF YOU DO IT WILL BREAK THE SETINGS MENU. YOU HAVE BEEN WARNED.
                            if (selected == 1) { selected = 2; }
                            else if (selected == 2) { selected = 3; }
                            else if (selected == 3) { selected = 1; }

                            if (selected == 1)
                            {
                                MkButton("General", 2, 22, WindowText, Black);
                                MkButton("Personalisation", 10, 22, Black, WindowText);
                                MkButton("Advanced", 26, 22, Black, WindowText);
                                menu = "General";
                            }
                            if (selected == 2)
                            {
                                MkButton("General", 2, 22, Black, WindowText);
                                MkButton("Personalisation", 10, 22, WindowText, Black);
                                MkButton("Advanced", 26, 22, Black, WindowText);
                                menu = "Personalisation";
                            }
                            if (selected == 3)
                            {
                                MkButton("General", 2, 22, Black, WindowText);
                                MkButton("Personalisation", 10, 22, Black, WindowText);
                                MkButton("Advanced", 26, 22, WindowText, Black);
                                menu = "Advanced";
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            selected2--;
                            if (selected2 < 1) selected2 = 3;
                            break;
                        case ConsoleKey.DownArrow:
                            selected2++;
                            if (selected2 > 3) selected2 = 1;
                            break;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                    }

                    if (selected2 == 1)
                    {
                        MkButton("Default", 2, 2, WindowText, Black);
                        MkButton("Mono", 2, 3, Black, WindowText);

                        textcolour(WindowText);
                        highlightcolour(Black);
                        Console.SetCursorPosition(15, 2);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 3);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 4);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 5);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 6);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 7);
                        Console.Write("                                                                ");
                    }
                    else if (selected2 == 2)
                    {
                        MkButton("Default", 2, 2, Black, WindowText);
                        MkButton("Mono", 2, 3, WindowText, Black);

                        textcolour(WindowText);
                        highlightcolour(Black);
                        Console.SetCursorPosition(15, 2);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 3);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 4);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 5);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 6);
                        Console.Write("                                                                ");
                        Console.SetCursorPosition(15, 7);
                        Console.Write("                                                                ");
                    }
                }
                else if (menu == "Advanced")
                {
                    // ID 4: General
                    // ID 5: Personalisation
                    // ID 6: Advanced

                    if (selected == 0)
                    {
                        selected = +1;
                    }
                    if (selected > 3)
                    {
                        selected = selected - 1;
                    }

                    //MkButton("General", 2, 22, Black, WindowText);
                    //MkButton("Personalisation", 10, 22, Black, WindowText);
                    //MkButton("Advanced", 26, 22, WindowText, Black);

                    Console.SetCursorPosition(1, 1);
                    Console.ForegroundColor = Black; Console.BackgroundColor = Black;
                    Console.Write("           ");
                    Console.SetCursorPosition(2, 2);
                    Console.Write("           ");
                    Console.SetCursorPosition(2, 3);
                    Console.Write("           ");
                    Console.SetCursorPosition(1, 4);
                    Console.Write("           ");
                    Console.SetCursorPosition(1, 5);
                    Console.Write("           ");
                    Console.ForegroundColor = WindowBorder;
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Tab:
                            // xrc2. This comment is for you (again x2).
                            // DO NOT TOUCH THE CODE DIRECLY BELOW THIS COMMENT, IF YOU DO IT WILL BREAK THE SETINGS MENU. YOU HAVE BEEN WARNED.
                            if (selected == 1) { selected = 2; }
                            else if (selected == 2) { selected = 3; }
                            else if (selected == 3) { selected = 1; }

                            if (selected == 1)
                            {
                                MkButton("General", 2, 22, WindowText, Black);
                                MkButton("Personalisation", 10, 22, Black, WindowText);
                                MkButton("Advanced", 26, 22, Black, WindowText);
                                menu = "General";
                            }
                            if (selected == 2)
                            {
                                MkButton("General", 2, 22, Black, WindowText);
                                MkButton("Personalisation", 10, 22, WindowText, Black);
                                MkButton("Advanced", 26, 22, Black, WindowText);
                                menu = "Personalisation";
                            }
                            if (selected == 3)
                            {
                                MkButton("General", 2, 22, Black, WindowText);
                                MkButton("Personalisation", 10, 22, Black, WindowText);
                                MkButton("Advanced", 26, 22, WindowText, Black);
                                menu = "Advanced";
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
                    DrawTitle(" Change Username - Settings ", 0); // Do not remove spaces!
                    DrawControls("[ENTER - Save]");
                    Console.SetCursorPosition(2, 11);
                    Console.Write("New Username: ");
                    string thingtosave = Console.ReadLine();

                    System.IO.File.Delete(@"0:\content\sys\setup.gms");
                    System.IO.File.Create(@"0:\content\sys\setup.gms");
                    var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\setup.gms");
                    var setupstream = setupcontent.GetFileStream();
                    byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {thingtosave}\ncomputername: {Kernel.computername}");
                    setupstream.Write(textToWrite, 0, textToWrite.Length);
                    Kernel.username = thingtosave;
                    
                    menu = "main";
                    selected = 2;
                    DrawFrame();
                    Console.ForegroundColor = WindowText;
                }

                else if (menu == "computer name")
                {
                    DrawFrame();
                    DrawTitle(" Change Computer Name - Settings ", 0); // Do not remove spaces!
                    DrawControls("[ENTER - Save]");
                    Console.SetCursorPosition(2, 11);
                    Console.Write("New Computer Name: ");
                    string thingtosave = Console.ReadLine();

                    System.IO.File.Delete(@"0:\content\sys\user.gms");
                    System.IO.File.Create(@"0:\content\sys\user.gms");
                    var setupcontent = Sys.FileSystem.VFS.VFSManager.GetFile(@"0:\content\sys\user.gms");
                    var setupstream = setupcontent.GetFileStream();
                    byte[] textToWrite = Encoding.ASCII.GetBytes($"username: {Kernel.username}\ncomputername: {thingtosave}");
                    setupstream.Write(textToWrite, 0, textToWrite.Length);
                    Kernel.computername = thingtosave;


                    menu = "main";
                    selected = 2;
                    DrawFrame();
                    Console.ForegroundColor = WindowText;
                }

                else if (menu == "reset system")
                {
                    Console.ForegroundColor = WindowBorder;
                    Console.Write("╔════════════════════════╗", 27, 10);
                    Console.Write("║                        ║", 27, 11);
                    Console.Write("║                        ║", 27, 12);
                    Console.Write("║                        ║", 27, 13);
                    Console.Write("╚════════════════════════╝", 27, 14);

                    Console.ForegroundColor = WindowText;
                    DrawTitle(" Confirmation ", 10);
                    Console.SetCursorPosition(29, 12);
                    Console.Write("Are you sure? (Y/N): ");

                    #region Key reading

                    string thingtosave = "n";

                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Y:
                            Console.Write("Y");
                            thingtosave = "y";
                            break;

                        case ConsoleKey.N:
                            Console.Write("N");
                            thingtosave = "n";
                            break;
                    }

                    #endregion


                    Console.SetCursorPosition(2, 11);
                    Console.Write("Are you sure? (Y/N)");

                    if (thingtosave == "y")
                    {
                        // note that that wont do shit

                        try
                        {
                            //DONT ALTER. SOMEHOW IT WORKS
                            //YOU ARE FRENCH IF YOU TOUCH IT
                            // no
                            /*
                            System.IO.File.Delete(@"0:\content\sys\setup.gms");
                            emptyDir(@"0:\content\prf");
                            System.IO.Directory.Delete(@"0:\content\prf");
                            emptyDir(@"0:\content\");
                            System.IO.Directory.Delete(@"0:\content\");
                            emptyDir(@"0:\");
                            System.IO.Directory.Delete(@"0:\");
                            System.IO.File.Delete(@"0:\content\sys\version.gms");
                            System.IO.File.Delete(@"0:\content\sys\userinfo.gms");
                            System.IO.File.Delete(@"0:\content\sys\option-showprotectedfiles.gms");
                            System.IO.File.Delete(@"0:\content\sys\option-editprotectedfiles.gms");
                            System.IO.File.Delete(@"0:\content\sys\option-deleteprotectedfiles.gms");
                            var directory_list = System.IO.Directory.GetFiles(@"0:\");
                            foreach (var file in directory_list)
                            {
                                System.IO.File.Delete(@"0:\" + file);
                            }
                            var directory_list3 = System.IO.Directory.GetDirectories(@"0:\");
                            foreach (var directory in directory_list3)
                            {
                                System.IO.File.Delete(@"0:\" + directory);
                            }
                            */
                            DrawFrame();
                            int screenWidth = 80;
                            string title = "RESET IN PROGRESS";
                            string q = @"GoOS is performing a full format on drive 0:\";
                            string w = "Do not turn off your computer.";
                            string e = "";
                            string r = "This process will take up to a few minutes";
                            string t = "depending on your Disk size.";
                            string y = "";
                            string u = "";
                            string i = "";

                            int titlePos = screenWidth / 2 - title.Length / 2;
                            int qPos = screenWidth / 2 - q.Length / 2;
                            int wPos = screenWidth / 2 - w.Length / 2;
                            int ePos = screenWidth / 2 - e.Length / 2;
                            int rPos = screenWidth / 2 - r.Length / 2;
                            int tPos = screenWidth / 2 - t.Length / 2;
                            int yPos = screenWidth / 2 - y.Length / 2;
                            int uPos = screenWidth / 2 - u.Length / 2;
                            int iPos = screenWidth / 2 - i.Length / 2;

                            Console.SetCursorPosition(titlePos, 2);
                            Console.Write(title);
                            Console.SetCursorPosition(qPos, 3);
                            Console.Write(q);
                            Console.SetCursorPosition(wPos, 4);
                            Console.Write(w);
                            Console.SetCursorPosition(rPos, 10);
                            Console.Write(r);
                            Console.SetCursorPosition(rPos, 11);
                            Console.Write(t);

                            Kernel.FS.Disks[0].FormatPartition(0, "FAT32", false);

                            DrawFrame();
                            screenWidth = 80;
                            title = "System reset complete.";
                            q = "GoOS is now back to factory default settings.";
                            w = "The system will no longer operate until restarted.";
                            e = "";
                            r = "Once restarted, the GoOS setup will launch instantly.";
                            t = "";
                            y = "";
                            u = "";
                            i = "";

                            titlePos = screenWidth / 2 - title.Length / 2;
                            qPos = screenWidth / 2 - q.Length / 2;
                            wPos = screenWidth / 2 - w.Length / 2;
                            ePos = screenWidth / 2 - e.Length / 2;
                            rPos = screenWidth / 2 - r.Length / 2;
                            tPos = screenWidth / 2 - t.Length / 2;
                            yPos = screenWidth / 2 - y.Length / 2;
                            uPos = screenWidth / 2 - u.Length / 2;
                            iPos = screenWidth / 2 - i.Length / 2;

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
                        catch (Exception monkeyballs)
                        {
                            DrawFrame();
                            int screenWidth = 80;
                            string title = "error";
                            string q = @"error";
                            string w = "error";
                            string e = "";
                            string r = "error";
                            string t = monkeyballs.ToString();
                            string y = "";
                            string u = "";
                            string i = "";

                            int titlePos = screenWidth / 2 - title.Length / 2;
                            int qPos = screenWidth / 2 - q.Length / 2;
                            int wPos = screenWidth / 2 - w.Length / 2;
                            int ePos = screenWidth / 2 - e.Length / 2;
                            int rPos = screenWidth / 2 - r.Length / 2;
                            int tPos = screenWidth / 2 - t.Length / 2;
                            int yPos = screenWidth / 2 - y.Length / 2;
                            int uPos = screenWidth / 2 - u.Length / 2;
                            int iPos = screenWidth / 2 - i.Length / 2;

                            Console.SetCursorPosition(titlePos, 2);
                            Console.Write(title);
                            Console.SetCursorPosition(qPos, 3);
                            Console.Write(q);
                            Console.SetCursorPosition(wPos, 4);
                            Console.Write(w);
                            Console.SetCursorPosition(rPos, 10);
                            Console.Write(r);
                            Console.SetCursorPosition(rPos, 11);
                            Console.Write(t);
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
                        menu = "main";
                        selected = 2;
                        DrawFrame();
                        Console.ForegroundColor = WindowText;
                    }

                    
                    menu = "main";
                    selected = 2;
                    DrawFrame();
                    Console.ForegroundColor = WindowText;
                }
            }
            Console.BackgroundColor = Black;
            Console.Clear();
        }

        private static void emptyDir(string path)
        {
            var dirtokill = System.IO.Directory.GetFiles(path);
            foreach (var files in dirtokill)
            {
                System.IO.File.Delete(@"0:\" + files);
            }
        }
    }
}