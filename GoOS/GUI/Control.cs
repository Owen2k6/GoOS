using GoOS.GUI.Models;
using PrismAPI.Graphics;
using System;
using Cosmos.System;

namespace GoOS.GUI
{
    public abstract class Control
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

        public Action Clicked;

        public Window Parent;

        public bool IsMouseOver
        {
            get
            {
                return MouseManager.X >= Parent.X + X                                                     &&
                   MouseManager.X     < Parent.X  + X + Contents.Width                                    &&
                   MouseManager.Y     >= Parent.Y + Y + (Parent.HasTitlebar ? 19 : 0)                     &&
                   MouseManager.Y     < Parent.Y  + Y + Contents.Height + (Parent.HasTitlebar ? 19 : 0);
            }
        }

        public abstract void Render();

        internal virtual void HandleDown() { }

        internal virtual void HandleRelease() { }
    }
}
