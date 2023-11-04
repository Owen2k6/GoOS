using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI
{
    public class LoadingDialogue : Window
    {
        public LoadingDialogue(string message)
        {
            Contents = new Canvas(320, 128);
            RenderOutsetWindowBackground();
            X = 240;
            Y = 236;
            Title = "GoOS";
            Visible = true;
            Closable = true;

            Contents.DrawImage(20, 20, drumIcon, true);

            Contents.DrawString(80, 20, message, Resources.Font_1x, Color.Black);
        }
    }
}
