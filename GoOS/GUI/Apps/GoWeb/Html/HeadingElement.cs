using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI.Apps.GoWeb.Html
{
    public class HeadingElement : Element
    {
        public enum HeadingLevel
        {
            H1, H2, H3, H4, H5, H6,
        }

        public HeadingElement(HeadingLevel level)
        {
            _level = level;
        }

        public override string GetTag()
        {
            return _level switch
            {
                HeadingLevel.H1 => "h1",
                HeadingLevel.H2 => "h2",
                HeadingLevel.H3 => "h3",
                HeadingLevel.H4 => "h4",
                HeadingLevel.H5 => "h5",
                HeadingLevel.H6 => "h6",
                _ => throw new System.Exception()
            };
        }

        public override Font GetFont()
        {
            return Resources.Font_2x;
        }

        private readonly HeadingLevel _level;
    }
}
