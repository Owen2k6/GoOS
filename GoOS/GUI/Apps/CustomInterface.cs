using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class CustomInterface : Window
{
    private Button closeButton;

    public CustomInterface(string Name, ushort width, ushort height)
    {
        if (Name == null) Name = "Unnamed application";
        Contents = new Canvas(width, height);
        Contents.Clear(Color.LightGray);
        Title = Name;
        Visible = true; //Any non OS application must always be visible and closable. No exceptions.
        Closable = true;
        SetDock(WindowDock.Center);
    }

    public static void AddString(Window me, string Text, int X, int Y)
    {
        me.Contents.DrawString(X, Y, Text, Font_1x, Color.Black);
    }
}