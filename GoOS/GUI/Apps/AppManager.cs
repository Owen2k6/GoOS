using System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class AppManager : Window
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.gterm.bmp")] private static byte[] gtermIconRaw;
        private static Canvas gtermIcon = Image.FromBitmap(gtermIconRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.clock.bmp")] private static byte[] clockIconRaw;
        private static Canvas clockIcon = Image.FromBitmap(clockIconRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.TaskManager.bmp")] private static byte[] taskmanIconRaw;
        private static Canvas taskmanIcon = Image.FromBitmap(taskmanIconRaw, false);

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.ide.bmp")] private static byte[] ideIconRaw;
        private static Canvas ideIcon = Image.FromBitmap(ideIconRaw, false);

        Button[] AppButtons;
        Button CloseButton;

        public AppManager()
        {
            // Generate the fonts.
            Fonts.Generate();

            // Create the window.
            Contents = new Canvas(400, 350);
            Title = "GoOS Applications";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            // Initialize the controls.
            AppButtons = new Button[]
            {
                new Button(this, 10, 52, 64, 80, "GTerm")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = gtermIcon,
                    Clicked = GTerm_Click
                },
                new Button(this, 84, 52, 96, 80, "Task Manager")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = taskmanIcon,
                    Clicked = TaskMan_Click
                },
                new Button(this, 10, 184, 64, 80, "Clock")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = clockIcon,
                    Clicked = Clock_Click
                },
                new Button(this, 84, 184, 64, 80, "IDE")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = ideIcon,
                    Clicked = IDE_Click
                }
            };
            CloseButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Close") { Clicked = CloseButton_Click };

            // Paint the window.
            Contents.Clear(Color.LightGray);
            RenderSystemStyleBorder();
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
            Contents.DrawString(10, 10, "System Applications", Fonts.Font_2x, Color.White);
            Contents.DrawString(10, 142, "Accessories", Fonts.Font_2x, Color.White);
            foreach (Button AppButton in AppButtons) AppButton.Render();
            CloseButton.Render();
        }

        private void GTerm_Click()
        {
            WindowManager.AddWindow(new GTerm());
        }

        private void TaskMan_Click()
        {
            WindowManager.AddWindow(new TaskManager());
        }

        private void Clock_Click()
        {
            WindowManager.AddWindow(new Clock());
        }

        private void IDE_Click()
        {
            WindowManager.AddWindow(new GoIDE.ProjectsFrame());
        }

        private void CloseButton_Click()
        {
            Dispose();
        }
    }
}