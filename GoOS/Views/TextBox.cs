using CitrineUI.Input;
using CitrineUI.Text;
using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A view that displays text.
    /// </summary>
    public class TextBox : TextView, IKeyListener, IMouseListener
    {
        public TextBox(View initialParent) : base(initialParent)
        {
            text = "";
            Rectangle = new Rectangle(0, 0, 100, 20);
            VAlign = Alignment.Middle;
            Parent = initialParent;

            var cut = new ContextMenuItem("Cut", new Bitmap(cutIconBytes));
            cut.Clicked += Cut;

            var copy = new ContextMenuItem("Copy", new Bitmap(copyIconBytes));
            copy.Clicked += Copy;

            var paste = new ContextMenuItem("Paste", new Bitmap(pasteIconBytes));
            paste.Clicked += Paste;

            ContextMenuItems = new List<ContextMenuItem>() { cut, copy, paste };
        }

        public void Cut(object? sender, EventArgs e)
        {
            if (SelectionStart == SelectionEnd) return;
            var cut = text.Substring(SelectionStart, SelectionEnd - SelectionStart);
            Desktop.Clipboard = text;
            text = text.Remove(SelectionStart, SelectionEnd - SelectionStart);
            SelectionStart = SelectionEnd = 0;
            Invalid = true;
        }
        
        public void Copy(object? sender, EventArgs e)
        {
            if (SelectionStart == SelectionEnd) return;
            var copied = text.Substring(SelectionStart, SelectionEnd - SelectionStart);
            Desktop.Clipboard = copied;
            SelectionStart = SelectionEnd = 0;
            Invalid = true;
        }

        public void Paste(object? sender, EventArgs e)
        {
            if ((Text.Length + Desktop.Clipboard.Length - (SelectionEnd - SelectionStart)) * TextRenderer.Font.Width > ScreenBounds.Width)
            {
                return;
            }
            if (Desktop.Focus != this)
            {
                SelectionStart = SelectionEnd = GetIndexAtCursor();
            }
            if (SelectionStart != SelectionEnd)
            {
                text = text.Remove(SelectionStart, SelectionEnd - SelectionStart);
                SelectionEnd = SelectionStart;
            }
            text = text.Insert(SelectionStart, Desktop.Clipboard);
            SelectionStart = SelectionStart + Desktop.Clipboard.Length;
            SelectionEnd = SelectionStart;
            Invalid = true;
        }

        [ManifestResourceStream(ResourceName = "GoOS.Assets.ContextIcons.cut.bmp")]
        private static byte[] cutIconBytes;
        [ManifestResourceStream(ResourceName = "GoOS.Assets.ContextIcons.copy.bmp")]
        private static byte[] copyIconBytes;
        [ManifestResourceStream(ResourceName = "GoOS.Assets.ContextIcons.paste.bmp")]
        private static byte[] pasteIconBytes;

        private string placeholder = "";
        /// <summary>
        /// The text that will be displayed if the text box is empty.
        /// </summary>
        public string Placeholder
        {
            get { return placeholder; }
            set
            {
                placeholder = value;
                if (text == "")
                {
                    Invalid = true;
                }
            }
        }

        protected Pen pen = new Pen(Color.White);
        /// <summary>
        /// The colour of the background of the text box.
        /// </summary>
        public new Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; Invalid = true; }
        }

        private Pen textPen = new Pen(Color.Black);
        /// <summary>
        /// The colour of the text.
        /// </summary>
        public Color TextColor
        {
            get { return textPen.Color; }
            set { textPen.Color = value; Invalid = true; }
        }

        private Pen borderPen = new Pen(Color.Black);
        /// <summary>
        /// The border colour of the text box.
        /// </summary>
        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; Invalid = true; }
        }

        private Pen placeholderPen = new Pen(Color.Gray);
        /// <summary>
        /// The colour of the placeholder.
        /// </summary>
        public Color PlaceholderColor
        {
            get { return placeholderPen.Color; }
            set
            {
                placeholderPen.Color = value;
                if (text == "" && placeholder != "")
                {
                    Invalid = true;
                }
            }
        }

        private Pen selectionPen = new Pen(Color.FromArgb(137, 176, 255));
        /// <summary>
        /// The background colour of the selection.
        /// </summary>
        public Color SelectionColor
        {
            get { return selectionPen.Color; }
            set
            {
                selectionPen.Color = value;
                if (SelectionStart != SelectionEnd)
                {
                    Invalid = true;
                }
            }
        }

        private void RenderSelection()
        {
            int x = ScreenBounds.X + TextRenderer.Font.Width * SelectionStart + (Text == "" ? CARET_MARGIN : 0);
            int y = ScreenBounds.Y + ScreenBounds.Height / 2 - TextRenderer.Font.Height / 2;
            if (SelectionStart == SelectionEnd)
            {
                if (CaretAnimatedPos != x)
                {
                    Invalid = true;
                    if (Math.Abs(CaretAnimatedPos - x) > TextRenderer.Font.Width)
                    {
                        CaretAnimatedPos = x;
                    }
                    else
                    {
                        if (CaretAnimatedPos < x)
                        {
                            CaretAnimatedPos++;
                        }
                        if (CaretAnimatedPos > x)
                        {
                            CaretAnimatedPos--;
                        }
                    }
                }
                if (CaretAnimatedPos >= ScreenBounds.X + ScreenBounds.Width || CaretAnimatedPos <= ScreenBounds.X) return;
                Desktop.canvas.DrawLine(textPen, CaretAnimatedPos, y, CaretAnimatedPos, y + TextRenderer.Font.Height);
            }
            else
            {
                int width = TextRenderer.Font.Width * (SelectionEnd - SelectionStart);
                if (x + width > ScreenBounds.X + ScreenBounds.Width)
                {
                    width = ScreenBounds.X + ScreenBounds.Width - x;
                }
                Desktop.canvas.DrawFilledRectangle(selectionPen, x, y, width, TextRenderer.Font.Height);
            }
        }

        /// <summary>
        /// The starting index of the selected text.
        /// </summary>
        public int SelectionStart = 0;
        /// <summary>
        /// The ending index of the selected text.
        /// </summary>
        public int SelectionEnd = 0;

        private int CaretAnimatedPos = 0;

        private bool selecting = false;

        private int doubleClickThreshold = 0;

        private const int CARET_MARGIN = 2;
        private const int SELECTION_OFFSET = 3;

        private int GetIndexAtCursor()
        {
            int index = (Desktop.Cursor.Rectangle.X - ScreenBounds.X + SELECTION_OFFSET) / TextRenderer.Font.Width;
            return Math.Clamp(index, 0, Text.Length);
        }

        public void OnMouseDown(int relativeX, int relativeY)
        {
            if (selecting || doubleClickThreshold > 0) return;
            SelectionStart = GetIndexAtCursor();
            SelectionEnd = SelectionStart;
            selecting = true;
            Invalid = true;
        }

        public void OnClick(int relativeX, int relativeY)
        {
            /*doubleClickThreshold = 30;
            if (doubleClickThreshold > 0)
            {
                //SelectionStart = 0;
                //SelectionEnd = text.Length;
                for (int start = SelectionStart; start > 0; start--)
                {
                    if (text[start] == ' ')
                    {
                        start++;
                        for (int end = SelectionStart; end < text.Length; end++)
                        {
                            if (text[end] == ' ')
                            {
                                if (start == SelectionStart && end == SelectionEnd)
                                {
                                    SelectionStart = 0;
                                    SelectionEnd = text.Length;
                                    doubleClickThreshold = 0;
                                    return;
                                }
                                SelectionStart = start;
                                SelectionEnd = end;
                                break;
                            }
                        }
                        break;
                    }
                }
                Invalid = true;
            }*/
        }

        public void OnKeyPressed(KeyEvent key)
        {
            selecting = false;
            Invalid = true;
            switch (key.Key)
            {
                case ConsoleKeyEx.Enter:
                    Desktop.Focus = null;
                    return;
                case ConsoleKeyEx.LeftArrow:
                    SelectionStart = Math.Max(SelectionStart - 1, 0);
                    SelectionEnd = SelectionStart;
                    return;
                case ConsoleKeyEx.RightArrow:
                    SelectionStart = Math.Min(SelectionStart + 1, Text.Length);
                    SelectionEnd = SelectionStart;
                    return;
                case ConsoleKeyEx.Backspace:
                    if (SelectionStart == SelectionEnd)
                    {
                        // If there is no selection, delete the character before the caret.
                        if (SelectionStart > 0)
                        {
                            Text = Text.Remove(SelectionStart - 1, 1);
                            SelectionStart--;
                            SelectionEnd--;
                        }
                    }
                    else
                    {
                        // If there is a selection, delete the selection.
                        Text = Text.Remove(SelectionStart, SelectionEnd - SelectionStart);
                        SelectionEnd = SelectionStart;
                    }
                    break;
                default:
                    // Typing
                    if ((Text.Length + 1) * TextRenderer.Font.Width > ScreenBounds.Width)
                    {
                        return;
                    }
                    if (SelectionStart == SelectionEnd)
                    {
                        // If there is no selection, just insert the character.
                        Text += key.KeyChar;
                        SelectionStart++;
                        SelectionEnd++;
                    }
                    else
                    {
                        // If there is a selection, replace the selection with the character.
                        Text = Text.Remove(SelectionStart, SelectionEnd - SelectionStart);
                        Text = Text.Insert(SelectionStart, key.KeyChar.ToString());
                        SelectionStart++;
                        SelectionEnd = SelectionStart;
                    }
                    break;
            }
        }

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            Invalid = selecting;
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }

            if (doubleClickThreshold > 0)
            {
                Invalid = true;
                doubleClickThreshold--;
            }

            if (selecting)
            {
                if (MouseManager.MouseState != MouseState.Left)
                {
                    selecting = false;
                    return;
                }
                int indexAtCursor = GetIndexAtCursor();
                if (indexAtCursor <= SelectionStart)
                {
                    SelectionStart = indexAtCursor;
                }
                else
                {
                    SelectionEnd = indexAtCursor;
                }
                Invalid = true;
            }
            CalculateScreenBounds();

            desktop.canvas.DrawFilledRectangle(borderPen, ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);
            desktop.canvas.DrawFilledRectangle(pen, ScreenBounds.X + 1, ScreenBounds.Y + 1, ScreenBounds.Width - 2, ScreenBounds.Height - 2);

            bool showPlaceholder = Text == "" && Placeholder != "";
            if (Focused)
            {
                RenderSelection();
            }
            TextRenderer.Render(showPlaceholder ? Placeholder : Text, ScreenBounds, HAlign, VAlign, Desktop.canvas, showPlaceholder ? placeholderPen : textPen);

            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
