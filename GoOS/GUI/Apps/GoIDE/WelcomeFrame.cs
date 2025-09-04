using System;
using System.IO;
using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoIDE;

public class WelcomeFrame : Window
{
    private readonly Button CancelButton;
    private readonly Button NextButton;

    public WelcomeFrame()
    {
        // Create the window.
        AutoCreate(WindowDock.Center, 400, 300, "Welcome - GoIDE");

        // Initialize the controls.
        CancelButton =
            new Button(this, Convert.ToUInt16(Contents.Width - 132), Convert.ToUInt16(Contents.Height - 30), 64, 20,
                "Cancel") { Clicked = CancelButton_Click };
        NextButton =
            new Button(this, Convert.ToUInt16(Contents.Width - 58), Convert.ToUInt16(Contents.Height - 30), 48, 20,
                "Next") { Clicked = NextButton_Click };

        // Paint the window.
        Contents.Clear(Color.White);
        RenderSystemStyleBorder();
        Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4),
            38, 0, new Color(0xFFCCCCCC));
        Contents.DrawString(10, 10, "Welcome", Font_2x, Color.Black);
        Contents.DrawString(10, 52,
            "Welcome to GoIDE! This program will let you\ncreate and debug GoOS applications.\n\nGoIDE currently supports GoCode and 9xCode.\n\nPress next to install GoIDE and create a new\nproject.",
            Font_1x, Color.Black);
        CancelButton.Render();
        NextButton.Render();
    }

    private void NextButton_Click()
    {
        Directory.CreateDirectory(@"0:\content\prf\GoIDE");
        Directory.CreateDirectory(@"0:\content\prf\GoIDE\Projects");
        Directory.CreateDirectory(@"0:\content\prf\GoIDE\SaveData");

        Dispose();
        WindowManager.AddWindow(new NewProjectFrame());
    }

    private void CancelButton_Click()
    {
        Dispose();
        Dialogue.Show("GoIDE", "You have canceled GoIDE setup.", default, WindowManager.errorIcon);
    }
}