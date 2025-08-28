namespace GoOS.GUI.Apps.GoWeb.Html;

public class ScriptElement : Element
{
    public override string GetTag()
    {
        return "script";
    }

    public override bool IsVisible()
    {
        return false;
    }
}