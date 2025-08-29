using System;
using Cosmos.System;
using Gold.Graphics;
using GoOS.GUI.Models;

namespace GoOS.GUI;

public abstract class Control
{
    public Action Clicked;
    public Action<string> ClickedAlt;

    public Canvas Contents;

    public string Name;

    public Window Parent;
    public bool RenderWithAlpha = false;
    public bool Visible = true;
    public ushort X, Y;

    public Control(Window parent, ushort x, ushort y, ushort width, ushort height)
    {
        Parent = parent;
        parent.Controls.Add(this);

        Contents = new Canvas(width, height);
        X = x;
        Y = y;
    }

    public bool IsMouseOver =>
        MouseManager.X >= Parent.X + X &&
        MouseManager.X < Parent.X + X + Contents.Width &&
        MouseManager.Y >= Parent.Y + Y + (Parent.HasTitlebar ? Window.TITLE_BAR_HEIGHT : 0) &&
        MouseManager.Y < Parent.Y + Y + Contents.Height +
        (Parent.HasTitlebar ? Window.TITLE_BAR_HEIGHT : 0);

    public abstract void Render();

    internal virtual void HandleDown(MouseEventArgs args)
    {
    }

    internal virtual void HandleRelease()
    {
    }

    internal virtual void HandleClick(MouseEventArgs args)
    {
        Clicked?.Invoke();
        ClickedAlt?.Invoke(Name);
    }

    internal virtual void HandleUnfocus()
    {
    }

    internal virtual void HandleKey(KeyEvent key)
    {
    }

    public virtual void Update()
    {
    }
}