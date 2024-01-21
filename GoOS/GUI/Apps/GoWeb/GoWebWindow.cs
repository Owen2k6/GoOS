#nullable enable
using GoOS.GUI.Apps.GoWeb.Html;
using GoOS.GUI.Apps.GoWeb.Http;
using GoOS.GUI.Apps.GoWeb.Render;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps.GoWeb
{
    public class GoWebWindow : Window
    {
        readonly Input AddressBar;

        readonly Button HomeButton;
        readonly Button GoButton;

        Document? ActiveDocument;

        private static readonly Uri HOMEPAGE = Uri.FromString("about:welcome");

        private const int TOOLBAR_HEIGHT = 48;

        private const int CONTENT_PADDING = 2; /* window border */

        public GoWebWindow()
        {
            Contents = new Canvas(800, 600);
            Title = "GoWeb";
            Visible = true;
            Closable = true;
            Sizable = false;
            SetDock(WindowDock.Auto);

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

        void Goto(Uri uri)
        {
            try
            {
                ActiveDocument = Document.LoadFromUri(uri);
                Title = $"{ActiveDocument.Title} - GoWeb";
                //AddressBar.Text = uri.ToString(); /* crash? */

                //Dialogue.Show("Document Loaded", ActiveDocument.Title);

                DocumentLayout layout = new DocumentLayout(ActiveDocument, Contents.Width);
                
                Canvas target = new((ushort)(Contents.Width - CONTENT_PADDING * 2), (ushort)(Contents.Height - TOOLBAR_HEIGHT - CONTENT_PADDING));
                
                Renderer.Render(ActiveDocument, target);
                
                Contents.DrawImage(CONTENT_PADDING, TOOLBAR_HEIGHT, target, false);
                
                RenderSystemStyleBorder();
            }
            catch (System.Exception e)
            {
                Dialogue.Show("GoWeb", $"The page at '{uri.ToString()}' failed to load.\n{e.ToString()}", null, WindowManager.errorIcon);
                AddressBar.Text = string.Empty;
            }
        }

        void Go_Click()
        {
            string address = AddressBar.Text;
            if (string.IsNullOrWhiteSpace(address))
            {
                return;
            }
            if (!address.Contains(':'))
            {
                address = "http://" + address;
            }
            Goto(Uri.FromString(address));
        }

        void Home_Click() => Goto(HOMEPAGE);
    }
}
