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
        private static int TopWindow;

        public static Display Canvas;
        public static List<Window> Windows = new List<Window>(10);

        public static void Update()
        {
            if (runOnce)
            {
                Sys.MouseManager.ScreenWidth = 1280;
                Sys.MouseManager.ScreenHeight = 720;
                runOnce = false;
            }

            Canvas.Clear(Color.UbuntuPurple);

            int lastWindowShown = 0;

            for (int i = 0; i < Windows.Count; i++)
            {
                if (Windows[i] != null)
                {
                    if (Windows[i].MouseOnTop())
                    {
                    }

                    if (Windows[i].Visible)
                        DrawWindow(Windows[i].Contents, Windows[i].X, Windows[i].Y, Windows[i].Title, Windows[i].Closeable);
                    else
                        Windows[i] = null;

                    lastWindowShown = i;
                }
            }

            Windows[lastWindowShown].InternalFullUpdate();
            Windows[lastWindowShown].Update();

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