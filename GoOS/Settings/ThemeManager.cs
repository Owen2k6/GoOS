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
            else if (theme == Theme.Mono)
            {
                Default = White;
                Startup = new ConsoleColor[3] { White, White, White };
                WindowText = White;
                WindowBorder = White;
                ErrorText = White;
                Other1 = White;
            }
            CurrentTheme = theme;
        }
    }
}
