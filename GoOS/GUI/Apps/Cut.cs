using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Cut : Window
{
    public Cut() : base(0, 0, 148, 150, "Cut")
    {
        SetDock(WindowDock.Auto);

        Contents.Clear(Color.White);
        
        Contents.DrawImage(0, 0, cutIcon);
        
        base.Render();
    }
}