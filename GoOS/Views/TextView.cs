using CitrineUI.Text;
using Cosmos.System.Graphics;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A view that displays text.
    /// </summary>
    public class TextView : View
    {
        public TextView(View initialParent)
        {
            Rectangle = new Rectangle(0, 0, 100, 20);
            Parent = initialParent;
        }

        private Pen pen = new Pen(Color.White);
        /// <summary>
        /// The colour of the text.
        /// </summary>
        public Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; Invalid = true; }
        }

        protected string text = "Text";
        /// <summary>
        /// The text.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; Invalid = true; }
        }

        private Alignment hAlign = Alignment.Start;
        /// <summary>
        /// The horizontal alignment of the text.
        /// </summary>
        public Alignment HAlign
        {
            get { return hAlign; }
            set { hAlign = value; Invalid = true; }
        }

        private Alignment vAlign = Alignment.Start;
        /// <summary>
        /// The vertical alignment of the text.
        /// </summary>
        public Alignment VAlign
        {
            get { return vAlign; }
            set { vAlign = value; Invalid = true; }
        }

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            Invalid = false;
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();
            TextRenderer.Render(Text, ScreenBounds, hAlign, vAlign, Desktop.canvas, pen);
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
