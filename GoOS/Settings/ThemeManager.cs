using System;
using System.IO;
using System.Collections.Generic;
using static System.ConsoleColor;

namespace GoOS.Themes
{
    public enum Theme
    {
        Default = 0
    }

    public static class ThemeManager
    {
        public static Theme CurrentTheme;

        public static ConsoleColor Default;
        public static ConsoleColor[] Startup;
        public static ConsoleColor WindowText;
        public static ConsoleColor WindowBorder;
        public static ConsoleColor ErrorText;
        public static ConsoleColor Other1;

        public static void SetTheme(Theme theme)
        {
            if (theme == Theme.Default)
            {
                Default = White;
                Startup = new ConsoleColor[3] { DarkMagenta, Red, DarkRed };
                WindowText = Cyan;
                WindowBorder = Green;
                ErrorText = Red;
                Other1 = Yellow;
            }
            CurrentTheme = theme;
        }

        private static Dictionary<string, ConsoleColor> StringToConsoleColor = new Dictionary<string, ConsoleColor>()
        {
            { "Black", Black }, { "DarkBlue", DarkBlue },
            { "DarkGreen", DarkGreen }, { "DarkCyan", DarkCyan },
            { "DarkRed", DarkRed },{ "DarkMagenta", DarkMagenta },
            { "DarkYellow", DarkYellow }, { "Gray", Gray },
            { "DarkGray", DarkGray },{ "Blue", Blue },
            { "Green", Green },{ "Cyan", Cyan },
            { "Red", Red }, { "Magenta", Magenta },
            { "Yellow", Yellow }, { "White", White },
        };

        public static void SetTheme(string themeFile)
        {
            try
            {
                if (File.Exists(themeFile) && themeFile.EndsWith(".gtheme"))
                {
                    string[] themeContents = File.ReadAllLines(themeFile);

                    foreach (string line in themeContents)
                    {
                        if (line.StartsWith("Default = "))
                        {
                            string result = line.Substring(10);

                            if (StringToConsoleColor.TryGetValue(result, out ConsoleColor colorval))
                            {
                                Default = colorval;
                            }
                        }
                        else if (line.StartsWith("Startup = "))
                        {
                            string[] result = line.Substring(10).Split(',');

                            for (int i = 0; i < 3; i++)
                            {
                                if (StringToConsoleColor.TryGetValue(result[i], out ConsoleColor colorval))
                                {
                                    Startup[i] = colorval;
                                }
                            }
                        }
                        else if (line.StartsWith("WindowText = "))
                        {
                            string result = line.Substring(13);

                            if (StringToConsoleColor.TryGetValue(result, out ConsoleColor colorval))
                            {
                                WindowText = colorval;
                            }
                        }
                        else if (line.StartsWith("WindowBorder = "))
                        {
                            string result = line.Substring(15);

                            if (StringToConsoleColor.TryGetValue(result, out ConsoleColor colorval))
                            {
                                WindowBorder = colorval;
                            }
                        }
                        else if (line.StartsWith("ErrorText = "))
                        {
                            string result = line.Substring(12);

                            if (StringToConsoleColor.TryGetValue(result, out ConsoleColor colorval))
                            {
                                ErrorText = colorval;
                            }
                        }
                        else if (line.StartsWith("Other1 = "))
                        {
                            string result = line.Substring(9);

                            if (StringToConsoleColor.TryGetValue(result, out ConsoleColor colorval))
                            {
                                Other1 = colorval;
                            }
                        }
                    }

                    Console.ForegroundColor = ThemeManager.WindowText;
                    Console.WriteLine("Theme changed successfully!");
                }
                else
                {
                    Console.ForegroundColor = ThemeManager.ErrorText;
                    Console.WriteLine("Theme file doesn't exist or is not a Goplex Theme File!");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ThemeManager.ErrorText;
                Console.WriteLine("Error while setting theme!\n" + e);
            }
        }
    }
}
