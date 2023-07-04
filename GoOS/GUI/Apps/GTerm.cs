using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOS.GUI.Apps
{
    public class GTerm : Window
    {
        public GTerm()
        {
            this.Contents = BetterConsole.Canvas;
            this.X = 20;
            this.Y = 50;
            this.Title = "GTerm";
            this.Visible = true;
            this.Closable = true;
            BetterConsole.Visible = true;
        }

        public override void Update()
        {
            if (!BetterConsole.Visible)
            {
                Closing = true;
            }
        }
    }
}
