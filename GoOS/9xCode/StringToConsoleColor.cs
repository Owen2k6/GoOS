using System.Collections.Generic;
using ConsoleColor = Gold.Graphics.Color;
using static Gold.Graphics.Color;

namespace GoOS._9xCode
{
    public static partial class Interpreter
    {
        private static Dictionary<string, ConsoleColor> StringToConsoleColor = new Dictionary<string, ConsoleColor>()
        {
            { "Black", Black }, { "DarkBlue", DeepBlue },
            { "DarkGreen", Green }, { "DarkCyan", Cyan },
            { "DarkRed", Red },{ "DarkMagenta", Magenta },
            { "DarkYellow", Yellow }, { "Gray", LightGray },
            { "DarkGray", DeepGray },{ "Blue", Blue },
            { "Green", Green },{ "Cyan", Cyan },
            { "Red", Red }, { "Magenta", Magenta },
            { "Yellow", Yellow }, { "White", White },
        };
    }
}