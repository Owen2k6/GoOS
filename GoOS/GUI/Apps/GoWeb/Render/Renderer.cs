using GoOS.GUI.Apps.GoWeb.Html;
using GoGL.Graphics;

namespace GoOS.GUI.Apps.GoWeb.Render
{
    public class RenderContext
    {
        internal RenderContext(Canvas target)
        {
            Target = target;
        }

        public Canvas Target;
    }

    public static class Renderer
    {
        public static void Render(Document document, Canvas target)
        {
            RenderContext ctx = new RenderContext(target);
            target.Clear(Color.White);
            document.Body.Render(ctx);
        }
    }
}
