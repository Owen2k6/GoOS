using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class AppManager : Window
    {
        Button GTerm;

        public AppManager()
        {
            this.Contents = new Canvas(200, 150);
            this.Contents.Clear(Color.White);
            this.X = 830;
            this.Y = 100;
            this.Title = "GoOS App Manager";
            this.Visible = true;
            this.Closeable = false;

            GTerm = new Button(Convert.ToUInt16(X + 10), Convert.ToUInt16(Y + 16 + 10), 50, 16, "GTerm", true);
            this.Contents.DrawImage(GTerm.X - X, GTerm.Y - Y - 16, GTerm.Contents, false);
        }

        public override void Update()
        {
            GTerm.X = Convert.ToUInt16(X + 10);
            GTerm.Y = Convert.ToUInt16(Y + 16 + 10);
            GTerm.Handle();
        }
    }
}
