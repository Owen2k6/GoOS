using System;
using Cosmos.HAL;
using GoGL.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps;

public class Clock : Window
{
    private readonly string[] contextMenuButtons =
    {
        " Analog view",
        " Digital view"
    };

    private bool digitalView;
    private byte lastSecond = RTC.Second;

    public Clock()
    {
        // Create the window.
        Contents = new Canvas(192, 192);
        Title = "Clock";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);
    }

    private void RenderHand(int originX, int originY, int handLength, double radians, Color color)
    {
        var x = originX + (int)(handLength * Math.Sin(radians));
        var y = originY - (int)(handLength * Math.Cos(radians));
        Contents.DrawLine(originX, originY, x, y, color);
    }

    public override void Paint()
    {
        // Paint the window.
        var now = DateTime.Now;
        var timeText = DateTime.Now.ToString("HH:mm:ss");

        Contents.Clear(Color.White);
        RenderSystemStyleBorder();

        if (!digitalView)
        {
            var originX = (ushort)(Contents.Width / 2);
            var originY = (ushort)(Contents.Height / 2);
            var diameter = (ushort)(Math.Min(Contents.Width, Contents.Height) * 0.75f);
            var radius = (ushort)(diameter / 2);

            Contents.DrawCircle(originX, originY, radius, Color.Black);

            for (var i = 1; i <= 12; i++)
            {
                var numX = (int)(originX + Math.Sin(i * Math.PI / 6) * radius * 0.8);
                var numY = (int)(originY - Math.Cos(i * Math.PI / 6) * radius * 0.8);
                Contents.DrawFilledCircle(numX, numY, 2, Color.Black);
            }

            /* Second hand */
            double second = now.Second;
            var secondRad = second * Math.PI / 30;
            RenderHand(originX, originY, radius, secondRad, Color.Red);

            /* Minute hand*/
            var minute = now.Minute + second / 60;
            var minuteRad = minute * Math.PI / 30;
            RenderHand(originX, originY, (int)(radius * 0.75f), minuteRad, Color.Black);

            /* Hour hand */
            var hour = now.Hour + minute / 60;
            var hourRad = hour * Math.PI / 6;
            RenderHand(originX, originY, (int)(radius * 0.5f), hourRad, Color.Black);
        }
        else
        {
            var x = Contents.Width / 2 - Font_2x.MeasureString(timeText) / 2;
            var y = Contents.Height / 2 - 32 / 2;

            Contents.DrawString(x, y, timeText, Font_2x, Color.CoolGreen);
        }
    }

    public override void HandleRun()
    {
        base.HandleRun();

        if (RTC.Second != lastSecond)
        {
            lastSecond = RTC.Second;
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