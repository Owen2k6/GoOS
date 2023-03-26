using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoOS.Themes;

namespace GoOS.Commands
{
    public class Run
    {
        static Dictionary<string, string> Strings = new Dictionary<string, string>() { };

        static Dictionary<string, int> Integers = new Dictionary<string, int>() { };

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

        public static void Main(string run)
        {
            String inputaman = run;


            try
            {
                //log(ConsoleColor.Blue, "GoOS Admin: Attempting to run " + inputaman);
                if (!inputaman.EndsWith(".gexe") && !inputaman.EndsWith(".goexe"))
                {
                    log(ThemeManager.ErrorText, "Incompatible format.");
                    log(ThemeManager.ErrorText, "File must be .gexe");
                }

                if (inputaman.EndsWith(".goexe") || inputaman.EndsWith(".gexe"))
                {
                    string fuckingprogramname = null;

                    log(ConsoleColor.Yellow, "Application.Start");
                    var content = File.ReadAllLines(@"0:\" + inputaman);
                    string theysaid = null;
                    ConsoleKey keypressed = ConsoleKey.O;
                    int count = 1;
                    String endmessage = "Process has ended.";
                    Boolean hasbeenregistered = false;
                    foreach (string line in content)
                    {
                        count = count + 1;
                        //log(ConsoleColor.Magenta, "LINE FOUND: CONTENT: " + line);
                        if (line.StartsWith("#"))
                        {
                        }

                        if (line.StartsWith(""))
                        {
                        }

                        if (line.StartsWith("sleep"))
                        {
                            String howlong = line.Split("=")[1];
                            int potato = Convert.ToInt32(howlong);
                            sleep(potato);
                        }

                        if (line.StartsWith("input"))
                        {
                            if (line == "input=")
                            {
                                textcolour(ConsoleColor.Blue);
                                theysaid = Console.ReadLine();
                            }
                            else
                            {
                                String addon = line.Replace("input=", "");
                                write(addon);
                                textcolour(ConsoleColor.Blue);
                                theysaid = Console.ReadLine();
                            }
                        }

                        if (line.StartsWith("stop"))
                        {
                            if (line == "stop=")
                            {
                                textcolour(ConsoleColor.Blue);
                                log(ConsoleColor.Green, "Press any key to continue...");
                                Console.ReadKey();
                                Console.WriteLine();
                            }
                            else
                            {
                                String addon = line.Replace("stop=", "");
                                textcolour(ConsoleColor.DarkRed);
                                write(addon);
                                textcolour(ConsoleColor.Blue);
                                Console.ReadKey();
                                Console.WriteLine();
                            }
                        }

                        if (line.StartsWith("endmsg"))
                        {
                            endmessage = line.Replace("endmsg=", "");
                        }

                        if (line.StartsWith("regprog"))
                        {
                            if (hasbeenregistered)
                            {
                                log(ThemeManager.ErrorText,
                                    "Attempted second register. Application may be attempting to reregister as another application!!!");
                                break;
                            }

                            fuckingprogramname = line.Replace("regprog=", "");
                            hasbeenregistered = true;
                            if (!Directory.Exists(@"0:\content\prf\"))
                            {
                                Directory.CreateDirectory(@"0:\content\prf\");
                                if (!Directory.Exists(@"0:\content\prf\" + fuckingprogramname + @"\"))
                                {
                                    Directory.CreateDirectory(@"0:\content\prf\" + fuckingprogramname + @"\");
                                }
                            }
                            else if (!Directory.Exists(@"0:\content\prf\" + fuckingprogramname + @"\"))
                            {
                                Directory.CreateDirectory(@"0:\content\prf\" + fuckingprogramname + @"\");
                            }
                        }

                        if (line.StartsWith("string"))
                        {
                            string whythehellnotwork = line.Replace(@"string ", "");
                            string varName = whythehellnotwork.Split(@" = ")[0];
                            string varContents = whythehellnotwork.Split(@" = ")[1];

                            if (Strings.ContainsKey(varName))
                            {
                                Strings.Remove(varName);
                            }


                            Strings.Add(varName, varContents);
                        }

                        if (line.StartsWith("int"))
                        {
                            int intCont = 0;
                            string whythehellnotwork = line.Replace(@"int ", "");
                            string varName = whythehellnotwork.Split(@" = ")[0];
                            string varContents = whythehellnotwork.Split(@" = ")[1];
                            try
                            {
                                intCont = int.Parse(varContents);
                            }
                            catch
                            {
                                Console.WriteLine(@"Integer: int " + varName + " is formatted incorrectly.");
                            }

                            string trueCont = intCont.ToString();

                            if (Integers.ContainsKey(varName))
                            {
                                Integers.Remove(varName);
                            }


                            Strings.Add(varName, trueCont);
                        }

                        if (line.StartsWith(@"print="))
                        {
                            string assSplitter = line.Replace(@"print=", "");
                            // we like splitting ass round here
                            if (assSplitter.Contains("\""))
                            {
                                string thighs = assSplitter.Replace("\"", "");
                                Console.WriteLine(thighs);
                            }
                            else if (Strings.TryGetValue(assSplitter, out string what))
                            {
                                try
                                {
                                    Console.WriteLine(what);
                                }
                                catch
                                {
                                    Console.WriteLine("owen is gay");
                                }
                            }
                            else if (Integers.TryGetValue(assSplitter, out int whatint))
                            {
                                try
                                {
                                    Console.WriteLine(what);
                                }
                                catch
                                {
                                    Console.WriteLine("owen is gay");
                                }
                            }
                        }

                        if (line.StartsWith("if"))
                        {
                            string removeIf = line.Substring(3);
                            if (removeIf.Contains("=="))
                            {
                                string equals2split1 = removeIf.Split(@" == ")[0];
                                string equals2split2 = removeIf.Split(@" == ")[1];
                                string intorstring = "unsure";
                                string string1 = null;
                                string string2 = null;
                                int int1 = 0;
                                int int2 = 0;
                                // this line was used for debugging // log(ConsoleColor.White, equals2split1 + "" + equals2split2);
                                    
                                if (Strings.TryGetValue(equals2split1, out string strval))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "string";
                                    }
                                    if (intorstring == "int")
                                    {
                                        
                                    }
                                    else if (intorstring == "string")
                                    {
                                        string1 = strval;
                                    }
                                }
                                else if (Integers.TryGetValue(equals2split2, out int intval))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "int";
                                    }
                                    if (intorstring == "string")
                                    {
                                        
                                    }
                                    else if (intorstring == "int")
                                    {
                                        int1 = intval;
                                    }
                                }
                                if (Strings.TryGetValue(equals2split2, out string strval2))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "string";
                                    }
                                    if (intorstring == "int")
                                    {
                                        
                                    }
                                    else if (intorstring == "string")
                                    {
                                        string2 = strval2;
                                    }
                                }
                                else if (Integers.TryGetValue(equals2split2, out int intval2))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "int";
                                    }
                                    if (intorstring == "string")
                                    {
                                        
                                    }
                                    else if (intorstring == "int")
                                    {
                                        int2 = intval2;
                                    }
                                }
                                
                                if (intorstring == "string")
                                {
                                    if (string1 != string2)
                                    {
                                        break;
                                    }
                                }
                                else if (intorstring == "int")
                                {
                                    if (int1 != int2)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (removeIf.Contains("!="))
                            {
                                string equals2split1 = removeIf.Split(@" != ")[0];
                                string equals2split2 = removeIf.Split(@" != ")[1];
                                string intorstring = "unsure";
                                string string1 = null;
                                string string2 = null;
                                int int1 = 0;
                                int int2 = 0;
                                // this line was used for debugging //    log(ConsoleColor.White, equals2split1 + "" + equals2split2);

                                if (Strings.TryGetValue(equals2split1, out string strval))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "string";
                                    }
                                    if (intorstring == "int")
                                    {
                                        
                                    }
                                    else if (intorstring == "string")
                                    {
                                        string1 = strval;
                                    }
                                }
                                else if (Integers.TryGetValue(equals2split2, out int intval))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "int";
                                    }
                                    if (intorstring == "string")
                                    {
                                        
                                    }
                                    else if (intorstring == "int")
                                    {
                                        int1 = intval;
                                    }
                                }
                                if (Strings.TryGetValue(equals2split2, out string strval2))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "string";
                                    }
                                    if (intorstring == "int")
                                    {
                                        
                                    }
                                    else if (intorstring == "string")
                                    {
                                        string2 = strval2;
                                    }
                                }
                                else if (Integers.TryGetValue(equals2split2, out int intval2))
                                {
                                    if (intorstring == "unsure")
                                    {
                                        intorstring = "int";
                                    }
                                    if (intorstring == "string")
                                    {
                                        
                                    }
                                    else if (intorstring == "int")
                                    {
                                        int2 = intval2;
                                    }
                                }
                                
                                if (intorstring == "string")
                                {
                                    if (string1 == string2)
                                    {
                                        break;
                                    }
                                }
                                else if (intorstring == "int")
                                {
                                    if (int1 == int2)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (line.StartsWith(@"save="))
                        {
                            string whatvartosave = line.Substring(5);
                            if (Strings.TryGetValue(whatvartosave, out string strval))
                            {
                                if (Directory.Exists(@"0:\content\prf\"))
                                {
                                    if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                     @".txt"))
                                    {
                                        File.Create(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                    @".txt");
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Write(@"type=string");
                                        tw.Close();
                                    }
                                    else if (File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                                         whatvartosave + @".txt"))
                                    {
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Write(@"type=string");
                                        tw.Close();
                                    }
                                }
                                else if (!Directory.Exists(@"0:\content\prf\"))
                                {
                                    Directory.CreateDirectory(@"0:\content\prf\");
                                    if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                     @".txt"))
                                    {
                                        File.Create(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                    @".txt");
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Write(@"type=string");
                                        tw.Close();
                                    }
                                    else if (File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                                         whatvartosave + @".txt"))
                                    {
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Write(@"type=string");
                                        tw.Close();
                                    }
                                }
                            }
                            else if (Integers.TryGetValue(whatvartosave, out int intval))
                            {
                                if (Directory.Exists(@"0:\content\prf\"))
                                {
                                    if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                     @".txt"))
                                    {
                                        File.Create(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                    @".txt");
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(intval);
                                        tw.Write(@"type=int");
                                        tw.Close();
                                    }
                                    else if (File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                                         whatvartosave + @".txt"))
                                    {
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(intval);
                                        tw.Write(@"type=int");
                                        tw.Close();
                                    }
                                }
                                else if (!Directory.Exists(@"0:\content\prf\"))
                                {
                                    Directory.CreateDirectory(@"0:\content\prf\");
                                    if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                     @".txt"))
                                    {
                                        File.Create(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave +
                                                    @".txt");
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(intval);
                                        tw.Write(@"type=int");
                                        tw.Close();
                                    }
                                    else if (File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                                         whatvartosave + @".txt"))
                                    {
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname +
                                                                         @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(intval);
                                        tw.Write(@"type=int");
                                        tw.Close();
                                    }
                                }
                            }
                        }

                        if (line.StartsWith(@"load="))
                        {
                            int intCont = 0;
                            string whatvartoload = line.Substring(5);
                            string ass = null;
                            string assType = null;
                            if (Strings.TryGetValue(whatvartoload, out string strval))
                            {
                                if (File.Exists(
                                        @"0:\content\prf\" + fuckingprogramname + @"\" + whatvartoload + @".txt"))
                                {
                                    using (StreamReader streamReader = new StreamReader(
                                               @"0:\content\prf\" + fuckingprogramname + @"\" + whatvartoload + @".txt",
                                               Encoding.UTF8))
                                    {
                                        ass = File.ReadLines(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                                             whatvartoload + @".txt").Skip(0).Take(1).First();
                                        assType = File
                                            .ReadLines(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartoload +
                                                       @".txt").Skip(1).Take(1).First();
                                        string assSplitter2 = assType.Split('=')[1];
                                        if (assSplitter2 == "string")
                                        {
                                            if (Strings.ContainsKey(whatvartoload))
                                            {
                                                Strings.Remove(whatvartoload);
                                            }

                                            Strings.Add(whatvartoload, ass);
                                        }

                                        if (assSplitter2 == "int")
                                        {
                                            try
                                            {
                                                intCont = int.Parse(ass);
                                            }
                                            catch
                                            {
                                                Console.WriteLine(@"Load: int " + whatvartoload +
                                                                  " is formatted incorrectly.");
                                            }

                                            string trueCont = intCont.ToString();

                                            if (Integers.ContainsKey(whatvartoload))
                                            {
                                                Integers.Remove(whatvartoload);
                                            }

                                            Integers.Add(whatvartoload, intCont);
                                        }
                                    }
                                }
                            }
                        }

                        if (line.StartsWith("frontcolor="))
                        {
                            string ass = line.Substring(11);
                            if (ass == "white")
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (ass == "blue")
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            else if (ass == "green")
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            else if (ass == "yellow")
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            else if (ass == "black")
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else if (ass == "cyan")
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                            }
                            else if (ass == "gray")
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                            else if (ass == "magenta")
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                            }
                            else if (ass == "red")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else if (ass == "darkblue")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                            }
                            else if (ass == "darkcyan")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                            }
                            else if (ass == "darkgray")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            }
                            else if (ass == "darkgreen")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                            }
                            else if (ass == "darkmageneta")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            }
                            else if (ass == "darkred")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                            }
                            else if (ass == "darkyellow")
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                            }
                        }

                        if (line.StartsWith("backcolor="))
                        {
                            string ass = line.Substring(10);
                            if (ass == "white")
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                            }
                            else if (ass == "blue")
                            {
                                Console.BackgroundColor = ConsoleColor.Blue;
                            }
                            else if (ass == "green")
                            {
                                Console.BackgroundColor = ConsoleColor.Green;
                            }
                            else if (ass == "yellow")
                            {
                                Console.BackgroundColor = ConsoleColor.Yellow;
                            }
                            else if (ass == "black")
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                            }
                            else if (ass == "cyan")
                            {
                                Console.BackgroundColor = ConsoleColor.Cyan;
                            }
                            else if (ass == "gray")
                            {
                                Console.BackgroundColor = ConsoleColor.Gray;
                            }
                            else if (ass == "magenta")
                            {
                                Console.BackgroundColor = ConsoleColor.Magenta;
                            }
                            else if (ass == "red")
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                            }
                            else if (ass == "darkblue")
                            {
                                Console.BackgroundColor = ConsoleColor.DarkBlue;
                            }
                            else if (ass == "darkcyan")
                            {
                                Console.BackgroundColor = ConsoleColor.DarkCyan;
                            }
                            else if (ass == "darkgray")
                            {
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                            }
                            else if (ass == "darkgreen")
                            {
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                            }
                            else if (ass == "darkmageneta")
                            {
                                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                            }
                            else if (ass == "darkred")
                            {
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                            }
                            else if (ass == "darkyellow")
                            {
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                            }
                        }
                    }

                    if (endmessage != null)
                    {
                        endmessage = "Process has ended.";
                    }

                    log(ThemeManager.ErrorText, endmessage);
                }
            }
            catch (Exception e)
            {
                log(ThemeManager.ErrorText, e.Message);
            }
        }
    }
}