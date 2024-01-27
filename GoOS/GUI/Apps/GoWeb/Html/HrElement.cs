using GoOS.GUI.Apps.GoWeb.Render;
using GoGL.Graphics;

namespace GoOS.GUI.Apps.GoWeb.Html
{
    public class HrElement : Element
    {
        public override string GetTag() => "hr";

        private const int MARGIN = 10;

        public override void Render(RenderContext ctx)
        {
            ctx.Target.DrawLine(X + MARGIN, Y, ctx.Target.Width - MARGIN, Y, Color.LightGray);
        }
    }
}
