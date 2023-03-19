using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ConsoleColor;

namespace GoOS.Themes
{
    public enum Theme
    {
        Default = 0,
        Mono = 1
    }

    public static class ThemeManager
    {
        public static Theme CurrentTheme;

        public static ConsoleColor Default;
        public static ConsoleColor Startup1;
        public static ConsoleColor Startup2;
        public static ConsoleColor Startup3;
        public static ConsoleColor Startup4;
        public static ConsoleColor Startup5;
        public static ConsoleColor Startup6;
        public static ConsoleColor Startup7;
        public static ConsoleColor Startup8;
        public static ConsoleColor Startup9;
        public static ConsoleColor Startup10;
        public static ConsoleColor Startup11;
        public static ConsoleColor Startup12;
        public static ConsoleColor Startup13;
        public static ConsoleColor Startup14;
        public static ConsoleColor Startup15;
        public static ConsoleColor Startup16;
        public static ConsoleColor Startup17;
        public static ConsoleColor Startup18;
        public static ConsoleColor Startup19;
        public static ConsoleColor WindowText;
        public static ConsoleColor WindowBorder;
        public static ConsoleColor ErrorText;
        public static ConsoleColor Other1;

        public static void SetTheme(Theme theme)
        {
            if (theme == Theme.Default)
            {
                Default = White;
                Startup1 = DarkMagenta;
                Startup2 = Red;
                Startup3 = DarkRed;
                Startup4 = Magenta;
                Startup5 = DarkMagenta;
                Startup6 = Red;
                Startup7 = DarkRed;
                Startup8 = Magenta;
                Startup9 = Red;
                Startup10 = DarkRed;
                Startup11 = Magenta;
                Startup12 = DarkMagenta;
                Startup13 = Red;
                Startup14 = DarkRed;
                Startup15 = Magenta;
                Startup16 = DarkMagenta;
                Startup17 = Red;
                Startup18 = DarkRed;
                Startup19 = Magenta;
                WindowText = Cyan;
                WindowBorder = Green;
                ErrorText = Red;
                Other1 = Yellow;
            }
            else if (theme == Theme.Mono)
            {
                Default = White;
                Startup1 = White;
                Startup2 = White;
                Startup3 = White;
                Startup4 = White;
                Startup5 = White;
                Startup6 = White;
                Startup7 = White;
                Startup8 = White;
                Startup9 = White;
                Startup10 = White;
                Startup11 = White;
                Startup12 = White;
                Startup13 = White;
                Startup14 = White;
                Startup15 = White;
                Startup16 = White;
                Startup17 = White;
                Startup18 = White;
                Startup19 = White;
                WindowText = White;
                WindowBorder = White;
                ErrorText = White;
                Other1 = White;
            }
            CurrentTheme = theme;
        }
    }
}
