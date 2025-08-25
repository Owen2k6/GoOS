using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class CustomInterface : Window
    {

        Button closeButton;

        public CustomInterface(string Name, ushort width, ushort height) : base(0, 0, width, height, Name == null ? "Unnamed application" : Name)
        {
            Contents = new Canvas(width, height);
            Contents.Clear(Color.LightGray);
            Title = Name;
            SetDock(WindowDock.Center);
        }
        
        public static void AddString(Window me, string Text, int X, int Y)
        {
            me.Contents.DrawString(X, Y, Text, Font_1x, Color.Black);
        }
    }
}