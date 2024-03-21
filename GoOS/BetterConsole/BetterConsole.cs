﻿using System;
using System.Collections.Generic;
using Cosmos.Core.Memory;
using Cosmos.System;
using GoOS.GUI;
using GoOS.Themes;
using GoGL.Graphics;
using GoOS.GUI.Apps.Settings;
using static GoOS.Resources;

/// <summary>
/// <see cref="BetterConsole"/> class
/// </summary>
public static class BetterConsole
{
    /* The canvas for the console */
    public static Canvas Canvas;

    /* Character width and height */
    public const ushort CharWidth = 8;
    public const ushort CharHeight = 16;

    private static readonly List<string> MenuOptions = new()
    {
        "Launch Settings",
        "Reboot"
    };

    public static bool ConsoleMode = false;

    public static bool Visible = false;

    /// <summary>
    /// The X position of the cursor
    /// </summary>
    public static int CursorLeft = 0;

    /// <summary>
    /// The Y position of the cursor
    /// </summary>
    public static int CursorTop = 0;

    /// <summary>
    /// The width of the <see cref="BetterConsole"/>
    /// </summary>
    public static ushort WindowWidth = 0;

    /// <summary>
    /// The height of the <see cref="BetterConsole"/>
    /// </summary>
    public static ushort WindowHeight = 0;

    /// <summary>
    /// The foreground color of the <see cref="BetterConsole"/>
    /// </summary>
    public static Color ForegroundColor = ConsoleColorEx.White;

    /// <summary>
    /// The background color of the <see cref="BetterConsole"/>
    /// </summary>
    public static Color BackgroundColor = ConsoleColorEx.Black;

    /// <summary>
    /// Determines if the cursor is visible
    /// </summary>
    public static bool CursorVisible = true;

    /// <summary>
    /// Determines if every command calls Render() when finishes
    /// </summary>
    public static bool DoubleBufferedMode = false;

    /// <summary>
    /// The queue of key events to send to the <see cref="BetterConsole"/>
    /// </summary>
    public static Queue<KeyEvent> KeyBuffer = new Queue<KeyEvent>();

    public static string Title = "GTerm";

    /// <summary>
    /// Initializes the <see cref="BetterConsole">
    /// </summary>
    /// <param name="videoWidth">The width of the canvas</param>
    /// <param name="videoHeight">The height of the canvas</param>
    public static void Init(ushort width, ushort height)
    {
        Canvas = new Canvas(width, height);
        WindowWidth = (ushort)(width / CharWidth);
        WindowHeight = (ushort)(height / CharHeight);
        Canvas.Clear();
    }

    /// <summary>
    /// Clears the console
    /// </summary>
    public static void Clear(bool render = true)
    {
        Canvas.Clear(BackgroundColor);
        CursorLeft = 0;
        CursorTop = 0;
        if (render || !DoubleBufferedMode)
            Render();
    }

    /// <summary>
    /// Renders the <see cref="BetterConsole">
    /// </summary>
    public static void Render()
    {
        WindowManager.Update();
    }

    /// <summary>
    /// Writes a string to the <see cref="BetterConsole"/>
    /// </summary>
    /// <param name="text">The string to write</param>
    public static void Write(object text, bool quick = false)
    {
        foreach (char c in text.ToString())
        {
            Newline();

            if (c == '\n')
            {
                CursorLeft = 0;
                CursorTop++;
            }
            else
            {
                PutChar(c, CursorLeft, CursorTop, quick);
                CursorLeft++;
            }
        }

        if (!DoubleBufferedMode)
            Render();
    }

    /// <summary>
    /// Writes a string to the <see cref="BetterConsole"/>
    /// </summary>
    /// <param name="text">The string to write</param>
    public static void WriteLine(object text = null, bool quick = false) => Write(text + "\n", quick);

    /// <summary>
    /// Reads input from the user
    /// </summary>
    /// <param name="intercept">Print the key pressed</param>
    /// <returns>The key pressed</returns>
    public static ConsoleKeyInfo ReadKey(bool intercept = true)
    {
        while (true)
        {
            if (CursorVisible)
            {
                Canvas.DrawString(CursorLeft * CharWidth, CursorTop * CharHeight, '_'.ToString(), Font_1x, ForegroundColor);
            }

            var keyPressed = KeyBuffer.TryDequeue(out var key);
            if (keyPressed)
            {
                if (intercept == false)
                {
                    Write(key.KeyChar);
                }

                bool xShift = (key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift;
                bool xAlt = (key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt;
                bool xControl = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;

                return new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey(), xShift, xAlt, xControl);
            }
            else
            {
                WindowManager.Update();
            }

            if (CursorVisible)
            {
                // Just to be safe
                Canvas.DrawString((CursorLeft - 1) * CharWidth, CursorTop * CharHeight, '_'.ToString(), Font_1x, Color.Black);
                Canvas.DrawString((CursorLeft + 1) * CharWidth, CursorTop * CharHeight, '_'.ToString(), Font_1x, Color.Black);
                Canvas.DrawString(CursorLeft * CharWidth, (CursorTop - 1) * CharHeight, '_'.ToString(), Font_1x, Color.Black);
                Canvas.DrawString(CursorLeft * CharWidth, (CursorTop + 1) * CharHeight, '_'.ToString(), Font_1x, Color.Black);
            }
        }
    }

    private static string lastInput = string.Empty;

    /// <summary>
    /// Gets input from the user
    /// </summary>
    /// <returns>The text that the user typed</returns>
    public static string ReadLine()
    {
        int startCursorLeft = CursorLeft, startY = CursorTop;
        string returnValue = string.Empty;

        bool reading = true;
        while (reading)
        {
            if (CursorVisible)
            {
                PutChar('_', CursorLeft, CursorTop, true);
                Render();
            }

            var keyPressed = KeyBuffer.TryDequeue(out var key);
            if (keyPressed)
            {
                switch (key.Key)
                {
                    case ConsoleKeyEx.Enter:
                        PutChar(' ', CursorLeft, CursorTop);
                        CursorLeft = 0;
                        CursorTop++;
                        Newline();
                        lastInput = returnValue;
                        reading = false;
                        break;

                    case ConsoleKeyEx.Backspace:
                        if (!(CursorLeft == startCursorLeft && CursorTop == startY))
                        {
                            if (CursorLeft == 0)
                            {
                                PutChar(' ', CursorLeft, CursorTop); // Erase the cursor
                                CursorTop--;
                                CursorLeft = Canvas.Width / CharWidth - 1;
                                PutChar(' ', CursorLeft, CursorTop); // Erase the actual character
                            }
                            else
                            {
                                PutChar(' ', CursorLeft, CursorTop); // Erase the cursor
                                CursorLeft--;
                                PutChar(' ', CursorLeft, CursorTop); // Erase the actual character
                            }

                            returnValue =
                                returnValue.Remove(returnValue.Length - 1); // Remove the last character of the string
                        }

                        break;
                    case ConsoleKeyEx.Tab:
                        Write(new string(' ', 4));
                        returnValue += new string(' ', 4);
                        break;

                    case ConsoleKeyEx.UpArrow:
                        SetCursorPosition(startCursorLeft, startY);
                        Write(new string(' ', returnValue.Length));
                        SetCursorPosition(startCursorLeft, startY);
                        Write(lastInput);
                        returnValue = lastInput;
                        break;

                    default:
                        if (KeyboardManager.ControlPressed)
                        {
                            if (key.Key == ConsoleKeyEx.G)
                            {
                                string collected = Heap.Collect() + " items collected";
                                Canvas.DrawString(Canvas.Width - (collected.Length * 8) - 8, Canvas.Height - 32, collected, Font_1x, ThemeManager.WindowText);
                                Write(returnValue);
                            }
                            else if (key.Key == ConsoleKeyEx.L)
                            {
                                Clear();
                                returnValue = string.Empty;
                                reading = false;
                            }
                            else if (KeyboardManager.ShiftPressed && key.Key == ConsoleKeyEx.E)
                            {
                                Write("> ");
                                string input = ReadLine();
                                if (input == "e015")
                                {
                                    Clear();
                                    Canvas.DrawImage(0, 0, easterEgg, false);
                                    ReadKey(true);
                                    Clear();
                                }
                            }
                            else if (ConsoleMode && KeyboardManager.AltPressed && key.Key == ConsoleKeyEx.Delete)
                            {
                                int selected = 0;

                                Clear();
                                Canvas.DrawRectangle((Canvas.Width / 2) - (144 / 2) + 0,
                                    (Canvas.Height / 2) - ((MenuOptions.Count + 4) * 16 / 2) + 0, 144,
                                    Convert.ToUInt16((MenuOptions.Count + 4) * 16), 0, ThemeManager.WindowBorder);
                                Canvas.DrawRectangle((Canvas.Width / 2) - (144 / 2) + 1,
                                    (Canvas.Height / 2) - ((MenuOptions.Count + 4) * 16 / 2) + 1, 144,
                                    Convert.ToUInt16((MenuOptions.Count + 4) * 16), 0, ThemeManager.WindowBorder);

                                Refresh:
                                if (selected > MenuOptions.Count - 1)
                                {
                                    selected = 0;
                                }

                                if (selected < 0)
                                {
                                    selected = MenuOptions.Count - 1;
                                }

                                for (int i = 0; i < MenuOptions.Count; i++)
                                {
                                    SetCursorPosition((WindowWidth / 2) - (15 / 2) - 1,
                                        (WindowHeight / 2) - 1 + (i * 2));
                                    if (i == selected)
                                    {
                                        ForegroundColor = ThemeManager.Background;
                                        BackgroundColor = ThemeManager.WindowText;
                                    }
                                    else
                                    {
                                        ForegroundColor = ThemeManager.WindowText;
                                        BackgroundColor = ThemeManager.Background;
                                    }

                                    Write(MenuOptions[i]);
                                }

                                var key2 = KeyboardManager.ReadKey();
                                switch (key2.Key)
                                {
                                    case ConsoleKeyEx.Escape:
                                        break;

                                    case ConsoleKeyEx.Enter:
                                        if (MenuOptions[selected] == MenuOptions[0])
                                            WindowManager.AddWindow(new Frame());
                                        else if (MenuOptions[selected] == MenuOptions[1])
                                            Power.Reboot();
                                        break;

                                    case ConsoleKeyEx.UpArrow:
                                        selected--;
                                        goto Refresh;

                                    case ConsoleKeyEx.DownArrow:
                                        selected++;
                                        goto Refresh;

                                    default:
                                        goto Refresh;
                                }

                                Clear();
                                GoOS.Kernel.DrawPrompt();
                            }
                        }
                        else
                        {
                            Write(key.KeyChar.ToString());
                            Newline();
                            returnValue += key.KeyChar;
                        }

                        break;
                }

                Render();
            }
            else
            {
                WindowManager.Update();
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Set the cursor position of the <see cref="BetterConsole"/>
    /// </summary>
    /// <param name="CursorLeft">The CursorLeft position of the cursor</param>
    /// <param name="y">The Y position of the cursor</param>
    public static void SetCursorPosition(int x, int y)
    {
        CursorLeft = x;
        CursorTop = y;
    }

    public static (int Left, int Top) GetCursorPosition()
    {
        return (CursorLeft, CursorTop);
    }

    public static void Beep(uint freq = 800, uint duration = 125)
    {
        PCSpeaker.Beep(freq, duration);
    }

    #region Private functions

    private static void Newline()
    {
        if (CursorLeft >= Canvas.Width / CharWidth)
        {
            CursorLeft = 0;
            CursorTop++;
        }

        if (CursorTop >= Canvas.Height / CharHeight)
        {
            Canvas.DrawImage(0, -CharHeight, Canvas, false);
            Canvas.DrawFilledRectangle(0, Canvas.Height - CharHeight, Canvas.Width, CharHeight, 0, Color.Black);
            CursorLeft = 0;
            CursorTop = (Canvas.Height / CharHeight) - 1;

            if (!DoubleBufferedMode)
                Render();

            Heap.Collect();
        }
    }

    public static void PutChar(char c, int CursorLeft, int y, bool quick = false)
    {
        if (!quick)
            Canvas.DrawFilledRectangle(CursorLeft * CharWidth, y * CharHeight,
                Convert.ToUInt16(CharWidth + (CharWidth / 8)), CharHeight, 0, BackgroundColor);
        if (c != ' ')
            Canvas.DrawString(CursorLeft * CharWidth, y * CharHeight, c.ToString(), Font_1x, ForegroundColor);
    }

    #endregion
}

/// <summary>
/// <see cref="ConsoleColorEx"/> class
/// </summary>
public static class ConsoleColorEx
{
    public static readonly Color Black = new Color(0, 0, 0);
    public static readonly Color DarkBlue = new Color(0, 0, 170);
    public static readonly Color DarkGreen = new Color(0, 170, 0);
    public static readonly Color DarkCyan = new Color(0, 170, 170);
    public static readonly Color DarkRed = new Color(170, 0, 0);
    public static readonly Color DarkMagenta = new Color(170, 0, 170);
    public static readonly Color DarkYellow = new Color(170, 85, 0);
    public static readonly Color Gray = new Color(170, 170, 170);
    public static readonly Color DarkGray = new Color(85, 85, 85);
    public static readonly Color Blue = new Color(85, 85, 255);
    public static readonly Color Green = new Color(85, 255, 85);
    public static readonly Color Cyan = new Color(85, 255, 255);
    public static readonly Color Red = new Color(255, 85, 85);
    public static readonly Color Magenta = new Color(255, 85, 255);
    public static readonly Color Yellow = new Color(255, 255, 85);
    public static readonly Color White = new Color(255, 255, 255);
}
