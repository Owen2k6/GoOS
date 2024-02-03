using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using GoGL.Graphics;

namespace GoOS.GUI.Apps
{
    public class About : Window
    {
        public About()
        {
            // Create the window.
            Contents = new Canvas(200, 180);
            Title = "About this GoPC";
            Visible = true;
            Closable = true;
            Sizable = false;
            SetDock(WindowDock.Auto);
            // Paint the window.
            Contents.DrawImage(0, 0, Resources.abtbg, false);
            Contents.DrawString(50, 150, "(Version " + Kernel.version + ")", Resources.Font_1x, Color.White);
        }
    }
}