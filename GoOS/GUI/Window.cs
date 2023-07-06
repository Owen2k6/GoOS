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
using GoOS.GUI.Models;

namespace GoOS.GUI
{
    public abstract class Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton.bmp")] private static byte[] closeButtonRaw;
        private static Canvas closeButton = Image.FromBitmap(closeButtonRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton_hover.bmp")] private static byte[] closeButtonHoverRaw;
        private static Canvas closeButtonHover = Image.FromBitmap(closeButtonHoverRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton_pressed.bmp")] private static byte[] closeButtonPressedRaw;
        private static Canvas closeButtonPressed = Image.FromBitmap(closeButtonPressedRaw, false);

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
        private bool wasDown = false;
        private Control downOnControl = null;

        public Control FocusedControl = null;

        /// <summary>
        /// Runs every cycle, regardless of focus.
        /// You must call the base if you override this function.
        /// </summary>
        public virtual void HandleRun()
        {
            if (wasDown && MouseManager.MouseState == MouseState.None)
            {
                wasDown = false;

                HandleRelease(new MouseEventArgs()
                {
                    X = RelativeMouseX,
                    Y = RelativeMouseY,
                    MouseState = previousMouseState
                });

                downOnControl?.HandleRelease();

                downOnControl = null;
            }
        }

        public virtual void RenderControls()
        {
            foreach (Control control in Controls)
            {
                Contents.DrawImage(control.X, control.Y, control.Contents, false);
            }
        }

        public int RelativeMouseX
        {
            get
            {
                return (int)(MouseManager.X - X);
            }
        }

        public int RelativeMouseY
        {
            get
            {
                return (int)(MouseManager.Y - Y - (HasTitlebar ? 19 : 0));
            }
        }

        public bool IsMouseOver
        {
            get
            {
                return MouseManager.X >= X                                             &&
                       MouseManager.X <  X + Contents.Width                            &&
                       MouseManager.Y >= Y                                             &&
                       MouseManager.Y <  Y + Contents.Height + (HasTitlebar ? 19 : 0);
            }
        }

        public bool IsMouseOverContent
        {
            get
            {
                return MouseManager.X >= X                                             &&
                       MouseManager.X <  X + Contents.Width                            &&
                       MouseManager.Y >= Y + (HasTitlebar ? 19 : 0)                    &&
                       MouseManager.Y <  Y + (HasTitlebar ? 19 : 0) + Contents.Height;
            }
        }

        public bool IsMouseOverTitleBar
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
                       MouseManager.Y <  Y + 19;
            }
        }

        public bool IsMouseOverCloseButton
        {
            get
            {
                return IsMouseOverTitleBar &&
                       MouseManager.X >= X + Contents.Width - 19;
            }
        }

        private Control GetHoveredControl()
        {
            foreach (Control control in Controls)
            {
                if (control.IsMouseOver)
                {
                    return control;
                }
            }

            return null;
        }

        internal void HandleMouseInput()
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

            Control hoveredControl = GetHoveredControl();

            // Down, any button.
            if (MouseManager.MouseState != MouseState.None && previousMouseState == MouseState.None)
            {
                wasDown = true;
                downOnControl = hoveredControl;

                HandleDown(new MouseEventArgs()
                {
                    X = RelativeMouseX,
                    Y = RelativeMouseY,
                    MouseState = MouseManager.MouseState
                });

                if (MouseManager.MouseState == MouseState.Left)
                {
                    FocusedControl = hoveredControl;

                    hoveredControl?.HandleDown(new MouseEventArgs()
                    {
                        X = RelativeMouseX - hoveredControl.X,
                        Y = RelativeMouseY - hoveredControl.Y,
                        MouseState = MouseManager.MouseState
                    });
                }
            }

            // Click, any button.
            if (MouseManager.MouseState == MouseState.None && previousMouseState != MouseState.None)
            {
                HandleClick(new MouseEventArgs()
                {
                    X = RelativeMouseX,
                    Y = RelativeMouseY,
                    MouseState = previousMouseState
                });

                if (previousMouseState == MouseState.Left)
                {
                    hoveredControl?.Clicked?.Invoke();
                }

                foreach (Control control in Controls)
                {
                    if (control != hoveredControl)
                    {
                        control.HandleUnfocus();
                    }
                }
            }

            previousMouseState = MouseManager.MouseState;
        }

        public void DrawWindow(Canvas cv, bool focused)
        {
            if (HasTitlebar)
            {
                // Title bar.
                cv.DrawFilledRectangle(X, Y, Contents.Width, 19, 0, 
                    focused ? Color.LighterBlack : Color.DeepGray);

                cv.DrawString(X + 2, Y, Title, BetterConsole.font, Color.White);

                // Close button.
                if (Closable)
                {
                    Canvas closeButtonImage = closeButton;
                    if (IsMouseOverCloseButton)
                    {
                        closeButtonImage = MouseManager.MouseState == MouseState.Left ?
                            closeButtonPressed : closeButtonHover;
                    }

                    cv.DrawImage(X + Contents.Width - 21, Y + 3, closeButtonImage);
                }
            }

            // Window contents.
            cv.DrawImage(X, Y + (HasTitlebar ? 19 : 0), Contents, false);
        }

        /// <summary>
        /// User function to handle a mouse click, which is defined as a mouse being held down on the window then released.
        /// </summary>
        public virtual void HandleClick(MouseEventArgs e) { }

        /// <summary>
        /// User function to handle a mouse down, which is defined as a mouse being held down on the window then released.
        /// </summary>
        public virtual void HandleDown(MouseEventArgs e) { }

        /// <summary>
        /// User function to handle a mouse release, which is defined as a mouse being held down on the window then released.
        /// This is different to a click, in that it will fire even if the mouse is pressed and then leaves the window before being released.
        /// </summary>
        /// <param name="e">The arguments of the event. MouseState will contain the previous state, not <see cref="MouseState.None"/>.</param>
        public virtual void HandleRelease(MouseEventArgs e) { }

        /// <summary>
        /// User function to handle a key being pressed. Only routed to the focused window.
        /// </summary>
        public virtual void HandleKey(KeyEvent key)
        {
            foreach (Control control in Controls)
            {
                if (control == FocusedControl)
                {
                    control.HandleKey(key);
                }
            }
        }

        public void Dispose()
        {
            Closing = true;
        }

        public void RenderOutsetWindowBackground()
        {
            // Background.
            Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, new Color(191, 191, 191));

            // Highlight.
            Contents.DrawLine(0, 0, Contents.Width - 1, 0, Color.White);
            Contents.DrawLine(0, 0, 0, Contents.Height - 1, Color.White);

            // Light shadow.
            Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2, new Color(127, 127, 127));
            Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(127, 127, 127));

            // Dark shadow.
            Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.Black);
            Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.Black);
        }
    }
}
