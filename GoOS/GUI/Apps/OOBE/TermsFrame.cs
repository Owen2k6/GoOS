using System.IO;
using PrismAPI.Graphics;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.OOBE
{
    public class TermsFrame : Window
    {
        Button NextButton;
        Button NoButton;

        public TermsFrame()
        {
            // Create the window.
            Contents = new Canvas(800, 600);
            Title = "Terms of Use and Licencing - GoOS Setup";
            Visible = true;
            Closable = false;
            SetDock(WindowDock.Center);

            // Initialize the controls.
            NoButton = new Button(this, 560, 570, 100, 20, "Decline") { Clicked = NoButton_Click };
            NextButton = new Button(this, 680, 570, 100, 20, "Accept") { Clicked = NextButton_Click };

            // Paint the window.
            Contents.DrawImage(0, 0, OOBEblank, false);
            Contents.DrawString(20, 10, "GoOS Licence and Terms of Use", Font_2x, Color.White);
            Contents.DrawString(20, 50,
                "Thank you for choosing GoOS, an open-source operating system released under the Owen2k6 Open\nSource License. Please read the following terms carefully before using GoOS.\n\nBy using GoOS, you agree to comply with and be bound by the terms and conditions set forth\nin this document. If you do not agree to these terms, please do not use GoOS.\n\nGoOS is released under the Owen2k6 Open Source License. This license grants you a worldwide,\nroyalty-free, non-exclusive license to use, modify, and distribute GoOS in accordance with\nthe terms of this license.\n\nYou may not distribute GoOS or any derivative work based on GoOS for a fee. The distribution\nmust be free of charge. Any distribution or redistribution of GoOS or its derivatives,\nwhether in source code or compiled form, must include the corresponding source code.\nThe source code must be easily accessible and accompany the distributed software.\n\nYou may not use GoOS for the development of proprietary operating systems, and you may not\nkeep any modifications or enhancements to the GoOS source code private when used for distribution.\n\nGoOS is provided \"as-is\" without any warranty or guarantee of any kind. The entire risk of\nusing GoOS lies with the user. The developers of GoOS and the Owen2k6 Open Source License\ndisclaim any and all warranties, whether expressed or implied.\n\nIn no event shall the developers of GoOS or the Owen2k6 Open Source License be liable for\nany damages, including but not limited to direct, indirect, special, incidental, or\nconsequential damages arising out of the use or inability to use GoOS.\n\nThe terms of use and license for GoOS may be revised periodically. Users are encouraged to\nreview the terms regularly for any updates. Continued use of GoOS after changes to the\nterms implies acceptance of the revised terms.\n\nBy using GoOS, you acknowledge that you have read, understood, and agreed to these terms.\nIf you have any questions or concerns, please contact @Owen2k6.",
                Font_1x, Color.White);
            NextButton.Render();
            NoButton.Render();
        }

        private void NextButton_Click()
        {
            // Continue.
            WindowManager.AddWindow(new UserFrame());
            Dispose();
        }
        private void NoButton_Click()
        {
            // Continue.
            Cosmos.System.Power.Shutdown();
            Dispose();
        }
    }
}