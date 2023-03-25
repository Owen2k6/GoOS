using System;
using GoOS.Themes;

namespace GoOS.Commands
{
    public class ToggleTheme
    {
        public static void Main()
        {
            Console.Clear();
            if (ThemeManager.CurrentTheme == Theme.Default)
            {
                ThemeManager.SetTheme(Theme.Mono);
            }
            else
            {
                ThemeManager.SetTheme(Theme.Default);
            }
        }
    }
}