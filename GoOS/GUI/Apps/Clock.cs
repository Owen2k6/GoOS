using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps
{
    public class Clock : Window
    {
        private byte lastSecond = Cosmos.HAL.RTC.Second;

        private bool digitalView = false;

        private string[] contextMenuButtons =
        {
            " Analog view",
            " Digital view"
        };

        public Clock()
        {
            // Create the window.
            Contents = new Canvas(192, 192);
            Title = "Clock";
            Visible = true;
            Closable = true;
            Sizable = true;
            SetDock(WindowDock.Auto);
        }

        private void RenderHand(int originX, int originY, int handLength, double radians, Color color)
        {
            int x = originX + (int)(handLength * Math.Sin(radians));
            int y = originY - (int)(handLength * Math.Cos(radians));
            Contents.DrawLine(originX, originY, x, y, color);
        }

        public override void Paint()
        {
            // Paint the window.
            DateTime now = DateTime.Now;
            string timeText = DateTime.Now.ToString("HH:mm:ss");

            Contents.Clear(Color.White);
            RenderSystemStyleBorder();

            if (!digitalView)
            {
                ushort originX = (ushort)(Contents.Width / 2);
                ushort originY = (ushort)(Contents.Height / 2);
                ushort diameter = (ushort)(Math.Min(Contents.Width, Contents.Height) * 0.75f);
                ushort radius = (ushort)(diameter / 2);

                Contents.DrawCircle(originX, originY, radius, Color.Black);

                for (int i = 1; i <= 12; i++)
                {
                    int numX = (int)(originX + (Math.Sin(i * Math.PI / 6) * radius * 0.8));
                    int numY = (int)(originY - Math.Cos(i * Math.PI / 6) * radius * 0.8);
                    Contents.DrawFilledCircle(numX, numY, 2, Color.Black);
                }

                /* Second hand */
                double second = now.Second;
                double secondRad = second * Math.PI / 30;
                RenderHand(originX, originY, radius, secondRad, Color.Red);

                /* Minute hand*/
                double minute = now.Minute + (second / 60);
                double minuteRad = minute * Math.PI / 30;
                RenderHand(originX, originY, (int)(radius * 0.75f), minuteRad, Color.Black);

                /* Hour hand */
                double hour = now.Hour + (minute / 60);
                double hourRad = hour * Math.PI / 6;
                RenderHand(originX, originY, (int)(radius * 0.5f), hourRad, Color.Black);
            }
            else
            {
                int x = (Contents.Width / 2) - (Font_2x.MeasureString(timeText) / 2);
                int y = (Contents.Height / 2) - (32 / 2);

                Contents.DrawString(x, y, timeText, Font_2x, Color.CoolGreen);
            }
        }

        public override void HandleRun()
        {
            base.HandleRun();

            if (Cosmos.HAL.RTC.Second != lastSecond)
            {
                lastSecond = Cosmos.HAL.RTC.Second;
                Paint();
            }
        }

        public override void ShowContextMenu()
        {
            ContextMenu.Show(contextMenuButtons, 112, ContextMenu_Handle);
        }

        private void ContextMenu_Handle(string item)
        {
            digitalView = item == contextMenuButtons[1];
            Paint();
        }
    }
}
