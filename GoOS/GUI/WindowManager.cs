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
using GoOS.GUI.Apps;
using PrismAPI.Runtime.SystemCall;

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

        public static bool Dimmed = false;

        internal static Action<Window> TaskbarWindowAddedHook;

        internal static Action<Window> TaskbarWindowRemovedHook;

        public static void AddWindow(Window window)
        {
            windows.Add(window);

            TaskbarWindowAddedHook?.Invoke(window);
        }

        public static T GetWindowByType<T>()
        {
            foreach (Window window in windows)
            {
                if (window is T winOfT)
                {
                    return winOfT;
                }
            }

            return default; // null
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

        private static void ToggleStartMenu()
        {
            if (!Dimmed)
            {
                StartMenu startMenu = GetWindowByType<StartMenu>();

                startMenu.Visible = !startMenu.Visible;
                if (startMenu.Visible)
                {
                    MoveWindowToFront(startMenu);
                }
            }
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
                if (!Dimmed &&
                    windows.IndexOf(draggingWindow) != windows.Count - 1)
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
                    MouseManager.MouseState != MouseState.None &&
                    !Dimmed)
                {
                    MoveWindowToFront(windows[hoveredWindowIdx]);
                }
            }

            Window focusedWindow = windows[windows.Count - 1];

            bool keyPressed = KeyboardManager.TryReadKey(out var key);
            if (keyPressed)
            {
                if ((key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt &&
                    key.Key == ConsoleKeyEx.Tab &&
                    !Dimmed)
                {
                    AltTab();
                    return;
                }

                if (key.Key == ConsoleKeyEx.LWin ||
                    key.Key == ConsoleKeyEx.RWin)
                {
                    ToggleStartMenu();
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
                        TaskbarWindowRemovedHook?.Invoke(windows[i]);

                        if (windows[i].Title == "GoOS")
                            Dimmed = false;

                        windows.RemoveAt(i);
                    }
                }

                DoInput();

                for (int i = 0; i <= windows.Count - 1; i++)
                {
                    Window window = windows[i];
                    bool focused = i == windows.Count - 1;

                    window.HandleRun();

                    if (focused && Dimmed)
                        DimBackground();

                    if (window.Visible)
                    {
                        window.DrawWindow(Canvas, focused);
                    }
                }

                string fps = Canvas.GetFPS() + "fps";

                Canvas.DrawString(Canvas.Width - 85, Canvas.Height - 13, fps, BetterConsole.font, Color.Black, true);

                // Todo, move this clock to the taskbar for perf. reasons

                string Hour = Cosmos.HAL.RTC.Hour.ToString(), Minute = Cosmos.HAL.RTC.Minute.ToString();
                if (Minute.Length < 2) Minute = "0" + Minute;
                Canvas.DrawString(Canvas.Width - 30, Canvas.Height - 13, Hour + ":" + Minute, BetterConsole.font, Color.Black, true);

                DrawMouse();

                Canvas.Update();

                previousMouseState = MouseManager.MouseState;

                MemoryWatch.Watch();
            }
            else
            {
                bool keyPressed = KeyboardManager.TryReadKey(out var key);
                if (keyPressed)
                {
                    BetterConsole.KeyBuffer.Enqueue(key);
                }

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

        private static void DimBackground()
        {
            for (int y = 0; y < WindowManager.Canvas.Height - 1; y++)
            {
                for (int x = 0; x < WindowManager.Canvas.Width - 1; x++)
                {
                    if ((y % 2) == 0)
                    {
                        if ((x % 2) == 0)
                        {
                            WindowManager.Canvas[x, y] = Color.Black;
                        }
                    }
                    else
                    {
                        if ((x % 2) == 0)
                        {
                            WindowManager.Canvas[x + 1, y] = Color.Black;
                        }
                    }
                }
            }
        }
    }
}
