using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoOS.Commands
{
    public class run
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
            log(ConsoleColor.Red, "GoOS Admin: Enter file name");
            textcolour(ConsoleColor.Yellow);
            write("FilePath: 0:\\");
            String inputaman = Console.ReadLine();
            try
            {
                log(ConsoleColor.Blue, "GoOS Admin: Attempting to run " + inputaman);
                if (!inputaman.EndsWith(".goexe"))
                {
                    log(ConsoleColor.Red, "GoOS Admin: Incompatible format.");
                    log(ConsoleColor.Red, "GoOS Admin: File must be .goexe");
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
                        if (line.StartsWith("print"))
                        {
                            string thingtosay = line.Replace("print=", "");
                            thingtosay = thingtosay.Replace("{getInput}", theysaid);
                            thingtosay = thingtosay.Replace("{1}", a);
                            thingtosay = thingtosay.Replace("{2}", b);
                            thingtosay = thingtosay.Replace("{3}", c);
                            thingtosay = thingtosay.Replace("{4}", d);
                            thingtosay = thingtosay.Replace("{5}", e);
                            thingtosay = thingtosay.Replace("{6}", f);
                            thingtosay = thingtosay.Replace("{7}", g);
                            thingtosay = thingtosay.Replace("{8}", h);
                            thingtosay = thingtosay.Replace("{9}", i);
                            thingtosay = thingtosay.Replace("{10}", j);
                            log(ConsoleColor.Magenta, thingtosay);
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
                        if (line.StartsWith("if"))
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
                            if(hasbeenregistered) {
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
