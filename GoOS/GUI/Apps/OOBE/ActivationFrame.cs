using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoOS.Themes;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class ActivationFrame : Window
    {
        Button NextButton;
        String Username, Computername;

        public ActivationFrame(string username, string computername)
        {
            Username = username;
            Computername = computername;

            // Create the window.
            Contents = new Canvas(800, 600);
            Title = "Activation - GoOS Setup";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            NextButton = new Button(this, 20, 570, 100, 20, "Continue") { Clicked = NextButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEblank, false);
            NextButton.Render();
            Contents.DrawString(20, 5, "GoOS Activation", Font_2x, Color.White);
            Contents.DrawString(20, 40, "You're all set to continue and begin setting up the OS files.\nClicking continue will begin creating files onto your HDD\nThere is still time to cancel setup if you do not wish to install GoOS Files.", Font_1x, Color.White);
        }

        private void NextButton_Click()
        {
            // Continue.
            Directory.CreateDirectory(@"0:\content");
            Directory.CreateDirectory(@"0:\content\sys");
            Directory.CreateDirectory(@"0:\content\themes");
            Directory.CreateDirectory(@"0:\content\prf");
            Directory.CreateDirectory(@"0:\framework");
            Directory.CreateDirectory(@"0:\go");
            File.Create(@"0:\content\sys\option-showprotectedfiles.gms");
            File.Create(@"0:\content\sys\option-editprotectedfiles.gms");
            File.Create(@"0:\content\sys\option-deleteprotectedfiles.gms");
            File.Create(@"0:\content\sys\setup.gms");
            File.Create(@"0:\content\sys\pinnedapps.gms");
            File.WriteAllText(@"0:\content\sys\version.gms", $"System.Version is set to {Kernel.version} \n Note to users reading this: DO NOT ALTER. IMPORTANT IF USER DATA NEEDS CONVERTING.");
            File.WriteAllText(@"0:\content\sys\user.gms", $"username: {Username}\ncomputername: {Computername}");
            File.WriteAllBytes(@"0:\content\sys\resolution.gms", new byte[] { 2 }); // Video mode 2: 1280x720
            File.WriteAllText(@"0:\content\themes\default.gtheme", "Default = White\nBackground = Black\nStartup = DarkMagenta,Red,DarkRed\nWindowText = Cyan\nWindowBorder = Green\nErrorText = Red\nOther1 = Yellow");
            File.WriteAllText(@"0:\content\themes\mono.gtheme", "Default = White\nBackground = Black\nStartup = White,White,White\nWindowText = White\nWindowBorder = White\nErrorText = White\nOther1 = White");
            File.WriteAllText(@"0:\content\themes\dark.gtheme", "Default = Gray\nBackground = Black\nStartup = DarkGray,Gray,DarkGray\nWindowText = Gray\nWindowBorder = DarkGray\nErrorText = DarkGray\nOther1 = DarkGray");
            File.WriteAllText(@"0:\content\themes\light.gtheme", "Default = Black\nBackground = White\nStartup = Black,Black,Black\nWindowText = Black\nWindowBorder = Black\nErrorText = Black\nOther1 = Black");
            File.WriteAllText(@"0:\content\sys\theme.gms", "ThemeFile = " + @"0:\content\themes\default.gtheme");
            WindowManager.AddWindow(new DoneFrame());
            Dispose();
        }
    }
}
