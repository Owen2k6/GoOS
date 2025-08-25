using GoOS.GUI.Models;
using Gold.Graphics;
using System;
using Cosmos.System;

namespace GoOS.GUI
{
    internal abstract class Control
    {
        internal int X, Y;
        internal ushort Width, Height;
        internal bool RenderWithAlpha = false;
        internal bool Pressed;

        internal Window Parent;
        internal Action Clicked;
        internal Canvas Contents;

        public bool IsMouseOver
        {
            get => MouseManager.X > Parent.X + X + (Parent.Borderless ? 2 : 6) &&
                   MouseManager.X < Parent.X + X + Width + (Parent.Borderless ? 2 : 6) &&
                   MouseManager.Y > Parent.Y + Y + (Parent.Borderless ? 2 : 22) &&
                   MouseManager.Y < Parent.Y + Y + Height + (Parent.Borderless ? 2 : 22);
        }

        protected Control(Window Parent, int X, int Y, ushort Width, ushort Height)
        {
            this.Parent = Parent;
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;

            Contents = new Canvas(Width, Height);
            Parent.Controls.Add(this);
        }

        internal virtual void HandleDown()
            => Pressed = true;
        
        internal virtual void HandleDown(MouseEventArgs args)
        {
            HandleDown();
        }

        internal virtual void HandleUp()
            => Pressed = false;

        internal virtual void Render()
            => Parent.RenderControls();
        
        internal virtual void HandleKey(KeyEvent key)
        {
        }

        internal virtual void HandleRun()
        {
            if (Parent.Focused && IsMouseOver &&
                MouseManager.LastMouseState == MouseState.None &&
                MouseManager.MouseState == MouseState.Left)
            {
                HandleDown();
            }

            if (Parent.Focused && IsMouseOver &&
                MouseManager.LastMouseState == MouseState.Left &&
                MouseManager.MouseState == MouseState.None)
            {
                HandleUp();

                if (IsMouseOver) Clicked?.Invoke();
            }
        }
    }
    
    /*public abstract class Control
    {
        public Control(Window parent, ushort x, ushort y, ushort width, ushort height)
        {
            Parent = parent;
            parent.Controls.Add(this);

            Contents = new Canvas(width, height);
            X = x;
            Y = y;
        }

        public Canvas Contents;
        public ushort X, Y;
        public bool Visible = true;
        public bool RenderWithAlpha = false;

        public Action Clicked;
        public Action<string> ClickedAlt;

        public string Name;

        public Window Parent;

        public bool IsMouseOver
        {
            get
            {
                return MouseManager.X >= Parent.X + X &&
                       MouseManager.X < Parent.X + X + Contents.Width &&
                       MouseManager.Y >= Parent.Y + Y + (Parent.HasTitlebar ? Window.TITLE_BAR_HEIGHT : 0) &&
                       MouseManager.Y < Parent.Y + Y + Contents.Height +
                       (Parent.HasTitlebar ? Window.TITLE_BAR_HEIGHT : 0);
            }
        }

        public abstract void Render();

        internal virtual void HandleDown(MouseEventArgs args)
        {
        }

        internal virtual void HandleRelease()
        {
        }

        internal virtual void HandleClick(MouseEventArgs args)
        {
            Clicked?.Invoke();
            ClickedAlt?.Invoke(Name);
        }

        internal virtual void HandleUnfocus()
        {
        }

        

        public virtual void Update()
        {
        }
    }*/
}