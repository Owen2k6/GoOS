using System;
using System.IO;
using System.Text;
using Gold.Graphics;
using GoOS.Commands;
using GoOS.Security;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Notepad : Window
{
    private readonly InputNUMBERS AttemptOne;
    private readonly Button CopyButton;

    private readonly string infi = "";
    private readonly Button PasteButton;
    private readonly Button SaveButton;
    private Input Dialog_TextBox;
    private bool gms;


    public Notepad(bool openFile, string fileToOpen)
    {
        var infi = fileToOpen;


        Contents = new Canvas(500, 300);
        Title = "GoOS Notepad";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        AttemptOne = new InputNUMBERS(this, 5, 25, 500 - 10, 300 - 30, "")
        {
            MultiLine = true
        };

        SaveButton = new Button(this, 5, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = saveIcon,
            Clicked = SaveClick
        };

        CopyButton = new Button(this, 25, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = copyIcon,
            Clicked = CopyClick
        };

        PasteButton = new Button(this, 45, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = pasteIcon,
            Clicked = PasteClick
        };

        Contents.Clear(Color.LightGray);
        RenderSystemStyleBorder();

        SaveButton.Render();
        CopyButton.Render();
        PasteButton.Render();
        //AttemptOne.Render();

        if (openFile)
            LoadFile(fileToOpen);
        else
            AttemptOne.Render();
    }

    private void LoadFile(string filefile)
    {
        var infi = filefile;

        if (infi.ToLower().Contains(".gms") && !Kernel.devMode)
        {
            gms = true;

            var toreturn = "You cannot open .gms files in notepad.";

            AttemptOne.Text = toreturn;

            AttemptOne.Render();
        }
        else
        {
            var toreturn = "";
            var lines = File.ReadAllLines(filefile);
            foreach (var line in lines) toreturn = toreturn + line + "\n";

            AttemptOne.Text = toreturn;

            AttemptOne.Render();
        }
    }

    private void SaveClick()
    {
        var shittosave = AttemptOne.Text;

        var punchyouintheface = HashPasswordSha256(shittosave);

        /* BUILD YOU DAMNED COMPILER FROM HELL! */

        if (punchyouintheface.Contains("YwB49Aqwtew1XeHJk0+rrEDtRq7Y6kfF5zGV9sKNe6w="))
        {
            WindowManager.AddWindow(new Cut());
        }
        else if (punchyouintheface.Contains("P0LqY7Fbr9GbGncLcBkxKhGheiV72MIE5oTrAQSnaSI="))
        {
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
            WindowManager.AddWindow(new Cut());
        }

        if (!gms) WindowManager.AddWindow(new NotepadSaveAs(infi, shittosave));
    }

    internal static string HashPasswordSha256(string hellomario)
    {
        var sha256 = new Sha256();

        var passwordBytesUnhashed = Encoding.Unicode.GetBytes(hellomario);
        sha256.AddData(passwordBytesUnhashed, 0, (uint)passwordBytesUnhashed.Length);

        return Convert.ToBase64String(sha256.GetHash());
    }

    private void CopyClick()
    {
    }

    private void PasteClick()
    {
    }
}

public class NotepadSaveAs : Window
{
    private readonly Input AttemptOne;
    private readonly Button CancelButton;
    private readonly Button SaveButton;

    private readonly string sts = "";

    public NotepadSaveAs(string placetheholderofthetextplease, string stufftosave)
    {
        sts = stufftosave;

        Contents = new Canvas(300, 80);
        Title = "Save - GoOS Notepad";
        Visible = true;
        Closable = false;
        SetDock(WindowDock.Auto);

        SaveButton = new Button(this, 5, 50, 60, 20, "Save")
        {
            Clicked = SaveClick
        };

        CancelButton = new Button(this, 300 - 65, 50, 60, 20, "Cancel")
        {
            Clicked = Dispose
        };

        AttemptOne = new Input(this, 5, 25, 300 - 10, 20, "");

        Contents.Clear(Color.LightGray);
        RenderSystemStyleBorder();

        Contents.DrawString(5, 5, "Please input file name below:", Font_1x, Color.White);

        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;

        SaveButton.Render();
        CancelButton.Render();
        AttemptOne.Render();

        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
        AttemptOne.Text = placetheholderofthetextplease;
    }

    private void SaveClick()
    {
        try
        {
            Make.MakeFile(AttemptOne.Text);
            File.WriteAllText(AttemptOne.Text, sts);
            Dialogue.Show(
                "Saved!",
                "Your file has been saved.");

            Dispose();
        }
        catch (Exception e)
        {
            Dialogue.Show(
                "Error",
                e.Message,
                null, // default buttons
                WindowManager.errorIcon);
        }
    }
}