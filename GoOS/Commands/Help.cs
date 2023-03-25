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
                        log(ThemeManager.WindowText, "help - Shows this exact page.");
                        log(ThemeManager.WindowText, "run {program}- Run a goexe file.");
                        log(ThemeManager.WindowText, "delfile {file} - Delete a file.");
                        log(ThemeManager.WindowText, "deldir {file} - Delete a directory.");
                        log(ThemeManager.WindowText, "mkfile {file} - Make a file.");
                        log(ThemeManager.WindowText, "mkdir {file} - Make a Directory.");
                        break;
                    case 2:
                        log(ThemeManager.WindowText, "cd {file} - enter a directory.");
                        log(ThemeManager.WindowText, "cd.. - go to parent directory.");
                        log(ThemeManager.WindowText, "cdr - Jump to root from anywhere.");
                        log(ThemeManager.WindowText, "dir - list all files and folders in the current directory.");
                        log(ThemeManager.WindowText, "vm {vmname}- Make a file.");
                        log(ThemeManager.WindowText, "toggletheme - Make a Directory.");
                        break;
                    case 3:
                        log(ThemeManager.WindowText, "settings - open the settings app.");
                        log(ThemeManager.WindowText, "notepad - open the notepad app.");
                        log(ThemeManager.WindowText, "clear - clears the terminal.");
                        //log(ThemeManager.WindowText, "dir - list all files and folders in the current directory.");
                        //log(ThemeManager.WindowText, "vm - Make a file");
                        //log(ThemeManager.WindowText, "toggletheme - Make a Directory");
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
                if (page > 3)
                {
                    page = 1;
                }
            }
        }
    }
}