using Cosmos.System;
using Gold.Graphics;
using GoOS.GUI.Apps;
using GoOS.GUI.Models;
using IL2CPU.API.Attribs;

namespace GoOS.GUI;

public class Button : Control
{
    private const int PUSH_DEPTH = 1;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse_click.bmp")]
    private static byte[] mouseClickRaw;

    private static readonly Canvas mouseClick = Gold.Graphics.Image.FromBitmap(mouseClickRaw);

    public bool AppearPressed = false;

    public Color BackgroundColour = new(12, 12, 12);

    public bool CenterTitle = true;

    public bool HasSelectionColour = false;

    /// <summary>
    ///     Optional image.
    /// </summary>
    public Canvas Image;


    private uint lastMouseX, lastMouseY;

    public byte lastSecondClicked;

    public bool pressed;

    public Color SelectionColour = new(12, 12, 12);

    public Color TextColour = Color.White;

    public int textX;

    public int textY;

    public string Title;

    public bool UseSystemStyle = true;

    public Button(Window parent, ushort x, ushort y, ushort width, ushort height, string title)
        : base(parent, x, y, width, height)
    {
        Title = title;
    }

    public override void Render()
    {
        // Ensure alpha so cleared corners are truly transparent (safe if property missing).
        try
        {
            RenderWithAlpha = true;
        }
        catch
        {
        }

        var drawPressed = AppearPressed || pressed;

        // ---- Bevelled DESIGN for non-imaged buttons (backport) ----
        if (UseSystemStyle && Image == null)
        {
            int w = Contents.Width;
            int h = Contents.Height;

            // Fully transparent clear (ARGB 0x00_000000), not opaque black.
            Contents.Clear(new Color(0x00000000));

            if (w >= 4 && h >= 4)
            {
                // Inner fills
                Contents.DrawFilledRectangle(1, 1, (ushort)(w - 2), (ushort)(h - 2), 0,
                    drawPressed ? new Color(0xFF666666) : new Color(0xFFE7E7E7));
                Contents.DrawFilledRectangle(3, 3, (ushort)(w - 4), (ushort)(h - 4), 0,
                    drawPressed ? new Color(0xFF878787) : new Color(0xFFE7E7E7));

                // Corner pixels & soft bevel tones (match source look)
                Contents[2, 0] = new Color(0xFF3F3F3F);
                Contents[1, 1] = Color.Black;
                Contents[0, 2] = new Color(0xFF3F3F3F);
                Contents[2, 1] = new Color(drawPressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[1, 2] = new Color(drawPressed ? 0xFF666666 : 0xFFCDCDCD);

                Contents[w - 3, 0] = new Color(0xFF3F3F3F);
                Contents[w - 2, 1] = Color.Black;
                Contents[w - 1, 2] = new Color(0xFF3F3F3F);
                Contents[w - 3, 1] = new Color(drawPressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[w - 2, 2] = new Color(drawPressed ? 0xFF666666 : 0xFFCDCDCD);

                Contents[0, h - 3] = new Color(0xFF3F3F3F);
                Contents[1, h - 2] = Color.Black;
                Contents[2, h - 1] = new Color(0xFF3F3F3F);
                Contents[1, h - 3] = new Color(drawPressed ? 0xFF666666 : 0xFFCDCDCD);
                Contents[2, h - 2] = new Color(drawPressed ? 0xFF666666 : 0xFFCDCDCD);

                Contents[w - 1, h - 3] = new Color(0xFF3F3F3F);
                Contents[w - 2, h - 2] = Color.Black;
                Contents[w - 3, h - 1] = new Color(0xFF3F3F3F);

                Contents[3, 3] = drawPressed ? new Color(0xFF777777) : Color.White;
                Contents[w - 3, h - 3] = drawPressed ? new Color(0xFFA5A5A5) : new Color(0xFF969696);
                Contents[w - 4, h - 4] = drawPressed ? new Color(0xFF969696) : new Color(0xFFC0C0C0);

                // Bevel lines (trimmed to avoid corners)
                if (w >= 6)
                {
                    Contents.DrawLine(3, 0, w - 3, 0, Color.Black);
                    Contents.DrawLine(3, h - 1, w - 3, h - 1, Color.Black);
                }

                if (h >= 6)
                {
                    Contents.DrawLine(0, 3, 0, h - 3, Color.Black);
                    Contents.DrawLine(w - 1, 3, w - 1, h - 3, Color.Black);
                }

                Contents.DrawLine(2, 2, w - 3, 2, drawPressed ? new Color(0xFF777777) : Color.White);
                Contents.DrawLine(2, 2, 2, h - 3, drawPressed ? new Color(0xFF777777) : Color.White);

                Contents.DrawLine(w - 2, 3, w - 2, h - 2,
                    drawPressed ? new Color(0xFFA5A5A5) : new Color(0xFF969696));
                Contents.DrawLine(3, h - 2, w - 2, h - 2,
                    drawPressed ? new Color(0xFFA5A5A5) : new Color(0xFF969696));

                Contents.DrawLine(w - 3, 3, w - 3, h - 3,
                    drawPressed ? new Color(0xFF969696) : new Color(0xFFC0C0C0));
                Contents.DrawLine(3, h - 3, w - 3, h - 3,
                    drawPressed ? new Color(0xFF969696) : new Color(0xFFC0C0C0));

                // Punch out 2×2 rounded corners (ensure transparency)
                void ClearCorner(int x, int y)
                {
                    if (x + 1 >= w || y + 1 >= h || x < 0 || y < 0) return;
                    Contents[x + 0, y + 0] = new Color(0x00000000);
                    Contents[x + 1, y + 0] = new Color(0x00000000);
                    Contents[x + 0, y + 1] = new Color(0x00000000);
                    Contents[x + 1, y + 1] = new Color(0x00000000);
                }

                ClearCorner(0, 0); // top-left
                ClearCorner(w - 2, 0); // top-right
                ClearCorner(0, h - 2); // bottom-left
                ClearCorner(w - 2, h - 2); // bottom-right
            }

            // Title (Charcoal with fallback), centred precisely
            if (!string.IsNullOrEmpty(Title))
            {
                var font = Resources.Charcoal ?? Resources.Font_1x;

                var tx = Contents.Width / 2 - font.MeasureString(Title) / 2 - 1;
                var ty = Contents.Height / 2 - font.Size / 2 - 1;

                if (drawPressed)
                {
                    tx += PUSH_DEPTH;
                    ty += PUSH_DEPTH;
                }

                var textColour = drawPressed ? Color.White : Color.Black;
                Contents.DrawString(tx, ty, Title, font, textColour);
            }

            Parent.RenderControls();
            return;
        }

        // ---- Original paths preserved (your existing behaviour) ----
        if (UseSystemStyle)
        {
            if (drawPressed)
            {
                // Background.
                Contents.Clear(new Color(71, 71, 71));

                // Dark shadow.
                Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.Black);
                Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.Black);

                // Highlight.
                Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2,
                    new Color(80, 80, 80));
                Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1,
                    new Color(80, 80, 80));

                // Light highlight.
                Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1,
                    new Color(89, 89, 89));
                Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1,
                    new Color(89, 89, 89));
            }
            else
            {
                // Background.
                Contents.Clear(new Color(71, 71, 71));

                // Highlight.
                Contents.DrawLine(0, 0, Contents.Width - 1, 0, new Color(80, 80, 80));
                Contents.DrawLine(0, 0, 0, Contents.Height - 1, new Color(80, 80, 80));

                // Light shadow.
                Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2,
                    new Color(89, 89, 89));
                Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1,
                    new Color(89, 89, 89));

                // Dark shadow.
                Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.Black);
                Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.Black);
            }
        }
        else
        {
            if (IsMouseOver && HasSelectionColour)
                Contents.Clear(SelectionColour);
            else
                Contents.Clear(BackgroundColour);
        }

        // Optional image (unchanged)
        if (Image != null)
        {
            var imageX = (Contents.Width - Image.Width) / 2;
            var imageY = 0;

            if (drawPressed && UseSystemStyle)
            {
                imageX += PUSH_DEPTH;
                imageY += PUSH_DEPTH;
            }

            Contents.DrawImage(imageX, imageY, Image);
        }

        // Title — switch to Charcoal (with safe fallback), preserving your centred flag path
        if (!string.IsNullOrEmpty(Title))
        {
            if (textX == 0)
                textX = Contents.Width / 2;

            if (textY == 0)
                textY = Image != null ? Image.Height + 12 : Contents.Height / 2;

            var tx = textX;
            var ty = textY;

            if (drawPressed && UseSystemStyle)
            {
                tx += PUSH_DEPTH;
                ty += PUSH_DEPTH;
            }

            var font = Resources.Charcoal ?? Resources.Font_1x;
            var textColour = UseSystemStyle ? Color.White : TextColour;

            Contents.DrawString(tx, ty, Title, font, textColour, CenterTitle);

            if (drawPressed && UseSystemStyle)
            {
                // restore locals only (textX/textY fields are not mutated here)
                tx -= PUSH_DEPTH;
                ty -= PUSH_DEPTH;
            }
        }

        Parent.RenderControls();
    }

    public override void Update()
    {
        if (IsMouseOver && Visible && Parent.Visible && (Parent.Focused || Parent.Title == nameof(Desktop)))
            WindowManager.MouseToDraw = mouseClick;

        if (lastMouseX != MouseManager.X || lastMouseY != MouseManager.Y)
            if (!UseSystemStyle && HasSelectionColour)
                Render();

        lastMouseX = MouseManager.X;
        lastMouseY = MouseManager.Y;
    }

    internal override void HandleDown(MouseEventArgs _args)
    {
        pressed = true;
        Render();
        Parent.RenderControls();
    }

    internal override void HandleRelease()
    {
        pressed = false;
        Render();
        Parent.RenderControls();
    }
}