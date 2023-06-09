using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Sys = Cosmos.System;
using Cosmos.System.ScanMaps;
using GoOS.Themes;

// Goplex Studios - GoOS
// Copyright (C) 2022  Owen2k6

namespace GoOS
{
    public static class ControlPanel
    {
        private static bool isRunning = true, canEnterMenu = false;

        /// <summary>
        /// Launches Settings.
        /// </summary>
        public static void Launch()
        {
            isRunning = true;
            // Hide the cursor while in Settings, show it back when exited
            Console.Clear();
            Console.CursorVisible = false;
            MainLoop();
            Console.CursorVisible = true;
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
                Console.CursorVisible = false; // Make the cursor not visible every loop cycle because for some reason it gets set back to be visible
                DrawMenu();

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Escape:
                        isRunning = false;
                        break;

                    case ConsoleKey.Enter:
                        canEnterMenu = true;
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

                switch (menuSelectedButton)
                {
                    case 0: // General menu
                        if (categorieSelectedButton > 1)
                        {
                            categorieSelectedButton = 0;
                        }
                        if (categorieSelectedButton < 0)
                        {
                            categorieSelectedButton = 1;
                        }
                        break;

                    case 1: // Advanced menu
                        if (categorieSelectedButton > 0)
                        {
                            categorieSelectedButton = 0;
                        }
                        if (categorieSelectedButton < 0)
                        {
                            categorieSelectedButton = 0;
                        }
                        break;
                }

                Console.SetCursorPosition(0, 0); // Do this instead of clearing the screen, will remove flickering
            }
        }

        private static string Frame = "┌──────────────┬─────────────────────────────────────────────────────────────────────────┐" +
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
                                      "└──────────────┴────┤ ESC - Exit │ ARROWS - Select Item │ ENTER - Enter Menu ├───────────";

        private static List<string> categoryButtonsGeneralMenu = new List<string>
        {
            // These need spaces for proper centering
            "Keyboard",
            "Themes  ",
        };

        private static List<string> categoryButtonsAdvancedMenu = new List<string>
        {
            // These too
            "Format  ",
        };

        private static List<string> categoryButtonsInfoMenu = new List<string>
        {
            // These too
            "Info    ",
            "Support ",
            "Hardware"
        };

        private static List<string> menuButtons = new List<string>
        {
            // These don't
            "General",
            "Advanced",
            "Info"
        };

        /// <summary>
        /// Draws the menu.
        /// </summary>
        private static void DrawMenu()
        {
            DrawFrame();
            DrawTitle("GoOS Control Panel");
            DrawClock();

            // Draw the menu buttons
            string categorieToShow = string.Empty, menuToShow = string.Empty;
            int nextPos = 18;
            for (int i = 0; i < menuButtons.Count; i++)
            {
                bool highlighed = i == menuSelectedButton;
                DrawButton(menuButtons[i], nextPos, 27, highlighed); // Draw button automatically at the correct coordinates
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
                        DrawButton(categoryButtonsGeneralMenu[i], 3, 2 + (i * 2), highlighed); // Draw button automatically at the correct coordinates
                        if (highlighed)
                            menuToShow = categoryButtonsGeneralMenu[i];
                    }
                    break;

                case 1:
                    // Draw the sidebar buttons for the Advanced category

                    for (int i = 0; i < categoryButtonsAdvancedMenu.Count; i++)
                    {
                        bool highlighed = i == categorieSelectedButton;
                        DrawButton(categoryButtonsAdvancedMenu[i], 3, 2 + (i * 2), highlighed); // Draw button automatically at the correct coordinates
                        if (highlighed)
                            menuToShow = categoryButtonsAdvancedMenu[i];
                    }
                    break;

                case 2:
                    // Draw the sidebar buttons for the Advanced category

                    for (int i = 0; i < categoryButtonsInfoMenu.Count; i++)
                    {
                        bool highlighed = i == categorieSelectedButton;
                        DrawButton(categoryButtonsInfoMenu[i], 3, 2 + (i * 2), highlighed); // Draw button automatically at the correct coordinates
                        if (highlighed)
                            menuToShow = categoryButtonsInfoMenu[i];
                    }
                    break;
            }

            if (canEnterMenu)
            {
                ShowMenu(menuToShow, categorieToShow);
                canEnterMenu = false;
            }
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
            DrawButton(DateTime.Now.Hour + ":" + DateTime.Now.Minute, 5, 27, true);
        }

        /// <summary>
        /// Draws a title at the top, center of the screen.
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Y"></param>
        private static void DrawTitle(string title)
        {
            ClearTitle(); // No need to redraw the entire frame, this will reduce lag
            DrawText(" " + title + " ", 45 - (title.Length / 2) - 2, 0, ThemeManager.WindowText, ThemeManager.Background); // Draw the title
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
            Console.CursorVisible = false;
            DrawTitle(message);
            Thread.Sleep(500);
            ClearTitle(); // There's a blank space at (0, 0), fix it with this
            DrawTitle("GoOS Control Panel");
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

        private static List<(string, Sys.ScanMapBase)> scanMaps = new List<(string, Sys.ScanMapBase)>()
        {
            ("105GBQWERTY-GB-1.0", new GBStandardLayout()), // 4
            ("104USQWERTY-US-1.0", new USStandardLayout()), // 6
            ("105DEQWERTY-DE-1.0", new DEStandardLayout()), // 8
            ("105ESQWERTY-ES-1.0", new ESStandardLayout()), // 10
            ("105FRQWERTY-FR-1.0", new FRStandardLayout()), // 12
            ("105TRQWERTY-TR-1.0", new TRStandardLayout())  // 14
        };

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

                    DrawText("Allows you to change your keyboard distribution.", 18, 23, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("Available keyboard distributions:", 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    for (int i = 0; i < scanMaps.Count; i++)
                    {
                        DrawButton(scanMaps[i].Item1, 18, 4 + i, i == keyboardMenuSelectedButton); // Draw button automatically at the correct coordinates
                    }

                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Escape:
                            ClearMenu();
                            break;

                        case ConsoleKey.Enter:
                            //Cosmos.System.KeyboardManager.SetKeyLayout(scanMaps[keyboardMenuSelectedButton].Item2);
                            break;

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
                    string currentThemeFile = File.ReadAllText(@"0:\content\sys\theme.gms").Substring(12);

                    DrawText("Allows you to change GoOS's theme.", 18, 23, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("Current theme file: " + currentThemeFile, 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("New theme file (esc to cancel): ", 18, 4, ThemeManager.WindowText, ThemeManager.Background);

                    string newThemeFile = string.Empty;

                Refresh:
                    Console.CursorVisible = true;
                    Console.ForegroundColor = ThemeManager.WindowText;
                    Console.BackgroundColor = ThemeManager.Background;

                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            ClearMenu();
                            break;

                        case ConsoleKey.Enter:
                            newThemeFile = newThemeFile.Trim();
                            if (newThemeFile.Length > 0)
                            {
                                //File.WriteAllText(@"0:\content\sys\theme.gms", "ThemeFile = " + newThemeFile);
                                //ThemeManager.SetTheme(newThemeFile);
                                DrawMessage("Saved as \"" + newThemeFile + "\"");
                            }
                            else
                            {
                                DrawMessage("Cannot be empty!");
                            }
                            break;

                        case ConsoleKey.Backspace:
                            try
                            {
                                newThemeFile = newThemeFile.Remove(newThemeFile.Length - 1);
                                Console.CursorLeft--;
                                Console.Write(' ');
                                Console.CursorLeft--;
                            }
                            catch
                            {
                                Console.Beep(880, 100);
                            }
                            goto Refresh;

                        default:
                            Console.Write(key.KeyChar);
                            newThemeFile += key.KeyChar;
                            goto Refresh;
                    }

                    Console.CursorVisible = false;

                    ClearMenu();
                }
            }
            else if (category == menuButtons[1])
            {
                if (menu == categoryButtonsAdvancedMenu[0])
                {
                    DrawText("Allows you to system reset GoOS.", 18, 23, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("Press \"R\" to reset the system, otherwise press ESC to", 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("exit this menu.", 18, 3, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("WARNING: This will erase all the data in your hard disk", 18, 5, ThemeManager.ErrorText, ThemeManager.Background);
                    DrawText("drive!", 18, 6, ThemeManager.ErrorText, ThemeManager.Background);

                Refresh:
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.Escape:
                            ClearMenu();
                            break;

                        case ConsoleKey.R:
                            ClearMenu();
                            DrawText("Reset in progress...", 18, 2, ThemeManager.ErrorText, ThemeManager.Background);
                            DrawText("Please, DON'T TURN OFF YOUR COMPUTER!", 18, 23, ThemeManager.ErrorText, ThemeManager.Background);
                            DrawText("1. Formatting drive...", 18, 4, ThemeManager.WindowText, ThemeManager.Background);
                            //Kernel.FS.Disks[0].FormatPartition(0, "FAT32", false);
                            Console.SetCursorPosition(18, 6);
                            Console.Write("2. Restarting...");
                            //Cosmos.HAL.Power.CPUReboot();
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
                    DrawText("GoOS " + Kernel.BuildType + " " + Kernel.version, 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("GoOS is a free and open source software built with", 18, 4, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("CosmosOS. If you paid for this software, you should request", 18, 5, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("a refund or report to proper authorities.", 18, 6, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("GoOS is and always will be open-source and free.", 18, 8, ThemeManager.WindowText, ThemeManager.Background);
                }
                else if (menu == categoryButtonsInfoMenu[1])
                {
                    DrawText("GoOS Support", 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("For support, open a ticket in the discord server.", 18, 4, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("For reporting an issue, please report the issue at", 18, 6, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("http://bugs.owen2k6.com/", 18, 7, ThemeManager.WindowText, ThemeManager.Background);
                }
                else if (menu == categoryButtonsInfoMenu[2])
                {
                    DrawText("GoOS " + Kernel.BuildType + " " + Kernel.version, 18, 2, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("CPU: " + Cosmos.Core.CPU.GetCPUBrandString(), 18, 4, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("Available RAM: " + Cosmos.Core.CPU.GetAmountOfRAM(), 18, 5, ThemeManager.WindowText, ThemeManager.Background);
                    DrawText("Available disk space (Bytes): " + Sys.FileSystem.VFS.VFSManager.GetAvailableFreeSpace(@"0:\"), 18, 6, ThemeManager.WindowText, ThemeManager.Background);
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
