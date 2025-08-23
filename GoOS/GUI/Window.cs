using System.Collections.Generic;
using Cosmos.System;
using GoOS.GUI.Models;
using System;
using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI
{
    public class Window
    {
        public Canvas Contents;

        public int X = 50, Y = 50;
        public string Title;
        public bool Visible;
        public bool Closable;
        public bool Dragging;
        public bool Closing;
        public bool HasTitlebar = true;
        public bool Unkillable = false;
        public bool Sizable = false;

        // OS 9 style roll-up (minimise) state
        public bool Collapsed = false;

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

        // ---- Platinum palette (dark chrome) ----
        private static readonly Color PlatFace = new Color(71, 71, 71);   // your existing body fill
        private static readonly Color FrameHi = new Color(100, 100, 100);  // inner highlight
        private static readonly Color FrameMid = new Color(64, 64, 64);   // mid shadow
        private static readonly Color FrameLo = new Color(32, 32, 32);   // deep shadow
        private static readonly Color FrameBlack = Color.Black;

        private static readonly Color TitleFocusedBG = Color.LighterBlack;      // dark focused bar
        private static readonly Color TitleUnfocusBG = Color.DeepGray;          // dark unfocused bar
        private static readonly Color TitleStripe = new Color(96, 96, 96);   // subtle pin-stripes
        private static readonly Color TitleText = Color.White;

        public virtual void HandleRun()
        {
            if (!Collapsed)
            {
                foreach (var control in Controls)
                    control.Update();
            }

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
            if (Collapsed) return;
            foreach (var control in Controls)
                Contents.DrawImage(control.X, control.Y, control.Contents, control.RenderWithAlpha);
        }

        public bool Focused => WindowManager.windows[WindowManager.windows.Count - 1] == this;

        public int RelativeMouseX => (int)(MouseManager.X - X);

        public int RelativeMouseY => (int)(MouseManager.Y - Y - (HasTitlebar ? TITLE_BAR_HEIGHT : 0));

        public bool IsMouseOver
        {
            get
            {
                int totalHeight = HasTitlebar
                    ? (Collapsed ? TITLE_BAR_HEIGHT : TITLE_BAR_HEIGHT + Contents.Height)
                    : (Collapsed ? 0 : Contents.Height);

                return MouseManager.X >= X &&
                       MouseManager.X < X + Contents.Width &&
                       MouseManager.Y >= Y &&
                       MouseManager.Y < Y + totalHeight;
            }
        }

        public bool IsMouseOverContent
        {
            get
            {
                if (Collapsed) return false;

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
                if (!HasTitlebar) return false;

                return MouseManager.X >= X &&
                       MouseManager.X < X + Contents.Width &&
                       MouseManager.Y >= Y &&
                       MouseManager.Y < Y + TITLE_BAR_HEIGHT;
            }
        }

        // Close: LEFT segment
        public bool IsMouseOverCloseButton =>
            IsMouseOverTitleBar &&
            MouseManager.X >= X &&
            MouseManager.X <= X + TITLE_BAR_HEIGHT;

        // Minimise (roll-up toggle): RIGHT segment
        public bool IsMouseOverMinimizeButton =>
            IsMouseOverTitleBar &&
            MouseManager.X >= X + Contents.Width - TITLE_BAR_HEIGHT &&
            MouseManager.X <= X + Contents.Width;

        private Control GetHoveredControl()
        {
            if (Collapsed) return null;
            foreach (var control in Controls)
                if (control.IsMouseOver) return control;
            return null;
        }

        private bool IsHandling = false;

        internal void HandleMouseInput()
        {
            if (IsHandling) return;
            IsHandling = true;

            // Close (left)
            if (Closable &&
                IsMouseOverCloseButton &&
                MouseManager.MouseState == MouseState.None &&
                previousMouseState == MouseState.Left)
            {
                Closing = true;
            }

            // Roll-up toggle (right)
            if (IsMouseOverMinimizeButton &&
                MouseManager.MouseState == MouseState.None &&
                previousMouseState == MouseState.Left)
            {
                ToggleCollapsed();
            }

            // Dragging (title bar, not on buttons)
            if (IsMouseOverTitleBar &&
                !IsMouseOverCloseButton &&
                !IsMouseOverMinimizeButton &&
                MouseManager.MouseState == MouseState.Left &&
                previousMouseState == MouseState.None)
            {
                DragStartX = X;
                DragStartY = Y;
                DragStartMouseX = (int)MouseManager.X;
                DragStartMouseY = (int)MouseManager.Y;
                Dragging = true;
            }

            if (MouseManager.MouseState == MouseState.None)
                Dragging = false;

            if (MouseManager.MouseState == MouseState.None && previousMouseState == MouseState.Right)
                ShowContextMenu();

            if (Dragging)
            {
                X = (int)(DragStartX + (MouseManager.X - DragStartMouseX));
                int newY = (int)(DragStartY + (MouseManager.Y - DragStartMouseY));
                if (newY < WindowManager.Canvas.Height - 28) Y = newY;
            }

            var hoveredControl = GetHoveredControl();

            // Down, any button.
            if (!Collapsed && MouseManager.MouseState != MouseState.None && previousMouseState == MouseState.None)
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
            if (!Collapsed && MouseManager.MouseState == MouseState.None && previousMouseState == MouseState.Left)
            {
                HandleClick(new MouseEventArgs()
                {
                    X = RelativeMouseX,
                    Y = RelativeMouseY,
                    MouseState = previousMouseState
                });

                FocusedControl = hoveredControl;

                hoveredControl?.HandleClick(new MouseEventArgs()
                {
                    X = RelativeMouseX - hoveredControl.X,
                    Y = RelativeMouseY - hoveredControl.Y,
                    MouseState = MouseManager.MouseState
                });

                foreach (var control in Controls)
                    if (control != hoveredControl) control.HandleUnfocus();
            }

            previousMouseState = MouseManager.MouseState;
            IsHandling = false;
        }

        private void ToggleCollapsed() => Collapsed = !Collapsed;

        public void DrawWindow(Canvas cv, bool focused)
        {
            if (HasTitlebar)
                DrawPlatinumTitleBar(cv, focused);

            if (!Collapsed)
            {
                DrawPlatinumBodyAndFrame(cv);
                cv.DrawImage(X, Y + (HasTitlebar ? TITLE_BAR_HEIGHT : 0), Contents, false);
            }
        }

        private void DrawPlatinumTitleBar(Canvas cv, bool focused)
        {
            // Background
            Color bg = focused ? TitleFocusedBG : TitleUnfocusBG;
            cv.DrawFilledRectangle(X, Y, Contents.Width, TITLE_BAR_HEIGHT, 0, bg);

            // Pin-stripes (subtle)
            for (int yy = 2; yy < TITLE_BAR_HEIGHT - 1; yy += 2)
                cv.DrawLine(X + 1, Y + yy, X + Contents.Width - 2, Y + yy, TitleStripe);

            // Simple bevel
            cv.DrawLine(X, Y, X + Contents.Width - 1, Y, FrameHi);
            cv.DrawLine(X, Y, X, Y + TITLE_BAR_HEIGHT - 1, FrameHi);
            cv.DrawLine(X, Y + TITLE_BAR_HEIGHT - 1, X + Contents.Width - 1, Y + TITLE_BAR_HEIGHT - 1, FrameMid);
            cv.DrawLine(X + Contents.Width - 1, Y, X + Contents.Width - 1, Y + TITLE_BAR_HEIGHT - 1, FrameMid);

            // Buttons
            if (Closable)
            {
                Canvas closeImg = closeButton;
                Canvas miniImg = minimise;

                if (IsMouseOverCloseButton)
                    closeImg = MouseManager.MouseState == MouseState.Left ? closeButtonPressed : closeButtonHover;
                else if (IsMouseOverMinimizeButton)
                    miniImg = MouseManager.MouseState == MouseState.Left ? minimisePressed : minimiseHover;

                cv.DrawImage(X + 1, Y + 1, closeImg);
                cv.DrawImage(X + Contents.Width - TITLE_BAR_HEIGHT + 1, Y + 1, miniImg);
            }

            // Title: centre by pixel width, clamped between buttons
            int textWidth = MeasureTextWidth(Title);
            int centredX = X + ((Contents.Width - textWidth) / 2);

            // Clamp between buttons
            int leftLimit = X + TITLE_BAR_HEIGHT + 3;
            int rightLimit = X + Contents.Width - TITLE_BAR_HEIGHT - 3 - textWidth;

            int titleX = Math.Clamp(centredX, leftLimit, rightLimit);

            cv.DrawString(titleX, Y, Title, Resources.Font_1x, TitleText);
        }

        private void DrawPlatinumBodyAndFrame(Canvas cv)
        {
            int bodyY = Y + (HasTitlebar ? TITLE_BAR_HEIGHT : 0);
            int w = Contents.Width;
            int h = Contents.Height;

            // Fill body face (your darker grey)
            cv.DrawRectangle(X, bodyY, (ushort)w, (ushort)h, 1, FrameBlack);

            // Multi-line Platinum frame (outer->inner), with correct casts + thickness + colour
            DrawRect(cv, X, bodyY, w, h, 1, FrameBlack);
            DrawRect(cv, X + 1, bodyY + 1, w - 2, h - 2, 1, FrameLo);
            DrawRect(cv, X + 2, bodyY + 2, w - 4, h - 4, 1, FrameMid);
            DrawRect(cv, X + 3, bodyY + 3, w - 6, h - 6, 1, FrameHi);
        }

        // Safe wrapper that clamps to >= 1 before casting to ushort
        private static void DrawRect(Canvas cv, int x, int y, int w, int h, ushort thickness, Color colour)
        {
            if (w < 1 || h < 1) return;
            ushort uw = (ushort)(w < 1 ? 1 : w);
            ushort uh = (ushort)(h < 1 ? 1 : h);
            cv.DrawRectangle(x, y, uw, uh, thickness, colour);
        }

        public virtual void HandleClick(MouseEventArgs e) { }
        public virtual void HandleDown(MouseEventArgs e) { }
        public virtual void HandleRelease(MouseEventArgs e) { }

        public virtual void HandleKey(KeyEvent key)
        {
            if (Collapsed) return;
            foreach (var control in Controls)
                if (control == FocusedControl) control.HandleKey(key);
        }

        public virtual void ShowContextMenu() { }

        public void Dispose() { Closing = true; }

        public void RenderOutsetWindowBackground()
        {
            if (Collapsed) return;
            Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, PlatFace);
        }

        public void RenderSystemStyleBorder()
        {
            // No-op: border drawn in DrawPlatinumBodyAndFrame.
        }

        protected void ShowAboutDialog(string version)
        {
            Dialogue.Show(
                $"About {Title}",
                $"GoOS {Title} v{version}\n\nCopyright (c) " + Kernel.Copyright + " Owen2k6\nAll rights reserved.",
                null,
                heightOverride: 144);
        }
        protected void ShowAboutDialog() => ShowAboutDialog(Kernel.version);

        protected void SetDock(WindowDock dock)
        {
            switch (dock)
            {
                case WindowDock.None:
                    X = 0; Y = 0; break;
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

        public virtual void Paint() { }

        public void AutoCreate(WindowDock dock, int Width, int Height, string Title)
        {
            Contents = new Canvas((ushort)Width, (ushort)Height);
            SetDock(dock);
            this.Title = Title;
            Visible = true;
            Closable = true;
        }

        public void AutoCreate(int X, int Y, int Width, int Height, string Title)
        {
            Contents = new Canvas((ushort)Width, (ushort)Height);
            this.X = X;
            this.Y = Y;
            this.Title = Title;
            Visible = true;
            Closable = true;
        }

        public void ShowCrashDialogue(Exception e)
        {
            Dialogue.Show(nameof(WindowManager),
                "The app " + Title + " has thrown an exception and has had to close:\n" + e, default,
                WindowManager.errorIcon);
            Dispose();
        }

        // --- Text measurement (approx) ---
        private static int MeasureTextWidth(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            const int CHAR_W = 8; // adjust: likely 8px for Font_1x, not 6
            return s.Length * CHAR_W;
        }
    }
}
