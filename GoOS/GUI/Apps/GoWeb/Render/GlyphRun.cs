using Gold.Graphics;
using Gold.Graphics.Fonts;
using GoOS.GUI.Apps.GoWeb.Html;

namespace GoOS.GUI.Apps.GoWeb.Render;

public class GlyphRun
{
    public GlyphRun(DocumentLayout layout, TextNode textNode)
    {
        Font = textNode.GetFont();
        Color = textNode.GetColor();
        Underline = textNode.GetUnderline();
        Glyphs = new (int, int)[textNode.Text.Length];
        var preformatted = textNode.IsPreformatted();
        for (var i = 0; i < textNode.Text.Length; i++)
        {
            var c = textNode.Text[i];
            var glyph = Font.GetGlyph(c);
            if (layout.X > layout.ScreenWidth - glyph.Width - 32)
            {
                layout.X = 0;
                layout.Y += Font.Size;
            }
            else if (c == '\n')
            {
                if (layout.PermitWhitespace || preformatted)
                {
                    if (preformatted)
                    {
                        layout.X = 0;
                        layout.Y += Font.Size;
                    }
                    else
                    {
                        layout.X += Font.Size / 2 + 2;
                    }

                    layout.PermitWhitespace = false;
                }
            }
            else if (char.IsWhiteSpace(c))
            {
                if (layout.PermitWhitespace | preformatted)
                {
                    layout.X += Font.Size / 2 + 2;
                    layout.PermitWhitespace = false;
                }
            }
            else if (char.IsControl(c))
            {
            }
            else
            {
                layout.X += glyph.Width + 2;
                layout.PermitWhitespace = true;
            }

            Glyphs[i] = (layout.X, layout.Y);
        }
    }

    public (int x, int y)[] Glyphs { get; init; }

    public Font Font { get; init; }

    public Color Color { get; init; }

    public bool Underline { get; init; }
}