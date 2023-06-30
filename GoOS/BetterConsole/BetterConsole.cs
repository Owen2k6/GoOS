using System;
using Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Hardware.GPU;
using PrismAPI.Graphics;
using PrismAPI.Graphics.Fonts;

/// <summary>
/// <see cref="BetterConsole"/> class
/// </summary>
public static class BetterConsole
{
    #region Graphics variables
    [ManifestResourceStream(ResourceName = "GoOS.BetterConsole.Font_1x.btf")] private static byte[] rawFont;
    private static Font font;
    public static Display Canvas;
    private static ushort charWidth = 8, charHeight = 16;
    #endregion

    /// <summary>
    /// The X position of the cursor
    /// </remarks>
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
    public static Color ForegroundColor = Color.White;
    /// <summary>
    /// The background color of the <see cref="BetterConsole"/>
    /// </summary>
    public static Color BackgroundColor = Color.Black;
    /// <summary>
    /// Determines if the cursor is visible
    /// </summary>
    public static bool CursorVisible = true;
    /// <summary>
    /// Determines if every command calls Render() when finishes
    /// </summary>
    public static bool DoubleBufferedMode = false;

    /// <summary>
    /// Initializes the <see cref="BetterConsole">
    /// </summary>
    /// <param name="videoWidth">The width of the canvas</param>
    /// <param name="videoHeight">The height of the canvas</param>
    public static void Init(ushort width, ushort height)
    {
        font = new Font(rawFont, charHeight);
        Canvas = Display.GetDisplay(width, height);
        WindowWidth = Convert.ToUInt16(width / charWidth);
        WindowHeight = Convert.ToUInt16(height / charHeight);
        Canvas.Clear();
    }

    /// <summary>
    /// Clears the console
    /// </summary>
    public static void Clear(bool render = true)
    {
        Canvas.Clear(BackgroundColor);
        CursorLeft = 0; CursorTop = 0;
        if (render || !DoubleBufferedMode)
            Render();
        Cosmos.Core.Memory.Heap.Collect();
    }

    /// <summary>
    /// Renders the <see cref="BetterConsole">
    /// </summary>
    public static void Render() => Canvas.Update();

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
    /// <param name="intercept">Print the key that the user pressed</param>
    /// <returns>The key that the user pressed</returns>
    public static ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        ConsoleKeyInfo key = System.Console.ReadKey();
        if (!intercept) Write(key.KeyChar);
        return key;
    }

    /// <summary>
    /// Gets input from the user
    /// </summary>
    /// <returns>The teCursorLeftt that the user typed</returns>
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

            var key = KeyboardManager.ReadKey();
            switch (key.Key)
            {
                case ConsoleKeyEx.Enter:
                    PutChar(' ', CursorLeft, CursorTop);
                    CursorLeft = 0; CursorTop++;
                    Newline();
                    reading = false;
                    break;

                case ConsoleKeyEx.Backspace:
                    if (!(CursorLeft == startCursorLeft && CursorTop == startY))
                    {
                        if (CursorLeft == 0)
                        {
                            PutChar(' ', CursorLeft, CursorTop); // Erase the cursor
                            CursorTop--;
                            CursorLeft = Canvas.Width / charWidth - 1;
                            PutChar(' ', CursorLeft, CursorTop); // Erase the actual character
                        }
                        else
                        {
                            PutChar(' ', CursorLeft, CursorTop); // Erase the cursor
                            CursorLeft--;
                            PutChar(' ', CursorLeft, CursorTop); // Erase the actual character
                        }
                        returnValue = returnValue.Remove(returnValue.Length - 1); // Remove the last character of the string
                    }
                    break;

                case ConsoleKeyEx.Tab:
                    Write(new string(' ', 4));
                    returnValue += new string(' ', 4);
                    break;

                default:
                    if (KeyboardManager.ControlPressed && key.Key == ConsoleKeyEx.L)
                    {
                        Clear();
                        returnValue = string.Empty;
                        reading = false;
                    }
                    else if (KeyboardManager.ControlPressed)
                    {
                        if (KeyboardManager.AltPressed)
                            if (key.Key == ConsoleKeyEx.Delete)
                                Cosmos.Core.CPU.Reboot();
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
        if (CursorLeft >= Canvas.Width / charWidth)
        {
            CursorLeft = 0;
            CursorTop++;
        }
        if (CursorTop >= Canvas.Height / charHeight)
        {
            Canvas.DrawFilledRectangle(0, 0, Canvas.Width, charHeight, 0, Color.Black);
            for (int y = charHeight; y < Canvas.Height; y++)
            {
                for (int CursorLeft = 0; CursorLeft < Canvas.Width; CursorLeft++)
                {
                    Canvas[CursorLeft, y - charHeight] = Canvas[CursorLeft, y];
                }
            }
            Canvas.DrawFilledRectangle(0, Canvas.Height - charHeight, Canvas.Width, charHeight, 0, Color.Black);
            CursorLeft = 0; CursorTop = (Canvas.Height / charHeight) - 1;
            if (!DoubleBufferedMode)
                Render();
            Cosmos.Core.Memory.Heap.Collect();
        }
    }

    private static void PutChar(char c, int CursorLeft, int y, bool quick = false)
    {
        if (!quick)
            Canvas.DrawFilledRectangle(CursorLeft * charWidth, y * charHeight, Convert.ToUInt16(charWidth + (charWidth / 8)), charHeight, 0, BackgroundColor); //yes this is correct
        if (c != ' ')
            Canvas.DrawString(CursorLeft * charWidth, y * charHeight, c.ToString(), font, ForegroundColor);
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
