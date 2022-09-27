using CitrineUI.Text;
using Cosmos.System.Graphics;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A button that provides feedback when clicked.
    /// </summary>
    public class Button : View
    {
        public Button(View initialParent)
        {
            Rectangle = new Rectangle(0, 0, 100, 20);
            Parent = initialParent;
        }

        private Pen pen = new Pen(Color.FromArgb(73, 73, 73));
        /// <summary>
        /// The colour of the button.
        /// </summary>
        public Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; Invalid = true; }
        }

        private Pen highlightPen = new Pen(Color.FromArgb(127, 127, 127));
        /// <summary>
        /// The highlight colour of the button.
        /// </summary>
        public Color HighlightColor
        {
            get { return highlightPen.Color; }
            set { highlightPen.Color = value; Invalid = true; }
        }

        private Pen textPen = new Pen(Color.White);
        /// <summary>
        /// The text colour of the button.
        /// </summary>
        public Color TextColor
        {
            get { return textPen.Color; }
            set { textPen.Color = value; Invalid = true; }
        }

        private Pen borderPen = new Pen(Color.FromArgb(127, 127, 127));
        /// <summary>
        /// The border colour of the button.
        /// </summary>
        public Color BorderColor
        {
            get { return borderPen.Color; }
            set { borderPen.Color = value; Invalid = true; }
        }

        private string text = "Button";
        /// <summary>
        /// The text of the button.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; Invalid = true; }
        }

        private Alignment hAlign = Alignment.Middle;
        /// <summary>
        /// The horizontal alignment of the text.
        /// </summary>
        public Alignment HAlign
        {
            get { return hAlign; }
            set { hAlign = value; Invalid = true; }
        }

        private Alignment vAlign = Alignment.Middle;
        /// <summary>
        /// The vertical alignment of the text.
        /// </summary>
        public Alignment VAlign
        {
            get { return vAlign; }
            set { vAlign = value; Invalid = true; }
        }

        /// <summary>
        /// How much the button should be highlighted.
        /// </summary>
        internal float Highlight = 0;

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            desktop.canvas.DrawFilledRectangle(borderPen, ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);
            
            Pen temp = new Pen(desktop.canvas.AlphaBlend(highlightPen.Color, Color, (byte)(Highlight * 255)));
            desktop.canvas.DrawFilledRectangle(Highlight > 0 ? temp : pen, ScreenBounds.X + 1, ScreenBounds.Y + 1, ScreenBounds.Width - 2, ScreenBounds.Height - 2);
            if (Highlight > 0)
            {
                Highlight -= 0.05f;
                Invalid = true;
            }
            
            TextRenderer.Render(Text, ScreenBounds, hAlign, vAlign, Desktop.canvas, textPen);
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
