using System.IO;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class UserFrame : Window
    {
        Input UserName;
        Input ComputerName;
        Button NextButton;

        public UserFrame()
        {
            // Create the window.
            Contents = new Canvas(800, 600);
            Title = "User settings - GoOS Setup";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            UserName = new Input(this, 420, 400, 100, 20, "user");
            ComputerName = new Input(this, 420, 426, 100, 20, "GoOS");
            NextButton = new Button(this, 350, 456, 100, 20, "Next") { Clicked = NextButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEblank, false);
            Contents.DrawString(300, 400, "Username: ", Font_1x, Color.White);
            Contents.DrawString(300, 426, "Computer name: ", Font_1x, Color.White);
            UserName.Render();
            ComputerName.Render();
            NextButton.Render();
        }

        private void NextButton_Click()
        {
            // Continue.
            WindowManager.AddWindow(new ActivationFrame(UserName.Text, ComputerName.Text));
            Dispose();
        }
    }
}
