using GoOS.GUI.Models;
using Gold.Graphics;
using System;
using Cosmos.System;

namespace GoOS.GUI
{
    internal class Control
    {
        internal int X, Y;
        internal ushort Width, Height;
        internal bool RenderWithAlpha = false;
        internal bool Pressed;
        internal bool NoOffset;

        internal Window Parent;
        internal Action Clicked;
        internal Action<string> ClickedStr;
        internal Canvas Contents;
        internal string Name;

        public bool IsMouseOver
        {
            get => MouseManager.X > Parent.X + X + (!NoOffset ? Parent.Borderless ? 2 : 6 : 0) &&
                   MouseManager.X < Parent.X + X + Width + (!NoOffset ? Parent.Borderless ? 2 : 6 : 0) &&
                   MouseManager.Y > Parent.Y + Y + (!NoOffset ? Parent.Borderless ? 2 : 22 : 0) &&
                   MouseManager.Y < Parent.Y + Y + Height + (!NoOffset ? Parent.Borderless ? 2 : 22 : 0);
        }

        internal Control(Window Parent, int X, int Y, ushort Width, ushort Height, string Name = "", bool noOffset = false)
        {
            this.Parent = Parent;
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            NoOffset = noOffset;

            Contents = new Canvas(Width, Height);
            Parent.Controls.Add(this);
        }

        internal virtual void HandleDown()
            => Pressed = true;
        
        internal virtual void HandleUp()
            => Pressed = false;

        internal virtual void Render()
            => Parent.RenderControls();

        internal virtual void HandleRun()
        {
            if (Parent.Focused && IsMouseOver &&
                MouseManager.LastMouseState == MouseState.None &&
                MouseManager.MouseState == MouseState.Left && !Pressed)
            {
                HandleDown();
            }

            if (Parent.Focused && IsMouseOver &&
                MouseManager.LastMouseState == MouseState.Left &&
                MouseManager.MouseState == MouseState.None && Pressed)
            {
                HandleUp();
                
                Clicked?.Invoke();
                ClickedStr?.Invoke(Name);
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