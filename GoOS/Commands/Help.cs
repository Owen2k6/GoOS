using System;
using GoOS.Themes;
using static GoOS.Core;
using Console = BetterConsole;

namespace GoOS.Commands
{
    public class Help
    {
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
                        log(ThemeManager.WindowText, "cd {file} - Enter a directory.");
                        log(ThemeManager.WindowText, "cd.. - Go to the parent directory.");
                        log(ThemeManager.WindowText, "cdr - Jump to root from anywhere.");
                        log(ThemeManager.WindowText, "dir - List all files and folders in the current directory.");
                        log(ThemeManager.WindowText, "vm {vmname} - Make a file.");
                        log(ThemeManager.WindowText, "settheme - Change the theme.");
                        break;
                    case 3:
                        log(ThemeManager.WindowText, "settings - Open the settings app.");
                        log(ThemeManager.WindowText, "notepad - Open the Notepad app.");
                        log(ThemeManager.WindowText, "clear - Clears the terminal.");
                        //log(ThemeManager.WindowText, "dir - list all files and folders in the current directory.");
                        //log(ThemeManager.WindowText, "vm - Make a file");
                        //log(ThemeManager.WindowText, "toggletheme - Make a Directory");
                        break;
                    default:
                        log(ThemeManager.ErrorText, "Invalid page number.");
                        break;
                }

                log(ThemeManager.WindowBorder, "Press Enter to continue or Q to quit.");

                ConsoleKeyInfo input = Console.ReadKey(true);
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