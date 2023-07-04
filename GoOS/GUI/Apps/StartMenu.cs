using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class StartMenu : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.user.bmp")] private static byte[] userImageRaw;
        private static Canvas userImage = Image.FromBitmap(userImageRaw, false);

        Button[] apps;

        public StartMenu()
        {
            Contents = new Canvas(380, 500);
            Contents.Clear(Color.DeepGray);
            X = 0;
            Y = WindowManager.Canvas.Height - 28 - Contents.Height;
            HasTitlebar = false;
            Visible = true;

            Contents.DrawImage(8, 8, userImage);
            Contents.DrawString(40, 16, Kernel.username, BetterConsole.font, Color.White);

            apps = new Button[2];
            apps[0] = new Button(this, 8, 48, 134, 16, "GTerm");
            apps[0].Clicked = GTerm_Click;

            apps[1] = new Button(this, 8, 80, 134, 16, "Clock");
            apps[1].Clicked = Clock_Click;

            foreach (var app in apps)
            {
                if (app != null)
                {
                    app.Render();
                }
            }
        }

        private void GTerm_Click()
        {
            WindowManager.AddWindow(new GTerm());
            Dispose();
        }

        private void Clock_Click()
        {
            WindowManager.AddWindow(new Clock());
            Dispose();
        }
    }
}
