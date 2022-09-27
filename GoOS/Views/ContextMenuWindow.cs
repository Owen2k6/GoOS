using CitrineUI.Input;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// Allows the user to choose a context menu item.
    /// </summary>
    internal class ContextMenuWindow : Window, IMouseListener
    {
        public ContextMenuWindow(int mouseX, int mouseY, View view, View initialParent) : base(initialParent)
        {
            View = view;
            Rectangle = new Rectangle(
                mouseX,
                mouseY,
                96,
                GetMinimumHeight()
            );
            MakeModal();
            Color = Color.FromArgb(38, 38, 38);
            Parent = initialParent;
        }

        private static PCScreenFont font = PCScreenFont.Default;

        private Pen textPen = new Pen(Color.White);
        /// <summary>
        /// The text colour of the items in the context menu.
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
        /// The view associated with this context menu.
        /// </summary>
        public View View;

        /// <summary>
        /// The height of each item in the context menu.
        /// </summary>
        public int ItemHeight = 20;

        private const int ICON_MARGIN = 3;

        /// <summary>
        /// Get the minimum height to contain all items.
        /// </summary>
        /// <returns>The minimum height to contain all items.</returns>
        public int GetMinimumHeight()
        {
            return ItemHeight * View.ContextMenuItems.Count;
        }

        public void OnClick(int relativeX, int relativeY)
        {
            int index = relativeY / ItemHeight;
            highlight = 1f;
            highlightIndex = index;
            waitingToClose = true;
            Invalid = true;
        }

        // How much the clicked item should be highlighted.
        private float highlight = 0;
        // The index of the item to highlight.
        private int highlightIndex = 0;

        private bool waitingToClose = false;

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            desktop.canvas.DrawFilledRectangle(borderPen, ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);
            desktop.canvas.DrawFilledRectangle(Pen, ScreenBounds.X + 1, ScreenBounds.Y + 1, ScreenBounds.Width - 2, ScreenBounds.Height - 2);
            for (int i = 0; i < View.ContextMenuItems.Count; i++)
            {
                ContextMenuItem item = View.ContextMenuItems[i];
                bool highlightThis = highlightIndex == i;

                if (highlight > 0 && highlightThis)
                {
                    Pen temp = new Pen(desktop.canvas.AlphaBlend(highlightPen.Color, Color, (byte)(highlight * 255)));
                    Desktop.canvas.DrawFilledRectangle(temp, ScreenBounds.X, ScreenBounds.Y + i * ItemHeight, ScreenBounds.Width, ItemHeight);
                    highlight -= 0.05f;
                    Invalid = true;
                }
                if (highlight <= 0 && highlightThis && waitingToClose)
                {
                    item.ClickedHandler?.Invoke(View, new EventArgs());
                    Remove();
                }

                if (item.Icon != null)
                {
                    desktop.canvas.DrawImageAlpha(item.Icon, ScreenBounds.X, (int)(ScreenBounds.Y + ItemHeight / 2 - item.Icon.Height / 2 + i * ItemHeight));
                }

                int x = (int)(ScreenBounds.X + (item.Icon != null ? item.Icon.Width + ICON_MARGIN : 0));
                int y = ScreenBounds.Y + ItemHeight / 2 - font.Height / 2 + i * ItemHeight;
                desktop.canvas.DrawString(item.Name, font, textPen, x, y);
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
