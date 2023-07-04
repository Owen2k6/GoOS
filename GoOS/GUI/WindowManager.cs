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

        private static int altTabIndex;

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

        public static void MoveWindowToFront(Window window)
        {
            windows.Add(window);
            windows.Remove(window);
        }

        private static int GetHoveredWindow()
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                if (windows[i].IsMouseOver && windows[i].Visible)
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

        private static void AltTab()
        {
            List<Window> tabbableWindows = new List<Window>();
            foreach (Window window in windows)
            {
                if (window.HasTitlebar)
                {
                    tabbableWindows.Add(window);
                }
            }

            if (tabbableWindows.Count < 2)
            {
                return;
            }

            int tabIndex = tabbableWindows.IndexOf(windows[^1]);
            if (tabIndex == -1)
            {
                tabIndex = 0;
            }
            tabIndex = (tabIndex + 1) % tabbableWindows.Count;

            MoveWindowToFront(tabbableWindows[tabIndex]);
        }

        private static void DoInput()
        {
            if (windows.Count == 0)
            {
                return;
            }

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

            Window focusedWindow = windows[windows.Count - 1];

            bool keyPressed = KeyboardManager.TryReadKey(out var key);
            if (keyPressed)
            {
                if ((key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt &&
                    key.Key == ConsoleKeyEx.Tab)
                {
                    AltTab();
                    return;
                }

                focusedWindow.HandleKey(key);
            }
        }

        public static void CloseAll()
        {
            windows.Clear();
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

                Canvas.Clear(Color.UbuntuPurple);

                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    if (windows[i].Closing)
                    {
                        windows.RemoveAt(i);
                    }
                }

                DoInput();

                for (int i = 0; i <= windows.Count - 1; i++)
                {
                    Window window = windows[i];
                    bool focused = i == windows.Count - 1;

                    window.HandleRun();

                    if (window.Visible)
                    {
                        window.DrawWindow(Canvas, focused);
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

                MemoryWatch.Watch();
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
