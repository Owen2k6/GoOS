using Gold.Graphics;
using GoOS.GUI.Apps.GoWeb.Render;

namespace GoOS.GUI.Apps.GoWeb.Html;

public class HrElement : Element
{
    private const int MARGIN = 10;

    public override string GetTag()
    {
        return "hr";
    }

    public override void Render(RenderContext ctx)
    {
        ctx.Target.DrawLine(X + MARGIN, Y, ctx.Target.Width - MARGIN, Y, Color.LightGray);
    }
}