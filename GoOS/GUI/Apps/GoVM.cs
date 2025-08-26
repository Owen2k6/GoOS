using System;
using Gold.Graphics;
using GoOS.GUI.Apps.Terminal;

namespace GoOS.GUI.Apps;

public class GoVM : Window
{
    private Button ChaOS_VM_b;
    private bool oig = true;

    public GoVM() : base(0, 0, 300, 600, "Home - GoVM")
    {
        SetDock(WindowDock.Auto);

        ChaOS_VM_b = new Button(this, 5, 5, 60, 20, "ChaOS")
        {
            Clicked = ChaOS_VM_b_Click
        };
        
        Contents.Clear(Color.White);
        //RenderSystemStyleBorder();
        
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
    public GoTerminal VMTERM;

    public ChaOS_VM() : base(0, 0, 300, 600, "ChaOS - GoVM")
    {
        /*Dialogue.Show(
            "Error",
            "Owen is gay ;)",
            null, // default buttons
            WindowManager.errorIcon);*/
        try
        {
            VMTERM = new GoTerminal();

            Contents = VMTERM.term.terminal.Contents;
            Title = "ChaOS - GoVM";
            SetDock(WindowDock.Auto);

            Commands.VM.Run("chaos");
        }
        catch (Exception eee)
        {
            Dialogue.Show(
                "Error",
                eee.Message,
                null, // default buttons
                Resources.errorIcon);
        }
    }
}