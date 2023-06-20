using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Sys = Cosmos.System;
using Cosmos.System.ScanMaps;
using GoOS.Themes;

namespace GoOS
{
    /// <summary>
    /// Settings app.
    /// </summary>
    public static class ControlPanel
    {
        private static bool isRunning = true;

        private static readonly string Frame =
            "┌──────────────┬─────────────────────────────────────────────────────────────────────────┐" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "├──────────────┼─────────────────────────────────────────────────────────────────────────┤" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "│              │                                                                         │" +
            "└──────────────┴───────────────────────────────┤ ESC - Exit / Arrow Keys - Select Item ├─";

        private static readonly List<string> categoryButtonsGeneralMenu = new List<string>
        {
            "Keyboard",
            "Themes",
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
            ("105GBQWERTY-GB-1.0", new GBStandardLayout()), // 4
            ("104USQWERTY-US-1.0", new USStandardLayout()), // 6
            ("105DEQWERTY-DE-1.0", new DEStandardLayout()), // 8
            ("105ESQWERTY-ES-1.0", new ESStandardLayout()), // 10
            ("105FRQWERTY-FR-1.0", new FRStandardLayout()), // 12
            ("105TRQWERTY-TR-1.0", new TRStandardLayout()) // 14
        };

        /// <summary>
        /// Launches Settings.
        /// </summary>
        public static void Launch()
        {
            isRunning = true;
            Console.Clear();
            MainLoop();
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

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Escape:
                        isRunning = false;
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
                        if (categorieSelectedButton > 1)
                        {
                            categorieSelectedButton = 0;
                        }
                        else if (categorieSelectedButton < 0)
                        {
                            categorieSelectedButton = 1;
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
        private static void DrawMenu()
        {
            DrawFrame();
            DrawTitle("GoOS Settings");
            DrawClock();
            DrawButtons();
        }

        private static void DrawButtons(bool quick = false)
        {
            // Draw the menu buttons
            string categorieToShow = string.Empty, menuToShow = string.Empty;
            int nextPos = 18;
            for (int i = 0; i < menuButtons.Count; i++)
            {
                bool highlighed = i == menuSelectedButton;
                DrawButton(menuButtons[i], nextPos, 27,
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

            if (!quick)
                ShowMenu(menuToShow, categorieToShow);
        }

        /// <summary>
        /// Draws the frame.
        /// </summary>
        private static void DrawFrame()
        {
            DrawText(Frame, 0, 0, ThemeManager.WindowBorder, ThemeManager.Background);
        }

        /// <summary>
        /// Draws the clock.
        /// </summary>
        private static void DrawClock()
        {
            string Hour = Cosmos.HAL.RTC.Hour.ToString(), Minute = Cosmos.HAL.RTC.Minute.ToString();
            if (Minute.Length < 2) Minute = "0" + Minute;
            DrawButton(Hour + ":" + Minute, 5, 27, true);
        }

        /// <summary>
        /// Draws a title at the top center of the screen.
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Y"></param>
        private static void DrawTitle(string title)
        {
            ClearTitle(); // No need to redraw the entire frame, this will reduce lag
            DrawText(" " + title + " ", 45 - title.Length / 2 - 2, 0, ThemeManager.WindowText,
                ThemeManager.Background); // Draw the title
        }

        /// <summary>
        /// Clears the title for DrawTitle()
        /// </summary>
        private static void ClearTitle()
        {
            DrawText(Frame.Split('\n')[0], 0, 0, ThemeManager.WindowBorder, ThemeManager.Background);
        }

        /// <summary>
        /// Draws a message where the title is for a brief moment, then draws the old title
        /// </summary>
        private static void DrawMessage(string message)
        {

            DrawTitle(message);
            Thread.Sleep(500);
            ClearTitle();
            DrawTitle("GoOS Settings");
        }

        /// <summary>
        /// Draws a button.
        /// </summary>
        /// <param name="text">The text of the button.</param>
        /// <param name="x">X coordinate of the button.</param>
        /// <param name="y">Y coordinate of the button.</param>
        /// <param name="highlighted">Makes the button highlighted or not.</param>
        private static void DrawButton(string text, int x, int y, bool highlighted)
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
        private static void DrawText(string text, int x, int y, ConsoleColor foreColor, ConsoleColor backColor)
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
        private static void ShowMenu(string menu, string category)
        {
            if (category == menuButtons[0]) // General category
            {
                if (menu == categoryButtonsGeneralMenu[0])
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

                    DrawText("Allows you to change your keyboard distribution.", 18, 23, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("Available keyboard distributions:", 18, 2, ThemeManager.WindowText,
                        ThemeManager.Background);
                    for (int i = 0; i < scanMaps.Count; i++)
                    {
                        DrawButton(scanMaps[i].Item1, 18, 4 + i,
                            i == keyboardMenuSelectedButton); // Draw button automatically at the correct coordinates
                    }

                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Escape:
                            ClearMenu();
                            break;

                        case ConsoleKey.Enter:
                            Sys.KeyboardManager.SetKeyLayout(scanMaps[keyboardMenuSelectedButton].Item2);
                            DrawMessage("Set layout to " + scanMaps[keyboardMenuSelectedButton].Item1);
                            DrawButtons(true);
                            DrawClock();
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
                else if (menu == categoryButtonsGeneralMenu[1])
                {
                    int themeMenuSelectedButton = 0;

                Refresh:
                    DrawText("Allows you to change GoOS's theme.", 18, 23, ThemeManager.WindowText,
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
                            DrawMessage("Theme changed successfully!");
                            DrawButtons(true);
                            DrawClock();
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
            else if (category == menuButtons[1])
            {
                if (menu == categoryButtonsAdvancedMenu[0])
                {
                    DrawText("Allows you to system reset GoOS.", 18, 23, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("Press R to reset the system, otherwise press ESC to", 18, 2, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("exit this menu.", 18, 3, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("WARNING: This will erase all the data in your hard disk", 18, 5, ThemeManager.ErrorText,
                        ThemeManager.Background);
                    DrawText("drive!", 18, 6, ThemeManager.ErrorText, ThemeManager.Background);

                    Refresh:
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Escape:
                            ClearMenu();
                            break;

                        case ConsoleKey.R:
                            Console.Clear();
                            DrawText("Reset in progress...", 0, 0, ThemeManager.ErrorText, ThemeManager.Background);
                            DrawText("Don't turn off your computer!", 0, 1, ThemeManager.Background,
                                ThemeManager.ErrorText);
                            DrawText("Formatting drive...", 0, 3, ThemeManager.WindowText, ThemeManager.Background);
                            Console.SetCursorPosition(0, 5);
                            Kernel.FS.Disks[0].FormatPartition(0, "FAT32", false);
                            Console.Clear();

                            for (int i = 40; i > 0; i--) // 125ms * 40 = 5s
                            {
                                DrawText("Restarting in " + i / 4 + " seconds...", 0, 0, ThemeManager.WindowText,
                                    ThemeManager.Background);
                                //DrawText(new string('█', i) + new string('▒', 40 - i), 0, 2, ThemeManager.WindowText, ThemeManager.Background);
                                DrawText(new string('/', i) + new string('_', 40 - i), 0, 2, ThemeManager.WindowText,
                                    ThemeManager.Background);
                                Thread.Sleep(250);
                            }

                            Cosmos.HAL.Power.CPUReboot();
                            break;

                        default:
                            goto Refresh;
                    }
                }
            }
            else if (category == menuButtons[2])
            {
                if (menu == categoryButtonsInfoMenu[0])
                {
                    DrawText("Shows you info about GoOS.", 18, 23, ThemeManager.WindowText, ThemeManager.Background);
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
                }
                else if (menu == categoryButtonsInfoMenu[1])
                {
                    DrawText("Shows you info about support for GoOS.", 18, 23, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("GoOS Support", 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("For support, open a ticket in the discord server.", 18, 4, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("For reporting an issue, please report the issue in the", 18, 6, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("issues tab in the Github repository.", 18, 7, ThemeManager.WindowText,
                        ThemeManager.Background);
                }
                else if (menu == categoryButtonsInfoMenu[2])
                {
                    DrawText("Shows you info about your system.", 18, 23, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("GoOS Kernel " + Kernel.BuildType + " " + Kernel.version, 18, 2, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("CPU: " + Cosmos.Core.CPU.GetCPUBrandString(), 18, 4, ThemeManager.WindowText,
                        ThemeManager.Background);
                    DrawText("Available RAM: " + Cosmos.Core.CPU.GetAmountOfRAM() + "mb", 18, 5,
                        ThemeManager.WindowText, ThemeManager.Background);
                    //This freezes the vm so keep it commented out until we have a solution
                    //DrawText("Available disk space: " + Sys.FileSystem.VFS.VFSManager.GetAvailableFreeSpace(@"0:\"), 18, 6, ThemeManager.WindowText, ThemeManager.Background);
                }
            }
        }

        /// <summary>
        /// Clear the space for menus.
        /// </summary>
        private static void ClearMenu()
        {
            Console.BackgroundColor = ThemeManager.Background;
            for (int i = 2; i < 24; i++) // Loop from Y(2) to Y(19)
            {
                Console.SetCursorPosition(18, i);
                Console.Write(new string(' ', 59));
            }
        }
    }
}
