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
                if (inputaman.EndsWith(".goexe"))
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
                                log(ThemeManager.ErrorText, "Attempted second register. Application may be attempting to reregister as another application!!!");
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
                        }
                        if (line.StartsWith("if"))
                        {
                            string removeIf = line.Substring(3);
                            if (removeIf.Contains("=="))
                            {
                                string equals2split1 = removeIf.Split(@"==")[0];
                                string equals2split2 = removeIf.Split(@"==")[1];

                                if (equals2split1 == equals2split2)
                                {

                                }
                            }
                            if (removeIf.Contains("!="))
                            {
                                string equals2split1 = removeIf.Split(@"!=")[0];
                                string equals2split2 = removeIf.Split(@"!=")[1];

                                if (equals2split1 != equals2split2)
                                {

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
                                    if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt"))
                                    {
                                        File.Create(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt");
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Close();
                                    }
                                    else if (File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt"))
                                    {
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Close();
                                    }
                                }
                                else if (!Directory.Exists(@"0:\content\prf\"))
                                {
                                    Directory.CreateDirectory(@"0:\content\prf\");
                                    if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt"))
                                    {
                                        File.Create(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt");
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Close();
                                    }
                                    else if (File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt"))
                                    {
                                        TextWriter tw = new StreamWriter(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartosave + @".txt");
                                        tw.WriteLine(strval);
                                        tw.Close();
                                    }
                                }
                            }
                        }
                        if (line.StartsWith(@"load="))
                        {
                            string whatvartoload = line.Substring(5);
                            string ass = null;
                            if (Strings.TryGetValue(whatvartoload, out string strval))
                            {
                                if (File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartoload + @".txt"))
                                {
                                    using (StreamReader streamReader = new StreamReader(@"0:\content\prf\" + fuckingprogramname + @"\" + whatvartoload + @".txt", Encoding.UTF8))
                                    {
                                        ass = streamReader.ReadToEnd();

                                        if (Strings.ContainsKey(whatvartoload))
                                        {
                                            Strings.Remove(whatvartoload);
                                        }

                                        Strings.Add(whatvartoload, ass);
                                    }
                                }
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

