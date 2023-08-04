using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using GoOS.Commands;
using PrismAPI.Graphics.Fonts;

namespace GoOS.GUI.Apps;

public class Gosplorer : Window
{
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.NEW.bmp")]
    private static byte[] NewIconRaw;
    
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
    
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.REFRESH.bmp")]
    private static byte[] refIconRaw;
    
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.Gosplorer.LOADINNOTEPAD.bmp")]
    private static byte[] linIconRaw;
    
    private static Canvas newIcon = Image.FromBitmap(NewIconRaw, false);
    private static Canvas binIcon = Image.FromBitmap(BinIconRaw, false);
    private static Canvas childIcon = Image.FromBitmap(ChildIconRaw, false);
    private static Canvas parentIcon = Image.FromBitmap(ParentIconRaw, false);
    private static Canvas moveIcon = Image.FromBitmap(MoveIconRaw, false);
    private static Canvas copyIcon = Image.FromBitmap(copyIconRaw, false);
    private static Canvas pasteIcon = Image.FromBitmap(pasteIconRaw, false);
    private static Canvas refIcon = Image.FromBitmap(refIconRaw, false);
    private static Canvas linIcon = Image.FromBitmap(linIconRaw, false);

    List DAF;
    Button Parent;
    Button Child;
    Button Move;
    Button Bin;
    Button New;
    Button Ref;
    Button Lin;
    private Input cdirdis;

    // TODO: Implement copy & paste.
    Button Copy;
    Button Paste;

    private string cdir5000 = @"0:\";

    private string clickcounter = "";
    private int ccnum = 0;

    public Gosplorer()
    {
        BetterConsole.font = new Font(BetterConsole.rawFont, BetterConsole.charHeight);
        Contents = new Canvas(500, 310);
        Title = "Gosplorer";
        Visible = true;
        Closable = true;
        Unkillable = false;
        SetDock(WindowDock.Auto);
        
        New = new Button(this, 5, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = newIcon,
            Clicked = NewClick
        };
        
        Lin = new Button(this, 125, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image =  linIcon,
            Clicked = LinClick
        };
        
        Ref = new Button(this, 105, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = refIcon,
            Clicked = RefClick
        };
        
        Bin = new Button(this, 25, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = binIcon,
            Clicked = BinClick
        };

        Parent = new Button(this, 45, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = parentIcon,
            Clicked = ParentClick
        };

        Child = new Button(this, 65, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = childIcon,
            Clicked = ChildClick
        };

        Move = new Button(this, 85, 5, 15, 15, "")
        {
            //UseSystemStyle = false,
            BackgroundColour = Color.White,
            TextColour = Color.Black,

            Image = moveIcon,
            Clicked = MoveClick
        };
        
        // FFS build
        
        DAF = new List(this, 5, 25, Convert.ToUInt16(Contents.Width - 10), Convert.ToUInt16(310 - 50),"Directory Listing", Array.Empty<string>())
        {
            Clicked = ListClick
        };

        // Render the buttons.
        Contents.Clear(Color.LightGray); // >:^(
        RenderSystemStyleBorder();

        Bin.Render();
        Parent.Render();
        Child.Render();
        Move.Render();
        New.Render();
        Ref.Render();
        Lin.Render();
        
        Update(true);
    }

    private void Update(bool UpdateSelection)
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

        for (int i = 0; i < directory2_list.Length; i++)
            DAF.Items[i] = directory2_list[i];

        for (int i = 0; i < directory_list.Length; i++)
            DAF.Items[i + directory2_list.Length] = directory_list[i];

        if (UpdateSelection)
        {
            DAF.Selected = 0;
        }
        
        cdirdis = new Input(this, 5, 310-20, 500-10, 15, " " + cdir3003)
        {
            //UseSystemStyle = false,
            ReadOnly = true
        };
        
        cdirdis.Render();
        DAF.Render();
    }

    private void ListClick()
    {
        string WhatWasClicked = DAF.Items[DAF.Selected];
        
        if (clickcounter == WhatWasClicked)
        {
            ccnum++;
            if (ccnum >= 2)
            {
                DoubleHandler(WhatWasClicked);
            }
        }
        else
        {
            clickcounter = WhatWasClicked;
            ccnum = 1;
        }
    }

    private void DoubleHandler(string Item)
    {
        if (Item.Contains('.'))
        {
            string filefigured = cdir5000 + @"\" + Item;
            WindowManager.AddWindow(new Notepad(true, filefigured));
        }
        else
        {
            cdir5000 = cdir5000 + @"\" + Item;
            Update(true);
        }
    }
    
    private void RefClick()
    {
        Update(false);
    }
    
    private void LinClick()
    {
        if (DAF.Items[DAF.Selected].Contains('.'))
        {
            string filefigured = cdir5000 + @"\" + DAF.Items[DAF.Selected];
            WindowManager.AddWindow(new Notepad(true, filefigured));
        }
    }
    
    private void BinClick()
    {
        string cdir3003 = cdir5000;
        string args = DAF.Items[DAF.Selected];

        string bdir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(cdir3003);
        
        if (args.Contains("0:\\"))
        {
            args.Replace(@"0:\", "");
        }
        
        if (File.Exists(Directory.GetCurrentDirectory() + @"\" + args))
            File.Delete(Directory.GetCurrentDirectory() + @"\" + args);
        else if (Directory.Exists(Directory.GetCurrentDirectory() + @"\" + args))
            Directory.Delete(Directory.GetCurrentDirectory() + @"\" + args, true);
        else
        {
            Dialogue.Show(
                "Error",
                "File or Directory not found!",
                null, // default buttons
                WindowManager.errorIcon);
        }

        Directory.SetCurrentDirectory(bdir);
        
        Update(false);
    }

    private void ParentClick()
    {
        string cdir3003 = cdir5000;

        string parentofcdir3003 = cdir3003.Remove(cdir3003.LastIndexOf("\\"));

        cdir5000 = parentofcdir3003;

        Update(true);
    }

    private void ChildClick()
    {
        string cdir3003 = cdir5000;

        if (!DAF.Items[DAF.Selected].Contains('.'))
        {
            cdir5000 = cdir3003 + @"\" + DAF.Items[DAF.Selected];
            Update(true);
        }
    }

    private void MoveClick()
    {
        Kernel.gcdir = cdir5000;
        WindowManager.AddWindow(new MoveFWindow());
    }
    
    private void NewClick()
    {
        Kernel.gcdir = cdir5000;
        WindowManager.AddWindow(new NewFDWindow());
    }
}

public class NewFDWindow : Window
{
    private Button OK;
    private Button Canel;
    private Input Filename;
    
    public NewFDWindow()
    {
        BetterConsole.font = new Font(BetterConsole.rawFont, BetterConsole.charHeight);
        Contents = new Canvas(300, 80);
        Title = "New - Gosplorer";
        Visible = true;
        Closable = false;
        Unkillable = false;
        SetDock(WindowDock.Auto);
        
        OK = new Button(this, 5, 50, 60, 20, "Ok")
        {
            Clicked = OKClick
        };
        Canel = new Button(this, 300-65, 50, 60, 20, "Cancel")
        {
            Clicked = CancelClick
        };
        Filename = new Input(this, 5, 25, 300 - 10, 20, "")
        {
        };
        
        Contents.Clear(Color.LightGray); // >:^(
        RenderSystemStyleBorder();
        
        OK.Render();
        Canel.Render();
        Filename.Render();
        
        
        Contents.DrawString(5,5,"Please input file name below:", BetterConsole.font, Color.White);
    }

    private void OKClick()
    {
        string fn = Filename.Text;
        string args = fn;

        if (!fn.Contains('.'))
        {
            if (!Directory.Exists(args))
                Directory.CreateDirectory(Kernel.gcdir + @"\" + args);
            
            Dispose();
        }
        else
        {
            if (args.Contains(@"if"))
            {
                args.Replace(@"if", "THAT NAME IS FORBIDDEN");
            }

            //potato2 = potato2.Split("mkfile ")[1];
            if (!File.Exists(args))
                File.Create(Kernel.gcdir + @"\" + args);
            
            Dispose();
        }
    }
    
    private void CancelClick()
    {
        Dispose();
    }
}

public class MoveFWindow : Window
{
    private Button OK;
    private Button Canel;
    private Input Filename;
    
    public MoveFWindow()
    {
        BetterConsole.font = new Font(BetterConsole.rawFont, BetterConsole.charHeight);
        Contents = new Canvas(300, 80);
        Title = "Move - Gosplorer";
        Visible = true;
        Closable = false;
        Unkillable = false;
        SetDock(WindowDock.Auto);
        
        OK = new Button(this, 5, 50, 60, 20, "Ok")
        {
            Clicked = OKClick
        };
        Canel = new Button(this, 300-65, 50, 60, 20, "Cancel")
        {
            Clicked = CancelClick
        };
        Filename = new Input(this, 5, 25, 300 - 10, 20, "")
        {
        };
        
        Contents.Clear(Color.LightGray); // >:^(
        RenderSystemStyleBorder();
        
        OK.Render();
        Canel.Render();
        Filename.Render();
        
        
        Contents.DrawString(5,5,"Please input new file path below:", BetterConsole.font, Color.White);
    }

    private void OKClick()
    {
        string to = Filename.Text;

        string[] testes = to.Split('\\');

        int ltu = testes.Length - 1;

        string stufftoremove = "";
        
        for (int i = 0; i < ltu; i++)
        {
            stufftoremove = stufftoremove + testes[i];
        }

        string frommfilename = to.Replace(stufftoremove, "");
        
        if (frommfilename.Contains('\\'))
        {
            frommfilename = frommfilename.Replace("\\", "");
        }

        string from = Kernel.gcdir + "\\" + frommfilename;
        
        GoOS.Commands.ExtendedFilesystem.MoveFile(from, to);
    }
    
    private void CancelClick()
    {
        Dispose();
    }
}