using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// A window on the desktop.
    /// </summary>
    public class Window : View
    {
        public Window(View initialParent)
        {
            if (initialParent is not Views.Desktop)
            {
                throw new Exception("Window must be added to the desktop.");
            }

            Rectangle = new Rectangle(64, 64, 384, 192);
            Parent = initialParent;

            // Titlebar
            Titlebar = new RectangleView(this)
            {
                Color = Color.Black,
                FillWidth = true,
                Rectangle = new Rectangle(0, 0, 0, 32),
                Tag = "Titlebar"
            };

            // Window title
            titleView = new TextView(Titlebar)
            {
                Color = Color.White,
                Text = "Window",
                HAlign = Alignment.Middle,
                VAlign = Alignment.Middle,
                Anchor = new PointF(0.5f, 0.5f),
                Active = false
            };

            // Close button
            ImageView close = new ImageView(Titlebar)
            {
                Anchor = new PointF(1f, 0.5f),
                Image = new Bitmap(closeIconBytes),
                Tag = "Close"
            };

            // Icon
            iconView = new ImageView(Titlebar)
            {
                Anchor = new PointF(0f, 0.5f),
                Rectangle = new Rectangle(8, 0, 16, 16),
                // Image = new Bitmap(defaultIconBytes)
            };
        }

        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.closeIcon.bmp")]
        private static byte[] closeIconBytes;

        [ManifestResourceStream(ResourceName = "GoOS.Assets.Views.defaultIcon.bmp")]
        private static byte[] defaultIconBytes;

        public RectangleView Titlebar;
        private TextView titleView;
        private ImageView iconView;

        public string Title
        {
            get { return titleView.Text; }
            set { titleView.Text = value; }
        }

        internal Pen Pen = new Pen(Color.FromArgb(38, 38, 38));
        /// <summary>
        /// The colour of the window.
        /// </summary>
        public Color Color
        {
            get { return Pen.Color; }
            set { Pen.Color = value; Invalid = true; }
        }

        /// <summary>
        /// The 16x16 icon to be displayed in the window's titlebar.
        /// </summary>
        public Bitmap Icon
        {
            get
            {
                return iconView.Image;
            }
            set
            {
                iconView.Image = value;
            }
        }

        internal bool Dragging = false;
        internal int DragStartMouseX;
        internal int DragStartMouseY;
        internal int DragStartWindowX;
        internal int DragStartWindowY;

        public bool Modal { get; private set; } = false;
        /// <summary>
        /// Remove the window's titlebar and make it automatically close when a click is registered outside its bounds.
        /// </summary>
        public void MakeModal()
        {
            Modal = true;
            RemoveTitlebar();
        }

        /// <summary>
        /// Remove the window's titlebar.
        /// </summary>
        public void RemoveTitlebar()
        {
            Titlebar.Remove();
            Titlebar = null;
            titleView = null;
            Invalid = true;
        }

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            Invalid = false;
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            if (Dragging)
            {
                Rectangle = new Rectangle(
                    DragStartWindowX + (Desktop.Cursor.Rectangle.X - DragStartMouseX),
                    DragStartWindowY + (Desktop.Cursor.Rectangle.Y - DragStartMouseY),
                    Rectangle.Width,
                    Rectangle.Height
                );
            }
            CalculateScreenBounds();
            if (Titlebar != null)
            {
                desktop.canvas.DrawFilledRectangle(Pen, ScreenBounds.X, ScreenBounds.Y + Titlebar.Rectangle.Height, ScreenBounds.Width, ScreenBounds.Height - Titlebar.Rectangle.Height);
            }
            else
            {
                desktop.canvas.DrawFilledRectangle(Pen, ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);
            }
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
