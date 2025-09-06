using System;
using System.Collections.Generic;
using System.IO;
using Cosmos.System;
using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Notepad : Window
{
    private Input Textbox;
    private Input Dialog_TextBox;
    private string Path = null;

    public Notepad(string file = null)
    {
        Contents = new Canvas(300, 400);
        Title = "Notepad";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        Textbox = new Input(this, 0, 0, Contents.Width, Contents.Height, "") { MultiLine = true };

        Contents.Clear(Color.White);
        RenderSystemStyleBorder();
        Textbox.Render();

        if (file != null)
        {
            Textbox.Text = File.ReadAllText(file);
            Path = file;
            Title = System.IO.Path.GetFileNameWithoutExtension(Path) + " - Notepad";
        }
    }

    public override void ShowContextMenu()
    {
        ContextMenu.Show(new[] { " Open", " Save", " Save as", "----", " About Notepad" }, 132, ContextMenu_Handle);
    }

    public override void HandleKey(KeyEvent key)
    {
        base.HandleKey(key);

        switch (key.Key)
        {
            case ConsoleKeyEx.F1:
                ShowAboutDialog("2.0");
                break;
        }
    }

    private void ContextMenu_Handle(string entry)
    {
        switch (entry)
        {
            case " Open":
                var openDialogue = new Dialogue(
                    "Open",
                    "Please input file path:   ",
                    new List<DialogueButton>
                    {
                        new()
                        {
                            Text = "OK",
                            Callback = Open_Handler
                        },
                        new()
                        {
                            Text = "Cancel"
                        }
                    },
                    question);

                Dialog_TextBox = new Input(openDialogue, 80, 52, 195, 20, "");

                WindowManager.AddWindow(openDialogue);
                break;

            case " Save":
                if (Path == null)
                {
                    goto case " Save as";
                }
                File.WriteAllText(Path, Textbox.Text);
                Dialogue.Show("Notepad", "File saved.");
                break;

            case " Save as":
                var saveDialogue = new Dialogue(
                    "Save as",
                    "Please input file path:   ",
                    new List<DialogueButton>
                    {
                        new()
                        {
                            Text = "OK",
                            Callback = SaveAs_Handler
                        },
                        new()
                        {
                            Text = "Cancel"
                        }
                    },
                    question);

                Dialog_TextBox = new Input(saveDialogue, 80, 52, 195, 20, "");

                WindowManager.AddWindow(saveDialogue);
                break;

            case " About Notepad":
                ShowAboutDialog("2.0");
                break;
        }
    }

    private void Open_Handler()
    {
        if (!File.Exists(Dialog_TextBox.Text))
        {
            Dialogue.Show("Notepad", "File not found", null, errorIcon);
            return;
        }
        Path = Dialog_TextBox.Text;
        Textbox.Text = File.ReadAllText(Path);
        Title = System.IO.Path.GetFileNameWithoutExtension(Path) + " - Notepad";
    }

    private void SaveAs_Handler()
    {
        Path = Dialog_TextBox.Text;
        File.WriteAllText(Path, Textbox.Text);

        Title = System.IO.Path.GetFileNameWithoutExtension(Path) + " - Notepad";
    }
}