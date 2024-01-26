using System;
using Cosmos.System;
using GoOS.GUI.Models;
using GoGL.Graphics;

namespace GoOS.GUI
{
    public class List : Control
    {
        public string[] Items;
        public string Title;

        public int Selected;

        public Color ForegroundColor = Color.White;
        public Color BackgroundColor = new Color(143, 143, 143);

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
            Contents.Clear(Color.LightGray);

            // Dark shadow.
            Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
            Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

            // Highlight.
            Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2,
                new Color(130, 130, 130));
            Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(130, 130, 130));

            // Light highlight.
            Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.White);
            Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.White);

            //So much for saying your code is "dry". this is from Button.cs - Owen2k6 :^(
            //That was atmo's :skull: nmf

            if (Title != "")
            {
                // Title
                Contents.DrawLine(0, 19, Contents.Width - 2, 19, new Color(216, 216, 216));
                /*Contents.DrawLine(0, 20, Contents.Width - 2, 19, new Color(216, 216, 216));
                Contents.DrawLine(0, 21, Contents.Width - 2, 19, new Color(216, 216, 216)); // fuck you owen*/
                Contents.DrawString(2, 2, Title, Resources.Font_1x, ForegroundColor);
            }

            // Items
            for (int i = 0; i < Items.Length; i++)
            {
                if (i == Selected)
                {
                    Contents.DrawFilledRectangle(1, 20 + (i * 20), Convert.ToUInt16(Contents.Width - 3), 18, 0,
                        new Color(130, 130, 130));
                    //TODO: XRC you fucking dumbass label areas where you force color changes.
                    //TODO: For example, you could just simply put //Creates a WHITE rectangle.
                    //TODO: This fucked up my theme changing work just cus you made it unnoticeable.
                    //-Owen2k6 >:^(
                    Contents.DrawString(2, 21 + (i * 20), Items[i], Resources.Font_1x, Color.Black);
                }
                else Contents.DrawString(2, 21 + (i * 20), Items[i], Resources.Font_1x, ForegroundColor);
            }

            Parent.RenderControls();
        }

        internal override void HandleDown(MouseEventArgs args)
        {
            if (IsMouseOver)
            {
                Selected = (((int)MouseManager.Y - Parent.Y - Window.TITLE_BAR_HEIGHT - Y) / 20) - 1;
            }

            Render();
            Parent.RenderControls();
        }
    }
}