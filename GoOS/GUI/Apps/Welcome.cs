using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class Welcome : Window
    {
        Button closeButton;

        public Welcome()
        {
            Contents = new Canvas(400, 300);
            Title = "Welcome";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Center);

            Contents.DrawImage(0, 0, welcomeImage, true);

            closeButton = new Button(this, 315, 270, 80, 25, "Close");
            closeButton.Clicked = Dispose;

            closeButton.Render();
        }
    }
}
