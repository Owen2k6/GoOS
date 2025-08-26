using System.Collections.Generic;
using Cosmos.System;
using Gold.Graphics;

namespace GoOS.GUI.Apps;

public class Taskbar : Window
{
    private List<(Window window, Button button)> windowButtons = new(10);

    private const int windowButtonSpacing = 10;
    private const int windowButtonPadding = 10;

    private byte lastSecond = Cosmos.HAL.RTC.Second;
    private int timeAreaWidth = 0;
    private bool needsRedraw = true;

    public Taskbar() : base(0, WindowManager.Screen.Height - 28, WindowManager.Screen.Width, 28, "taskbar", true)
    {
        //Visible = true;
        //Closable = false;
        //HasTitlebar = false;
        //Unkillable = true;
        //InitialiseHooks();

        Render();
    }

    /*private void InitialiseHooks()
    {
        WindowManager.TaskbarWindowAddedHook = WindowAdded;
        WindowManager.TaskbarWindowRemovedHook = WindowRemoved;
        WindowManager.TaskbarFocusChangedHook = UpdateFocusIndication;
    }*/

    internal override void Render()
    {
        if (!needsRedraw) return;
        Contents.Clear(new Color(0x00FFFFFF));

        DrawBackgroundTiled();   // ⬅️ tile the 1px-wide resource across the width
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
            for (int x = 0; x < Contents.Width; x++)
                Contents.DrawImage(x, 0, tile, true);
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
        int indexToRemove = -1;
        Button removedButton = null;

        // Find the button to remove
        for (int i = 0; i < windowButtons.Count; i++)
        {
            if (windowButtons[i].window == window)
            {
                indexToRemove = i;
                removedButton = windowButtons[i].button;
                break;
            }
        }

        if (indexToRemove == -1) return;

        // Remove the button
        Controls.Remove(removedButton);
        int buttonWidth = removedButton.Contents.Width;
        windowButtons.RemoveAt(indexToRemove);

        // Shift remaining buttons
        for (int i = indexToRemove; i < windowButtons.Count; i++)
        {
            var button = windowButtons[i].button;
            button.X -= (ushort)(buttonWidth + windowButtonSpacing);
        }

        UpdateFocusIndication();
        needsRedraw = true;
        Render();
    }

    private void UpdateFocusIndication()
    {
        Window focusedWindow = WindowManager.FocusedWindow;
        bool anyChanges = false;

        foreach (var item in windowButtons)
        {
            bool shouldBePressed = focusedWindow == item.window;
            if (item.button.Pressed != shouldBePressed)
            {
                item.button.Pressed = shouldBePressed;
                item.button.Render();
                anyChanges = true;
            }
        }

        if (anyChanges)
        {
            needsRedraw = true;
        }
    }

    private void WindowAdded(Window window)
    {
        // Don't add windows without titlebars to the taskbar
        if (!window.Borderless)
        {
            return;
        }

        // Calculate button dimensions
        int width = (windowButtonPadding * 2) + Resources.Font_1x.MeasureString(window.Title);
        int x = CalculateNewButtonX(width);

        // Create and render button
        Button button = new Button(this, (ushort)x, 4, (ushort)width, 20, window.Title);
        button.Clicked = () => WindowManager.FocusedWindow = window;
        button.Render();

        // Add to collection
        windowButtons.Add((window, button));

        UpdateFocusIndication();
        needsRedraw = true;
        Render();
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

    internal override void HandleRun()
    {
        base.HandleRun();

        // Only redraw when time changes (once per second)
        byte currentSecond = Cosmos.HAL.RTC.Second;
        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            needsRedraw = true;
            Render();
        }
    }
}
