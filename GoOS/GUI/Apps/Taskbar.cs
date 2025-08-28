using System.Collections.Generic;
using Cosmos.HAL;
using GoGL.Graphics;

namespace GoOS.GUI.Apps;

public class Taskbar : Window
{
    private const int windowButtonSpacing = 10;
    private const int windowButtonPadding = 10;
    private readonly List<(Window window, Button button)> windowButtons = new(10);

    private byte lastSecond = RTC.Second;
    private bool needsRedraw = true;
    private int timeAreaWidth = 0;

    public Taskbar()
    {
        Contents = new Canvas(WindowManager.Canvas.Width, 28);
        X = 0;
        Y = WindowManager.Canvas.Height - 28;
        Title = nameof(Taskbar);
        Visible = true;
        Closable = false;
        HasTitlebar = false;
        Unkillable = true;
        InitialiseHooks();

        RenderWindow();
    }

    private void InitialiseHooks()
    {
        WindowManager.TaskbarWindowAddedHook = WindowAdded;
        WindowManager.TaskbarWindowRemovedHook = WindowRemoved;
        WindowManager.TaskbarFocusChangedHook = UpdateFocusIndication;
    }

    private void RenderWindow()
    {
        if (!needsRedraw) return;

        DrawBackgroundTiled(); // ⬅️ tile the 1px-wide resource across the width
        RenderControls();

        needsRedraw = false;
    }

    // --- NEW: tile the taskbar background exactly like the menubar ---
    private void DrawBackgroundTiled()
    {
        var tile = Resources.taskbarBackground;

        // If it’s the expected 1×barHeight strip, tile it horizontally
        if (tile != null && tile.Width == 1 && tile.Height == Contents.Height)
        {
            for (var x = 0; x < Contents.Width; x++)
                Contents.DrawImage(x, 0, tile, false);
        }
        else
        {
            // Fallback: original single draw (handles legacy full-width bitmap)
            if (tile != null)
                Contents.DrawImage(0, 0, tile, false);
        }
    }

    private void WindowRemoved(Window window)
    {
        var indexToRemove = -1;
        Button removedButton = null;

        // Find the button to remove
        for (var i = 0; i < windowButtons.Count; i++)
            if (windowButtons[i].window == window)
            {
                indexToRemove = i;
                removedButton = windowButtons[i].button;
                break;
            }

        if (indexToRemove == -1) return;

        // Remove the button
        Controls.Remove(removedButton);
        int buttonWidth = removedButton.Contents.Width;
        windowButtons.RemoveAt(indexToRemove);

        // Shift remaining buttons
        for (var i = indexToRemove; i < windowButtons.Count; i++)
        {
            var button = windowButtons[i].button;
            button.X -= (ushort)(buttonWidth + windowButtonSpacing);
        }

        UpdateFocusIndication();
        needsRedraw = true;
        RenderWindow();
    }

    private void UpdateFocusIndication()
    {
        var focusedWindow = WindowManager.FocusedWindow;
        var anyChanges = false;

        foreach (var item in windowButtons)
        {
            var shouldBePressed = focusedWindow == item.window;
            if (item.button.AppearPressed != shouldBePressed)
            {
                item.button.AppearPressed = shouldBePressed;
                item.button.Render();
                anyChanges = true;
            }
        }

        if (anyChanges) needsRedraw = true;
    }

    private void WindowAdded(Window window)
    {
        // Don't add windows without titlebars to the taskbar
        if (!window.HasTitlebar) return;

        // Calculate button dimensions
        var width = windowButtonPadding * 2 + Resources.Font_1x.MeasureString(window.Title);
        var x = CalculateNewButtonX(width);

        // Create and render button
        var button = new Button(this, (ushort)x, 4, (ushort)width, 20, window.Title);
        button.Clicked = () => WindowManager.MoveWindowToFront(window);
        button.Render();

        // Add to collection
        windowButtons.Add((window, button));

        UpdateFocusIndication();
        needsRedraw = true;
        RenderWindow();
    }

    private int CalculateNewButtonX(int buttonWidth)
    {
        if (windowButtons.Count > 0)
        {
            var lastButton = windowButtons[^1].button;
            return lastButton.X + lastButton.Contents.Width + windowButtonSpacing;
        }

        return 5 + 5 + windowButtonSpacing;
    }

    public override void HandleRun()
    {
        base.HandleRun();

        // Only redraw when time changes (once per second)
        var currentSecond = RTC.Second;
        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            needsRedraw = true;
            RenderWindow();
        }
    }
}