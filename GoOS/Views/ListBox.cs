using CitrineUI.Input;
using CitrineUI.Text;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A view that allows a user to select one item from a list.
    /// </summary>
    public class ListBox : Button, IMouseListener
    {
        public ListBox(View initialParent) : base(initialParent)
        {
            Rectangle = new Rectangle(0, 0, 100, 20);
            Parent = initialParent;
        }

        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.listArrow.bmp")]
        private static byte[] arrowBytes;
        private static Bitmap arrowBitmap = new Bitmap(arrowBytes);

        private Pen pen = new Pen(Color.FromArgb(73, 73, 73));
        /// <summary>
        /// The colour of the list box.
        /// </summary>
        public new Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; Invalid = true; }
        }

        private Pen highlightPen = new Pen(Color.FromArgb(127, 127, 127));
        /// <summary>
        /// The highlight colour of the list box.
        /// </summary>
        public new Color HighlightColor
        {
            get { return highlightPen.Color; }
            set { highlightPen.Color = value; Invalid = true; }
        }

        private Pen textPen = new Pen(Color.White);
        /// <summary>
        /// The text colour of the list box.
        /// </summary>
        public new Color TextColor
        {
            get { return textPen.Color; }
            set { textPen.Color = value; Invalid = true; }
        }

        private Pen borderPen = new Pen(Color.FromArgb(127, 127, 127));
        /// <summary>
        /// The border colour of the list box.
        /// </summary>
        public new Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; Invalid = true; }
        }

        /// <summary>
        /// The name of the currently selected item.
        /// </summary>
        public new string Text
        {
            get { return Items[SelectedIndex]; }
            set { throw new NotImplementedException("Cannot set the text of a ListBox."); }
        }

        private Alignment hAlign = Alignment.Start;

        private Alignment vAlign = Alignment.Middle;

        /// <summary>
        /// The items to choose from.
        /// </summary>
        public List<string> Items = new List<string>();

        /// <summary>
        /// The index of the currently selected item.
        /// </summary>
        public int SelectedIndex = 0;

        /// <summary>
        /// The padding around the left and right of the list box.
        public int HorizontalPadding = 3;

        public void OnClick(int relativeX, int relativeY)
        {
            Window selectionWindow = new Window(Desktop);
            selectionWindow.MakeModal();
            ListBoxSelection selection = new ListBoxSelection(selectionWindow);
            selection.ListBox = this;
            selectionWindow.Rectangle = new Rectangle(ScreenBounds.X, ScreenBounds.Y + ScreenBounds.Height, 100, selection.GetMinimumHeight());
            Invalid = true;
        }

        internal void SelectionFinished()
        {
            Invalid = true;
        }

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            if (Items.Count == 0)
            {
                throw new Exception("List boxes cannot be rendered without items.");
            }
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            desktop.canvas.DrawFilledRectangle(borderPen, ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);

            Pen temp = new Pen(desktop.canvas.AlphaBlend(highlightPen.Color, Color, (byte)(Highlight * 255)));
            desktop.canvas.DrawFilledRectangle(Highlight > 0 ? temp : pen, ScreenBounds.X + 1, ScreenBounds.Y + 1, ScreenBounds.Width - 2, ScreenBounds.Height - 2);
            if (Highlight > 0)
            {
                Highlight -= 0.05f;
                Invalid = true;
            }

            Rectangle textBounds = new Rectangle(ScreenBounds.X + HorizontalPadding, ScreenBounds.Y, ScreenBounds.Width - HorizontalPadding * 2, ScreenBounds.Height);
            TextRenderer.Render(Text, textBounds, hAlign, vAlign, Desktop.canvas, textPen);

            desktop.canvas.DrawImage(arrowBitmap, (int)(ScreenBounds.X + ScreenBounds.Width - arrowBitmap.Width) - HorizontalPadding, (int)(ScreenBounds.Y + (ScreenBounds.Height - arrowBitmap.Height) / 2));
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }

        public void OnMouseDown(int relativeX, int relativeY)
        {
            return;
        }
    }
}
