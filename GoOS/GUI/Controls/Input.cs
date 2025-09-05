using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.System;
using Gold.Graphics;
using GoOS.GUI.Models;

namespace GoOS.GUI;

public class Input : Control
{
    private const int padding = 3;

    private string _placeholderText = string.Empty;
    private int caretCol;
    private int caretLine;
    private int lineOffset;

    public Action Changed;
    public Action Submitted;

    /// <summary>
    /// Optional image.
    /// </summary>
    public Canvas Image;

    public List<string> lines = new() { string.Empty };

    private int scrollX;
    private int scrollY;

    public bool ReadOnly { get; set; } = false;
    public bool MultiLine { get; set; } = false;
    public bool Shield { get; set; } = false;
    public bool Numbers { get; set; } = false;
    private bool scrollMode;

    public Input(Window parent, ushort x, ushort y, ushort width, ushort height, string placeholder,
        Canvas image = null)
        : base(parent, x, y, width, height)
    {
        PlaceholderText = placeholder;
        Image = image;
    }

    public string Text
    {
        get
        {
            var builder = new StringBuilder();
            for (var i = 0; i < lines.Count; i++)
            {
                builder.Append(lines[i]);
                if (i != lines.Count - 1) builder.AppendLine();
            }

            return builder.ToString();
        }
        set
        {
            lines = value.Split('\n').ToList();

            caretLine = 0;
            caretCol = 0;

            Render();
        }
    }

    public string PlaceholderText
    {
        get => _placeholderText;
        set
        {
            _placeholderText = value;
            Render();
        }
    }

    private void MoveCaret(int line, int col)
    {
        if (caretLine == line && caretCol == col) return;
        caretLine = Math.Clamp(line, 0, lines.Count - 1);
        caretCol = Math.Clamp(col, 0, lines[caretLine].Length);
        Render();
    }

    private int GetEndXAtCol(int col)
    {
        if (col > lines[caretLine].Length) col = lines[caretLine].Length;
        string here = lines[caretLine].Substring(0, col);
        return Resources.Font_1x.MeasureString(here);
    }

    internal override void HandleDown(MouseEventArgs args)
    {
        var isDone = false;
        int lineHeight = Resources.Font_1x.Size;

        caretLine = args.Y / lineHeight;
        if (caretLine < 0) caretLine = 0;
        if (caretLine >= lines.Count) caretLine = lines.Count - 1;

        for (var i = 0; i < lines[caretLine].Length; i++)
        {
            var here = lines[caretLine].Substring(0, i);
            int hereWidth = Resources.Font_1x.MeasureString(here) + (Numbers ? 32 : 0);

            if (args.X <= hereWidth && !isDone)
            {
                MoveCaret(caretLine, i);
                isDone = true;
            }
        }

        if (!isDone)
            MoveCaret(caretLine, lines[caretLine].Length);
    }

    internal override void HandleUnfocus()
    {
        caretLine = -1;
        caretCol = 0;

        Render();
    }

    private void AutoScroll()
    {
        if (caretLine == -1) return;

        if (scrollY + Contents.Height < (caretLine + 1) * 20)
            scrollY = (caretLine + 1) * 20 - Contents.Height;
        if (caretLine * 20 < scrollY)
            scrollY = caretLine * 20;
        if (scrollX + Contents.Width < GetEndXAtCol(caretCol))
            scrollX = GetEndXAtCol(caretCol) - Contents.Width;
        if (GetEndXAtCol(caretCol) < scrollX)
            scrollX = GetEndXAtCol(caretCol);
    }

    internal override void HandleKey(KeyEvent key)
    {
        if (caretLine == -1 || ReadOnly) return;

        switch (key.Key)
        {
            case ConsoleKeyEx.LeftArrow:
                if (caretCol == 0)
                {
                    if (caretLine == 0) return;
                    caretLine--;
                    caretCol = lines[caretLine].Length;
                }
                else caretCol--;
                break;

            case ConsoleKeyEx.RightArrow:
                if (caretCol == lines[caretLine].Length)
                {
                    if (caretLine == lines.Count - 1) return;
                    caretLine++;
                    caretCol = 0;
                }
                else caretCol++;
                break;

            case ConsoleKeyEx.UpArrow:
                if (caretLine == 0) return;
                caretLine--;
                caretCol = Math.Min(lines[caretLine].Length, caretCol);
                break;

            case ConsoleKeyEx.DownArrow:
                if (caretLine == lines.Count - 1) return;
                caretLine++;
                caretCol = Math.Min(lines[caretLine].Length, caretCol);
                break;

            case ConsoleKeyEx.Enter:
                if (!MultiLine)
                {
                    Submitted?.Invoke();
                    caretLine = -1;
                    caretCol = 0;
                    break;
                }

                lines.Insert(caretLine + 1, lines[caretLine].Substring(caretCol));
                lines[caretLine] = lines[caretLine].Substring(0, caretCol);
                caretLine++;
                caretCol = 0;
                Changed?.Invoke();
                break;

            case ConsoleKeyEx.Backspace:
                if (caretCol == 0)
                {
                    if (caretLine == 0) return;
                    caretLine--;
                    caretCol = lines[caretLine].Length;
                    lines[caretLine] += lines[caretLine + 1];
                    lines.RemoveAt(caretLine + 1);
                    Changed?.Invoke();
                }
                else
                {
                    lines[caretLine] = lines[caretLine].Remove(caretCol - 1, 1);
                    caretCol--;
                    Changed?.Invoke();
                }
                break;

            default:
                lines[caretLine] = lines[caretLine].Insert(caretCol, key.KeyChar.ToString());
                caretCol++;
                Changed?.Invoke();
                break;
        }

        Render();
    }

    public override void Render()
    {
        AutoScroll();

        if (Image == null)
        {
            Contents.Clear(Color.White);

            // Dark shadow
            Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
            Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

            // Highlight
            Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2,
                new Color(216, 216, 216));
            Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1,
                new Color(216, 216, 216));

            // Light highlight
            Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.White);
            Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.White);
        }
        else
        {
            Contents.DrawImage(0, 0, Image);
        }

        int gutter = Numbers ? 32 : 0;

        if (Text == string.Empty)
        {
            if (Image == null) Contents.DrawRectangle(0, 0, Contents.Width, Contents.Height, 0, Color.DeepGray);
            Contents.DrawString(gutter + 2, 0, PlaceholderText, Resources.Font_1x, Color.LightGray);

            if (caretLine >= 0)
            {
                int care = GetEndXAtCol(caretCol) + gutter;
                if (Image == null) Contents.DrawLine(care, caretLine * 16, care, caretLine * 16 + 16, Color.Black);
            }

            if (Numbers)
            {
                Contents.DrawFilledRectangle(0, 0, 32, Convert.ToUInt16(Contents.Height), 0, new Color(0xFFCCCCCC));
                for (var i = 0; i < Contents.Height / 14; i++)
                    Contents.DrawString(4, i * 14, (i + 1 + lineOffset).ToString(), Resources.Font_1x,
                        Color.LighterBlack);
            }

            Parent.RenderControls();
            return;
        }

        for (var i = 0; i < lines.Count; i++)
            Contents.DrawString(gutter + -scrollX + 2, i * 14,
                Shield ? new string('*', lines[i].Length) : lines[i],
                Resources.Font_1x, Color.Black);

        if (caretLine >= 0)
        {
            int caretTwitter = GetEndXAtCol(caretCol) + gutter;
            Contents.DrawLine(caretTwitter, caretLine * 14, caretTwitter, caretLine * 14 + 16, Color.Black);
        }

        if (Numbers)
        {
            Contents.DrawFilledRectangle(0, 0, 32, Convert.ToUInt16(Contents.Height), 0, new Color(0xFFCCCCCC));
            for (var i = 0; i < Contents.Height / 14; i++)
                if (i + lineOffset < lines.Count)
                    Contents.DrawString(4, i * 14, (i + 1 + lineOffset).ToString(), Resources.Font_1x, Color.LighterBlack);
        }

        Parent.RenderControls();
    }
}
