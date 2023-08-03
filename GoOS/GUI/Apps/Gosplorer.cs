using System;
using System.IO;
using System.Linq;
using Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using GoOS.Commands;

namespace GoOS.GUI.Apps;

public class Gosplorer : Window
{
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.BIN.bmp")]
    private static byte[] BinIconRaw;
    
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.CHILD.bmp")]
    private static byte[] ChildIconRaw;
    
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.PARENT.bmp")]
    private static byte[] ParentIconRaw;
    
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.MOVE.bmp")]
    private static byte[] MoveIconRaw;
    
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.COPY.bmp")]
    private static byte[] copyIconRaw;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Notepad.PASTE.bmp")]
    private static byte[] pasteIconRaw;
    
    private static Canvas binIcon = Image.FromBitmap(BinIconRaw, false);
    private static Canvas childIcon = Image.FromBitmap(ChildIconRaw, false);
    private static Canvas parentIcon = Image.FromBitmap(ParentIconRaw, false);
    private static Canvas moveIcon = Image.FromBitmap(MoveIconRaw, false);
    private static Canvas copyIcon = Image.FromBitmap(copyIconRaw, false);
    private static Canvas pasteIcon = Image.FromBitmap(pasteIconRaw, false);
    
    List DAF;
    Button Parent;
    Button Child;
    Button Move;
    Button Bin;
    
    // TODO: Implement copy & paste.
    Button Copy;
    Button Paste;
    
    private string cdir5000 = @"0:\";

    public Gosplorer()
    {
        Contents = new Canvas(500, 310);
        Title = "Gosplorer";
        Visible = true;
        Closable = true;
        Unkillable = false;
        SetDock(WindowDock.Auto);
        
        Bin = new Button(this, 5, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = binIcon,
            Clicked = BinClick
        };
        
        Parent = new Button(this, 25, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = parentIcon,
            Clicked = ParentClick
        };
        
        Child = new Button(this, 45, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = childIcon,
            Clicked = ChildClick
        };
        
        Move = new Button(this, 65, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = moveIcon,
            Clicked = MoveClick
        };
        
        DAF = new List(this, 5, 25, Convert.ToUInt16(Contents.Width - 10), Convert.ToUInt16(Contents.Height - 60),
            "Directory Listing", Array.Empty<string>());

        // Render the buttons.
        Contents.Clear(Color.LightGray); // >:^(
        RenderSystemStyleBorder();
        
        Bin.Render();
        Parent.Render();
        Child.Render();
        Move.Render();
        
        Update();
    }

    private void Update()
    {
        string cdir3003 = @"0:\";
        if (cdir5000.Contains(@"0:\\"))
        {
            cdir3003 = cdir5000.Replace(@"0:\\", @"0:\");
        }

        var directory_list = Directory.GetFiles(cdir3003);
        var directory2_list = Directory.GetDirectories(cdir3003);

        int fullLength = directory_list.Length + directory2_list.Length;

        DAF.Items = new string[fullLength];
        
        for (int i = 0; i < directory_list.Length; i++)
            DAF.Items[i] = directory_list[i];

        for (int i = 0; i < directory2_list.Length; i++)
            DAF.Items[i + directory_list.Length] = directory2_list[i];
        
        DAF.Render();
    }
    
    private void BinClick()
    {
        Delete.UniveralDelete(DAF.Items[DAF.Selected]);
        Update();
    }
    
    private void ParentClick()
    {
        string cdir3003 = @"0:\";
        if (cdir5000.Contains(@"0:\\"))
        {
            cdir3003 = cdir5000.Replace(@"0:\\", @"0:\");
        }

        if (cdir3003 != @"0:\")
        {
            string parentofcdir3003 = cdir3003.Substring(cdir3003.LastIndexOf(@"\"));

            cdir5000 = parentofcdir3003;
            
            Update();
        }
    }
    
    private void ChildClick()
    {
        string cdir3003;
        if (cdir5000.Contains(@"0:\\"))
        {
            cdir3003 = cdir5000.Replace(@"0:\\", @"0:\");
        }
        else
        {
            cdir3003 = cdir5000;
        }
        
        cdir5000 = cdir3003 + @"\" + DAF.Items[DAF.Selected];
        
        Update();
    }
    
    private void MoveClick()
    {
        
    }
}