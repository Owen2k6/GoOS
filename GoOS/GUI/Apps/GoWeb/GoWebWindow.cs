#nullable enable
using System;
using GoGL.Graphics;
using GoOS.GUI.Apps.GoWeb.Html;
using GoOS.GUI.Apps.GoWeb.Render;
using Uri = GoOS.GUI.Apps.GoWeb.Http.Uri;

namespace GoOS.GUI.Apps.GoWeb;

public class GoWebWindow : Window
{
    private const int TOOLBAR_HEIGHT = 48;
    public static string Version = "0.1.0";

    private static readonly Uri HOMEPAGE = Uri.FromString("about:welcome");
    private readonly Input AddressBar;
    private readonly Button GoButton;

    private readonly Button HomeButton;

    private readonly Canvas PageCanvas;

    private Document? ActiveDocument;

    public GoWebWindow()
    {
        Contents = new Canvas(800, 600);
        Title = "GoWeb";
        Visible = true;
        Closable = true;
        SetDock(WindowDock.Auto);

        PageCanvas = new Canvas(Contents.Width, (ushort)(Contents.Height - TOOLBAR_HEIGHT));

        Contents.Clear(Color.White);

        Contents.DrawImage(0, 0, GoWebResources.toolbarBackground, false);

        AddressBar = new Input(this, 44, 16, (ushort)(Contents.Width - 158), 20, "Enter Web address");
        AddressBar.Render();

        HomeButton = new Button(this, 16, 16, 20, 20, string.Empty)
        {
            Image = GoWebResources.home,
            Clicked = Home_Click
        };
        HomeButton.Render();

        GoButton = new Button(this, (ushort)(AddressBar.X + AddressBar.Contents.Width + 8), 16, 20, 20, string.Empty)
        {
            Image = GoWebResources.go,
            Clicked = Go_Click
        };
        GoButton.Render();

        RenderSystemStyleBorder();

        Goto(HOMEPAGE);
    }

    private void DisplayStatus(string message)
    {
        var boxX = 10;
        var boxY = TOOLBAR_HEIGHT + 10;
        var boxW = (ushort)(Resources.Font_1x.MeasureString(message) + 100);
        ushort boxH = 56;
        const byte shdDist = 2;

        Contents.DrawFilledRectangle(0, TOOLBAR_HEIGHT, Contents.Width, (ushort)(Contents.Height - TOOLBAR_HEIGHT), 0,
            Color.White);

        Contents.DrawFilledRectangle(boxX + shdDist, boxY + shdDist, boxW, boxH, 0, Color.Black);
        Contents.DrawFilledRectangle(boxX, boxY, boxW, boxH, 0, Color.White);
        Contents.DrawRectangle(boxX, boxY, boxW, boxH, 0, Color.Black);

        Contents.DrawImage(boxX + 10, boxY + 10, Resources.drumIcon);

        Contents.DrawString(boxX + 70, boxY + (boxH - Resources.Font_1x.Size) / 2, message, Resources.Font_1x,
            Color.Black);

        RenderSystemStyleBorder();

        WindowManager.Update();
    }

    private void Goto(Uri uri)
    {
        try
        {
            DisplayStatus($"Loading {uri}.");

            ActiveDocument = Document.LoadFromUri(uri);
            Title = $"{ActiveDocument.Title} - GoWeb";
            //AddressBar.Text = uri.ToString(); /* crash? */

            DisplayStatus($"Laying out {uri}.");

            var layout = new DocumentLayout(ActiveDocument, Contents.Width);

            DisplayStatus($"Rendering {uri}.");

            Renderer.Render(ActiveDocument, PageCanvas);

            Contents.DrawImage(0, TOOLBAR_HEIGHT, PageCanvas, false);

            RenderSystemStyleBorder();
        }
        catch (Exception e)
        {
            Contents.DrawFilledRectangle(0, TOOLBAR_HEIGHT, Contents.Width, (ushort)(Contents.Height - TOOLBAR_HEIGHT),
                0, Color.White);
            Dialogue.Show("GoWeb", $"The page at '{uri}' failed to load.\n{e}", null, WindowManager.errorIcon);
        }
    }

    private void Go_Click()
    {
        var address = AddressBar.Text;
        if (string.IsNullOrWhiteSpace(address)) return;
        if (!address.Contains(':')) address = "http://" + address;
        Goto(Uri.FromString(address));
    }

    private void Home_Click()
    {
        Goto(HOMEPAGE);
    }
}