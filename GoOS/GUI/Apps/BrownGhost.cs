using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoOS.GUI;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class BrownGhost : Window
    {
        public BrownGhost()
        {
            Contents = new Canvas(238, 150);
            Title = "Boooo";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            Contents.Clear(Color.Black);
            RenderSystemStyleBorder();
            Contents.DrawImage(10, 51, brownGhost);
            Contents.DrawString(61, 67, "Aaaa! A brown ghost!", Font_1x, Color.White);
        }
    }
}
