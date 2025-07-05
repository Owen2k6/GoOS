using System;
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

    // Maximum size for key buffer to prevent memory issues
    private const int MaxKeyBufferSize = 256;

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
    /// The foreground colour of the <see cref="BetterConsole"/>
    /// </summary>
    public static Color ForegroundColor = ConsoleColorEx.White;

    /// <summary>
    /// The background colour of the <see cref="BetterConsole"/>
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

    private static string lastInput = string.Empty;
    private static byte scrollCounter = 0;

    /// <summary>
    /// Initialises the <see cref="BetterConsole">
    /// </summary>
    /// <param name="width">The width of the canvas</param>
    /// <param name="height">The height of the canvas</param>
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
    /// <param name="quick">Whether to use quick rendering</param>
    public static void Write(object text, bool quick = false)
    {
        if (text == null) return;

        string textStr = text.ToString();

        foreach (char c in textStr)
        {
            // Handle newlines
            if (c == '\n')
            {
                CursorLeft = 0;
                CursorTop++;
                CheckNewline();
                continue;
            }

            // Check if we need to wrap to next line
            if (CursorLeft >= WindowWidth)
            {
                CursorLeft = 0;
                CursorTop++;
                CheckNewline();
            }

            // Draw the character
            PutChar(c, CursorLeft, CursorTop, quick);
            CursorLeft++;
        }

        if (!DoubleBufferedMode)
            Render();
    }

    /// <summary>
    /// Writes a string to the <see cref="BetterConsole"/> followed by a newline
    /// </summary>
    /// <param name="text">The string to write</param>
    /// <param name="quick">Whether to use quick rendering</param>
    public static void WriteLine(object text = null, bool quick = false) =>
        Write((text ?? "") + "\n", quick);

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
                Canvas.DrawString(CursorLeft * CharWidth, CursorTop * CharHeight, "_", Font_1x, ForegroundColor);
                Render();
            }

            // Check for keypress
            if (KeyBuffer.Count > 0)
            {
                var key = KeyBuffer.Dequeue();

                // Clear cursor
                Canvas.DrawFilledRectangle(CursorLeft * CharWidth, CursorTop * CharHeight,
                    CharWidth, CharHeight, 0, BackgroundColor);

                if (!intercept)
                {
                    Write(key.KeyChar);
                }

                bool xShift = (key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift;
                bool xAlt = (key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt;
                bool xControl = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;

                return new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey(), xShift, xAlt, xControl);
            }

            // Update the display
            WindowManager.Update();

            // Limit key buffer size to prevent memory issues
            while (KeyBuffer.Count > MaxKeyBufferSize)
            {
                KeyBuffer.Dequeue();
            }
        }
    }

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
                PutChar('_', CursorLeft, CursorTop);
                Render();
            }

            // Check for keypress
            if (KeyBuffer.Count > 0)
            {
                var key = KeyBuffer.Dequeue();

                // Clear cursor
                PutChar(' ', CursorLeft, CursorTop);

                switch (key.Key)
                {
                    case ConsoleKeyEx.Enter:
                        CursorLeft = 0;
                        CursorTop++;
                        CheckNewline();
                        lastInput = returnValue;
                        reading = false;
                        break;

                    case ConsoleKeyEx.Backspace:
                        if (CursorLeft > startCursorLeft || CursorTop > startY)
                        {
                            if (CursorLeft == 0 && CursorTop > startY)
                            {
                                // At beginning of line - move to end of previous line
                                CursorTop--;
                                CursorLeft = WindowWidth - 1;
                                PutChar(' ', CursorLeft, CursorTop);
                            }
                            else if (CursorLeft > 0)
                            {
                                // Normal backspace within a line
                                CursorLeft--;
                                PutChar(' ', CursorLeft, CursorTop);
                            }

                            // Remove the last character from the string
                            if (returnValue.Length > 0)
                            {
                                returnValue = returnValue.Remove(returnValue.Length - 1);
                            }
                        }
                        break;

                    case ConsoleKeyEx.Tab:
                        Write(new string(' ', 4));
                        returnValue += new string(' ', 4);
                        break;

                    case ConsoleKeyEx.UpArrow:
                        if (!string.IsNullOrEmpty(lastInput))
                        {
                            // Clear current input
                            SetCursorPosition(startCursorLeft, startY);
                            for (int i = 0; i < returnValue.Length; i++)
                            {
                                PutChar(' ', CursorLeft, CursorTop);
                                CursorLeft++;
                                if (CursorLeft >= WindowWidth)
                                {
                                    CursorLeft = 0;
                                    CursorTop++;
                                }
                            }

                            // Write previous input
                            SetCursorPosition(startCursorLeft, startY);
                            Write(lastInput);
                            returnValue = lastInput;
                        }
                        break;

                    default:
                        if (KeyboardManager.ControlPressed)
                        {
                            if (key.Key == ConsoleKeyEx.G)
                            {
                                string collected = Heap.Collect() + " items collected";
                                Canvas.DrawString(Canvas.Width - (collected.Length * 8) - 8, Canvas.Height - 32,
                                    collected, Font_1x, ThemeManager.WindowText);
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
                                ShowConsoleMenu();
                                GoOS.Kernel.DrawPrompt();
                            }
                        }
                        else
                        {
                            Write(key.KeyChar.ToString());
                            returnValue += key.KeyChar;
                        }
                        break;
                }

                Render();
            }
            else
            {
                WindowManager.Update();

                // Limit key buffer size to prevent memory issues
                while (KeyBuffer.Count > MaxKeyBufferSize)
                {
                    KeyBuffer.Dequeue();
                }
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Set the cursor position of the <see cref="BetterConsole"/>
    /// </summary>
    /// <param name="x">The X position of the cursor</param>
    /// <param name="y">The Y position of the cursor</param>
    public static void SetCursorPosition(int x, int y)
    {
        // Ensure we don't position outside of console bounds
        x = Math.Clamp(x, 0, WindowWidth - 1);
        y = Math.Clamp(y, 0, WindowHeight - 1);

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

    private static void CheckNewline()
    {
        if (CursorTop >= WindowHeight)
        {
            // Scroll the screen up
            Canvas.DrawImage(0, -CharHeight, Canvas, false);
            Canvas.DrawFilledRectangle(0, Canvas.Height - CharHeight, Canvas.Width, CharHeight, 0, BackgroundColor);
            CursorTop = WindowHeight - 1;

            if (!DoubleBufferedMode)
                Render();

            // Collect memory periodically on screen scroll to reduce memory pressure
            scrollCounter++;
            if (scrollCounter >= 3)
            {
                Heap.Collect();
                scrollCounter = 0;
            }
        }
    }

    private static void ShowConsoleMenu()
    {
        int selected = 0;

        Clear();
        DrawMenuBorder();

        while (true)
        {
            // Handle selection wrapping
            selected = (selected + MenuOptions.Count) % MenuOptions.Count;

            // Draw menu options
            for (int i = 0; i < MenuOptions.Count; i++)
            {
                SetCursorPosition((WindowWidth / 2) - (15 / 2) - 1,
                    (WindowHeight / 2) - 1 + (i * 2));

                // Highlight selected option
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

            // Handle key input
            var key = KeyboardManager.ReadKey();
            switch (key.Key)
            {
                case ConsoleKeyEx.Escape:
                    Clear();
                    return;

                case ConsoleKeyEx.Enter:
                    if (selected == 0)
                    {
                        WindowManager.AddWindow(new Frame());
                    }
                    else if (selected == 1)
                    {
                        Power.Reboot();
                    }
                    Clear();
                    return;

                case ConsoleKeyEx.UpArrow:
                    selected--;
                    break;

                case ConsoleKeyEx.DownArrow:
                    selected++;
                    break;
            }
        }
    }

    private static void DrawMenuBorder()
    {
        ushort menuWidth = 144;
        ushort menuHeight = (ushort)((MenuOptions.Count + 4) * 16);
        ushort menuX = (ushort)((Canvas.Width / 2) - (menuWidth / 2));
        ushort menuY = (ushort)((Canvas.Height / 2) - (menuHeight / 2));

        // Draw border with double line for emphasis
        Canvas.DrawRectangle(menuX, menuY, menuWidth, menuHeight, 0, ThemeManager.WindowBorder);
        Canvas.DrawRectangle((ushort)(menuX + 1), (ushort)(menuY + 1), (ushort)(menuWidth - 2), (ushort)(menuHeight - 2), 0, ThemeManager.WindowBorder);
    }

    public static void PutChar(char c, int x, int y, bool quick = false)
    {
        // Ensure we're within bounds
        if (x < 0 || x >= WindowWidth || y < 0 || y >= WindowHeight)
            return;

        // Always clear the background first to ensure visibility
        Canvas.DrawFilledRectangle(x * CharWidth, y * CharHeight,
            CharWidth, CharHeight, 0, BackgroundColor);

        // Draw the character if it's not a space
        if (c != ' ')
            Canvas.DrawString(x * CharWidth, y * CharHeight, c.ToString(), Font_1x, ForegroundColor);
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