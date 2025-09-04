using System;
using Cosmos.System;
using Gold.Graphics;
using GoOS.GUI.Models;

namespace GoOS.GUI;

public class ContextMenu : Window
{
    private static readonly Color MenuBackground = new(238, 238, 238);
    private static readonly Color MenuBorder = new(170, 170, 170);
    private static readonly Color MenuHighlight = new(205, 229, 252);
    private static readonly Color MenuText = Color.Black;
    private static readonly Color MenuSeparator = new(200, 200, 200);
    public Action<string> Handle;
    public string[] Items;
    private MouseState lastState = MouseState.None;
    private bool suppressNextOutsideRelease = true;

    public ContextMenu(string[] items, ushort width)
    {
        Items = items;
        WindowManager.MouseMove = MouseMove;

        Contents = new Canvas(width, Convert.ToUInt16(items.Length * 16 + 2));
        X = (int)MouseManager.X;
        Y = (int)MouseManager.Y;
        Title = "ContextMenu";
        Visible = true;
        Closable = false;
        HasTitlebar = false;

        Contents.Clear(MenuBackground);
        MouseMove();

        lastState = MouseManager.MouseState;
        suppressNextOutsideRelease = true;
    }

    public ContextMenu(string[] items, ushort width, int x, int y)
    {
        Items = items;
        WindowManager.MouseMove = MouseMove;

        Contents = new Canvas(width, Convert.ToUInt16(items.Length * 16 + 2));
        X = x;
        Y = y;
        Title = "ContextMenu";
        Visible = true;
        Closable = false;
        HasTitlebar = false;

        Contents.Clear(MenuBackground);
        MouseMove();

        lastState = MouseManager.MouseState;
        suppressNextOutsideRelease = true;
    }

    public static ContextMenu Show(string[] items, ushort width, Action<string> handle)
    {
        WindowManager.RemoveWindowByTitle("ContextMenu");

        var cm = new ContextMenu(items, width) { Handle = handle };
        WindowManager.AddWindow(cm);
        return cm;
    }

    public static ContextMenu ShowAt(int x, int y, string[] items, ushort width, Action<string> handle)
    {
        WindowManager.RemoveWindowByTitle("ContextMenu");

        var cm = new ContextMenu(items, width, x, y) { Handle = handle };
        WindowManager.AddWindow(cm);
        return cm;
    }

    private void MouseMove()
    {
        Contents.Clear(MenuBackground);

        for (var i = 0; i < Items.Length; i++)
            if (Items[i] == "----" || Items[i].StartsWith(" -"))
            {
                Contents.DrawLine(4, i * 16 + 8, Contents.Width - 4, i * 16 + 8, MenuSeparator);
            }
            else
            {
                var isHovered = IsMouseOver &&
                                MouseManager.Y >= Y + i * 16 &&
                                MouseManager.Y < Y + (i + 1) * 16;

                if (isHovered) Contents.DrawFilledRectangle(0, i * 16, Contents.Width, 16, 0, MenuHighlight);

                Contents.DrawString(4, i * 16 + 2, Items[i], Resources.Font_1x, MenuText);
            }

        Contents.DrawRectangle(0, 0, (ushort)(Contents.Width - 1), (ushort)(Contents.Height - 1), 0, MenuBorder);
    }

    public override void HandleRun()
    {
        base.HandleRun();

        var cur = MouseManager.MouseState;

        if (lastState == MouseState.Left && cur == MouseState.None)
        {
            if (suppressNextOutsideRelease)
            {
                suppressNextOutsideRelease = false;
            }
            else
            {
                if (!IsMouseOver)
                {
                    Dispose();
                    return;
                }
            }
        }

        lastState = cur;
    }

    public override void HandleClick(MouseEventArgs e)
    {
        if (e.MouseState == MouseState.Left)
        {
            if (IsMouseOver)
            {
                var index = (int)((MouseManager.Y - Y) / 16);
                if (index >= 0 && index < Items.Length)
                    if (Items[index] != "----" && !Items[index].StartsWith(" -"))
                    {
                        var localHandle = Handle;
                        var selectedItem = Items[index];

                        Dispose();

                        localHandle?.Invoke(selectedItem);
                        return;
                    }
            }

            Dispose();
        }
    }
}