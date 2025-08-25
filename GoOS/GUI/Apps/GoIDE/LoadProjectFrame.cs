using System;
using System.IO;
using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoIDE
{
    public class LoadProjectFrame : Window
    {
        Button LoadButton;
        Button CancelButton;

        Input ScriptLocation;

        public LoadProjectFrame() : base(0, 0, 400, 300, "Load project - GoIDE")
        {
            try
            {
                // Create the window.
                SetDock(WindowDock.Center);

                // Initialise the controls.
                LoadButton = new Button(this, Convert.ToUInt16(Contents.Width - 180), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Load") { Clicked = LoadButton_Click };
                CancelButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Cancel") { Clicked = CancelButton_Click };
                ScriptLocation = new Input(this, 100, 52, Convert.ToUInt16(Contents.Width - 110), 20, @"0:\");

                // Paint the window.
                Contents.Clear(Color.LightGray);
                Contents.DrawString(10, 10, "Load project", Font_2x, Color.White);
                Contents.DrawString(10, 52, "Location: ", Font_1x, Color.White);
                Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
                LoadButton.Render();
                CancelButton.Render();
                ScriptLocation.Render();
            }
            catch
            {
                Dialogue.Show("GoIDE", "Something went wrong.\nPlease try again.", null, errorIcon);
            }
        }

        void LoadButton_Click()
        {
            // Load the file.
            string name = ScriptLocation.Text.Substring(ScriptLocation.Text.LastIndexOf(@"\"));
            string location = ScriptLocation.Text.Trim();

            if (!File.Exists(location))
            {
                Dialogue.Show("GoIDE", "File doesn't exist.", null, Resources.errorIcon);
                return;
            }

            WindowManager.AddWindow(new IDEFrame(name.Remove(name.LastIndexOf(".")).Substring(1), location, name.EndsWith(".9xc")));
            Dispose();
        }

        void CancelButton_Click()
        {
            WindowManager.AddWindow(new ProjectsFrame());
            Dispose();
        }
    }
}
