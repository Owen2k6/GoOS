using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.System;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;
using GoOS;
using Gold.Graphics;
using Gold.Graphics.Fonts;
using GoOS.Commands;
using GoOS.Themes;
using LibDotNetParser.CILApi;
using System;
using GoOS.Tasking;
using GoOS.GUI.Apps.Terminal.Shells;
using Microsoft.VisualBasic;

namespace GoOS.GUI.Apps.Terminal
{
    public sealed class GoTerminal : Window
    {
        internal GUI.Terminal term;

        internal GoTerminal() : base(0, 0, 300, 200, "Terminal")
        {
            SetDock(WindowDock.Auto);
            
            term = new GUI.Terminal(this, 0, 0, Width, Height);

            Controls.Add(term);

            Render();
        }
        
    }
}