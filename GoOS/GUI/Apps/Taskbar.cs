﻿using System;
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
    Button startButton;

    StartMenu startMenu;

    List<(Window window, Button button)> windowButtons = new(10);

    private const int windowButtonSpacing = 10;
    private const int windowButtonPadding = 10;

    private byte lastSecond = Cosmos.HAL.RTC.Second;

    public Taskbar()
    {
        Contents = new Canvas(WindowManager.Canvas.Width, 28);
        //so it displays at the bottom of the screen
        X = 0;
        Y = WindowManager.Canvas.Height - 28;
        Title = nameof(Taskbar);
        Visible = true;
        Closable = false;
        HasTitlebar = false;
        Unkillable = true;

        startMenu = new StartMenu();
        //WindowManager.AddWindow(startMenu);
        startButton = new Button(this, 0, 0, 74, 28, String.Empty)
        {
            Image = Resources.startBackground,
            BackgroundColour = Color.Transparent,
            UseSystemStyle = false,
            RenderWithAlpha = true,
        };
        startButton.Clicked = StartClicked;
        startButton.Render();

        RenderWindow();

        WindowManager.TaskbarWindowAddedHook = WindowAdded;
        WindowManager.TaskbarWindowRemovedHook = WindowRemoved;

        WindowManager.TaskbarFocusChangedHook = UpdateFocusIndication;
    }

    private void RenderWindow()
    {
        // Contents.Clear(Color.DeepGray);
        // Contents.DrawFilledRectangle(0, 0, WindowManager.Canvas.Width, 3, 0, Color.LightGray);

        //RenderOutsetWindowBackground(); // 3d
        Contents.DrawImage(0,0, Resources.taskbarBackground, false);

        RenderControls();

        RenderInformation();
    }
    
    private void StartClicked()
    {
        WindowManager.ToggleStartMenu();
    }

    private void WindowRemoved(Window window)
    {
        for (int i = 0; i < windowButtons.Count; i++)
        {
            var item = windowButtons[i];

            if (item.window == window)
            {
                // remove the window's button

                Controls.Remove(item.button);

                // shift the buttons to the right of it left
                for (int j = i; j < windowButtons.Count; j++)
                {
                    var buttonToBeShiftedLeft = windowButtons[j].button;

                    buttonToBeShiftedLeft.X -= (ushort)(item.button.Contents.Width + windowButtonSpacing);
                }
            }
        }

        UpdateFocusIndication();

        // refresh
        RenderWindow();
    }

    private void UpdateFocusIndication()
    {
        Window focusedWindow = WindowManager.FocusedWindow;

        foreach (var item in windowButtons)
        {
            item.button.AppearPressed = focusedWindow == item.window;
            item.button.Render();
        }
    }

    private void WindowAdded(Window window)
    {
        // Don't bother putting windows without 
        // titlebars in the taskbar
        if (!window.HasTitlebar)
        {
            return;
        }

        int width = (windowButtonPadding * 2) + Resources.Font_1x.MeasureString(window.Title);

        int x;
        if (windowButtons.Count > 0)
        {
            Button lastWindowButton = windowButtons[^1].button;
            x = lastWindowButton.X + lastWindowButton.Contents.Width + windowButtonSpacing;
        }
        else
        {
            x = startButton.X + startButton.Contents.Width + windowButtonSpacing;
        }

        Button button = new Button(this, (ushort)x, 4, (ushort)width, 20, window.Title);
        button.Render();

        windowButtons.Add((window, button));

        button.Clicked = () =>
        {
            WindowManager.MoveWindowToFront(window);
        };

        UpdateFocusIndication();

        // refresh
        RenderWindow();
    }

    public void HandleStartMenuOpen()
    {
        startButton.AppearPressed = true;
        startButton.Render();
        RenderControls();
    }

    public void HandleStartMenuClose()
    {
        startButton.AppearPressed = false;
        startButton.Render();
        RenderControls();
    }

    private void RenderInformation()
    {
        string timeString = DateTime.Now.ToString("HH:mm");
        int timeWidth = Resources.Font_1x.MeasureString(timeString);
        Contents.DrawString(Contents.Width - timeWidth - 3, 13, timeString, Resources.Font_1x, Color.White, true);

        string fpsString = $"{WindowManager.Canvas.GetFPS()} fps";
        int fpsWidth = Resources.Font_1x.MeasureString(fpsString);
        Contents.DrawString(Contents.Width - timeWidth - fpsWidth - 12, 13, fpsString, Resources.Font_1x, Color.White, true);
    }

    public override void HandleRun()
    {
        base.HandleRun();
        byte second = Cosmos.HAL.RTC.Second;
        if (second != lastSecond)
        {
            lastSecond = second;
            RenderWindow();
        }
    }
}
