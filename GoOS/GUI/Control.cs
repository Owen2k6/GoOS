using GoOS.GUI.Models;
using PrismAPI.Graphics;
using System;

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

        public abstract void Render();
    }
}
