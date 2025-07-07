using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.HAL.Drivers.Video;
using IL2CPU.API.Attribs;
using GoGL.Graphics;

namespace GoOS.GUI.Apps;

public class Taskbar : Window
{
    private Button startButton;
    private StartMenu startMenu;
    private List<(Window window, Button button)> windowButtons = new(10);

    private const int windowButtonSpacing = 10;
    private const int windowButtonPadding = 10;

    private byte lastSecond = Cosmos.HAL.RTC.Second;
    private int timeAreaWidth = 0;
    private bool needsRedraw = true;

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

        InitialiseStartMenu();
        InitialiseHooks();

        RenderWindow();
    }

    private void InitialiseStartMenu()
    {
        startMenu = new StartMenu();
        startButton = new Button(this, 0, 0, 74, 28, String.Empty)
        {
            Image = Resources.startBackground,
            BackgroundColour = Color.Transparent,
            UseSystemStyle = false,
            RenderWithAlpha = true,
        };
        startButton.Clicked = StartClicked;
        startButton.Render();
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

        Contents.DrawImage(0, 0, Resources.taskbarBackground, false);
        RenderControls();
        RenderInformation();

        needsRedraw = false;
    }

    private void StartClicked()
    {
        WindowManager.ToggleStartMenu();
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
        RenderWindow();
    }

    private void UpdateFocusIndication()
    {
        Window focusedWindow = WindowManager.FocusedWindow;
        bool anyChanges = false;

        foreach (var item in windowButtons)
        {
            bool shouldBePressed = focusedWindow == item.window;
            if (item.button.AppearPressed != shouldBePressed)
            {
                item.button.AppearPressed = shouldBePressed;
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
        if (!window.HasTitlebar)
        {
            return;
        }

        // Calculate button dimensions
        int width = (windowButtonPadding * 2) + Resources.Font_1x.MeasureString(window.Title);
        int x = CalculateNewButtonX(width);

        // Create and render button
        Button button = new Button(this, (ushort)x, 4, (ushort)width, 20, window.Title);
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

        return startButton.X + startButton.Contents.Width + windowButtonSpacing;
    }

    public void HandleStartMenuOpen()
    {
        startButton.AppearPressed = true;
        startButton.Render();
        RenderControls();
        needsRedraw = true;
    }

    public void HandleStartMenuClose()
    {
        startButton.AppearPressed = false;
        startButton.Render();
        RenderControls();
        needsRedraw = true;
    }

    private void RenderInformation()
    {
        DateTime now = DateTime.Now;
        string timeString = now.ToString("HH:mm");
        string dateString = now.ToString("dd/MM/yyyy");

        int timeWidth = Resources.Font_1x.MeasureString(timeString);
        int dateWidth = Resources.Font_1x.MeasureString(dateString);

        int rightMargin = 3;
        int timeX = Contents.Width - timeWidth - rightMargin - 12 + 5+ 3;

        int timeCenterX = timeX + (timeWidth);
        int dateX = timeCenterX - (dateWidth / 2) + 5;

        Contents.DrawString(timeX, 9, timeString, Resources.Font_1x, Color.White, true);
        Contents.DrawString(dateX, 21, dateString, Resources.Font_1x, Color.White, true);

        string fpsString = $"{WindowManager.Canvas.GetFPS()} fps";
        int fpsWidth = Resources.Font_1x.MeasureString(fpsString);
        int fpsX = timeX - fpsWidth - 24;
        Contents.DrawString(fpsX, 15, fpsString, Resources.Font_1x, Color.White, true);

        timeAreaWidth = Math.Max(timeWidth, dateWidth) + fpsWidth + 15;
    }

    public override void HandleRun()
    {
        base.HandleRun();

        // Only redraw when time changes (once per second)
        byte currentSecond = Cosmos.HAL.RTC.Second;
        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;
            needsRedraw = true;
            RenderWindow();
        }
    }
}