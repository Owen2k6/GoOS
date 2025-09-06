using System;
using System.IO;
using Gold.Graphics;

namespace GoOS.GUI.Apps.GoIDE;

public class ImportProjectFrame : Window
{
    private readonly Button CancelButton;
    private readonly Button ImportButton;

    private readonly Input ScriptLocation;

    public ImportProjectFrame()
    {
        try
        {
            // Create the window.
            Contents = new Canvas(400, 300);
            Title = "Import project - GoIDE";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            ImportButton =
                new Button(this, Convert.ToUInt16(Contents.Width - 180), Convert.ToUInt16(Contents.Height - 30), 80, 20,
                    "Import") { Clicked = ImportButton_Click };
            CancelButton =
                new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20,
                    "Cancel") { Clicked = CancelButton_Click };
            ScriptLocation = new Input(this, 100, 52, Convert.ToUInt16(Contents.Width - 110), 20, @"0:\");

            // Paint the window.
            Contents.Clear(Color.White);
            RenderSystemStyleBorder();
            Contents.DrawString(10, 10, "Import project", Resources.Font_2x, Color.Black);
            Contents.DrawString(10, 52, "Location: ", Resources.Font_1x, Color.Black);
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40),
                Convert.ToUInt16(Contents.Width - 4), 38, 0, new Color(0xFFCCCCCC));
            ImportButton.Render();
            CancelButton.Render();
            ScriptLocation.Render();
        }
        catch
        {
            Dialogue.Show("GoIDE", "Something went wrong.\nPlease try again.", null, WindowManager.errorIcon);
        }
    }

    private void ImportButton_Click()
    {
        // Import the file.
        var name = ScriptLocation.Text.Substring(ScriptLocation.Text.LastIndexOf(@"\"));
        var location = ScriptLocation.Text.Trim();

        if (!File.Exists(location))
        {
            Dialogue.Show("GoIDE", "File doesn't exist.", null, WindowManager.errorIcon);
            return;
        }

        File.WriteAllBytes(@"0:\content\prf\GoIDE\Projects\" + name, File.ReadAllBytes(location));

        WindowManager.AddWindow(new ProjectsFrame());
        Dispose();
    }

    private void CancelButton_Click()
    {
        WindowManager.AddWindow(new ProjectsFrame());
        Dispose();
    }
}