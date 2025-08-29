using System;
using System.Collections.Generic;
using System.IO;
using Console = BetterConsole;
using ConsoleColor = Gold.Graphics.Color;
using static ConsoleColorEx;

namespace GoOS.Themes;

public enum Theme
{
    Fallback = 0
}

public static class ThemeManager
{
    public static ConsoleColor Default;
    public static ConsoleColor Background;
    public static ConsoleColor[] Startup;
    public static ConsoleColor WindowText;
    public static ConsoleColor WindowBorder;
    public static ConsoleColor ErrorText;
    public static ConsoleColor Other1;

    private static readonly Dictionary<string, ConsoleColor> StringToConsoleColor = new()
    {
        { "Black", Black }, { "DarkBlue", DarkBlue },
        { "DarkGreen", DarkGreen }, { "DarkCyan", DarkCyan },
        { "DarkRed", DarkRed }, { "DarkMagenta", DarkMagenta },
        { "DarkYellow", DarkYellow }, { "Gray", Gray },
        { "DarkGray", DarkGray }, { "Blue", Blue },
        { "Green", Green }, { "Cyan", Cyan },
        { "Red", Red }, { "Magenta", Magenta },
        { "Yellow", Yellow }, { "White", White }
    };

    public static void SetTheme(Theme theme)
    {
        if (theme == Theme.Fallback)
        {
            Default = White;
            Background = Black;
            Startup = new ConsoleColor[3] { DarkMagenta, Red, DarkRed };
            WindowText = Cyan;
            WindowBorder = Green;
            ErrorText = Red;
            Other1 = Yellow;
        }
    }

    public static void SetTheme(string themeFile, bool echo = true)
    {
        try
        {
            if (File.Exists(themeFile) && themeFile.EndsWith(".gtheme"))
            {
                var themeContents = File.ReadAllLines(themeFile);

                foreach (var line in themeContents)
                    if (line.StartsWith("Default = "))
                    {
                        var result = line.Substring(10);

                        if (StringToConsoleColor.TryGetValue(result, out var colorval)) Default = colorval;
                    }
                    else if (line.StartsWith("Background = "))
                    {
                        var result = line.Substring(13);

                        if (StringToConsoleColor.TryGetValue(result, out var colorval))
                        {
                            Background = colorval;
                            Console.BackgroundColor = colorval;
                        }
                    }
                    else if (line.StartsWith("Startup = "))
                    {
                        var result = line.Substring(10).Split(',');

                        for (var i = 0; i < 3; i++)
                            if (StringToConsoleColor.TryGetValue(result[i], out var colorval))
                                Startup[i] = colorval;
                    }
                    else if (line.StartsWith("WindowText = "))
                    {
                        var result = line.Substring(13);

                        if (StringToConsoleColor.TryGetValue(result, out var colorval)) WindowText = colorval;
                    }
                    else if (line.StartsWith("WindowBorder = "))
                    {
                        var result = line.Substring(15);

                        if (StringToConsoleColor.TryGetValue(result, out var colorval)) WindowBorder = colorval;
                    }
                    else if (line.StartsWith("ErrorText = "))
                    {
                        var result = line.Substring(12);

                        if (StringToConsoleColor.TryGetValue(result, out var colorval)) ErrorText = colorval;
                    }
                    else if (line.StartsWith("Other1 = "))
                    {
                        var result = line.Substring(9);

                        if (StringToConsoleColor.TryGetValue(result, out var colorval)) Other1 = colorval;
                    }

                File.WriteAllText(@"0:\content\sys\theme.gms", @"ThemeFile = " + themeFile);

                if (echo)
                {
                    Console.ForegroundColor = WindowText;
                    Console.WriteLine("ThemeManager - Theme changed successfully!");
                }
            }
            else
            {
                if (echo)
                {
                    Console.ForegroundColor = ErrorText;
                    Console.WriteLine("ThemeManager - Theme file doesn't exist or is not a Goplex Theme File!");
                }
            }
        }
        catch (Exception e)
        {
            if (echo)
            {
                Console.ForegroundColor = ErrorText;
                Console.WriteLine("ThemeManager - Error while setting theme!\n" + e);
            }
        }
    }
}