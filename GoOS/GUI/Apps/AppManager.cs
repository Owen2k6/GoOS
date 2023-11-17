using System;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class AppManager : Window
    {
        Button[] AppButtons;
        Button CloseButton;

        public AppManager()
        {
            // Create the window.
            Contents = new Canvas(400, 300);
            Title = "GoOS Applications";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            // Initialize the controls.
            AppButtons = new Button[]
            {
                new Button(this, 10, 10, 64, 80, "GTerm")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = gtermIcon,
                    Clicked = GTerm_Click
                },
                new Button(this, 84, 10, 96, 80, "Task Manager")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = taskmanIcon,
                    Clicked = TaskMan_Click
                },
                new Button(this, 190, 10, 64, 80, "Clock")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = clockIcon,
                    Clicked = Clock_Click
                },
                new Button(this, 262, 10, 64, 80, "IDE")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = ideIcon,
                    Clicked = IDE_Click
                },
                new Button(this, 10, 100, 64, 80, "Store")
                {
                    UseSystemStyle = false,
                    BackgroundColour = Color.LightGray,
                    SelectionColour = new Color(100, 100, 100),
                    TextColour = Color.White,

                    Image = GoStoreicon,
                    Clicked = Store_Click
                },
            };
            CloseButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, "Close") { Clicked = CloseButton_Click };

            // Paint the window.
            Contents.Clear(Color.LightGray);
            RenderSystemStyleBorder();
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, Color.DeepGray);
            foreach (Button AppButton in AppButtons) AppButton.Render();
            CloseButton.Render();
        }

        private void GTerm_Click() => WindowManager.AddWindow(new GTerm());

        private void TaskMan_Click() => WindowManager.AddWindow(new TaskManager());

        private void Clock_Click() => WindowManager.AddWindow(new Clock());

        private void IDE_Click() => WindowManager.AddWindow(new GoIDE.ProjectsFrame());

        private void Store_Click() => WindowManager.AddWindow(new GoStore.MainFrame());

        private void CloseButton_Click() => Dispose();
    }
}