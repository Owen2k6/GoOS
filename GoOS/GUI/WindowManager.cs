using System.Collections.Generic;
using Cosmos.Core.Memory;
using Cosmos.System;
using Gold.Graphics;
using Gold.Hardware.GPU; 
using Cosmos.HAL;
using GoOS.Tasking;
using System.IO;

namespace GoOS.GUI
{
    internal sealed class WindowManager : Process
    {
        private static int _framesToHeapCollect = 20;
        private static byte _lastSecond = RTC.Second;

        internal static List<Window> Windows = new List<Window>();
        internal static List<Window> WindowOrder = new List<Window>();
        internal static Display Screen = null;

        internal static WindowManager Instance;

        internal WindowManager(Display screen = null) : base(nameof(WindowManager))
        {
            if (screen == null)
            {
                byte[] screenRes = File.Exists(@"0:\content\sys\resolution.gms")
                    ? File.ReadAllBytes(@"0:\content\sys\resolution.gms")
                    : new byte[] { 6 };

                var videoMode = ControlPanel.videoModes[screenRes[0]].Item2;
                
                Screen = Display.GetDisplay(videoMode.Width, videoMode.Height);
            }
            else 
                Screen = screen;

            MouseManager.ScreenWidth = Screen.Width;
            MouseManager.ScreenHeight = Screen.Height;
            MouseManager.X = (uint)Screen.Width / 2;
            MouseManager.Y = (uint)Screen.Height / 2;

            Screen.DefineCursor(Resources.Mouse);
        }

        internal static Window FocusedWindow
        {
            get
            {
                if (Windows.Count < 1)
                    return null;

                return Windows[^1];
            }
            set
            {
                Windows.Remove(value);
                Windows.Add(value);
            }
        }

        internal static void AddWindow(Window window) {
            Windows.Add(window);
            WindowOrder.Add(window);
        }

        internal static bool RemoveWindow(Window window) {
            bool wo = WindowOrder.Remove(window);
            bool w = Windows.Remove(window);
            Render();
            return w && wo;
        } 

        internal static void Render()
        {
            //Screen.DrawImage(0, 0, Resources.background, false);

            Screen.DrawString(10, 10, "GoOS v1.6", Resources.Charcoal, Color.White, Shadow: true);
            Screen.DrawString(10, 36, Screen.GetFPS() + " FPS", Resources.Charcoal, Color.White, Shadow: true);

            // This generates MASSIVE lag spikes; only use if you really need to debug this sort of stuff
            /*Screen.DrawString(10, 88, "Process list:", Resources.Charcoal, Color.White, Shadow: true);
            for (int i = 0; i < ProcessScheduler.Processes.Count; i++)
                Screen.DrawString(10, 114 + (i * 16), ProcessScheduler.Processes[i].PID + " (" + ProcessScheduler.Processes[i].Name + ")",
                    Resources.Charcoal, Color.White, Shadow: true);

            Screen.DrawString(200, 88, "Window list:", Resources.Charcoal, Color.White, Shadow: true);
            for (int i = 0; i < Windows.Count; i++)
                Screen.DrawString(200, 114 + (i * 16), Windows[i].PID + " (" + Windows[i].Name + ")",
                    Resources.Charcoal, Color.White, Shadow: true);*/

            foreach (Window w in Windows)
            {
                if (w == null)
                {
                    RemoveWindow(w);
                    continue;
                }

                if (WindowOrder.Contains(w)) WindowOrder.Remove(w);
                WindowOrder.Add(w);

                Screen.DrawImage(w.X, w.Y, w.Contents, false);

                if (w.Borderless) continue;

                Screen.DrawLine(w.X + 2, w.Y + w.Height, w.X + w.Width + 1, w.Y + w.Height, Color.Black);
                Screen.DrawLine(w.X + w.Width, w.Y + 2, w.X + w.Width, w.Y + w.Height + 1, Color.Black);
            }

            /*WindowOrder.Reverse();
            Screen.DrawString(400, 88, "WindowOrder list (Count: " + WindowOrder.Count + "):", Resources.Charcoal, Color.White, Shadow: true);
            for (int i = 0; i < WindowOrder.Count; i++)
                Screen.DrawString(400, 114 + (i * 16), WindowOrder[i].PID + " (" + WindowOrder[i].Name + ") " + i + " " + WindowOrder[i].IsMouseOver() + " " + WindowOrder[i].isMouseOverArea + " " + WindowOrder[i].Focused,
                    Resources.Charcoal, Color.White, Shadow: true);
            WindowOrder.Reverse();*/

            Screen.Update(false);

            if (_framesToHeapCollect <= 0)
            {
                Heap.Collect();
                _framesToHeapCollect = 20;
            }

            _framesToHeapCollect--;
        }

        internal override void HandleRun()
        {
            if (_lastSecond != RTC.Second)
            {
                Render();
                _lastSecond = RTC.Second;
            }

            // Rendering the mouse technically counts as rendering a frame.
            // Since we don't need to copy the framebuffer for rendering the
            // mouse, just increase the FPS counter by one.
            Screen.SetCursor(MouseManager.X, MouseManager.Y, true);
            Screen.Frames++;
        }
    }
    
    /*public class WindowManager
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse.bmp")]
        private static byte[] mouseRaw;

        public static Canvas mouse = Image.FromBitmap(mouseRaw, false);

        private static int framesToHeapCollect = 10;

        private static bool MouseDrawn = false;

        public static Canvas MouseToDraw = mouse;

        public static int MouseOffsetX = 0, MouseOffsetY = 0;

        public static List<Window> windows = new List<Window>(10);

        public static Display Canvas;

        public static bool Dimmed = false;

        internal static Action<Window> TaskbarWindowAddedHook;

        internal static Action<Window> TaskbarWindowRemovedHook;

        internal static Action TaskmanHook;

        internal static Action TaskbarFocusChangedHook;

        public static bool IsInOOBE = false;

        public static Window FocusedWindow
        {
            get
            {
                if (windows.Count < 1)
                {
                    return null;
                }

                return windows[^1];
            }
        }

        public static Window GetWindowByTitle(string wnd)
        {
            foreach (Window w in windows)
            {
                if (w.Title == wnd)
                {
                    return w;
                }
            }

            return null;
        }

        public static Action MouseMove;

        public static bool AreThereWindowsInRange(int StartX, int StartY, int EndX, int EndY)
        {
            foreach (Window w in windows)
            {
                if (w is not Desktop && w.X > StartX && w.X < EndX && w.Y > StartY && w.Y < EndY)
                {
                    return true;
                }
            }

            return false;
        }

        public static int GetAmountOfWindowsByTitle(string wnd)
        {
            int count = 0;

            foreach (Window w in windows)
            {
                if (w.Title == wnd)
                {
                    count++;
                }
            }

            return count;
        }

        public static bool ContainsWindowByTitle(string wnd) => GetAmountOfWindowsByTitle(wnd) > 0;

        public static bool ContainsAWindow(List<Window> wnds)
        {
            foreach (Window w in wnds)
            {
                if (windows.Contains(w)) return true;
            }

            return false;
        }

        public static void RemoveWindowByTitle(string wnd)
        {
            foreach (Window w in windows)
            {
                if (w.Title == wnd)
                {
                    w.Dispose();
                }
            }
        }

        public static void AddWindow(Window window)
        {
            Resources.Generate(ResourceType.Fonts);

            window.Paint();
            windows.Add(window);

            TaskmanHook?.Invoke();
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

            TaskmanHook?.Invoke();
            TaskbarFocusChangedHook?.Invoke();
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

        public static Window GetDraggingWindow()
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

        public static bool AreThereDraggingWindows
        {
            get { return GetDraggingWindow() != null; }
        }

        private static void DrawMouse()
        {
            int drawX = (int)MouseManager.X - MouseOffsetX;
            int drawY = (int)MouseManager.Y - MouseOffsetY;

            if (drawX < -MouseToDraw.Width) drawX = -MouseToDraw.Width;
            if (drawY < -MouseToDraw.Height) drawY = -MouseToDraw.Height;
            int maxX = Canvas.Width - 1;
            int maxY = Canvas.Height - 1;
            if (drawX > maxX) drawX = maxX;
            if (drawY > maxY) drawY = maxY;

            // enable alpha
            Canvas.DrawImage(drawX, drawY, MouseToDraw, true);
        }

        private static void SyncMouseBoundsAndClamp()
        {
            // Ensure MouseManager knows the real screen size (only when it changes)
            if (MouseManager.ScreenWidth != Canvas.Width || MouseManager.ScreenHeight != Canvas.Height)
            {
                MouseManager.ScreenWidth = Canvas.Width;
                MouseManager.ScreenHeight = Canvas.Height;

                // If Cosmos didn’t clamp internally, keep X/Y sane
                if (MouseManager.X >= MouseManager.ScreenWidth) MouseManager.X = (uint)(MouseManager.ScreenWidth - 1);
                if (MouseManager.Y >= MouseManager.ScreenHeight) MouseManager.Y = (uint)(MouseManager.ScreenHeight - 1);
            }
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

        private static bool startOpen = false;

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

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].Title == nameof(Desktop) && !AreThereWindowsInRange(20, 20, 84, 100))
                {
                    var w = GetWindowByTitle(nameof(ContextMenu));
                    if (w != null)
                    {
                        if (!AreThereWindowsInRange(w.X, w.Y, w.X + w.Contents.Width, w.Y + w.Contents.Height))
                        {
                            windows[i].HandleMouseInput();
                            break;
                        }
                    }
                    else
                    {
                        windows[i].HandleMouseInput();
                        break;
                    }
                }
            }

            int hoveredWindowIdx = GetHoveredWindow();
            if (hoveredWindowIdx != -1)
            {
                if (windows[hoveredWindowIdx].Title != nameof(Desktop))
                    windows[hoveredWindowIdx].HandleMouseInput();

                if (hoveredWindowIdx != windows.Count - 1 &&
                    MouseManager.MouseState != MouseState.None &&
                    !Dimmed &&
                    windows[hoveredWindowIdx].Title != nameof(Desktop))
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

                else if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed && key.Key == ConsoleKeyEx.Delete)
                {
                    AddWindow(new TaskManager());
                }


                focusedWindow.HandleKey(key);
            }
        }

        public static void CloseAll()
        {
            windows.Clear();
        }

        private static uint LastCursorX, LastCursorY;

        public static void Update()
        {
            try
            {
                SyncMouseBoundsAndClamp();

                if (MouseManager.ScreenWidth != Canvas.Width || MouseManager.ScreenHeight != Canvas.Height)
                {
                    MouseManager.ScreenWidth = Canvas.Width;
                    MouseManager.ScreenHeight = Canvas.Height;
                }

                if (IsInOOBE) Canvas.DrawImage(0, 0, Resources.background, false);

                if (MouseManager.X != LastCursorX || MouseManager.Y != LastCursorY)
                {
                    MouseMove?.Invoke();
                }

                DoInput();

                /*if (KeyboardManager.TryReadKey(out var key))
                {
                    if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed && key.Key == ConsoleKeyEx.Delete)
                    {
                        AddWindow(new TaskManager());
                    }
                }*//*

                // Regular windows
                for (int i = 0; i <= windows.Count - 1; i++)
                {
                    Window window = windows[i];
                    bool focused = i == windows.Count - 1;

                    window.HandleRun();

                    if (window.Visible && window.Title != nameof(Taskbar))
                    {
                        window.DrawWindow(Canvas, focused);
                    }
                }

                // Special windows (hard coded)
                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].Title == nameof(Taskbar))
                    {
                        windows[i].DrawWindow(Canvas, i == windows.Count - 1);
                    }
                }

                // move back up if it doesn't work
                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    if (windows[i].Closing)
                    {
                        TaskbarWindowRemovedHook?.Invoke(windows[i]);

                        if (windows[i].Title == "GoOS")
                            Dimmed = false;

                        windows.RemoveAt(i);

                        TaskmanHook?.Invoke();
                    }
                }

                DrawMouse();

                MouseToDraw = mouse;
                MouseOffsetX = 0;
                MouseOffsetY = 0;

                //Canvas.Update();

                MemoryWatch.Watch();

                LastCursorX = MouseManager.X;
                LastCursorY = MouseManager.Y;
                
                Canvas.Update();

                if (framesToHeapCollect == 0)
                {
                    Heap.Collect();
                    framesToHeapCollect = 10;
                }

                framesToHeapCollect--;
            }
            catch (Exception ex)
            {
                Dialogue.Show(
                    "Error",
                    ex.Message,
                    null, // default buttons
                    errorIcon);
            }
        }
    }*/
}