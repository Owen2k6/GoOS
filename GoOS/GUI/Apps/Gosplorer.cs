using System;
using System.IO;
using Cosmos.System;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps;

public class Gosplorer : Window
{
    List DAF;
    private string cdir = @"0:\";

    public Gosplorer()
    {
        Contents = new Canvas(270, 310);
        Title = "Gosplorer";
        Visible = true;
        Closable = true;
        Unkillable = false;
        SetDock(WindowDock.Auto);

        DAF = new List(this, 5, 5, Convert.ToUInt16(Contents.Width - 10), Convert.ToUInt16(Contents.Height - 60),"Directory Listing", Array.Empty<string>());
        
        // Render the buttons.
        Contents.Clear(Color.White);
        RenderSystemStyleBorder();
        
        DAF.Render();
        try
        {
            Update();
        }
        catch(Exception e)
        {
            Dialogue.Show(
                "Error",
                e.Message,
                null, // default buttons
                WindowManager.errorIcon);
        }
    }

    private void Update()
    {
        String BackTo = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(cdir);
        
        int fullLength = 0;

        bool calcLength = true;

        string cdir3002 = Directory.GetCurrentDirectory();
        string cdir3003 = @"0:\";
        if (cdir3002.Contains(@"0:\\"))
        {
            cdir3003 = cdir3002.Replace(@"0:\\", @"0:\");
        }

        var directory_list = Directory.GetFiles(cdir3003);
        var directory2_list = Directory.GetDirectories(cdir3003);
        
        for (int i = 0; i < directory_list.Length + directory2_list.Length; i++)
            fullLength++;

        if (fullLength == directory_list.Length + directory2_list.Length)
        {
            for (int i = 0; i < directory_list.Length; i++)
                DAF.Items[i] = directory_list[i];
            
            for (int i = 0; i < directory2_list.Length; i++)
                DAF.Items[i] = directory2_list[i];
            
            DAF.Render(); // Render the window list.
            
            Directory.SetCurrentDirectory(BackTo);
        }
    }
}