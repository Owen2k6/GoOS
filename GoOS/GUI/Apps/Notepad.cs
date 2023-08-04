using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using System.IO;
using System.Net.Security;
using System.Threading;
using GoOS.Security;

namespace GoOS.GUI.Apps;

public class Notepad : Window
{
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.SAVE.bmp")]
    private static byte[] saveIconRaw;

    private static Canvas saveIcon = Image.FromBitmap(saveIconRaw, false);

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.COPY.bmp")]
    private static byte[] copyIconRaw;

    private static Canvas copyIcon = Image.FromBitmap(copyIconRaw, false);

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.PASTE.bmp")]
    private static byte[] pasteIconRaw;

    private static Canvas pasteIcon = Image.FromBitmap(pasteIconRaw, false);

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.question.bmp")]
    private static byte[] questionRaw;


    private static Canvas question = Image.FromBitmap(questionRaw, false);

    private Button SaveButton;
    private Button CopyButton;
    private Button PasteButton;
    private Input AttemptOne;
    private Input Dialog_TextBox;

    private string infi = "";


    public Notepad(bool openFile, string fileToOpen)
    {
        string infi = fileToOpen;

        Contents = new Canvas(500, 300);
        Title = "GoOS Notepad";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        AttemptOne = new Input(this, 5, 25, 500 - 10, 300 - 30, "")
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
        {
            LoadFile(fileToOpen);
        }
        else
        {
            AttemptOne.Render();
        }
    }

    private void LoadFile(string filefile)
    {
        string infi = filefile;    
        
        string toreturn = "";
        string[] lines = File.ReadAllLines(filefile);
        foreach (var line in lines)
        {
            toreturn = toreturn + line + "\n";
        }

        AttemptOne.Text = toreturn;

        AttemptOne.Render();
    }

    private void SaveClick()
    {
        string shittosave = AttemptOne.Text;

        string punchyouintheface = HashPasswordSha256(shittosave);

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


        WindowManager.AddWindow(new NotepadSaveAs(infi, shittosave));
    }

    internal static string HashPasswordSha256(string hellomario)
    {
        Sha256 sha256 = new Sha256();

        byte[] passwordBytesUnhashed = Encoding.Unicode.GetBytes(hellomario);
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
    private Button SaveButton;
    private Button CancelButton;
    private Input AttemptOne;

    private string sts = "";
    
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

        AttemptOne = new Input(this, 5, 25, 300 - 10, 20, "")
        {
        };

        Contents.Clear(Color.LightGray);
        RenderSystemStyleBorder();

        Contents.DrawString(5, 5, "Please input file name below:", BetterConsole.font, Color.White);
        
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
            Commands.Make.MakeFile(AttemptOne.Text);
            File.WriteAllText(AttemptOne.Text, sts);
            Dialogue.Show(
                "Saved!",
                "Your file has been saved.",
                null);

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