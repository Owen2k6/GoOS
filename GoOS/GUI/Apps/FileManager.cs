using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class FileManager : Window
    {
        string Path = @"0:\";

        Input AddressBar;
        Button BackButton;
        Button ForwardButton;
        Button UpButton;
        Button GoButton;

        public FileManager()
        {
            AutoCreate(WindowDock.Auto, 480, 360, $"{Path} - Gosplorer");
            //AutoCreate(WindowDock.Auto, 480, 360, Path + " - Gosplorer");

            AddressBar = new Input(this, 90, 10, 350, 20, "Path");
            BackButton = new Button(this, 10, 10, 20, 20, string.Empty) { Image = arrowleft };
            ForwardButton = new Button(this, 30, 10, 20, 20, string.Empty) { Image = arrowright };
            UpButton = new Button(this, 60, 10, 20, 20, string.Empty) { Image = arrowup };
            GoButton = new Button(this, 450, 10, 20, 20, "Go");
        }

        public override void Paint()
        {
            Contents.DrawImage(0, 0, appbackground, false);
            Contents.DrawFilledRectangle(0, 0, Contents.Width, 40, 0, new Color(75, 75, 75));
            Contents.DrawFilledRectangle(0, 0, 40, Contents.Height, 0, new Color(75, 75, 75));

            AddressBar.Render();
            BackButton.Render();
            ForwardButton.Render();
            UpButton.Render();
            GoButton.Render();

            RenderSystemStyleBorder();
        }
    }
}
