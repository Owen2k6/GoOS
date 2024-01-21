using GoOS.GUI.Apps.GoWeb.Html;
using PrismAPI.Graphics;
using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI.Apps.GoWeb.Render
{
    public class GlyphRun
    {
        public GlyphRun(DocumentLayout layout, TextNode textNode, int containerWidth)
        {
            Font = textNode.GetFont();
            Color = textNode.GetColor();
            Underline = textNode.GetUnderline();
            Glyphs = new (Glyph, int, int)[textNode.Text.Length];
            bool preformatted = textNode.IsPreformatted();
            for (int i = 0; i < textNode.Text.Length; i++)
            {
                char c = textNode.Text[i];
                Glyph glyph = Font.GetGlyph(c);
                if (layout.X > containerWidth - glyph.Width - 32)
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
                Glyphs[i] = (Font.GetGlyph(c), layout.X, layout.Y);
            }
        }

        public (Glyph glyph, int x, int y)[] Glyphs { get; init; }

        public Font Font { get; init; }

        public Color Color { get; init; }

        public bool Underline { get; init; }
    }
}
