using System;
using ConsoleColor = Gold.Graphics.Color;

namespace GoOS.Virtualisation.ChaOS.Core
{
    public class Core
    {
        private GoOS.Virtualisation.ChaOS.Kernel parent;

        private SVGAIITerminal Console;
        
        public Core(GoOS.Virtualisation.ChaOS.Kernel parent)
        {
            this.parent = parent;
            Console = parent.Console;
        }
        
        public void log(string text = null) => Console.WriteLine(text);
        public void clog(string text, ConsoleColor ForeColor)
        {
            var OldFore = Console.ForegroundColor;
            Console.ForegroundColor = ForeColor;
            Console.WriteLine(text);
            Console.ForegroundColor = OldFore;
        }
        public void write(string text) => Console.Write(text);
        public void cwrite(string text, Gold.Graphics.Color Color)
        {
            var OldColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            write(text);
            Console.ForegroundColor = OldColor;
        }

        public void SetScreenColor(ConsoleColor BackColor, ConsoleColor ForeColor, bool ClearScreen = true)
        {
            Console.BackgroundColor = BackColor; Console.ForegroundColor = ForeColor;
            if (ClearScreen) Console.Clear();
        }

        public void Crash(Exception exc, bool isFatal = false)
        {
            ConsoleColor OldFore = Console.ForegroundColor; ConsoleColor OldBack = Console.BackgroundColor;
            SetScreenColor(ConsoleColor.DeepBlue, ConsoleColor.White); Console.Beep();
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
