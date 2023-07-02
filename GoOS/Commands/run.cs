using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;
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
                    log(Yellow, "Application.Start");
                    var content = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\" + inputaman);
                    string theysaid = null;
                    ConsoleKey keypressed = ConsoleKey.O;
                    String endmessage = "Process has ended.";
                    Boolean hasbeenregistered = false;

                    bool poo = false;

                    for (int i = 0; i < content.Length; i++)
                    {
                        string line = content[i];
                        
                        if (poo)
                        {
                            line = line.Split(": ")[1].Trim();
                            poo = false;
                        }

                        //log(Magenta, "LINE FOUND: CONTENT: " + line);

                        if (line.StartsWith("#"))
                        {
                        }

                        if (line.StartsWith(""))
                        {
                        }

                        if (line.StartsWith("goto"))
                        {
                            try
                            {
                                String howlong = line.Split("=")[1];
                                int potato = Convert.ToInt32(howlong);
                                i = potato;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("owen caused 9/11");
                            }
                        }

                        if (line.StartsWith("sleep"))
                        {
                            String howlong = line.Split("=")[1];
                            int potato = Convert.ToInt32(howlong);
                            
                            //while (true)
                            //{
                            //    Console.WriteLine("Haha you've been fooled!");
                            //}

                            System.Threading.Thread.Sleep(potato);
                        }

                        if (line.StartsWith("input"))
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

                        if (line.StartsWith("stop"))
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

                            string[] ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN = assSplitter.Split(" + ");

                            foreach (var ASS in ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN)
                            {
                                if (ASS.Contains("\""))
                                {
                                    string thighs = ASS.Replace("\"", "");
                                    string AccountingForNewline = thighs;

                                    if (thighs.Contains("\\n"))
                                    {
                                        AccountingForNewline = thighs.Replace("\\n", "\n");
                                    }

                                    /////////////////////////////////////// Trying to make \n work ////////////////////////////////////////
                                    // Input: "Hi!\nHello."                                                                              //
                                    // What I want it to output: "Hi!\n" "Hello."                                                        //
                                    // (and actually make a new line)                                                                    //
                                    // Just that the code itself has the quotes removed, I had to use them to show the separate strings. //
                                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                                    
                                    Console.Write(AccountingForNewline);
                                }
                                else if (Strings.TryGetValue(ASS, out string what))
                                {
                                    try
                                    {
                                        Console.Write(what);
                                    }
                                    catch
                                    {
                                        Console.Write("owen is gay");
                                    }
                                }
                                else if (Integers.TryGetValue(ASS, out int whatint))
                                {
                                    try
                                    {
                                        Console.Write(what);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("owen is gay");
                                    }
                                }
                            }
                        }
                        
                        if (line.StartsWith(@"println="))
                        {
                            string assSplitter = line.Replace(@"println=", "");
                            // we like splitting ass round here

                            string[] ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN = assSplitter.Split(" + ");

                            for (int e = 0; e < ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN.Length; e++)
                            {
                                if (ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN[e].Contains("\""))
                                {
                                    string thighs = ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN[e].Replace("\"", "");
                                    string AccountingForNewline = thighs;

                                    if (thighs.Contains("\\n"))
                                    {
                                        AccountingForNewline = thighs.Replace("\\n", "\n");
                                    }

                                    /////////////////////////////////////// Trying to make \n work ////////////////////////////////////////
                                    // Input: "Hi!\nHello."                                                                              //
                                    // What I want it to output: "Hi!\n" "Hello."                                                        //
                                    // (and actually make a new line)                                                                    //
                                    // Just that the code itself has the quotes removed, I had to use them to show the separate strings. //
                                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                                    
                                    Console.Write(AccountingForNewline);
                                }
                                else if (Strings.TryGetValue(ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN[e], out string what))
                                {
                                    try
                                    {
                                        Console.Write(what);
                                    }
                                    catch
                                    {
                                        Console.Write("owen is gay");
                                    }
                                }
                                else if (Integers.TryGetValue(ARE_YOU_GONNA_SINK_OR_SWIM_IN_questionMark_FIGHTING_FOR_MY_ATTENTION_questionMark_ONE_LOOK_GOT_YOU_LIMPING_comma_ANNY_GOT_YOU_SIMPIN[e], out int whatint))
                                {
                                    try
                                    {
                                        Console.Write(what);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("owen is gay");
                                    }
                                }
                            }
                            
                            Console.WriteLine();
                        }

                        if (line.StartsWith("if"))
                        {
                            string[] args = line.Split(" ");

                            if (args[2] == " == ")
                            {
                                if (Strings.TryGetValue(args[1].Trim(), out string strval))
                                {
                                    if (strval == args[4])
                                    {
                                        poo = true;
                                        i--;
                                        continue;
                                    }
                                    else
                                    {
                                        poo = false;
                                        continue;
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

                        if (line.StartsWith("backcolor="))
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