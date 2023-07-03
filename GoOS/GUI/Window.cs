using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using PrismAPI.Graphics;

namespace GoOS.GUI
{
    public abstract class Window
    {
        private bool mouseClicked = false;

        public Canvas Contents;
        public int X, Y; //This doesnt have to be a UInt16
        public string Title;
        public bool Visible;
        public bool Closeable;
        public bool Moving;

        public abstract void Update();

        internal bool MouseOnTop()
        {
            if (Sys.MouseManager.X > this.X && Sys.MouseManager.X < this.X + this.Contents.Width && Sys.MouseManager.Y > this.Y + 16 && Sys.MouseManager.Y < this.Y + this.Contents.Height)
                return true;
            else
                return false;
        }

        internal void InternalFullUpdate()
        {
            if (Sys.MouseManager.MouseState == Sys.MouseState.Left)
            {
                if (mouseClicked)
                {
                    if (Sys.MouseManager.X > this.X && Sys.MouseManager.X < this.X + this.Contents.Width - 16 && Sys.MouseManager.Y > this.Y - (this.Contents.Height / 2) && Sys.MouseManager.Y < this.Y + this.Contents.Height)
                    {
                        this.X = (int)Sys.MouseManager.X - (this.Contents.Width / 2);
                        this.Y = (int)Sys.MouseManager.Y - 8;
                    }
                }
                else
                {
                    if (Sys.MouseManager.X > this.X && Sys.MouseManager.X < this.X + this.Contents.Width - 16 && Sys.MouseManager.Y > this.Y && Sys.MouseManager.Y < this.Y + 16)
                    {
                        this.X = (int)Sys.MouseManager.X - (this.Contents.Width / 2);
                        this.Y = (int)Sys.MouseManager.Y - 8;
                        mouseClicked = true;
                    }
                }

                if (this.Closeable)
                {
                    if (Sys.MouseManager.X > this.X + this.Contents.Width - 14 && Sys.MouseManager.X < this.X + this.Contents.Width - 2 && Sys.MouseManager.Y > this.Y + 2 && Sys.MouseManager.Y < this.Y + 14)
                    {
                        this.Visible = false;
                    }
                }
            }
            else
            {
                mouseClicked = false;
            }
        }
    }
}
