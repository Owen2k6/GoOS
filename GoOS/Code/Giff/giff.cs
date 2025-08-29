// File: GoOS/GUI/Giff/Giff.cs
// Safe, minimal, single-threaded Giff runtime for GoOS on Cosmos (IL2CPU).
// British English throughout. No reflection, no threads, no async, no P/Invoke, no System.Drawing.
// Draw once; let the engine composite. No dialogues from Paint()/HandleRun().

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

// Engine UI
using GoOS.GUI;                   // Window, WindowManager, Dialogue

// Graphics (your tree)
using GCanvas = Gold.Graphics.Canvas;
using GColor  = Gold.Graphics.Color;

using static GoOS.Resources;      // Font_1x

namespace GoOS.Giff
{
    public static class Giff
    {
        public static List<Window> Run(string script)
        {
            var built = new List<Window>(4);
            try
            {
                var tokens  = new Lexer(script).Tokenise();
                var program = new Parser(tokens).ParseProgram();

                var pendingOnLoad = new List<ScriptWindow>(program.Windows.Count);

                for (int i = 0; i < program.Windows.Count; i++)
                {
                    var wn = program.Windows[i];
                    var w  = Runtime.BuildWindow(wn);
                    WindowManager.AddWindow(w);
                    built.Add(w);

                    if (w is ScriptWindow sw && sw.HasOnLoad) pendingOnLoad.Add(sw);
                }

                // Execute queued onLoad outside Paint()
                for (int i = 0; i < pendingOnLoad.Count; i++)
                    pendingOnLoad[i].ExecuteOnLoadQueued();
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Failed to execute script:\n" + ex);
            }
            return built;
        }
    }

    // ---------- Lexer ----------
    internal enum TokKind { Eof, Ident, Number, String, LBrace, RBrace, Colon, Semi, Comma, Arrow, HashWord }
    internal readonly struct Tok { public readonly TokKind Kind; public readonly string Text; public readonly int Pos;
        public Tok(TokKind k, string t, int p){ Kind=k; Text=t; Pos=p; } }

    internal sealed class Lexer
    {
        private readonly string _s; private int _i;
        public Lexer(string s){ _s = s ?? string.Empty; _i = 0; }

        public List<Tok> Tokenise()
        {
            var t = new List<Tok>(256);
            int safety = Math.Max(256, _s.Length * 4);

            while (safety-- > 0)
            {
                SkipWs();
                if (_i >= _s.Length){ t.Add(new Tok(TokKind.Eof, "", _i)); break; }

                int before = _i;
                char c = _s[_i];

                if (c=='{') t.Add(new Tok(TokKind.LBrace,"{",_i++));
                else if (c=='}') t.Add(new Tok(TokKind.RBrace,"}",_i++));
                else if (c==':') t.Add(new Tok(TokKind.Colon, ":",_i++));
                else if (c==';') t.Add(new Tok(TokKind.Semi,  ";",_i++));
                else if (c==',') t.Add(new Tok(TokKind.Comma, ",",_i++));
                else if (c=='=' && Peek()=='>'){ t.Add(new Tok(TokKind.Arrow,"=>",_i)); _i+=2; }
                else if (c=='"' || c=='\'') t.Add(ReadString());
                else if (c=='#') t.Add(ReadHashWord());
                else if (char.IsDigit(c))
                {
                    int s0=_i; while(_i<_s.Length && char.IsDigit(_s[_i])) _i++;
                    t.Add(new Tok(TokKind.Number,_s.Substring(s0,_i-s0),s0));
                }
                else if (char.IsLetter(c) || c=='_' || c=='.')
                {
                    int s0=_i;
                    while(_i<_s.Length)
                    {
                        char ch=_s[_i];
                        if (char.IsLetterOrDigit(ch) || ch=='_' || ch=='.') _i++; else break;
                    }
                    t.Add(new Tok(TokKind.Ident,_s.Substring(s0,_i-s0),s0));
                }
                else _i++;

                if (_i <= before) _i++; // guarantee progress
            }

            if (safety<=0) t.Add(new Tok(TokKind.Eof,"",_i));
            return t;
        }

        private void SkipWs()
        {
            while (_i < _s.Length)
            {
                char c = _s[_i];
                if (char.IsWhiteSpace(c)) { _i++; continue; }
                if (c=='/' && Peek()=='/')
                { _i+=2; while(_i<_s.Length && _s[_i]!='\n') _i++; continue; }
                break;
            }
        }

        private Tok ReadString()
        {
            char q = _s[_i++]; var b = new StringBuilder(); int start=_i;
            while (_i < _s.Length)
            {
                char c = _s[_i++];
                if (c==q) break;
                if (c=='\\' && _i < _s.Length)
                {
                    char e = _s[_i++];
                    b.Append(e switch {'n'=>'\n','r'=>'\r','t'=>'\t','\\'=>'\\','"'=>'"','\''=>'\'', _=>e});
                }
                else b.Append(c);
            }
            return new Tok(TokKind.String,b.ToString(),start-1);
        }

        private Tok ReadHashWord()
        {
            int s0 = _i++; // include '#'
            while (_i < _s.Length && char.IsLetterOrDigit(_s[_i])) _i++;
            return new Tok(TokKind.HashWord,_s.Substring(s0,_i-s0),s0);
        }

        private char Peek() => (_i+1<_s.Length) ? _s[_i+1] : '\0';
    }

    // ---------- AST ----------
    internal sealed class ProgramNode { public readonly List<WindowNode> Windows = new(4); }

    internal sealed class WindowNode
    {
        public string Name = "Window";
        public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase);

        public readonly List<LabelNode>   Labels    = new(8);
        public readonly List<InputNode>   Inputs    = new(8);
        public readonly List<ButtonNode>  Buttons   = new(8);
        public readonly List<PanelNode>   Panels    = new(4);
        public readonly List<CheckboxNode>Checkboxes= new(8);
        public readonly List<RadioNode>   Radios    = new(8);
        public readonly List<ImageNode>   Images    = new(8);

        public readonly List<CommandNode> OnLoad    = new(4);
        public readonly List<CommandNode> OnClose   = new(4);
    }

    internal sealed class LabelNode   { public string Name="label";   public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase); }
    internal sealed class InputNode   { public string Name="input";   public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase); }
    internal sealed class ButtonNode  { public string Name="button";  public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase); public readonly List<CommandNode> OnClick = new(4); }
    internal sealed class CheckboxNode{ public string Name="checkbox";public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase); }
    internal sealed class RadioNode   { public string Name="radio";   public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase); }
    internal sealed class ImageNode   { public string Name="image";   public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase); }

    internal sealed class PanelNode
    {
        public readonly Dictionary<string,string> Props = new(StringComparer.OrdinalIgnoreCase);
        public readonly List<LabelNode>   Labels    = new(8);
        public readonly List<InputNode>   Inputs    = new(8);
        public readonly List<ButtonNode>  Buttons   = new(8);
        public readonly List<CheckboxNode>Checkboxes= new(8);
        public readonly List<RadioNode>   Radios    = new(8);
        public readonly List<ImageNode>   Images    = new(4);
    }

    internal abstract class CommandNode { }
    internal sealed class MessageCmd : CommandNode { public string Text; public MessageCmd(string t){ Text=t; } }
    internal sealed class CloseCmd   : CommandNode { }

    // ---------- Parser ----------
    internal sealed class Parser
    {
        private readonly List<Tok> _t; private int _i;
        public Parser(List<Tok> t){ _t=t; _i=0; }

        public ProgramNode ParseProgram()
        {
            var p = new ProgramNode();
            int safety = Math.Max(64, _t.Count * 4);

            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("window")) p.Windows.Add(ParseWindow());
                else _i++;
            }
            return p;
        }

        private WindowNode ParseWindow()
        {
            var w = new WindowNode();
            if (Peek().Kind==TokKind.Ident) w.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(64, _t.Count * 2);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("label"))    { w.Labels.Add(ParseLabel());       continue; }
                if (TryMatchIdent("input"))    { w.Inputs.Add(ParseInput());       continue; }
                if (TryMatchIdent("button"))   { w.Buttons.Add(ParseButton());     continue; }
                if (TryMatchIdent("panel"))    { w.Panels.Add(ParsePanel());       continue; }
                if (TryMatchIdent("checkbox")) { w.Checkboxes.Add(ParseCheckbox());continue; }
                if (TryMatchIdent("radio"))    { w.Radios.Add(ParseRadio());       continue; }
                if (TryMatchIdent("image"))    { w.Images.Add(ParseImage());       continue; }
                if (TryMatchIdent("onLoad"))   { ParseBlockInto(w.OnLoad);         continue; }
                if (TryMatchIdent("onClose"))  { ParseBlockInto(w.OnClose);        continue; }
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(w.Props);       continue; }
                _i++;
            }
            return w;
        }

        private PanelNode ParsePanel()
        {
            var p = new PanelNode();
            Expect(TokKind.LBrace);
            int safety = Math.Max(32, _t.Count);

            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("label"))    { p.Labels.Add(ParseLabel());       continue; }
                if (TryMatchIdent("input"))    { p.Inputs.Add(ParseInput());       continue; }
                if (TryMatchIdent("button"))   { p.Buttons.Add(ParseButton());     continue; }
                if (TryMatchIdent("checkbox")) { p.Checkboxes.Add(ParseCheckbox());continue; }
                if (TryMatchIdent("radio"))    { p.Radios.Add(ParseRadio());       continue; }
                if (TryMatchIdent("image"))    { p.Images.Add(ParseImage());       continue; }
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(p.Props);       continue; }
                _i++;
            }
            return p;
        }

        private LabelNode ParseLabel()
        {
            var n = new LabelNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private InputNode ParseInput()
        {
            var n = new InputNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private ButtonNode ParseButton()
        {
            var b = new ButtonNode();
            if (Peek().Kind==TokKind.Ident) b.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("onClick")){ ParseBlockInto(b.OnClick); continue; }
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(b.Props); continue; }
                _i++;
            }
            return b;
        }

        private CheckboxNode ParseCheckbox()
        {
            var n = new CheckboxNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private RadioNode ParseRadio()
        {
            var n = new RadioNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private ImageNode ParseImage()
        {
            var n = new ImageNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private void ParsePropertyInto(Dictionary<string,string> dict)
        {
            var key = Expect(TokKind.Ident);
            Expect(TokKind.Colon);
            var val = Next(); // string|number|ident|hashword
            dict[key.Text] = val.Text;
            if (Peek().Kind==TokKind.Semi) _i++; // optional ;
        }

        private void ParseBlockInto(List<CommandNode> list)
        {
            Expect(TokKind.LBrace);
            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("message")){ var s = Expect(TokKind.String); list.Add(new MessageCmd(s.Text)); if (Peek().Kind==TokKind.Semi) _i++; continue; }
                if (TryMatchIdent("close"))  { list.Add(new CloseCmd()); if (Peek().Kind==TokKind.Semi) _i++; continue; }
                _i++;
            }
        }

        private void SkipSeps(){ while (Peek().Kind==TokKind.Semi || Peek().Kind==TokKind.Comma) _i++; }
        private Tok Peek()=> _i<_t.Count ? _t[_i] : new Tok(TokKind.Eof,"",_i);
        private Tok Next(){ var x=Peek(); if (_i<_t.Count) _i++; return x; }
        private Tok Expect(TokKind k){ var t=Next(); return t.Kind==k ? t : new Tok(k,"",t.Pos); }
        private bool Match(TokKind k){ if (Peek().Kind==k){ _i++; return true; } return false; }
        private bool TryMatchIdent(string s){ var t=Peek(); if (t.Kind==TokKind.Ident && string.Equals(t.Text,s,StringComparison.OrdinalIgnoreCase)){ _i++; return true; } return false; }
        private bool AtEnd()=> Peek().Kind==TokKind.Eof;
    }

    // ---------- Runtime ----------
    internal static class Runtime
    {
        public static Window BuildWindow(WindowNode n)
        {
            try
            {
                int width  = ReadInt(n.Props, "width",  320);
                int height = ReadInt(n.Props, "height", 240);

                var win = new ScriptWindow((ushort)width, (ushort)height)
                {
                    Title       = ReadString(n.Props, "title", n.Name),
                    Closable    = ReadBool(n.Props, "closable", true),
                    HasTitlebar = ReadBool(n.Props, "titlebar", true),
                    Visible     = true
                };

                win.X = ReadInt(n.Props, "x", 40);
                win.Y = ReadInt(n.Props, "y", 40);

                var bg = ParseColour(ReadString(n.Props, "background", ""));
                if (bg.HasValue) win.BackgroundColor = bg.Value;

                // Top-level
                for (int i=0;i<n.Labels.Count;i++)     BuildLabel (win, n.Labels[i]);
                for (int i=0;i<n.Inputs.Count;i++)     BuildInput (win, n.Inputs[i]);
                for (int i=0;i<n.Buttons.Count;i++)    BuildButton(win, n.Buttons[i]);
                for (int i=0;i<n.Checkboxes.Count;i++) BuildCheckbox(win, n.Checkboxes[i], 0, 0);
                for (int i=0;i<n.Radios.Count;i++)     BuildRadio   (win, n.Radios[i], 0, 0);
                for (int i=0;i<n.Images.Count;i++)     BuildImage   (win, n.Images[i], 0, 0);

                // Panels
                for (int i=0;i<n.Panels.Count;i++) BuildPanel(win, n.Panels[i]);

                if (n.OnLoad.Count  > 0) win.QueueOnLoad (n.OnLoad);
                if (n.OnClose.Count > 0) win.QueueOnClose(n.OnClose);

                return win;
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "BuildWindow failed:\n" + ex);
                var w = new ScriptWindow(200, 80) { Title = "Giff Error", Visible = true };
                w.BackgroundColor = new GColor(64, 0, 0);
                return w;
            }
        }

        private static void BuildPanel(ScriptWindow host, PanelNode p)
        {
            int px = ReadInt(p.Props, "x", 10);
            int py = ReadInt(p.Props, "y", 10);
            int pW = ReadInt(p.Props, "width",  200);
            int pH = ReadInt(p.Props, "height", 100);

            ClampRect(host.ClientWidth, host.ClientHeight, ref px, ref py, ref pW, ref pH);

            var pbg = ParseColour(ReadString(p.Props, "background", ""));
            if (pbg.HasValue) host.AddPanelFill(px, py, pW, pH, pbg.Value);

            // labels (relative to panel)
            for (int j=0;j<p.Labels.Count;j++)
            {
                var l = p.Labels[j];
                int x = ReadInt(l.Props, "x", 10) + px;
                int y = ReadInt(l.Props, "y", 10) + py;
                string text = ReadString(l.Props, "text", l.Name);
                var col  = ParseColour(ReadString(l.Props, "textColour", "")) ?? GColor.Black;

                bool centre = ReadCentreFlag(l.Props);
                if (centre)
                {
                    int wpx = Font_1x.MeasureString(text);
                    x = px + Math.Max(0, (pW - wpx) / 2);
                }

                ClampPoint(host.ClientWidth, host.ClientHeight, ref x, ref y);
                host.AddLabel(new ScriptWindow.LabelSpec { X=x, Y=y, Text=text, Colour=col });
            }

            // inputs
            for (int j=0;j<p.Inputs.Count;j++)
            {
                var i = p.Inputs[j];
                int x = ReadInt(i.Props, "x", 10) + px;
                int y = ReadInt(i.Props, "y", 10) + py;
                int w = ReadInt(i.Props, "width", 120);
                int h = ReadInt(i.Props, "height", 20);
                ClampRect(host.ClientWidth, host.ClientHeight, ref x, ref y, ref w, ref h);

                var input = new Input(host, (ushort)x, (ushort)y, (ushort)w, (ushort)h,
                                      ReadString(i.Props, "placeholder", ""))
                { Text = ReadString(i.Props, "text", "") };

                input.X = (ushort)x; input.Y = (ushort)y;
                input.Render();
            }

            // buttons
            for (int j=0;j<p.Buttons.Count;j++)
                BuildButtonAbs(host, p.Buttons[j], px, py);

            // checkboxes
            for (int j=0;j<p.Checkboxes.Count;j++)
                BuildCheckbox(host, p.Checkboxes[j], px, py);

            // radios
            for (int j=0;j<p.Radios.Count;j++)
                BuildRadio(host, p.Radios[j], px, py);

            // images
            for (int j=0;j<p.Images.Count;j++)
                BuildImage(host, p.Images[j], px, py);
        }

        private static void BuildLabel(ScriptWindow w, LabelNode n)
        {
            int x = ReadInt(n.Props, "x", 10);
            int y = ReadInt(n.Props, "y", 10);
            string text = ReadString(n.Props, "text", n.Name);
            var col  = ParseColour(ReadString(n.Props, "textColour", "")) ?? GColor.Black;

            bool centre = ReadCentreFlag(n.Props);
            if (centre)
            {
                int wpx = Font_1x.MeasureString(text);
                x = Math.Max(0, (w.ClientWidth - wpx) / 2);
            }

            ClampPoint(w.ClientWidth, w.ClientHeight, ref x, ref y);
            w.AddLabel(new ScriptWindow.LabelSpec { X=x, Y=y, Text=text, Colour=col });
        }

        private static void BuildInput(ScriptWindow w, InputNode n)
        {
            int x = ReadInt(n.Props, "x", 10);
            int y = ReadInt(n.Props, "y", 10);
            int width  = ReadInt(n.Props, "width",  120);
            int height = ReadInt(n.Props, "height", 20);
            ClampRect(w.ClientWidth, w.ClientHeight, ref x, ref y, ref width, ref height);

            var input = new Input(w, (ushort)x, (ushort)y, (ushort)width, (ushort)height,
                                  ReadString(n.Props, "placeholder", ""))
            { Text = ReadString(n.Props, "text", "") };

            input.X = (ushort)x; input.Y = (ushort)y;
            input.Render();
        }

        private static void BuildButton(ScriptWindow w, ButtonNode n) => BuildButtonAbs(w, n, 0, 0);

        private static void BuildButtonAbs(ScriptWindow w, ButtonNode n, int ox, int oy)
        {
            int x = ReadInt(n.Props, "x", 10) + ox;
            int y = ReadInt(n.Props, "y", 10) + oy;
            int width  = ReadInt(n.Props, "width",  80);
            int height = ReadInt(n.Props, "height", 24);
            ClampRect(w.ClientWidth, w.ClientHeight, ref x, ref y, ref width, ref height);

            var btn = new Button(w, (ushort)x, (ushort)y, (ushort)width, (ushort)height,
                                 ReadString(n.Props, "text", n.Name))
            {
                UseSystemStyle   = ReadBool(n.Props, "useSystemStyle", true),
                BackgroundColour = ParseColour(ReadString(n.Props, "backgroundColour", "")) ?? GColor.Transparent,
                TextColour       = ParseColour(ReadString(n.Props, "textColour", "")) ?? GColor.White,
                RenderWithAlpha  = true
            };

            if (n.OnClick.Count > 0)
            {
                var cmds = n.OnClick;
                btn.Clicked = () => Execute(cmds, w);
            }

            btn.X = (ushort)x; btn.Y = (ushort)y;
            btn.Render();
        }

        private static void BuildCheckbox(ScriptWindow w, CheckboxNode n, int ox, int oy)
        {
            int x = ReadInt(n.Props, "x", 10) + ox;
            int y = ReadInt(n.Props, "y", 10) + oy;
            int width  = ReadInt(n.Props, "width",  120);
            int height = ReadInt(n.Props, "height", 20);
            ClampRect(w.ClientWidth, w.ClientHeight, ref x, ref y, ref width, ref height);

            string text = ReadString(n.Props, "text", n.Name);
            bool   chk  = ReadBool  (n.Props, "checked", false);
            var bgCol   = ParseColour(ReadString(n.Props, "backgroundColour", "")) ?? GColor.Transparent;
            var txtCol  = ParseColour(ReadString(n.Props, "textColour", "")) ?? GColor.Black;

            var btn = new Button(w, (ushort)x, (ushort)y, (ushort)width, (ushort)height, "")
            {
                UseSystemStyle   = false,
                BackgroundColour = bgCol,
                TextColour       = txtCol,
                RenderWithAlpha  = true
            };

            // No artificial offset: draw caption exactly at control origin.
            int overlayIndex = w.AddOverlayTextReturnIndex(x, y, FormatCheckbox(text, chk), txtCol);

            var item = new ScriptWindow.CheckboxItem
            {
                Button = btn, BaseText = text, Checked = chk,
                X = x, Y = y, W = width, H = height,
                OverlayIndex = overlayIndex, TextCol = txtCol
            };
            w.AddCheckbox(item);

            btn.Clicked = () =>
            {
                item.Checked = !item.Checked;
                w.UpdateOverlayText(item.OverlayIndex, FormatCheckbox(item.BaseText, item.Checked));
                btn.Render();
            };

            btn.X = (ushort)x; btn.Y = (ushort)y;
            btn.Render();
        }

        private static string FormatCheckbox(string text, bool isChecked) => isChecked ? "[x] " + text : "[ ] " + text;

        private static void BuildRadio(ScriptWindow w, RadioNode n, int ox, int oy)
        {
            int x = ReadInt(n.Props, "x", 10) + ox;
            int y = ReadInt(n.Props, "y", 10) + oy;
            int width  = ReadInt(n.Props, "width",  120);
            int height = ReadInt(n.Props, "height", 20);
            ClampRect(w.ClientWidth, w.ClientHeight, ref x, ref y, ref width, ref height);

            string text  = ReadString(n.Props, "text", n.Name);
            string group = ReadString(n.Props, "group", "default");
            bool   chk   = ReadBool  (n.Props, "checked", false);
            var txtCol   = ParseColour(ReadString(n.Props, "textColour", "")) ?? GColor.Black;
            var bgCol    = ParseColour(ReadString(n.Props, "backgroundColour", "")) ?? GColor.Transparent;

            var btn = new Button(w, (ushort)x, (ushort)y, (ushort)width, (ushort)height, "")
            {
                UseSystemStyle   = false,
                BackgroundColour = bgCol,
                TextColour       = txtCol,
                RenderWithAlpha  = true
            };

            // No artificial offset for radio caption either.
            int overlayIndex = w.AddOverlayTextReturnIndex(x, y, FormatRadio(text, chk), txtCol);

            var item = new ScriptWindow.RadioItem
            {
                Button = btn, BaseText = text, Group = group, Checked = chk,
                X = x, Y = y, W = width, H = height,
                OverlayIndex = overlayIndex, TextCol = txtCol
            };
            w.AddRadio(item);

            btn.Clicked = () => { w.SetRadioGroupCheckedRebuild(group, item); };

            btn.X = (ushort)x; btn.Y = (ushort)y;
            btn.Render();
        }

        private static string FormatRadio(string text, bool isChecked) => isChecked ? "(•) " + text : "( ) " + text;

        private static void BuildImage(ScriptWindow w, ImageNode n, int ox, int oy)
        {
            int x = ReadInt(n.Props, "x", 10) + ox;
            int y = ReadInt(n.Props, "y", 10) + oy;
            int width  = ReadInt(n.Props, "width",  64);
            int height = ReadInt(n.Props, "height", 64);
            ClampRect(w.ClientWidth, w.ClientHeight, ref x, ref y, ref width, ref height);

            string src = ReadString(n.Props, "src", "");
            w.AddImagePlaceholder(x, y, width, height, src);
        }

        private static void Execute(List<CommandNode> cmds, Window ctx)
        {
            for (int i=0;i<cmds.Count;i++)
            {
                var c = cmds[i];
                if (c is MessageCmd m) Dialogue.Show(ctx.Title, m.Text);
                else if (c is CloseCmd)
                {
                    if (ctx is ScriptWindow sw) sw.ExecuteOnCloseQueued();
                    ctx.Dispose();
                    return;
                }
            }
        }

        // ---- helpers ----
        private static void ClampPoint(int cw, int ch, ref int x, ref int y)
        {
            if (x < 0) x = 0; if (y < 0) y = 0;
            if (x > cw - 1) x = cw - 1;
            if (y > ch - 1) y = ch - 1;
        }

        private static void ClampRect(int cw, int ch, ref int x, ref int y, ref int w, ref int h)
        {
            if (w < 1) w = 1; if (h < 1) h = 1;
            if (x < 0) x = 0; if (y < 0) y = 0;          // only nudge if actually out of range
            if (x + w > cw) w = Math.Max(1, cw - x);     // trim to fit — do not move
            if (y + h > ch) h = Math.Max(1, ch - y);     // trim to fit — do not move
        }

        private static int ReadInt(Dictionary<string,string> map, string key, int def)
        { if (map.TryGetValue(key, out var v) && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)) return n; return def; }
        private static string ReadString(Dictionary<string,string> map, string key, string def)
        { if (map.TryGetValue(key, out var v)) return v; return def; }
        private static bool ReadBool(Dictionary<string,string> map, string key, bool def)
        {
            if (map.TryGetValue(key, out var v))
            {
                if (string.Equals(v,"true",StringComparison.OrdinalIgnoreCase) || v=="1") return true;
                if (string.Equals(v,"false",StringComparison.OrdinalIgnoreCase) || v=="0") return false;
            }
            return def;
        }
        private static bool ReadCentreFlag(Dictionary<string,string> map)
        {
            // Accept both British and American spellings, plus the X-suffixed variant.
            if (map.ContainsKey("centre") || map.ContainsKey("centreX") ||
                map.ContainsKey("center") || map.ContainsKey("centerX"))
                return ReadBool(map, "centre", true) || ReadBool(map, "centreX", true)
                    || ReadBool(map, "center", true) || ReadBool(map, "centerX", true);
            return false;
        }
        private static GColor? ParseColour(string v)
        {
            if (string.IsNullOrEmpty(v)) return null;
            var s = v.ToLowerInvariant();
            if (s=="white") return GColor.White;
            if (s=="black") return GColor.Black;
            if (s=="lightgray" || s=="lightgrey") return GColor.LightGray;
            if (s=="gray" || s=="grey") return GColor.DeepGray;
            if (s=="transparent") return GColor.Transparent;

            if (v[0]=='#' && v.Length==7) v = v.Substring(1);
            if (v.Length==6)
            {
                byte r = byte.Parse(v.Substring(0,2), NumberStyles.HexNumber);
                byte g = byte.Parse(v.Substring(2,2), NumberStyles.HexNumber);
                byte b = byte.Parse(v.Substring(4,2), NumberStyles.HexNumber);
                return new GColor(r,g,b);
            }
            return null;
        }
    }

    // ---------- host window ----------
    internal sealed class ScriptWindow : Window
    {
        // Store intended client size for all build-time maths (Contents.* may be 0 before first paint).
        public readonly int ClientWidth;
        public readonly int ClientHeight;

        public struct LabelSpec   { public int X; public int Y; public string Text; public GColor Colour; }
        public struct ImageSpec   { public int X; public int Y; public int W; public int H; public string Src; }
        public struct OverlayText { public int X; public int Y; public string Text; public GColor Colour; }

        internal sealed class CheckboxItem {
            public Button Button; public string BaseText; public bool Checked;
            public int X; public int Y; public int W; public int H;
            public int OverlayIndex; public GColor TextCol;
        }

        internal sealed class RadioItem {
            public Button Button; public string BaseText; public string Group; public bool Checked;
            public int X; public int Y; public int W; public int H;
            public int OverlayIndex; public GColor TextCol;
        }

        private readonly List<LabelSpec> _labels = new(32);
        private readonly List<(int x,int y,int w,int h,GColor col)> _fills = new(8);
        private readonly List<ImageSpec> _images = new(8);
        private readonly List<OverlayText> _overlays = new(32);

        private readonly List<CheckboxItem> _checkboxes = new(8);
        private readonly Dictionary<string, List<RadioItem>> _radioGroups = new(StringComparer.OrdinalIgnoreCase);

        private List<CommandNode> _onLoad  = null;
        private List<CommandNode> _onClose = null;

        public bool HasOnLoad  => _onLoad  != null && _onLoad.Count  > 0;
        public bool HasOnClose => _onClose != null && _onClose.Count > 0;

        public GColor? BackgroundColor;

        public ScriptWindow(ushort width, ushort height)
        {
            Contents    = new GCanvas(width, height);
            ClientWidth  = width;
            ClientHeight = height;

            Visible     = true;
            Closable    = true;
            HasTitlebar = true;
            Unkillable  = false;
        }

        public void AddLabel(LabelSpec spec) => _labels.Add(spec);

        public void AddPanelFill(int x,int y,int w,int h,GColor col)
        {
            if (w<=0 || h<=0) return;
            int cw = ClientWidth, ch = ClientHeight;
            if (x < 0) { w += x; x = 0; }
            if (y < 0) { h += y; y = 0; }
            if (x >= cw || y >= ch) return;
            if (x + w > cw) w = cw - x;
            if (y + h > ch) h = ch - y;
            if (w<=0 || h<=0) return;
            _fills.Add((x,y,w,h,col));
        }

        public void AddImagePlaceholder(int x, int y, int w, int h, string src)
        {
            _images.Add(new ImageSpec { X=x, Y=y, W=w, H=h, Src=src ?? string.Empty });
        }

        public int AddOverlayTextReturnIndex(int x, int y, string text, GColor col)
        {
            _overlays.Add(new OverlayText { X=x, Y=y, Text=text ?? string.Empty, Colour=col });
            return _overlays.Count - 1;
        }

        public void UpdateOverlayText(int index, string text)
        {
            if (index < 0 || index >= _overlays.Count) return;
            var o = _overlays[index];
            o.Text = text ?? string.Empty;
            _overlays[index] = o;
        }

        public void AddCheckbox(CheckboxItem item) => _checkboxes.Add(item);

        public void AddRadio(RadioItem item)
        {
            if (!_radioGroups.TryGetValue(item.Group, out var list))
            {
                list = new List<RadioItem>(4);
                _radioGroups[item.Group] = list;
            }
            list.Add(item);
        }

        public void SetRadioGroupCheckedRebuild(string group, RadioItem selected)
        {
            if (!_radioGroups.TryGetValue(group, out var list)) return;

            for (int i=0;i<list.Count;i++)
            {
                var it = list[i];
                bool shouldBeChecked = ReferenceEquals(it, selected) || ReferenceEquals(it.Button, selected.Button);
                if (it.Checked != shouldBeChecked)
                {
                    it.Checked = shouldBeChecked;
                    UpdateOverlayText(it.OverlayIndex, shouldBeChecked ? "(•) " + it.BaseText : "( ) " + it.BaseText);
                    it.Button.Render();
                }
                list[i] = it;
            }
        }

        public void QueueOnLoad (List<CommandNode> cmds){ _onLoad  = new List<CommandNode>(cmds); }
        public void QueueOnClose(List<CommandNode> cmds){ _onClose = new List<CommandNode>(cmds); }

        public void ExecuteOnLoadQueued()
        {
            if (_onLoad == null || _onLoad.Count == 0) return;
            var cmds = _onLoad;
            _onLoad = null;
            for (int i=0;i<cmds.Count;i++)
            {
                var c = cmds[i];
                if (c is MessageCmd m) Dialogue.Show(Title, m.Text);
                else if (c is CloseCmd) { Dispose(); return; }
            }
        }

        public void ExecuteOnCloseQueued()
        {
            if (_onClose == null || _onClose.Count == 0) return;
            var cmds = _onClose;
            _onClose = null;
            for (int i=0;i<cmds.Count;i++)
            {
                var c = cmds[i];
                if (c is MessageCmd m) Dialogue.Show(Title, m.Text);
            }
        }

        public override void Paint()
        {
            if (BackgroundColor.HasValue)
                Contents.DrawFilledRectangle(0, 0, (ushort)ClientWidth, (ushort)ClientHeight, 0, BackgroundColor.Value);

            // panel fills
            for (int i=0;i<_fills.Count;i++)
            {
                var r=_fills[i];
                Contents.DrawFilledRectangle(r.x, r.y, (ushort)r.w, (ushort)r.h, 0, r.col);
            }

            // image placeholders
            for (int i=0;i<_images.Count;i++)
            {
                var im = _images[i];
                var grey = new GColor(200,200,200);
                var dark = new GColor(120,120,120);
                Contents.DrawFilledRectangle(im.X, im.Y, (ushort)im.W, (ushort)im.H, 0, grey);
                Contents.DrawLine(im.X, im.Y, im.X + im.W - 1, im.Y, dark);
                Contents.DrawLine(im.X, im.Y, im.X, im.Y + im.H - 1, dark);
                Contents.DrawLine(im.X + im.W - 1, im.Y, im.X + im.W - 1, im.Y + im.H - 1, dark);
                Contents.DrawLine(im.X, im.Y + im.H - 1, im.X + im.W - 1, im.Y + im.H - 1, dark);
                Contents.DrawLine(im.X, im.Y, im.X + im.W - 1, im.Y + im.H - 1, dark);
                Contents.DrawLine(im.X + im.W - 1, im.Y, im.X, im.Y + im.H - 1, dark);
            }

            // labels
            for (int i=0;i<_labels.Count;i++)
            {
                var l = _labels[i];
                if (l.X >= 0 && l.Y >= 0)
                    Contents.DrawString(l.X, l.Y, l.Text ?? string.Empty, Font_1x, l.Colour, true);
            }

            // controls then overlay captions (so text sits above buttons)
            RenderControls();

            for (int i=0;i<_overlays.Count;i++)
            {
                var o = _overlays[i];
                Contents.DrawString(o.X, o.Y, o.Text ?? string.Empty, Font_1x, o.Colour, true);
            }

            RenderSystemStyleBorder();
        }
    }
}
