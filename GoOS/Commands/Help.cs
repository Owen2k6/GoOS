using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoOS.Themes;

namespace GoOS.Commands
{
    public class Help
    {
        //GoOS Core
        public static void print(string str)
        {
            Console.WriteLine(str);
        }
        public static void log(System.ConsoleColor colour, string str)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(str);
        }
        public static void write(string str)
        {
            Console.Write(str);
        }
        public static void textcolour(System.ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
        }
        public static void highlightcolour(System.ConsoleColor colour)
        {
            Console.BackgroundColor = colour;
        }
        public static void sleep(int time)
        {
            Thread.Sleep(time);
        }

        public static void Main()
        {
            int page = 1;
            while (true)
            {
                textcolour(ThemeManager.WindowBorder);
                Console.WriteLine($"Page {page}:");
                switch (page)
                {
                    case 1:
                        log(ThemeManager.WindowText, "HELP - Shows this exact page.");
                        log(ThemeManager.WindowText, "RUN - Run a goexe file."); // Useless comment, ignore me or your family goes away.
                        log(ThemeManager.WindowText, "delfile - Delete a file");
                        log(ThemeManager.WindowText, "deldir - Delete a directory");
                        log(ThemeManager.WindowText, "mkfile - Make a file");
                        log(ThemeManager.WindowText, "mkdir - Make a Directory");
                        break;
                    default:
                        log(ThemeManager.ErrorText, "Invalid page number.");
                        break;
                }

                Console.WriteLine("Press Enter to continue or Q to quit.");

                ConsoleKeyInfo input = Console.ReadKey();
                if (input.KeyChar == 'Q' || input.KeyChar == 'q')
                {
                    break;
                }

                page++;
                if (page > 1)
                {
                    page = 1;
                }
            }

        }
    }
}
