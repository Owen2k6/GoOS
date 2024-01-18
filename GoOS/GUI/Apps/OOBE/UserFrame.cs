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
            Title = "You and your Computer - GoOS Setup";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            UserName = new Input(this, 20, 190, 250, 20, "Enter a Username (e.g User)");
            ComputerName = new Input(this, 20, 216, 250, 20, "Name your computer (e.g MyPC)");
            NextButton = new Button(this, 20, 284, 100, 20, "Continue") { Clicked = NextButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEblank, false);
            Contents.DrawString(20, 10, "You and your Computer", Font_2x, Color.White);
            Contents.DrawString(20, 50,
                "Please make a username and name your computer. \nIt can be anything you want! We won't ever know or judge :) \nSpaces will be replaced with underscores (_) \nas to prevent Application compatability issues.\n\nYou will appear as user@computer in the terminal.",
                Font_1x, Color.White);
            Contents.DrawString(20, 246,
                "Please keep in mind that this can not be changed later. \nWe plan on adding this feature in the future.",
                Font_1x, Color.White);
            UserName.Render();
            ComputerName.Render();
            NextButton.Render();
        }

        private void NextButton_Click()
        {
            // Check if the username is valid.
            if (UserName.Text == "")
            {
                Dialogue.Show("Error", "Your username can't be empty.");
                return;
            }

            if (ComputerName.Text == "")
            {
                Dialogue.Show("Error", "Your computer's name can't be empty.");
                return;
            }

            if (UserName.Text.Contains(' '))
            {
                Dialogue.Show("Notice", "Spaces in your username have been replaced with underscores (_)");
            }

            if (ComputerName.Text.Contains(' '))
            {
                Dialogue.Show("Notice", "Spaces in your computer's name have been replaced with underscores (_)");
            }


            string username = UserName.Text = UserName.Text.Replace(" ", "_");
            string computername = ComputerName.Text = ComputerName.Text.Replace(" ", "_");
            // Continue.
            WindowManager.AddWindow(new ActivationFrame(username, computername));
            Dispose();
        }
    }
}