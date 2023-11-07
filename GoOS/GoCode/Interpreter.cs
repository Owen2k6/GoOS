using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using GoOS.GUI;
using GoOS.GUI.Apps;
using GoOS.Themes;
using Console = BetterConsole;
using static ConsoleColorEx;
using static GoOS.Core;

namespace GoOS.GoCode;

public class Interpreter
{
    // TODO: This is past ekeleze
    // TODO: i, past ekeleze, am putting off doing if statements
    // TODO: there is a likley chance current ekeleze has forgotten all about this and has not implemeted if statments.
    // TODO: If current ekeleze has said he is done, and if statements are not implemented
    // TODO: REMIND HIM!

    private Dictionary<string, string> Strings = new Dictionary<string, string>() { };
    private Dictionary<string, int> Integers = new Dictionary<string, int>() { };

    public string[] InstallLines;

    public string[] splitit = null;

    public Window window = null;

    public Boolean windowed = false;

    public ushort windowwidth = 0;
    public ushort windowheight = 0;

    public void Interpret(string[] lines, bool unnecessaryOutputs = true)
    {
        string fuckingprogramname = "";
        bool hasbeenregistered = false;
        String endmessage = "Process has ended.";

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (Cosmos.System.KeyboardManager.TryReadKey(out var key))
            {
                if (key.Key == ConsoleKeyEx.Escape)
                {
                    i = lines.Length;
                }
            }

            switch (line)
            {
                case { } a when a.StartsWith("goto"):
                    String howlong = line.Split("=")[1];
                    int potato = Convert.ToInt32(howlong);
                    i = potato - 2;
                    break;
                case { } a when a.StartsWith("sleep"):
                    String howlonga = line.Split("=")[1];
                    int potatoa = Convert.ToInt32(howlonga);
                    System.Threading.Thread.Sleep(potatoa);
                    break;
                case { } a when a.StartsWith("input"):
                    if (line == "input=")
                    {
                        textcolour(Blue);
                        string theysaid = Console.ReadLine();

                        if (Strings.ContainsKey("input"))
                        {
                            Strings.Remove("input");
                            Strings.Add("input", theysaid);
                        }

                        Strings.Add("input", theysaid);
                    }
                    else
                    {
                        String addon = line.Replace("input=", "");
                        write(addon);
                        textcolour(Blue);
                        string theysaid = Console.ReadLine();

                        if (Strings.ContainsKey("input"))
                        {
                            Strings.Remove("input");
                        }

                        Strings.Add("input", theysaid);
                    }

                    break;
                case { } a when a.StartsWith("stop"):
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

                    break;
                case { } a when a.StartsWith("regprog"):
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

                    break;
                case { } a when a.StartsWith("string"):
                    string whythehellnotwork = "";
                    string varName = "";
                    string varContents = "";
                    if (line.StartsWith("string "))
                    {
                        whythehellnotwork = line.Replace(@"string ", "");
                        varName = whythehellnotwork.Split(@" = ")[0];
                        varContents = whythehellnotwork.Split(@" = ")[1];
                    }
                    else
                    {
                        whythehellnotwork = line.Replace(@"string=", "");
                        varName = whythehellnotwork.Split(@"=")[0];
                        varContents = whythehellnotwork.Split(@"=")[1];
                    }

                    if (Strings.ContainsKey(varName))
                    {
                        Strings.Remove(varName);
                    }

                    Strings.Add(varName, varContents);
                    break;
                case { } a when a.StartsWith("int"):
                    int intCont = 0;
                    string whythehellnotworka = "";
                    string varNamea = "";
                    string varContentsa = "";
                    if (line.StartsWith("int "))
                    {
                        whythehellnotworka = line.Replace(@"int ", "");
                        varNamea = whythehellnotworka.Split(@" = ")[0];
                        varContentsa = whythehellnotworka.Split(@" = ")[1];
                    }
                    else
                    {
                        whythehellnotworka = line.Replace(@"int=", "");
                        varNamea = whythehellnotworka.Split(@"=")[0];
                        varContentsa = whythehellnotworka.Split(@"=")[1];
                    }

                    try
                    {
                        intCont = int.Parse(varContentsa);
                    }
                    catch
                    {
                        Console.WriteLine(@"Integer: int " + varNamea + " is formatted incorrectly.");
                    }

                    if (Integers.ContainsKey(varNamea))
                    {
                        Integers.Remove(varNamea);
                    }

                    Integers.Add(varNamea, intCont);
                    break;
                case { } a when a.StartsWith("print="):
                    string assSplitter = line.Replace(@"print=", "");
                    // We (still) like splitting ass round here

                    string[] plusSplitter = assSplitter.Split(" + ");
                    // Im sorry owen, but i had to remove it. It was so long it was getting in the way, may add it back.

                    foreach (var str in plusSplitter)
                    {
                        if (assSplitter.Contains("\""))
                        {
                            string thighs = str.Replace("\"", "");

                            if (thighs.Contains(@"\n"))
                            {
                                thighs = thighs.Replace(@"\n", "\n");
                            }

                            Console.Write(thighs);
                        }
                        else if (Strings.TryGetValue(str, out string what))
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
                        else if (Integers.TryGetValue(str, out int whatint))
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

                    break;
                case { } a when a.StartsWith(""):
                    string assSplittera = line.Replace(@"print=", "");
                    // We (still) like splitting ass round here

                    string[] plusSplittera = assSplittera.Split(" + ");
                    // Im sorry owen, but i had to remove it. It was so long it was getting in the way, may add it back.

                    foreach (var str in plusSplittera)
                    {
                        if (assSplittera.Contains("\""))
                        {
                            string thighs = str.Replace("\"", "");

                            if (thighs.Contains(@"\n"))
                            {
                                thighs = thighs.Replace(@"\n", "\n");
                            }

                            Console.Write(thighs);
                        }
                        else if (Strings.TryGetValue(str, out string what))
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
                        else if (Integers.TryGetValue(str, out int whatint))
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

                    Console.WriteLine("");
                    break;
                case { } a when a.StartsWith("save="):
                    string whatvartosave = line.Substring(5);
                    if (Strings.TryGetValue(whatvartosave, out string strval))
                    {
                        if (Directory.Exists(@"0:\content\prf\"))
                        {
                            if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                             whatvartosave +
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
                            if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                             whatvartosave +
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
                            if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                             whatvartosave +
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
                            if (!File.Exists(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                             whatvartosave +
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

                    break;
                case { } a when a.StartsWith("load="):
                    int intConta = 0;
                    string whatvartoload = line.Substring(5);
                    string ass = null;
                    string assType = null;
                    if (Strings.TryGetValue(whatvartoload, out string strvala))
                    {
                        if (File.Exists(
                                @"0:\content\prf\" + fuckingprogramname + @"\" + whatvartoload + @".txt"))
                        {
                            using (StreamReader streamReader = new StreamReader(
                                       @"0:\content\prf\" + fuckingprogramname + @"\" + whatvartoload +
                                       @".txt",
                                       Encoding.UTF8))
                            {
                                ass = File.ReadLines(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                                     whatvartoload + @".txt").Skip(0).Take(1).First();
                                assType = File
                                    .ReadLines(@"0:\content\prf\" + fuckingprogramname + @"\" +
                                               whatvartoload +
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
                                        intConta = int.Parse(ass);
                                    }
                                    catch
                                    {
                                        Console.WriteLine(@"Load: int " + whatvartoload +
                                                          " is formatted incorrectly.");
                                    }

                                    string trueCont = intConta.ToString();

                                    if (Integers.ContainsKey(whatvartoload))
                                    {
                                        Integers.Remove(whatvartoload);
                                    }

                                    Integers.Add(whatvartoload, intConta);
                                }
                            }
                        }
                    }

                    break;
                case { } a when a.StartsWith("frontcolor="):
                    string assa = line.Substring(11);
                    if (assa == "white")
                    {
                        Console.ForegroundColor = White;
                    }
                    else if (assa == "blue")
                    {
                        Console.ForegroundColor = Blue;
                    }
                    else if (assa == "green")
                    {
                        Console.ForegroundColor = Green;
                    }
                    else if (assa == "yellow")
                    {
                        Console.ForegroundColor = Yellow;
                    }
                    else if (assa == "black")
                    {
                        Console.ForegroundColor = Black;
                    }
                    else if (assa == "cyan")
                    {
                        Console.ForegroundColor = Cyan;
                    }
                    else if (assa == "gray")
                    {
                        Console.ForegroundColor = Gray;
                    }
                    else if (assa == "magenta")
                    {
                        Console.ForegroundColor = Magenta;
                    }
                    else if (assa == "red")
                    {
                        Console.ForegroundColor = Red;
                    }
                    else if (assa == "darkblue")
                    {
                        Console.ForegroundColor = DarkBlue;
                    }
                    else if (assa == "darkcyan")
                    {
                        Console.ForegroundColor = DarkCyan;
                    }
                    else if (assa == "darkgray")
                    {
                        Console.ForegroundColor = DarkGray;
                    }
                    else if (assa == "darkgreen")
                    {
                        Console.ForegroundColor = DarkGreen;
                    }
                    else if (assa == "darkmageneta")
                    {
                        Console.ForegroundColor = DarkMagenta;
                    }
                    else if (assa == "darkred")
                    {
                        Console.ForegroundColor = DarkRed;
                    }
                    else if (assa == "darkyellow")
                    {
                        Console.ForegroundColor = DarkYellow;
                    }

                    break;
                case { } a when a.StartsWith("backcolor="):
                    string assb = line.Substring(10);
                    if (assb == "white")
                    {
                        Console.BackgroundColor = White;
                    }
                    else if (assb == "blue")
                    {
                        Console.BackgroundColor = Blue;
                    }
                    else if (assb == "green")
                    {
                        Console.BackgroundColor = Green;
                    }
                    else if (assb == "yellow")
                    {
                        Console.BackgroundColor = Yellow;
                    }
                    else if (assb == "black")
                    {
                        Console.BackgroundColor = Black;
                    }
                    else if (assb == "cyan")
                    {
                        Console.BackgroundColor = Cyan;
                    }
                    else if (assb == "gray")
                    {
                        Console.BackgroundColor = Gray;
                    }
                    else if (assb == "magenta")
                    {
                        Console.BackgroundColor = Magenta;
                    }
                    else if (assb == "red")
                    {
                        Console.BackgroundColor = Red;
                    }
                    else if (assb == "darkblue")
                    {
                        Console.BackgroundColor = DarkBlue;
                    }
                    else if (assb == "darkcyan")
                    {
                        Console.BackgroundColor = DarkCyan;
                    }
                    else if (assb == "darkgray")
                    {
                        Console.BackgroundColor = DarkGray;
                    }
                    else if (assb == "darkgreen")
                    {
                        Console.BackgroundColor = DarkGreen;
                    }
                    else if (assb == "darkmageneta")
                    {
                        Console.BackgroundColor = DarkMagenta;
                    }
                    else if (assb == "darkred")
                    {
                        Console.BackgroundColor = DarkRed;
                    }
                    else if (assb == "darkyellow")
                    {
                        Console.BackgroundColor = DarkYellow;
                    }

                    break;
            }
        }
        
        if (unnecessaryOutputs)
        {
            if (endmessage != null)
            {
                endmessage = "Process has ended.";
            }

            log(ThemeManager.ErrorText, endmessage);
        }
    }
}