using System;
using Console = BetterConsole;
using ConsoleColor = GoGL.Graphics.Color;
using static ConsoleColorEx;

namespace ChaOS
{
    public class Core
    {
        public static void log(string text = null) => Console.WriteLine(text);
        public static void clog(string text, ConsoleColor ForeColor)
        {
            var OldFore = Console.ForegroundColor;
            Console.ForegroundColor = ForeColor;
            Console.WriteLine(text);
            Console.ForegroundColor = OldFore;
        }
        public static void write(string text) => Console.Write(text);
        public static void cwrite(string text, ConsoleColor Color)
        {
            var OldColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            write(text);
            Console.ForegroundColor = OldColor;
        }

        public static void SetScreenColor(ConsoleColor BackColor, ConsoleColor ForeColor, bool ClearScreen = true)
        {
            Console.BackgroundColor = BackColor; Console.ForegroundColor = ForeColor;
            if (ClearScreen) Console.Clear();
        }

        public static void Crash(Exception exc, bool isFatal = false)
        {
            ConsoleColor OldFore = Console.ForegroundColor; ConsoleColor OldBack = Console.BackgroundColor;
            SetScreenColor(DarkBlue, White); Console.Beep();
            Console.CursorTop = 10; log("              ChaOS has hit a brick wall and died in the wreckage!\n");
            try { Console.CursorLeft = 39 - (exc.Message.Length / 2); } catch { Console.CursorLeft = 0; }
            write(exc.ToString() + "\n\n");

            if (!isFatal)
            {
                write("                          Press any key to continue... ");
                Console.ReadKey(true); SetScreenColor(OldBack, OldFore);
            }
            else
            {
                write("                                You can restart. ");
                while (true) Console.ReadKey(true);
            }
        }
    }
}
