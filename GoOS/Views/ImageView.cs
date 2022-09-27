using Cosmos.System.Graphics;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A view that displays an image.
    /// </summary>
    public class ImageView : View
    {
        public ImageView(View initialParent)
        {
            Parent = initialParent;
        }

        private Bitmap image;
        /// <summary>
        /// The image to display. Setting this will automatically resize the view to match the image.
        /// </summary>
        public Bitmap Image
        {
            get { return image; }
            set
            {
                image = value;
                if (image != null)
                {
                    Rectangle = new Rectangle(Rectangle.X, Rectangle.Y, (int)image.Width, (int)image.Height);
                }
                Invalid = true;
            }
        }

        private bool alpha = false;
        /// <summary>
        /// Whether the image has a transparent background.
        /// </summary>
        public bool Alpha
        {
            get { return alpha; }
            set { alpha = value; Invalid = true; }
        }

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            Invalid = false;
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            if (image != null)
            {
                if (alpha)
                {
                    desktop.canvas.DrawImageAlpha(Image, ScreenBounds.X, ScreenBounds.Y);
                }
                else
                {
                    desktop.canvas.DrawImage(Image, ScreenBounds.X, ScreenBounds.Y);
                }
            }
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
