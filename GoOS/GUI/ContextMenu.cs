using System;
using Cosmos.System;
using Gold.Graphics;
using GoOS.GUI.Models;

namespace GoOS.GUI;

public class ContextMenu : Window
{
    public Action<string> Handle;
    public string[] Items;
    private MouseState lastState = MouseState.None;
    private bool suppressNextOutsideRelease = true;
    
    // Mac OS 9 style colours
    private static readonly Color MenuBackground = new Color(238, 238, 238);
    private static readonly Color MenuBorder = new Color(170, 170, 170);
    private static readonly Color MenuHighlight = new Color(205, 229, 252);
    private static readonly Color MenuText = Color.Black;
    private static readonly Color MenuSeparator = new Color(200, 200, 200);

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
        // Clear any existing context menu
        WindowManager.RemoveWindowByTitle("ContextMenu");
        
        var cm = new ContextMenu(items, width) { Handle = handle };
        WindowManager.AddWindow(cm);
        return cm;
    }

    public static ContextMenu ShowAt(int x, int y, string[] items, ushort width, Action<string> handle)
    {
        // Clear any existing context menu
        WindowManager.RemoveWindowByTitle("ContextMenu");
        
        var cm = new ContextMenu(items, width, x, y) { Handle = handle };
        WindowManager.AddWindow(cm);
        return cm;
    }

    private void MouseMove()
    {
        Contents.Clear(MenuBackground);
        
        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i] == "----" || Items[i].StartsWith(" -"))
            {
                // Draw separator
                Contents.DrawLine(4, i * 16 + 8, Contents.Width - 4, i * 16 + 8, MenuSeparator);
            }
            else
            {
                // Check if mouse is over this item
                bool isHovered = IsMouseOver && 
                    MouseManager.Y >= Y + i * 16 && 
                    MouseManager.Y < Y + (i + 1) * 16;

                // Draw highlight if needed
                if (isHovered)
                {
                    Contents.DrawFilledRectangle(0, i * 16, Contents.Width, 16, 0, MenuHighlight);
                }
                
                // Draw the text
                Contents.DrawString(4, i * 16 + 2, Items[i], Resources.Font_1x, MenuText);
            }
        }
        
        // Draw a simple border
        Contents.DrawRectangle(0, 0, (ushort)(Contents.Width - 1), (ushort)(Contents.Height - 1),0, MenuBorder);
    }

    public override void HandleRun()
    {
        base.HandleRun();

        var cur = MouseManager.MouseState;
        
        // Handle mouse release
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
                {
                    if (Items[index] != "----" && !Items[index].StartsWith(" -"))
                    {
                        // Copy the handle and item to local variables
                        var localHandle = Handle;
                        var selectedItem = Items[index];
                        
                        // Dispose first to prevent focus issues
                        Dispose();
                        
                        // Then invoke the handler with the selected item
                        localHandle?.Invoke(selectedItem);
                        return;
                    }
                }
            }
            
            Dispose();
        }
    }
}