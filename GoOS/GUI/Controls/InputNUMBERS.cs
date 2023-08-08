using Cosmos.System;
using PrismAPI.Graphics.Rasterizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.System.Graphics.Fonts;
using PrismAPI.Graphics;
using GoOS.GUI.Models;

namespace GoOS.GUI
{
    public class InputNUMBERS : Control
    {
        public InputNUMBERS(Window parent, ushort x, ushort y, ushort width, ushort height, string placeholder)
            : base(parent, x, y, width, height)
        {
            PlaceholderText = placeholder;
        }

        public Action Submitted;
        public Action Changed;

        private int lineOffset = 0;

        private const int padding = 3;

        private bool scrollMode = false;

        public string Text
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < lines.Count; i++)
                {
                    builder.Append(lines[i]);
                    if (i != lines.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }

                return builder.ToString();
            }
            set
            {
                lines = value.Split('\n').ToList();

                caretLine = -1;
                caretCol = 0;

                Render();
            }
        }

        private string _placeholderText = string.Empty;

        public string PlaceholderText
        {
            get { return _placeholderText; }
            set
            {
                _placeholderText = value;
                Render();
            }
        }

        public bool ReadOnly { get; set; } = false;

        // Todo.
        public bool MultiLine { get; set; } = false;

        public bool Shield { get; set; } = false;

        private void MoveCaret(int line, int col)
        {
            if (caretLine == line && caretCol == col) return;
            caretLine = Math.Clamp(line, 0, lines.Count - 1);
            caretCol = Math.Clamp(col, 0, lines[caretLine].Length);
            Render();
        }

        private int GetEndXAtCol(int col)
        {
            string here = lines[caretLine].Substring(0, col);
            return BetterConsole.font.MeasureString(here);
        }

        internal override void HandleDown(MouseEventArgs args)
        {
            caretLine = 0;
            for (int i = 0; i < lines[caretLine].Length; i++)
            {
                string here = lines[caretLine].Substring(0, i);
                int hereWidth = BetterConsole.font.MeasureString(here);
                if (args.X <= hereWidth)
                {
                    MoveCaret(0, i);
                    return;
                }
            }
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
            {
                // Scroll up.
                scrollY = ((caretLine + 1) * 20) - Contents.Height;
                //MarkAllLines();
            }

            if (caretLine * 20 < scrollY)
            {
                // Scroll down.
                scrollY = caretLine * 20;
                //MarkAllLines();
            }

            if (scrollX + Contents.Width < GetEndXAtCol(caretCol))
            {
                // Scroll right.
                scrollX = GetEndXAtCol(caretCol) - Contents.Width;
            }

            if (GetEndXAtCol(caretCol) < scrollX)
            {
                // Scroll left.
                scrollX = GetEndXAtCol(caretCol);
            }
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
                        caretCol = lines[caretLine + lineOffset].Length;
                    }
                    else
                    {
                        caretCol--;
                    }

                    break;
                case ConsoleKeyEx.RightArrow:
                    if (caretCol == lines[caretLine + lineOffset].Length)
                    {
                        if (caretLine == lines.Count - 1) return;
                        caretLine++;

                        caretCol = 0;
                    }
                    else
                    {
                        caretCol++;
                    }

                    break;
                case ConsoleKeyEx.UpArrow:
                    if (!scrollMode)
                    {
                        if (caretLine == 0) return;

                        caretLine--;
                        caretCol = Math.Min(lines[caretLine + lineOffset].Length, caretCol);

                        if (caretLine <= 1 && lineOffset >= 1)
                        {
                            lineOffset--;
                            caretLine = 1;
                            caretCol = Math.Min(lines[caretLine + lineOffset].Length, caretCol);
                        }

                        break;
                    }
                    
                    if (lineOffset >= 1)
                    {
                        lineOffset--;
                        caretLine = 1;
                        caretCol = Math.Min(lines[caretLine + lineOffset].Length, caretCol);
                    }

                    break;
                case ConsoleKeyEx.DownArrow:
                    if (!scrollMode)
                    {
                        if (caretLine == lines.Count - 1) return;

                        caretLine++;
                        caretCol = Math.Min(lines[caretLine + lineOffset].Length, caretCol);

                        if (caretLine >= Contents.Height / 14)
                        {
                            lineOffset++;
                            caretLine--;
                            caretCol = Math.Min(lines[caretLine + lineOffset].Length, caretCol);
                        }

                        break;
                    }

                    lineOffset++;
                    caretLine--;
                    caretCol = Math.Min(lines[caretLine + lineOffset].Length, caretCol);

                    break;

                case ConsoleKeyEx.RCtrl:
                    if (scrollMode)
                        scrollMode = false;
                    else
                        scrollMode = true;
                    break;
                case ConsoleKeyEx.Enter:
                    if (!MultiLine)
                    {
                        Submitted?.Invoke();

                        caretLine = -1;
                        caretCol = 0;

                        break;
                    }

                    lines.Insert(caretLine + 1 + lineOffset, lines[caretLine + lineOffset].Substring(caretCol));
                    lines[caretLine + lineOffset] = lines[caretLine + lineOffset].Substring(0, caretCol);
                    // 
                    caretLine++;
                    caretCol = 0;

                    if (caretLine >= Contents.Height / 14)
                    {
                        lineOffset++;
                        caretLine--;
                        caretCol = Math.Min(lines[caretLine + lineOffset].Length, caretCol);
                    }

                    // 
                    Changed?.Invoke();
                    break;
                case ConsoleKeyEx.Backspace:
                    if (caretCol == 0)
                    {
                        if (caretLine == 0) return;

                        caretLine--;
                        caretCol = lines[caretLine + lineOffset].Length;

                        lines[caretLine + lineOffset] += lines[caretLine + lineOffset + 1];
                        lines.RemoveAt(caretLine + lineOffset + 1);

                        Changed?.Invoke();
                    }
                    else
                    {
                        lines[caretLine + lineOffset] = lines[caretLine + lineOffset].Remove(caretCol - 1, 1);
                        caretCol--;

                        Changed?.Invoke();
                    }

                    break;
                default:
                    lines[caretLine + lineOffset] = lines[caretLine + lineOffset].Insert(caretCol, key.KeyChar.ToString());
                    caretCol++;

                    Changed?.Invoke();
                    break;
            }

            Render();
        }

        private List<string> lines = new List<string>() { string.Empty };

        private int caretLine = 0;
        private int caretCol = 0;

        private int scrollX = 0;
        private int scrollY = 0;

        public override void Render()
        {
            AutoScroll();

            // Background.
            Contents.Clear(Color.White);

            // Dark shadow.
            Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
            Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

            // Highlight.
            Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2,
                new Color(216, 216, 216));
            Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(216, 216, 216));

            // Light highlight.
            Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.White);
            Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.White);

            if (Text == string.Empty)
            {
                Contents.DrawRectangle(0, 0, Contents.Width, Contents.Height, 0, Color.DeepGray);
                Contents.DrawString(0, 0, PlaceholderText, BetterConsole.font, Color.LightGray);

                Contents.DrawLine(34, caretLine * 14, 34, caretLine * 14 + 14, Color.Black);

                Contents.DrawFilledRectangle(0, 0, 32, Contents.Height, 0, Color.LightGray);

                for (int i = 0; i < Contents.Height / 14; i++)
                {
                    Contents.DrawString(4, i * 14, (i + 1 + lineOffset).ToString(), BetterConsole.font,
                        Color.LighterBlack);
                }

                Parent.RenderControls();
                return;
            }

            for (var i = 0; i + lineOffset < lines.Count; i++)
            {
                Contents.DrawString(32 + (-scrollX), i * 14,
                    Shield ? new string('*', lines[i + lineOffset].Length) : lines[i + lineOffset], BetterConsole.font,
                    Color.Black);
            }

            int caretTwitter = GetEndXAtCol(caretCol) + 34;
            Contents.DrawLine(caretTwitter, caretLine * 14, caretTwitter, caretLine * 14 + 14, Color.Black);

            Contents.DrawFilledRectangle(0, 0, 32, Contents.Height, 0, Color.LightGray);

            for (int i = 0; i < Contents.Height / 14; i++)
            {
                Contents.DrawString(4, i * 14, (i + 1 + lineOffset).ToString(), BetterConsole.font, Color.LighterBlack);
            }

            Parent.RenderControls();
        }
    }
}