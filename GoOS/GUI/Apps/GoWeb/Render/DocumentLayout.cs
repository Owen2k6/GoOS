using GoOS.GUI.Apps.GoWeb.Html;

namespace GoOS.GUI.Apps.GoWeb.Render
{
    public class DocumentLayout
    {
        public DocumentLayout(Document document, int screenWidth)
        {
            _document = document;
            ScreenWidth = screenWidth;
            LayOut();
        }

        public void LayOutRecurse(Element element)
        {;
            if (!element.IsVisible())
            {
                return;
            }

            bool isBlockLevel = element.IsBlockLevel();
            int height = element.GetFont().Size;

            element.X = X;
            element.Y = Y;

            if (X > 0 && isBlockLevel)
            {
                X = 0;
                Y += LineHeight;
                LineHeight = height;
                PermitWhitespace = false;
            }

            if (element is TextNode textNode)
            {
                var glyphRun = new GlyphRun(this, textNode);

                textNode.GlyphRun = glyphRun;
            }

            foreach (Element child in element.Children)
            {
                LayOutRecurse(child);
            }

            if (element is BreakElement || isBlockLevel)
            {
                X = 0;
                Y += height;
                LineHeight = height;
                PermitWhitespace = false;
            }

            if (height > LineHeight)
            {
                LineHeight = height;
            }
        }

        private void LayOut()
        {
            LayOutRecurse(_document.Body);
        }

        readonly Document _document;

        public int X = 0;
        public int Y = 0;
        public int LineHeight = 0;
        public bool PermitWhitespace = false;

        public int ScreenWidth;
    }
}
