using Cosmos.System.Graphics;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A basic rectangle.
    /// </summary>
    public class RectangleView : View
    {
        public RectangleView(View initialParent)
        {
            Rectangle = new Rectangle(0, 0, 100, 100);
            Parent = initialParent;
        }

        protected Pen pen = new Pen(Color.Black);
        /// <summary>
        /// The colour of the rectangle.
        /// </summary>
        public Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; Invalid = true; }
        }

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            Invalid = false;
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            desktop.canvas.DrawFilledRectangle(pen, ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
