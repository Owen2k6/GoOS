using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using PrismAPI.UI;

namespace GoOS.GUI.Apps
{
    public class About : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.aboutGoOS.bmp")]
        private static byte[] aboutbgRAW;

        private static Canvas abtbg = Image.FromBitmap(aboutbgRAW, false);

        public About()
        {
            // Generate the fonts.
            Fonts.Generate();

            // Create the window.
            Contents = new Canvas(200, 180);
            Title = "About this GoPC";
            Visible = true;
            Closable = true;
            Sizable = false;
            SetDock(WindowDock.Auto);
            // Paint the window.
            Contents.DrawImage(0, 0, abtbg, false);
            Contents.DrawString(50, 150, "(Version " + Kernel.version + ")", Fonts.Font_1x, Color.White);
        }
    }
}