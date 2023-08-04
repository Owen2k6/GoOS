using System.Collections.Generic;
using Cosmos.System;
using GoOS.GUI.Models;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

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
        
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Maximize.bmp")] private static byte[] maximiseRaw;
        private static Canvas maximize = Image.FromBitmap(maximiseRaw, false);
        
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize.bmp")] private static byte[] minimiseRaw;
        private static Canvas minimise = Image.FromBitmap(minimiseRaw, false);
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize_Hovered.bmp")] private static byte[] minimiseHoverRaw;
        private static Canvas minimiseHover = Image.FromBitmap(minimiseHoverRaw, false);
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Minimize_Pressed.bmp")] private static byte[] minimisePressedRaw;
        private static Canvas minimisePressed = Image.FromBitmap(minimisePressedRaw, false);

        public Canvas Contents;

        public int X = 50, Y = 50;
        public string Title;
        public bool Visible;
        public bool Closable;
        public bool Dragging;
        public bool Closing;
        public bool HasTitlebar = true;
        public bool Unkillable = false;

        public List<Control> Controls = new();

        private int DragStartX;
        private int DragStartY;
        private int DragStartMouseX;
        private int DragStartMouseY;

        private MouseState previousMouseState = MouseState.None;
        private bool wasDown = false;
        private Control downOnControl = null;

        public Control FocusedControl = null;

        public const int TITLE_BAR_HEIGHT = 19;

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

        public bool Focused
        {
            get
            {
                return WindowManager.windows[WindowManager.windows.Count - 1] == this;
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
                return (int)(MouseManager.Y - Y - (HasTitlebar ? TITLE_BAR_HEIGHT : 0));
            }
        }

        public bool IsMouseOver
        {
            get
            {
                return MouseManager.X >= X &&
                       MouseManager.X < X + Contents.Width &&
                       MouseManager.Y >= Y &&
                       MouseManager.Y < Y + Contents.Height + (HasTitlebar ? TITLE_BAR_HEIGHT : 0);
            }
        }

        public bool IsMouseOverContent
        {
            get
            {
                return MouseManager.X >= X &&
                       MouseManager.X < X + Contents.Width &&
                       MouseManager.Y >= Y + (HasTitlebar ? TITLE_BAR_HEIGHT : 0) &&
                       MouseManager.Y < Y + (HasTitlebar ? TITLE_BAR_HEIGHT : 0) + Contents.Height;
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

                return MouseManager.X >= X &&
                       MouseManager.X < X + Contents.Width &&
                       MouseManager.Y >= Y &&
                       MouseManager.Y < Y + TITLE_BAR_HEIGHT;
            }
        }

        public bool IsMouseOverCloseButton
        {
            get
            {
                return IsMouseOverTitleBar &&
                       MouseManager.X >= X + Contents.Width - TITLE_BAR_HEIGHT;
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
            if (Closable &&
                IsMouseOverCloseButton &&
                MouseManager.MouseState == MouseState.None &&
                previousMouseState == MouseState.Left)
            {
                // Close the window.

                Closing = true;
            }

            if (IsMouseOverTitleBar &&
                !IsMouseOverCloseButton &&
                MouseManager.MouseState == MouseState.Left &&
                previousMouseState == MouseState.None)
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
                if ((int)(DragStartY + (MouseManager.Y - DragStartMouseY)) < WindowManager.Canvas.Height - 28)
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
                if (previousMouseState == MouseState.Right)
                {
                    ShowContextMenu();
                }

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
                cv.DrawFilledRectangle(X, Y, Contents.Width, TITLE_BAR_HEIGHT, 0,
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

                    cv.DrawImage(X + Contents.Width - 21, Y +1, closeButtonImage);
                    cv.DrawImage(X + Contents.Width - 39, Y +1, maximize);
                    cv.DrawImage(X + Contents.Width - 57, Y +1, minimise);
                }
            }

            // Window contents.
            cv.DrawImage(X, Y + (HasTitlebar ? TITLE_BAR_HEIGHT : 0), Contents, false);
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

        /// <summary>
        /// User function to handle context menus.
        /// </summary>
        public virtual void ShowContextMenu() { }

        public void Dispose()
        {
            Closing = true;
        }

        public void RenderOutsetWindowBackground()
        {
            // Background.
            Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, new Color(71, 71, 71));

            // Border.
            RenderSystemStyleBorder();
        }

        public void RenderSystemStyleBorder()
        {
            // Highlight.
            Contents.DrawLine(0, 0, Contents.Width - 1, 0, new Color(80, 80, 80));
            Contents.DrawLine(0, 0, 0, Contents.Height - 1, new Color(80, 80, 80));

            // Light shadow.
            Contents.DrawLine(1, Contents.Height - 2, Contents.Width - 2, Contents.Height - 2, new Color(89, 89, 89));
            Contents.DrawLine(Contents.Width - 2, 1, Contents.Width - 2, Contents.Height - 1, new Color(89, 89, 89));

            // Dark shadow.
            Contents.DrawLine(0, Contents.Height - 1, Contents.Width, Contents.Height - 1, Color.Black);
            Contents.DrawLine(Contents.Width - 1, 0, Contents.Width - 1, Contents.Height - 1, Color.Black);
        }

        protected void ShowAboutDialog()
        {
            Dialogue.Show(
                $"About {Title}",
                $"GoOS {Title} v{Kernel.version}\n\nCopyright (c) 2023 Owen2k6\nAll rights reserved.",
                null,
                heightOverride: 144);
        }

        protected void SetDock(WindowDock dock)
        {
            switch (dock)
            {
                case WindowDock.None:
                    X = 0;
                    Y = 0;
                    break;

                case WindowDock.Auto:
                    X = 50 + (WindowManager.GetAmountOfWindowsByTitle(Title) * 50);
                    Y = 50 + (WindowManager.GetAmountOfWindowsByTitle(Title) * 50);
                    break;

                case WindowDock.Center:
                    X = (WindowManager.Canvas.Width / 2) - (Contents.Width / 2);
                    Y = (WindowManager.Canvas.Height / 2) - (Contents.Height / 2);
                    break;
            }
        }
    }
}
