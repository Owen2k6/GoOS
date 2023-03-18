using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public static void main()
        {
            int page = 1;
            while (true)
            {
                
                Console.WriteLine($"Page {page}:");
                switch (page)
                {
                    case 1:
                        log(ConsoleColor.White, "HELP - Shows this exact page.");
                        log(ConsoleColor.White, "RUN - Run a goexe file.");
                        log(ConsoleColor.White, "delfile - Delete a file");
                        log(ConsoleColor.White, "deldir - Delete a directory");
                        log(ConsoleColor.White, "mkfile - Make a file");
                        log(ConsoleColor.White, "mkdir - Make a Directory");
                        break;
                    default:
                        log(ConsoleColor.White, "Invalid page number.");
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
