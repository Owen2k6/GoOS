using System;
using System.IO;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.GoIDE
{
    public class NewProjectFrame : Window
    {
        Button CreateButton;
        Button CancelButton;
        Button GoCodeButton;
        Button _9xCodeButton;

        Input ScriptName;
        Input ScriptLocation;

        public NewProjectFrame()
        {
            try
            {
                // Generate the fonts.
                Fonts.Generate();

                // Create the window.
                Contents = new Canvas(400, 300);
                Title = "New project - GoIDE";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize the controls.
                CreateButton = new Button(this, Convert.ToUInt16(Contents.Width - 180), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Create") { Clicked = CreateButton_Click };
                CancelButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Cancel") { Clicked = CancelButton_Click };
                GoCodeButton = new Button(this, 100, 112, 80, 20, "GoCode") { Clicked = GoCodeButton_Click, AppearPressed = true };
                _9xCodeButton = new Button(this, 190, 112, 80, 20, "9xCode") { Clicked = _9xCodeButton_Click, AppearPressed = false };
                ScriptName = new Input(this, 100, 52, Convert.ToUInt16(Contents.Width - 110), 20, "Project1");
                ScriptLocation = new Input(this, 100, 82, Convert.ToUInt16(Contents.Width - 110), 20, @"0:\content\prf\GoIDE\Projects");

                // Paint the window.
                Contents.Clear(Color.LightGray);
                RenderSystemStyleBorder();
                Contents.DrawString(10, 10, "New project", Fonts.Font_2x, Color.White);
                Contents.DrawString(10, 52, "Name: ", Fonts.Font_1x, Color.White);
                Contents.DrawString(10, 82, "Location: ", Fonts.Font_1x, Color.White);
                Contents.DrawString(10, 112, "Language: ", Fonts.Font_1x, Color.White);
                Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
                CreateButton.Render();
                CancelButton.Render();
                ScriptName.Render();
                ScriptLocation.Render();
                GoCodeButton.Render();
                _9xCodeButton.Render();
            }
            catch
            {
                Dialogue.Show("GoIDE", "Something went wrong.\nPlease try again.", null, WindowManager.errorIcon);
            }
        }

        void CreateButton_Click()
        {
            // Create the file
            string name = ScriptName.Text;
            string location = ScriptLocation.Text;

            if (name.Trim() == string.Empty)
                name = "Project1";

            if (location.Trim() == string.Empty)
                location = @"0:\content\prf\GoIDE\Projects";

            if (!location.EndsWith(@"\"))
                location += @"\";

            if (!Directory.Exists(location))
            {
                Dialogue.Show("GoIDE", "Invalid path.", null, WindowManager.errorIcon);
                return;
            }

            if (File.Exists(location + name + (GoCodeButton.AppearPressed ? ".gexe" : ".9xc")))
            {
                Dialogue.Show("GoIDE", "File already exists.", null, WindowManager.errorIcon);
                return;
            }

            File.Create(location + name + (GoCodeButton.AppearPressed ? ".gexe" : ".9xc"));

            WindowManager.AddWindow(new IDEFrame(name, location + name + (GoCodeButton.AppearPressed ? ".gexe" : ".9xc"), _9xCodeButton.AppearPressed ? true : false));
            Dispose();
        }

        void CancelButton_Click()
        {
            WindowManager.AddWindow(new ProjectsFrame());
            Dispose();
        }

        void GoCodeButton_Click()
        {
            // Toggle the GoCode and 9xCode buttons
            GoCodeButton.AppearPressed = true;
            _9xCodeButton.AppearPressed = false;
            GoCodeButton.Render();
            _9xCodeButton.Render();
        }

        void _9xCodeButton_Click()
        {
            // Toggle the GoCode and 9xCode buttons
            GoCodeButton.AppearPressed = false;
            _9xCodeButton.AppearPressed = true;
            GoCodeButton.Render();
            _9xCodeButton.Render();
        }
    }
}
