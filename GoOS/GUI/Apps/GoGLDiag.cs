using GoGL;
using GoGL.Graphics;
using GoGL.Graphics.Fonts;

namespace GoOS.GUI.Apps
{
    public class GoGLDiag : Window
    {
        private Info ii = new Info();
        
        
        public GoGLDiag()
        {
            // Create the window.
            Contents = new Canvas(210, 135);
            Title = "About GoGL";
            Visible = true;
            Closable = true;
            Sizable = false;
            
            SetDock(WindowDock.Auto);
            // Paint the window.
            Contents.Clear(Color.LightGray);
            RenderSystemStyleBorder();
            Contents.DrawImage(5, 5, ii.getLogo() , true);
            Contents.DrawString(5, 105, "Version: "+ ii.getVersion(), Resources.Font_1x, Color.White, false);
            Contents.DrawString(5, 118, "API Level: "+ ii.getApiVersion(), Resources.Font_1x, Color.White, false);
        }
    }
}