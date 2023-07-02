using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class Demo : Window
    {
        Button test;

        public Demo()
        {
            this.Contents = new Canvas(400, 400);
            this.Contents.Clear(Color.White);
            this.X = 500;
            this.Y = 100;
            this.Title = "Demo";
            this.Visible = true;

            test = new Button(Convert.ToUInt16(X + 10), Convert.ToUInt16(Y + 16 + 10), 50, 16, "Button", true);
            this.Contents.DrawImage(test.X - X, test.Y - Y - 16, test.Contents, false);
        }

        public override void Update()
        {
            test.Handle();
        }
    }
}
