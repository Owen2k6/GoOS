using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoOS.GUI;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class BrownGhost : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.brown_ghost.bmp")] static byte[] brownGhostRaw;
        static Canvas brownGhost = Image.FromBitmap(brownGhostRaw, false);

        public BrownGhost()
        {
            Contents = new Canvas(238, 150);
            Title = "Boooo";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            Fonts.Generate();

            Contents.Clear(Color.Black);
            RenderSystemStyleBorder();
            Contents.DrawImage(10, 51, brownGhost);
            Contents.DrawString(61, 67, "Aaaa! A brown ghost!", Fonts.Font_1x, Color.White);
        }
    }
}
