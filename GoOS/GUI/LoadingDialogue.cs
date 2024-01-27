using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI
{
    public class LoadingDialogue : Window
    {
        public LoadingDialogue(string message)
        {
            Contents = new Canvas(320, 128);
            RenderOutsetWindowBackground();
            Title = "GoOS";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            Contents.DrawImage(20, 20, drumIcon, true);

            Contents.DrawString(80, 20, message, Font_1x, Color.White);
        }
    }
}