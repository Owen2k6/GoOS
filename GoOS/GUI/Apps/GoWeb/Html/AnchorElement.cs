using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.GoWeb.Html
{
    public class AnchorElement : Element
    {
        public override string GetTag() => "a";

        public override Color GetColor() => Color.Blue;

        public override bool GetUnderline() => true;
    }
}
