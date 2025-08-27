using System;
using System.IO;
using Gold.Graphics;
using static GoOS.Resources;

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

        public NewProjectFrame() : base(0, 0, 400, 300, "New project - GoIDE")
        {
            try
            {
                // Create the window.
                SetDock(WindowDock.Center);

                // Initialize the controls.
                CreateButton = new Button(this, Convert.ToUInt16(Contents.Width - 180), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Create") { Clicked = CreateButton_Click };
                CancelButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Cancel") { Clicked = CancelButton_Click };
                GoCodeButton = new Button(this, 100, 112, 80, 20, "GoCode") { Clicked = GoCodeButton_Click, Pressed = true };
                _9xCodeButton = new Button(this, 190, 112, 80, 20, "9xCode") { Clicked = _9xCodeButton_Click, Pressed = false };
                ScriptName = new Input(this, 100, 52, Convert.ToUInt16(Contents.Width - 110), 20, "Project1");
                ScriptLocation = new Input(this, 100, 82, Convert.ToUInt16(Contents.Width - 110), 20, @"0:\content\prf\GoIDE\Projects");

                // Paint the window.
                Contents.Clear(Color.LightGray);
                //RenderSystemStyleBorder();
                Contents.DrawString(10, 10, "New project", Font_2x, Color.White);
                Contents.DrawString(10, 52, "Name: ", Font_1x, Color.White);
                Contents.DrawString(10, 82, "Location: ", Font_1x, Color.White);
                Contents.DrawString(10, 112, "Language: ", Font_1x, Color.White);
                Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
                
                base.Render();
            }
            catch
            {
                Dialogue.Show("GoIDE", "Something went wrong.\nPlease try again.", null, Resources.errorIcon);
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

            if (location.StartsWith(@"1:\")) {
                Dialogue.Show("GoIDE", "Cannot create projects on the CD drive.", null, errorIcon);
                return;
            }
            
            if (!Directory.Exists(location))
            {
                Dialogue.Show("GoIDE", "Invalid path.", null, errorIcon);
                return;
            }

            if (File.Exists(location + name + (GoCodeButton.Pressed ? ".gexe" : ".9xc")))
            {
                Dialogue.Show("GoIDE", "File already exists.", null, errorIcon);
                return;
            }

            File.Create(location + name + (GoCodeButton.Pressed ? ".gexe" : ".9xc"));

            Kernel.ProcessScheduler.AddProcess(new IDEFrame(name, location + name + (GoCodeButton.Pressed ? ".gexe" : ".9xc"), _9xCodeButton.Pressed));
            Dispose();
        }

        void CancelButton_Click()
        {
            Kernel.ProcessScheduler.AddProcess(new ProjectsFrame());
            Dispose();
        }

        void GoCodeButton_Click()
        {
            // Toggle the GoCode and 9xCode buttons
            GoCodeButton.Pressed = true;
            _9xCodeButton.Pressed = false;
            base.Render();
        }

        void _9xCodeButton_Click()
        {
            // Toggle the GoCode and 9xCode buttons
            GoCodeButton.Pressed = false;
            _9xCodeButton.Pressed = true;
            base.Render();
        }
    }
}
