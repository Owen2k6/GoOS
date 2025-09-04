using Gold;
using Gold.Graphics;
using GoOS._9xCode;

namespace GoOS.GUI.Apps;

public class About : Window
{
    public About()
    {
        // Create the window.
        Contents = new Canvas(300, 230);
        Title = "About GoOS";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);
        // Paint the window.
        Contents.DrawImage(0, 0, Resources.abtbg, false);
        Contents.DrawString(10, 67, "GoOS " + Kernel.version, Resources.Charcoal, Color.White);
        Contents.DrawString(10, 79, "Gold " + Info.Version, Resources.Charcoal, Color.White);
        Contents.DrawString(10, 91, "GoCode " + GoCode.GoCode.Version, Resources.Charcoal, Color.White);
        Contents.DrawString(10, 103, "9xCode " + Interpreter.Version, Resources.Charcoal, Color.White);
        Contents.DrawString(10, 115, "Giff Engine " + Giff.Giff.Version, Resources.Charcoal, Color.White);
    }
}