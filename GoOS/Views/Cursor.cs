using CitrineUI.Input;
using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// Renders the mouse cursor, handles interaction, and dispatches mouse events.
    /// </summary>
    public class Cursor : View
    {
        public Cursor(View initialParent)
        {
            Parent = initialParent;
        }

        [ManifestResourceStream(ResourceName = "GoOS.Assets.Cursors.cursor.bmp")]
        private static byte[] cursorBytes;

        [ManifestResourceStream(ResourceName = "GoOS.Assets.Cursors.grab.bmp")]
        private static byte[] grabCursorBytes;

        [ManifestResourceStream(ResourceName = "GoOS.Assets.Cursors.text.bmp")]
        private static byte[] textCursorBytes;

        private static Bitmap cursorBitmap = new Bitmap(cursorBytes);
        private static Bitmap grabCursorBitmap = new Bitmap(grabCursorBytes);
        private static Bitmap textCursorBitmap = new Bitmap(textCursorBytes);

        private bool grabbing = false;

        // The state of the mouse last render
        private MouseState lastMouseState = MouseState.None;

        private CursorDisplay cursorDisplay = CursorDisplay.Cursor;

        Bitmap currentBitmap = cursorBitmap;

        private View GetViewAtCursor(View checkView)
        {
            if (checkView.Children.Count == 0) return checkView;
            for (int i = checkView.Children.Count - 1; i >= 0; i--)
            {
                if (!checkView.Children[i].Active || checkView.Children[i] is Cursor) continue;
                if (checkView.Children[i].Rectangle.Contains(ScreenBounds.X, ScreenBounds.Y))
                {
                    return GetViewAtCursor(checkView.Children[i]);
                }
            }
            return null;
        }

        private View GetViewAtCursor()
        {
            for (int i = Desktop.AllViews.Count - 1; i >= 0; i--)
            {
                if (!Desktop.AllViews[i].Active || Desktop.AllViews[i] is Cursor) continue;
                if (Desktop.AllViews[i].ScreenBounds.Contains(ScreenBounds.X, ScreenBounds.Y))
                {
                    return Desktop.AllViews[i];
                }
            }
            return Desktop;
        }

        /// <summary>
        /// Update the cursor bounds and dispatch events.
        /// </summary>
        public void Update()
        {
            View view = GetViewAtCursor();

            int relativeX = Rectangle.X - view.ScreenBounds.X;
            int relativeY = Rectangle.Y - view.ScreenBounds.Y;

            if (grabbing)
            {
                cursorDisplay = CursorDisplay.Grab;
            }
            else if (view is TextBox)
            {
                cursorDisplay = CursorDisplay.Text;
            }
            else
            {
                cursorDisplay = CursorDisplay.Cursor;
            }

            if (MouseManager.MouseState == MouseState.None && lastMouseState != MouseState.None)
            {
                bool interrupted = false;

                // If there is a modal open, close it.
                foreach (View view2 in Desktop.AllViews)
                {
                    if (view2 is Window window)
                    {
                        if (window.Modal && !window.ScreenBounds.Contains(Rectangle.X, Rectangle.Y))
                        {
                            window.Remove();
                            interrupted = true;
                        }
                    }
                }

                if (!interrupted)
                {
                    if (lastMouseState == MouseState.Left)
                    {
                        // Check boxes.
                        // Must be set before the click event is dispatched.
                        if (view is CheckBox checkBox)
                        {
                            checkBox.Checked = !checkBox.Checked;
                        }

                        // Invocation.
                        view.ClickedHandler?.Invoke(view, new EventArgs());

                        // Effects.
                        if (view is Button button)
                        {
                            button.Highlight = 1f;
                        }

                        // Tags.
                        if (view.Tag == "Titlebar")
                        {
                            Window window = (Window)view.Parent;
                            window.Dragging = false;
                            window.Invalid = true;

                            grabbing = false;
                        }
                        if (view.Tag == "Close")
                        {
                            ((Window)view.Parent.Parent).Remove();
                        }

                        // Interfaces.
                        if (view is IMouseListener mouseListener)
                        {
                            mouseListener.OnClick(relativeX, relativeY);
                        }
                    }
                    else if (lastMouseState == MouseState.Right)
                    {
                        if (view.ContextMenuItems != null && view.ContextMenuItems.Count > 0)
                        {
                            new ContextMenuWindow((int)MouseManager.X, (int)MouseManager.Y, view, Desktop);
                        }
                    }
                }
            }
            else if (MouseManager.MouseState == MouseState.Left && lastMouseState == MouseState.None)
            {
                Desktop.Focus = view;

                if (view is IMouseListener mouseListener)
                {
                    mouseListener.OnMouseDown(relativeX, relativeY);
                }

                if (view.Tag == "Titlebar")
                {
                    Window window = (Window)view.Parent;
                    window.Dragging = true;
                    window.DragStartMouseX = Rectangle.X;
                    window.DragStartMouseY = Rectangle.Y;
                    window.DragStartWindowX = window.Rectangle.X;
                    window.DragStartWindowY = window.Rectangle.Y;
                    window.Invalid = true;

                    grabbing = true;
                }
            }
            lastMouseState = MouseManager.MouseState;
        }

        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            /*Invalid = false;
            if (buffer == null)
            {
                buffer = new Color[Area];
            }
            if (hasOldPosition)
            {
                for (int y = 0; y < Rectangle.Height; y++)
                {
                    for (int x = 0; x < Rectangle.Width; x++)
                    {
                        pen.Color = buffer[(Rectangle.Width * y) + x];
                        Desktop.canvas.DrawPoint(pen, oldPosition.X + x, oldPosition.Y + y);
                    }
                }
            }
            CalculateScreenBounds();
            oldPosition = ScreenBounds.Location;
            hasOldPosition = true;
            for (int y = 0; y < Rectangle.Height; y++)
            {
                for (int x = 0; x < Rectangle.Width; x++)
                {
                    buffer[(Rectangle.Width * y) + x] = Desktop.canvas.GetPointColor(ScreenBounds.X + x, ScreenBounds.Y + y);
                }
            }
            desktop.canvas.DrawFilledRectangle(new Cosmos.System.Graphics.Pen(Color.LightSalmon), ScreenBounds.X, ScreenBounds.Y, ScreenBounds.Width, ScreenBounds.Height);
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }*/
            Invalid = false;
            if (!OldBounds.IsEmpty)
            {
                RenderOldBounds();
            }
            CalculateScreenBounds();

            switch (cursorDisplay)
            {
                case CursorDisplay.Cursor:
                    Rectangle = new Rectangle((int)MouseManager.X, (int)MouseManager.Y, (int)cursorBitmap.Width, (int)cursorBitmap.Height);
                    currentBitmap = cursorBitmap;
                    break;
                case CursorDisplay.Grab:
                    Rectangle = new Rectangle((int)MouseManager.X, (int)MouseManager.Y, (int)grabCursorBitmap.Width, (int)grabCursorBitmap.Height);
                    currentBitmap = grabCursorBitmap;
                    break;
                case CursorDisplay.Text:
                    Rectangle = new Rectangle((int)MouseManager.X, (int)MouseManager.Y, (int)textCursorBitmap.Width, (int)textCursorBitmap.Height);
                    currentBitmap = textCursorBitmap;
                    break;
            }

            desktop.canvas.DrawImageAlpha(currentBitmap, ScreenBounds.X, ScreenBounds.Y);
            foreach (var child in Children)
            {
                child.Render(desktop, ScreenBounds);
            }
        }
    }
}
