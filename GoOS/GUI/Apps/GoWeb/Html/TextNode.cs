#nullable enable
using GoOS.GUI.Apps.GoWeb.Render;

namespace GoOS.GUI.Apps.GoWeb.Html;

public class TextNode : Element
{
    public TextNode(string text)
    {
        Text = text;
    }

    public override string TextContent => Text;

    public string Text { get; init; }

    public GlyphRun? GlyphRun { get; set; }

    public override string GetTag()
    {
        return "#text";
    }

    public override void Render(RenderContext ctx)
    {
        int lastX = 1, lastY = -1;
        for (var i = 0; i < Text.Length; i++)
        {
            var c = Text[i];
            var (x, y) = GlyphRun!.Glyphs[i];
            var glyph = GlyphRun.Font.GetGlyph(c);
            for (var p = 0; p < glyph.Points.Count; p++)
                ctx.Target[x + glyph.Points[p].X, y + glyph.Points[p].Y] = GlyphRun.Color;
            if (GlyphRun.Underline)
                if (lastY == y)
                    ctx.Target.DrawLine(lastX, y + glyph.Height, x + glyph.Width, y + glyph.Height, GlyphRun.Color);

            lastX = x;
            lastY = y;
        }
    }
}