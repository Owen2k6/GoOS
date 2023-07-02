using System.Threading;
using Console = BetterConsole;
using ConsoleColor = PrismAPI.Graphics.Color;

namespace GoOS
{
    public static class Core
    {
        public static void print(string str)
        {
            Console.WriteLine(str);
        }

        public static void log(ConsoleColor colour, string str)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(str);
        }

        public static void write(string str)
        {
            Console.Write(str);
        }

        public static void textcolour(ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
        }

        public static void highlightcolour(ConsoleColor colour)
        {
            Console.BackgroundColor = colour;
        }

        public static void sleep(int time)
        {
            Thread.Sleep(time);
        }
    }
}
