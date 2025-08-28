using GoGL.Graphics;

namespace GoOS.GUI.Apps.GoWeb.Html;

public class AnchorElement : Element
{
    public override string GetTag()
    {
        return "a";
    }

    public override Color GetColor()
    {
        return Color.Blue;
    }

    public override bool GetUnderline()
    {
        return true;
    }
}