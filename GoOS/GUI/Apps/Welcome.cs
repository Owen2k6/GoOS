using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class Welcome : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.Welcome.bmp")] private static byte[] welcomeImageRaw;
        private static Canvas welcomeImage = Image.FromBitmap(welcomeImageRaw, false);

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
