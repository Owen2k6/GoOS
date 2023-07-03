using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.HAL.Drivers.Video;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps;

public class Taskbar : Window
{
    public Taskbar()
    {
        Contents = new Canvas(WindowManager.Canvas.Width, 28);
        Contents.Clear(new Color(191, 191, 191));
        //so it displays at the bottom of the screen
        X = 0;
        Y = WindowManager.Canvas.Height - 28;
        Title = "GoOS";
        Visible = true;
        Closable = false;
        HasTitlebar = false;
        Contents.Clear(Color.DeepGray);;
        Contents.DrawFilledRectangle(0,0 , WindowManager.Canvas.Width, 3,0, Color.LightGray);
    }
}