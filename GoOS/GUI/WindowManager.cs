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
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse.bmp")] private static byte[] mouseRaw;
        private static Canvas mouse = Image.FromBitmap(mouseRaw, false);

        private static bool initialised = false;

        private static readonly List<Window> windows = new List<Window>(10);

        public static Display Canvas;

        public static void AddWindow(Window window)
        {
            windows.Add(window);
        }

        private static void DrawMouse()
        {
            Canvas.DrawImage((int)Sys.MouseManager.X, (int)Sys.MouseManager.Y, mouse, true);
        }

        public static void Update()
        {
            if (!initialised)
            {
                Sys.MouseManager.ScreenWidth = 1280;
                Sys.MouseManager.ScreenHeight = 720;
                initialised = true;
            }

            Canvas.Clear(Color.UbuntuPurple);

            Window lastWindowShown = null;

            for (int i = windows.Count - 1; i >= 0; i--)
            {
                if (windows[i].Closing)
                {
                    windows.RemoveAt(i);
                }
            }

            foreach (Window window in windows)
            {
                if (window.Visible)
                {
                    window.DrawWindow(Canvas);
                }

                lastWindowShown = window;
            }

            if (lastWindowShown != null)
            {
                lastWindowShown.InternalHandle();
                lastWindowShown.Update();
            }

            Canvas.DrawString(128, Canvas.Height - 32,
                Canvas.GetFPS() + "fps / " + Cosmos.Core.GCImplementation.GetAvailableRAM() + "mb", BetterConsole.font,
                Color.White, true); // debug

            DrawMouse();

            Canvas.Update();

            Heap.Collect();
        }
    }
}
