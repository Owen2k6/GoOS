using CitrineUI.Text;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A box that can be checked and shows text to its right.
    /// </summary>
    public class CheckBox : View
    {
        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.CheckBox.0.bmp")]
        private static byte[] frame0;
        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.CheckBox.1.bmp")]
        private static byte[] frame1;
        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.CheckBox.2.bmp")]
        private static byte[] frame2;
        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.CheckBox.3.bmp")]
        private static byte[] frame3;
        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.CheckBox.4.bmp")]
        private static byte[] frame4;

        private static Bitmap[] animationFrames = new Bitmap[]
        {
            new Bitmap(frame0),
            new Bitmap(frame1),
            new Bitmap(frame2),
            new Bitmap(frame3),
            new Bitmap(frame4)
        };

        public CheckBox(View initialParent)
        {
            Rectangle = new Rectangle(0, 0, 100, 20);
            Parent = initialParent;
        }

        private Pen pen = new Pen(Color.FromArgb(38, 38, 38));
        /// <summary>
        /// The background colour of the check box.
        /// </summary>
        public Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; Invalid = true; }
        }

        private Pen textPen = new Pen(Color.White);
        /// <summary>
        /// The text colour of the check box.
        /// </summary>
        public Color TextColor
        {
            get { return textPen.Color; }
            set { textPen.Color = value; Invalid = true; }
        }

        private string text = "Check box";
        /// <summary>
        /// The text to show to the right of the check box.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; Invalid = true; }
        }

        private bool @checked = false;
        /// <summary>
        /// Whether or not the check box is checked.
        /// </summary>
        public bool Checked
        {
            get { return @checked; }
            set { @checked = value; Invalid = true; }
        }

        private const int TEXT_MARGIN = 8;

        private Rectangle textBounds = Rectangle.Empty;

        private void CalculateTextBounds()
        {
            textBounds = new Rectangle(
                (int)(ScreenBounds.X + animationFrames[0].Width + TEXT_MARGIN),
                ScreenBounds.Y,
                (int)(Rectangle.Width - (animationFrames[0].Width + TEXT_MARGIN)),
                Rectangle.Height
            );
        }

        private int animationFrame = 0;
        private int frame = 0;

        private const int FRAME_DIVISOR = 5;

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            CalculateTextBounds();

            if (frame % FRAME_DIVISOR == 0)
            {
                int goalFrame = @checked ? animationFrames.Length - 1 : 0;
                if (animationFrame < goalFrame)
                {
                    animationFrame++;
                }
                else if (animationFrame > goalFrame)
                {
                    animationFrame--;
                }
            }
            frame++;

            Bitmap frameBitmap = animationFrames[animationFrame];
            desktop.canvas.DrawImageAlpha(
                frameBitmap,
                ScreenBounds.X,
                (int)(ScreenBounds.Y + (ScreenBounds.Height / 2 - frameBitmap.Height / 2))
            );
            TextRenderer.Render(Text, textBounds, Alignment.Start, Alignment.Middle, Desktop.canvas, textPen);
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
