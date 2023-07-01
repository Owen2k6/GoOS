using System.IO;
using System.Threading;
using System.Collections.Generic;
using Sys = Cosmos.System;
using Cosmos.System.ScanMaps;
using GoOS.Themes;
using Convert = System.Convert;
using ConsoleKey = System.ConsoleKey;
using Console = BetterConsole;
using ConsoleColor = PrismAPI.Graphics.Color;

namespace GoOS
{
    /// <summary>
    /// Settings app.
    /// </summary>
    public static class ControlPanel
    {
        private static bool isRunning = true;

        private static readonly List<string> categoryButtonsGeneralMenu = new List<string>
        {
            "Keyboard",
            "Themes",
            "Display"
        };

        private static readonly List<string> categoryButtonsAdvancedMenu = new List<string>
        {
            "Format",
        };

        private static readonly List<string> categoryButtonsInfoMenu = new List<string>
        {
            "Info",
            "Support",
            "Hardware"
        };

        private static readonly List<string> menuButtons = new List<string>
        {
            "General",
            "Advanced",
            "Info"
        };

        private static readonly List<(string, Sys.ScanMapBase)> scanMaps = new()
        {
            ("105GBQWERTY-GB-1.0", new GBStandardLayout()),
            ("104USQWERTY-US-1.0", new USStandardLayout()),
            ("105DEQWERTY-DE-1.0", new DEStandardLayout()),
            ("105ESQWERTY-ES-1.0", new ESStandardLayout()),
            ("105FRQWERTY-FR-1.0", new FRStandardLayout()),
            ("105TRQWERTY-TR-1.0", new TRStandardLayout())
        };

        public static readonly List<(string, (ushort Width, ushort Height))> videoModes = new()
        {
            ("640 x 480", (640, 480)),
            ("720 x 480", (720, 480)),
            ("800 x 600", (800, 600)),
            ("1024 x 768", (1024, 768)),
            ("1280 x 720", (1280, 720)),
            ("1600 x 900", (1600, 900)),
            ("1920 x 1080", (1920, 1080))
        };

        private static readonly List<string> mainMenuControls = new()
        {
            "Escape - Return to console",
            "Arrow Keys - Select item",
            "Return - Enter menu"
        };

        /// <summary>
        /// Launches Settings.
        /// </summary>
        public static void Launch()
        {
            isRunning = true;
            menuToShow = categoryButtonsGeneralMenu[0];
            categorieToShow = menuButtons[0];
            Console.DoubleBufferedMode = true;
            Console.Clear();
            MainLoop();
            Console.ForegroundColor = ThemeManager.WindowText;
            Console.BackgroundColor = ThemeManager.Background;
            Console.DoubleBufferedMode = false;
            Console.Clear();
        }

        private static int categorieSelectedButton = 0, menuSelectedButton = 0;

        /// <summary>
        /// Main loop for Settings.
        /// </summary>
        private static void MainLoop()
        {
            while (isRunning)
            {
                DrawMenu();
                ShowMenu(menuToShow, categorieToShow, true);
                Console.Render();

                ConsoleKey key = System.Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Escape:
                        isRunning = false;
                        break;

                    case ConsoleKey.Enter:
                        ShowMenu(menuToShow, categorieToShow, false);
                        break;

                    case ConsoleKey.UpArrow:
                        categorieSelectedButton--;
                        break;

                    case ConsoleKey.DownArrow:
                        categorieSelectedButton++;
                        break;

                    case ConsoleKey.LeftArrow:
                        menuSelectedButton--;
                        break;

                    case ConsoleKey.RightArrow:
                        menuSelectedButton++;
                        break;
                }

                if (menuSelectedButton > 2)
                {
                    menuSelectedButton = 0;
                }
                else if (menuSelectedButton < 0)
                {
                    menuSelectedButton = 2;
                }

                switch (menuSelectedButton)
                {
                    case 0: // General menu
                        if (categorieSelectedButton > 2)
                        {
                            categorieSelectedButton = 0;
                        }
                        else if (categorieSelectedButton < 0)
                        {
                            categorieSelectedButton = 2;
                        }

                        break;

                    case 1: // Advanced menu
                        if (categorieSelectedButton > 0)
                        {
                            categorieSelectedButton = 0;
                        }
                        else if (categorieSelectedButton < 0)
                        {
                            categorieSelectedButton = 0;
                        }

                        break;

                    case 2: // Info menu
                        if (categorieSelectedButton > 2)
                        {
                            categorieSelectedButton = 0;
                        }
                        else if (categorieSelectedButton < 0)
                        {
                            categorieSelectedButton = 2;
                        }

                        break;
                }

                Console.SetCursorPosition(0, 0); // Do this instead of clearing the screen, will remove flickering
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        private static void DrawMenu(bool quick = false)
        {
            DrawFrame();
            DrawTitle("GoOS Settings");
            DrawControls(mainMenuControls);
            DrawClock();
            DrawButtons();
            Console.Render();
        }

        private static string categorieToShow = string.Empty, menuToShow = string.Empty;

        private static void DrawButtons()
        {
            // Clear the buttons
            Console.Canvas.DrawFilledRectangle(3 * 8, 2 * 16, 12 * 8, Convert.ToUInt16(Console.Canvas.Height - (6 * 16)), 0, ThemeManager.Background);

            // Draw the menu buttons
            int nextPos = 18;
            for (int i = 0; i < menuButtons.Count; i++)
            {
                bool highlighed = i == menuSelectedButton;
                DrawButton(menuButtons[i], nextPos, Console.WindowHeight - 3,
                    highlighed); // Draw button automatically at the correct coordinates
                if (highlighed)
                    categorieToShow = menuButtons[i];

                nextPos += menuButtons[i].Length + 4;
            }

            // Draw the sidebar buttons
            switch (menuSelectedButton)
            {
                case 0:
                    // Draw the sidebar buttons for the General category

                    for (int i = 0; i < categoryButtonsGeneralMenu.Count; i++)
                    {
                        bool highlighed = i == categorieSelectedButton;
                        DrawButton(categoryButtonsGeneralMenu[i], 3, 2 + i * 2,
                            highlighed); // Draw button automatically at the correct coordinates
                        if (highlighed)
                            menuToShow = categoryButtonsGeneralMenu[i];
                    }

                    break;

                case 1:
                    // Draw the sidebar buttons for the Advanced category

                    for (int i = 0; i < categoryButtonsAdvancedMenu.Count; i++)
                    {
                        bool highlighed = i == categorieSelectedButton;
                        DrawButton(categoryButtonsAdvancedMenu[i], 3, 2 + i * 2,
                            highlighed); // Draw button automatically at the correct coordinates
                        if (highlighed)
                            menuToShow = categoryButtonsAdvancedMenu[i];
                    }

                    break;

                case 2:
                    // Draw the sidebar buttons for the Advanced category

                    for (int i = 0; i < categoryButtonsInfoMenu.Count; i++)
                    {
                        bool highlighed = i == categorieSelectedButton;
                        DrawButton(categoryButtonsInfoMenu[i], 3, 2 + i * 2,
                            highlighed); // Draw button automatically at the correct coordinates
                        if (highlighed)
                            menuToShow = categoryButtonsInfoMenu[i];
                    }

                    break;
            }
        }

        /// <summary>
        /// Draws the frame.
        /// </summary>
        private static void DrawFrame()
        {
            // Draw the frame with GUI instead of TUI
            Console.Canvas.DrawRectangle(3, 7, Convert.ToUInt16(Console.Canvas.Width - 6), Convert.ToUInt16(Console.Canvas.Height - 14), 0, ThemeManager.WindowBorder);
            Console.Canvas.DrawRectangle(4, 8, Convert.ToUInt16(Console.Canvas.Width - 6), Convert.ToUInt16(Console.Canvas.Height - 14), 0, ThemeManager.WindowBorder);
            Console.Canvas.DrawLine(123, 9, 123, Console.Canvas.Height - 7, ThemeManager.WindowBorder);
            Console.Canvas.DrawLine(124, 9, 124, Console.Canvas.Height - 7, ThemeManager.WindowBorder);
            Console.Canvas.DrawLine(5, Console.Canvas.Height - 64, Console.Canvas.Width - 5, Console.Canvas.Height - 64, ThemeManager.WindowBorder);
            Console.Canvas.DrawLine(6, Console.Canvas.Height - 64, Console.Canvas.Width - 5, Console.Canvas.Height - 64, ThemeManager.WindowBorder);
        }

        private static void DrawControls(List<string> controls)
        {
            // Clear controls
            Console.Canvas.DrawFilledRectangle(8, Console.Canvas.Height - 8, Convert.ToUInt16(Console.Canvas.Width - 16), 16, 0, ThemeManager.Background);
            Console.Canvas.DrawLine(8, Console.Canvas.Height - 7, Console.Canvas.Width - 8, Console.Canvas.Height - 7, ThemeManager.WindowBorder);
            Console.Canvas.DrawLine(8, Console.Canvas.Height - 8, Console.Canvas.Width - 8, Console.Canvas.Height - 8, ThemeManager.WindowBorder);

            // Draw controls
            string controlsStr = string.Empty;
            for (int i = 0; i < controls.Count; i++)
                controlsStr += controls[i] + " / ";
            controlsStr = controlsStr.Remove(controlsStr.Length - 3);
            DrawText(" " + controlsStr + " ", Console.WindowWidth - 3 - controlsStr.Length, Console.WindowHeight - 1, ThemeManager.WindowBorder, ThemeManager.Background);
        }

        /// <summary>
        /// Draws the clock.
        /// </summary>
        private static void DrawClock()
        {
            string Hour = Cosmos.HAL.RTC.Hour.ToString(), Minute = Cosmos.HAL.RTC.Minute.ToString();
            if (Minute.Length < 2) Minute = "0" + Minute;
            DrawButton(Hour + ":" + Minute, 5, Console.WindowHeight - 3, true);
        }

        /// <summary>
        /// Draws a title at the top center of the screen.
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Y"></param>
        private static void DrawTitle(string title)
        {
            Console.Canvas.DrawFilledRectangle(0, 0, Console.Canvas.Width, 16, 0, ThemeManager.Background);
            DrawFrame();

            DrawText(" " + title + " ", (Console.WindowWidth / 2) - (title.Length / 2) - 2, 0, ThemeManager.WindowText,
                ThemeManager.Background); // Draw the title
        }

        /// <summary>
        /// Draws a message where the title is for a brief moment, then draws the old title
        /// </summary>
        private static void DrawMessage(string message)
        {
            DrawTitle(message);
            Console.Render();
            Thread.Sleep(500);
            DrawTitle("GoOS Settings");
            Console.Render();
        }

        /// <summary>
        /// Draws a button.
        /// </summary>
        /// <param name="text">The text of the button.</param>
        /// <param name="x">X coordinate of the button.</param>
        /// <param name="y">Y coordinate of the button.</param>
        /// <param name="highlighted">Makes the button highlighted or not.</param>
        public static void DrawButton(string text, int x, int y, bool highlighted)
        {
            switch (highlighted)
            {
                case true:
                    DrawText(" " + text + " ", x, y, ThemeManager.Background, ThemeManager.WindowText);
                    break;

                case false:
                    DrawText(" " + text + " ", x, y, ThemeManager.WindowText, ThemeManager.Background);
                    break;
            }
        }

        /// <summary>
        /// Draws some text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="x">X coordinate of the text.</param>
        /// <param name="y">Y coordinate of the text.</param>
        public static void DrawText(string text, int x, int y, ConsoleColor foreColor, ConsoleColor backColor)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.SetCursorPosition(x, y); // Set the cursor to the desired coordinate
            Console.Write(text); // Draw the text
        }

        /// <summary>
        /// Shows a menu.
        /// </summary>
        /// <param name="menu">The menu to show.</param>
        /// <param name="category">The category to show.</param>
        private static void ShowMenu(string menu, string category, bool preview)
        {
            ClearMenu();
            if (category == menuButtons[0]) // General category
            {
                if (menu == categoryButtonsGeneralMenu[0])
                {
                    if (preview)
                    {
                        DrawText("Allows you to change your keyboard distribution.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("Available keyboard distributions:", 18, 2, ThemeManager.WindowText,
                            ThemeManager.Background);
                        for (int i = 0; i < scanMaps.Count; i++)
                        {
                            DrawButton(scanMaps[i].Item1, 18, 4 + i, false); // Draw button automatically at the correct coordinates
                        }
                    }
                    else
                    {
                        int keyboardMenuSelectedButton = 0;

                    Refresh:
                        if (keyboardMenuSelectedButton > 5)
                        {
                            keyboardMenuSelectedButton = 0;
                        }
                        else if (keyboardMenuSelectedButton < 0)
                        {
                            keyboardMenuSelectedButton = 5;
                        }

                        Console.ForegroundColor = ThemeManager.WindowText;
                        Console.BackgroundColor = ThemeManager.Background;

                        DrawText("Allows you to change your keyboard distribution.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("Available keyboard distributions:", 18, 2, ThemeManager.WindowText,
                            ThemeManager.Background);
                        for (int i = 0; i < scanMaps.Count; i++)
                        {
                            DrawButton(scanMaps[i].Item1, 18, 4 + i,
                                i == keyboardMenuSelectedButton); // Draw button automatically at the correct coordinates
                        }
                        Console.Render();

                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.Escape:
                                ClearMenu();
                                break;

                            case ConsoleKey.Enter:
                                Sys.KeyboardManager.SetKeyLayout(scanMaps[keyboardMenuSelectedButton].Item2);
                                DrawMessage("Set layout to " + scanMaps[keyboardMenuSelectedButton].Item1);
                                //DrawMenu();
                                goto Refresh;

                            case ConsoleKey.UpArrow:
                                keyboardMenuSelectedButton--;
                                goto Refresh;

                            case ConsoleKey.DownArrow:
                                keyboardMenuSelectedButton++;
                                goto Refresh;

                            default:
                                goto Refresh;
                        }
                    }
                }
                else if (menu == categoryButtonsGeneralMenu[1])
                {
                    if (preview)
                    {
                        DrawText("Allows you to change GoOS's theme.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("Available themes: ", 18, 2, ThemeManager.WindowText, ThemeManager.Background);

                        string[] themes = Directory.GetFiles(@"0:\content\themes\");
                        for (int i = 0; i < themes.Length; i++)
                        {
                            DrawButton(themes[i].Replace(".gtheme", ""), 18, 4 + i, false); // Draw button automatically at the correct coordinates
                        }
                    }
                    else
                    {
                        int themeMenuSelectedButton = 0;

                    Refresh:
                        DrawText("Allows you to change GoOS's theme.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("Available themes: ", 18, 2, ThemeManager.WindowText, ThemeManager.Background);

                        string[] themes = Directory.GetFiles(@"0:\content\themes\");

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
                            DrawButton(themes[i].Replace(".gtheme", ""), 18, 4 + i,
                                i == themeMenuSelectedButton); // Draw button automatically at the correct coordinates
                        }
                        Console.Render();

                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.Escape:
                                ClearMenu();
                                break;

                            case ConsoleKey.Enter:
                                File.WriteAllText(@"0:\content\sys\theme.gms",
                                    "ThemeFile = " + themes[themeMenuSelectedButton]);
                                ThemeManager.SetTheme(@"0:\content\themes\" + themes[themeMenuSelectedButton], false);
                                //DrawMenu();
                                DrawMessage("Theme changed successfully!");
                                goto Refresh;

                            case ConsoleKey.UpArrow:
                                themeMenuSelectedButton--;
                                goto Refresh;

                            case ConsoleKey.DownArrow:
                                themeMenuSelectedButton++;
                                goto Refresh;

                            default:
                                goto Refresh;

                        }
                    }
                }
                else if (menu == categoryButtonsGeneralMenu[2])
                {
                    if (preview)
                    {
                        DrawText("Allows you to change your video card's resolution.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("Available resolutions:", 18, 2, ThemeManager.WindowText,
                            ThemeManager.Background);

                        for (int i = 0; i < videoModes.Count; i++)
                        {
                            DrawButton(videoModes[i].Item1, 18, 4 + i, false); // Draw button automatically at the correct coordinates
                        }
                    }
                    else
                    {
                        int displayMenuSelectedButton = 0;

                    Refresh:
                        if (displayMenuSelectedButton > videoModes.Count - 1)
                        {
                            displayMenuSelectedButton = 0;
                        }
                        else if (displayMenuSelectedButton < 0)
                        {
                            displayMenuSelectedButton = videoModes.Count - 1;
                        }

                        Console.ForegroundColor = ThemeManager.WindowText;
                        Console.BackgroundColor = ThemeManager.Background;

                        DrawText("Allows you to change your video card's resolution.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("Available resolutions:", 18, 2, ThemeManager.WindowText,
                            ThemeManager.Background);

                        for (int i = 0; i < videoModes.Count; i++)
                        {
                            DrawButton(videoModes[i].Item1, 18, 4 + i,
                                i == displayMenuSelectedButton); // Draw button automatically at the correct coordinates
                        }
                        Console.Render();

                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.Escape:
                                ClearMenu();
                                break;

                            case ConsoleKey.Enter:
                                Console.Init(videoModes[displayMenuSelectedButton].Item2.Width, videoModes[displayMenuSelectedButton].Item2.Height);
                                File.Create(@"0:\content\sys\resolution.gms");
                                File.WriteAllBytes(@"0:\content\sys\resolution.gms", new byte[] { (byte)displayMenuSelectedButton });
                                DrawMenu();
                                DrawMessage("Set video mode to " + scanMaps[displayMenuSelectedButton].Item1);
                                //DrawMenu();
                                goto Refresh;

                            case ConsoleKey.UpArrow:
                                displayMenuSelectedButton--;
                                goto Refresh;

                            case ConsoleKey.DownArrow:
                                displayMenuSelectedButton++;
                                goto Refresh;

                            default:
                                goto Refresh;
                        }
                    }
                }
            }
            else if (category == menuButtons[1])
            {
                if (menu == categoryButtonsAdvancedMenu[0])
                {
                    if (preview)
                    {
                        DrawText("Allows you to system reset GoOS to factory settings.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                        ThemeManager.Background);
                        DrawText("Press R to reset the system, otherwise press ESC to", 18, 2, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("exit this menu.", 18, 3, ThemeManager.WindowText, ThemeManager.Background);
                        DrawText("WARNING: This will erase all the data in your hard disk", 18, 5, ThemeManager.ErrorText,
                            ThemeManager.Background);
                        DrawText("drive!", 18, 6, ThemeManager.ErrorText, ThemeManager.Background);
                    }
                    else
                    {
                        DrawText("Allows you to system reset GoOS to factory settings.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                        ThemeManager.Background);
                        DrawText("Press R to reset the system, otherwise press ESC to", 18, 2, ThemeManager.WindowText,
                            ThemeManager.Background);
                        DrawText("exit this menu.", 18, 3, ThemeManager.WindowText, ThemeManager.Background);
                        DrawText("WARNING: This will erase all the data in your hard disk", 18, 5, ThemeManager.ErrorText,
                            ThemeManager.Background);
                        DrawText("drive!", 18, 6, ThemeManager.ErrorText, ThemeManager.Background);
                        Console.Render();

                    Refresh:
                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.Escape:
                                ClearMenu();
                                break;

                            case ConsoleKey.R:
                                ClearMenu();
                                DrawText("Allows you to system reset GoOS to factory settings.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                                    ThemeManager.Background);

                                DrawText("RESET IN PROGRESS", 18, 2, ThemeManager.ErrorText, ThemeManager.Background);
                                DrawText("Do not turn off your computer.", 18, 3, ThemeManager.Background,
                                    ThemeManager.ErrorText);
                                Console.Render();
                                Kernel.FS.Disks[0].FormatPartition(0, "FAT32", false);

                                ClearMenu();
                                DrawText("Allows you to system reset GoOS to factory settings.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                                    ThemeManager.Background);

                                for (int i = 40; i > 0; i--) // 250ms * 40 = 10s
                                {
                                    DrawText("Restarting in " + i / 4 + " seconds...", 18, 2, ThemeManager.WindowText,
                                        ThemeManager.Background);
                                    DrawText(new string('█', i) + new string('▒', 40 - i), 18, 4, ThemeManager.WindowText,
                                        ThemeManager.Background);
                                    Console.Render();
                                    Thread.Sleep(250);
                                }

                                Cosmos.HAL.Power.CPUReboot();
                                break;

                            default:
                                goto Refresh;
                        }
                    }
                }
            }
            else if (category == menuButtons[2])
            {
                if (menu == categoryButtonsInfoMenu[0])
                {
                    DrawText("Shows you info about GoOS.", 18, Console.WindowHeight - 7, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("GoOS Kernel " + Kernel.BuildType + " " + Kernel.version, 18, 2, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("GoOS is a free and open source software designed with", 18, 4, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("CosmosOS. If you paid for this software, you should request", 18, 5,
                        ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("a refund or report to proper authorities.", 18, 6, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("GoOS is and always will be open-source and free.", 18, 8, ThemeManager.WindowText,
                        ThemeManager.Background);
                    Console.Render();
                }
                else if (menu == categoryButtonsInfoMenu[1])
                {
                    DrawText("Shows you info about support for GoOS.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("GoOS Support", 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("For support, open a ticket in the discord server.", 18, 4, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("For reporting an issue, please report the issue in the", 18, 6, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("issues tab in the Github repository.", 18, 7, ThemeManager.WindowText,
                        ThemeManager.Background);
                    Console.Render();
                }
                else if (menu == categoryButtonsInfoMenu[2])
                {
                    DrawText("Shows you info about your system.", 18, Console.WindowHeight - 7, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("GoOS Kernel " + Kernel.BuildType + " " + Kernel.version, 18, 2, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("CPU: " + Cosmos.Core.CPU.GetCPUBrandString(), 18, 4, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("Available RAM: " + Cosmos.Core.CPU.GetAmountOfRAM() + "mb", 18, 5,
                        ThemeManager.WindowText, ThemeManager.Background);
                    Console.Render();
                }
            }
            Console.DoubleBufferedMode = true;
        }

        /// <summary>
        /// Clear the space for menus.
        /// </summary>
        private static void ClearMenu()
        {
            // Clear the menu with GUI instead of TUI
            Console.Canvas.DrawFilledRectangle(18 * 8, 2 * 16, Convert.ToUInt16(Console.Canvas.Width - (20 * 8)), Convert.ToUInt16(Console.Canvas.Height - (6 * 16)), 0, ThemeManager.Background);
            //Console.Render();
        }
    }
}
