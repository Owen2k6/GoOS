using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.Gosplorer
{
    public class OpenWithFrame : Window
    {
        public OpenWithFrame()
        {
            Contents = new Canvas(480, 360);
            Title = nameof(OpenWithFrame);
            Visible = true;
            HasTitlebar = false;
            SetDock(WindowDock.Center);
        }

        public override void Paint()
        {
            

            RenderSystemStyleBorder();
        }
    }
}
