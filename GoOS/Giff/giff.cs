// File: GoOS/GUI/Giff/Giff.cs  (SAFE, no menubar integration; supports label, input, button)
using System;
using System.Collections.Generic;
using System.Globalization;

// Avoid Canvas ambiguity
using GCanvas = GoGL.Graphics.Canvas;
using GColor = GoGL.Graphics.Color;

using GoOS.GUI;          // Dialogue, Window, WindowManager
using GoGL.Graphics;     // Color
using static GoOS.Resources; // Font_1x

namespace GoOS.Giff
{
    public static class Giff
    {
        public static List<Window> Run(string script)
        {
            try
            {
                var lexer = new Lexer(script);
                var tokens = lexer.Tokenise();
                var parser = new Parser(tokens);
                var program = parser.ParseProgram();

                var built = new List<Window>(program.Windows.Count);
                foreach (var w in program.Windows)
                {
                    var win = Runtime.BuildWindow(w);
                    WindowManager.AddWindow(win);
                    built.Add(win);
                }
                return built;
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Failed to execute script:\n" + ex);
                return new List<Window>(0);
            }
        }
    }

    // ---------- Lexer (with progress guard) ----------
    internal enum TokKind { Eof, Ident, Number, String, LBrace, RBrace, Colon, Semi, Comma, Arrow, HashWord }
    internal readonly struct Tok
    {
        public readonly TokKind Kind; public readonly string Text; public readonly int Pos;
        public Tok(TokKind k, string t, int p) { Kind = k; Text = t; Pos = p; }
    }

    internal sealed class Lexer
    {
        private readonly string _s; private int _i;
        public Lexer(string s) { _s = s ?? string.Empty; _i = 0; }

        public List<Tok> Tokenise()
        {
            var t = new List<Tok>(256);
            int safety = Math.Max(256, _s.Length * 4); // hard ceiling

            while (safety-- > 0)
            {
                SkipWs();
                if (_i >= _s.Length) { t.Add(new Tok(TokKind.Eof, "", _i)); break; }
                int before = _i;
                char c = _s[_i];

                if (c == '{') { t.Add(new Tok(TokKind.LBrace, "{", _i++)); }
                else if (c == '}') { t.Add(new Tok(TokKind.RBrace, "}", _i++)); }
                else if (c == ':') { t.Add(new Tok(TokKind.Colon, ":", _i++)); }
                else if (c == ';') { t.Add(new Tok(TokKind.Semi, ";", _i++)); }
                else if (c == ',') { t.Add(new Tok(TokKind.Comma, ",", _i++)); }
                else if (c == '=' && Peek() == '>') { t.Add(new Tok(TokKind.Arrow, "=>", _i)); _i += 2; }
                else if (c == '"' || c == '\'') { t.Add(ReadString()); }
                else if (c == '#') { t.Add(ReadHashWord()); }
                else if (char.IsDigit(c))
                {
                    int s0 = _i; while (_i < _s.Length && char.IsDigit(_s[_i])) _i++;
                    t.Add(new Tok(TokKind.Number, _s.Substring(s0, _i - s0), s0));
                }
                else if (char.IsLetter(c) || c == '_' || c == '.')
                {
                    int s0 = _i;
                    while (_i < _s.Length)
                    {
                        char ch = _s[_i];
                        if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.') _i++;
                        else break;
                    }
                    t.Add(new Tok(TokKind.Ident, _s.Substring(s0, _i - s0), s0));
                }
                else
                {
                    _i++; // skip unknown
                }

                if (_i <= before) _i++; // force progress
            }

            if (safety <= 0)
                t.Add(new Tok(TokKind.Eof, "", _i));

            return t;
        }

        private void SkipWs()
        {
            while (_i < _s.Length)
            {
                char c = _s[_i];
                if (char.IsWhiteSpace(c)) { _i++; continue; }
                if (c == '/' && Peek() == '/')
                {
                    _i += 2; while (_i < _s.Length && _s[_i] != '\n') _i++; continue;
                }
                break;
            }
        }

        private Tok ReadString()
        {
            char q = _s[_i++]; var buf = new System.Text.StringBuilder(); int start = _i;
            while (_i < _s.Length)
            {
                char c = _s[_i++];
                if (c == q) break;
                if (c == '\\' && _i < _s.Length)
                {
                    char e = _s[_i++];
                    buf.Append(e switch { 'n' => '\n', 'r' => '\r', 't' => '\t', '\\' => '\\', '"' => '"', '\'' => '\'', _ => e });
                }
                else buf.Append(c);
            }
            return new Tok(TokKind.String, buf.ToString(), start - 1);
        }

        private Tok ReadHashWord()
        {
            int s0 = _i++; // include '#'
            while (_i < _s.Length)
            {
                char ch = _s[_i];
                if (char.IsLetterOrDigit(ch)) { _i++; continue; }
                break;
            }
            return new Tok(TokKind.HashWord, _s.Substring(s0, _i - s0), s0);
        }

        private char Peek() => (_i + 1 < _s.Length) ? _s[_i + 1] : '\0';
    }

    // ---------- AST ----------
    internal sealed class ProgramNode { public readonly List<WindowNode> Windows = new(4); }

    internal sealed class WindowNode
    {
        public string Name = "Window";
        public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);
        public readonly List<LabelNode> Labels = new(8);
        public readonly List<InputNode> Inputs = new(8);
        public readonly List<ButtonNode> Buttons = new(8);
        public readonly List<CommandNode> OnLoad = new(4);
    }

    internal sealed class LabelNode
    {
        public string Name = "label";
        public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);
    }

    internal sealed class InputNode
    {
        public string Name = "input";
        public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);
    }

    internal sealed class ButtonNode
    {
        public string Name = "button";
        public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);
        public readonly List<CommandNode> OnClick = new(4);
    }

    internal abstract class CommandNode { }
    internal sealed class MessageCmd : CommandNode { public string Text; public MessageCmd(string t) { Text = t; } }
    internal sealed class CloseCmd : CommandNode { }

    // ---------- Parser (with safety) ----------
    internal sealed class Parser
    {
        private readonly List<Tok> _t; private int _i;
        public Parser(List<Tok> t) { _t = t; _i = 0; }

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
            if (Peek().Kind == TokKind.Ident) w.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(64, _t.Count * 2);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("label")) { w.Labels.Add(ParseLabel()); continue; }
                if (TryMatchIdent("input")) { w.Inputs.Add(ParseInput()); continue; }
                if (TryMatchIdent("button")) { w.Buttons.Add(ParseButton()); continue; }
                if (TryMatchIdent("onLoad")) { ParseBlockInto(w.OnLoad); continue; }
                if (Peek().Kind == TokKind.Ident) { ParsePropertyInto(w.Props); continue; }
                _i++;
            }
            return w;
        }

        private LabelNode ParseLabel()
        {
            var n = new LabelNode();
            if (Peek().Kind == TokKind.Ident) n.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (Peek().Kind == TokKind.Ident) { ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private InputNode ParseInput()
        {
            var n = new InputNode();
            if (Peek().Kind == TokKind.Ident) n.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (Peek().Kind == TokKind.Ident) { ParsePropertyInto(n.Props); continue; }
                _i++;
            }
            return n;
        }

        private ButtonNode ParseButton()
        {
            var b = new ButtonNode();
            if (Peek().Kind == TokKind.Ident) b.Name = Next().Text;
            Expect(TokKind.LBrace);

            int safety = Math.Max(16, _t.Count);
            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("onClick")) { ParseBlockInto(b.OnClick); continue; }
                if (Peek().Kind == TokKind.Ident) { ParsePropertyInto(b.Props); continue; }
                _i++;
            }
            return b;
        }

        private void ParsePropertyInto(Dictionary<string, string> dict)
        {
            var key = Expect(TokKind.Ident);
            Expect(TokKind.Colon);
            var valTok = Next(); // string | number | ident | hashword
            dict[key.Text] = valTok.Text;
            if (Peek().Kind == TokKind.Semi) _i++; // optional ';'
        }

        private void ParseBlockInto(List<CommandNode> list)
        {
            Expect(TokKind.LBrace);
            int safety = Math.Max(16, _t.Count);

            while (!AtEnd() && !Match(TokKind.RBrace) && safety-- > 0)
            {
                SkipSeps();
                if (TryMatchIdent("message")) { var s = Expect(TokKind.String); list.Add(new MessageCmd(s.Text)); if (Peek().Kind == TokKind.Semi) _i++; continue; }
                if (TryMatchIdent("close")) { list.Add(new CloseCmd()); if (Peek().Kind == TokKind.Semi) _i++; continue; }
                _i++;
            }
        }

        private void SkipSeps()
        {
            while (Peek().Kind == TokKind.Semi || Peek().Kind == TokKind.Comma)
                _i++;
        }

        private Tok Peek() => (_i < _t.Count) ? _t[_i] : new Tok(TokKind.Eof, "", _i);
        private Tok Next() { var x = Peek(); if (_i < _t.Count) _i++; return x; }
        private Tok Expect(TokKind k) { var t = Next(); return t.Kind == k ? t : new Tok(k, "", t.Pos); }
        private bool Match(TokKind k) { if (Peek().Kind == k) { _i++; return true; } return false; }
        private bool TryMatchIdent(string s) { var t = Peek(); if (t.Kind == TokKind.Ident && string.Equals(t.Text, s, StringComparison.OrdinalIgnoreCase)) { _i++; return true; } return false; }
        private bool AtEnd() => Peek().Kind == TokKind.Eof;
    }

    // ---------- Runtime ----------
    internal static class Runtime
    {
        public static Window BuildWindow(WindowNode n)
        {
            try
            {
                int width = ReadInt(n.Props, "width", 320);
                int height = ReadInt(n.Props, "height", 240);

                var win = new ScriptWindow((ushort)width, (ushort)height)
                {
                    Title = ReadString(n.Props, "title", n.Name),
                    Closable = ReadBool(n.Props, "closable", true),
                    HasTitlebar = ReadBool(n.Props, "titlebar", true),
                    Visible = true
                };

                // Position
                win.X = ReadInt(n.Props, "x", 40);
                win.Y = ReadInt(n.Props, "y", 40);

                // Background colour (stored; ScriptWindow repaints every Paint)
                var bg = ParseColour(ReadString(n.Props, "background", ""));
                if (bg.HasValue) win.BackgroundColor = bg.Value;

                // Labels (drawn in Paint)
                foreach (var l in n.Labels)
                    BuildLabel(win, l);

                // Inputs (real controls)
                foreach (var i in n.Inputs)
                    BuildInput(win, i);

                // Buttons (real controls)
                foreach (var b in n.Buttons)
                    BuildButton(win, b);

                // onLoad — defer to run loop so we don’t throw dialogues during construction
                if (n.OnLoad.Count > 0)
                    win.DeferredCommands = n.OnLoad;

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

        private static void BuildLabel(ScriptWindow w, LabelNode n)
        {
            try
            {
                int x = ReadInt(n.Props, "x", 10);
                int y = ReadInt(n.Props, "y", 10);
                string text = ReadString(n.Props, "text", n.Name);
                var col = ParseColour(ReadString(n.Props, "textColour", "")) ?? GColor.Black;

                w.AddLabel(new ScriptWindow.LabelSpec
                {
                    X = x,
                    Y = y,
                    Text = text,
                    Colour = col
                });
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Label build failed:\n" + ex);
            }
        }

        private static void BuildInput(Window w, InputNode n)
        {
            try
            {
                int x = ReadInt(n.Props, "x", 10);
                int y = ReadInt(n.Props, "y", 10);
                int width = ReadInt(n.Props, "width", 120);
                int height = ReadInt(n.Props, "height", 20);
                string placeholder = ReadString(n.Props, "placeholder", "");
                string text = ReadString(n.Props, "text", "");

                var input = new Input(w, (ushort)x, (ushort)y, (ushort)width, (ushort)height, placeholder)
                {
                    Text = text
                };

                // >>> Ensure position is applied even if ctor ignores it
                input.X = (ushort)x;
                input.Y = (ushort)y;

                input.Render();
                w.RenderControls();
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Input build failed:\n" + ex);
            }
        }


        private static void BuildButton(Window w, ButtonNode n)
        {
            try
            {
                int x = ReadInt(n.Props, "x", 10);
                int y = ReadInt(n.Props, "y", 10);
                int width = ReadInt(n.Props, "width", 80);
                int height = ReadInt(n.Props, "height", 24);
                string text = ReadString(n.Props, "text", n.Name);

                var btn = new Button(w, (ushort)x, (ushort)y, (ushort)width, (ushort)height, text)
                {
                    UseSystemStyle = ReadBool(n.Props, "useSystemStyle", true),
                    BackgroundColour = ParseColour(ReadString(n.Props, "backgroundColour", "")) ?? GColor.Transparent,
                    TextColour = ParseColour(ReadString(n.Props, "textColour", "")) ?? GColor.White,
                    RenderWithAlpha = true
                };

                // >>> Ensure position is applied even if ctor ignores it
                btn.X = (ushort)x;
                btn.Y = (ushort)y;

                if (n.OnClick.Count > 0)
                {
                    var cmds = n.OnClick; // capture
                    btn.Clicked = () => ExecuteCommands(cmds, w);
                }

                btn.Render();
                w.RenderControls();
            }
            catch (Exception ex)
            {
                Dialogue.Show("Giff", "Button build failed:\n" + ex);
            }
        }


        private static void ExecuteCommands(List<CommandNode> cmds, Window ctx)
        {
            foreach (var c in cmds)
            {
                if (c is MessageCmd m) Dialogue.Show(ctx.Title, m.Text);
                else if (c is CloseCmd) ctx.Dispose();
            }
        }

        private static int ReadInt(Dictionary<string, string> map, string key, int def)
        {
            if (map.TryGetValue(key, out var v) && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
                return n;
            return def;
        }
        private static string ReadString(Dictionary<string, string> map, string key, string def)
        {
            if (map.TryGetValue(key, out var v)) return v;
            return def;
        }
        private static bool ReadBool(Dictionary<string, string> map, string key, bool def)
        {
            if (map.TryGetValue(key, out var v))
            {
                if (string.Equals(v, "true", StringComparison.OrdinalIgnoreCase) || v == "1") return true;
                if (string.Equals(v, "false", StringComparison.OrdinalIgnoreCase) || v == "0") return false;
            }
            return def;
        }

        private static GColor? ParseColour(string v)
        {
            if (string.IsNullOrEmpty(v)) return null;
            var s = v.ToLowerInvariant();

            if (s == "white") return GColor.White;
            if (s == "black") return GColor.Black;
            if (s == "lightgray" || s == "lightgrey") return GColor.LightGray;
            if (s == "transparent") return GColor.Transparent;

            if (v[0] == '#' && v.Length == 7) v = v.Substring(1);
            if (v.Length == 6)
            {
                byte r = byte.Parse(v.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(v.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(v.Substring(4, 2), NumberStyles.HexNumber);
                return new GColor(r, g, b); // RGB ctor (opaque)
            }

            return null;
        }
    }

    // ---------- Script-host window (labels drawn in Paint; deferred onLoad) ----------
    internal sealed class ScriptWindow : Window
    {
        public struct LabelSpec
        {
            public int X;
            public int Y;
            public string Text;
            public GColor Colour;
        }

        public GColor? BackgroundColor;
        public List<CommandNode> DeferredCommands;
        private bool _ranDeferred;

        private readonly List<LabelSpec> _labels = new List<LabelSpec>(16);

        public ScriptWindow(ushort width, ushort height)
        {
            Contents = new GCanvas(width, height);
            Visible = true;
            Closable = true;
            HasTitlebar = true;
            Unkillable = false;
        }

        public void AddLabel(LabelSpec spec) => _labels.Add(spec);

        public override void Paint()
        {
            if (BackgroundColor.HasValue)
                Contents.Clear(BackgroundColor.Value);

            // draw labels
            for (int i = 0; i < _labels.Count; i++)
            {
                var l = _labels[i];
                Contents.DrawString(l.X, l.Y, l.Text ?? string.Empty, Font_1x, l.Colour, true);
            }

            RenderControls();
            RenderSystemStyleBorder();
        }

        public override void HandleRun()
        {
            base.HandleRun();

            if (!_ranDeferred && DeferredCommands != null && DeferredCommands.Count > 0)
            {
                _ranDeferred = true;
                try
                {
                    foreach (var c in DeferredCommands)
                    {
                        if (c is MessageCmd m) Dialogue.Show(Title, m.Text);
                        else if (c is CloseCmd) { Dispose(); return; }
                    }
                }
                catch { /* keep UI alive */ }
            }
        }
    }
}
