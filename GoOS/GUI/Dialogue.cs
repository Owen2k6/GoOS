using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class Dialogue : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.info.bmp")] private static byte[] infoIconRaw;
        private static Canvas infoIcon = Image.FromBitmap(infoIconRaw, false);

        private Button closeButton;

        /// <summary>
        /// Show a system dialogue.
        /// </summary>
        public static Dialogue Show(string title, string message, Canvas icon = null)
        {
            var dialogue = new Dialogue(title, message, icon);
            WindowManager.AddWindow(dialogue);
            return dialogue;
        }

        public Dialogue(string title, string message, Canvas icon = null)
        {
            if (icon == null)
            {
                // default icon
                icon = infoIcon;
            }

            Contents = new Canvas(320, 128);
            Contents.Clear(new Color(191, 191, 191));
            X = 480;
            Y = 296;
            Title = title;
            Visible = true;
            Closable = true;

            Contents.DrawImage(20, 20, icon, true);

            Contents.DrawString(80, 20, message, BetterConsole.font, Color.Black);

            closeButton = new Button(this, (ushort)(Contents.Width - 80 - 20), (ushort)(Contents.Height - 25 - 20), 80, 25, "OK");
            closeButton.Clicked = Close;

            closeButton.Render();
        }

        private void Close()
        {
            Closing = true;
        }
    }
}
