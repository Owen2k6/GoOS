using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class MainFrame : Window
    {
        Button NextButton;

        public MainFrame()
        {
            // Create the window.
            Contents = new Canvas(800, 600);
            Title = "Welcome to GoOS";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            NextButton = new Button(this, 350, 456, 100, 20, "Next") { Clicked = NextButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEmain, false);
            NextButton.Render();
        }

        private void NextButton_Click()
        {
            // Continue.
            WindowManager.AddWindow(new UserFrame());
            Dispose();
        }
    }
}
