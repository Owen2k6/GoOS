using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class StartMenu : Window
    {
        Button[] apps;

        public StartMenu()
        {
            Contents = new Canvas(100, 300);
            Contents.Clear(Color.White);
            X = 0;
            Y = WindowManager.Canvas.Height - 16 - Contents.Height;
            HasTitlebar = false;
            Visible = true;

            apps = new Button[1];
            apps[0] = new Button(this, 8, 8, 84, 16, "GTerm")
            {
                BackgroundColour = Color.LightGray,
                TextColour = Color.Black,
                Clicked = GTerm_Click
            };

            foreach (var app in apps)
            {
                app.Render();
            }
        }

        private void GTerm_Click()
        {
            WindowManager.AddWindow(new GTerm());
            //Visible = false;
        }
    }
}
