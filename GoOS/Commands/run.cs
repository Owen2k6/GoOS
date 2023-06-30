using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GoOS.Themes;
using Console = BetterConsole;
using static ConsoleColorEx;
using static GoOS.Core;

namespace GoOS.Commands
{
    public class Run
    {
        static Dictionary<string, string> Strings = new Dictionary<string, string>() { };

        static Dictionary<string, int> Integers = new Dictionary<string, int>() { };

        public static void Main(string run)
        {
            String inputaman = run;


            try
            {
                //log(Blue, "GoOS Admin: Attempting to run " + inputaman);
                if (!inputaman.EndsWith(".gexe") && !inputaman.EndsWith(".goexe"))
                {
                    log(ThemeManager.ErrorText, "Incompatible format.");
                    log(ThemeManager.ErrorText, "File must be .gexe");
                }

                if (inputaman.EndsWith(".goexe") || inputaman.EndsWith(".gexe"))
                {
                    string fuckingprogramname = null;
                    bool isif = false;
                    bool badif = false;

                    log(Yellow, "Application.Start");
                    var content = File.ReadAllLines(@"0:\" + inputaman);
                    string theysaid = null;
                    ConsoleKey keypressed = ConsoleKey.O;
                    int count = 1;
                    String endmessage = "Process has ended.";
                    Boolean hasbeenregistered = false;
                    foreach (string line in content)
                    {
                        count = count + 1;
                        //log(Magenta, "LINE FOUND: CONTENT: " + line);
                        if (line.StartsWith("#"))
                        {
                        }

                        if (line.StartsWith(""))
                        {
                        }

                        if (line.StartsWith("sleep") && badif == false)
                        {
                            String howlong = line.Split("=")[1];
                            int potato = Convert.ToInt32(howlong);
                            sleep(potato);
                        }

                        if (line.StartsWith("input") && badif == false)
                        {
                            if (line == "input=")
                            {
                                textcolour(Blue);
                                theysaid = Console.ReadLine();
                            }
                            else
                            {
                                String addon = line.Replace("input=", "");
                                write(addon);
                                textcolour(Blue);
                                theysaid = Console.ReadLine();
                            }
                        }

                        if (line.StartsWith("stop") && badif == false)
                        {
                            if (line == "stop=")
                            {
                                textcolour(Blue);
                                log(Green, "Press any key to continue...");
                                Console.ReadKey();
                                Console.WriteLine();
                            }
                            else
                            {
                                String addon = line.Replace("stop=", "");
                                textcolour(DarkRed);
                                write(addon);
                                textcolour(Blue);
                                Console.ReadKey();
                                Console.WriteLine();
                            }
                        }

                        if (line.StartsWith("endmsg") && badif == false)
                        {
                            endmessage = line.Replace("endmsg=", "");
                        }

                        if (line.StartsWith("regprog") && badif == false)
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

                        if (line.StartsWith("string") && badif == false)
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

                        if (line.StartsWith("int") && badif == false)
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

                        if (line.StartsWith(@"print=") && badif == false)
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

                        if (line.StartsWith("if") && badif == false)
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
                                // this line was used for debugging // log(White, equals2split1 + "" + equals2split2);
                                    
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
                                        badif = true;
                                    }
                                }
                                else if (intorstring == "int")
                                {
                                    if (int1 != int2)
                                    {
                                        badif = true;
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
                                // this line was used for debugging //    log(White, equals2split1 + "" + equals2split2);

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
                                        badif = true;
                                    }
                                }
                                else if (intorstring == "int")
                                {
                                    if (int1 == int2)
                                    {
                                        badif = true;
                                    }
                                }
                            }
                        }
                        
                        if (line.StartsWith("break") && badif == true && isif == true)
                        {
                            badif = false;
                            isif = false;
                            continue;
                        }

                        if (line.StartsWith(@"save=") && badif == false)
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

                        if (line.StartsWith(@"load=") && badif == false)
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

                        if (line.StartsWith("frontcolor=") && badif == false)
                        {
                            string ass = line.Substring(11);
                            if (ass == "white")
                            {
                                Console.ForegroundColor = White;
                            }
                            else if (ass == "blue")
                            {
                                Console.ForegroundColor = Blue;
                            }
                            else if (ass == "green")
                            {
                                Console.ForegroundColor = Green;
                            }
                            else if (ass == "yellow")
                            {
                                Console.ForegroundColor = Yellow;
                            }
                            else if (ass == "black")
                            {
                                Console.ForegroundColor = Black;
                            }
                            else if (ass == "cyan")
                            {
                                Console.ForegroundColor = Cyan;
                            }
                            else if (ass == "gray")
                            {
                                Console.ForegroundColor = Gray;
                            }
                            else if (ass == "magenta")
                            {
                                Console.ForegroundColor = Magenta;
                            }
                            else if (ass == "red")
                            {
                                Console.ForegroundColor = Red;
                            }
                            else if (ass == "darkblue")
                            {
                                Console.ForegroundColor = DarkBlue;
                            }
                            else if (ass == "darkcyan")
                            {
                                Console.ForegroundColor = DarkCyan;
                            }
                            else if (ass == "darkgray")
                            {
                                Console.ForegroundColor = DarkGray;
                            }
                            else if (ass == "darkgreen")
                            {
                                Console.ForegroundColor = DarkGreen;
                            }
                            else if (ass == "darkmageneta")
                            {
                                Console.ForegroundColor = DarkMagenta;
                            }
                            else if (ass == "darkred")
                            {
                                Console.ForegroundColor = DarkRed;
                            }
                            else if (ass == "darkyellow")
                            {
                                Console.ForegroundColor = DarkYellow;
                            }
                        }

                        if (line.StartsWith("backcolor=") && badif == false)
                        {
                            string ass = line.Substring(10);
                            if (ass == "white")
                            {
                                Console.BackgroundColor = White;
                            }
                            else if (ass == "blue")
                            {
                                Console.BackgroundColor = Blue;
                            }
                            else if (ass == "green")
                            {
                                Console.BackgroundColor = Green;
                            }
                            else if (ass == "yellow")
                            {
                                Console.BackgroundColor = Yellow;
                            }
                            else if (ass == "black")
                            {
                                Console.BackgroundColor = Black;
                            }
                            else if (ass == "cyan")
                            {
                                Console.BackgroundColor = Cyan;
                            }
                            else if (ass == "gray")
                            {
                                Console.BackgroundColor = Gray;
                            }
                            else if (ass == "magenta")
                            {
                                Console.BackgroundColor = Magenta;
                            }
                            else if (ass == "red")
                            {
                                Console.BackgroundColor = Red;
                            }
                            else if (ass == "darkblue")
                            {
                                Console.BackgroundColor = DarkBlue;
                            }
                            else if (ass == "darkcyan")
                            {
                                Console.BackgroundColor = DarkCyan;
                            }
                            else if (ass == "darkgray")
                            {
                                Console.BackgroundColor = DarkGray;
                            }
                            else if (ass == "darkgreen")
                            {
                                Console.BackgroundColor = DarkGreen;
                            }
                            else if (ass == "darkmageneta")
                            {
                                Console.BackgroundColor = DarkMagenta;
                            }
                            else if (ass == "darkred")
                            {
                                Console.BackgroundColor = DarkRed;
                            }
                            else if (ass == "darkyellow")
                            {
                                Console.BackgroundColor = DarkYellow;
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