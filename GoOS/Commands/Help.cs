using System;
using GoOS.Themes;
using GoOS.GUI.Apps.Terminal;

namespace GoOS.Commands
{
    public class Help
    {
        public static void Main(Shell terminal)
        {
            SVGAIITerminal Console = terminal._terminal;
            
            int page = 1;
            while (true)
            {
                Console.ForegroundColor = ThemeManager.WindowBorder;
                Console.WriteLine($"Page {page}:");
                switch (page)
                {
                    case 1:
                        terminal.log(ThemeManager.WindowText, "help - Shows this exact page.");
                        terminal.log(ThemeManager.WindowText, "run {program}- Run a goexe file.");
                        terminal.log(ThemeManager.WindowText, "delfile {file} - Delete a file.");
                        terminal.log(ThemeManager.WindowText, "deldir {file} - Delete a directory.");
                        terminal.log(ThemeManager.WindowText, "mkfile {file} - Make a file.");
                        terminal.log(ThemeManager.WindowText, "mkdir {file} - Make a Directory.");
                        break;
                    case 2:
                        terminal.log(ThemeManager.WindowText, "cd {file} - Enter a directory.");
                        terminal.log(ThemeManager.WindowText, "cd.. - Go to the parent directory.");
                        terminal.log(ThemeManager.WindowText, "cdr - Jump to root from anywhere.");
                        terminal.log(ThemeManager.WindowText, "dir - List all files and folders in the current directory.");
                        terminal.log(ThemeManager.WindowText, "vm {vmname} - Launches a \"VM\".");
                        terminal.log(ThemeManager.WindowText, "settheme - Change the theme.");
                        break;
                    case 3:
                        terminal.log(ThemeManager.WindowText, "settings - Open the settings app.");
                        terminal.log(ThemeManager.WindowText, "notepad - Open the Notepad app.");
                        terminal.log(ThemeManager.WindowText, "clear - Clears the terminal.");
                        //log(ThemeManager.WindowText, "dir - list all files and folders in the current directory.");
                        //log(ThemeManager.WindowText, "vm - Make a file");
                        //log(ThemeManager.WindowText, "toggletheme - Make a Directory");
                        break;
                    default:
                        terminal.log(ThemeManager.ErrorText, "Invalid page number.");
                        break;
                }

                terminal.log(ThemeManager.WindowBorder, "Press Enter to continue or Q to quit.");

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