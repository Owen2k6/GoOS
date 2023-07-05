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

        public bool pressed = false;

        private const int PUSH_DEPTH = 1;

        public Button(Window parent, ushort x, ushort y, ushort width, ushort height, string title)
            : base(parent, x, y, width, height)
        {
            Title = title;
        }

        public override void Render()
        {
            if (UseSystemStyle)
            {
                if (pressed)
                {
                    // Background.
                    Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, new Color(191, 191, 191));

                    // Dark shadow.
                    Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
                    Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

                    // Highlight.
                    Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2, new Color(216, 216, 216));
                    Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(216, 216, 216));

                    // Light highlight.
                    Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.White);
                    Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.White);
                }
                else
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
            }
            else
            {
                // Background.
                Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, BackgroundColour);
            }

            if (Image != null)
            {
                int imageX = (Contents.Width - Image.Width) / 2;
                int imageY = 0;

                if (pressed && UseSystemStyle)
                {
                    imageX += PUSH_DEPTH;
                    imageY += PUSH_DEPTH;
                }

                Contents.DrawImage(imageX, imageY, Image, true);
            }

            // Title.
            int textX = Contents.Width / 2;
            int textY = Image != null ? Image.Height + 12 : Contents.Height / 2;

            if (pressed && UseSystemStyle)
            {
                textX += PUSH_DEPTH;
                textY += PUSH_DEPTH;
            }

            Color textColour = UseSystemStyle ? Color.Black : TextColour;

            Contents.DrawString(textX, textY, Title, BetterConsole.font, textColour, true);

            Parent.RenderControls();
        }

        internal override void HandleDown()
        {
            pressed = true;
            Render();
            Parent.RenderControls();
        }

        internal override void HandleRelease()
        {
            pressed = false;
            Render();
            Parent.RenderControls();
        }
    }
}
