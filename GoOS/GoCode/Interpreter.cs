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
    private Dictionary<string, string> Strings = new Dictionary<string, string>() { };
    private Dictionary<string, int> Integers = new Dictionary<string, int>() { };
    
    public static string[] InstallLines;
        
    public static string[] splitit = null;
        
    public static Window window = null;

    public static Boolean windowed = false;
        
    public static ushort windowwidth = 0;
    public static ushort windowheight = 0;

    public void Interpret(string[] lines)
    {
        string fuckingprogramname = "";
        bool hasbeenregistered = false;
        
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
                // TODO: You stopped at int.
            }
        }
    }
}