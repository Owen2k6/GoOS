namespace GoOS.GUI.Apps.GoWeb.Html;

public class UnderlineElement : Element
{
    public override string GetTag()
    {
        return "u";
    }

    public override bool GetUnderline()
    {
        return true;
    }
}