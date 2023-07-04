using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Hardware.GPU;
using PrismAPI.Graphics;
using Cosmos.Core.Memory;
using PrismAPI.UI;
using Cosmos.System;

namespace GoOS.GUI
{
    public class WindowManager
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse.bmp")] private static byte[] mouseRaw;
        private static Canvas mouse = Image.FromBitmap(mouseRaw, false);

        // private static bool initialised = false;

        private static int framesToHeapCollect = 10;

        private static MouseState previousMouseState = MouseState.None;

        private static readonly List<Window> windows = new List<Window>(10);

        public static Display Canvas;

        public static void AddWindow(Window window)
        {
            windows.Add(window);
        }

        public static Window GetWindowByType<T>()
        {
            foreach (Window window in windows)
            {
                if (window is T winOfT)
                {
                    return window;
                }
            }

            return null;
        }

        private static void MoveWindowToFront(Window window)
        {
            windows.Add(window);
            windows.Remove(window);
        }

        private static int GetHoveredWindow()
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                if (windows[i].IsMouseOver)
                {
                    return i;
                }
            }

            return -1;
        }

        private static Window GetDraggingWindow()
        {
            foreach (Window window in windows)
            {
                if (window.Dragging)
                {
                    return window;
                }
            }

            return null;
        }

        private static void DrawMouse()
        {
            Canvas.DrawImage((int)MouseManager.X, (int)MouseManager.Y, mouse, true);
        }

        private static void DoInput()
        {
            Window draggingWindow = GetDraggingWindow();
            if (draggingWindow != null)
            {
                if (windows.IndexOf(draggingWindow) != windows.Count - 1)
                {
                    MoveWindowToFront(draggingWindow);
                }

                draggingWindow.HandleMouseInput();

                return;
            }

            int hoveredWindowIdx = GetHoveredWindow();
            if (hoveredWindowIdx != -1)
            {
                windows[hoveredWindowIdx].HandleMouseInput();

                if (hoveredWindowIdx        != windows.Count - 1 &&
                    MouseManager.MouseState != MouseState.None)
                {
                    MoveWindowToFront(windows[hoveredWindowIdx]);
                }
            }
        }

        public static void Update()
        {
            if (!BetterConsole.ConsoleMode)
            {
                if (MouseManager.ScreenWidth  != Canvas.Width ||
                    MouseManager.ScreenHeight != Canvas.Height)
                {
                    MouseManager.ScreenWidth = Canvas.Width;
                    MouseManager.ScreenHeight = Canvas.Height;
                }

                // MouseManager.ScreenWidth = Canvas.Width; // make it like this, cosmos is sometimes weird and makes your code not work
                // MouseManager.ScreenHeight = Canvas.Height;

                Canvas.Clear(Color.UbuntuPurple);

                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    if (windows[i].Closing)
                    {
                        windows.RemoveAt(i);
                    }
                }

                DoInput();

                foreach (Window window in windows)
                {
                    window.HandleRun();

                    if (window.Visible)
                    {
                        window.DrawWindow(Canvas);
                    }
                }

                string fps = Canvas.GetFPS() + "fps";

                Canvas.DrawString(Canvas.Width - 85, Canvas.Height - 12, fps, BetterConsole.font, Color.White, true);

                string Hour = Cosmos.HAL.RTC.Hour.ToString(), Minute = Cosmos.HAL.RTC.Minute.ToString();
                if (Minute.Length < 2) Minute = "0" + Minute;
                Canvas.DrawString(Canvas.Width - 30, Canvas.Height - 12, Hour + ":" + Minute, BetterConsole.font, Color.White, true);

                DrawMouse();

                Canvas.Update();

                previousMouseState = MouseManager.MouseState;
            }
            else
            {
                Canvas.DrawImage(0, 0, BetterConsole.Canvas, false);
                Canvas.Update();
            }

            if (framesToHeapCollect == 0)
            {
                Heap.Collect();
                framesToHeapCollect = 10;
            }
            framesToHeapCollect--;
        }
    }
}
