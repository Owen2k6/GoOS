using System;
using System.Collections.Generic;
using Cosmos.System;
using Gold.Graphics;
using GoOS.GUI.Models;
using static GoOS.Resources;

namespace GoOS.GUI;

public class Window
{
    public const int TITLE_BAR_HEIGHT = 22;
    private const int FRAME_L = 6, FRAME_R = 6, FRAME_B = 7;
    private static readonly Color TitleBandFill = new(0xFFDADADA);
    private static readonly Color TitleStripeLight = Color.White;
    private static readonly Color TitleStripeDark = new(0xFF969696);
    private static readonly Color TitleTextColour = Color.Black;
    private static readonly Color OuterTL = Color.White;
    private static readonly Color OuterBR = new(0xFFB3B3B3);
    private static readonly Color ContentFill = new(0xFFE7E7E7);
    private static readonly Color PlatFace = new(0xFFE7E7E7);
    public bool Closable;
    public bool Closing;
    public bool Collapsed;
    public Canvas Contents;
    public List<Control> Controls = new();
    private Control downOnControl;
    public bool Dragging;
    private int DragStartX, DragStartY, DragStartMouseX, DragStartMouseY;
    public Control FocusedControl;
    public bool HasTitlebar = true;

    private bool IsHandling;
    private MouseState previousMouseState = MouseState.None;
    public string Title;
    public bool Unkillable = false;
    public bool Visible;
    private bool wasDown;
    public int X = 50, Y = 50;

    private int OuterWidth
        => (Contents != null ? Contents.Width : 0) + (HasTitlebar ? FRAME_L + FRAME_R : 0);

    private int OuterHeight
        => !HasTitlebar
            ? Contents != null ? Contents.Height : 0
            : Collapsed
                ? TITLE_BAR_HEIGHT
                : Contents != null
                    ? TITLE_BAR_HEIGHT + FRAME_B + Contents.Height
                    : TITLE_BAR_HEIGHT;

    private int ContentLeft => HasTitlebar ? X + FRAME_L : X;
    private int ContentTop => HasTitlebar ? Y + TITLE_BAR_HEIGHT : Y;

    public bool Focused => WindowManager.windows[WindowManager.windows.Count - 1] == this;

    public int RelativeMouseX => (int)MouseManager.X - ContentLeft;
    public int RelativeMouseY => (int)MouseManager.Y - ContentTop;

    public bool IsMouseOver
    {
        get
        {
            if (Contents == null && !HasTitlebar) return false;
            return MouseManager.X >= X && MouseManager.X < X + OuterWidth &&
                   MouseManager.Y >= Y && MouseManager.Y < Y + OuterHeight;
        }
    }

    public bool IsMouseOverContent
    {
        get
        {
            if (Contents == null || Collapsed) return false;
            return MouseManager.X >= ContentLeft && MouseManager.X < ContentLeft + Contents.Width &&
                   MouseManager.Y >= ContentTop && MouseManager.Y < ContentTop + Contents.Height;
        }
    }

    public bool IsMouseOverTitleBar
    {
        get
        {
            if (!HasTitlebar) return false;
            return MouseManager.X >= X && MouseManager.X < X + OuterWidth &&
                   MouseManager.Y >= Y && MouseManager.Y < Y + TITLE_BAR_HEIGHT;
        }
    }


    public bool IsMouseOverCloseButton
    {
        get
        {
            if (!HasTitlebar) return false;
            int cx, sx, by;
            GetButtonRects(out cx, out sx, out by);
            int px = (int)MouseManager.X, py = (int)MouseManager.Y;
            return px >= cx && px < cx + 13 && py >= by && py < by + 13;
        }
    }

    public bool IsMouseOverMinimizeButton
    {
        get
        {
            if (!HasTitlebar) return false;
            int cx, sx, by;
            GetButtonRects(out cx, out sx, out by);
            int px = (int)MouseManager.X, py = (int)MouseManager.Y;
            return px >= sx && px < sx + 13 && py >= by && py < by + 13;
        }
    }

    public virtual void HandleRun()
    {
        if (!Collapsed)
            foreach (var control in Controls)
                control.Update();

        if (wasDown && MouseManager.MouseState == MouseState.None)
        {
            wasDown = false;
            HandleRelease(new MouseEventArgs
                { X = RelativeMouseX, Y = RelativeMouseY, MouseState = previousMouseState });
            downOnControl?.HandleRelease();
            downOnControl = null;
        }
    }

    public virtual void RenderControls()
    {
        if (Collapsed) return;
        foreach (var c in Controls)
            Contents.DrawImage(c.X, c.Y, c.Contents, c.RenderWithAlpha);
    }

    private void GetButtonRects(out int closeX, out int shadeX, out int btnY)
    {
        const int BTN_MARGIN = 4;
        const int BTN_SIZE = 13;
        closeX = X + BTN_MARGIN;
        shadeX = X + OuterWidth - BTN_MARGIN - BTN_SIZE;
        btnY = Y + 4;
    }

    private Control GetHoveredControl()
    {
        if (Collapsed) return null;
        foreach (var control in Controls)
            if (control.IsMouseOver)
                return control;
        return null;
    }

    internal void HandleMouseInput()
    {
        if (IsHandling) return;
        IsHandling = true;
        if (HasTitlebar && Closable &&
            IsMouseOverCloseButton &&
            MouseManager.MouseState == MouseState.None &&
            previousMouseState == MouseState.Left)
            Closing = true;
        if (HasTitlebar &&
            IsMouseOverMinimizeButton &&
            MouseManager.MouseState == MouseState.None &&
            previousMouseState == MouseState.Left)
            Collapsed = !Collapsed;
        if (HasTitlebar &&
            IsMouseOverTitleBar &&
            !IsMouseOverCloseButton &&
            !IsMouseOverMinimizeButton &&
            MouseManager.MouseState == MouseState.Left &&
            previousMouseState == MouseState.None)
        {
            DragStartX = X;
            DragStartY = Y;
            DragStartMouseX = (int)MouseManager.X;
            DragStartMouseY = (int)MouseManager.Y;
            Dragging = true;
        }

        if (MouseManager.MouseState == MouseState.None)
            Dragging = false;
        if (IsMouseOver &&
            MouseManager.MouseState == MouseState.None &&
            previousMouseState == MouseState.Right)
            ShowContextMenu();

        if (Dragging)
        {
            X = DragStartX + ((int)MouseManager.X - DragStartMouseX);
            Y = DragStartY + ((int)MouseManager.Y - DragStartMouseY);
            if (WindowManager.Canvas != null)
            {
                var clamp = WindowManager.Canvas.Height - 28;
                if (Y >= clamp) Y = clamp - 1;
            }
        }

        var hovered = GetHoveredControl();
        if (!Collapsed && MouseManager.MouseState != MouseState.None && previousMouseState == MouseState.None)
        {
            wasDown = true;
            downOnControl = hovered;

            HandleDown(new MouseEventArgs
                { X = RelativeMouseX, Y = RelativeMouseY, MouseState = MouseManager.MouseState });

            if (MouseManager.MouseState == MouseState.Left)
            {
                FocusedControl = hovered;
                hovered?.HandleDown(new MouseEventArgs
                {
                    X = RelativeMouseX - hovered.X,
                    Y = RelativeMouseY - hovered.Y,
                    MouseState = MouseManager.MouseState
                });
            }
        }

        if (!Collapsed && MouseManager.MouseState == MouseState.None && previousMouseState == MouseState.Left)
        {
            HandleClick(new MouseEventArgs
                { X = RelativeMouseX, Y = RelativeMouseY, MouseState = previousMouseState });

            FocusedControl = hovered;

            hovered?.HandleClick(new MouseEventArgs
            {
                X = RelativeMouseX - hovered.X,
                Y = RelativeMouseY - hovered.Y,
                MouseState = MouseManager.MouseState
            });

            foreach (var c in Controls)
                if (c != hovered)
                    c.HandleUnfocus();
        }

        previousMouseState = MouseManager.MouseState;
        IsHandling = false;
    }

    public void DrawWindow(Canvas cv, bool focused)
    {
        if (!HasTitlebar)
        {
            if (!Collapsed && Contents != null)
                cv.DrawImage(X, Y, Contents, false);
            return;
        }

        DrawDropShadow(cv);
        cv.DrawFilledRectangle(X, Y, (ushort)OuterWidth, (ushort)OuterHeight, 0, TitleBandFill);
        cv.DrawRectangle(X, Y, (ushort)OuterWidth, (ushort)OuterHeight, 0, Color.Black);
        cv.DrawLine(X + 1, Y + 1, X + OuterWidth - 2, Y + 1, OuterTL);
        cv.DrawLine(X + 1, Y + 1, X + 1, Y + OuterHeight - 2, OuterTL);
        cv.DrawLine(X + 1, Y + OuterHeight - 2, X + OuterWidth - 2, Y + OuterHeight - 2, OuterBR);
        cv.DrawLine(X + OuterWidth - 2, Y + 1, X + OuterWidth - 2, Y + OuterHeight - 2, OuterBR);
        DrawPlatinumTitleBar(cv);
        if (!Collapsed && Contents != null)
        {
            var ctL = ContentLeft;
            var ctT = ContentTop;
            int ctW = Contents.Width;
            int ctH = Contents.Height;
            cv.DrawLine(ctL - 2, ctT - 2, ctL + ctW + 1, ctT - 2, OuterBR); // top
            cv.DrawLine(ctL - 2, ctT - 2, ctL - 2, ctT + ctH + 1, OuterBR); // left
            cv.DrawLine(ctL - 2, ctT + ctH + 1, ctL + ctW + 2, ctT + ctH + 1, OuterTL); // bottom
            cv.DrawLine(ctL + ctW + 1, ctT - 1, ctL + ctW + 1, ctT + ctH + 1, OuterTL); // right
            cv.DrawFilledRectangle(ctL, ctT, (ushort)ctW, (ushort)ctH, 0, ContentFill);
            cv.DrawImage(ctL, ctT, Contents, false);
            if (ctW > 2 && ctH > 2)
                cv.DrawRectangle(ctL + 0, ctT + 0, (ushort)(ctW - 1), (ushort)(ctH - 1), 0, Color.Black);
        }
    }


    private void DrawPlatinumTitleBar(Canvas cv)
    {
        var w = OuterWidth;
        int cx, sx, by;
        GetButtonRects(out cx, out sx, out by);
        var stripeLeft = X + 21;
        var stripeRight = sx - 3;
        if (stripeRight < stripeLeft) stripeRight = stripeLeft;
        for (var i = 0; i < 6; i++)
            cv.DrawLine(stripeLeft, Y + 4 + i * 2, stripeRight, Y + 4 + i * 2, TitleStripeLight);
        for (var i = 0; i < 6; i++)
            cv.DrawLine(stripeLeft + 1, Y + 5 + i * 2, stripeRight + 1, Y + 5 + i * 2, TitleStripeDark);
        var t = Title ?? string.Empty;
        int textW = Charcoal.MeasureString(t);
        const int PLAQUE_PAD = 3;
        var avail = stripeRight - stripeLeft;
        var tx = stripeLeft + (avail - textW) / 2;
        var minTx = stripeLeft + PLAQUE_PAD;
        var maxTx = stripeRight - PLAQUE_PAD - textW;
        if (tx < minTx) tx = minTx;
        if (tx > maxTx) tx = maxTx;
        var px = tx - PLAQUE_PAD;
        var plaqueW = textW + PLAQUE_PAD * 2;
        cv.DrawFilledRectangle(px, Y + 4, (ushort)plaqueW, 12, 0, TitleBandFill);
        for (var i = 0; i < 6; i++)
            cv.DrawLine(px + plaqueW, Y + 4 + i * 2, px + plaqueW, Y + 4 + i * 2, TitleStripeLight);
        for (var i = 0; i < 6; i++)
            cv.DrawLine(px - 1, Y + 5 + i * 2, px - 1, Y + 5 + i * 2, TitleStripeDark);
        cv.DrawString(tx, Y + 3, t, Charcoal, TitleTextColour);
        var closeImg = IsMouseOverCloseButton && MouseManager.MouseState == MouseState.Left
            ? closeButtonPressed
            : closeButton;
        var shadImg = IsMouseOverMinimizeButton && MouseManager.MouseState == MouseState.Left
            ? minimisePressed
            : minimise;
        cv.DrawImage(cx, by, closeImg);
        cv.DrawImage(sx, by, shadImg);
    }

    private void DrawDropShadow(Canvas cv)
    {
        var shadowX = X + OuterWidth;
        var shadowY = Y + OuterHeight;
        cv.DrawLine(shadowX, Y + 2, shadowX, shadowY, Color.Black);
        cv.DrawLine(X + 2, shadowY, shadowX, shadowY, Color.Black);
    }

    public virtual void HandleClick(MouseEventArgs e)
    {
    }

    public virtual void HandleDown(MouseEventArgs e)
    {
    }

    public virtual void HandleRelease(MouseEventArgs e)
    {
    }

    public virtual void HandleKey(KeyEvent key)
    {
        if (Collapsed) return;
        foreach (var control in Controls)
            if (control == FocusedControl)
                control.HandleKey(key);
    }

    public virtual void ShowContextMenu()
    {
    }

    public void Dispose()
    {
        Closing = true;
    }

    public void RenderOutsetWindowBackground()
    {
        if (Collapsed) return;
        Contents.DrawFilledRectangle(0, 0, Contents.Width, Contents.Height, 0, PlatFace);
    }

    public void RenderSystemStyleBorder()
    {
    }

    protected void ShowAboutDialog(string version)
    {
        Dialogue.Show(
            $"About {Title}",
            $"GoOS {Title} v{version}\n\nCopyright (c) " + Kernel.Copyright + " Owen2k6, et al.\nGPLv3 - free software.",
            heightOverride: 144);
    }

    protected void ShowAboutDialog()
    {
        ShowAboutDialog(Kernel.version);
    }

    protected void SetDock(WindowDock dock)
    {
        switch (dock)
        {
            case WindowDock.None:
                X = 0;
                Y = 0;
                break;
            case WindowDock.Auto:
                X = 50 + WindowManager.GetAmountOfWindowsByTitle(Title) * 50;
                Y = 50 + WindowManager.GetAmountOfWindowsByTitle(Title) * 50;
                break;
            case WindowDock.Center:
                X = WindowManager.Canvas.Width / 2 - OuterWidth / 2;
                Y = WindowManager.Canvas.Height / 2 - OuterHeight / 2;
                break;
            case WindowDock.Desktop:
                X = 0;
                Y = 19;
                break;
        }
    }

    public virtual void Paint()
    {
    }

    public void AutoCreate(WindowDock dock, int Width, int Height, string Title)
    {
        Contents = new Canvas((ushort)Width, (ushort)Height);
        SetDock(dock);
        this.Title = Title;
        Visible = true;
        Closable = true;
    }

    public void AutoCreate(int X, int Y, int Width, int Height, string Title)
    {
        Contents = new Canvas((ushort)Width, (ushort)Height);
        this.X = X;
        this.Y = Y;
        this.Title = Title;
        Visible = true;
        Closable = true;
    }

    public void ShowCrashDialogue(Exception e)
    {
        Dialogue.Show(nameof(WindowManager),
            "The app " + Title + " has thrown an exception and has had to close:\n" + e, default,
            WindowManager.errorIcon);
        Dispose();
    }

    // fallback
    private static int MeasureTextWidth(string s)
    {
        return string.IsNullOrEmpty(s) ? 0 : s.Length * 8;
    }
}