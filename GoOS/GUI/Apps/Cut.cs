using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Cut : Window
{
    public Cut()
    {
        Contents = new Canvas(148, 150);
        Title = "Cut";
        Visible = true;
        Closable = false;
        Unkillable = true;
        SetDock(WindowDock.Auto);

        Contents.Clear(Color.White);
        RenderSystemStyleBorder();

        Contents.DrawImage(0, 0, cutIcon);
    }
}