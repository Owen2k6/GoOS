using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class DoneFrame : Window
    {
        Button RestartButton;

        public DoneFrame() : base(0, 0, 800, 600, "Finished - GoOS Setup")
        {
            // Create the window.
            SetDock(WindowDock.Center);

            // Initialize the controls.
            RestartButton = new Button(this, 20, 570, 100, 20, "Restart") { Clicked = RestartButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEblank, false);
            Contents.DrawString(20, 5, "Welcome to GoOS.", Font_2x, Color.White);
            Contents.DrawString(20, 40, "We just need to restart the system in order to apply settings and load the OS fully!\nOnce you've restarted, You will be welcomed to the GoOS Desktop!", Font_1x, Color.White);
            
            base.Render();
        }

        private void RestartButton_Click()
        {
            Cosmos.System.Power.Reboot();
        }
    }
}
