namespace GoOS.GUI.Apps.GoWeb.Html
{
    public class PreElement : Element
    {
        public override string GetTag() => "pre";

        public override bool IsPreformatted() => true;
    }
}
