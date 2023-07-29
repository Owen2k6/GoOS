using System;
using Cosmos.System;
using GoOS.GUI.Models;
using PrismAPI.Graphics;

namespace GoOS.GUI
{
    public class List : Control
    {
        public string[] Items;
        public string Title;

        public int Selected;

        public Color ForegroundColor = Color.Black;
        public Color BackgroundColor = Color.White;

        public List(Window parent, ushort x, ushort y, ushort width, ushort height, string title, string[] items)
            : base(parent, x, y, width, height)
        {
            Items = items;
            Title = title;
            Selected = 0;
        }

        public override void Render()
        {
            if (Selected < 0)
            {
                Selected = 0;
            }
            else if (Selected > Items.Length)
            {
                Selected = Items.Length - 1;
            }

            Contents.Clear(BackgroundColor);

            // Background.
            Contents.Clear(Color.White);

            // Dark shadow.
            Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
            Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

            // Highlight.
            Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2, new Color(216, 216, 216));
            Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(216, 216, 216));

            // Light highlight.
            Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.White);
            Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.White);

            if (Title != "")
            {
                // Title
                Contents.DrawLine(1, 19, Contents.Width - 2, 19, new Color(216, 216, 216));
                Contents.DrawString(2, 2, Title, BetterConsole.font, ForegroundColor);
            }

            // Items
            for (int i = 0; i < Items.Length; i++)
            {
                if (i == Selected)
                {
                    Contents.DrawFilledRectangle(1, 20 + (i * 20), Convert.ToUInt16(Contents.Width - 3), 18, 0, new Color(234, 234, 234));
                }

                Contents.DrawString(2, 21 + (i * 20), Items[i], BetterConsole.font, ForegroundColor);
            }

            Parent.RenderControls();
        }

        internal override void HandleDown(MouseEventArgs args)
        {
            if (IsMouseOver)
            {
                Selected = ((int)MouseManager.Y - Parent.Y + Window.TITLE_BAR_HEIGHT + Y) / 20 - 4;
            }

            Render();
            Parent.RenderControls();
        }
    }
}
