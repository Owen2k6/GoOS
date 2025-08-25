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

namespace GoOS.GUI.Apps.Terminal
{
    public sealed class Terminal : Window
    {
        private bool _reading;

        internal SVGAIITerminal terminal;

        internal Shell _shell;

        internal Terminal(Shell shell = null) : base(420, 150, 300, 200, "Terminal") {
            terminal = new SVGAIITerminal(this.Width - 12, this.Height - 29, Resources.Fragment, UpdateRequest, IdleRequest);
            terminal.SetCursorPosition(0, 0);

            if (shell == null) shell = new GShell(terminal);
            
            _shell = shell;
        }

        private void IdleRequest() {
            ProcessScheduler.HandleRun();

            if (KeyboardManager.TryReadKey(out var key)) terminal.KeyBuffer.Enqueue(key);
        }

        private void UpdateRequest() {
            Contents.DrawImage(6, 22, terminal.Contents, false);

            WindowManager.Render();
        }

        internal override void HandleRun()
        {
            base.HandleRun();

            _shell?.Run();
        }

        
    }
}