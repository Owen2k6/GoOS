using System;
using System.IO;
using Gold.Graphics;
using GoOS._9xCode;
using static GoOS.Commands.Run;
using Console = BetterConsole;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoIDE;

public class IDEFrame : Window
{
    private readonly Input Code;
    private readonly bool Is9xCode;

    private readonly string ProjectPath;
    private readonly Button RunButton;
    private readonly Button SaveButton;

    private bool Debugging;

    public IDEFrame(string projectName, string projectPath, bool is9xCode)
    {
        try
        {
            // Store project name and project path to local variable.
            ProjectPath = projectPath;
            Is9xCode = is9xCode;

            // Create the window.
            Contents = new Canvas(800, 600);
            Title = projectName + " - GoIDE";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            // Initialize the controls.
            SaveButton = new Button(this, 2, 2, 48, 18, "Save")
                { Clicked = SaveButton_Click, UseSystemStyle = false, BackgroundColour = new Color(0xFFCCCCCC) };
            RunButton = new Button(this, Convert.ToUInt16(Contents.Width - 42), 2, 40, 18, "Run")
                { Clicked = RunButton_Click, UseSystemStyle = false, BackgroundColour = new Color(0xFFCCCCCC) };
            Code = new Input(this, 2, 20, Convert.ToUInt16(Contents.Width - 4),
                Convert.ToUInt16(Contents.Height - 43), string.Empty) { MultiLine = true, Numbers = true };
            Code.Text = File.ReadAllText(projectPath);

            // Paint the window.
            Paint("Loaded");
        }
        catch (Exception ex)
        {
            Dialogue.Show("GoIDE", "Something went wrong.\nPlease try again.\n\n" + ex, null, WindowManager.errorIcon);
        }
    }

    private void Paint(string status)
    {
        Contents.Clear(new Color(0xFFCCCCCC));
        RenderSystemStyleBorder();
        Contents.DrawImage(Contents.Width - 62, 0, RunImage);
        SaveButton.Render();
        RunButton.Render();
        Code.Render();
        Contents.DrawString(4, Contents.Height - 20, status, Font_1x, Color.LighterBlack);
    }

    private void SaveButton_Click()
    {
        File.WriteAllText(ProjectPath, Code.Text);
        Paint("Saved");
    }

    private void RunButton_Click()
    {
        if (!Debugging)
        {
            Debugging = true;

            File.WriteAllText(ProjectPath, Code.Text);

            Console.Clear();
            Console.Title = "Terminal - GoIDE";
            WindowManager.AddWindow(new GTerm(false));
            Console.Clear();

            if (!Is9xCode)
                Main(ProjectPath, false);
            else
                Interpreter.Run(ProjectPath);

            Console.Clear();
            WindowManager.RemoveWindowByTitle("Terminal - GoIDE");
            Console.Title = "GTerm";

            Debugging = false;
        }
    }
}