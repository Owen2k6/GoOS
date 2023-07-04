using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using PrismAPI.Graphics;
using IL2CPU.API.Attribs;
using Cosmos.System;
using PrismAPI.Graphics.Animation;
using PrismAPI.UI;

namespace GoOS.GUI
{
    public abstract class Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton.bmp")] private static byte[] closeButtonRaw;
        private static Canvas closeButton = Image.FromBitmap(closeButtonRaw, false);

        public Canvas Contents;

        public int X, Y;
        public string Title;
        public bool Visible;
        public bool Closable;
        public bool Dragging;
        public bool Closing;
        public bool HasTitlebar = true;

        public List<Control> Controls = new();

        private int DragStartX;
        private int DragStartY;
        private int DragStartMouseX;
        private int DragStartMouseY;

        private MouseState previousMouseState = MouseState.None;

        public virtual void Update() { }

        public virtual void RenderControls()
        {
            foreach (Control control in Controls)
            {
                Contents.DrawImage(control.X, control.Y, control.Contents, false);
            }
        }

        internal bool IsMouseOver
        {
            get
            {
                return MouseManager.X >= X                          &&
                       MouseManager.X <  X + Contents.Width         &&
                       MouseManager.Y >= Y + (HasTitlebar ? 16 : 0) &&
                       MouseManager.Y <  Y + Contents.Height;
            }
        }

        internal bool IsMouseOverTitleBar
        {
            get
            {
                if (!HasTitlebar)
                {
                    return false;
                }

                return MouseManager.X >= X                  &&
                       MouseManager.X <  X + Contents.Width &&
                       MouseManager.Y >= Y                  &&
                       MouseManager.Y <  Y + 16;
            }
        }

        internal bool IsMouseOverCloseButton
        {
            get
            {
                return IsMouseOverTitleBar &&
                       MouseManager.X >= X + Contents.Width - 16;
            }
        }

        internal void InternalHandle()
        {
            if (Closable                                   &&
                IsMouseOverCloseButton                     &&
                MouseManager.MouseState == MouseState.None &&
                previousMouseState      == MouseState.Left)
            {
                // Close the window.

                Closing = true;
            }

            if (IsMouseOverTitleBar                         &&
                !IsMouseOverCloseButton                     &&
                MouseManager.MouseState == MouseState.Left  &&
                previousMouseState      == MouseState.None)
            {
                // Start dragging the window.

                DragStartX = X;
                DragStartY = Y;
                DragStartMouseX = (int)MouseManager.X;
                DragStartMouseY = (int)MouseManager.Y;

                Dragging = true;
            }

            if (MouseManager.MouseState == MouseState.None)
            {
                // Stop dragging the window.

                Dragging = false;
            }

            if (Dragging)
            {
                // Do the drag operation.

                X = (int)(DragStartX + (MouseManager.X - DragStartMouseX));
                Y = (int)(DragStartY + (MouseManager.Y - DragStartMouseY));
            }

            foreach (Control control in Controls)
            {
                if (MouseManager.X >= X + control.X                            &&
                    MouseManager.X <  X + control.X + control.Contents.Width   &&
                    MouseManager.Y >= Y + control.Y                            &&
                    MouseManager.Y <  Y + control.Y + control.Contents.Height)
                {
                    if (MouseManager.MouseState == MouseState.None && previousMouseState == MouseState.Left)
                    {
                        control.Clicked?.Invoke();
                    }
                }
            }

            previousMouseState = MouseManager.MouseState;
        }

        public void DrawWindow(Canvas cv)
        {
            if (HasTitlebar)
            {
                // Title bar.
                cv.DrawFilledRectangle(X, Y, Contents.Width, 19, 0, Color.DeepGray);
                cv.DrawString(X + 2, Y, Title, BetterConsole.font, Color.White);

                // Close button.
                if (Closable)
                    cv.DrawImage(X + Contents.Width - 18, Y + 3, closeButton);
            }

            // Window contents.
            cv.DrawImage(X, Y + (HasTitlebar ? 19 : 0), Contents, false);
        }

        internal void Dispose()
        {
            Closing = true;
        }
    }
}
