using GoGL.Graphics;
using GoOS.GUI.Apps.GoWeb.Html;

namespace GoOS.GUI.Apps.GoWeb.Render;

public class RenderContext
{
    public Canvas Target;

    internal RenderContext(Canvas target)
    {
        Target = target;
    }
}

public static class Renderer
{
    public static void Render(Document document, Canvas target)
    {
        var ctx = new RenderContext(target);
        target.Clear(Color.White);
        document.Body.Render(ctx);
    }
}