using System;
using Cosmos.System;
using Gold.Graphics;
namespace GoOS.GUI.Apps;

public class GoVM : Window
{
    private Button ChaOS_VM_b;
    private bool oig = true;

    public GoVM()
    {
        Contents = new Canvas(300, 300);
        Title = "Home - GoVM";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        ChaOS_VM_b = new Button(this, 5, 5, 60, 20, "ChaOS")
        {
            Clicked = ChaOS_VM_b_Click
        };
        
        Contents.Clear(Color.White);
        RenderSystemStyleBorder();
        
        ChaOS_VM_b.Render();
    }

    private void ChaOS_VM_b_Click()
    {
        if (oig)
        {
            oig = false;
            WindowManager.AddWindow(new ChaOS_VM());
        }
    }
}

public class ChaOS_VM : Window
{
    public Terminal VMTERM;

    public ChaOS_VM()
    {
        /*Dialogue.Show(
            "Error",
            "Owen is gay ;)",
            null, // default buttons
            WindowManager.errorIcon);*/
        try
        {
            VMTERM = new Terminal();

            Contents = VMTERM.terminal.Contents;
            Title = "ChaOS - GoVM";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);
            VMTERM.Visible = true;

            Commands.VM.Run("chaos");
        }
        catch (Exception eee)
        {
            Dialogue.Show(
                "Error",
                eee.Message,
                null, // default buttons
                WindowManager.errorIcon);
        }
    }
}