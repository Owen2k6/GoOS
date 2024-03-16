using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using GoOS._9xCode;

namespace GoOS.GUI.Apps
{
    public class About : Window
    {
        public About()
        {
            // Create the window.
            Contents = new Canvas(200, 220);
            Title = "About this GoPC";
            Visible = true;
            Closable = true;
            Sizable = false;
            SetDock(WindowDock.Auto);
            // Paint the window.
            Contents.DrawImage(0, 0, Resources.abtbg, false);
            Contents.DrawString(10, 152,  "GoOS "+Kernel.version, Resources.Font_1x, Color.White);
            Contents.DrawString(10, 164,  "GoGL "+new GoGL.Info().getVersion(), Resources.Font_1x, Color.White);
            Contents.DrawString(10, 176,  "GoCode "+GoCode.GoCode.Version, Resources.Font_1x, Color.White);
            Contents.DrawString(10, 188,  "9xCode "+Interpreter.Version, Resources.Font_1x, Color.White);
        }
    }
}