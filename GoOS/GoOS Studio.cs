using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GoOS
{
    class GOSStudio
    {
        public static void printGSSStartScreen()
        {
            Console.Clear();
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~            GoOS Studio (MIV But with colours)");
            Console.WriteLine("~");
            Console.WriteLine("~            ");
            Console.WriteLine("~            ");
            Console.WriteLine("~            ");
            Console.WriteLine("~            ");
            Console.WriteLine("~");
            Console.WriteLine("~                     type :help<Enter>          for information");
            Console.WriteLine("~                     type :q<Enter>             to exit");
            Console.WriteLine("~                     type :wq<Enter>            save to file and exit");
            Console.WriteLine("~                     press i                    to write");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.WriteLine("~");
            Console.Write("~");
        }

        public static String stringCopy(String value)
        {
            String newString = String.Empty;

            for (int i = 0; i < value.Length - 1; i++)
            {
                newString += value[i];
            }

            return newString;
        }

        public static void printGSSScreen(char[] chars, int pos, String infoBar, Boolean editMode)
        {
            int countNewLine = 0;
            int countChars = 0;
            delay(10000000);
            Console.Clear();

            bool startOfLine = true;
            for (int i = 0; i < pos; i++)
            {
                if (startOfLine == true && chars[i] == '#')
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 'p' && chars[i + 1] == 'r' && chars[i + 2] == 'i' && chars[i + 3] == 'n' && chars[i + 4] == 't' && chars[i + 5] == '=')
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 's' && chars[i + 1] == 'l' && chars[i + 2] == 'e' && chars[i + 3] == 'e' && chars[i + 4] == 'p' && chars[i + 5] == '=')
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 'i' && chars[i + 1] == 'n' && chars[i + 2] == 'p' && chars[i + 3] == 'u' && chars[i + 4] == 't' && chars[i + 5] == '=')
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 's' && chars[i + 1] == 't' && chars[i + 2] == 'o' && chars[i + 3] == 'p' && chars[i + 4] == '=')
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 'i' && chars[i + 1] == 'f' && chars[i + 2] == '=')
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 'v' && chars[i + 1] == 'a' && chars[i + 2] == 'r' && chars[i + 3] == 'i' && chars[i + 4] == 'a' && chars[i + 5] == 'b' && chars[i + 6] == 'l' && chars[i + 7] == 'e' && chars[i + 8] == '=')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 'r' && chars[i + 1] == 'a' && chars[i + 2] == 'n' && chars[i + 3] == 'd' && chars[i + 4] == 'o' && chars[i + 5] == 'm' && chars[i + 6] == 'n' && chars[i + 7] == 'u' && chars[i + 8] == 'm' && chars[i + 9] == '=')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    startOfLine = false;
                }
                if (startOfLine == true && chars[i] == 'c' && chars[i + 1] == 'l' && chars[i + 2] == 'e' && chars[i + 3] == 'a' && chars[i + 4] == 'r')
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    startOfLine = false;
                }
                if (chars[i] == '\n')
                {
                    Console.WriteLine("");
                    countNewLine++;
                    countChars = 0;
                    Console.ForegroundColor = ConsoleColor.White;
                    startOfLine = true;
                }
                else
                {
                    Console.Write(chars[i]);
                    countChars++;
                    if (countChars % 80 == 79)
                    {
                        countNewLine++;
                    }
                    

                }
            }

            Console.Write("/");

            for (int i = 0; i < 23 - countNewLine; i++)
            {
                Console.WriteLine("");
                Console.Write("~");
            }

            //PRINT INSTRUCTION
            Console.WriteLine();
            for (int i = 0; i < 72; i++)
            {
                if (i < infoBar.Length)
                {
                    Console.Write(infoBar[i]);
                }
                else
                {
                    Console.Write(" ");
                }
            }

            if (editMode)
            {
                Console.Write(countNewLine + 1 + "," + countChars);
            }

        }

        public static String GSS(String start)
        {
            Boolean editMode = false;
            int pos = 0;
            char[] chars = new char[2000];
            String infoBar = String.Empty;

            if (start == null)
            {
                printGSSStartScreen();
            }
            else
            {
                pos = start.Length;

                for (int i = 0; i < start.Length; i++)
                {
                    chars[i] = start[i];
                }
                printGSSScreen(chars, pos, infoBar, editMode);
            }

            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);

                if (isForbiddenKey(keyInfo.Key)) continue;

                else if (!editMode && keyInfo.KeyChar == ':')
                {
                    infoBar = ":";
                    printGSSScreen(chars, pos, infoBar, editMode);
                    do
                    {
                        keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            if (infoBar == ":wq")
                            {
                                String returnString = String.Empty;
                                for (int i = 0; i < pos; i++)
                                {
                                    returnString += chars[i];
                                }
                                return returnString;
                            }
                            else if (infoBar == ":q")
                            {
                                return null;

                            }
                            else if (infoBar == ":help")
                            {
                                printGSSStartScreen();
                                break;
                            }
                            else
                            {
                                infoBar = "ERROR: No such command";
                                printGSSScreen(chars, pos, infoBar, editMode);
                                break;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace)
                        {
                            infoBar = stringCopy(infoBar);
                            printGSSScreen(chars, pos, infoBar, editMode);
                        }
                        else if (keyInfo.KeyChar == 'q')
                        {
                            infoBar += "q";
                        }
                        else if (keyInfo.KeyChar == ':')
                        {
                            infoBar += ":";
                        }
                        else if (keyInfo.KeyChar == 'w')
                        {
                            infoBar += "w";
                        }
                        else if (keyInfo.KeyChar == 'h')
                        {
                            infoBar += "h";
                        }
                        else if (keyInfo.KeyChar == 'e')
                        {
                            infoBar += "e";
                        }
                        else if (keyInfo.KeyChar == 'l')
                        {
                            infoBar += "l";
                        }
                        else if (keyInfo.KeyChar == 'p')
                        {
                            infoBar += "p";
                        }
                        else
                        {
                            continue;
                        }
                        printGSSScreen(chars, pos, infoBar, editMode);



                    } while (keyInfo.Key != ConsoleKey.Escape);
                }

                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    editMode = false;
                    infoBar = String.Empty;
                    printGSSScreen(chars, pos, infoBar, editMode);
                    continue;
                }

                else if (keyInfo.Key == ConsoleKey.I && !editMode)
                {
                    editMode = true;
                    infoBar = "-- INSERT --";
                    printGSSScreen(chars, pos, infoBar, editMode);
                    continue;
                }

                else if (keyInfo.Key == ConsoleKey.Enter && editMode && pos >= 0)
                {
                    chars[pos++] = '\n';
                    printGSSScreen(chars, pos, infoBar, editMode);
                    continue;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && editMode && pos >= 0)
                {
                    if (pos > 0) pos--;

                    chars[pos] = '\0';

                    printGSSScreen(chars, pos, infoBar, editMode);
                    continue;
                }

                if (editMode && pos >= 0)
                {
                    chars[pos++] = keyInfo.KeyChar;
                    printGSSScreen(chars, pos, infoBar, editMode);
                }

            } while (true);
        }

        public static bool isForbiddenKey(ConsoleKey key)
        {
            ConsoleKey[] forbiddenKeys = { ConsoleKey.Print, ConsoleKey.PrintScreen, ConsoleKey.Pause, ConsoleKey.Home, ConsoleKey.PageUp, ConsoleKey.PageDown, ConsoleKey.End, ConsoleKey.NumPad0, ConsoleKey.NumPad1, ConsoleKey.NumPad2, ConsoleKey.NumPad3, ConsoleKey.NumPad4, ConsoleKey.NumPad5, ConsoleKey.NumPad6, ConsoleKey.NumPad7, ConsoleKey.NumPad8, ConsoleKey.NumPad9, ConsoleKey.Insert, ConsoleKey.F1, ConsoleKey.F2, ConsoleKey.F3, ConsoleKey.F4, ConsoleKey.F5, ConsoleKey.F6, ConsoleKey.F7, ConsoleKey.F8, ConsoleKey.F9, ConsoleKey.F10, ConsoleKey.F11, ConsoleKey.F12, ConsoleKey.Add, ConsoleKey.Divide, ConsoleKey.Multiply, ConsoleKey.Subtract, ConsoleKey.LeftWindows, ConsoleKey.RightWindows };
            for (int i = 0; i < forbiddenKeys.Length; i++)
            {
                if (key == forbiddenKeys[i]) return true;
            }
            return false;
        }

        public static void delay(int time)
        {
            for (int i = 0; i < time; i++) ;
        }
        public static void StartGSS()
        {
            Console.WriteLine("Enter file's filename to open:");
            Console.WriteLine("If the specified file does not exist, it will be created.");
            Kernel.file = Console.ReadLine();
            try
            {
                if (File.Exists(@"0:\" + Kernel.file))
                {
                    Console.WriteLine("Found file!");
                }
                else if (!File.Exists(@"0:\" + Kernel.file))
                {
                    Console.WriteLine("Creating file!");
                    File.Create(@"0:\" + Kernel.file);
                }
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            String text = String.Empty;
            Console.WriteLine("Do you want to open " + Kernel.file + " content? (Yes/No)");
            if (Console.ReadLine().ToLower() == "yes" || Console.ReadLine().ToLower() == "y")
            {
                text = GSS(File.ReadAllText(@"0:\" + Kernel.file));
            }
            else
            {
                text = GSS(null);
            }

            Console.Clear();

            if (text != null)
            {
                File.WriteAllText(@"0:\" + Kernel.file, text);
                Console.WriteLine("Content has been saved to " + Kernel.file);
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
