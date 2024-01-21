#nullable enable
using GoOS.GUI.Apps.GoWeb.Render;
using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI.Apps.GoWeb.Html
{
    public class TextNode : Element
    {
        public TextNode(string text)
        {
            Text = text;
        }

        public override string GetTag() => "#text";

        public override string TextContent => Text;

        public string Text { get; init; }

        public GlyphRun? GlyphRun { get; set; }

        public override void Render(RenderContext ctx)
        {
            int lastX = 1, lastY = -1;
            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];
                (int x, int y) = GlyphRun!.Glyphs[i];
                Glyph glyph = GlyphRun.Font.GetGlyph(c);
                for (int p = 0; p < glyph.Points.Count; p++)
                {
                    ctx.Target[x + glyph.Points[p].X, y + glyph.Points[p].Y] = GlyphRun.Color;
                }
                if (GlyphRun.Underline)
                {
                    if (lastY == y)
                    {
                        ctx.Target.DrawLine(lastX, y + glyph.Height, x + glyph.Width, y + glyph.Height, GlyphRun.Color);
                    }
                }
                lastX = x;
                lastY = y;
            }
        }
    }
}
