﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using GoOS.GUI;
using PrismAPI.Graphics;
using static ConsoleColorEx;
using Console = BetterConsole;
using ConsoleColor = PrismAPI.Graphics.Color;

// 9xCode Beta 3.1
// Licensed under the MIT license
// Use permitted in GoOS

namespace GoOS._9xCode
{
    public static partial class Interpreter
    {
        public static void Run(string file)
        {
            if (!File.Exists(file))
            {
                HandleError("9xCode", "Script doesn't exist!");
            }

            Console.Title = $"{file.Substring(file.LastIndexOf(@"\")).Replace(".9xc", "")} - 9xCode";
            Console.ForegroundColor = White;
            Interpret(File.ReadAllLines(file));
        }

        private static bool Interpreting = false;

        private static void Interpret(string[] code)
        {
            Interpreting = true;

            Console.ForegroundColor = Cyan;
            Console.WriteLine("Mobren 9xCode Interpreter Version b3.1\n");
            Console.ForegroundColor = White;

            bool SysLib = false, ConsoleLib = false, IOLib = false, TimeLib = false, _9xGLLib = false, GoOSLib = false;

            Dictionary<string, bool> Booleans = new Dictionary<string, bool>() { };
            Dictionary<string, int> Integers = new Dictionary<string, int>() { };
            Dictionary<string, string> Strings = new Dictionary<string, string>() { { "Version", "b3.0" } };
            Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor>() { };
            Dictionary<string, Window> Windows = new Dictionary<string, Window>();

            for (int i = 0; i < code.Length; i++)
            {
                if (Interpreting)
                {
                    try
                    {
                        WindowManager.Update(); // Don't lock the WM

                        string line = code[i].Trim().Replace("\\n", "\n");

                        #region Globals

                        if (ConsoleLib)
                        {
                            if (Colors.ContainsKey("ForeColor"))
                            {
                                Colors.Remove("ForeColor");
                            }
                            if (Colors.ContainsKey("BackColor"))
                            {
                                Colors.Remove("BackColor");
                            }
                            if (Integers.ContainsKey("CursorX"))
                            {
                                Integers.Remove("CursorX");
                            }
                            if (Integers.ContainsKey("CursorY"))
                            {
                                Integers.Remove("CursorY");
                            }
                            if (Booleans.ContainsKey("Cursor"))
                            {
                                Booleans.Remove("Cursor");
                            }

                            Colors.Add("ForeColor", Console.ForegroundColor);
                            Colors.Add("BackColor", Console.BackgroundColor);
                            Integers.Add("CursorX", Console.CursorLeft);
                            Integers.Add("CursorY", Console.CursorTop);
                            Booleans.Add("Cursor", Console.CursorVisible);
                        }

                        if (TimeLib)
                        {
                            if (Integers.ContainsKey("Milliseconds"))
                            {
                                Integers.Remove("Milliseconds");
                            }
                            if (Integers.ContainsKey("Seconds"))
                            {
                                Integers.Remove("Seconds");
                            }
                            if (Integers.ContainsKey("Minutes"))
                            {
                                Integers.Remove("Minutes");
                            }
                            if (Integers.ContainsKey("Hours"))
                            {
                                Integers.Remove("Hours");
                            }

                            Integers.Add("Milliseconds", DateTime.Now.Millisecond);
                            Integers.Add("Seconds", DateTime.Now.Second);
                            Integers.Add("Minutes", DateTime.Now.Minute);
                            Integers.Add("Hours", DateTime.Now.Hour);
                        }

                        #endregion

                        #region Standard library

                        if (line == string.Empty)
                        {
                            continue;
                        }

                        if (line.StartsWith(";"))
                        {
                            continue;
                        }

                        else if (line.StartsWith("endif"))
                        {
                            continue;
                        }

                        if (line.Contains(";"))
                        {
                            int start = line.IndexOf(';');
                            line = line.Remove(start).Trim();
                        }

                        if (line.StartsWith("import"))
                        {
                            string sub = line.Substring(7);

                            if (sub.StartsWith("System"))
                            {
                                SysLib = true;
                                continue;
                            }

                            else if (sub.StartsWith("Console"))
                            {
                                ConsoleLib = true;
                                continue;
                            }

                            else if (sub.StartsWith("IO"))
                            {
                                IOLib = true;
                                continue;
                            }

                            else if (sub.StartsWith("Time"))
                            {
                                TimeLib = true;
                                continue;
                            }

                            else if (sub.StartsWith("9xGL"))
                            {
                                _9xGLLib = true;
                                continue;
                            }

                            else if (sub.StartsWith("GoOS"))
                            {
                                GoOSLib = true;
                                continue;
                            }

                            else
                            {
                                HandleError("Error", "Unknown library.");
                            }
                        }

                        else if (line.StartsWith("Bool"))
                        {
                            if (line.Contains('='))
                            {
                                string key = line.Substring(5, line.IndexOf(" =") - 5);
                                string sub = line.Substring(line.IndexOf("= ") + 2).ToLower();

                                #region Console library

                                if (!ConsoleLib)
                                {
                                    if (key == "Cursor")
                                    {
                                        HandleError("Error", "Console library not imported");
                                        break;
                                    }
                                }

                                if (ConsoleLib && key == "Cursor")
                                {
                                    Console.CursorVisible = Convert.ToBoolean(sub);
                                    continue;
                                }

                                #endregion

                                if (Booleans.ContainsKey(key))
                                {
                                    Booleans.Remove(key);
                                }

                                Booleans.Add(key, Convert.ToBoolean(sub));
                                continue;
                            }
                            else
                            {
                                string key = line.Substring(5);

                                if (Booleans.ContainsKey(key))
                                {
                                    Booleans.Remove(key);
                                }

                                Booleans.Add(key, false);
                                continue;
                            }
                        }

                        else if (line.StartsWith("Color"))
                        {
                            if (line.Contains('='))
                            {
                                string key = line.Substring(6, line.IndexOf(" =") - 6);
                                string sub = line.Substring(line.IndexOf("= ") + 2);

                                #region Console library

                                if (!ConsoleLib)
                                {
                                    if (key == "ForeColor" || key == "BackColor")
                                    {
                                        HandleError("Error", "Console library not imported");
                                        break;
                                    }
                                }

                                if (ConsoleLib && key == "ForeColor")
                                {
                                    if (StringToConsoleColor.TryGetValue(sub, out ConsoleColor colval1))
                                    {
                                        Console.ForegroundColor = colval1;
                                    }
                                    continue;
                                }

                                if (ConsoleLib && key == "BackColor")
                                {
                                    if (StringToConsoleColor.TryGetValue(sub, out ConsoleColor colval2))
                                    {
                                        Console.BackgroundColor = colval2;
                                    }
                                    continue;
                                }

                                #endregion

                                if (Colors.ContainsKey(key))
                                {
                                    Colors.Remove(key);
                                }

                                if (StringToConsoleColor.TryGetValue(sub, out ConsoleColor colval3))
                                {
                                    Colors.Add(key, colval3);
                                }
                                else
                                {
                                    HandleError("Syntax Error", "Unknown Color.");
                                }
                                continue;
                            }
                            else
                            {
                                string key = line.Substring(6);

                                if (Booleans.ContainsKey(key))
                                {
                                    Booleans.Remove(key);
                                }

                                Booleans.Add(key, false);
                                continue;
                            }
                        }

                        else if (line.StartsWith("Int"))
                        {
                            if (line.Contains('='))
                            {
                                string key = line.Substring(4, line.IndexOf(" =") - 4);
                                string sub = line.Substring(line.IndexOf("= ") + 2);

                                if (Integers.ContainsKey(key))
                                {
                                    Integers.Remove(key);
                                }

                                #region Console library

                                if (!ConsoleLib)
                                {
                                    if (sub.StartsWith("CursorX") || sub.StartsWith("CursorY"))
                                    {
                                        HandleError("Error", "Console library not imported");
                                        break;
                                    }
                                }

                                if (ConsoleLib && key == "CursorX")
                                {
                                    Console.CursorLeft = Convert.ToInt32(sub);
                                    continue;
                                }

                                if (ConsoleLib && key == "CursorY")
                                {
                                    Console.CursorTop = Convert.ToInt32(sub);
                                    continue;
                                }

                                #endregion

                                #region Time library

                                if (!TimeLib)
                                {
                                    if (sub.StartsWith("Seconds") || sub.StartsWith("Minutes") || sub.Equals("Hours"))
                                    {
                                        HandleError("Error", "Time library not imported");
                                        break;
                                    }
                                }

                                #endregion

                                if (TimeLib && sub.StartsWith("Seconds"))
                                {
                                    Integers.Add(key, DateTime.Now.Second);
                                }
                                else if (TimeLib && sub.StartsWith("Minutes"))
                                {
                                    Integers.Add(key, DateTime.Now.Minute);
                                }
                                else if (TimeLib && sub.StartsWith("Hours"))
                                {
                                    Integers.Add(key, DateTime.Now.Hour);
                                }
                                else if (sub.StartsWith("0x"))
                                {
                                    sub.Remove(0, 2);
                                    Integers.Add(key, Convert.ToInt32(sub, 16));
                                }
                                else
                                {
                                    Integers.Add(key, Convert.ToInt32(sub));
                                }
                                continue;
                            }
                            else if (line.Contains("++"))
                            {
                                string key = line.Substring(4, line.IndexOf("++") - 4);

                                if (!Integers.TryGetValue(key, out int intval))
                                {
                                    HandleError("Error", "Unknown variable.");
                                    break;
                                }

                                if (Integers.ContainsKey(key))
                                {
                                    Integers.Remove(key);
                                }

                                Integers.Add(key, intval + 1);
                            }
                            else if (line.Contains("--"))
                            {
                                string key = line.Substring(4, line.IndexOf("--") - 4);

                                if (!Integers.TryGetValue(key, out int intval))
                                {
                                    HandleError("Error", "Unknown variable.");
                                    break;
                                }

                                if (Integers.ContainsKey(key))
                                {
                                    Integers.Remove(key);
                                }

                                Integers.Add(key, intval - 1);
                            }
                            else
                            {
                                string key = line.Substring(4);

                                if (Integers.ContainsKey(key))
                                {
                                    Integers.Remove(key);
                                }

                                Integers.Add(key, 0);
                                continue;
                            }
                        }

                        else if (line.StartsWith("String"))
                        {
                            if (line.Contains('='))
                            {
                                string key = line.Substring(7, line.IndexOf(" =") - 7);

                                if (Strings.ContainsKey(key))
                                {
                                    Strings.Remove(key);
                                }

                                if (!IOLib)
                                {
                                    if (line.Substring(line.IndexOf("= ") + 2).StartsWith("ReadFile"))
                                    {
                                        HandleError("Error", "IO library not imported");
                                        break;
                                    }
                                }

                                if (IOLib && line.Substring(line.IndexOf("= ") + 2).StartsWith("ReadFile"))
                                {
                                    string arg = line.Split('>')[2].Trim().Replace("\"", "");

                                    if (Strings.TryGetValue(arg.Trim(), out string strval))
                                    {
                                        arg = strval;
                                    }

                                    Strings.Add(key, File.ReadAllText(arg));
                                }
                                else if (line.Substring(line.IndexOf("= ") + 2).StartsWith("ReadLine"))
                                {
                                    Strings.Add(key, Console.ReadLine());
                                }
                                else if (line.Substring(line.IndexOf("= ") + 2).StartsWith("Read"))
                                {
                                    Strings.Add(key, Console.ReadKey(true).Key.ToString());
                                }
                                else
                                {
                                    Strings.Add(key, line.Substring(line.IndexOf("\"") + 1, line.LastIndexOf("\"") - (line.IndexOf("\"") + 1)));
                                }
                                continue;
                            }
                            else
                            {
                                string key = line.Substring(7);

                                if (Strings.ContainsKey(key))
                                {
                                    Strings.Remove(key);
                                }

                                Strings.Add(key, "");
                                continue;
                            }
                        }

                        else if (_9xGLLib && line.StartsWith("Window"))
                        {
                            if (line.Contains('='))
                            {
                                string key = line.Substring(7, line.IndexOf(" =") - 7);

                                if (Windows.ContainsKey(key))
                                {
                                    Windows.Remove(key);
                                }

                                if (_9xGLLib && line.Substring(line.IndexOf("= ") + 2).StartsWith("GetWindow"))
                                {
                                    string[] args2 = line.Split('>')[2].Trim().Split(',');

                                    if (args2.Length < 3)
                                    {
                                        HandleError("Error", "Argument underflow.");
                                        break;
                                    }
                                    if (args2.Length > 3)
                                    {
                                        HandleError("Error", "Argument overflow.");
                                        break;
                                    }

                                    Window wnd = new Window();
                                    ushort width, height;

                                    if (args2[0].Trim().Contains('"'))
                                        wnd.Title = args2[0].Substring(1, args2[0].Length - 2);
                                    else if (Strings.TryGetValue(args2[0].Trim(), out string strval))
                                        wnd.Title = strval;

                                    if (Integers.TryGetValue(args2[1].Trim(), out int widthval))
                                        width = Convert.ToUInt16(widthval);
                                    else
                                        width = Convert.ToUInt16(args2[1].Trim());

                                    if (Integers.TryGetValue(args2[2].Trim(), out int heightval))
                                        height = Convert.ToUInt16(heightval);
                                    else
                                        height = Convert.ToUInt16(args2[2].Trim());

                                    wnd.Contents = new Canvas(width, height);
                                    wnd.Visible = true;
                                    wnd.Closable = true;

                                    wnd.Contents.Clear(Color.LightGray);
                                    wnd.RenderSystemStyleBorder();

                                    Windows.Add(key, wnd);
                                    WindowManager.AddWindow(wnd);
                                }
                                continue;
                            }
                            else
                            {
                                string key = line.Substring(7);

                                if (Strings.ContainsKey(key))
                                {
                                    Strings.Remove(key);
                                }

                                Strings.Add(key, "");
                                continue;
                            }
                        }

                        else if (line.StartsWith("if"))
                        {
                            int start = line.IndexOf('(');
                            int end = line.IndexOf(')') - start;

                            string[] comparation;
                            int endif = 0;

                            for (int o = i; o < code.Length; o++)
                            {
                                if (code[o] == "endif")
                                {
                                    endif = o;
                                    break;
                                }
                            }

                            if (line.Contains('='))
                            {
                                comparation = line.Substring(4).Split('=');

                                if (Integers.TryGetValue(comparation[0].Trim(), out int intval))
                                {
                                    comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                    if (comparation[1] != intval.ToString())
                                    {
                                        i = endif + 1;
                                    }
                                }
                                else if (Strings.TryGetValue(comparation[0].Trim(), out string strval))
                                {
                                    comparation[1] = comparation[1].Substring(comparation[1].IndexOf('"') + 1, comparation[1].LastIndexOf('"') - 2);
                                    if (comparation[1] != strval) 
                                    {
                                        i = endif + 1;
                                    }
                                }
                                else if (Booleans.TryGetValue(comparation[0].Trim(), out bool bolval))
                                {
                                    comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                    if (comparation[1] != bolval.ToString().ToLower())
                                    {
                                        i = endif + 1;
                                    }
                                }
                                continue;
                            }
                            else if (line.Contains('!'))
                            {
                                comparation = line.Substring(4).Split('!');

                                if (Integers.TryGetValue(comparation[0].Trim(), out int intval))
                                {
                                    comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                    if (comparation[1] == intval.ToString())
                                    {
                                        i = endif + 1;
                                    }
                                }
                                else if (Strings.TryGetValue(comparation[0].Trim(), out string strval))
                                {
                                    comparation[1] = comparation[1].Substring(comparation[1].IndexOf('"') + 1, comparation[1].LastIndexOf('"') - 2);
                                    if (comparation[1] == strval)
                                    {
                                        i = endif + 1;
                                    }
                                }
                                else if (Booleans.TryGetValue(comparation[0].Trim(), out bool bolval))
                                {
                                    comparation[1] = comparation[1].Substring(1, comparation[1].Length - 2);
                                    if (comparation[1] == bolval.ToString().ToLower())
                                    {
                                        i = endif + 1;
                                    }
                                }
                                continue;
                            }
                        }

                        #endregion

                        #region System library

                        else if (SysLib && line.StartsWith("Stop"))
                        {
                            break;
                        }

                        else if (SysLib && line.StartsWith("Goto") && line.Contains(">>"))
                        {
                            string arg = line.Split('>')[2];

                            if (Integers.TryGetValue(arg, out int value))
                            {
                                arg = value.ToString();
                            }

                            i = Convert.ToInt32(line.Split('>')[2]) - 2;
                            continue;
                        }

                        else if (SysLib && line.StartsWith("Delay") && line.Contains(">>"))
                        {
                            string arg = line.Split('>')[2];

                            if (Integers.TryGetValue(arg, out int value))
                            {
                                arg = value.ToString();
                            }

                            Thread.Sleep(Convert.ToInt32(arg));
                            continue;
                        }

                        #endregion

                        #region Console library

                        else if (ConsoleLib && line.StartsWith("Write") && line.Contains(">>") || ConsoleLib && line.StartsWith("Print") && line.Contains(">>"))
                        {
                            string[] args = line.Split('>')[2].Trim().Split(',');

                            foreach (string arg in args)
                            {
                                string sub = arg.Trim();

                                if (arg.Contains("\""))
                                {
                                    Console.Write(sub.Substring(1, sub.Length - 2));
                                }
                                else if (Booleans.TryGetValue(sub, out bool bolval))
                                {
                                    Console.Write(bolval);
                                }
                                else if (Integers.TryGetValue(sub, out int intval))
                                {
                                    Console.Write(intval);
                                }
                                else if (Colors.TryGetValue(sub, out ConsoleColor colval))
                                {
                                    Console.Write(colval);
                                }
                                else if (Strings.TryGetValue(sub, out string strval))
                                {
                                    Console.Write(strval);
                                }
                                else
                                {
                                    Console.Write(sub);
                                }
                            }

                            if (line.StartsWith("Print"))
                            {
                                Console.Write('\n');
                            }
                            continue;
                        }

                        else if (ConsoleLib && line.StartsWith("Clear"))
                        {
                            Console.Clear();
                            continue;
                        }

                        else if (ConsoleLib && line.StartsWith("Read"))
                        {
                            Console.ReadKey(true);
                            continue;
                        }

                        else if (ConsoleLib && line.StartsWith("ReadLine"))
                        {
                            Console.ReadLine();
                            continue;
                        }

                        #endregion

                        #region IO library

                        else if (IOLib && line.StartsWith("MkFile") && line.Contains(">>"))
                        {
                            string sub = line.Split('>')[2].Trim();
                            sub = sub.Substring(1, sub.Length - 2);

                            if (Strings.TryGetValue(sub, out string strval))
                            {
                                sub = strval;
                            }

                            using (FileStream stream = new FileStream(sub, FileMode.Create))
                            {
                                stream.Close();
                            }
                            continue;
                        }

                        else if (IOLib && line.StartsWith("MkDir") && line.Contains(">>"))
                        {
                            string sub = line.Split('>')[2].Trim();
                            sub = sub.Substring(1, sub.Length - 2);

                            if (Strings.TryGetValue(sub, out string strval))
                            {
                                sub = strval;
                            }

                            Directory.CreateDirectory(sub);
                            continue;
                        }

                        else if (IOLib && line.StartsWith("WrFile") && line.Contains(">>"))
                        {
                            string[] args = line.Split('>')[2].Trim().Split(',');

                            if (Strings.TryGetValue(args[0].Replace("\"", ""), out string strval1))
                            {
                                args[0] = strval1;
                            }

                            if (Strings.TryGetValue(args[1].Remove(0, 1), out string strval2))
                            {
                                args[1] = strval2;
                            }

                            using (FileStream stream = new FileStream(args[0].Replace("\"", ""), FileMode.Open))
                            {
                                byte[] bytes;
                                if (args[1].Contains('"'))
                                {
                                    args[1] = args[1].Replace("\"", "");
                                    bytes = Encoding.UTF8.GetBytes(args[1].Remove(0, 1));
                                }
                                else
                                {
                                    bytes = Encoding.UTF8.GetBytes(args[1]);
                                }

                                stream.Write(bytes, 0, bytes.Length);
                            }
                        }

                        #endregion

                        #region 9xGL Library

                        else if (_9xGLLib && line.StartsWith("DrawString") && line.Contains(">>"))
                        {
                            string[] args = line.Split('>')[2].Trim().Split(',');

                            if (args.Length < 5)
                            {
                                HandleError("Error", "Argument underflow.");
                                break;
                            }
                            if (args.Length > 5)
                            {
                                HandleError("Error", "Argument overflow.");
                                break;
                            }

                            if (!Windows.TryGetValue(args[0].Trim(), out Window wndval))
                            {
                                HandleError("Error", "Unknown variable.");
                                break;
                            }

                            if (!Strings.TryGetValue(args[1].Trim(), out string strval))
                            {
                                if (args[1].Trim().Contains('"'))
                                {
                                    strval = args[1].Trim().Substring(1, args[1].Length - 3);
                                }
                                else
                                {
                                    HandleError("Error", "Unknown variable.");
                                    break;
                                }
                            }

                            if (!Integers.TryGetValue(args[2].Trim(), out int xval))
                            {
                                xval = Convert.ToInt32(args[2].Trim());
                            }

                            if (!Integers.TryGetValue(args[3].Trim(), out int yval))
                            {
                                yval = Convert.ToInt32(args[3].Trim());
                            }

                            if (!Colors.TryGetValue(args[4].Trim(), out Color colval))
                            {
                                if (StringToConsoleColor.TryGetValue(args[4].Trim(), out Color colval2))
                                {
                                    colval = colval2;
                                }
                                else
                                {
                                    HandleError("Syntax Error", "Unknown Color.");
                                }
                            }

                            wndval.Contents.DrawString(xval, yval, strval, Fonts.Font_1x, colval);
                        }

                        else if (_9xGLLib && line.StartsWith("SetWindowPos") && line.Contains(">>"))
                        {
                            string[] args = line.Split('>')[2].Trim().Split(',');

                            if (args.Length < 3)
                            {
                                HandleError("Error", "Argument underflow.");
                                break;
                            }
                            if (args.Length > 3)
                            {
                                HandleError("Error", "Argument overflow.");
                                break;
                            }

                            if (!Windows.TryGetValue(args[0].Trim(), out Window wndval))
                            {
                                HandleError("Error", "Unknown variable.");
                                break;
                            }

                            if (Integers.TryGetValue(args[1].Trim(), out int xval))
                            {
                                wndval.X = xval;
                            }
                            else
                            {
                                wndval.X = Convert.ToInt32(args[1].Trim());
                            }

                            if (Integers.TryGetValue(args[2].Trim(), out int yval))
                            {
                                wndval.Y = yval;
                            }
                            else
                            {
                                wndval.Y = Convert.ToInt32(args[2].Trim());
                            }

                            wndval.X = xval;
                            wndval.Y = yval;
                        }

                        #endregion

                        #region GoOS Library

                        else if (GoOSLib && line.StartsWith("EnableKillingSystemTasks"))
                        {
                            GUI.Apps.TaskManager.pko = true;
                        }

                        else if (GoOSLib && line.StartsWith("DisableKillingSystemTasks"))
                        {
                            GUI.Apps.TaskManager.pko = false;
                        }

                        else if (GoOSLib && line.StartsWith("RegProg") && line.Contains(">>"))
                        {
                            string[] args = line.Split('>')[2].Trim().Split(',');

                            if (args.Length < 1)
                            {
                                HandleError("Error", "Argument underflow.");
                                break;
                            }
                            if (args.Length > 1)
                            {
                                HandleError("Error", "Argument overflow.");
                                break;
                            }

                            if (!Strings.TryGetValue(args[0].Trim(), out string strval))
                            {
                                if (args[0].Trim().Contains('"'))
                                {
                                    strval = args[0].Trim().Substring(1, args[0].Length - 3);
                                }
                                else
                                {
                                    HandleError("Error", "Unknown variable.");
                                    break;
                                }
                            }

                            if (!Directory.Exists(@"0:\content\prf\" + strval + @"\"))
                            {
                                Directory.CreateDirectory(@"0:\content\prf\" + strval + @"\");
                            }
                        }

                        #endregion

                        else
                        {
                            HandleError("Syntax Error", "Unknown function.\nMaybe try importing a library?");
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleError("9xCode", "Unhandled exception in runtime:\n" + ex.Message);
                    }
                }
            }

            while (WindowManager.ContainsAWindow(new List<Window>(Windows.Values)))
            {
                WindowManager.Update();
            }
        }

        private static void HandleError(string Title, string Message)
        {
            Dialogue.Show(Title, Message, null, WindowManager.errorIcon);
            Interpreting = false;
        }
    }
}