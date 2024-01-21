using GoOS.GUI.Apps.GoWeb.Html;
using System;
using System.Linq;

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

        private void LayOut()
        {
            _document.Body.LayOut(this);
        }

        readonly Document _document;

        public int X = 0;
        public int Y = 0;
        public int LineHeight = 0;
        public bool PermitWhitespace = false;

        public int ScreenWidth;
    }
}
