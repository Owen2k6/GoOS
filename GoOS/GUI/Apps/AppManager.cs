﻿using System;
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

        Button gtermButton;
        Button clockButton;
        Button taskmanButton;

        public AppManager()
        {
            Contents = new Canvas(400, 400);
            Contents.Clear(Color.White);
            Title = "GoOS Applications";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Auto);

            Contents.DrawString(10, 5, "GoOS System Applications", BetterConsole.font, Color.Black);

            gtermButton = new Button(this, 10, 20, 64, 80, "GTerm")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.White,
                TextColour = Color.Black,

                Image = gtermIcon
            };

            taskmanButton = new Button(this, 90, 30, 90, 60, "TaskManager")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.White,
                TextColour = Color.Black,

                Image = taskmanIcon
            };
            
            gtermButton.Clicked = OpenGTerm;
            taskmanButton.Clicked = OpenTaskman;

            Contents.DrawString(10, 100, "GoOS Accessories", BetterConsole.font, Color.Black);

            clockButton = new Button(this, 10, 120, 64, 80, "Clock")
            {
                UseSystemStyle = false,
                BackgroundColour = Color.White,
                TextColour = Color.Black,

                Image = clockIcon
            };
            clockButton.Clicked = OpenClock;

            foreach (Control control in Controls)
            {
                control.Render();
            }
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