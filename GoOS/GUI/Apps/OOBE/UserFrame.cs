using System.IO;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class UserFrame : Window
    {
        Input UserName;
        Input ComputerName;
        Button NextButton;

        public UserFrame()
        {
            // Create the window.
            Contents = new Canvas(800, 600);
            Title = "Welcome to GoOS";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            UserName = new Input(this, 420, 400, 100, 20, "user");
            ComputerName = new Input(this, 420, 426, 100, 20, "GoOS");
            NextButton = new Button(this, 350, 456, 100, 20, "Next") { Clicked = NextButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEmain, false);
            Contents.DrawString(300, 400, "Username: ", Font_1x, Color.White);
            Contents.DrawString(300, 426, "Computer name: ", Font_1x, Color.White);
            UserName.Render();
            ComputerName.Render();
            NextButton.Render();
        }

        private void NextButton_Click()
        {
            Directory.CreateDirectory(@"0:\content");
            Directory.CreateDirectory(@"0:\content\sys");
            Directory.CreateDirectory(@"0:\content\themes");
            Directory.CreateDirectory(@"0:\content\prf");
            Directory.CreateDirectory(@"0:\framework");
            File.Create(@"0:\content\sys\option-showprotectedfiles.gms");
            File.Create(@"0:\content\sys\option-editprotectedfiles.gms");
            File.Create(@"0:\content\sys\option-deleteprotectedfiles.gms");
            File.Create(@"0:\content\sys\setup.gms");
            File.WriteAllText(@"0:\content\sys\version.gms", $"System.Version is set to {Kernel.version} \n Note to users reading this: DO NOT ALTER. IMPORTANT IF USER DATA NEEDS CONVERTING.");
            File.WriteAllText(@"0:\content\sys\user.gms", $"username: {UserName.Text}\ncomputername: {ComputerName.Text}");
            File.WriteAllBytes(@"0:\content\sys\resolution.gms", new byte[] { 2 }); // Video mode 2: 1280x720
            File.WriteAllText(@"0:\content\themes\default.gtheme", "Default = White\nBackground = Black\nStartup = DarkMagenta,Red,DarkRed\nWindowText = Cyan\nWindowBorder = Green\nErrorText = Red\nOther1 = Yellow");
            File.WriteAllText(@"0:\content\themes\mono.gtheme", "Default = White\nBackground = Black\nStartup = White,White,White\nWindowText = White\nWindowBorder = White\nErrorText = White\nOther1 = White");
            File.WriteAllText(@"0:\content\themes\dark.gtheme", "Default = Gray\nBackground = Black\nStartup = DarkGray,Gray,DarkGray\nWindowText = Gray\nWindowBorder = DarkGray\nErrorText = DarkGray\nOther1 = DarkGray");
            File.WriteAllText(@"0:\content\themes\light.gtheme", "Default = Black\nBackground = White\nStartup = Black,Black,Black\nWindowText = Black\nWindowBorder = Black\nErrorText = Black\nOther1 = Black");
            File.WriteAllText(@"0:\content\sys\theme.gms", "ThemeFile = " + @"0:\content\themes\default.gtheme");

            Cosmos.System.Power.Reboot();
        }
    }
}
