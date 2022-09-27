using System;
using System.Collections.Generic;
using System.Drawing;

namespace CitrineUI.Views
{
    /// <summary>
    /// The View is an abstract base class for all UI elements. It implements basic hierarchy, layout, and event handling.
    /// </summary>
    public abstract class View
    {
        /// <summary>
        /// The children of this view. Children are automatically rendered when this view is rendered.
        /// </summary>
        public List<View> Children = new List<View>();

        internal Desktop Desktop;

        private View parent;
        /// <summary>
        /// The parent of this view.
        /// </summary>
        public View Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (value == null)
                {
                    Remove();
                }
                else
                {
                    if (parent == value) return;
                    if (parent != null) parent.Children.Remove(this);
                    parent = value;
                    value.Children.Add(this);
                    if (value is Desktop desktop)
                    {
                        Desktop = desktop;
                    }
                    else
                    {
                        Desktop = value.Desktop;
                    }
                    if (Desktop != null)
                    {
                        Desktop.RegisterView(this);
                        foreach (View descendant in GetDescendants())
                        {
                            descendant.Desktop = Desktop;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add the descendants of this view to the given list.
        /// </summary>
        /// <param name="descendants">The list to add to.</param>
        private void AddDescendants(List<View> descendants)
        {
            foreach (var child in Children)
            {
                descendants.Add(child);
                child.AddDescendants(descendants);
            }
        }

        /// <summary>
        /// Gets all descendants of this view.
        /// </summary>
        /// <returns>A list containing all descendants of the view</returns>
        public List<View> GetDescendants()
        {
            List<View> descendants = new List<View>();
            AddDescendants(descendants);
            return descendants;
        }

        private Rectangle screenBounds = Rectangle.Empty;
        /// <summary>
        /// The calculated bounds of the view, relative to the screen.
        /// </summary>
        public Rectangle ScreenBounds
        {
            get
            {
                return screenBounds;
            }
            set
            {
                if (screenBounds != value)
                {
                    OldBounds = screenBounds;
                }
                screenBounds = value;
            }
        }

        /// <summary>
        /// The position of the view, relative to its parent.
        /// </summary>
        public Point Position { get; private set; }
        /// <summary>
        /// The size of the view.
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// The rectangle of the view.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return new Rectangle(Position, Size); }
            set { Position = value.Location; Size = value.Size; Invalid = true; }
        }

        /// <summary>
        /// The area of the view.
        /// </summary>
        public int Area { get { return Rectangle.Width * Rectangle.Height; } }

        /// <summary>
        /// The perimeter of the view.
        /// </summary>
        public int Perimeter { get { return 2 * (Rectangle.Width + Rectangle.Height); } }

        /// <summary>
        /// If the view needs to be rendered.
        /// </summary>
        public bool Invalid = true;

        /// <summary>
        /// The old bounds of the view if it has not been rendered since it was moved or resized.
        /// </summary>
        public Rectangle OldBounds;

        /// <summary>
        /// Where the Position should be relative to. By default, the top left.
        /// </summary>
        public PointF Anchor = new PointF(0f, 0f);

        private bool fillWidth = false;
        /// <summary>
        /// Whether the view should fill the width of its parent.
        /// </summary>
        public virtual bool FillWidth
        {
            get { return fillWidth; }
            set { fillWidth = value; Invalid = true; }
        }

        private bool fillHeight = false;
        /// <summary>
        /// Whether the view should fill the height of its parent.
        /// </summary>
        public virtual bool FillHeight
        {
            get { return fillHeight; }
            set { fillHeight = value; Invalid = true; }
        }

        /// <summary>
        /// Whether the view recieves user interaction. If set to false, it will pass it through.
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// Context menu items to display when the view is right clicked.
        /// </summary>
        public List<ContextMenuItem> ContextMenuItems = null;

        /// <summary>
        /// Whether the view is focused.
        /// </summary>
        public bool Focused
        {
            get
            {
                return Desktop.Focus == this;
            }
            set
            {
                if (value)
                {
                    Desktop.Focus = this;
                }
                else if (Desktop.Focus == this)
                {
                    Desktop.Focus = null;
                }
            }
        }

        internal string Tag;

        /// <summary>
        /// Renders the view and all of its children.
        /// </summary>
        /// <param name="desktop">The desktop to render to.</param>
        /// <param name="parentBounds">The bounds of the parent view.</param>
        public abstract void Render(Desktop desktop, Rectangle parentBounds);

        /// <summary>
        /// Calculate the bounds of the view, relative to the screen.
        /// </summary>
        /// <param name="parentBounds">The bounds of the parent view.</param>
        internal virtual void CalculateScreenBounds()
        {
            Rectangle parentBounds = Parent.ScreenBounds;
            if (parent is Window window && window.Titlebar != null && Tag != "Titlebar")
            {
                parentBounds = new Rectangle(
                    parentBounds.X,
                    parentBounds.Y + window.Titlebar.Rectangle.Height,
                    parentBounds.Width,
                    parentBounds.Height - window.Titlebar.Rectangle.Height
                );
            }
            int x;
            int y;
            if (FillWidth)
            {
                x = parentBounds.X;
            }
            else
            {
                x = (int)(parentBounds.X + Rectangle.X + parentBounds.Width * Anchor.X - Rectangle.Width * Anchor.X);
            }
            if (FillHeight)
            {
                y = parentBounds.Y;
            }
            else
            {
                y = (int)(parentBounds.Y + Rectangle.Y + parentBounds.Height * Anchor.Y - Rectangle.Height * Anchor.Y);
            }
            ScreenBounds = new Rectangle(
                x,
                y,
                FillWidth ? parentBounds.Width : Rectangle.Width,
                FillHeight ? parentBounds.Height : Rectangle.Height
            );
        }

        /// <summary>
        /// Mark all views in the old bounds as invalid.
        /// </summary>
        public virtual void RenderOldBounds()
        {
            Desktop.ClearRectangles.Add(OldBounds);
            foreach (View view in Desktop.AllViews)
            {
                if (view == this)
                {
                    continue;
                }
                if (view.ScreenBounds.IntersectsWith(OldBounds))
                {
                    ////Kernel.PrintDebug("Marking " + view.Tag + " as invalid, from a view " + view.Tag);
                    ////Kernel.PrintDebug("My old bounds are " + OldBounds.ToString());
                    view.Invalid = true;
                }
            }
            OldBounds = Rectangle.Empty;
        }

        /// <summary>
        /// Remove the view from its parent and render views that were behind it.
        /// You must remove your reference to the view or the garbage collector will not free it.
        /// </summary>
        public void Remove()
        {
            //Kernel.PrintDebug("Removing... " + Rectangle.ToString());
            if (parent != null)
            {
                parent.Children.Remove(this);
                parent = null;
            }
            if (Desktop != null)
            {
                //Kernel.PrintDebug("DR");
                Desktop.AllViews.Remove(this);
                //Kernel.PrintDebug("DR--OK");
                foreach (View view in GetDescendants())
                {
                    Desktop.AllViews.Remove(view);
                }
                //Kernel.PrintDebug("DR--OK2");
            }
            if (!screenBounds.IsEmpty)
            {
                OldBounds = ScreenBounds;
                RenderOldBounds();
            }
            //Kernel.PrintDebug("Removed.");
        }

        /// <summary>
        /// Set the order of the view in its parent's child list.
        /// </summary>
        /// <param name="z">The new index of the view in the parent's child list.</param>
        public void SetOrder(int z)
        {
            throw new NotImplementedException();
            if (Parent == null)
            {
                return;
            }
            ////Kernel.PrintDebug("-----------------------------------");
            ////Kernel.PrintDebug("||| My current index is: " + Parent.Children.IndexOf(this));
            ////Kernel.PrintDebug(">>> My goal is " + z);
            //Parent.Children.Move(Parent.Children.IndexOf(this), Math.Clamp(z, 0, Parent.Children.Count - 1));
            ////Kernel.PrintDebug("+++ My *fresh, new* index is: " + Parent.Children.IndexOf(this));
            ////Kernel.PrintDebug("-----------------------------------");
            Invalid = true;
        }

        /// <summary>
        /// Bring the view to the front.
        /// </summary>
        public void BringToFront()
        {
            SetOrder(Parent.Children.Count - 1);
        }

        /// <summary>
        /// Send the view to the back.
        /// </summary>
        public void SendToBack()
        {
            SetOrder(0);
        }

        internal EventHandler ClickedHandler;
        /// <summary>
        /// Fires when the view is clicked.
        /// </summary>
        public event EventHandler Clicked
        {
            add
            {
                ClickedHandler = value;
            }
            remove
            {
                ClickedHandler -= value;
            }
        }
    }
}
