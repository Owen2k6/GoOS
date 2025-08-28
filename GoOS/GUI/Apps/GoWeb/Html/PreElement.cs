namespace GoOS.GUI.Apps.GoWeb.Html;

public class PreElement : Element
{
    public override string GetTag()
    {
        return "pre";
    }

    public override bool IsPreformatted()
    {
        return true;
    }
}