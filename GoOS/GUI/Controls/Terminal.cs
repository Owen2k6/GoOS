using Cosmos.System;
using GoOS.GUI.Apps.Terminal;
using GoOS.GUI.Apps.Terminal.Shells;
using GoOS.Tasking;

namespace GoOS.GUI;

internal class Terminal : Control
{
    private bool _reading;

    internal SVGAIITerminal terminal;

    internal Shell _shell;
    
    internal Terminal(Window Parent, int X, int Y, ushort Width, ushort Height, Shell shell = null, string Name = "", bool noOffset = false) : base(Parent, X, Y, Width, Height, Name, noOffset)
    {
        terminal = new SVGAIITerminal(Width, Height, Resources.Fragment, UpdateRequest, IdleRequest);
        terminal.SetCursorPosition(0, 0);
        
        Contents = terminal.Contents;

        if (shell == null) shell = new GShell(terminal);
            
        _shell = shell;
    }
    
    private void IdleRequest() {
        Kernel.ProcessScheduler.HandleRun();

        if (KeyboardManager.TryReadKey(out var key)) terminal.KeyBuffer.Enqueue(key);
    }

    private void UpdateRequest() {
        Parent.Render();
    }

    internal override void HandleRun()
    {
        base.HandleRun();

        _shell?.Run();
    }
}