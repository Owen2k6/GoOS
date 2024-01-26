using System;
using System.Collections.Generic;
using Cosmos.Core.Memory;
using Cosmos.System;
using GoOS.GUI;
using GoOS.Themes;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using GoGL.Graphics.Fonts;

/// <summary>
/// <see cref="VMBetterConsole"/> class
/// </summary>
public class VMBetterConsole //Shitter Console
{
    /* The raw global font */
    [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_1x.btf")]
    public byte[] rawFont;

    /* The credits easter egg */
    [ManifestResourceStream(ResourceName = "GoOS.Resources.Credits05.bmp")]
    private byte[] easterEgg;

    /* The global font */
    public Font font;

    /* The canvas for the console */
    public Canvas Canvas;

    /* Character width and height */
    public ushort charWidth = 8, charHeight = 16;

    /*private List<string> menuOptions = new()
    {
        "Launch Settings",
        "Reboot"
    };*/

    public bool ConsoleMode = false;

    public bool Visible = false;

    /// <summary>
    /// The X position of the cursor
    /// </remarks>
    public int CursorLeft = 0;

    /// <summary>
    /// The Y position of the cursor
    /// </summary>
    public int CursorTop = 0;

    /// <summary>
    /// The width of the <see cref="BetterConsole"/>
    /// </summary>
    public ushort WindowWidth = 0;

    /// <summary>
    /// The height of the <see cref="BetterConsole"/>
    /// </summary>
    public ushort WindowHeight = 0;

    /// <summary>
    /// The foreground color of the <see cref="BetterConsole"/>
    /// </summary>
    public Color ForegroundColor = Color.White;

    /// <summary>
    /// The background color of the <see cref="BetterConsole"/>
    /// </summary>
    public Color BackgroundColor = Color.Black;

    /// <summary>
    /// Determines if the cursor is visible
    /// </summary>
    public bool CursorVisible = true;

    /// <summary>
    /// Determines if every command calls Render() when finishes
    /// </summary>
    public bool DoubleBufferedMode = false;

    /// <summary>
    /// The queue of key events to send to the <see cref="BetterConsole"/>
    /// </summary>
    public Queue<KeyEvent> KeyBuffer = new Queue<KeyEvent>();

    /// <summary>
    /// Initializes the <see cref="BetterConsole">
    /// </summary>
    /// <param name="videoWidth">The width of the canvas</param>
    /// <param name="videoHeight">The height of the canvas</param>
    public VMBetterConsole(ushort width, ushort height)
    {
        font = new Font(rawFont, charHeight);
        //Canvas = Display.GetDisplay(width, height);
        Canvas = new Canvas(width, height);
        WindowWidth = Convert.ToUInt16(width / charWidth);
        WindowHeight = Convert.ToUInt16(height / charHeight);
        Canvas.Clear();
    }

    /// <summary>
    /// Clears the console
    /// </summary>
    public void Clear(bool render = true)
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
    public void Render()
    {
        WindowManager.Update();
    }

    /// <summary>
    /// Writes a string to the <see cref="BetterConsole"/>
    /// </summary>
    /// <param name="text">The string to write</param>
    public void Write(object text, bool quick = false)
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
    public void WriteLine(object text = null, bool quick = false) => Write(text + "\n", quick);

    /// <summary>
    /// Reads input from the user
    /// </summary>
    /// <param name="intercept">Print the key pressed</param>
    /// <returns>The key pressed</returns>
    public ConsoleKeyInfo ReadKey(bool intercept = true)
    {
        while (true)
        {
            if (CursorVisible)
            {
                Canvas.DrawString(CursorLeft * charWidth, CursorTop * charHeight, '_'.ToString(), font, ForegroundColor);
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
                Canvas.DrawString((CursorLeft - 1) * charWidth, CursorTop * charHeight, '_'.ToString(), font, Color.Black);
                Canvas.DrawString((CursorLeft + 1) * charWidth, CursorTop * charHeight, '_'.ToString(), font, Color.Black);
            }
        }
    }

    /// <summary>
    /// Gets input from the user
    /// </summary>
    /// <returns>The teCursorLeftt that the user typed</returns>
    public string ReadLine()
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

                            returnValue =
                                returnValue.Remove(returnValue.Length - 1); // Remove the last character of the string
                        }

                        break;
                    case ConsoleKeyEx.Tab:
                        Write(new string(' ', 4));
                        returnValue += new string(' ', 4);
                        break;

                    default:
                        if (KeyboardManager.ControlPressed)
                        {
                            if (key.Key == ConsoleKeyEx.G)
                            {
                                string collected = Heap.Collect() + " items collected";
                                //Init(Canvas.Width, Canvas.Height);
                                Canvas.DrawString(Canvas.Width - (collected.Length * 8) - 8, Canvas.Height - 32,
                                    collected, font, ThemeManager.WindowText);
                                // SetCursorPosition(0, 0);
                                // GoOS.Kernel.DrawPrompt();
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
                                    Canvas.DrawImage(0, 0, Image.FromBitmap(easterEgg, false), false);
                                    //Canvas.Update(); it stopped working?
                                    ReadKey(true);
                                    Clear();
                                }
                                else
                                {
                                    Write("Nope");
                                }
                            }
                            /*else if (KeyboardManager.AltPressed && key.Key == ConsoleKeyEx.Delete)
                            {
                                int selected = 0;

                                Clear();
                                Canvas.DrawRectangle((Canvas.Width / 2) - (144 / 2) + 0,
                                    (Canvas.Height / 2) - ((menuOptions.Count + 4) * 16 / 2) + 0, 144,
                                    Convert.ToUInt16((menuOptions.Count + 4) * 16), 0, ThemeManager.WindowBorder);
                                Canvas.DrawRectangle((Canvas.Width / 2) - (144 / 2) + 1,
                                    (Canvas.Height / 2) - ((menuOptions.Count + 4) * 16 / 2) + 1, 144,
                                    Convert.ToUInt16((menuOptions.Count + 4) * 16), 0, ThemeManager.WindowBorder);

                             Refresh:
                                if (selected > menuOptions.Count - 1)
                                {
                                    selected = 0;
                                }

                                if (selected < 0)
                                {
                                    selected = menuOptions.Count - 1;
                                }

                                for (int i = 0; i < menuOptions.Count; i++)
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

                                    Write(menuOptions[i]);
                                }

                                var key2 = KeyboardManager.ReadKey();
                                switch (key2.Key)
                                {
                                    case ConsoleKeyEx.Escape:
                                        break;

                                    case ConsoleKeyEx.Enter:
                                        if (menuOptions[selected] == menuOptions[0])
                                            ControlPanel.Launch();
                                        else if (menuOptions[selected] == menuOptions[1])
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
                            }*/
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
    public void SetCursorPosition(int x, int y)
    {
        CursorLeft = x;
        CursorTop = y;
    }

    public (int Left, int Top) GetCursorPosition()
    {
        return (CursorLeft, CursorTop);
    }

    public void Beep(uint freq = 800, uint duration = 125)
    {
        PCSpeaker.Beep(freq, duration);
    }

    #region Private functions

    private void Newline()
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
            CursorLeft = 0;
            CursorTop = (Canvas.Height / charHeight) - 1;
            if (!DoubleBufferedMode)
                Render();
            Heap.Collect();
        }
    }

    public void PutChar(char c, int CursorLeft, int y, bool quick = false)
    {
        if (!quick)
            Canvas.DrawFilledRectangle(CursorLeft * charWidth, y * charHeight,
                Convert.ToUInt16(charWidth + (charWidth / 8)), charHeight, 0, BackgroundColor); //yes this is correct
        if (c != ' ')
            Canvas.DrawString(CursorLeft * charWidth, y * charHeight, c.ToString(), font, ForegroundColor);
    }

    #endregion
}
