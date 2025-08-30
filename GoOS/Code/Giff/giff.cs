// File: GoOS/GUI/Giff/Giff.cs
// British English implementation for GoOS.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GoOS.GUI;
using Gold.Graphics;
using static GoOS.Resources;

namespace GoOS.Giff
{
    public static class Giff
    {
        public static List<Window> Run(string script)
        {
            var built = new List<Window>(4);
            try
            {
                var tokens = new Lexer(script).Tokenise();
                var program = new Parser(tokens).ParseProgram();

                for (int i = 0; i < program.Windows.Count; i++)
                {
                    var w = Runtime.BuildWindow(program.Windows[i]);
                    WindowManager.AddWindow(w);
                    built.Add(w);
                }
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Failed to execute script:\n" + ex);
            }
            return built;
        }
    }

    // ---------- Lexer ----------
    internal enum TokKind { Eof, Ident, Number, String, LBrace, RBrace, Equals, Colon, Semi, Comma, HashWord }
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
                else if (c=='=') t.Add(new Tok(TokKind.Equals,"=",_i++));
                else if (c==':') t.Add(new Tok(TokKind.Colon, ":",_i++));
                else if (c==';') t.Add(new Tok(TokKind.Semi,  ";",_i++));
                else if (c==',') t.Add(new Tok(TokKind.Comma, ",",_i++));
                else if (c=='"' || c=='\'') t.Add(ReadString());
                else if (c=='#') t.Add(ReadHashWord());
                else if (char.IsDigit(c) || ((c=='-'||c=='+') && (_i+1<_s.Length) && char.IsDigit(_s[_i+1])))
                {
                    int s0=_i;
                    if (c=='-'||c=='+') _i++;
                    while(_i<_s.Length && char.IsDigit(_s[_i])) _i++;
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
                if (c=='/' && _i+1<_s.Length && _s[_i+1]=='/')
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
    }

    // ---------- AST ----------
    internal sealed class ProgramNode { public readonly List<WindowNode> Windows = new List<WindowNode>(4); }

    internal sealed class WindowNode
    {
        public string Name = "Window";
        public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);

        public readonly List<LabelNode>   Labels     = new List<LabelNode>(8);
        public readonly List<InputNode>   Inputs     = new List<InputNode>(8);
        public readonly List<ButtonNode>  Buttons    = new List<ButtonNode>(8);
        public readonly List<PanelNode>   Panels     = new List<PanelNode>(4);
        public readonly List<CheckboxNode>Checkboxes = new List<CheckboxNode>(8);
        public readonly List<RadioNode>   Radios     = new List<RadioNode>(8);
        public readonly List<ImageNode>   Images     = new List<ImageNode>(8);

        public readonly List<CommandNode> OnLoad  = new List<CommandNode>(4);
        public readonly List<CommandNode> OnClose = new List<CommandNode>(4);
    }

    internal sealed class LabelNode   { public string Name="label";   public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase); }
    internal sealed class InputNode   { public string Name="input";   public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase); }
    internal sealed class ButtonNode  { public string Name="button";  public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase); public readonly List<CommandNode> OnClick = new List<CommandNode>(4); }
    internal sealed class CheckboxNode{ public string Name="checkbox";public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase); }
    internal sealed class RadioNode   { public string Name="radio";   public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase); }
    internal sealed class ImageNode   { public string Name="image";   public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase); }

    internal sealed class PanelNode
    {
        public readonly Dictionary<string,string> Props = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
        public readonly List<LabelNode>   Labels     = new List<LabelNode>(8);
        public readonly List<InputNode>   Inputs     = new List<InputNode>(8);
        public readonly List<ButtonNode>  Buttons    = new List<ButtonNode>(8);
        public readonly List<CheckboxNode>Checkboxes = new List<CheckboxNode>(8);
        public readonly List<RadioNode>   Radios     = new List<RadioNode>(8);
        public readonly List<ImageNode>   Images     = new List<ImageNode>(4);
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
            Require(TokKind.LBrace, "expected '{' to start window block");

            int safety = Math.Max(64, _t.Count * 2);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (TryMatchIdent("label"))    { w.Labels.Add(ParseLabel());       continue; }
                if (TryMatchIdent("input"))    { w.Inputs.Add(ParseInput());       continue; }
                if (TryMatchIdent("button"))   { w.Buttons.Add(ParseButton());     continue; }
                if (TryMatchIdent("panel"))    { w.Panels.Add(ParsePanel());       continue; }
                if (TryMatchIdent("checkbox")) { w.Checkboxes.Add(ParseCheckbox());continue; }
                if (TryMatchIdent("radio"))    { w.Radios.Add(ParseRadio());       continue; }
                if (TryMatchIdent("image"))    { w.Images.Add(ParseImage());       continue; }
                if (TryMatchIdent("onLoad"))   { ParseBlockInto(w.OnLoad);         continue; }
                if (TryMatchIdent("onClose"))  { ParseBlockInto(w.OnClose);        continue; }

                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(w.Props); continue; }
                _i++; // progress guard
            }
            return w;
        }

        private PanelNode ParsePanel()
        {
            var p = new PanelNode();
            Require(TokKind.LBrace, "expected '{' to start panel block");
            int safety = Math.Max(32, _t.Count);

            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (TryMatchIdent("label"))    { p.Labels.Add(ParseLabel());       continue; }
                if (TryMatchIdent("input"))    { p.Inputs.Add(ParseInput());       continue; }
                if (TryMatchIdent("button"))   { p.Buttons.Add(ParseButton());     continue; }
                if (TryMatchIdent("checkbox")) { p.Checkboxes.Add(ParseCheckbox());continue; }
                if (TryMatchIdent("radio"))    { p.Radios.Add(ParseRadio());       continue; }
                if (TryMatchIdent("image"))    { p.Images.Add(ParseImage());       continue; }

                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(p.Props); continue; }
                _i++;
            }
            return p;
        }

        private LabelNode ParseLabel()
        {
            var n = new LabelNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Require(TokKind.LBrace, "expected '{' to start label block");

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private InputNode ParseInput()
        {
            var n = new InputNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Require(TokKind.LBrace, "expected '{' to start input block");

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private ButtonNode ParseButton()
        {
            var b = new ButtonNode();
            if (Peek().Kind==TokKind.Ident) b.Name = Next().Text;
            Require(TokKind.LBrace, "expected '{' to start button block");

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

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
            Require(TokKind.LBrace, "expected '{' to start checkbox block");

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private RadioNode ParseRadio()
        {
            var n = new RadioNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Require(TokKind.LBrace, "expected '{' to start radio block");

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private ImageNode ParseImage()
        {
            var n = new ImageNode();
            if (Peek().Kind==TokKind.Ident) n.Name = Next().Text;
            Require(TokKind.LBrace, "expected '{' to start image block");

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (Peek().Kind==TokKind.Ident){ ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private void ParseBlockInto(List<CommandNode> list)
        {
            Require(TokKind.LBrace, "expected '{' to start block");
            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && safety-- > 0)
            {
                SkipSeps();
                if (Match(TokKind.RBrace)) break;

                if (TryMatchIdent("message")){ var s = Require(TokKind.String, "expected string after message"); list.Add(new MessageCmd(s.Text)); if (Peek().Kind==TokKind.Semi) _i++; continue; }
                if (TryMatchIdent("close"))  { list.Add(new CloseCmd()); if (Peek().Kind==TokKind.Semi) _i++; continue; }
                _i++;
            }
        }

        // Accepts: key = value  |  key : value  |  key value
        private void ParsePropertyInto(Dictionary<string,string> dict)
        {
            var keyTok = Require(TokKind.Ident, "expected property name");
            string key = keyTok.Text.ToLowerInvariant(); // Ensure lowercase key

            // Optional '=' or ':'.
            if (Peek().Kind == TokKind.Equals || Peek().Kind == TokKind.Colon)
                _i++;

            var valTok = Next();
            if (!(valTok.Kind==TokKind.String || valTok.Kind==TokKind.Number || valTok.Kind==TokKind.Ident || valTok.Kind==TokKind.HashWord))
                throw new Exception("Giff: expected a value for '" + key + "'.");

            // Store with lowercase key for consistent access
            dict[key] = valTok.Text;
            if (Peek().Kind==TokKind.Semi) _i++; // optional ;
        }

        private void SkipSeps(){ while (Peek().Kind==TokKind.Semi || Peek().Kind==TokKind.Comma) _i++; }
        private Tok Peek(){ return _i<_t.Count ? _t[_i] : new Tok(TokKind.Eof,"",_i); }
        private Tok Next(){ var x=Peek(); if (_i<_t.Count) _i++; return x; }
        private Tok Require(TokKind k, string msg)
        {
            var t = Next();
            if (t.Kind != k) throw new Exception("Giff parse error near " + t.Pos + ": " + msg);
            return t;
        }
        private bool Match(TokKind k){ if (Peek().Kind==k){ _i++; return true; } return false; }
        private bool TryMatchIdent(string s){ var t=Peek(); if (t.Kind==TokKind.Ident && string.Equals(t.Text,s,StringComparison.OrdinalIgnoreCase)){ _i++; return true; } return false; }
        private bool AtEnd(){ return Peek().Kind==TokKind.Eof; }
    }

    // ---------- Runtime ----------
    internal static class Runtime
    {
        public static Window BuildWindow(WindowNode n)
        {
            try
            {
                int width = ParseInt(n.Props, "width", 320);
                int height = ParseInt(n.Props, "height", 240);

                var win = new Window()
                {
                    Title = ParseString(n.Props, "title", n.Name),
                    Contents = new Canvas((ushort)width, (ushort)height),
                    Closable = ParseBool(n.Props, "closable", true),
                    HasTitlebar = ParseBool(n.Props, "titlebar", true),
                    Visible = true
                };

                win.X = ParseInt(n.Props, "x", 40);
                win.Y = ParseInt(n.Props, "y", 40);

                var bg = ParseColour(ParseString(n.Props, "background", ""));
                if (bg.HasValue)
                {
                    win.Contents.Clear(bg.Value);
                }

                // Add buttons with explicit position forcing
                foreach (var btn in n.Buttons)
                {
                    int x = ParseInt(btn.Props, "x", 0);
                    int y = ParseInt(btn.Props, "y", 0);
                    int w = ParseInt(btn.Props, "width", 80);
                    int h = ParseInt(btn.Props, "height", 24);
                    string text = ParseString(btn.Props, "text", btn.Name);
                    
                    // Create button at position specified
                    var button = new Button(win, 0, 0, (ushort)w, (ushort)h, text);
                    
                    // Explicitly set position AFTER creation (key fix)
                    button.X = (ushort)x;
                    button.Y = (ushort)y;
                    
                    // Set appearance
                    button.UseSystemStyle = ParseBool(btn.Props, "useSystemStyle", true);
                    button.BackgroundColour = ParseColour(ParseString(btn.Props, "backgroundColour", "")) ?? Color.Transparent;
                    button.TextColour = ParseColour(ParseString(btn.Props, "textColour", "")) ?? Color.White;
                    
                    // Add click handler
                    if (btn.OnClick.Count > 0)
                    {
                        var cmds = btn.OnClick;
                        button.Clicked = () => Execute(cmds, win);
                    }
                    
                    // Force render immediately
                    button.Render();
                }

                // Process other elements
                foreach (var label in n.Labels)
                {
                    BuildLabel(win, label);
                }

                foreach (var input in n.Inputs)
                {
                    BuildInput(win, input);
                }

                foreach (var checkbox in n.Checkboxes)
                {
                    BuildCheckbox(win, checkbox);
                }

                foreach (var radio in n.Radios)
                {
                    BuildRadio(win, radio);
                }

                foreach (var img in n.Images)
                {
                    BuildImage(win, img);
                }

                foreach (var panel in n.Panels)
                {
                    BuildPanel(win, panel);
                }

                // Execute onLoad commands if present
                if (n.OnLoad.Count > 0)
                {
                    Execute(n.OnLoad, win);
                }

                return win;
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Script error: " + ex.Message);
                var w = new Window() { Title = "Giff Error", Visible = true };
                w.Contents = new Canvas(200, 80);
                w.Contents.Clear(new Color(64, 0, 0));
                return w;
            }
        }

        private static void BuildLabel(Window win, LabelNode n)
        {
            int x = ParseInt(n.Props, "x", 0);
            int y = ParseInt(n.Props, "y", 0);
            string text = ParseString(n.Props, "text", n.Name);
            var col = ParseColour(ParseString(n.Props, "textColour", "")) ?? Color.Black;
            
            bool centre = ParseCentreFlag(n.Props);
            if (centre)
            {
                int textWidth = Charcoal.MeasureString(text);
                x = (win.Contents.Width - textWidth) / 2;
            }
            
            win.Contents.DrawString(x, y, text, Charcoal, col, false);
        }

        private static void BuildInput(Window win, InputNode n)
        {
            int x = ParseInt(n.Props, "x", 0);
            int y = ParseInt(n.Props, "y", 0);
            int w = ParseInt(n.Props, "width", 120);
            int h = ParseInt(n.Props, "height", 20);
            string placeholder = ParseString(n.Props, "placeholder", "");
            string text = ParseString(n.Props, "text", "");
            
            var input = new Input(win, 0, 0, (ushort)w, (ushort)h, placeholder);
            input.X = (ushort)x;
            input.Y = (ushort)y;
            input.Text = text;
            input.Render();
        }

        private static void BuildCheckbox(Window win, CheckboxNode n)
        {
            int x = ParseInt(n.Props, "x", 0);
            int y = ParseInt(n.Props, "y", 0);
            string text = ParseString(n.Props, "text", n.Name);
            bool isChecked = ParseBool(n.Props, "checked", false);
            var textCol = ParseColour(ParseString(n.Props, "textColour", "")) ?? Color.Black;
            
            win.Contents.DrawString(x, y, (isChecked ? "[x] " : "[ ] ") + text, Charcoal, textCol, false);
        }

        private static void BuildRadio(Window win, RadioNode n)
        {
            int x = ParseInt(n.Props, "x", 0);
            int y = ParseInt(n.Props, "y", 0);
            string text = ParseString(n.Props, "text", n.Name);
            bool isChecked = ParseBool(n.Props, "checked", false);
            var textCol = ParseColour(ParseString(n.Props, "textColour", "")) ?? Color.Black;
            
            win.Contents.DrawString(x, y, (isChecked ? "(•) " : "( ) ") + text, Charcoal, textCol, false);
        }

        private static void BuildImage(Window win, ImageNode n)
        {
            int x = ParseInt(n.Props, "x", 0);
            int y = ParseInt(n.Props, "y", 0);
            int w = ParseInt(n.Props, "width", 64);
            int h = ParseInt(n.Props, "height", 64);
            
            // Draw image placeholder
            var grey = new Color(200, 200, 200);
            var dark = new Color(120, 120, 120);
            win.Contents.DrawFilledRectangle(x, y, (ushort)w, (ushort)h, 0, grey);
            win.Contents.DrawRectangle(x, y, (ushort)w, (ushort)h, 0, dark);
            win.Contents.DrawLine(x, y, x + w - 1, y + h - 1, dark);
            win.Contents.DrawLine(x + w - 1, y, x, y + h - 1, dark);
        }

        private static void BuildPanel(Window win, PanelNode p)
        {
            int px = ParseInt(p.Props, "x", 0);
            int py = ParseInt(p.Props, "y", 0);
            int pW = ParseInt(p.Props, "width", 200);
            int pH = ParseInt(p.Props, "height", 120);

            var pbg = ParseColour(ParseString(p.Props, "background", ""));
            if (pbg.HasValue)
            {
                win.Contents.DrawFilledRectangle(px, py, (ushort)pW, (ushort)pH, 0, pbg.Value);
            }

            // Panel labels
            foreach (var label in p.Labels)
            {
                int x = ParseInt(label.Props, "x", 0) + px;
                int y = ParseInt(label.Props, "y", 0) + py;
                string text = ParseString(label.Props, "text", label.Name);
                var col = ParseColour(ParseString(label.Props, "textColour", "")) ?? Color.Black;
                
                win.Contents.DrawString(x, y, text, Charcoal, col, false);
            }

            // Panel buttons - similar approach as main buttons
            foreach (var btn in p.Buttons)
            {
                int x = ParseInt(btn.Props, "x", 0) + px;
                int y = ParseInt(btn.Props, "y", 0) + py;
                int w = ParseInt(btn.Props, "width", 80);
                int h = ParseInt(btn.Props, "height", 24);
                string text = ParseString(btn.Props, "text", btn.Name);
                
                var button = new Button(win, 0, 0, (ushort)w, (ushort)h, text);
                button.X = (ushort)x;
                button.Y = (ushort)y;
                button.UseSystemStyle = ParseBool(btn.Props, "useSystemStyle", true);
                button.BackgroundColour = ParseColour(ParseString(btn.Props, "backgroundColour", "")) ?? Color.Transparent;
                button.TextColour = ParseColour(ParseString(btn.Props, "textColour", "")) ?? Color.White;

                if (btn.OnClick.Count > 0)
                {
                    var cmds = btn.OnClick;
                    button.Clicked = () => Execute(cmds, win);
                }
                
                button.Render();
            }

            // Panel inputs
            foreach (var input in p.Inputs)
            {
                int x = ParseInt(input.Props, "x", 0) + px;
                int y = ParseInt(input.Props, "y", 0) + py;
                int w = ParseInt(input.Props, "width", 120);
                int h = ParseInt(input.Props, "height", 20);
                string placeholder = ParseString(input.Props, "placeholder", "");
                string text = ParseString(input.Props, "text", "");
                
                var inp = new Input(win, 0, 0, (ushort)w, (ushort)h, placeholder);
                inp.X = (ushort)x;
                inp.Y = (ushort)y;
                inp.Text = text;
                inp.Render();
            }

            // Other panel elements
            foreach (var checkbox in p.Checkboxes)
            {
                int x = ParseInt(checkbox.Props, "x", 0) + px;
                int y = ParseInt(checkbox.Props, "y", 0) + py;
                string text = ParseString(checkbox.Props, "text", checkbox.Name);
                bool isChecked = ParseBool(checkbox.Props, "checked", false);
                var textCol = ParseColour(ParseString(checkbox.Props, "textColour", "")) ?? Color.Black;
                
                win.Contents.DrawString(x, y, (isChecked ? "[x] " : "[ ] ") + text, Charcoal, textCol, false);
            }

            foreach (var radio in p.Radios)
            {
                int x = ParseInt(radio.Props, "x", 0) + px;
                int y = ParseInt(radio.Props, "y", 0) + py;
                string text = ParseString(radio.Props, "text", radio.Name);
                bool isChecked = ParseBool(radio.Props, "checked", false);
                var textCol = ParseColour(ParseString(radio.Props, "textColour", "")) ?? Color.Black;
                
                win.Contents.DrawString(x, y, (isChecked ? "(•) " : "( ) ") + text, Charcoal, textCol, false);
            }
        }

        public static void Execute(List<CommandNode> cmds, Window ctx)
        {
            foreach (var cmd in cmds)
            {
                if (cmd is MessageCmd m) 
                    Dialogue.Show(ctx.Title, m.Text);
                else if (cmd is CloseCmd)
                {
                    ctx.Dispose();
                    return;
                }
            }
        }

        // Value parsers with consistent case handling
        private static int ParseInt(Dictionary<string, string> props, string key, int defaultValue)
        {
            // Use lowercase key for consistent lookup
            key = key.ToLowerInvariant();
            
            if (props.TryGetValue(key, out string value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                return result;
                
            return defaultValue;
        }

        private static string ParseString(Dictionary<string, string> props, string key, string defaultValue)
        {
            // Use lowercase key for consistent lookup
            key = key.ToLowerInvariant();
            
            if (props.TryGetValue(key, out string value))
                return value;
                
            return defaultValue;
        }

        private static bool ParseBool(Dictionary<string, string> props, string key, bool defaultValue)
        {
            // Use lowercase key for consistent lookup
            key = key.ToLowerInvariant();
            
            if (props.TryGetValue(key, out string value))
            {
                if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1")
                    return true;
                if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase) || value == "0")
                    return false;
            }
            
            return defaultValue;
        }

        private static bool ParseCentreFlag(Dictionary<string, string> props)
        {
            return ParseBool(props, "centre", false) || 
                   ParseBool(props, "centrex", false) ||
                   ParseBool(props, "center", false) || 
                   ParseBool(props, "centerx", false);
        }

        private static Color? ParseColour(string v)
        {
            if (string.IsNullOrEmpty(v)) return null;
            var s = v.ToLowerInvariant();
            if (s=="white") return Color.White;
            if (s=="black") return Color.Black;
            if (s=="lightgray" || s=="lightgrey") return Color.LightGray;
            if (s=="gray" || s=="grey") return Color.DeepGray;
            if (s=="transparent") return Color.Transparent;

            if (v[0]=='#' && v.Length==7) v = v.Substring(1);
            if (v.Length==6)
            {
                try {
                    byte r = byte.Parse(v.Substring(0,2), NumberStyles.HexNumber);
                    byte g = byte.Parse(v.Substring(2,2), NumberStyles.HexNumber);
                    byte b = byte.Parse(v.Substring(4,2), NumberStyles.HexNumber);
                    return new Color(r,g,b);
                }
                catch {
                    return null;
                }
            }
            return null;
        }
    }
}