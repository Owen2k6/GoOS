using System;
using System.Collections.Generic;
using GoGL.Graphics;

namespace GoOS.GUI.Apps.Settings;

public class Frame : Window
{
    List<Button> sButtons = new();
    private const int buttonHeight = 24;
    private int page = 0;

    public Frame()
    {
        // Create the window.
        Contents = new Canvas(270, 285);
        Title = "Settings";
        Visible = true;
        Closable = true;
        Sizable = false;
        SetDock(WindowDock.Auto);
        // Paint the window.
        ReDraw();
    }
    
    public void ReDraw()
    {
        Contents.Clear(Color.LightGray);
        Draw();
        AddSideButtons();
        foreach (Control control in Controls)
        {
            control.Render();
        }
    }

    public void Draw()
    {
        Contents.DrawImage(0, 0, Resources.SBG, false);
        switch (page)
        {
            case (0):

                break;
        }
    }

    public void DrawSideBar(string name, Action clickedAction)
    {
        sButtons.Add(new Button(this, 0,
            (ushort)((sButtons.Count * buttonHeight)), 96, buttonHeight, name)
        {
            Clicked = clickedAction
        });
    }

    private void AddSideButtons()
    {
        DrawSideBar("General", () =>
        {
            page = 0;
        });
        DrawSideBar("Display", () =>
        {
            page = 1;
        });
        DrawSideBar("GoStore", () =>
        {
            page = 2;
        });
        DrawSideBar("Applications", () =>
        {
            page = 3;
        });
        DrawSideBar("Security", () =>
        {
            page = 4;
        });
    }
}