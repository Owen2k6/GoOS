using System;
using System.IO;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI.Apps.GoIDE
{
    public class LoadProjectFrame : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.Font_2x.btf")] static byte[] font2xRaw;
        static Font font2x = new Font(font2xRaw, 32);

        Button LoadButton;
        Button CancelButton;

        Input ScriptLocation;

        public LoadProjectFrame()
        {
            try
            {
                // Create the window.
                Contents = new Canvas(400, 300);
                Title = "Load Project - GoIDE";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialize the controls.
                LoadButton = new Button(this, Convert.ToUInt16(Contents.Width - 180), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Load") { Clicked = LoadButton_Click };
                CancelButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Cancel") { Clicked = CancelButton_Click };
                ScriptLocation = new Input(this, 100, 52, Convert.ToUInt16(Contents.Width - 110), 20, @"0:\");

                // Paint the window.
                Contents.Clear(Color.LightGray);
                RenderSystemStyleBorder();
                Contents.DrawString(10, 10, "Load Project", font2x, Color.White);
                Contents.DrawString(10, 52, "Location: ", BetterConsole.font, Color.White);
                Contents.DrawString(Contents.Width - 138, Contents.Height - 66, "(c) 2023 Owen2k6", BetterConsole.font, Color.Black);
                Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
                LoadButton.Render();
                CancelButton.Render();
                ScriptLocation.Render();
            }
            catch
            {
                Dialogue.Show("GoIDE", "Something went wrong.\nPlease try again.", null, WindowManager.errorIcon);
            }
        }

        void LoadButton_Click()
        {
            // Load the file.
            string name = ScriptLocation.Text.Substring(ScriptLocation.Text.LastIndexOf(@"\"));
            string location = ScriptLocation.Text;

            if (location.Trim() == string.Empty)
            {
                Dialogue.Show("GoIDE", "File doesn't exist.", null, WindowManager.errorIcon);
                return;
            }

            WindowManager.AddWindow(new IDEFrame(name.Remove(name.LastIndexOf(".")).Substring(1), location, name.EndsWith(".9xc")));
            Dispose();
        }

        void CancelButton_Click()
        {
            Dispose();
        }
    }
}
