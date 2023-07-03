using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class AppManager : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.gterm.bmp")] private static byte[] gtermIconRaw;
        private static Canvas gtermIcon = Image.FromBitmap(gtermIconRaw, false);

        Button gtermButton;

        public AppManager()
        {
            Contents = new Canvas(400, 400);
            Contents.Clear(Color.White);
            X = 830;
            Y = 100;
            Title = "GoOS App Manager";
            Visible = true;
            Closable = true;

            gtermButton = new Button(this, 10, 10, 64, 80, "GTerm")
            {
                BackgroundColour = Color.White,
                TextColour = Color.Black,

                Image = gtermIcon
            };
            
            gtermButton.Clicked = OpenGTerm;

            gtermButton.Render();
        }

        private static void OpenGTerm()
        {
            WindowManager.AddWindow(new Apps.GTerm());
        }
    }
}
