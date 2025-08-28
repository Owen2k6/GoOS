using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Welcome : Window
{
    private readonly Button closeButton;

    public Welcome()
    {
        Contents = new Canvas(400, 300);
        Title = "Welcome";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Center);

        Contents.DrawImage(0, 0, welcomeImage);

        closeButton = new Button(this, 315, 270, 80, 25, "Close");
        closeButton.Clicked = Dispose;

        closeButton.Render();
    }
}