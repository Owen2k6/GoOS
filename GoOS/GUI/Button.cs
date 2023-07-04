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

        public bool UseSystemStyle = true;

        public Color BackgroundColour = new Color(12, 12, 12);

        public Color TextColour = Color.White;

        public Button(Window parent, ushort x, ushort y, ushort width, ushort height, string title)
            : base(parent, x, y, width, height)
        {
            Title = title;
        }

        public override void Render()
        {
            if (UseSystemStyle)
            {
                // Background.
                Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, new Color(191, 191, 191));

                // Highlight.
                Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.White);
                Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.White);

                // Light shadow.
                Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2, new Color(127, 127, 127));
                Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(127, 127, 127));

                // Dark shadow.
                Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.Black);
                Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.Black);
            }
            else
            {
                // Background.
                Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, BackgroundColour);
            }

            if (Image != null)
            {
                Contents.DrawImage((Contents.Width - Image.Width) / 2, 0, Image, true);
            }

            // Title.
            int textY = Image != null ? Image.Height + 12 : Contents.Height / 2;
            Color textColour = UseSystemStyle ? Color.Black : TextColour;
            Contents.DrawString(Contents.Width / 2, textY, Title, BetterConsole.font, textColour, true);

            Parent.RenderControls();
        }
    }
}
