using System;
using System.Collections.Generic;
using Cosmos.Core.Memory;
using Cosmos.System;
using GoOS.GUI.Apps;
using IL2CPU.API.Attribs;
using GoGL.Graphics;
using GoGL.Graphics.Fonts;
using GoGL.Hardware.GPU;

namespace GoOS.GUI
{
    public class WindowManager
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

                else if (KeyboardManager.ShiftPressed && key.Key == ConsoleKeyEx.F10)
                {
                    AddWindow(new GTerm());
                }

                focusedWindow.HandleKey(key);
            }
        }

        public static void CloseAll()
        {
            windows.Clear();
        }

        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.error.bmp")]
        private static byte[] errorIconRaw;

        public static Canvas errorIcon = Image.FromBitmap(errorIconRaw, false);

        private static uint LastCursorX, LastCursorY;

        public static void Update()
        {
            try
            {
                if (!BetterConsole.ConsoleMode)
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
                    }*/

                    // Regular windows
                    for (int i = 0; i <= windows.Count - 1; i++)
                    {
                        Window window = windows[i];
                        bool focused = i == windows.Count - 1;

                        window.HandleRun();

                        if (focused && Dimmed)
                            DimBackground();

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

                    Canvas.Update();

                    MemoryWatch.Watch();

                    LastCursorX = MouseManager.X;
                    LastCursorY = MouseManager.Y;
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
            catch (Exception ex)
            {
                Dialogue.Show(
                    "Error",
                    ex.Message,
                    null, // default buttons
                    errorIcon);
            }
        }

        private static void DimBackground()
        {
            for (int y = 0; y < Canvas.Height - 1; y++)
            {
                for (int x = 0; x < Canvas.Width - 1; x++)
                {
                    if ((x % 2) == 0)
                    {
                        Canvas[x + y % 2, y] = Color.Black;
                    }
                }
            }
        }
    }
}