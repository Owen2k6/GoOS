using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;

namespace CitrineUI.Text
{
    public static class TextRenderer
    {
        /// <summary>
        /// The system font.
        /// </summary>
        public static PCScreenFont Font;

        /*[ManifestResourceStream(ResourceName = "CitrineUI.Assets.Fonts.TTF.OpenSans.ttf")]
        private static byte[] openSansBytes;*/

        private static readonly Pen debugPen = new Pen(Color.Red);

        private const bool SHOW_DEBUG_BOUNDS = false;
        private const bool TTF_ENABLED = false;

        /// <summary>
        /// Measure the size of text.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>The size of the text.</returns>
        public static Size Measure(string text)
        {
            text = text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');

            int textBoundsWidth = 0;
            int textBoundsHeight = lines.Length * Font.Height;

            for (int i = 0; i < lines.Length; i++)
            {
                textBoundsWidth = Math.Max(textBoundsWidth, Font.Width * lines[i].Length);
            }
            return new Size(textBoundsWidth, textBoundsHeight);
        }

        /// <summary>
        /// Load the system fonts.
        /// </summary>
        public static void Initialize()
        {
            Font = PCScreenFont.Default;
        }

        /// <summary>
        /// Render multiline text in a specific rectangle with alignment options.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="rectangle">The rectangle which the text should be contained in.</param>
        /// <param name="hAlign">The horizontal alignment of the text.</param>
        /// <param name="vAlign">The vertical alignment of the text.</param>
        /// <param name="canvas">The canvas to render the text on.</param>
        /// <param name="pen">The pen to use when rendering the text.</param>
        public static void Render(string text, Rectangle rectangle, Alignment hAlign, Alignment vAlign, Canvas canvas, Pen pen)
        {
            text = text.Replace("\r\n", "\n");
            string[] lines = text.Split('\n');

            int textBoundsX = 0;
            int textBoundsY = 0;
            int textBoundsWidth = 0;
            int textBoundsHeight = lines.Length * Font.Height;

            for (int i = 0; i < lines.Length; i++)
            {
                textBoundsWidth = Math.Max(textBoundsWidth, Font.Width * lines[i].Length);
            }
            switch (hAlign)
            {
                case Alignment.Start:
                    textBoundsX = rectangle.X;
                    break;
                case Alignment.Middle:
                    textBoundsX = rectangle.X + (rectangle.Width / 2 - textBoundsWidth / 2);
                    break;
                case Alignment.End:
                    textBoundsX = rectangle.X + rectangle.Width - textBoundsWidth;
                    break;
            }
            switch (vAlign)
            {
                case Alignment.Start:
                    textBoundsY = rectangle.Y;
                    break;
                case Alignment.Middle:
                    textBoundsY = rectangle.Y + (rectangle.Height / 2 - textBoundsHeight / 2);
                    break;
                case Alignment.End:
                    textBoundsY = rectangle.Y + rectangle.Height - textBoundsHeight;
                    break;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                int lineWidth = lines[i].Length * Font.Width;
                int lineX = 0;
                int lineY = textBoundsY + i * Font.Height;
                switch (hAlign)
                {
                    case Alignment.Start:
                        lineX = textBoundsX;
                        break;
                    case Alignment.Middle:
                        lineX = textBoundsX + (textBoundsWidth - lineWidth) / 2;
                        break;
                    case Alignment.End:
                        lineX = rectangle.Width - lineWidth;
                        break;
                }
                if (TTF_ENABLED)
                {
                    //canvas.DrawStringTTF(pen, lines[i], "Open Sans", 16f, new Cosmos.System.Graphics.Point(lineX, lineY), 1);
                }
                else
                {
                    canvas.DrawString(lines[i], Font, pen, lineX, lineY);
                }
            }

#pragma warning disable CS0162
            if (SHOW_DEBUG_BOUNDS)
            {
                canvas.DrawRectangle(debugPen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                canvas.DrawRectangle(debugPen, textBoundsX, textBoundsY, textBoundsWidth, textBoundsHeight);
            }
#pragma warning restore CS0162
        }
    }
}
