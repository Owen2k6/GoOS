using PrismAPI.Graphics;
using System.ComponentModel;

namespace GoOS.GUI
{
    public class Button : Control
    {
        public string Title;

        /// <summary>
        /// Optional image.
        /// </summary>
        public Canvas Image;

        public Color BackgroundColour = new Color(12, 12, 12);

        public Color TextColour = Color.White;

        public Button(Window parent, ushort x, ushort y, ushort width, ushort height, string title)
            : base(parent, x, y, width, height)
        {
            Title = title;
        }

        public override void Render()
        {
            // Background.
            Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, BackgroundColour);

            if (Image != null)
            {
                Contents.DrawImage((Contents.Width - Image.Width) / 2, 0, Image, true);
            }

            // Title.
            int textY = Image != null ? Image.Height + 8 : 0;
            Contents.DrawString(Contents.Width / 2, textY, Title, BetterConsole.font, TextColour, true);

            Parent.RenderControls();
        }
    }
}
