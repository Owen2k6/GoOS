// File: GoOS/GUI/Giff/Giff.cs
// British English implementation for GoOS. Cosmos-safe (no threads, no reflection, no P/Invoke).

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Gold.Graphics;
using GoOS.GUI;
using static GoOS.Resources;

namespace GoOS.Giff;

public static class Giff
{
    public const string Version = "0.35";
    private static readonly string[] Authors = { "Owen2k6", "GoOS Team" };

    public static List<Window> Run(string script)
    {
        var built = new List<Window>(4);
        try
        {
            var tokens = new Lexer(script).Tokenise();
            var program = new Parser(tokens).ParseProgram();

            for (var i = 0; i < program.Windows.Count; i++)
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
internal enum TokKind
{
    Eof,
    Ident,
    Number,
    String,
    LBrace,
    RBrace,
    Equals,
    Colon,
    Semi,
    Comma,
    HashWord,
    Plus,
    Minus,
    Multiply,
    Divide,
    Modulo,
    LParen,
    RParen,
    Dollar
}

internal readonly struct Tok
{
    public readonly TokKind Kind;
    public readonly string Text;
    public readonly int Pos;
    public readonly int Line;
    public readonly int Col;

    public Tok(TokKind k, string t, int p, int line, int col)
    {
        Kind = k;
        Text = t;
        Pos = p;
        Line = line;
        Col = col;
    }
}

internal sealed class Lexer
{
    private readonly string _s;
    private int _col = 1;
    private int _i;
    private int _line = 1;

    public Lexer(string s)
    {
        _s = s ?? string.Empty;
        _i = 0;
    }

    public List<Tok> Tokenise()
    {
        var t = new List<Tok>(256);
        var safety = Math.Max(256, _s.Length * 4);

        while (safety-- > 0)
        {
            SkipWsAndComments();
            if (_i >= _s.Length)
            {
                t.Add(Make(TokKind.Eof, ""));
                break;
            }

            var before = _i;
            var c = _s[_i];

            if (c == '{')
            {
                t.Add(MakeAdv(TokKind.LBrace, "{", 1));
            }
            else if (c == '}')
            {
                t.Add(MakeAdv(TokKind.RBrace, "}", 1));
            }
            else if (c == '=')
            {
                t.Add(MakeAdv(TokKind.Equals, "=", 1));
            }
            else if (c == ':')
            {
                t.Add(MakeAdv(TokKind.Colon, ":", 1));
            }
            else if (c == ';')
            {
                t.Add(MakeAdv(TokKind.Semi, ";", 1));
            }
            else if (c == ',')
            {
                t.Add(MakeAdv(TokKind.Comma, ",", 1));
            }
            else if (c == '(')
            {
                t.Add(MakeAdv(TokKind.LParen, "(", 1));
            }
            else if (c == ')')
            {
                t.Add(MakeAdv(TokKind.RParen, ")", 1));
            }
            else if (c == '+')
            {
                t.Add(MakeAdv(TokKind.Plus, "+", 1));
            }
            else if (c == '-')
            {
                if (_i + 1 < _s.Length && char.IsDigit(_s[_i + 1]))
                {
                    var s0 = _i;
                    var sl = _line;
                    var sc = _col;
                    _i++;
                    _col++;
                    while (_i < _s.Length && char.IsDigit(_s[_i]))
                    {
                        _i++;
                        _col++;
                    }

                    t.Add(new Tok(TokKind.Number, _s.Substring(s0, _i - s0), s0, sl, sc));
                }
                else
                {
                    t.Add(MakeAdv(TokKind.Minus, "-", 1));
                }
            }
            else if (c == '*')
            {
                t.Add(MakeAdv(TokKind.Multiply, "*", 1));
            }
            else if (c == '%')
            {
                t.Add(MakeAdv(TokKind.Modulo, "%", 1));
            }
            else if (c == '$')
            {
                t.Add(MakeAdv(TokKind.Dollar, "$", 1));
            }
            else if (c == '/')
            {
                if (_i + 1 < _s.Length && _s[_i + 1] == '/')
                {
                    // line comment
                    _i += 2;
                    _col += 2;
                    while (_i < _s.Length && _s[_i] != '\n')
                    {
                        _i++;
                        _col++;
                    }
                }
                else
                {
                    t.Add(MakeAdv(TokKind.Divide, "/", 1));
                }
            }
            else if (c == '"' || c == '\'')
            {
                t.Add(ReadString());
            }
            else if (c == '#')
            {
                t.Add(ReadHashWord());
            }
            else if (char.IsDigit(c))
            {
                var s0 = _i;
                var sl = _line;
                var sc = _col;
                while (_i < _s.Length && char.IsDigit(_s[_i]))
                {
                    _i++;
                    _col++;
                }

                t.Add(new Tok(TokKind.Number, _s.Substring(s0, _i - s0), s0, sl, sc));
            }
            else if (char.IsLetter(c) || c == '_' || c == '.')
            {
                var s0 = _i;
                var sl = _line;
                var sc = _col;
                while (_i < _s.Length)
                {
                    var ch = _s[_i];
                    if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
                    {
                        _i++;
                        _col++;
                    }
                    else
                    {
                        break;
                    }
                }

                t.Add(new Tok(TokKind.Ident, _s.Substring(s0, _i - s0), s0, sl, sc));
            }
            else
            {
                Advance(1);
            }

            if (_i <= before) Advance(1); // guarantee progress
        }

        if (safety <= 0) t.Add(Make(TokKind.Eof, ""));
        return t;
    }

    private void SkipWsAndComments()
    {
        while (_i < _s.Length)
        {
            var c = _s[_i];
            if (c == ' ' || c == '\t' || c == '\r')
            {
                _i++;
                _col++;
                continue;
            }

            if (c == '\n')
            {
                _i++;
                _line++;
                _col = 1;
                continue;
            }

            if (c == '/' && _i + 1 < _s.Length && _s[_i + 1] == '/')
            {
                _i += 2;
                _col += 2;
                while (_i < _s.Length && _s[_i] != '\n')
                {
                    _i++;
                    _col++;
                }

                continue;
            }

            break;
        }
    }

    private Tok ReadString()
    {
        var q = _s[_i++];
        var sl = _line;
        var sc = _col;
        _col++;
        var b = new StringBuilder();
        var start = _i;
        while (_i < _s.Length)
        {
            var c = _s[_i++];
            if (c == '\n')
            {
                _line++;
                _col = 1;
            }
            else
            {
                _col++;
            }

            if (c == q) break;
            if (c == '\\' && _i < _s.Length)
            {
                var e = _s[_i++];
                _col++;
                b.Append(e switch
                {
                    'n' => '\n', 'r' => '\r', 't' => '\t', '\\' => '\\', '"' => '"', '\'' => '\'', _ => e
                });
            }
            else
            {
                b.Append(c);
            }
        }

        return new Tok(TokKind.String, b.ToString(), start - 1, sl, sc);
    }

    private Tok ReadHashWord()
    {
        var s0 = _i++;
        var sl = _line;
        var sc = _col;
        _col++;
        while (_i < _s.Length && char.IsLetterOrDigit(_s[_i]))
        {
            _i++;
            _col++;
        }

        return new Tok(TokKind.HashWord, _s.Substring(s0, _i - s0), s0, sl, sc);
    }

    private Tok Make(TokKind k, string text)
    {
        return new Tok(k, text, _i, _line, _col);
    }

    private Tok MakeAdv(TokKind k, string text, int n)
    {
        var tok = new Tok(k, text, _i, _line, _col);
        Advance(n);
        return tok;
    }

    private void Advance(int n)
    {
        _i += n;
        _col += n;
    }
}

// ---------- AST ----------
internal sealed class ProgramNode
{
    public readonly List<WindowNode> Windows = new(4);
}

internal sealed class WindowNode
{
    public readonly List<ButtonNode> Buttons = new(8);
    public readonly List<ImageNode> Images = new(8);
    public readonly List<InputNode> Inputs = new(8);

    public readonly List<LabelNode> Labels = new(8);
    public readonly List<ListBoxNode> ListBoxes = new(4);
    public readonly List<CommandNode> OnClose = new(4);

    public readonly List<CommandNode> OnLoad = new(4);
    public readonly List<PanelNode> Panels = new(4);

    public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);

    public string Name = "Window";
}

internal sealed class LabelNode
{
    public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);

    public string Name = "label";
}

internal sealed class InputNode
{
    public readonly List<CommandNode> OnChange = new(4);

    public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);

    public string Name = "input";
}

internal sealed class ButtonNode
{
    public readonly List<CommandNode> OnClick = new(4);

    public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);

    public string Name = "button";
}

internal sealed class ImageNode
{
    public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);

    public string Name = "image";
}

internal sealed class ListBoxNode
{
    public readonly List<string> Items = new();
    public readonly List<CommandNode> OnSelect = new(4);

    public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);

    public string Name = "listbox";
}

internal sealed class PanelNode
{
    public readonly List<ButtonNode> Buttons = new(8);
    public readonly List<ImageNode> Images = new(4);
    public readonly List<InputNode> Inputs = new(8);

    public readonly List<LabelNode> Labels = new(8);
    public readonly List<ListBoxNode> ListBoxes = new(4);

    public readonly Dictionary<string, string> Props = new(StringComparer.OrdinalIgnoreCase);

    public string Name = "panel";
}

internal abstract class CommandNode
{
}

internal sealed class MessageCmd : CommandNode
{
    public string Text;

    public MessageCmd(string t)
    {
        Text = t;
    }
}

internal sealed class CloseCmd : CommandNode
{
}

internal sealed class SetVarCmd : CommandNode
{
    public string Name;
    public string Value;

    public SetVarCmd(string n, string v)
    {
        Name = n;
        Value = v;
    }
}

// File/Dir I/O
internal sealed class FileWriteCmd : CommandNode
{
    public string Content;
    public string Path;

    public FileWriteCmd(string p, string c)
    {
        Path = p;
        Content = c;
    }
}

internal sealed class FileReadCmd : CommandNode
{
    public string Path;
    public string Variable;

    public FileReadCmd(string p, string v)
    {
        Path = p;
        Variable = v;
    }
}

internal sealed class FileDeleteCmd : CommandNode
{
    public string Path;

    public FileDeleteCmd(string p)
    {
        Path = p;
    }
}

internal sealed class FileCopyCmd : CommandNode
{
    public string Destination;
    public string Source;

    public FileCopyCmd(string s, string d)
    {
        Source = s;
        Destination = d;
    }
}

internal sealed class FileAppendCmd : CommandNode
{
    public string Content;
    public string Path;

    public FileAppendCmd(string p, string c)
    {
        Path = p;
        Content = c;
    }
}

internal sealed class FileExistsCmd : CommandNode
{
    public string OutVar;
    public string Path;

    public FileExistsCmd(string p, string v)
    {
        Path = p;
        OutVar = v;
    }
}

internal sealed class DirCreateCmd : CommandNode
{
    public string Path;

    public DirCreateCmd(string p)
    {
        Path = p;
    }
}

internal sealed class DirDeleteCmd : CommandNode
{
    public string Path;

    public DirDeleteCmd(string p)
    {
        Path = p;
    }
}

internal sealed class DirExistsCmd : CommandNode
{
    public string OutVar;
    public string Path;

    public DirExistsCmd(string p, string v)
    {
        Path = p;
        OutVar = v;
    }
}

// Variables persistence
internal sealed class VarsSaveCmd : CommandNode
{
    public string Path;

    public VarsSaveCmd(string p)
    {
        Path = p;
    }
}

internal sealed class VarsLoadCmd : CommandNode
{
    public string Path;

    public VarsLoadCmd(string p)
    {
        Path = p;
    }
}

internal sealed class VarClearCmd : CommandNode
{
    public string Name;

    public VarClearCmd(string n)
    {
        Name = n;
    }
}

internal sealed class VarsClearAllCmd : CommandNode
{
}

// String/logic helpers
internal sealed class IfCmd : CommandNode
{
    public string Condition;
    public List<CommandNode> FalseBlock = new();
    public List<CommandNode> TrueBlock = new();

    public IfCmd(string c)
    {
        Condition = c;
    }
}

internal sealed class ConcatCmd : CommandNode
{
    public string Dest;
    public string Left;
    public string Right;

    public ConcatCmd(string l, string r, string d)
    {
        Left = l;
        Right = r;
        Dest = d;
    }
}

internal sealed class LengthCmd : CommandNode
{
    public string Dest;
    public string Source;

    public LengthCmd(string s, string d)
    {
        Source = s;
        Dest = d;
    }
}

internal sealed class SubstrCmd : CommandNode
{
    public string Dest;
    public int Len;
    public string Source;
    public int Start;

    public SubstrCmd(string s, int st, int ln, string d)
    {
        Source = s;
        Start = st;
        Len = ln;
        Dest = d;
    }
}

// Networking
internal sealed class NetGetFileCmd : CommandNode
{
    public string Dest;
    public string Url;

    public NetGetFileCmd(string u, string d)
    {
        Url = u;
        Dest = d;
    }
}

internal sealed class NetGetTextCmd : CommandNode
{
    public string DestVar;
    public string Url;

    public NetGetTextCmd(string u, string v)
    {
        Url = u;
        DestVar = v;
    }
}

// ---------- Parser ----------
internal sealed class Parser
{
    private readonly List<Tok> _t;
    private int _i;

    public Parser(List<Tok> t)
    {
        _t = t;
        _i = 0;
    }

    public ProgramNode ParseProgram()
    {
        var p = new ProgramNode();
        var safety = Math.Max(64, _t.Count * 4);

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
        Require(TokKind.LBrace, "expected '{' to start window block");

        var safety = Math.Max(64, _t.Count * 2);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;

            if (TryMatchIdent("label"))
            {
                w.Labels.Add(ParseLabel());
                continue;
            }

            if (TryMatchIdent("input"))
            {
                w.Inputs.Add(ParseInput());
                continue;
            }

            if (TryMatchIdent("button"))
            {
                w.Buttons.Add(ParseButton());
                continue;
            }

            if (TryMatchIdent("panel"))
            {
                w.Panels.Add(ParsePanel());
                continue;
            }

            if (TryMatchIdent("image"))
            {
                w.Images.Add(ParseImage());
                continue;
            }

            if (TryMatchIdent("listbox"))
            {
                w.ListBoxes.Add(ParseListBox());
                continue;
            }

            if (TryMatchIdent("onLoad"))
            {
                ParseBlockInto(w.OnLoad);
                continue;
            }

            if (TryMatchIdent("onClose"))
            {
                ParseBlockInto(w.OnClose);
                continue;
            }

            if (Peek().Kind == TokKind.Ident)
            {
                ParsePropertyInto(w.Props);
                continue;
            }

            _i++; // progress guard
        }

        return w;
    }

    private PanelNode ParsePanel()
    {
        var p = new PanelNode();
        if (Peek().Kind == TokKind.Ident) p.Name = Next().Text;

        Require(TokKind.LBrace, "expected '{' to start panel block");
        var safety = Math.Max(32, _t.Count);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;

            if (TryMatchIdent("label"))
            {
                p.Labels.Add(ParseLabel());
                continue;
            }

            if (TryMatchIdent("input"))
            {
                p.Inputs.Add(ParseInput());
                continue;
            }

            if (TryMatchIdent("button"))
            {
                p.Buttons.Add(ParseButton());
                continue;
            }

            if (TryMatchIdent("image"))
            {
                p.Images.Add(ParseImage());
                continue;
            }

            if (TryMatchIdent("listbox"))
            {
                p.ListBoxes.Add(ParseListBox());
                continue;
            }

            if (Peek().Kind == TokKind.Ident)
            {
                ParsePropertyInto(p.Props);
                continue;
            }

            _i++;
        }

        return p;
    }

    private LabelNode ParseLabel()
    {
        var n = new LabelNode();
        if (Peek().Kind == TokKind.Ident) n.Name = Next().Text;
        Require(TokKind.LBrace, "expected '{' to start label block");

        var safety = Math.Max(16, _t.Count);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;

            if (Peek().Kind == TokKind.Ident)
            {
                ParsePropertyInto(n.Props);
                continue;
            }

            _i++;
        }

        return n;
    }

    private InputNode ParseInput()
    {
        var n = new InputNode();
        if (Peek().Kind == TokKind.Ident) n.Name = Next().Text;
        Require(TokKind.LBrace, "expected '{' to start input block");

        var safety = Math.Max(16, _t.Count);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;

            if (TryMatchIdent("onChange"))
            {
                ParseBlockInto(n.OnChange);
                continue;
            }

            if (Peek().Kind == TokKind.Ident)
            {
                ParsePropertyInto(n.Props);
                continue;
            }

            _i++;
        }

        return n;
    }

    private ButtonNode ParseButton()
    {
        var b = new ButtonNode();
        if (Peek().Kind == TokKind.Ident) b.Name = Next().Text;
        Require(TokKind.LBrace, "expected '{' to start button block");

        var safety = Math.Max(16, _t.Count);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;

            if (TryMatchIdent("onClick"))
            {
                ParseBlockInto(b.OnClick);
                continue;
            }

            if (Peek().Kind == TokKind.Ident)
            {
                ParsePropertyInto(b.Props);
                continue;
            }

            _i++;
        }

        return b;
    }

    private ImageNode ParseImage()
    {
        var n = new ImageNode();
        if (Peek().Kind == TokKind.Ident) n.Name = Next().Text;
        Require(TokKind.LBrace, "expected '{' to start image block");

        var safety = Math.Max(16, _t.Count);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;
            if (Peek().Kind == TokKind.Ident)
            {
                ParsePropertyInto(n.Props);
                continue;
            }

            _i++;
        }

        return n;
    }

    private ListBoxNode ParseListBox()
    {
        var n = new ListBoxNode();
        if (Peek().Kind == TokKind.Ident) n.Name = Next().Text;
        Require(TokKind.LBrace, "expected '{' to start listbox block");

        var safety = Math.Max(16, _t.Count);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;

            if (TryMatchIdent("item"))
            {
                var itemTok = Require(TokKind.String, "expected string after item");
                n.Items.Add(itemTok.Text);
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("onSelect"))
            {
                ParseBlockInto(n.OnSelect);
                continue;
            }

            if (Peek().Kind == TokKind.Ident)
            {
                ParsePropertyInto(n.Props);
                continue;
            }

            _i++;
        }

        return n;
    }

    private void ParseBlockInto(List<CommandNode> list)
    {
        Require(TokKind.LBrace, "expected '{' to start block");
        var safety = Math.Max(32, _t.Count);
        while (!AtEnd() && safety-- > 0)
        {
            SkipSeps();
            if (Match(TokKind.RBrace)) break;

            // Basics
            if (TryMatchIdent("message"))
            {
                var s = Require(TokKind.String, "expected string after message");
                list.Add(new MessageCmd(s.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("close"))
            {
                list.Add(new CloseCmd());
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            // Variables
            if (TryMatchIdent("set"))
            {
                var varName = Require(TokKind.Ident, "expected variable name");
                Require(TokKind.Equals, "expected '=' after variable name");
                var value = Require(TokKind.String, "expected string value (supports $var substitution)");
                list.Add(new SetVarCmd(varName.Text, value.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("concat"))
            {
                var l = Require(TokKind.String, "expected left string");
                Require(TokKind.Comma, "expected ','");
                var r = Require(TokKind.String, "expected right string");
                Require(TokKind.Comma, "expected ','");
                var d = Require(TokKind.Ident, "expected destination variable");
                list.Add(new ConcatCmd(l.Text, r.Text, d.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("length"))
            {
                var s = Require(TokKind.String, "expected string or \"$var\"");
                Require(TokKind.Comma, "expected ','");
                var d = Require(TokKind.Ident, "expected destination variable");
                list.Add(new LengthCmd(s.Text, d.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("substr"))
            {
                var s = Require(TokKind.String, "expected string");
                Require(TokKind.Comma, "expected ','");
                var st = Require(TokKind.Number, "expected start");
                Require(TokKind.Comma, "expected ','");
                var ln = Require(TokKind.Number, "expected length");
                Require(TokKind.Comma, "expected ','");
                var d = Require(TokKind.Ident, "expected destination variable");
                list.Add(new SubstrCmd(s.Text, ParseIntToken(st), ParseIntToken(ln), d.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("varsSave"))
            {
                var p = Require(TokKind.String, "expected path string");
                list.Add(new VarsSaveCmd(p.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("varsLoad"))
            {
                var p = Require(TokKind.String, "expected path string");
                list.Add(new VarsLoadCmd(p.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("varClear"))
            {
                var n = Require(TokKind.Ident, "expected variable name");
                list.Add(new VarClearCmd(n.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("varsClearAll"))
            {
                list.Add(new VarsClearAllCmd());
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            // Conditionals
            if (TryMatchIdent("if"))
            {
                var cond = Require(TokKind.String, "expected condition");
                var ifCmd = new IfCmd(cond.Text);

                // true block
                Require(TokKind.LBrace, "expected '{' after condition");
                var trueBlockSafety = Math.Max(16, _t.Count);
                while (!AtEnd() && trueBlockSafety-- > 0)
                {
                    SkipSeps();
                    if (Match(TokKind.RBrace)) break;
                    // allow nested simple commands
                    if (TryMatchIdent("message"))
                    {
                        var s = Require(TokKind.String, "expected string");
                        ifCmd.TrueBlock.Add(new MessageCmd(s.Text));
                        if (Peek().Kind == TokKind.Semi) _i++;
                        continue;
                    }

                    if (TryMatchIdent("close"))
                    {
                        ifCmd.TrueBlock.Add(new CloseCmd());
                        if (Peek().Kind == TokKind.Semi) _i++;
                        continue;
                    }

                    if (TryMatchIdent("set"))
                    {
                        var vn = Require(TokKind.Ident, "expected name");
                        Require(TokKind.Equals, "expected '='");
                        var vv = Require(TokKind.String, "expected value");
                        ifCmd.TrueBlock.Add(new SetVarCmd(vn.Text, vv.Text));
                        if (Peek().Kind == TokKind.Semi) _i++;
                        continue;
                    }

                    _i++;
                }

                SkipSeps();
                if (TryMatchIdent("else"))
                {
                    Require(TokKind.LBrace, "expected '{' after else");
                    var falseBlockSafety = Math.Max(16, _t.Count);
                    while (!AtEnd() && falseBlockSafety-- > 0)
                    {
                        SkipSeps();
                        if (Match(TokKind.RBrace)) break;
                        if (TryMatchIdent("message"))
                        {
                            var s = Require(TokKind.String, "expected string");
                            ifCmd.FalseBlock.Add(new MessageCmd(s.Text));
                            if (Peek().Kind == TokKind.Semi) _i++;
                            continue;
                        }

                        if (TryMatchIdent("close"))
                        {
                            ifCmd.FalseBlock.Add(new CloseCmd());
                            if (Peek().Kind == TokKind.Semi) _i++;
                            continue;
                        }

                        if (TryMatchIdent("set"))
                        {
                            var vn = Require(TokKind.Ident, "expected name");
                            Require(TokKind.Equals, "expected '='");
                            var vv = Require(TokKind.String, "expected value");
                            ifCmd.FalseBlock.Add(new SetVarCmd(vn.Text, vv.Text));
                            if (Peek().Kind == TokKind.Semi) _i++;
                            continue;
                        }

                        _i++;
                    }
                }

                list.Add(ifCmd);
                continue;
            }

            // File ops
            if (TryMatchIdent("fileWrite"))
            {
                var path = Require(TokKind.String, "expected path");
                Require(TokKind.Comma, "expected ','");
                var body = Require(TokKind.String, "expected content");
                list.Add(new FileWriteCmd(path.Text, body.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("fileAppend"))
            {
                var path = Require(TokKind.String, "expected path");
                Require(TokKind.Comma, "expected ','");
                var body = Require(TokKind.String, "expected content");
                list.Add(new FileAppendCmd(path.Text, body.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("fileRead"))
            {
                var path = Require(TokKind.String, "expected path");
                Require(TokKind.Comma, "expected ','");
                var varN = Require(TokKind.Ident, "expected variable");
                list.Add(new FileReadCmd(path.Text, varN.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("fileDelete"))
            {
                var path = Require(TokKind.String, "expected path");
                list.Add(new FileDeleteCmd(path.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("fileCopy"))
            {
                var src = Require(TokKind.String, "expected source");
                Require(TokKind.Comma, "expected ','");
                var dst = Require(TokKind.String, "expected destination");
                list.Add(new FileCopyCmd(src.Text, dst.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("fileExists"))
            {
                var path = Require(TokKind.String, "expected path");
                Require(TokKind.Comma, "expected ','");
                var varN = Require(TokKind.Ident, "expected variable");
                list.Add(new FileExistsCmd(path.Text, varN.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            // Directory ops
            if (TryMatchIdent("dirCreate"))
            {
                var path = Require(TokKind.String, "expected directory");
                list.Add(new DirCreateCmd(path.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("dirDelete"))
            {
                var path = Require(TokKind.String, "expected directory");
                list.Add(new DirDeleteCmd(path.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("dirExists"))
            {
                var path = Require(TokKind.String, "expected directory");
                Require(TokKind.Comma, "expected ','");
                var varN = Require(TokKind.Ident, "expected variable");
                list.Add(new DirExistsCmd(path.Text, varN.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            // Networking
            if (TryMatchIdent("netGet"))
            {
                var url = Require(TokKind.String, "expected url");
                Require(TokKind.Comma, "expected ','");
                var dst = Require(TokKind.String, "expected file path");
                list.Add(new NetGetFileCmd(url.Text, dst.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            if (TryMatchIdent("netGetText"))
            {
                var url = Require(TokKind.String, "expected url");
                Require(TokKind.Comma, "expected ','");
                var varN = Require(TokKind.Ident, "expected variable");
                list.Add(new NetGetTextCmd(url.Text, varN.Text));
                if (Peek().Kind == TokKind.Semi) _i++;
                continue;
            }

            _i++; // skip unknown but guarantee progress
        }
    }

    // Accepts: key = value  |  key : value  |  key value
    private void ParsePropertyInto(Dictionary<string, string> dict)
    {
        var keyTok = Require(TokKind.Ident, "expected property name");
        var key = keyTok.Text.ToLowerInvariant();

        if (Peek().Kind == TokKind.Equals || Peek().Kind == TokKind.Colon) _i++;

        var valTok = Next();
        if (!(valTok.Kind == TokKind.String || valTok.Kind == TokKind.Number || valTok.Kind == TokKind.Ident ||
              valTok.Kind == TokKind.HashWord))
            throw Error("expected a value for '" + key + "'", valTok);

        dict[key] = valTok.Text;
        if (Peek().Kind == TokKind.Semi) _i++;
    }

    private static int ParseIntToken(Tok t)
    {
        int v;
        if (!int.TryParse(t.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
            v = 0;
        return v;
    }

    private void SkipSeps()
    {
        while (Peek().Kind == TokKind.Semi || Peek().Kind == TokKind.Comma) _i++;
    }

    private Tok Peek()
    {
        return _i < _t.Count ? _t[_i] : _t[_t.Count - 1];
    }

    private Tok Next()
    {
        var x = Peek();
        if (_i < _t.Count) _i++;
        return x;
    }

    private Tok Require(TokKind k, string msg)
    {
        var t = Next();
        if (t.Kind != k) throw Error(msg, t);
        return t;
    }

    private bool Match(TokKind k)
    {
        if (Peek().Kind == k)
        {
            _i++;
            return true;
        }

        return false;
    }

    private bool TryMatchIdent(string s)
    {
        var t = Peek();
        if (t.Kind == TokKind.Ident && string.Equals(t.Text, s, StringComparison.OrdinalIgnoreCase))
        {
            _i++;
            return true;
        }

        return false;
    }

    private bool AtEnd()
    {
        return Peek().Kind == TokKind.Eof;
    }

    private Exception Error(string message, Tok near)
    {
        return new Exception($"Giff parse error at line {near.Line}, col {near.Col}: {message}");
    }
}

// ---------- Runtime ----------
internal static class Runtime
{
    // Variables are per-process (per script run) and case-insensitive.
    private static readonly Dictionary<string, string> _variables = new(StringComparer.OrdinalIgnoreCase);

    // --- Window build ---
    public static Window BuildWindow(WindowNode n)
    {
        try
        {
            var width = ParseInt(n.Props, "width", 320);
            var height = ParseInt(n.Props, "height", 240);

            var win = new Window
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
            if (bg.HasValue) win.Contents.Clear(bg.Value);

            // Buttons
            for (var i = 0; i < n.Buttons.Count; i++) BuildButton(win, n.Buttons[i]);

            // Labels
            for (var i = 0; i < n.Labels.Count; i++) BuildLabel(win, n.Labels[i]);

            // Inputs
            for (var i = 0; i < n.Inputs.Count; i++) BuildInput(win, n.Inputs[i]);

            // Images
            for (var i = 0; i < n.Images.Count; i++) BuildImage(win, n.Images[i]);

            // ListBoxes (drawn)
            for (var i = 0; i < n.ListBoxes.Count; i++) BuildListBox(win, n.ListBoxes[i]);

            // Panels (child co-ords relative to panel)
            for (var i = 0; i < n.Panels.Count; i++) BuildPanel(win, n.Panels[i]);

            // Run onLoad now that the window is created and added by caller soon after. Keep very light.
            if (n.OnLoad.Count > 0) Execute(n.OnLoad, win);

            return win;
        }
        catch (Exception ex)
        {
            Dialogue.Show("Giff", "Script error: " + ex.Message);
            var w = new Window { Title = "Giff Error", Visible = true };
            w.Contents = new Canvas(200, 80);
            w.Contents.Clear(new Color(64, 0, 0));
            return w;
        }
    }

    // --- Builders (client-relative) ---
    private static void BuildButton(Window win, ButtonNode n)
    {
        var x = ParseInt(n.Props, "x", 0);
        var y = ParseInt(n.Props, "y", 0);
        var w = ParseInt(n.Props, "width", 80);
        var h = ParseInt(n.Props, "height", 24);
        var text = ParseString(n.Props, "text", n.Name);

        var btn = new Button(win, (ushort)x, (ushort)y, (ushort)w, (ushort)h, text)
        {
            UseSystemStyle = ParseBool(n.Props, "useSystemStyle", true),
            BackgroundColour = ParseColour(ParseString(n.Props, "backgroundColour", "")) ?? Color.Transparent,
            TextColour = ParseColour(ParseString(n.Props, "textColour", "")) ?? Color.White
        };

        if (n.OnClick.Count > 0)
        {
            var cmds = n.OnClick;
            btn.Clicked = () => Execute(cmds, win);
        }

        _variables["button_" + n.Name + "_text"] = text;

        // Honour controls that only lock position after first render
        btn.Render();
        btn.X = (ushort)x;
        btn.Y = (ushort)y;
        btn.Render();
    }

    private static void BuildLabel(Window win, LabelNode n)
    {
        var x = ParseInt(n.Props, "x", 0);
        var y = ParseInt(n.Props, "y", 0);
        var text = ParseString(n.Props, "text", n.Name);
        var col = ParseColour(ParseString(n.Props, "textColour", "")) ?? Color.Black;

        var centre = ParseCentreFlag(n.Props);
        if (centre)
        {
            int textWidth = Charcoal.MeasureString(text);
            x = (win.Contents.Width - textWidth) / 2;
        }

        // Clamp to client
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x > win.Contents.Width - 1) x = win.Contents.Width - 1;
        if (y > win.Contents.Height - 1) y = win.Contents.Height - 1;

        win.Contents.DrawString(x, y, text, Charcoal, col);
        _variables["label_" + n.Name + "_text"] = text;
    }

    private static void BuildInput(Window win, InputNode n)
    {
        var x = ParseInt(n.Props, "x", 0);
        var y = ParseInt(n.Props, "y", 0);
        var w = ParseInt(n.Props, "width", 120);
        var h = ParseInt(n.Props, "height", 20);
        var placeholder = ParseString(n.Props, "placeholder", "");
        var text = ParseString(n.Props, "text", "");

        var input = new Input(win, (ushort)x, (ushort)y, (ushort)w, (ushort)h, placeholder) { Text = text };

        if (n.OnChange.Count > 0)
        {
            var cmds = n.OnChange;
            input.Changed = () =>
            {
                _variables["input_" + n.Name + "_text"] = input.Text ?? string.Empty;
                Execute(cmds, win);
            };
        }

        _variables["input_" + n.Name + "_text"] = text ?? string.Empty;

        input.Render(); // honour initialisation quirk
        input.X = (ushort)x;
        input.Y = (ushort)y;
        input.Render();
    }

    private static void BuildImage(Window win, ImageNode n)
    {
        var x = ParseInt(n.Props, "x", 0);
        var y = ParseInt(n.Props, "y", 0);
        var w = ParseInt(n.Props, "width", 64);
        var h = ParseInt(n.Props, "height", 64);

        var grey = new Color(200, 200, 200);
        var dark = new Color(120, 120, 120);
        win.Contents.DrawFilledRectangle(x, y, (ushort)w, (ushort)h, 0, grey);
        win.Contents.DrawLine(x, y, x + w - 1, y, dark);
        win.Contents.DrawLine(x, y, x, y + h - 1, dark);
        win.Contents.DrawLine(x + w - 1, y, x + w - 1, y + h - 1, dark);
        win.Contents.DrawLine(x, y + h - 1, x + w - 1, y + h - 1, dark);
        win.Contents.DrawLine(x, y, x + w - 1, y + h - 1, dark);
        win.Contents.DrawLine(x + w - 1, y, x, y + h - 1, dark);
    }

    private static void BuildListBox(Window win, ListBoxNode n)
    {
        var x = ParseInt(n.Props, "x", 0);
        var y = ParseInt(n.Props, "y", 0);
        var w = ParseInt(n.Props, "width", 150);
        var h = ParseInt(n.Props, "height", 100);

        win.Contents.DrawFilledRectangle(x, y, (ushort)w, (ushort)h, 0, Color.White);
        win.Contents.DrawRectangle(x, y, (ushort)w, (ushort)h, 0, Color.DeepGray);

        var itemY = y + 2;
        for (var i = 0; i < n.Items.Count; i++)
        {
            if (itemY >= y + h - 12) break;
            win.Contents.DrawString(x + 4, itemY, n.Items[i], Charcoal, Color.Black);
            itemY += 16;
        }

        for (var i = 0; i < n.Items.Count; i++) _variables["listbox_" + n.Name + "_item_" + i] = n.Items[i];
        _variables["listbox_" + n.Name + "_count"] = n.Items.Count.ToString();
    }

    private static void BuildPanel(Window win, PanelNode p)
    {
        var px = ParseInt(p.Props, "x", 0);
        var py = ParseInt(p.Props, "y", 0);
        var pW = ParseInt(p.Props, "width", 200);
        var pH = ParseInt(p.Props, "height", 120);

        var pbg = ParseColour(ParseString(p.Props, "background", ""));
        if (pbg.HasValue) win.Contents.DrawFilledRectangle(px, py, (ushort)pW, (ushort)pH, 0, pbg.Value);

        for (var i = 0; i < p.Labels.Count; i++)
        {
            var label = p.Labels[i];
            var x = ParseInt(label.Props, "x", 0) + px;
            var y = ParseInt(label.Props, "y", 0) + py;
            var text = ParseString(label.Props, "text", label.Name);
            var col = ParseColour(ParseString(label.Props, "textColour", "")) ?? Color.Black;
            win.Contents.DrawString(x, y, text, Charcoal, col);
            _variables["panel_" + p.Name + "_label_" + label.Name + "_text"] = text;
        }

        for (var i = 0; i < p.Buttons.Count; i++)
        {
            var btn = p.Buttons[i];
            var x = ParseInt(btn.Props, "x", 0) + px;
            var y = ParseInt(btn.Props, "y", 0) + py;
            var w = ParseInt(btn.Props, "width", 80);
            var h = ParseInt(btn.Props, "height", 24);
            var text = ParseString(btn.Props, "text", btn.Name);

            var b = new Button(win, (ushort)x, (ushort)y, (ushort)w, (ushort)h, text)
            {
                UseSystemStyle = ParseBool(btn.Props, "useSystemStyle", true),
                BackgroundColour = ParseColour(ParseString(btn.Props, "backgroundColour", "")) ?? Color.Transparent,
                TextColour = ParseColour(ParseString(btn.Props, "textColour", "")) ?? Color.White
            };
            if (btn.OnClick.Count > 0)
            {
                var cmds = btn.OnClick;
                b.Clicked = () => Execute(cmds, win);
            }

            _variables["panel_" + p.Name + "_button_" + btn.Name + "_text"] = text;

            b.Render();
            b.X = (ushort)x;
            b.Y = (ushort)y;
            b.Render();
        }

        for (var i = 0; i < p.Inputs.Count; i++)
        {
            var input = p.Inputs[i];
            var x = ParseInt(input.Props, "x", 0) + px;
            var y = ParseInt(input.Props, "y", 0) + py;
            var w = ParseInt(input.Props, "width", 120);
            var h = ParseInt(input.Props, "height", 20);
            var placeholder = ParseString(input.Props, "placeholder", "");
            var text = ParseString(input.Props, "text", "");

            var inp = new Input(win, (ushort)x, (ushort)y, (ushort)w, (ushort)h, placeholder) { Text = text };
            if (input.OnChange.Count > 0)
            {
                var cmds = input.OnChange;
                inp.Changed = () =>
                {
                    _variables["panel_" + p.Name + "_input_" + input.Name + "_text"] = inp.Text ?? string.Empty;
                    Execute(cmds, win);
                };
            }

            _variables["panel_" + p.Name + "_input_" + input.Name + "_text"] = text ?? string.Empty;
            inp.Render();
            inp.X = (ushort)x;
            inp.Y = (ushort)y;
            inp.Render();
        }

        for (var i = 0; i < p.ListBoxes.Count; i++)
        {
            var n = p.ListBoxes[i];
            var x = ParseInt(n.Props, "x", 0) + px;
            var y = ParseInt(n.Props, "y", 0) + py;
            var w = ParseInt(n.Props, "width", 150);
            var h = ParseInt(n.Props, "height", 100);

            win.Contents.DrawFilledRectangle(x, y, (ushort)w, (ushort)h, 0, Color.White);
            win.Contents.DrawRectangle(x, y, (ushort)w, (ushort)h, 0, Color.DeepGray);

            var itemY = y + 2;
            for (var k = 0; k < n.Items.Count; k++)
            {
                if (itemY >= y + h - 12) break;
                win.Contents.DrawString(x + 4, itemY, n.Items[k], Charcoal, Color.Black);
                itemY += 16;
            }

            for (var k = 0; k < n.Items.Count; k++)
                _variables["panel_" + p.Name + "_listbox_" + n.Name + "_item_" + k] = n.Items[k];
            _variables["panel_" + p.Name + "_listbox_" + n.Name + "_count"] = n.Items.Count.ToString();
        }
    }

    // --- Command execution ---
    public static void Execute(List<CommandNode> cmds, Window ctx)
    {
        for (var i = 0; i < cmds.Count; i++)
        {
            var cmd = cmds[i];

            if (cmd is MessageCmd m)
            {
                Dialogue.Show(ctx.Title, EvaluateVariables(m.Text));
            }
            else if (cmd is CloseCmd)
            {
                ctx.Dispose();
                return;
            }
            else if (cmd is SetVarCmd sv)
            {
                _variables[sv.Name.ToLowerInvariant()] = EvaluateVariables(sv.Value) ?? string.Empty;
            }
            else if (cmd is ConcatCmd cc)
            {
                var left = EvaluateVariables(cc.Left) ?? string.Empty;
                var right = EvaluateVariables(cc.Right) ?? string.Empty;
                _variables[cc.Dest.ToLowerInvariant()] = left + right;
            }
            else if (cmd is LengthCmd lc)
            {
                var src = EvaluateVariables(lc.Source) ?? string.Empty;
                _variables[lc.Dest.ToLowerInvariant()] = src.Length.ToString();
            }
            else if (cmd is SubstrCmd sc)
            {
                var s = EvaluateVariables(sc.Source) ?? string.Empty;
                var st = sc.Start;
                if (st < 0) st = 0;
                if (st > s.Length) st = s.Length;
                var ln = sc.Len;
                if (ln < 0) ln = 0;
                if (st + ln > s.Length) ln = s.Length - st;
                _variables[sc.Dest.ToLowerInvariant()] = s.Substring(st, ln);
            }
            else if (cmd is IfCmd ifc)
            {
                var ok = EvaluateCondition(ifc.Condition);
                Execute(ok ? ifc.TrueBlock : ifc.FalseBlock, ctx);
            }
            // Files
            else if (cmd is FileWriteCmd fw)
            {
                TryIO(
                    () => { File.WriteAllText(EvaluateVariables(fw.Path) ?? "", EvaluateVariables(fw.Content) ?? ""); },
                    "Could not write to file");
            }
            else if (cmd is FileAppendCmd fa)
            {
                TryIO(
                    () =>
                    {
                        File.AppendAllText(EvaluateVariables(fa.Path) ?? "", EvaluateVariables(fa.Content) ?? "");
                    },
                    "Could not append to file");
            }
            else if (cmd is FileReadCmd fr)
            {
                TryIO(() =>
                {
                    var path = EvaluateVariables(fr.Path) ?? "";
                    var content = File.ReadAllText(path);
                    _variables[fr.Variable.ToLowerInvariant()] = content ?? "";
                }, "Could not read file");
            }
            else if (cmd is FileDeleteCmd fd)
            {
                TryIO(() =>
                {
                    var p = EvaluateVariables(fd.Path) ?? "";
                    if (File.Exists(p)) File.Delete(p);
                }, "Could not delete file");
            }
            else if (cmd is FileCopyCmd fc)
            {
                TryIO(() =>
                {
                    var s = EvaluateVariables(fc.Source) ?? "";
                    var d = EvaluateVariables(fc.Destination) ?? "";
                    File.Copy(s, d, true);
                }, "Could not copy file");
            }
            else if (cmd is FileExistsCmd fex)
            {
                var p = EvaluateVariables(fex.Path) ?? "";
                var exists = false;
                TryIO(() => { exists = File.Exists(p); }, null);
                _variables[fex.OutVar.ToLowerInvariant()] = exists ? "true" : "false";
            }
            // Dirs
            else if (cmd is DirCreateCmd dc)
            {
                TryIO(() => { Directory.CreateDirectory(EvaluateVariables(dc.Path) ?? ""); },
                    "Could not create directory");
            }
            else if (cmd is DirDeleteCmd dd)
            {
                TryIO(() =>
                {
                    var p = EvaluateVariables(dd.Path) ?? "";
                    if (Directory.Exists(p)) Directory.Delete(p, true);
                }, "Could not delete directory");
            }
            else if (cmd is DirExistsCmd dex)
            {
                var p = EvaluateVariables(dex.Path) ?? "";
                var exists = false;
                TryIO(() => { exists = Directory.Exists(p); }, null);
                _variables[dex.OutVar.ToLowerInvariant()] = exists ? "true" : "false";
            }
            // Vars persistence
            else if (cmd is VarsSaveCmd vs)
            {
                TryIO(() =>
                {
                    var path = EvaluateVariables(vs.Path) ?? "";
                    using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        foreach (var kv in _variables)
                        {
                            var v = Convert.ToBase64String(Encoding.UTF8.GetBytes(kv.Value ?? ""));
                            sw.WriteLine(kv.Key + "=" + v);
                        }
                    }
                }, "Could not save variables");
            }
            else if (cmd is VarsLoadCmd vl)
            {
                TryIO(() =>
                {
                    var path = EvaluateVariables(vl.Path) ?? "";
                    if (!File.Exists(path)) return;
                    var lines = File.ReadAllLines(path);
                    for (var li = 0; li < lines.Length; li++)
                    {
                        var line = lines[li];
                        var eq = line.IndexOf('=');
                        if (eq <= 0) continue;
                        var key = line.Substring(0, eq).Trim();
                        var val = line.Substring(eq + 1);
                        try
                        {
                            _variables[key] = Encoding.UTF8.GetString(Convert.FromBase64String(val));
                        }
                        catch
                        {
                            _variables[key] = "";
                        }
                    }
                }, "Could not load variables");
            }
            else if (cmd is VarClearCmd vc)
            {
                _variables.Remove(vc.Name);
            }
            else if (cmd is VarsClearAllCmd)
            {
                _variables.Clear();
            }
            // Networking
            else if (cmd is NetGetFileCmd ngf)
            {
                var url = EvaluateVariables(ngf.Url) ?? "";
                var dst = EvaluateVariables(ngf.Dest) ?? "";
                string status;
                byte[] body;
                if (TryHttpGet(url, out body, out status))
                {
                    TryIO(() => File.WriteAllBytes(dst, body), "Could not write downloaded file");
                    _variables["net_status"] = "ok";
                    _variables["net_size"] = body != null ? body.Length.ToString() : "0";
                }
                else
                {
                    _variables["net_status"] = status ?? "error";
                    _variables["net_size"] = "0";
                    Dialogue.Show("Network", "netGet failed: " + status);
                }
            }
            else if (cmd is NetGetTextCmd ngt)
            {
                var url = EvaluateVariables(ngt.Url) ?? "";
                string status;
                byte[] body;
                if (TryHttpGet(url, out body, out status))
                {
                    var text = SafeDecodeHttpBody(body);
                    _variables[ngt.DestVar.ToLowerInvariant()] = text ?? "";
                    _variables["net_status"] = "ok";
                    _variables["net_size"] = body != null ? body.Length.ToString() : "0";
                }
                else
                {
                    _variables[ngt.DestVar.ToLowerInvariant()] = "";
                    _variables["net_status"] = status ?? "error";
                    _variables["net_size"] = "0";
                    Dialogue.Show("Network", "netGetText failed: " + status);
                }
            }
        }
    }

    // --- Helpers ---
    private static void TryIO(Action act, string friendlyError)
    {
        try
        {
            act();
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(friendlyError))
                Dialogue.Show("Giff", friendlyError + ":\n" + ex.Message);
        }
    }

    private static string EvaluateVariables(string text)
    {
        if (text == null) return null;
        var pos = 0;
        var sb = new StringBuilder(text.Length);

        while (pos < text.Length)
        {
            var d = text.IndexOf('$', pos);
            if (d < 0)
            {
                sb.Append(text, pos, text.Length - pos);
                break;
            }

            if (d > pos) sb.Append(text, pos, d - pos);

            var i = d + 1;
            while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
            if (i > d + 1)
            {
                var name = text.Substring(d + 1, i - (d + 1)).ToLowerInvariant();
                string value;
                if (_variables.TryGetValue(name, out value)) sb.Append(value);
                else sb.Append(text, d, i - d); // keep literal if missing
            }
            else
            {
                sb.Append('$');
            }

            pos = i;
        }

        return sb.ToString();
    }

    private static bool EvaluateCondition(string cond)
    {
        var condition = EvaluateVariables(cond) ?? "";

        // equality
        var eq = condition.IndexOf("==", StringComparison.Ordinal);
        if (eq >= 0)
        {
            var l = condition.Substring(0, eq).Trim();
            var r = condition.Substring(eq + 2).Trim();
            return string.Equals(l, r, StringComparison.OrdinalIgnoreCase);
        }

        // inequality
        var ne = condition.IndexOf("!=", StringComparison.Ordinal);
        if (ne >= 0)
        {
            var l = condition.Substring(0, ne).Trim();
            var r = condition.Substring(ne + 2).Trim();
            return !string.Equals(l, r, StringComparison.OrdinalIgnoreCase);
        }

        // > / <
        var gt = condition.IndexOf('>');
        if (gt >= 0)
        {
            int li, ri;
            if (int.TryParse(condition.Substring(0, gt).Trim(), out li) &&
                int.TryParse(condition.Substring(gt + 1).Trim(), out ri))
                return li > ri;
        }

        var lt = condition.IndexOf('<');
        if (lt >= 0)
        {
            int li, ri;
            if (int.TryParse(condition.Substring(0, lt).Trim(), out li) &&
                int.TryParse(condition.Substring(lt + 1).Trim(), out ri))
                return li < ri;
        }

        // bool literal
        bool b;
        if (bool.TryParse(condition, out b)) return b;

        // variable truthiness
        string val;
        if (_variables.TryGetValue(condition.ToLowerInvariant(), out val))
        {
            if (string.IsNullOrEmpty(val)) return false;
            if (val == "0") return false;
            if (val.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        return false;
    }

    private static bool TryHttpGet(string url, out byte[] body, out string status)
    {
        body = null;
        status = null;
        if (string.IsNullOrEmpty(url))
        {
            status = "empty url";
            return false;
        }

        if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            status = "https not supported";
            return false;
        }

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            url = "http://" + url;

        string host;
        int port;
        string path;
        if (!TryParseHttpUrl(url, out host, out port, out path))
        {
            status = "invalid url";
            return false;
        }

        // Cosmos DNS (same style as GoStore)
        if (!TryResolveHost(host, out var serverIP))
        {
            status = "dns failed";
            return false;
        }

        try
        {
            using (var client = new TcpClient())
            {
                // DO NOT set ReceiveTimeout/SendTimeout — triggers setsockopt in Cosmos.
                client.Connect(serverIP, port);
                using (var s = client.GetStream())
                {
                    var req = Encoding.ASCII.GetBytes(
                        "GET " + path + " HTTP/1.0\r\n" +
                        "Host: " + host + "\r\n" +
                        "User-Agent: GoOS-Giff/" + Giff.Version + "\r\n" +
                        "Connection: close\r\n\r\n");
                    s.Write(req, 0, req.Length);

                    var ms = new MemoryStream(8 * 1024);
                    var buf = new byte[2048];
                    int n;
                    while ((n = s.Read(buf, 0, buf.Length)) > 0)
                        ms.Write(buf, 0, n);

                    var all = ms.ToArray();
                    var sep = IndexOf(all, Encoding.ASCII.GetBytes("\r\n\r\n"));
                    if (sep >= 0)
                    {
                        status = ReadStatusLine(all) ?? "ok";
                        var bodyOff = sep + 4;
                        body = new byte[all.Length - bodyOff];
                        if (body.Length > 0) Array.Copy(all, bodyOff, body, 0, body.Length);
                    }
                    else
                    {
                        status = "ok";
                        body = all; // no headers? treat as body
                    }

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            status = ex.Message;
            return false;
        }
    }

    private static bool TryResolveHost(string host, out string ip)
    {
        ip = null;
        try
        {
            var dns = new DnsClient();
            dns.Connect(DNSConfig.DNSNameservers[0]);
            dns.SendAsk(host);
            var addr = dns.Receive();
            dns.Close();
            ip = addr?.ToString();
            return !string.IsNullOrEmpty(ip);
        }
        catch
        {
            return false;
        }
    }


    private static bool TryParseHttpUrl(string url, out string host, out int port, out string path)
    {
        host = "";
        port = 80;
        path = "/";
        try
        {
            var p = url.IndexOf("://", StringComparison.Ordinal);
            var i = p >= 0 ? p + 3 : 0;
            var slash = url.IndexOf('/', i);
            var hostPort = slash >= 0 ? url.Substring(i, slash - i) : url.Substring(i);
            path = slash >= 0 ? url.Substring(slash) : "/";

            var colon = hostPort.LastIndexOf(':');
            if (colon >= 0)
            {
                host = hostPort.Substring(0, colon);
                int.TryParse(hostPort.Substring(colon + 1), out port);
                if (port <= 0) port = 80;
            }
            else
            {
                host = hostPort;
            }

            if (string.IsNullOrEmpty(host)) return false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static int IndexOf(byte[] hay, byte[] needle)
    {
        if (needle.Length == 0) return 0;
        for (var i = 0; i <= hay.Length - needle.Length; i++)
        {
            var ok = true;
            for (var j = 0; j < needle.Length; j++)
                if (hay[i + j] != needle[j])
                {
                    ok = false;
                    break;
                }

            if (ok) return i;
        }

        return -1;
    }

    private static string ReadStatusLine(byte[] all)
    {
        try
        {
            var end = IndexOf(all, Encoding.ASCII.GetBytes("\r\n"));
            if (end <= 0) return null;
            return Encoding.ASCII.GetString(all, 0, end);
        }
        catch
        {
            return null;
        }
    }

    private static string SafeDecodeHttpBody(byte[] body)
    {
        if (body == null || body.Length == 0) return "";
        try
        {
            return Encoding.UTF8.GetString(body);
        }
        catch
        {
        }

        try
        {
            return Encoding.ASCII.GetString(body);
        }
        catch
        {
        }

        return "";
    }

    // --- Value parsers ---
    private static int ParseInt(Dictionary<string, string> props, string key, int def)
    {
        key = key.ToLowerInvariant();
        string v;
        int n;
        return props.TryGetValue(key, out v) &&
               int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)
            ? n
            : def;
    }

    private static string ParseString(Dictionary<string, string> props, string key, string def)
    {
        key = key.ToLowerInvariant();
        string v;
        return props.TryGetValue(key, out v) ? v : def;
    }

    private static bool ParseBool(Dictionary<string, string> props, string key, bool def)
    {
        key = key.ToLowerInvariant();
        string v;
        if (props.TryGetValue(key, out v))
        {
            if (string.Equals(v, "true", StringComparison.OrdinalIgnoreCase) || v == "1") return true;
            if (string.Equals(v, "false", StringComparison.OrdinalIgnoreCase) || v == "0") return false;
        }

        return def;
    }

    private static bool ParseCentreFlag(Dictionary<string, string> props)
    {
        return ParseBool(props, "centre", false) || ParseBool(props, "centrex", false) ||
               ParseBool(props, "center", false) || ParseBool(props, "centerx", false);
    }

    private static Color? ParseColour(string v)
    {
        if (string.IsNullOrEmpty(v)) return null;
        var s = v.ToLowerInvariant();
        if (s == "white") return Color.White;
        if (s == "black") return Color.Black;
        if (s == "lightgray" || s == "lightgrey") return Color.LightGray;
        if (s == "gray" || s == "grey") return Color.DeepGray;
        if (s == "transparent") return Color.Transparent;

        if (v[0] == '#' && v.Length == 7) v = v.Substring(1);
        if (v.Length == 6)
            try
            {
                var r = byte.Parse(v.Substring(0, 2), NumberStyles.HexNumber);
                var g = byte.Parse(v.Substring(2, 2), NumberStyles.HexNumber);
                var b = byte.Parse(v.Substring(4, 2), NumberStyles.HexNumber);
                return new Color(r, g, b);
            }
            catch
            {
                return null;
            }

        return null;
    }
}