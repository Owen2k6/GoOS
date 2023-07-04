using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI
{
    public class LoadingDialogue : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.drum.bmp")] private static byte[] drumIconRaw;
        private static Canvas drumIcon = Image.FromBitmap(drumIconRaw, false);

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

            Contents.DrawString(80, 20, message, BetterConsole.font, Color.Black);
        }
    }
}
