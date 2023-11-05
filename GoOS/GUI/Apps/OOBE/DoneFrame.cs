using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class DoneFrame : Window
    {
        Button RestartButton;

        public DoneFrame()
        {
            // Create the window.
            Contents = new Canvas(800, 600);
            Title = "Finished - GoOS Setup";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            RestartButton = new Button(this, 350, 456, 100, 20, "Restart") { Clicked = RestartButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEblank, false);
            RestartButton.Render();
        }

        private void RestartButton_Click()
        {
            Cosmos.System.Power.Reboot();
        }
    }
}
