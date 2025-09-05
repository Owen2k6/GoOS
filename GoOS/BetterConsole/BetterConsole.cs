using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cosmos.Core.Memory;
using Cosmos.System;
using Gold.Graphics;
using GoOS.GUI;
using GoOS.GUI.Apps.Settings;
using GoOS.Themes;
using static GoOS.Resources;
using Kernel = GoOS.Kernel;
public static class BetterConsole
{
    public const ushort CharWidth = 8;
    public const ushort CharHeight = 16;
    private const int MaxKeyBufferSize = 256;
    private const int TabWidth = 4;
    public static Canvas Canvas;
    public static bool Visible = false;
    public static int CursorLeft;
    public static int CursorTop;
    public static ushort WindowWidth;
    public static ushort WindowHeight;
    public static Color ForegroundColor = ConsoleColorEx.White;
    public static Color BackgroundColor = ConsoleColorEx.Black;
    public static bool CursorVisible = true;
    public static bool DoubleBufferedMode;
    public static Queue<KeyEvent> KeyBuffer = new();
    public static string Title = "BetterConsole";
    private static string lastInput = string.Empty;
    private static byte scrollCounter;
    private struct Cell
    {
        public char Ch;
        public Color Fg;
        public Color Bg;
    }
    private static Cell[] _cells = Array.Empty<Cell>();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Index(int x, int y) => y * WindowWidth + x;
    private static readonly string[] _charLut = new string[256];
    private static bool _cursorDrawn;
    private static int _cursorLeftDrawn = -1;
    private static int _cursorTopDrawn = -1;
    public static void Init(ushort width, ushort height)
    {
        Canvas = new Canvas(width, height);
        WindowWidth = (ushort)(width / CharWidth);
        WindowHeight = (ushort)(height / CharHeight);
        for (int i = 0; i < _charLut.Length; i++)
            _charLut[i] = new string((char)i, 1);
        _cells = new Cell[WindowWidth * WindowHeight];
        Canvas.Clear();
        CursorLeft = 0;
        CursorTop = 0;
        _cursorDrawn = false;
        var bg = BackgroundColor;
        var fg = ForegroundColor;
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Ch = ' ';
            _cells[i].Bg = bg;
            _cells[i].Fg = fg;
        }
    }
    public static void Clear(bool render = true)
    {
        Canvas.Clear(BackgroundColor);
        CursorLeft = 0;
        CursorTop = 0;
        _cursorDrawn = false;
        var bg = BackgroundColor;
        var fg = ForegroundColor;
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Ch = ' ';
            _cells[i].Bg = bg;
            _cells[i].Fg = fg;
        }
        if (render || !DoubleBufferedMode)
            Render();
    }
    public static void Render()
    {
        WindowManager.Update();
    }
    public static void Write(object text, bool quick = false)
    {
        if (text == null) return;
        SmartCursor(false);
        string s = text as string ?? text.ToString();
        int len = s.Length;
        for (int i = 0; i < len; i++)
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
                int spaces = TabWidth - (CursorLeft % TabWidth);
                for (int t = 0; t < spaces; t++)
                {
                    PutChar(' ', CursorLeft, CursorTop, quick);
                    AdvanceCursor();
                }
                continue;
            }
            PutChar(c, CursorLeft, CursorTop, quick);
            AdvanceCursor();
        }
        if (!DoubleBufferedMode)
            Render();
        SmartCursor(true);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AdvanceCursor()
    {
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
    public static void WriteLine(object text = null, bool quick = false)
    {
        if (text != null)
            Write(text, quick);
        Write("\n", quick);
    }
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
                {
                    char ch = key.KeyChar;
                    if (ch == '\n')
                    {
                        CursorLeft = 0; CursorTop++; CheckNewline();
                    }
                    else if (ch == '\r')
                    {
                        CursorLeft = 0;
                    }
                    else if (ch == '\t')
                    {
                        int spaces = TabWidth - (CursorLeft % TabWidth);
                        for (int t = 0; t < spaces; t++) { PutChar(' ', CursorLeft, CursorTop, true); AdvanceCursor(); }
                    }
                    else if (ch >= ' ')
                    {
                        PutChar(ch, CursorLeft, CursorTop, true);
                        AdvanceCursor();
                    }
                }
                var xShift = (key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift;
                var xAlt = (key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt;
                var xControl = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;
                return new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey(), xShift, xAlt, xControl);
            }

            Render();
            TrimKeyBuffer();
        }
    }
    public static string ReadLine()
    {
        int startX = CursorLeft, startY = CursorTop;
        var line = string.Empty;
        var reading = true;
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
                                CursorTop--;
                                CursorLeft = WindowWidth - 1;
                                PutChar(' ', CursorLeft, CursorTop, true);
                            }
                            else if (CursorLeft > 0)
                            {
                                CursorLeft--;
                                PutChar(' ', CursorLeft, CursorTop, true);
                            }
                            if (line.Length > 0)
                                line = line.Remove(line.Length - 1);
                        }
                        break;
                    case ConsoleKeyEx.Tab:
                        {
                            int spaces = TabWidth - (CursorLeft % TabWidth);
                            for (int t = 0; t < spaces; t++)
                            {
                                PutChar(' ', CursorLeft, CursorTop, true);
                                AdvanceCursor();
                            }
                            line += new string(' ', spaces);
                        }
                        break;
                    case ConsoleKeyEx.UpArrow:
                        if (!string.IsNullOrEmpty(lastInput))
                        {
                            SetCursorPosition(startX, startY);
                            int toClear = line.Length;
                            for (int i = 0; i < toClear; i++)
                            {
                                PutChar(' ', CursorLeft, CursorTop, true);
                                AdvanceCursor();
                            }
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
                                var collected = Heap.Collect() + " items collected";
                                Canvas.DrawString(Canvas.Width - collected.Length * 8 - 8, Canvas.Height - 32,
                                    collected, Font_1x, ThemeManager.WindowText);
                                SetCursorPosition(startX, startY);
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
                                var input = ReadLine();
                                if (input == "e015")
                                {
                                    Clear();
                                    WriteLine("Thank you for using GoOS - Owen2k6");
                                    WriteLine("There used to be an image here however");
                                    WriteLine("due to Console resizing it had to be");
                                    WriteLine("removed.");
                                    WriteLine("Check the source code on github to see it!");
                                    WriteLine("Big thanks to all who helped develop GoOS");
                                    WriteLine("Over the years. Great people!");
                                    WriteLine("- Owen2k6");
                                    WriteLine("Press enter twice to escape!");
                                    ReadKey();
                                    Clear();
                                }
                            }
                        }
                        else
                        {
                            char ch = key.KeyChar;
                            if (ch >= ' ')
                            {
                                PutChar(ch, CursorLeft, CursorTop, true);
                                AdvanceCursor();
                                line += ch;
                            }
                        }
                        break;
                }
                if (!DoubleBufferedMode) Render();
            }
            else
            {
                Render();
                TrimKeyBuffer();
            }
        }
        return line;
    }
    public static void SetCursorPosition(int x, int y)
    {
        CursorLeft = Math.Clamp(x, 0, WindowWidth - 1);
        CursorTop = Math.Clamp(y, 0, WindowHeight - 1);
        _cursorDrawn = false; // force re-draw at new location when requested
    }
    public static (int Left, int Top) GetCursorPosition() => (CursorLeft, CursorTop);
    public static void Beep(uint freq = 800, uint duration = 125) => PCSpeaker.Beep(freq, duration);
    #region Private
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void TrimKeyBuffer()
    {
        while (KeyBuffer.Count > MaxKeyBufferSize)
            KeyBuffer.Dequeue();
    }
    internal static void SmartCursor(bool on)
    {
        if (!CursorVisible) return;
        if (on)
        {
            if (_cursorDrawn && _cursorLeftDrawn == CursorLeft && _cursorTopDrawn == CursorTop)
                return;
            if (_cursorDrawn)
                RedrawCell(_cursorLeftDrawn, _cursorTopDrawn);
            int px = CursorLeft * CharWidth;
            int py = CursorTop * CharHeight + CharHeight - 2;
            Canvas.DrawFilledRectangle(px, py, CharWidth, 2, 0, ForegroundColor);
            _cursorDrawn = true;
            _cursorLeftDrawn = CursorLeft;
            _cursorTopDrawn = CursorTop;
        }
        else
        {
            if (!_cursorDrawn) return;
            RedrawCell(_cursorLeftDrawn, _cursorTopDrawn);
            _cursorDrawn = false;
        }
    }
    private static void CheckNewline()
    {
        if (CursorTop >= WindowHeight)
        {
            Canvas.DrawImage(0, -CharHeight, Canvas, false);
            int cols = WindowWidth;
            int rows = WindowHeight;
            int rowStride = cols;
            Array.Copy(_cells, rowStride, _cells, 0, rowStride * (rows - 1));
            int start = rowStride * (rows - 1);
            var bg = BackgroundColor;
            var fg = ForegroundColor;
            for (int i = 0; i < rowStride; i++)
            {
                _cells[start + i].Ch = ' ';
                _cells[start + i].Bg = bg;
                _cells[start + i].Fg = fg;
            }
            CursorTop = WindowHeight - 1;
            if (!DoubleBufferedMode)
                Render();
            scrollCounter++;
            if (scrollCounter >= 3)
            {
                Heap.Collect();
                scrollCounter = 0;
            }
            _cursorDrawn = false;
        }
    }

    public static void PutChar(char c, int x, int y, bool quick = false)
    {
        if ((uint)x >= WindowWidth || ((uint)y >= WindowHeight))
            return;
        int idx = Index(x, y);
        ref Cell cell = ref _cells[idx];
        if (cell.Ch == c && cell.Fg.Equals(ForegroundColor) && cell.Bg.Equals(BackgroundColor))
        {
            _cursorDrawn = false;
            return;
        }
        int px = x * CharWidth;
        int py = y * CharHeight;
        if (!cell.Bg.Equals(BackgroundColor) || cell.Ch == ' ' || c == ' ')
            Canvas.DrawFilledRectangle(px, py, CharWidth, CharHeight, 0, BackgroundColor);
        if (c != ' ')
        {
            string glyph = (c <= 255) ? _charLut[c] : new string(c, 1);
            Canvas.DrawString(px, py, glyph, Font_1x, ForegroundColor);
        }
        cell.Ch = c;
        cell.Fg = ForegroundColor;
        cell.Bg = BackgroundColor;
        _cursorDrawn = false;
        if (!DoubleBufferedMode && quick == false)
            Render();
    }
    private static void RedrawCell(int x, int y)
    {
        if ((uint)x >= WindowWidth || (uint)y >= WindowHeight) return;
        int idx = Index(x, y);
        ref Cell cell = ref _cells[idx];
        int px = x * CharWidth;
        int py = y * CharHeight;
        Canvas.DrawFilledRectangle(px, py, CharWidth, CharHeight, 0, cell.Bg);
        if (cell.Ch != ' ')
        {
            string glyph = (cell.Ch <= 255) ? _charLut[cell.Ch] : new string(cell.Ch, 1);
            Canvas.DrawString(px, py, glyph, Font_1x, cell.Fg);
        }
    }
    #endregion
}
public static class ConsoleColorEx
{
    public static readonly Color Black       = new(0,   0,   0);
    public static readonly Color DarkBlue    = new(0,   0,   170);
    public static readonly Color DarkGreen   = new(0,   170,  0);
    public static readonly Color DarkCyan    = new(0,   170,  170);
    public static readonly Color DarkRed     = new(170, 0,    0);
    public static readonly Color DarkMagenta = new(170, 0,    170);
    public static readonly Color DarkYellow  = new(170, 85,   0);
    public static readonly Color Gray        = new(170, 170,  170);
    public static readonly Color DarkGray    = new(85,  85,   85);
    public static readonly Color Blue        = new(85,  85,   255);
    public static readonly Color Green       = new(85,  255,  85);
    public static readonly Color Cyan        = new(85,  255,  255);
    public static readonly Color Red         = new(255, 85,   85);
    public static readonly Color Magenta     = new(255, 85,   255);
    public static readonly Color Yellow      = new(255, 255,  85);
    public static readonly Color White       = new(255, 255,  255);
}
