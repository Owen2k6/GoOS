using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.GoIDE.gca.bmp")] private static byte[] ideIconRaw;
        private static Canvas ideIcon = Image.FromBitmap(ideIconRaw, false);

        Button gtermButton;
        Button clockButton;
        Button taskmanButton;
        Button ideButton;

        public AppManager()
        {
            Contents = new Canvas(400, 400);
            Contents.Clear(Color.LightGray);
            Title = "GoOS Applications";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            Contents.DrawString(10, 5, "GoOS System Applications", BetterConsole.font, Color.White);

            gtermButton = new Button(this, 10, 20, 64, 80, "GTerm")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.LightGray,
                TextColour = Color.White,

                Image = gtermIcon
            };

            taskmanButton = new Button(this, 90, 30, 90, 60, "TaskManager")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.LightGray,
                TextColour = Color.White,

                Image = taskmanIcon
            };
            
            gtermButton.Clicked = OpenGTerm;
            taskmanButton.Clicked = OpenTaskman;

            Contents.DrawString(10, 100, "GoOS Accessories", BetterConsole.font, Color.White);

            clockButton = new Button(this, 10, 120, 64, 80, "Clock")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.LightGray,
                TextColour = Color.White,

                Image = clockIcon
            };
            clockButton.Clicked = OpenClock;

            ideButton = new Button(this, 90, 130, 64, 80, "IDE")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.LightGray,
                TextColour = Color.White,

                Image = ideIcon
            };
            ideButton.Clicked = OpenIDE;

            foreach (Control control in Controls)
            {
                control.Render();
            }
        }

        void OpenIDE()
        {
            WindowManager.AddWindow(new Apps.GoIDE.NewProjectFrame());
        }

        private static void OpenGTerm()
        {
            WindowManager.AddWindow(new Apps.GTerm());
        }

        private static void OpenTaskman()
        {
            WindowManager.AddWindow(new Apps.TaskManager());
        }

        private static void OpenClock()
        {
            WindowManager.AddWindow(new Apps.Clock());
        }
    }
}