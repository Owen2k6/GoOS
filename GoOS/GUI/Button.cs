using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using PrismAPI.Graphics;

namespace GoOS.GUI
{
    public class Button
    {
        public Canvas Contents;
        public ushort X, Y;
        public string Title;
        public bool Visible;

        public Button(ushort X, ushort Y, ushort Width, ushort Height, string Title, bool Visible)
        {
            this.Contents = new Canvas(Width, Height);
            this.Contents.DrawFilledRectangle(0, 0, Width, Height, 3, new Color(12, 12, 12));
            this.Contents.DrawString(0, 0, Title, BetterConsole.font, Color.White);
            this.X = X;
            this.Y = Y;
            this.Title = Title;
            this.Visible = Visible;
        }

        public void Handle()
        {
            if (Sys.MouseManager.MouseState == Sys.MouseState.Left)
            {
                if (Sys.MouseManager.X > X && Sys.MouseManager.X < X + Contents.Width && Sys.MouseManager.Y > Y && Sys.MouseManager.Y < Y + Contents.Height)
                {
                    WindowManager.Windows.Add(new Apps.GTerm());
                }
            }
        }
    }
}
