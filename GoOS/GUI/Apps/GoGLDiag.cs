using GoGL;
using GoGL.Graphics;

namespace GoOS.GUI.Apps;

public class GoGLDiag : Window
{
    private readonly Info ii = new();


    public GoGLDiag()
    {
        // Create the window.
        Contents = new Canvas(210, 135);
        Title = "About GoGL";
        Visible = true;
        Closable = true;

        SetDock(WindowDock.Auto);
        // Paint the window.
        Contents.Clear(Color.LightGray);
        RenderSystemStyleBorder();
        Contents.DrawImage(5, 5, ii.getLogo());
        Contents.DrawString(5, 105, "Version: " + ii.getVersion(), Resources.Font_1x, Color.White);
        Contents.DrawString(5, 118, "API Level: " + ii.getApiVersion(), Resources.Font_1x, Color.White);
    }
}