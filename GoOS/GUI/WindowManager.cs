using System;
using System.Collections.Generic;
using Cosmos.Core.Memory;
using Cosmos.System;
using Gold.Graphics;
using Gold.Hardware.GPU;
using GoOS.GUI.Apps;
using IL2CPU.API.Attribs;

namespace GoOS.GUI;

public class WindowManager
{
    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.mouse.bmp")]
    private static byte[] mouseRaw;

    public static Canvas mouse = Image.FromBitmap(mouseRaw);

    private static int framesToHeapCollect = 10;

    private static bool MouseDrawn = false;

    public static Canvas MouseToDraw = mouse;

    public static int MouseOffsetX, MouseOffsetY;

    public static List<Window> windows = new(10);

    public static Display Canvas;

    public static bool Dimmed;

    internal static Action<Window> TaskbarWindowAddedHook;

    internal static Action<Window> TaskbarWindowRemovedHook;

    internal static Action TaskmanHook;

    internal static Action TaskbarFocusChangedHook;

    public static bool IsInOOBE = false;

    public static Action MouseMove;

    private static bool startOpen = false;

    [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.error.bmp")]
    private static byte[] errorIconRaw;

    public static Canvas errorIcon = Image.FromBitmap(errorIconRaw);

    private static uint LastCursorX, LastCursorY;

    public static Window FocusedWindow
    {
        get
        {
            if (windows.Count < 1) return null;

            return windows[^1];
        }
    }

    public static bool AreThereDraggingWindows => GetDraggingWindow() != null;

    public static Window GetWindowByTitle(string wnd)
    {
        foreach (var w in windows)
            if (w.Title == wnd)
                return w;

        return null;
    }

    public static bool AreThereWindowsInRange(int StartX, int StartY, int EndX, int EndY)
    {
        foreach (var w in windows)
            if (w is not Desktop && w.X > StartX && w.X < EndX && w.Y > StartY && w.Y < EndY)
                return true;

        return false;
    }

    public static int GetAmountOfWindowsByTitle(string wnd)
    {
        var count = 0;

        foreach (var w in windows)
            if (w.Title == wnd)
                count++;

        return count;
    }

    public static bool ContainsWindowByTitle(string wnd)
    {
        return GetAmountOfWindowsByTitle(wnd) > 0;
    }

    public static bool ContainsAWindow(List<Window> wnds)
    {
        foreach (var w in wnds)
            if (windows.Contains(w))
                return true;

        return false;
    }

    public static void RemoveWindowByTitle(string wnd)
    {
        foreach (var w in windows)
            if (w.Title == wnd)
                w.Dispose();
    }

    public static void AddWindow(Window window)
    {
        window.Paint();
        windows.Add(window);
        TaskmanHook?.Invoke();
        TaskbarWindowAddedHook?.Invoke(window);
    }

    public static T GetWindowByType<T>()
    {
        foreach (var window in windows)
            if (window is T winOfT)
                return winOfT;

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
        for (var i = windows.Count - 1; i >= 0; i--)
            if (windows[i].IsMouseOver && windows[i].Visible)
                return i;

        return -1;
    }

    public static Window GetDraggingWindow()
    {
        foreach (var window in windows)
            if (window.Dragging)
                return window;

        return null;
    }

    private static void DrawMouse()
    {
        var drawX = (int)MouseManager.X - MouseOffsetX;
        var drawY = (int)MouseManager.Y - MouseOffsetY;

        if (drawX < -MouseToDraw.Width) drawX = -MouseToDraw.Width;
        if (drawY < -MouseToDraw.Height) drawY = -MouseToDraw.Height;
        var maxX = Canvas.Width - 1;
        var maxY = Canvas.Height - 1;
        if (drawX > maxX) drawX = maxX;
        if (drawY > maxY) drawY = maxY;

        // enable alpha
        Canvas.DrawImage(drawX, drawY, MouseToDraw);
    }

    private static void SyncMouseBoundsAndClamp()
    {
        // Ensure MouseManager knows the real screen size (only when it changes)
        if (MouseManager.ScreenWidth != Canvas.Width || MouseManager.ScreenHeight != Canvas.Height)
        {
            MouseManager.ScreenWidth = Canvas.Width;
            MouseManager.ScreenHeight = Canvas.Height;

            // If Cosmos didn’t clamp internally, keep X/Y sane
            if (MouseManager.X >= MouseManager.ScreenWidth) MouseManager.X = MouseManager.ScreenWidth - 1;
            if (MouseManager.Y >= MouseManager.ScreenHeight) MouseManager.Y = MouseManager.ScreenHeight - 1;
        }
    }


    private static void AltTab()
    {
        var tabbableWindows = new List<Window>();
        foreach (var window in windows)
            if (window.HasTitlebar)
                tabbableWindows.Add(window);

        if (tabbableWindows.Count < 2) return;

        var tabIndex = tabbableWindows.IndexOf(windows[^1]);
        if (tabIndex == -1) tabIndex = 0;

        tabIndex = (tabIndex + 1) % tabbableWindows.Count;

        MoveWindowToFront(tabbableWindows[tabIndex]);
    }

    private static void DoInput()
    {
        if (windows.Count == 0) return;

        var draggingWindow = GetDraggingWindow();
        if (draggingWindow != null)
        {
            if (!Dimmed &&
                windows.IndexOf(draggingWindow) != windows.Count - 1)
                MoveWindowToFront(draggingWindow);

            draggingWindow.HandleMouseInput();

            return;
        }

        for (var i = 0; i < windows.Count; i++)
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

        var hoveredWindowIdx = GetHoveredWindow();
        if (hoveredWindowIdx != -1)
        {
            if (windows[hoveredWindowIdx].Title != nameof(Desktop))
                windows[hoveredWindowIdx].HandleMouseInput();

            if (hoveredWindowIdx != windows.Count - 1 &&
                MouseManager.MouseState != MouseState.None &&
                !Dimmed &&
                windows[hoveredWindowIdx].Title != nameof(Desktop))
                MoveWindowToFront(windows[hoveredWindowIdx]);
        }

        var focusedWindow = windows[windows.Count - 1];

        var keyPressed = KeyboardManager.TryReadKey(out var key);
        if (keyPressed)
        {
            if ((key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt &&
                key.Key == ConsoleKeyEx.Tab &&
                !Dimmed)
            {
                AltTab();
                return;
            }

            if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed && key.Key == ConsoleKeyEx.Delete)
                AddWindow(new TaskManager());

            else if (KeyboardManager.ShiftPressed && key.Key == ConsoleKeyEx.F10) AddWindow(new GTerm());

            focusedWindow.HandleKey(key);
        }
    }

    public static void CloseAll()
    {
        windows.Clear();
    }

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

            if (MouseManager.X != LastCursorX || MouseManager.Y != LastCursorY) MouseMove?.Invoke();

            DoInput();

            /*if (KeyboardManager.TryReadKey(out var key))
            {
                if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed && key.Key == ConsoleKeyEx.Delete)
                {
                    AddWindow(new TaskManager());
                }
            }*/

            // Regular windows
            for (var i = 0; i <= windows.Count - 1; i++)
            {
                var window = windows[i];
                var focused = i == windows.Count - 1;

                window.HandleRun();

                if (focused && Dimmed)
                    DimBackground();

                if (window.Visible && window.Title != nameof(Menubar)) window.DrawWindow(Canvas, focused);
            }

            // Special windows (hard coded)
            for (var i = 0; i < windows.Count; i++)
                if (windows[i].Title == nameof(Menubar))
                    windows[i].DrawWindow(Canvas, i == windows.Count - 1);

            // move back up if it doesn't work
            for (var i = windows.Count - 1; i >= 0; i--)
                if (windows[i].Closing)
                {
                    TaskbarWindowRemovedHook?.Invoke(windows[i]);

                    if (windows[i].Title == "GoOS")
                        Dimmed = false;

                    windows.RemoveAt(i);

                    TaskmanHook?.Invoke();
                }

            DrawMouse();

            MouseToDraw = mouse;
            MouseOffsetX = 0;
            MouseOffsetY = 0;

            Canvas.Update();

            MemoryWatch.Watch();

            LastCursorX = MouseManager.X;
            LastCursorY = MouseManager.Y;
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
        for (var y = 0; y < Canvas.Height - 1; y++)
        for (var x = 0; x < Canvas.Width - 1; x++)
            if (x % 2 == 0)
                Canvas[x + y % 2, y] = Color.Black;
    }
}