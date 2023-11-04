using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using PrismAPI.UI.Controls;

namespace GoOS.GUI.Apps
{
    public class GCLauncher : Window
    {

        Button LaunchButton;
        Input WhatToLaunch;

        public GCLauncher()
        {
            Contents = new Canvas(400, 200);
            Contents.Clear(Color.LightGray);
            Title = "GoCode Launcher";
            Visible = true;
            Closable = true;
            SetDock(WindowDock.Center);
            
            WhatToLaunch = new Input(this,5,5,390,20, "Provide a path to your gexe file. (\\path\\to\\file.gexe)");
            WhatToLaunch.Render();
            LaunchButton = new Button(this, 5, 30, 390, 20, "Launch"){
                Clicked = GCVM_Click
            };;
            LaunchButton.Render();
            
            
        }
        
        private void GCVM_Click()
        {
            WindowManager.AddWindow(new GCVM(this.WhatToLaunch.Text));
        }

        public class GCVM : Window
        {
    
            public static VMBetterConsole VMTERM;
    
            public GCVM(string path)
            {
                try
                {
                    VMTERM = new VMBetterConsole(640, 480);

                    Contents = VMTERM.Canvas;
                    Title = "GoCode Console Output";
                    Visible = true;
                    Closable = true;
                    SetDock(WindowDock.Auto);
                    VMTERM.Visible = true;

                    GoOS.Commands.VM.Run("run "+path);
                }
                catch (Exception eee)
                {
                    Dialogue.Show(
                        "Error",
                        eee.Message,
                        null, // default buttons
                        WindowManager.errorIcon);
                }
        
            }
        }
    }
}