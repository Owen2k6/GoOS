using System.IO;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.Gosplorer;

public class NewFileFrame : Window
{
    private Button OK;
    private Button Canel;
    private Input Filename;

    private string cdir = "";

    public NewFileFrame(string lcdir)
    {
        cdir = lcdir;
        
        Contents = new Canvas(300, 80);
        Title = "New File - Gosplorer";
        Visible = true;
        Closable = false;
        Unkillable = false;
        SetDock(WindowDock.Auto);

        OK = new Button(this, 5, 50, 60, 20, "Ok")
        {
            Clicked = OKClick
        };
        Canel = new Button(this, 300 - 65, 50, 60, 20, "Cancel")
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

        Contents.DrawString(5, 5, "Please input file name:", Resources.Font_1x, Color.White);
    }

    private void OKClick()
    {
        string fn = Filename.Text;
        string args = fn;

        if (!File.Exists(args))
            File.Create(cdir + @"\" + args);

        Dispose();
    }

    private void CancelClick()
    {
        Dispose();
    }
}