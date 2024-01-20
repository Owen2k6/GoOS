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

        Button SysDriveButton;
        Button CDDriveButton;

        public FileManager()
        {
            AutoCreate(WindowDock.Auto, 480, 360, Path + " - Gosplorer");

            AddressBar = new Input(this, 90, 10, 350, 20, "Path");
            BackButton = new Button(this, 10, 10, 20, 20, string.Empty) { Image = arrowleft };
            ForwardButton = new Button(this, 30, 10, 20, 20, string.Empty) { Image = arrowright };
            UpButton = new Button(this, 60, 10, 20, 20, string.Empty) { Image = arrowup };
            GoButton = new Button(this, 450, 10, 20, 20, "Go");

            SysDriveButton = new Button(this, 26, 50, 44, 20, "0:/")
            {
                UseSystemStyle = false,
                RenderWithAlpha = true,
                HasSelectionColour = true,
                BackgroundColour = new Color(75, 75, 75),
                SelectionColour = new Color(50, 50, 50)
            }; // TODO: Finish this and remove the highlight since it dont look right
            CDDriveButton = new Button(this, 26, 80, 44, 20, "1:/")
            {
                UseSystemStyle = false,
                RenderWithAlpha = true,
                HasSelectionColour = true,
                BackgroundColour = new Color(75, 75, 75),
                SelectionColour = new Color(50, 50, 50)
            };
        }

        public override void Paint()
        {
            Contents.DrawImage(0, 0, appbackground, false);
            Contents.DrawFilledRectangle(0, 0, Contents.Width, 40, 0, new Color(75, 75, 75));
            Contents.DrawFilledRectangle(0, 0, 80, Contents.Height, 0, new Color(75, 75, 75));
            Contents.DrawImage(10, 50, drive);
            Contents.DrawImage(10, 80, drive_locked);

            AddressBar.Render();
            BackButton.Render();
            ForwardButton.Render();
            UpButton.Render();
            GoButton.Render();

            SysDriveButton.Render();
            CDDriveButton.Render();

            RenderSystemStyleBorder();
        }
    }
}
