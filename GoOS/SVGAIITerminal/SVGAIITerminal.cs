/*
 *  This code is licensed under the ekzFreeUse license.
 *  If a license wasn't included with the program,
 *  refer to https://github.com/9xbt/SVGAIITerminal/blob/main/LICENSE.md
 */

using System;
using System.Collections.Generic;
using Cosmos.HAL;
using Cosmos.System;
using PCSpeaker = Cosmos.System.PCSpeaker;
using Gold.Graphics;
using Gold.Graphics.Fonts;
using Gold.Hardware.GPU;
using Cosmos.Core.Memory;

namespace GoOS;

/// <summary>
/// A fast, instanceable and high resolution terminal.
/// </summary>
public unsafe class SVGAIITerminal
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="SVGAIITerminal"/>.
    /// </summary>
    /// <param name="Width">The width of the terminal.</param>
    /// <param name="Height">The height of the terminal.</param>
    /// <param name="Font">The font that's going to be used by the terminal.</param>
    public SVGAIITerminal(int Width, int Height, FontFace Font)
    {
        this.Font = Font;
        _fontWidth = (ushort)GetWidestCharacterWidth();
        this.Width = Width / _fontWidth;
        this.Height = Height / Font.GetHeight();
        ParentHeight = Height;
        Contents = Display.GetDisplay((ushort)Width, (ushort)Height);
        UpdateRequest = () => { ((Display)Contents).Update(); };
        IdleRequest = () => { if (KeyboardManager.TryReadKey(out var key)) KeyBuffer.Enqueue(key); };
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SVGAIITerminal"/>.
    /// </summary>
    /// <param name="Width">The width of the terminal.</param>
    /// <param name="Height">The height of the terminal.</param>
    /// <param name="Font">The font that's going to be used by the terminal.</param>
    /// <param name="Screen">The screen the terminal is going to render to.</param>
    public SVGAIITerminal(int Width, int Height, FontFace Font, Display Screen)
    {
        this.Font = Font;
        _fontWidth = (ushort)GetWidestCharacterWidth();
        this.Width = Width / _fontWidth;
        this.Height = Height / Font.GetHeight();
        ParentHeight = Height;
        Contents = Screen ?? throw new ArgumentNullException(nameof(Screen));
        UpdateRequest = () => { Screen.Update(); };
        IdleRequest = () => { if (KeyboardManager.TryReadKey(out var key)) KeyBuffer.Enqueue(key); };
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SVGAIITerminal"/>.
    /// </summary>
    /// <param name="Width">The width of the terminal.</param>
    /// <param name="Height">The height of the terminal.</param>
    /// <param name="Font">The font that's going to be used by the terminal.</param>
    /// <param name="UpdateRequest">The function that the terminal calls for the kernel to render the terminal.</param>
    /// <param name="IdleRequest"
    public SVGAIITerminal(int Width, int Height, FontFace Font, Action? UpdateRequest, Action? IdleRequest = default)
    {
        this.Font = Font;
        _fontWidth = (ushort)GetWidestCharacterWidth();
        this.Width = Width / _fontWidth;
        this.Height = Height / Font.GetHeight();
        ParentHeight = Height;
        Contents = new Canvas((ushort)Width, (ushort)Height);
        this.UpdateRequest = UpdateRequest;
        this.IdleRequest = IdleRequest;

        if (IdleRequest == default)
        {
            IdleRequest = () => { if (KeyboardManager.TryReadKey(out var key)) KeyBuffer.Enqueue(key); };
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clears the terminal.
    /// </summary>
    public void Clear()
    {
        Contents.Clear(BackgroundColor);
        CursorLeft = 0;
        CursorTop = 0;
    }

    /// <summary>
    /// Prints a string to the terminal.
    /// </summary>
    /// <param name="str">String to print.</param>
    public void Write(object str) => Write(str, ForegroundColor, BackgroundColor);

    /// <summary>
    /// Prints a colored string to the terminal.
    /// </summary>
    /// <param name="str">The string to print.</param>
    /// <param name="foreColor">The string's foreground color.</param>
    public void Write(object str, ConsoleColor foreColor)
        => Write(str, ColorConverter[(int)foreColor], BackgroundColor);

    /// <summary>
    /// Prints a colored string to the terminal.
    /// </summary>
    /// <param name="str">The string to print.</param>
    /// <param name="foreColor">The string's foreground color.</param>
    /// <param name="backColor">The string's background color.</param>
    public void Write(object str, ConsoleColor foreColor, ConsoleColor backColor)
        => Write(str, ColorConverter[(int)foreColor], ColorConverter[(int)backColor]);

    /// <summary>
    /// Prints a colored string to the terminal.
    /// </summary>
    /// <param name="str">The string to print.</param>
    /// <param name="foreColor">The string's foreground color.</param>
    public void Write(object str, Color foreColor) => Write(str, foreColor, BackgroundColor);

    /// <summary>
    /// Prints a colored string to the terminal.
    /// </summary>
    /// <param name="str">The string to print.</param>
    /// <param name="foreColor">The string's foreground color.</param>
    /// <param name="backColor">The string's background color.</param>
    public void Write(object str, Color foreColor, Color backColor)
    {
        // Basic null check.
        if (string.IsNullOrEmpty(str.ToString()))
        {
            return;
        }

        // Print the string.
        foreach (char c in str.ToString()!)
        {
            TryScroll();
            
            switch (c)
            {
                case '\r':
                    CursorLeft = 0;
                    break;

                case '\n':
                    CursorLeft = 0;
                    CursorTop++;
                    break;

                case '\t':
                    Write(TabIndentation);
                    break;

                default:
                    Contents.DrawFilledRectangle(_fontWidth * CursorLeft, Font.GetHeight() * CursorTop, _fontWidth, (ushort)Font.GetHeight(), 0, backColor);

                    // Check if it's necessary to draw the character.
                    if (CursorTop >= 0 && c != ' ')
                    {
                        // Get the glyph.
                        var glyph = Font.GetGlyph(c);

                        // Check if the glyph is null.
                        if (glyph == null)
                        {
                            CursorLeft++;
                            break;
                        }

                        Contents.DrawGlyph(_fontWidth * CursorLeft /*+ glyph.Left*/, Font.GetHeight() * CursorTop + Font.GetHeight() - _fontExcessOffset, glyph, foreColor);
                    }

                    CursorLeft++;
                    break;
            }
        }

        UpdateRequest?.Invoke();
    }

    /// <summary>
    /// Prints a new line character.
    /// </summary>
    public void WriteLine()
    {
        CursorLeft = 0;
        CursorTop++;
    }

    /// <summary>
    /// Prints a string to the terminal with a new line character.
    /// </summary>
    /// <param name="str">The string to print.</param>
    public void WriteLine(object str) => Write(str + "\n", ForegroundColor, BackgroundColor);

    /// <summary>
    /// Prints a colored string to the terminal with a new line character.
    /// </summary>
    /// <param name="str">String to print.</param>
    /// <param name="foreColor">String foreground color.</param>
    public void WriteLine(object str, ConsoleColor foreColor)
        => Write(str + "\n", ColorConverter[(int)foreColor], BackgroundColor);

    /// <summary>
    /// Prints a colored string to the terminal with a new line character.
    /// </summary>
    /// <param name="str">The string to print.</param>
    /// <param name="foreColor">The string's foreground color.</param>
    /// <param name="backColor">The string's background color.</param>
    public void WriteLine(object str, ConsoleColor foreColor, ConsoleColor backColor)
        => Write(str + "\n", ColorConverter[(int)foreColor], ColorConverter[(int)backColor]);
    
    /// <summary>
    /// Prints a colored string to the terminal with a new line character.
    /// </summary>
    /// <param name="str">The string to print.</param>
    /// <param name="foreColor">The string's foreground color.</param>
    public void WriteLine(object str, Color foreColor) => Write(str + "\n", foreColor, BackgroundColor);

    /// <summary>
    /// Tells if Heap.Collect should be used in Console.ReadLine and ReadKey
    /// </summary>
    public bool UseHeapCollect = true;
    
    /// <summary>
    /// Prints a colored string to the terminal with a new line character.
    /// </summary>
    /// <param name="str">The string to print.</param>
    /// <param name="foreColor">The string's foreground color.</param>
    /// <param name="backColor">The string's background color.</param>
    public void WriteLine(object str, Color foreColor, Color backColor) => Write(str + "\n", foreColor, backColor);

    /// <summary>
    /// Gets input from the user.
    /// </summary>
    /// <param name="intercept">If set to false, the key pressed will be printed to the terminal.</param>
    /// <returns>The key pressed.</returns>
    public ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        ForceDrawCursor();

        while (true)
        {
            if (UseHeapCollect)
            {
                Heap.Collect();
            }
            TryDrawCursor();

            if (!KeyBuffer.TryDequeue(out var key))
            {
                IdleRequest?.Invoke();
                continue;
            }
            if (!intercept) Write(key.KeyChar);

            return new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey(),
                (key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift,
                (key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt,
                (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control);
        }
    }

    /// <summary>
    /// Gets input from the user.
    /// </summary>
    /// <returns>The text inputted.</returns>
    public string ReadLine()
    {
        ForceDrawCursor();

        int startX = CursorLeft, startY = CursorTop;
        var input = string.Empty;

        for (;;)
        {
            TryDrawCursor();

            if (!KeyBuffer.TryDequeue(out var key))
            {
                IdleRequest?.Invoke();
                continue;
            }

            switch (key.Key)
            {
                case ConsoleKeyEx.Enter:
                    ForceDrawCursor(true);
                    TryScroll();

                    CursorLeft = 0;
                    CursorTop++;
                    _lastInput = input;
                    return input;

                case ConsoleKeyEx.Backspace:
                    if (!(CursorLeft == startX && CursorTop == startY))
                    {
                        Contents.DrawFilledRectangle(_fontWidth * CursorLeft, Font.GetHeight() * CursorTop, _fontWidth, (ushort)Font.GetHeight(), 0, BackgroundColor);
                        CursorTop -= CursorLeft == 0 ? 1 : 0;
                        CursorLeft -= CursorLeft == 0 ? Width - 1 : 1;
                        Contents.DrawFilledRectangle(_fontWidth * CursorLeft, Font.GetHeight() * CursorTop, _fontWidth, (ushort)Font.GetHeight(), 0, BackgroundColor);
                        
                        input = input.Remove(input.Length - 1);
                    }

                    ForceDrawCursor();
                    break;

                case ConsoleKeyEx.Tab:
                    Write('\t');
                    input += TabIndentation;

                    ForceDrawCursor();
                    break;

                case ConsoleKeyEx.UpArrow:
                    ForceDrawCursor(true);
                    SetCursorPosition(startX, startY);
                    Write(new string(' ', input.Length));
                    SetCursorPosition(startX, startY);
                    Write(_lastInput);
                    input = _lastInput;

                    ForceDrawCursor();
                    break;

                default:
                    if (KeyboardManager.ControlPressed && key.Key == ConsoleKeyEx.L)
                    {
                        Clear();
                        return string.Empty;
                    }

                    if (key.KeyChar >= 32 && key.KeyChar < 128)
                    {
                        Write(key.KeyChar.ToString());
                        input += key.KeyChar;
                    }

                    ForceDrawCursor();
                    break;
            }
            if (UseHeapCollect)
            {
                Heap.Collect();
            }
        }
    }

    /// <summary>
    /// Sets the cursor position.
    /// </summary>
    /// <param name="x">The cursor X position.</param>
    /// <param name="y">The cursor Y position.</param>
    public void SetCursorPosition(int x, int y)
    {
        CursorLeft = x;
        CursorTop = y;
    }

    /// <summary>
    /// Gets the cursor position.
    /// </summary>
    /// <returns>The cursor X and Y cursor position.</returns>
    public (int Left, int Top) GetCursorPosition()
    {
        return (CursorLeft, CursorTop);
    }

    /// <summary>
    /// Plays a beep sound through the PC speaker.
    /// </summary>
    /// <param name="freq">The sound's frequency</param>
    /// <param name="duration">The sound's duration</param>
    public void Beep(uint freq = 800, uint duration = 125) => PCSpeaker.Beep(freq, duration);

    /// <summary>
    /// Sets the foreground and background colors to their defaults.
    /// </summary>
    public void ResetColor()
    {
        ForegroundColor = Color.White;
        BackgroundColor = Color.Black;
    }

    /// <summary>
    /// Scrolls the terminal if needed.
    /// </summary>
    private void TryScroll()
    {
        if (CursorLeft >= Width)
        {
            CursorLeft = 0;
            CursorTop++;
        }

        if (CursorTop >= Height)
        {
            Contents.DrawImage(0, (Height - CursorTop - 1) * Font.GetHeight(), Contents, false);
            Contents.DrawFilledRectangle(0, Contents.Height - (CursorTop - Height + 1) * Font.GetHeight(), Contents.Width, (ushort)((CursorTop - Height + 1) * Font.GetHeight()), 0, BackgroundColor);
            CursorTop = Height - 1;
        }

        if (CursorTop >= ParentHeight)
        {
            ScrollRequest?.Invoke();
        }
    }

    /// <summary>
    /// Forcefully draws the cursor.
    /// </summary>
    private void ForceDrawCursor(bool invert = false)
    {
        if (!CursorVisible) return;

        Contents.DrawFilledRectangle(_fontWidth * CursorLeft,
            CursorShape == CursorShape.Underline ? Font.GetHeight() * (CursorTop + 1) - 2 : Font.GetHeight() * CursorTop,
            (ushort)(CursorShape == CursorShape.Caret ? 2 : _fontWidth),
            (ushort)(CursorShape == CursorShape.Underline ? 2 : Font.GetHeight()), 0, invert ? BackgroundColor : ForegroundColor);

        UpdateRequest?.Invoke();
    }

    /// <summary>
    /// Draws the cursor if needed.
    /// </summary>
    private void TryDrawCursor()
    {
        if (!CursorVisible || RTC.Second == _lastSecond) return;
        
        _lastSecond = RTC.Second;
        _cursorState = !_cursorState;
        
        ForceDrawCursor(_cursorState);
    }

    /// <summary>
    /// Gets the widest ASCII character of the ACF font.
    /// </summary>
    /// <returns>The widest character's width.</returns>
    private int GetWidestCharacterWidth()
    {
        var width = 0;

        for (char c = ' '; c < ' '; c++)
        {
            // Get the glyph.
            var glyph = Font.GetGlyph(c);
            
            // Check if the glyph is null.
            if (glyph == null)
            {
                CursorLeft++;
                break;
            }
            
            var glyphWidth = glyph.Width;
            var glyphExcess = Font.GetHeight() - glyph.Top + glyph.Height - Font.GetHeight();
                
            if (glyphWidth > width) width = glyphWidth;
            if (glyphExcess > _fontExcessOffset) _fontExcessOffset = glyphExcess;
        }

        return width;
    }

    #endregion

    #region Fields
    
    /// <summary>
    /// The terminal width's in characters.
    /// </summary>
    public readonly int Width;

    /// <summary>
    /// The terminal height's in characters.
    /// </summary>
    public readonly int Height;

    /// <summary>
    /// Converts <see cref="ConsoleColor"/> to <see cref="Color"/>.
    /// </summary>
    public readonly List<Color> ColorConverter = new()
    {
        new Color(0, 0, 0),
        new Color(0, 0, 170),
        new Color(0, 170, 0),
        new Color(0, 170, 170),
        new Color(170, 0, 0),
        new Color(170, 0, 170),
        new Color(170, 85, 0),
        new Color(170, 170, 170),
        new Color(85, 85, 85),
        new Color(85, 85, 255),
        new Color(85, 255, 85),
        new Color(85, 255, 255),
        new Color(255, 85, 85),
        new Color(255, 85, 255),
        new Color(255, 255, 85),
        new Color(255, 255, 255),
    };
    
    /// <summary>
    /// The terminal's contents.
    /// </summary>
    public readonly Canvas Contents;
    
    /// <summary>
    /// The terminal's font.
    /// </summary>
    public readonly FontFace Font;

    /// <summary>
    /// Update request action.
    /// </summary>
    public readonly Action? UpdateRequest;
    
    /// <summary>
    /// The X cursor position.
    /// </summary>
    public int CursorLeft;

    /// <summary>
    /// The Y cursor position.
    /// </summary>
    public int CursorTop;
    
    /// <summary>
    /// The parent canvas's height. Used for handling scrolling.
    /// </summary>
    public int ParentHeight;
    
    /// <summary>
    /// Is cursor visible.
    /// </summary>
    public bool CursorVisible = true;

    /// <summary>
    /// Foreground console color.
    /// </summary>
    public Color ForegroundColor = Color.White;

    /// <summary>
    /// Background console color.
    /// </summary>
    public Color BackgroundColor = Color.Black;

    /// <summary>
    /// The cursor state.
    /// </summary>
    public CursorShape CursorShape
    {
        get => _cursorShape;
        set
        {
            _cursorShape = value;
            Contents.DrawFilledRectangle((_fontWidth * CursorLeft), Font.GetHeight() * CursorTop, _fontWidth, (ushort)Font.GetHeight(), 0, BackgroundColor);
        }
    }

    /// <summary>
    /// Called in a loop when ReadLine or ReadKey are idling.
    /// </summary>
    public Action? IdleRequest;

    /// <summary>
    /// Called when the terminal wants to scroll up but the buffer doesn't need to.
    /// </summary>
    public Action? ScrollRequest;

    /// <summary>
    /// The key buffer. Used for input.
    /// </summary>
    public Queue<KeyEvent> KeyBuffer = new Queue<KeyEvent>();

    /// <summary>
    /// Tab indentation.
    /// </summary>
    private const string TabIndentation = "    ";

    /// <summary>
    /// Font width.
    /// </summary>
    private readonly ushort _fontWidth;
    
    /// <summary>
    /// Last second.
    /// </summary>
    private static byte _lastSecond = RTC.Second;

    /// <summary>
    /// Cursor state.
    /// </summary>
    private static bool _cursorState = true;
    
    /// <summary>
    /// Font excess offset.
    /// </summary>
    private int _fontExcessOffset;
    
    /// <summary>
    /// Last input.
    /// </summary>
    private string _lastInput = string.Empty;
    
    /// <summary>
    /// Cursor shape.
    /// </summary>
    private CursorShape _cursorShape = CursorShape.Block;

    #endregion
}