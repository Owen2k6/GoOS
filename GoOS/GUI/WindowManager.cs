using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using IL2CPU.API.Attribs;
using PrismAPI.Hardware.GPU;
using PrismAPI.Graphics;
using Cosmos.Core.Memory;

namespace GoOS.GUI
{
    public class WindowManager
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse.bmp")]       private static byte[] mouseRaw;
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.closebutton.bmp")] private static byte[] closeButtonRaw;
        private static Canvas mouse       = Image.FromBitmap(mouseRaw, false);
        private static Canvas closeButton = Image.FromBitmap(closeButtonRaw, false);

        private static bool runOnce = true;

        public static Display Canvas;
        public static List<Window> Windows = new List<Window>(10);
        public static List<Window> Taskbar = new List<Window>(10);

        public static void Update()
        {
            if (runOnce)
            {
                Sys.MouseManager.ScreenWidth = 1280;
                Sys.MouseManager.ScreenHeight = 720;
                runOnce = false;
            }

            Canvas.Clear(Color.UbuntuPurple);

            for (int i = 0; i < Windows.Count; i++)
            {
                if (Windows[i] != null)
                {
                    if (Sys.MouseManager.MouseState == Sys.MouseState.Left)
                    {
                        if (Windows[i].Closeable)
                        {
                            if (Sys.MouseManager.X > Windows[i].X + Windows[i].Contents.Width - 14 &&
                                Sys.MouseManager.X < Windows[i].X + Windows[i].Contents.Width - 2 &&
                                Sys.MouseManager.Y > Windows[i].Y + 2 && Sys.MouseManager.Y < Windows[i].Y + 14)
                            {
                                Windows[i].Visible = false;
                                continue;
                            }
                        }

                        if (Sys.MouseManager.X > Windows[i].X && Sys.MouseManager.X < Windows[i].X + Windows[i].Contents.Width && Sys.MouseManager.Y > Windows[i].Y && Sys.MouseManager.Y < Windows[i].Y + 16)
                        {
                            Windows[i].X = (int)Sys.MouseManager.X - (Windows[i].Contents.Width / 2);
                            Windows[i].Y = (int)Sys.MouseManager.Y - 8;
                        }
                    }

                    DrawWindow(Windows[i].Contents, Windows[i].X, Windows[i].Y, Windows[i].Title, Windows[i].Closeable);
                    Windows[i].Update();
                }
            }

            Canvas.DrawString(128, Canvas.Height - 32,
                Canvas.GetFPS() + "fps / " + Cosmos.Core.GCImplementation.GetAvailableRAM() + "mb", BetterConsole.font,
                Color.White, true); // debug

            Canvas.DrawImage((int)Sys.MouseManager.X, (int)Sys.MouseManager.Y, mouse, true);
            Canvas.Update();
            Heap.Collect();
        }

        public static void DrawWindow(Canvas cv, int X, int Y, string Title, bool Closeable)
        {
            Canvas.DrawFilledRectangle(X, Y, Convert.ToUInt16(cv.Width), 16, 0, Color.DeepGray);
            Canvas.DrawString(X + 1, Y, Title, BetterConsole.font, Color.White);
            if (Closeable) Canvas.DrawImage(X + cv.Width - 14, Y + 2, closeButton);
            Canvas.DrawImage(X, Y + 16, cv, false);
        }
    }
}