using CitrineUI.Animations;
using CitrineUI.Input;
using CitrineUI.Text;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// The root view that manages all other views and a canvas.
    /// </summary>
    public class Desktop : View
    {
        /// <summary>
        /// The canvas that the desktop is drawn on.
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// The current mouse cursor.
        /// </summary>
        public Cursor Cursor;

        private Pen backgroundPen = new Pen(Color.Black);
        /// <summary>
        /// The background colour of the desktop.
        /// </summary>
        public Color BackgroundColor
        {
            get { return backgroundPen.Color; }
            set { backgroundPen.Color = value; Invalid = true; }
        }

        private View focus;
        /// <summary>
        /// The currently focused view.
        /// </summary>
        public View Focus
        {
            get
            {
                return focus;
            }
            set
            {
                if (focus != null)
                {
                    focus.Invalid = true;
                }
                if (value == this)
                {
                    focus = null;
                    return;
                }
                focus = value;
                if (value != null)
                {
                    value.Invalid = true;
                }
            }
        }

        /// <summary>
        /// All views in the desktop and their descendants.
        /// </summary>
        internal List<View> AllViews = new List<View>();

        /// <summary>
        /// A list of rectangles to clear with the background colour before rendering views.
        /// </summary>
        internal List<Rectangle> ClearRectangles = new List<Rectangle>();

        /// <summary>
        /// The currently playing animations.
        /// </summary>
        public List<Animation> Animations = new List<Animation>();

        /// <summary>
        /// The text on the clipboard.
        /// </summary>
        public string Clipboard = "";

        /// <summary>
        /// Initialise the desktop.
        /// </summary>
        /// <param name="canvas">The canvas that the desktop is drawn on.</param>
        public Desktop(Canvas canvas)
        {
            this.canvas = canvas;
            Rectangle = new Rectangle(0, 0, canvas.Mode.Columns, canvas.Mode.Rows);
        }

        /// <summary>
        /// Render the desktop.
        /// </summary>
        /// <remarks>Does not display the canvas.</remarks>
        public void Render()
        {
            Render(this, Rectangle);
        }

        /// <summary>
        /// Register a view with the desktop.
        /// </summary>
        internal void RegisterView(View view)
        {
            if (view.Parent != this && view.Parent != null && !GetDescendants().Contains(view.Parent))
            {
                return;
            }
            if (!AllViews.Contains(view))
            {
                AllViews.Add(view);
                foreach (View descendant in view.GetDescendants())
                {
                    RegisterView(descendant);
                }
            }
        }

        public void CreateCursor()
        {
            if (Cursor != null)
            {
                throw new Exception("A cursor already exists on this desktop.");
            }
            Cursor = new Cursor(this);
        }

        public void RemoveCursor()
        {
            if (Cursor == null)
            {
                throw new Exception("No cursor exists on this desktop.");
            }
            Cursor.Remove();
            Cursor = null;
        }

        /// <summary>
        /// Set the mode of the canvas and update the desktop accordingly.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <param name="depth">The new colour depth.</param>
        public void SetMode(int width, int height, ColorDepth depth)
        {
            if (width == Rectangle.Width && height == Rectangle.Height && canvas.Mode.ColorDepth == depth) return;
            canvas.Mode = new Mode(width, height, depth);
            MouseManager.ScreenWidth = (uint)width;
            MouseManager.ScreenHeight = (uint)height;
            Invalid = true;
            Rectangle = new Rectangle(0, 0, canvas.Mode.Columns, canvas.Mode.Rows);
        }

        public int Frame = 0;
        public int FPS = 0;

        private int framesThisSecond = 0;
        private int second = 0;

        private Pen black = new Pen(Color.Black);
        private Pen white = new Pen(Color.White);

        /// <summary>
        /// Render the desktop and all views it contains.
        /// </summary>
        /// <param name="desktop">The desktop to render.</param>
        /// <param name="parentBounds">The rectangle of the screen.</param>
        public override void Render(Desktop desktop, Rectangle parentBounds)
        {
            //Kernel.PrintDebug("1");
            if (desktop != this || parentBounds != Rectangle)
            {
                throw new ArgumentException();
            }
            ScreenBounds = Rectangle;

            if (second != Cosmos.HAL.RTC.Second)
            {
                FPS = framesThisSecond;
                framesThisSecond = 0;
                second = Cosmos.HAL.RTC.Second;
            }
            framesThisSecond++;
            Frame++;
            if (Cursor != null)
            {
                Cursor.Update();
            }
            //Kernel.PrintDebug("2");

            if (Focus != null && Focus is IKeyListener)
            {
                KeyEvent key;
                KeyboardManager.TryReadKey(out key);
                if (key != null)
                {
                    (Focus as IKeyListener).OnKeyPressed(key);
                }
            }

            foreach (Animation animation in Animations)
            {
                animation.Advance();
                animation.View.Invalid = true;
                if (animation.Finished)
                {
                    Animations.Remove(animation);
                }
            }
            //Kernel.PrintDebug("3");

            if (Invalid)
            {
                canvas.Clear(BackgroundColor);
                foreach (View view in Children)
                {
                    view.Invalid = true;
                }
                Invalid = false;
            }
            else if (ClearRectangles.Count > 0)
            {
                foreach (Rectangle rectangle in ClearRectangles)
                {
                    canvas.DrawFilledRectangle(backgroundPen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                }
                ClearRectangles.Clear();
            }
            //Kernel.PrintDebug("4");
            // Make all views above invalid views invalid as well.
            for (int i = 0; i < AllViews.Count; i++)
            {
                if (!AllViews[i].Invalid) continue;
                for (int j = i + 1; j < AllViews.Count; j++)
                {
                    if (AllViews[i].ScreenBounds.IntersectsWith(AllViews[j].ScreenBounds))
                    {
                        AllViews[j].Invalid = true;
                    }
                }
            }

            if (Cursor != null)
            {
                //canvas.DrawFilledRectangle(backgroundPen, Cursor.Rectangle.X, Cursor.Rectangle.Y, Cursor.Rectangle.Width, Cursor.Rectangle.Height);
                for (int i = 0 + 1; i < AllViews.Count; i++)
                {
                    if (AllViews[i].ScreenBounds.IntersectsWith(Cursor.ScreenBounds))
                    {
                        AllViews[i].Invalid = true;
                    }
                }
            }

            foreach (View view in AllViews)
            {
                if (view == Cursor) continue;
                // To avoid rendering a view twice, skip it if its parent is invalid.
                if (view.Parent != null && view.Parent != Desktop && view.Parent.Invalid)
                {
                    continue;
                }
                if (view.Invalid)
                {
                    view.Render(desktop, view.Parent.ScreenBounds);
                }
            }

            if (Cursor != null)
            {
                Cursor.Render(desktop, Cursor.Parent.ScreenBounds);
            }
            //Kernel.PrintDebug("5");

            if (Frame % 30 == 0 && false)
            {
                canvas.DrawFilledRectangle(black, 0, Rectangle.Height - TextRenderer.Font.Height, Rectangle.Width, TextRenderer.Font.Height);
                canvas.DrawString($"FPS: {FPS} - Frame: {Frame} - Used memory: {Cosmos.Core.GCImplementation.GetUsedRAM()}", TextRenderer.Font, white, 0, Rectangle.Height - TextRenderer.Font.Height);
            }
            //Kernel.PrintDebug("6");

            canvas.Display();
        }
    }
}
