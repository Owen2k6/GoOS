using Gold.Graphics;
using GoOS._9xCode;

namespace GoOS.GUI.Apps
{
    public class About : Window
    {
        public About() : base(0, 0, 300, 230, "About this GoPC")
        {
            // Create the window.
            SetDock(WindowDock.Auto);
            // Paint the window.
            Contents.DrawImage(0, 0, Resources.abtbg, false);
            Contents.DrawString(10, 152, "GoOS " + Kernel.version, Resources.Font_1x, Color.White);
            Contents.DrawString(10, 164, "Gold " + Gold.Info.Version, Resources.Font_1x, Color.White);
            Contents.DrawString(10, 176, "GoCode " + GoCode.GoCode.Version, Resources.Font_1x, Color.White);
            Contents.DrawString(10, 188, "9xCode " + Interpreter.Version, Resources.Font_1x, Color.White);
            Contents.DrawString(10, 200, "Giff " + Giff.Giff.Version, Resources.Font_1x, Color.White);
            
            base.Render();
        }
    }
}