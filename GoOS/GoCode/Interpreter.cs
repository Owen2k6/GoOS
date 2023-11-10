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
using PrismAPI.Graphics;
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
    private Dictionary<string, bool> Booleans = new Dictionary<string, bool>() { };
    private Dictionary<string, Window> Windows = new Dictionary<string, Window>();

    private Dictionary<string, int> ButtonActions = new Dictionary<string, int>();

    private string fuckingprogramname = "";
    private bool hasbeenregistered = false;
    private String endmessage = "Process has ended.";

    private int skipLevel = 1;
    private int currentLevel = 0;
    private int expectedLevel = 0;

    private int i = 0;

    public void Interpret(string[] lines, bool unnecessaryOutputs = true)
    {
        string tabSkip = "\t";
        for (int a = 0; a < skipLevel; a++)
        {
            tabSkip = tabSkip + "\t";
        }
        for (i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (Cosmos.System.KeyboardManager.TryReadKey(out var key))
            {
                if (key.Key == ConsoleKeyEx.Escape)
                {
                    i = lines.Length;
                }
            }

            string[] tabCountString = line.Split("\t");

            currentLevel = tabCountString.Length - 1;

            string trueLine = line.Replace("\t", "");

            if (currentLevel < expectedLevel)
            {
                expectedLevel = currentLevel;
                skipLevel = expectedLevel + 1;
            }
            if (currentLevel >= skipLevel)
                break;

            switchThis(trueLine);
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

    private void switchThis(String line)
    {
        switch (line)
        {
            case { } a when a.StartsWith("button="):
                string buttonLess = a.Replace("button=", "");
                string parentWindowName = buttonLess.Split("=")[0];
                
                string buttonName = buttonLess.Split("=")[1];
                
                string awindowWidthPre = buttonLess.Split("=")[2];
                string awindowHeightPre = buttonLess.Split("=")[3];
                ushort awindowWidth = ushort.Parse(awindowWidthPre);
                ushort awindowHeight = ushort.Parse(awindowHeightPre);
                
                string xpre = buttonLess.Split("=")[4];
                string ypre = buttonLess.Split("=")[5];
                ushort x = ushort.Parse(xpre);
                ushort y = ushort.Parse(ypre);
                
                string clickActionLinepre = buttonLess.Split("=")[5];
                ushort clickActionLine = ushort.Parse(clickActionLinepre);

                if (!Windows.ContainsKey(parentWindowName))
                {
                    log(ThemeManager.ErrorText, "Could not find specified parent window!");
                    break;
                }
                
                if (ButtonActions.ContainsKey(buttonName))
                {
                    ButtonActions.Remove(buttonName);
                }

                ButtonActions.Add(buttonName, clickActionLine);
                
                Windows.TryGetValue(parentWindowName, out Window buttonWindow);

                Button button = new Button(buttonWindow, x, y, awindowWidth, awindowHeight, buttonName);
                button.ClickedAlt = ClickAction;
                break;
            case { } a when a.StartsWith("window="):
                Window window = new Window();
                string windowLess = a.Replace("window=", "");
                string windowName = windowLess.Split("=")[0];

                window.Title = windowName;
                
                if (Windows.ContainsKey(windowName))
                {
                    Windows.Remove(windowName);
                }
                
                string windowWidthPre = windowLess.Split("=")[1];
                string windowHeightPre = windowLess.Split("=")[2];
                ushort windowWidth = ushort.Parse(windowWidthPre);
                ushort windowHeight = ushort.Parse(windowHeightPre);

                window.Contents = new Canvas(windowWidth, windowHeight);
                window.Visible = true;
                window.Closable = true;
                
                window.Contents.Clear(Color.LightGray);
                window.RenderSystemStyleBorder();
                
                Windows.Add(windowName, window);
                WindowManager.AddWindow(window);
                break;
            case { } a when a.StartsWith("bool="):
                string input = a.Replace("bool=", "");

                string name = input.Split("=")[0];
                bool content = input.Split("=")[1].ToLower() == "true";

                if (Booleans.ContainsKey(name))
                {
                    Booleans.Remove(name);
                    Booleans.Add(name, content);
                    break;
                }
                
                Booleans.Add(name, content);
                break;
            case { } a when a.StartsWith("if="):

                string iffingHell = line.Replace("if=", "");

                if (!iffingHell.StartsWith("!"))
                {
                    switch (iffingHell)
                    {
                        case { } q when q.Contains("<"):
                            int inputOne = 0;
                            int inputTwo = 0;
                            string[] inputs = iffingHell.Split("<");
                            if (inputs[0].Contains("\"") || inputs[0].Contains("\""))
                            {
                                log(ThemeManager.ErrorText, "You cannot use < with strings.");
                            }
                            else
                            {
                                try
                                {
                                    inputOne = int.Parse(inputs[0]);
                                    inputTwo = int.Parse(inputs[0]);
                                }
                                catch (Exception)
                                {
                                    inputOne = 0;
                                    inputTwo = 0;
                                }
                            }

                            if (Integers.ContainsKey(inputs[0]))
                            {
                                Integers.TryGetValue(inputs[0], out inputOne);
                            }

                            if (Integers.ContainsKey(inputs[1]))
                            {
                                Integers.TryGetValue(inputs[1], out inputTwo);
                            }

                            if (inputOne < inputTwo)
                            {
                                expectedLevel = currentLevel + 1;
                                skipLevel = expectedLevel + 1;
                            }

                            break;
                        case { } q when q.Contains("<="):
                            int inputOnea = 0;
                            int inputTwoa = 0;
                            string[] inputsa = iffingHell.Split("<=");
                            if (inputsa[0].Contains("\"") || inputsa[0].Contains("\""))
                            {
                                log(ThemeManager.ErrorText, "You cannot use <= with strings.");
                            }
                            else
                            {
                                try
                                {
                                    inputOnea = int.Parse(inputsa[0]);
                                    inputTwoa = int.Parse(inputsa[0]);
                                }
                                catch (Exception)
                                {
                                    inputOnea = 0;
                                    inputTwoa = 0;
                                }
                            }

                            if (Integers.ContainsKey(inputsa[0]))
                            {
                                Integers.TryGetValue(inputsa[0], out inputOnea);
                            }

                            if (Integers.ContainsKey(inputsa[1]))
                            {
                                Integers.TryGetValue(inputsa[1], out inputTwoa);
                            }

                            if (inputOnea <= inputTwoa)
                            {
                                expectedLevel = currentLevel + 1;
                                skipLevel = expectedLevel + 1;
                            }

                            break;
                        case { } q when q.Contains(">"):
                            int inputOneb = 0;
                            int inputTwob = 0;
                            string[] inputsb = iffingHell.Split(">");
                            if (inputsb[0].Contains("\"") || inputsb[0].Contains("\""))
                            {
                                log(ThemeManager.ErrorText, "You cannot use < with strings.");
                            }
                            else
                            {
                                try
                                {
                                    inputOneb = int.Parse(inputsb[0]);
                                    inputTwob = int.Parse(inputsb[0]);
                                }
                                catch (Exception)
                                {
                                    inputOneb = 0;
                                    inputTwob = 0;
                                }
                            }

                            if (Integers.ContainsKey(inputsb[0]))
                            {
                                Integers.TryGetValue(inputsb[0], out inputOneb);
                            }

                            if (Integers.ContainsKey(inputsb[1]))
                            {
                                Integers.TryGetValue(inputsb[1], out inputTwob);
                            }

                            if (inputOneb > inputTwob)
                            {
                                expectedLevel = currentLevel + 1;
                                skipLevel = expectedLevel + 1;
                            }

                            break;
                        case { } q when q.Contains(">="):
                            int inputOnec = 0;
                            int inputTwoc = 0;
                            string[] inputsc = iffingHell.Split(">=");
                            if (inputsc[0].Contains("\"") || inputsc[0].Contains("\""))
                            {
                                log(ThemeManager.ErrorText, "You cannot use < with strings.");
                            }
                            else
                            {
                                try
                                {
                                    inputOnec = int.Parse(inputsc[0]);
                                    inputTwoc = int.Parse(inputsc[0]);
                                }
                                catch (Exception)
                                {
                                    inputOnec = 0;
                                    inputTwoc = 0;
                                }
                            }

                            if (Integers.ContainsKey(inputsc[0]))
                            {
                                Integers.TryGetValue(inputsc[0], out inputOnec);
                            }

                            if (Integers.ContainsKey(inputsc[1]))
                            {
                                Integers.TryGetValue(inputsc[1], out inputTwoc);
                            }

                            if (inputOnec >= inputTwoc)
                            {
                                expectedLevel = currentLevel + 1;
                                skipLevel = expectedLevel + 1;
                            }

                            break;
                        case { } q when q.Contains("=="):
                            string[] inputsd = iffingHell.Split("==");

                            if (inputsd[0].StartsWith("\"") || inputsd[1].StartsWith("\"") ||
                                Strings.ContainsKey(inputsd[0]) || Strings.ContainsKey(inputsd[1]))
                            {
                                string inputOned = "";
                                string inputTwod = "";
                                if (Strings.ContainsKey(inputsd[0]))
                                {
                                    Strings.TryGetValue(inputsd[0], out inputOned);
                                }

                                if (Strings.ContainsKey(inputsd[1]))
                                {
                                    Strings.TryGetValue(inputsd[1], out inputTwod);
                                }

                                if (inputOned == inputTwod)
                                {
                                    expectedLevel = currentLevel + 1;
                                    skipLevel = expectedLevel + 1;
                                }
                            }
                            else
                            {
                                int inputOned = 0;
                                int inputTwod = 0;
                                try
                                {
                                    inputOned = int.Parse(inputsd[0]);
                                    inputTwod = int.Parse(inputsd[0]);
                                }
                                catch (Exception)
                                {
                                    inputOned = 0;
                                    inputTwod = 0;
                                }

                                if (Integers.ContainsKey(inputsd[0]))
                                {
                                    Integers.TryGetValue(inputsd[0], out inputOned);
                                }

                                if (Integers.ContainsKey(inputsd[1]))
                                {
                                    Integers.TryGetValue(inputsd[1], out inputTwod);
                                }

                                if (inputOned == inputTwod)
                                {
                                    expectedLevel = currentLevel + 1;
                                    skipLevel = expectedLevel + 1;
                                }
                            }

                            break;
                    }
                }
                else
                {
                    string shit = iffingHell.Replace("!", "");
                    string[] inputsd = shit.Split("==");

                    if (inputsd[0].StartsWith("\"") || inputsd[1].StartsWith("\"") ||
                        Strings.ContainsKey(inputsd[0]) || Strings.ContainsKey(inputsd[1]))
                    {
                        string inputOned = "";
                        string inputTwod = "";
                        if (Strings.ContainsKey(inputsd[0]))
                        {
                            Strings.TryGetValue(inputsd[0], out inputOned);
                        }

                        if (Strings.ContainsKey(inputsd[1]))
                        {
                            Strings.TryGetValue(inputsd[1], out inputTwod);
                        }

                        if (inputOned == inputTwod)
                        {
                            expectedLevel = currentLevel + 1;
                            skipLevel = expectedLevel + 1;
                        }
                    }
                    else
                    {
                        int inputOned = 0;
                        int inputTwod = 0;
                        try
                        {
                            inputOned = int.Parse(inputsd[0]);
                            inputTwod = int.Parse(inputsd[0]);
                        }
                        catch (Exception)
                        {
                            inputOned = 0;
                            inputTwod = 0;
                        }

                        if (Integers.ContainsKey(inputsd[0]))
                        {
                            Integers.TryGetValue(inputsd[0], out inputOned);
                        }

                        if (Integers.ContainsKey(inputsd[1]))
                        {
                            Integers.TryGetValue(inputsd[1], out inputTwod);
                        }

                        if (inputOned == inputTwod)
                        {
                            expectedLevel = currentLevel + 1;
                            skipLevel = expectedLevel + 1;
                        }
                    }
                }
                break;
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
                else if (line.StartsWith("string="))
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
                            Console.Write(whatint);
                        }
                        catch
                        {
                            Console.WriteLine("owen is gay");
                        }
                    }
                }

                break;
            case { } a when a.StartsWith("println="):
                string assSplittera = line.Replace(@"println=", "");
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
                            Console.Write(whatint);
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

    private void ClickAction(string name)
    {
        if (ButtonActions.ContainsKey(name))
        {
            ButtonActions.TryGetValue(name, out i);
        }
    }
    
    
}