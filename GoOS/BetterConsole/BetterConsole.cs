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

    private static readonly List<string> MenuOptions = new List<string>
    {
        "Launch Settings",
        "Reboot"
    };

    // Maximum size for key buffer to prevent memory issues
    private const int MaxKeyBufferSize = 256;
    private const string TabSpaces = "    ";

    public static bool ConsoleMode = false;
    public static bool Visible = false;

    /// <summary>The X position of the cursor</summary>
    public static int CursorLeft = 0;
    /// <summary>The Y position of the cursor</summary>
    public static int CursorTop = 0;

    /// <summary>The width of the <see cref="BetterConsole"/></summary>
    public static ushort WindowWidth = 0;
    /// <summary>The height of the <see cref="BetterConsole"/></summary>
    public static ushort WindowHeight = 0;

    /// <summary>The foreground colour</summary>
    public static Color ForegroundColor = ConsoleColorEx.White;
    /// <summary>The background colour</summary>
    public static Color BackgroundColor = ConsoleColorEx.Black;

    /// <summary>Determines if the cursor is visible</summary>
    public static bool CursorVisible = true;

    /// <summary>If true, callers control when Render() happens</summary>
    public static bool DoubleBufferedMode = false;

    /// <summary>Queue of key events destined for the console</summary>
    public static Queue<KeyEvent> KeyBuffer = new Queue<KeyEvent>();

    public static string Title = "GTerm";

    private static string lastInput = string.Empty;
    private static byte scrollCounter = 0;

    // cursor state cache to stop redundant redraws
    private static bool _cursorDrawn = false;
    private static int _cursorLeftDrawn = -1;
    private static int _cursorTopDrawn = -1;

    /// <summary>
    /// Initialises the <see cref="BetterConsole">
    /// </summary>
    public static void Init(ushort width, ushort height)
    {
        Canvas = new Canvas(width, height);
        WindowWidth = (ushort)(width / CharWidth);
        WindowHeight = (ushort)(height / CharHeight);
        Canvas.Clear();
        _cursorDrawn = false;
    }

    /// <summary>Clears the console</summary>
    public static void Clear(bool render = true)
    {
        Canvas.Clear(BackgroundColor);
        CursorLeft = 0;
        CursorTop = 0;
        _cursorDrawn = false;

        if (render || !DoubleBufferedMode)
            Render();
    }

    /// <summary>Renders the console</summary>
    public static void Render() => WindowManager.Update();

    /// <summary>Writes text</summary>
    public static void Write(object text, bool quick = false)
    {
        if (text == null) return;

        // hide cursor whilst drawing to avoid flicker
        SmartCursor(false);

        string s = text.ToString();
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (c == '\n')
            {
                CursorLeft = 0;
                CursorTop++;
                CheckNewline();
                continue;
            }
            if (c == '\r')
            {
                CursorLeft = 0;
                continue;
            }
            if (c == '\t')
            {
                // print 4 spaces using same post-draw wrap behaviour
                Write(TabSpaces, true);
                continue;
            }

            // draw at current position
            PutChar(c, CursorLeft, CursorTop, quick);

            // wrap AFTER drawing the last column — removes right-edge jitter
            if (CursorLeft == WindowWidth - 1)
            {
                CursorLeft = 0;
                CursorTop++;
                CheckNewline();
            }
            else
            {
                CursorLeft++;
            }
        }

        if (!DoubleBufferedMode)
            Render();

        // reshow cursor at new position
        SmartCursor(true);
    }

    /// <summary>Writes line</summary>
    public static void WriteLine(object text = null, bool quick = false)
        => Write((text ?? string.Empty).ToString() + "\n", quick);

    /// <summary>Read a single key</summary>
    public static ConsoleKeyInfo ReadKey(bool intercept = true)
    {
        while (true)
        {
            if (CursorVisible)
            {
                SmartCursor(true);
                if (!DoubleBufferedMode) Render();
            }

            if (KeyBuffer.Count > 0)
            {
                var key = KeyBuffer.Dequeue();

                SmartCursor(false);

                if (!intercept)
                    Write(key.KeyChar);

                bool xShift = (key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift;
                bool xAlt = (key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt;
                bool xControl = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;

                return new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey(), xShift, xAlt, xControl);
            }

            WindowManager.Update();

            while (KeyBuffer.Count > MaxKeyBufferSize)
                KeyBuffer.Dequeue();
        }
    }

    /// <summary>Read a line of input</summary>
    public static string ReadLine()
    {
        int startX = CursorLeft, startY = CursorTop;
        string line = string.Empty;

        bool reading = true;
        while (reading)
        {
            if (CursorVisible)
            {
                SmartCursor(true);
                if (!DoubleBufferedMode) Render();
            }

            if (KeyBuffer.Count > 0)
            {
                var key = KeyBuffer.Dequeue();
                SmartCursor(false);

                switch (key.Key)
                {
                    case ConsoleKeyEx.Enter:
                        CursorLeft = 0;
                        CursorTop++;
                        CheckNewline();
                        lastInput = line;
                        reading = false;
                        break;

                    case ConsoleKeyEx.Backspace:
                        if (CursorLeft > startX || CursorTop > startY)
                        {
                            if (CursorLeft == 0 && CursorTop > startY)
                            {
                                // go to end of previous visual line
                                CursorTop--;
                                CursorLeft = WindowWidth - 1;
                                PutChar(' ', CursorLeft, CursorTop);
                            }
                            else if (CursorLeft > 0)
                            {
                                CursorLeft--;
                                PutChar(' ', CursorLeft, CursorTop);
                            }
                            if (line.Length > 0)
                                line = line.Remove(line.Length - 1);
                        }
                        break;

                    case ConsoleKeyEx.Tab:
                        Write(TabSpaces, true);
                        line += TabSpaces;
                        break;

                    case ConsoleKeyEx.UpArrow:
                        if (!string.IsNullOrEmpty(lastInput))
                        {
                            // clear current input (overwrite with spaces)
                            SetCursorPosition(startX, startY);
                            int toClear = line.Length;
                            for (int i = 0; i < toClear; i++)
                            {
                                PutChar(' ', CursorLeft, CursorTop, true);
                                if (CursorLeft == WindowWidth - 1)
                                {
                                    CursorLeft = 0;
                                    CursorTop++;
                                    CheckNewline();
                                }
                                else
                                {
                                    CursorLeft++;
                                }
                            }
                            // write previous input
                            SetCursorPosition(startX, startY);
                            Write(lastInput, true);
                            line = lastInput;
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
                                Write(line, true);
                            }
                            else if (key.Key == ConsoleKeyEx.L)
                            {
                                Clear();
                                line = string.Empty;
                                reading = false;
                            }
                            else if (KeyboardManager.ShiftPressed && key.Key == ConsoleKeyEx.E)
                            {
                                Write("> ", true);
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
                            // printable
                            Write(key.KeyChar.ToString(), true);
                            line += key.KeyChar;
                        }
                        break;
                }

                if (!DoubleBufferedMode) Render();
            }
            else
            {
                WindowManager.Update();
                while (KeyBuffer.Count > MaxKeyBufferSize)
                    KeyBuffer.Dequeue();
            }
        }

        return line;
    }

    /// <summary>Set the cursor position</summary>
    public static void SetCursorPosition(int x, int y)
    {
        CursorLeft = Math.Clamp(x, 0, WindowWidth - 1);
        CursorTop = Math.Clamp(y, 0, WindowHeight - 1);
        _cursorDrawn = false; // force re-draw at new location when requested
    }

    public static (int Left, int Top) GetCursorPosition() => (CursorLeft, CursorTop);

    public static void Beep(uint freq = 800, uint duration = 125) => PCSpeaker.Beep(freq, duration);

    #region Private

    // draw/erase cursor only when necessary
    private static void SmartCursor(bool on)
    {
        if (!CursorVisible) return;

        if (on)
        {
            if (_cursorDrawn && _cursorLeftDrawn == CursorLeft && _cursorTopDrawn == CursorTop)
                return; // already drawn correctly

            // first make sure previous cursor (if any) is cleared
            if (_cursorDrawn)
            {
                int pxOld = _cursorLeftDrawn * CharWidth;
                int pyOld = _cursorTopDrawn * CharHeight;
                Canvas.DrawFilledRectangle(pxOld, pyOld, CharWidth, CharHeight, 0, BackgroundColor);
            }

            int px = CursorLeft * CharWidth;
            int py = CursorTop * CharHeight;
            Canvas.DrawString(px, py, "_", Font_1x, ForegroundColor);

            _cursorDrawn = true;
            _cursorLeftDrawn = CursorLeft;
            _cursorTopDrawn = CursorTop;
        }
        else
        {
            if (!_cursorDrawn) return;
            int px = _cursorLeftDrawn * CharWidth;
            int py = _cursorTopDrawn * CharHeight;
            Canvas.DrawFilledRectangle(px, py, CharWidth, CharHeight, 0, BackgroundColor);
            _cursorDrawn = false;
        }
    }

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

            // cursor cache invalid (content moved)
            _cursorDrawn = false;
        }
    }

    private static void ShowConsoleMenu()
    {
        // Save colours and buffering
        Color oldFg = ForegroundColor;
        Color oldBg = BackgroundColor;
        bool oldDb = DoubleBufferedMode;
        DoubleBufferedMode = true; // reduce flicker

        int selected = 0;

        Clear(false);
        DrawMenuBorder();

        while (true)
        {
            if (MenuOptions.Count > 0)
                selected = (selected + MenuOptions.Count) % MenuOptions.Count;

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

                Write(MenuOptions[i], true);
            }
            Render();

            var key = KeyboardManager.ReadKey();
            switch (key.Key)
            {
                case ConsoleKeyEx.Escape:
                    Clear(false);
                    ForegroundColor = oldFg; BackgroundColor = oldBg; DoubleBufferedMode = oldDb;
                    Render();
                    return;

                case ConsoleKeyEx.Enter:
                    if (selected == 0) WindowManager.AddWindow(new Frame());
                    else if (selected == 1) Power.Reboot();
                    Clear(false);
                    ForegroundColor = oldFg; BackgroundColor = oldBg; DoubleBufferedMode = oldDb;
                    Render();
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

        Canvas.DrawRectangle(menuX, menuY, menuWidth, menuHeight, 0, ThemeManager.WindowBorder);
        Canvas.DrawRectangle((ushort)(menuX + 1), (ushort)(menuY + 1),
                             (ushort)(menuWidth - 2), (ushort)(menuHeight - 2), 0, ThemeManager.WindowBorder);
    }

    public static void PutChar(char c, int x, int y, bool quick = false)
    {
        // Ensure we're within bounds
        if (x < 0 || x >= WindowWidth || y < 0 || y >= WindowHeight)
            return;

        // Clear the background for this cell
        int px = x * CharWidth;
        int py = y * CharHeight;
        Canvas.DrawFilledRectangle(px, py, CharWidth, CharHeight, 0, BackgroundColor);

        if (c != ' ')
            Canvas.DrawString(px, py, c.ToString(), Font_1x, ForegroundColor);

        // mark cursor as needing redraw (cell changed)
        _cursorDrawn = false;
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
