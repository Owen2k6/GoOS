using Gold.Graphics;
using GoOS.GUI.Models;

namespace GoOS.GUI
{
    internal class Button : Control
    {
        public Canvas Image;
        public Canvas PressedImage;
        
        public bool Borderless;
        
        internal string Text;

        internal Button(Window parent, int x, int y, ushort width, ushort height, string text, bool borderless = false, Canvas image = null, Canvas pressedImage = null, bool noOffset = false) : base(parent, x, y, width, height, text, noOffset)
        {
            Text = text;
            RenderWithAlpha = true;
            Image = image;
            PressedImage = pressedImage;
            Borderless = borderless;

            Render();
        }

        internal override void Render()
        {
            if (!Borderless)
            {
                Contents.Clear(new Color(0));
                Contents.DrawFilledRectangle(1, 1, (ushort)(Width - 2), (ushort)(Height - 2), 0,
                    Pressed ? new Color(0xFF666666) : new Color(0xFFE7E7E7));
                Contents.DrawFilledRectangle(3, 3, (ushort)(Width - 4), (ushort)(Height - 4), 0,
                    Pressed ? new Color(0xFF878787) : new Color(0xFFE7E7E7));

                Contents[2, 0] = new Color(0xFF3F3F3F);
                Contents[1, 1] = Color.Black;
                Contents[0, 2] = new Color(0xFF3F3F3F);
                Contents[2, 1] = new Color(Pressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[1, 2] = new Color(Pressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[Width - 3, 0] = new Color(0xFF3F3F3F);
                Contents[Width - 2, 1] = Color.Black;
                Contents[Width - 1, 2] = new Color(0xFF3F3F3F);
                Contents[Width - 3, 1] = new Color(Pressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[Width - 2, 2] = new Color(Pressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[0, Height - 3] = new Color(0xFF3F3F3F);
                Contents[1, Height - 2] = Color.Black;
                Contents[2, Height - 1] = new Color(0xFF3F3F3F);
                Contents[1, Height - 3] = new Color(Pressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[2, Height - 2] = new Color(Pressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[Width - 1, Height - 3] = new Color(0xFF3F3F3F);
                Contents[Width - 2, Height - 2] = Color.Black;
                Contents[Width - 3, Height - 1] = new Color(0xFF3F3F3F);
                Contents[3, 3] = Pressed ? new Color(0xFF777777) : Color.White;
                Contents[Width - 3, Height - 3] = Pressed ? new Color(0xFFA5A5A5) : new Color(0xFF969696);
                Contents[Width - 4, Height - 4] = Pressed ? new Color(0xFF969696) : new Color(0xFFC0C0C0);

                Contents.DrawLine(3, 0, Width - 3, 0, Color.Black);
                Contents.DrawLine(0, 3, 0, Height - 3, Color.Black);
                Contents.DrawLine(Width - 1, 3, Width - 1, Height - 3, Color.Black);
                Contents.DrawLine(3, Height - 1, Width - 3, Height - 1, Color.Black);
                Contents.DrawLine(2, 2, Width - 3, 2, Pressed ? new Color(0xFF777777) : Color.White);
                Contents.DrawLine(2, 2, 2, Height - 3, Pressed ? new Color(0xFF777777) : Color.White);
                Contents.DrawLine(Width - 2, 3, Width - 2, Height - 2,
                    Pressed ? new Color(0xFFA5A5A5) : new Color(0xFF969696));
                Contents.DrawLine(3, Height - 2, Width - 2, Height - 2,
                    Pressed ? new Color(0xFFA5A5A5) : new Color(0xFF969696));
                Contents.DrawLine(Width - 3, 3, Width - 3, Height - 3,
                    Pressed ? new Color(0xFF969696) : new Color(0xFFC0C0C0));
                Contents.DrawLine(3, Height - 3, Width - 3, Height - 3,
                    Pressed ? new Color(0xFF969696) : new Color(0xFFC0C0C0));
            }

            if (Image != null)
            {
                int imageX = NoOffset ? 0 : (Contents.Width - Image.Width) / 2;
                int imageY = 0;
                
                if (Pressed && PressedImage != null)    
                    Contents.DrawImage(imageX, imageY, PressedImage);
                else
                    Contents.DrawImage(imageX, imageY, Image);
            }

            if (Text != "")
            {
                Contents.DrawString((Width / 2) - (Resources.Charcoal.MeasureString(Text) / 2) - 1,
                    (Height / 2) - (Resources.Charcoal.GetHeight() / 2) - 1,
                    Text, Resources.Charcoal, Pressed ? Color.White : Color.Black);
            }

            base.Render();
        }

        internal override void HandleDown()
        {
            base.HandleDown();
            Render();
        }

        internal override void HandleUp()
        {
            base.HandleUp();
            Render();
        }
    }
    
    /*public class Button : Control
    {
        
        
        

        public string Title;

        /// <summary>
        /// Optional image.
        /// </summary>
        public Canvas Image;

        public bool UseSystemStyle = true;

        public Color BackgroundColour = new Color(12, 12, 12);

        public Color SelectionColour = new Color(12, 12, 12);

        public bool HasSelectionColour = false;

        public Color TextColour = Color.White;

        public bool pressed = false;

        public bool CenterTitle = true;

        public bool AppearPressed = false;

        public int textX = 0;

        public int textY = 0;

        private const int PUSH_DEPTH = 1;

        public byte lastSecondClicked;

        public Button(Window parent, ushort x, ushort y, ushort width, ushort height, string title)
            : base(parent, x, y, width, height)
        {
            Title = title;
        }

        public override void Render()
        {
            if (UseSystemStyle)
            {
                if (AppearPressed || pressed)
                {
                    // Background.
                    Contents.Clear(new Color(71, 71, 71));

                    // Dark shadow.
                    Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
                    Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

                    // Highlight.
                    Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2,
                        new Color(80, 80, 80));
                    Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1,
                        new Color(80, 80, 80));

                    // Light highlight.
                    Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1,
                        new Color(89, 89, 89));
                    Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1,
                        new Color(89, 89, 89));
                }
                else
                {
                    // Background.
                    Contents.Clear(new Color(71, 71, 71));

                    // Highlight.
                    Contents.DrawLine(0, 0, Contents.Width - 1, 0, new Color(80, 80, 80));
                    Contents.DrawLine(0, 0, 0, Contents.Height - 1, new Color(80, 80, 80));

                    // Light shadow.
                    Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2,
                        new Color(89, 89, 89));
                    Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1,
                        new Color(89, 89, 89));

                    // Dark shadow.
                    Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.Black);
                    Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.Black);
                }
            }
            else
            {
                if (IsMouseOver && HasSelectionColour)
                {
                    // Selection.
                    Contents.Clear(SelectionColour);
                }
                else
                {
                    // Background.
                    Contents.Clear(BackgroundColour);
                }
            }

            if (Image != null)
            {
                int imageX = (Contents.Width - Image.Width) / 2;
                int imageY = 0;

                if ((AppearPressed || pressed) && UseSystemStyle)
                {
                    imageX += PUSH_DEPTH;
                    imageY += PUSH_DEPTH;
                }

                Contents.DrawImage(imageX, imageY, Image, true);
            }

            // Title.

            if (textX == 0)
                textX = Contents.Width / 2;

            if (textY == 0)
                textY = Image != null ? Image.Height + 12 : Contents.Height / 2;

            if ((AppearPressed || pressed) && UseSystemStyle)
            {
                textX += PUSH_DEPTH;
                textY += PUSH_DEPTH;
            }

            Color textColour = UseSystemStyle ? Color.White : TextColour;

            Contents.DrawString(textX, textY, Title, Resources.Font_1x, textColour, CenterTitle);

            if ((AppearPressed || pressed) && UseSystemStyle)
            {
                textX -= PUSH_DEPTH;
                textY -= PUSH_DEPTH;
            }

            Parent.RenderControls();
        }

        private uint lastMouseX, lastMouseY;

        public override void Update()
        {
            if (IsMouseOver && Visible && Parent.Visible && (Parent.Focused || Parent.Title == nameof(Apps.Desktop)))
            {
                WindowManager.MouseToDraw = mouseClick;
            }

            if (lastMouseX != MouseManager.X || lastMouseY != MouseManager.Y)
            {
                if (!UseSystemStyle && HasSelectionColour)
                {
                    Render();
                }
            }

            lastMouseX = MouseManager.X;
            lastMouseY = MouseManager.Y;
        }

        internal override void HandleDown(MouseEventArgs _args)
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
    }*/
}