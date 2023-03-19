using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                if (!inputaman.EndsWith(".goexe"))
                {
                    log(ConsoleColor.Red, "Incompatible format.");
                    log(ConsoleColor.Red, "File must be .goexe");
                }
                if (inputaman.EndsWith(".goexe"))
                {
                    string fuckingprogramname = null;

                    log(ConsoleColor.Yellow, "Application.Start");
                    var content = File.ReadAllLines(@"0:\" + inputaman);
                    string theysaid = null;
                    ConsoleKey keypressed = ConsoleKey.O;
                    int count = 1;
                    String a = null;
                    String b = null;
                    String c = null;
                    String d = null;
                    String e = null;
                    String f = null;
                    String g = null;
                    String h = null;
                    String i = null;
                    String j = null;
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
                        /*old if code. No touchey!*/ if (line.StartsWith("if"))
                        {
                            if (line.Split("=")[1] == "1" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "2" || line.Split("=")[1] == "2" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "1")
                            {
                                if (a == b)
                                {
                                    log(ConsoleColor.Magenta, line.Split("=")[4]);
                                }
                                if (a != b)
                                {

                                    if (line.Split("=")[5].Equals("end"))
                                    {

                                        break;
                                    }
                                    log(ConsoleColor.Magenta, line.Split("=")[5]);

                                }

                            }
                            if (line.Split("=")[1] == "3" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "4" || line.Split("=")[1] == "4" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "3")
                            {
                                if (c == d)
                                {
                                    log(ConsoleColor.Magenta, line.Split("=")[4]);
                                }
                                if (c != d)
                                {

                                    if (line.Split("=")[5].Equals("end"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        log(ConsoleColor.Magenta, line.Split("=")[5]);
                                    }
                                }

                            }
                            if (line.Split("=")[1] == "5" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "6" || line.Split("=")[1] == "6" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "5")
                            {
                                if (e == f)
                                {
                                    log(ConsoleColor.Magenta, line.Split("=")[4]);
                                }
                                if (e != f)
                                {

                                    if (line.Split("=")[5].Equals("end"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        log(ConsoleColor.Magenta, line.Split("=")[5]);
                                    }
                                }

                            }
                            if (line.Split("=")[1] == "7" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "8" || line.Split("=")[1] == "8" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "7")
                            {
                                if (g == h)
                                {
                                    log(ConsoleColor.Magenta, line.Split("=")[4]);
                                }
                                if (g != h)
                                {

                                    if (line.Split("=")[5].Equals("end"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        log(ConsoleColor.Magenta, line.Split("=")[5]);
                                    }
                                }

                            }
                            if (line.Split("=")[1] == "9" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "10" || line.Split("=")[1] == "10" && line.Split("=")[2] == "equals" && line.Split("=")[3] == "9")
                            {
                                if (i == j)
                                {
                                    log(ConsoleColor.Magenta, line.Split("=")[4]);
                                }
                                if (i != j)
                                {

                                    if (line.Split("=")[5].Equals("end"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        log(ConsoleColor.Magenta, line.Split("=")[5]);
                                    }
                                }

                            }

                        }
                        if (line.StartsWith("variable"))
                        {
                            if (line.Split("=")[1] == "1")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    a = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "2")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    b = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "3")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    c = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "4")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    d = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "5")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    e = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "6")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    f = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "7")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    g = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "8")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    h = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "9")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    i = gethandled;
                                }
                            }
                            if (line.Split("=")[1] == "10")
                            {
                                if (line.Split("=")[2] == null || line.Split("=")[2] == "")
                                {
                                    log(ConsoleColor.Red, "ERROR ON LINE " + count);
                                    log(ConsoleColor.Red, "Variable creation must have a value and can not be blank.");
                                    break;
                                }
                                else
                                {
                                    String gethandled = line.Split("=")[2].Replace("{getInput}", theysaid);
                                    j = gethandled;
                                }
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
                                log(ConsoleColor.Red, "Attempted second register. Application may be attempting to reregister as another application!!!");
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
                            string key = line.Substring(6, line.IndexOf(" =") - 6);

                            if (Strings.ContainsKey(key))
                            {
                                Strings.Remove(key);
                            }

                           
                            
                                Strings.Add(key, line.Substring(line.IndexOf("\"") + 1, line.LastIndexOf("\"") - (line.IndexOf("\"") + 1)));
                            
                        }
                        if (line.StartsWith("print="))
                        {
                            string assSplitter = line.Substring(6);
                            //Console.WriteLine(assSplitter + "This is assSplitter");
                            
                            // we like splitting ass round here

                            if (assSplitter.Contains('+'))
                            {
                                string[] contents = assSplitter.Split('+');
                                string result = "";
                                foreach (string cunt in contents)
                                {
                                    if (cunt.Trim().Contains('"'))
                                    {
                                        string thing = cunt.Trim();
                                        thing = thing.Substring(1, thing.LastIndexOf('"') - 1);
                                        result += thing;
                                    }
                                    if (Strings.TryGetValue(cunt.Trim(), out string strval))
                                    {
                                        result += strval;
                                    } //broken area here.//
                                }

                                Console.WriteLine(result);
                            }
                            else
                            {
                                if(Strings.TryGetValue(assSplitter, out string what))
                                {
                                    try
                                    {
                                        Console.WriteLine(what + "---");
                                    } catch
                                    {
                                        Console.WriteLine("owen is gay");
                                    }
                                } //also broken here.//
                                
                                    
                            }
                        }
                        /*if (line.StartsWith("if"))
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
                        }*/
                        if(line.StartsWith(@"save="))
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
                        if(line.StartsWith(@"load="))
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
                    log(ConsoleColor.Red, endmessage);
                }
            }catch(Exception e)
            {
                log(ConsoleColor.Red, e.Message);
            }
        }
    }
}

