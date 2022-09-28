using CitrineUI.Input;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using System;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// Renders the list of items in a ListBox.
    /// </summary>
    internal class ListBoxSelection : RectangleView, IMouseListener
    {
        public ListBoxSelection(View initialParent) : base(initialParent)
        {
            Color = Color.FromArgb(38, 38, 38);
            Parent = initialParent;
        }

        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.listCheck.bmp")]
        private static byte[] checkBytes;
        private static Bitmap checkBitmap = new Bitmap(checkBytes);

        private static PCScreenFont font = PCScreenFont.Default;

        private Pen textPen = new Pen(Color.White);
        /// <summary>
        /// The text colour of the items in the list box.
        /// </summary>
        public Color TextColor
        {
            get { return textPen.Color; }
            set { textPen.Color = value; Invalid = true; }
        }

        private Pen highlightPen = new Pen(Color.FromArgb(127, 127, 127));
        /// <summary>
        /// The colour that is used to highlight clicked items.
        /// </summary>
        public Color HighlightColor
        {
            get { return highlightPen.Color; }
            set { highlightPen.Color = value; Invalid = true; }
        }

        private Pen borderPen = new Pen(Color.FromArgb(127, 127, 127));
        /// <summary>
        /// The border colour of the list.
        /// </summary>
        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; Invalid = true; }
        }

        /// <summary>
        /// The ListBox associated with this selection.
        /// </summary>
        public ListBox ListBox;

        /// <summary>
        /// The height of each item in the list.
        /// </summary>
        public int ItemHeight = 20;

        private const int CHECK_MARGIN = 3;

        public override bool FillWidth
        {
            get { return true; }
            set { throw new NotImplementedException(); }
        }
        public override bool FillHeight
        {
            get { return true; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get the minimum height to contain all items.
        /// </summary>
        /// <returns>The minimum height to contain all items.</returns>
        public int GetMinimumHeight()
        {
            return ItemHeight * ListBox.Items.Count;
        }

        public void OnClick(int relativeX, int relativeY)
        {
            int index = relativeY / ItemHeight;
            if (index < ListBox.Items.Count)
            {
                ListBox.SelectedIndex = index;
                highlight = 1f;
                waitingToClose = true;
                Invalid = true;
            }
        }

        // How much the newly selected item should be highlighted.
        private float highlight = 0;

        private bool waitingToClose = false;

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            desktop.canvas.DrawFilledRectangle(borderPen, ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);
            desktop.canvas.DrawFilledRectangle(pen, ScreenBounds.X + 1, ScreenBounds.Y + 1, ScreenBounds.Width - 2, ScreenBounds.Height - 2);
            for (int i = 0; i < ListBox.Items.Count; i++)
            {
                bool currentlySelected = ListBox.SelectedIndex == i;

                if (highlight > 0 && currentlySelected)
                {
                    Pen temp = new Pen(desktop.canvas.AlphaBlend(highlightPen.Color, Color, (byte)(highlight * 255)));
                    Desktop.canvas.DrawFilledRectangle(temp, ScreenBounds.X, ScreenBounds.Y + i * ItemHeight, ScreenBounds.Width, ItemHeight);
                    highlight -= 0.05f;
                }
                if (highlight <= 0 && waitingToClose)
                {
                    if (Parent is Window window)
                    {
                        window.Remove();
                    }
                    ListBox.SelectionFinished();
                }

                if (currentlySelected)
                {
                    desktop.canvas.DrawImageAlpha(checkBitmap, ScreenBounds.X, (int)(ScreenBounds.Y + ItemHeight / 2 - checkBitmap.Height / 2 + i * ItemHeight));
                }

                int x = (int)(ScreenBounds.X + (currentlySelected ? checkBitmap.Width + CHECK_MARGIN : 0));
                int y = ScreenBounds.Y + ItemHeight / 2 - font.Height / 2 + i * ItemHeight;
                desktop.canvas.DrawString(ListBox.Items[i], font, textPen, x, y);
            }
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
