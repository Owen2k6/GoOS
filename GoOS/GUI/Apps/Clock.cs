using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using PrismAPI.UI;

namespace GoOS.GUI.Apps
{
    public class Clock : Window
    {
        private byte lastSecond = Cosmos.HAL.RTC.Second;

        public Clock()
        {
            Contents = new Canvas(192, 192);
            Contents.Clear(Color.White);
            X = 200;
            Y = 100;
            Title = "Clock";
            Visible = true;
            Closable = true;

            RenderClock();
        }

        private void RenderHand(int originX, int originY, int handLength, double radians, Color color)
        {
            int x = originX + (int)(handLength * Math.Sin(radians));
            int y = originY - (int)(handLength * Math.Cos(radians));
            Contents.DrawLine(originX, originY, x, y, color);
        }

        private void RenderClock()
        {
            DateTime now = DateTime.Now;

            string timeText = DateTime.Now.ToString("HH:mm:ss");

            ushort originX = (ushort)(Contents.Width / 2);
            ushort originY = (ushort)(Contents.Height / 2);
            ushort diameter = (ushort)(Math.Min(Contents.Width, Contents.Height) * 0.75f);
            ushort radius = (ushort)(diameter / 2);

            Contents.Clear(Color.White);
            Contents.DrawCircle(originX, originY, radius, Color.Black);

            for (int i = 1; i <= 12; i++)
            {
                int numX = (int)(originX + (Math.Sin(i * Math.PI / 6) * radius * 0.8));
                int numY = (int)(originY - Math.Cos(i * Math.PI / 6) * radius * 0.8);
                Contents.DrawFilledCircle(numX, numY, 2, Color.Black);
            }

            Contents.DrawString((Contents.Width / 2) - (BetterConsole.font.MeasureString(timeText) / 2), 4, timeText, BetterConsole.font, Color.Red);

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

        public override void Update()
        {
            byte second = Cosmos.HAL.RTC.Second;
            if (second != lastSecond)
            {
                lastSecond = second;
                RenderClock();
            }
        }
    }
}
