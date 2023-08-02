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


    public Notepad()
    {
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

        Contents.Clear(Color.White);
        RenderSystemStyleBorder();

        SaveButton.Render();
        CopyButton.Render();
        PasteButton.Render();
    }

    private void SaveClick()
    {
        string shittosave = AttemptOne.Text;
        Kernel.Notepadtextsavething = shittosave;
        
        string punchyouintheface = HashPasswordSha256(Kernel.Notepadtextsavething);
        
        Dialogue.Show(
            "Error",
            punchyouintheface,
            null, // default buttons
            WindowManager.errorIcon);
        
        /* BUILD YOU DAMNED COMPILER FROM HELL! */
        
        if (punchyouintheface.Contains("YwB49Aqwtew1XeHJk0+rrEDtRq7Y6KfF5zGV9sKNe6w="))
        {
            WindowManager.AddWindow(new Cut());
        }
        else if (punchyouintheface.Contains("P0LqY7Fbr9GbGncLcBkxKhGheiV72MIE5oTrAQSnaSI="))
        {
            WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());WindowManager.AddWindow(new Cut());
        }
        
        
        WindowManager.AddWindow(new NotepadSaveAs());
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
    private Input text;

    public NotepadSaveAs()
    {
        Contents = new Canvas(300, 100);
        Title = "Save - GoOS Notepad";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        SaveButton = new Button(this, 5, 45, 60, 20, "Save")
        {
            Clicked = SaveClick
        };

        CancelButton = new Button(this, 300 - 60, 45, 60, 20, "Cancel")
        {
            Clicked = Dispose
        };

        text = new Input(this, 5, 5, 300 - 10, 20, "Input the filename below:")
        {
            ReadOnly = true
        };

        AttemptOne = new Input(this, 5, 25, 300 - 10, 20, "")
        {
        };

        Contents.Clear(Color.White);
        RenderSystemStyleBorder();

        SaveButton.Render();
        CancelButton.Render();
        text.Render();
        AttemptOne.Render();
    }

    private void SaveClick()
    {
        
        try
        {
            Commands.Make.MakeFile(Kernel.NotepadFileToSaveNameThing);
            File.WriteAllText(Kernel.NotepadFileToSaveNameThing, Kernel.Notepadtextsavething);
            Dialogue.Show(
                "Saved!",
                "Your file has been saved.",
                null);
        }
        catch (Exception e)
        {
            Dialogue.Show(
                "Error",
                "Your file has not been saved.",
                null, // default buttons
                WindowManager.errorIcon);
        }
    }
}

// TODO: implement the overwrite confirm-er. I didn't because apparently C# doesnt like the idea of launching a window from a window from a window.
public class NotepadOverwriteConfirm : Window
{
    private Button YesButton;
    private Button NoButton;
    private Input text;

    public NotepadOverwriteConfirm()
    {
        Contents = new Canvas(300, 100);
        Title = "Overwrite - GoOS Notepad";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        YesButton = new Button(this, 5, 15, 60, 20, "Yes")
        {
            Clicked = YesClick
        };

        NoButton = new Button(this, 200 - 60, 15, 60, 20, "No")
        {
            Clicked = Dispose
        };

        text = new Input(this, 5, 5, 200 - 10, 20, "Overwrite the existing file?")
        {
            ReadOnly = true
        };

        Contents.Clear(Color.White);
        RenderSystemStyleBorder();

        YesButton.Render();
        NoButton.Render();
        text.Render();
    }

    private void YesClick()
    {
        try
        {
            Commands.Make.MakeFile(Kernel.NotepadFileToSaveNameThing);
            File.WriteAllText(Kernel.NotepadFileToSaveNameThing, Kernel.Notepadtextsavething);
            Dialogue.Show(
                "Saved!",
                "Your file has been saved.",
                null, // default buttons
                WindowManager.errorIcon);
        }
        catch (Exception e)
        {
            Dialogue.Show(
                "Error",
                "Your file has not been saved.",
                null, // default buttons
                WindowManager.errorIcon);
        }
    }
}



