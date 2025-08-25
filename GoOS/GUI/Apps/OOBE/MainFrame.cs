using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class MainFrame : Window
    {
        Button NextButton;

        public MainFrame() : base(0, 0, 800, 600, "Welcome to GoOS!")
        {
            // Create the window.
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
            WindowManager.AddWindow(new TermsFrame());
            Dispose();
        }
    }
}
