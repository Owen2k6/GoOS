namespace GoOS.GUI.Apps.GoWeb.Html;

public class StyleElement : Element
{
    public override string GetTag()
    {
        return "style";
    }

    public override bool IsVisible()
    {
        return false;
    }
}