using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class LoadingDialogue : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.drum.bmp")] private static byte[] drumIconRaw;
        private static Canvas drumIcon = Image.FromBitmap(drumIconRaw, false);

        public LoadingDialogue()
        {
            Contents = new Canvas(320, 128);
            Contents.Clear(new Color(191, 191, 191));
            X = 240;
            Y = 236;
            Title = "GoOS";
            Visible = true;
            Closable = true;

            Contents.DrawImage(20, 20, drumIcon, true);

            Contents.DrawString(80, 20, "GoOS is starting\nPlease wait...", BetterConsole.font, Color.Black);
        }
    }
}
