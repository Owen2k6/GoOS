#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoOS.GUI.Apps.GoWeb.Html;

namespace GoOS.GUI.Apps.GoWeb.Parsers
{
    public static class HtmlParser
    {
        private static readonly string[] _singletons = new string[]
        {
            "area",
            "base",
            "br",
            "col",
            "embed",
            "hr",
            "img",
            "input",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr"
        };

        private static readonly (string name, string entity)[] _namedEntities =
        {
            ("&quot;", "\""),
            ("&num;", "#"),
            ("&dollar;", "$"),
            ("&percnt;", "%"),
            ("&amp;", "&"),
            ("&apos;", "'"),
            ("&lpar;", "("),
            ("&rpar;", ")"),
            ("&ast;", "*"),
            ("&plus;", "+"),
            ("&comma;", ","),
            ("&minus;", "-"),
            ("&period;", "."),
            ("&sol;", "/"),
            ("&colon;", ":"),
            ("&semi;", ";"),
            ("&lt;", "<"),
            ("&equals;", "="),
            ("&gt;", ">"),
            ("&quest;", "?"),
            ("&commat;", "@"),
            ("&lsqb;", "["),
            ("&bsol;", "\\"),
            ("&rsqb;", "]"),
            ("&Hat;", "^"),
            ("&lowbar;", "_"),
            ("&grave;", "`"),
            ("&lcub;", "{"),
            ("&verbar;", "|"),
            ("&rcub;", "}"),
            ("&tilde;", "˜"),
            ("&nbsp;", " "),
            ("&iexcl;", "¡"),
            ("&cent;", "¢"),
            ("&pound;", "£"),
            ("&curren;", "¤"),
            ("&yen;", "¥"),
            ("&#x20B9;", "₹"),
            ("&brvbar;", "¦"),
            ("&sect;", "§"),
            ("&uml;", "¨"),
            ("&copy;", "©"),
            ("&ordf;", "ª"),
            ("&laquo;", "«"),
            ("&not;", "¬"),
            ("&shy;", ""),
            ("&reg;", "®"),
            ("&macr;", "¯"),
            ("&deg;", "°"),
            ("&plusmn;", "±"),
            ("&sup2;", "²"),
            ("&sup3;", "³"),
            ("&acute;", "´"),
            ("&micro;", "µ"),
            ("&para;", "¶"),
            ("&middot;", "·"),
            ("&cedil;", "¸"),
            ("&sup1;", "¹"),
            ("&ordm;", "º"),
            ("&raquo;", "»"),
            ("&frac14;", "¼"),
            ("&frac12;", "½"),
            ("&frac34;", "¾"),
            ("&iquest;", "¿"),
            ("&Agrave;", "À"),
            ("&Aacute;", "Á"),
            ("&Acirc;", "Â"),
            ("&Atilde;", "Ã"),
            ("&Auml;", "Ä"),
            ("&Aring;", "Å"),
            ("&AElig;", "Æ"),
            ("&Ccedil;", "Ç"),
            ("&Egrave;", "È"),
            ("&Eacute;", "É"),
            ("&Ecirc;", "Ê"),
            ("&Euml;", "Ë"),
            ("&Igrave;", "Ì"),
            ("&Iacute;", "Í"),
            ("&Icirc;", "Î"),
            ("&Iuml;", "Ï"),
            ("&ETH;", "Ð"),
            ("&Ntilde;", "Ñ"),
            ("&Ograve;", "Ò"),
            ("&Oacute;", "Ó"),
            ("&Ocirc;", "Ô"),
            ("&Otilde;", "Õ"),
            ("&Ouml;", "Ö"),
            ("&times;", "×"),
            ("&Oslash;", "Ø"),
            ("&Ugrave;", "Ù"),
            ("&Uacute;", "Ú"),
            ("&Ucirc;", "Û"),
            ("&Uuml;", "Ü"),
            ("&Yacute;", "Ý"),
            ("&THORN;", "Þ"),
            ("&szlig;", "ß"),
            ("&agrave;", "à"),
            ("&aacute;", "á"),
            ("&acirc;", "â"),
            ("&atilde;", "ã"),
            ("&auml;", "ä"),
            ("&aring;", "å"),
            ("&aelig;", "æ"),
            ("&ccedil;", "ç"),
            ("&egrave;", "è"),
            ("&eacute;", "é"),
            ("&ecirc;", "ê"),
            ("&euml;", "ë"),
            ("&igrave;", "ì"),
            ("&iacute;", "í"),
            ("&icirc;", "î"),
            ("&iuml;", "ï"),
            ("&eth;", "ð"),
            ("&ntilde;", "ñ"),
            ("&ograve;", "ò"),
            ("&oacute;", "ó"),
            ("&ocirc;", "ô"),
            ("&otilde;", "õ"),
            ("&ouml;", "ö"),
            ("&divide;", "÷"),
            ("&oslash;", "ø"),
            ("&ugrave;", "ù"),
            ("&uacute;", "ú"),
            ("&ucirc;", "û"),
            ("&uuml;", "ü"),
            ("&yacute;", "ý"),
            ("&thorn;", "þ"),
            ("&yuml;", "ÿ"),
            ("&OElig;", "Œ"),
            ("&oelig;", "œ"),
            ("&Scaron;", "Š"),
            ("&scaron;", "š"),
            ("&Yuml;", "Ÿ"),
            ("&fnof;", "ƒ"),
            ("&circ;", "ˆ"),
            ("&Alpha;", "Α"),
            ("&Beta;", "Β"),
            ("&Gamma;", "Γ"),
            ("&Delta;", "Δ"),
            ("&Epsilon;", "Ε"),
            ("&Zeta;", "Ζ"),
            ("&Eta;", "Η"),
            ("&Theta;", "Θ"),
            ("&Iota;", "Ι"),
            ("&Kappa;", "Κ"),
            ("&Lambda;", "Λ"),
            ("&Mu;", "Μ"),
            ("&Nu;", "Ν"),
            ("&Xi;", "Ξ"),
            ("&Omicron;", "Ο"),
            ("&Pi;", "Π"),
            ("&Rho;", "Ρ"),
            ("&Sigma;", "Σ"),
            ("&Tau;", "Τ"),
            ("&Upsilon;", "Υ"),
            ("&Phi;", "Φ"),
            ("&Chi;", "Χ"),
            ("&Psi;", "Ψ"),
            ("&Omega;", "Ω"),
            ("&alpha;", "α"),
            ("&beta;", "β"),
            ("&gamma;", "γ"),
            ("&delta;", "δ"),
            ("&epsilon;", "ε"),
            ("&zeta;", "ζ"),
            ("&eta;", "η"),
            ("&theta;", "θ"),
            ("&iota;", "ι"),
            ("&kappa;", "κ"),
            ("&lambda;", "λ"),
            ("&mu;", "μ"),
            ("&nu;", "ν"),
            ("&xi;", "ξ"),
            ("&omicron;", "ο"),
            ("&pi;", "π"),
            ("&rho;", "ρ"),
            ("&sigmaf;", "ς"),
            ("&sigma;", "σ"),
            ("&tau;", "τ"),
            ("&upsilon;", "υ"),
            ("&phi;", "φ"),
            ("&chi;", "χ"),
            ("&psi;", "ψ"),
            ("&omega;", "ω"),
            ("&thetasym;", "ϑ"),
            ("&upsih;", "ϒ"),
            ("&piv;", "ϖ"),
            ("&ensp;", " "),
            ("&emsp;", " "),
            ("&thinsp;", " "),
            ("&zwnj;", ""),
            ("&zwj;", ""),
            ("&lrm;", ""),
            ("&rlm;", ""),
            ("&ndash;", "–"),
            ("&mdash;", "—"),
            ("&lsquo;", "‘"),
            ("&rsquo;", "’"),
            ("&sbquo;", "‚"),
            ("&ldquo;", "“"),
            ("&rdquo;", "”"),
            ("&bdquo;", "„"),
            ("&dagger;", "†"),
            ("&Dagger;", "‡"),
            ("&permil;", "‰"),
            ("&lsaquo;", "‹"),
            ("&rsaquo;", "›"),
            ("&bull;", "•"),
            ("&hellip;", "…"),
            ("&prime;", "′"),
            ("&Prime;", "″"),
            ("&oline;", "‾"),
            ("&frasl;", "⁄"),
            ("&weierp;", "℘"),
            ("&image;", "ℑ"),
            ("&real;", "ℜ"),
            ("&trade;", "™"),
            ("&alefsym;", "ℵ"),
            ("&larr;", "←"),
            ("&uarr;", "↑"),
            ("&rarr;", "→"),
            ("&darr;", "↓"),
            ("&harr;", "↔"),
            ("&crarr;", "↵"),
            ("&lArr;", "⇐"),
            ("&uArr;", "⇑"),
            ("&rArr;", "⇒"),
            ("&dArr;", "⇓"),
            ("&hArr;", "⇔"),
            ("&forall;", "∀"),
            ("&part;", "∂"),
            ("&exist;", "∃"),
            ("&empty;", "∅"),
            ("&nabla;", "∇"),
            ("&isin;", "∈"),
            ("&notin;", "∉"),
            ("&ni;", "∋"),
            ("&prod;", "∏"),
            ("&sum;", "∑"),
            ("&minus;", "−"),
            ("&lowast;", "∗"),
            ("&radic;", "√"),
            ("&prop;", "∝"),
            ("&infin;", "∞"),
            ("&ang;", "∠"),
            ("&and;", "∧"),
            ("&or;", "∨"),
            ("&cap;", "∩"),
            ("&cup;", "∪"),
            ("&int;", "∫"),
            ("&there4;", "∴"),
            ("&sim;", "∼"),
            ("&cong;", "≅"),
            ("&asymp;", "≈"),
            ("&ne;", "≠"),
            ("&equiv;", "≡"),
            ("&le;", "≤"),
            ("&ge;", "≥"),
            ("&sub;", "⊂"),
            ("&sup;", "⊃"),
            ("&nsub;", "⊄"),
            ("&sube;", "⊆"),
            ("&supe;", "⊇"),
            ("&oplus;", "⊕"),
            ("&otimes;", "⊗"),
            ("&perp;", "⊥"),
            ("&sdot;", "⋅"),
            ("&lceil;", "⌈"),
            ("&rceil;", "⌉"),
            ("&lfloor;", "⌊"),
            ("&rfloor;", "⌋"),
            ("&lang;", "〈"),
            ("&rang;", "〉"),
            ("&loz;", "◊"),
            ("&spades;", "♠"),
            ("&clubs;", "♣"),
            ("&hearts;", "♥"),
            ("&diams;", "♦"),
        };

        private static string DecodeEntities(string str)
        {
            /*
             * TODO: numeric entities
             */
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '&')
                {
                    foreach ((string name, string entity) in _namedEntities)
                    {
                        if (name.Length > str.Length - i)
                        {
                            continue;
                        }
                        if (str.IndexOf(name, i, name.Length) != -1)
                        {
                            str = str.Remove(i, name.Length).Insert(i, entity);
                            break;
                        }
                    }
                }
            }
            return str;
        }

        private static bool IsSingleton(string tag)
        {
            return _singletons.Contains(tag);
        }

        private static string[] SplitOutsideQuotes(string str)
        {
            var result = new List<string>();
            var inQuotes = false;
            var builder = new StringBuilder();
            foreach (char c in str)
            {
                switch (c)
                {
                    case '"':
                        inQuotes = !inQuotes;
                        break;
                    case ' ' when !inQuotes:
                        result.Add(builder.ToString());
                        builder.Clear();
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            result.Add(builder.ToString());
            return result.ToArray();
        }

        public static Element Parse(string text)
        {
            var root = new RootNode();
            Element current = root;
            var stack = new List<Element>() { current };
            var startingIndex = 0;

            void Push(Element element)
            {
                stack.Add(element);
                current.Add(element);
                current = element;
            }

            void Pop()
            {
                if (current == root) return;
                stack.RemoveAt(stack.Count - 1);
                current = stack[^1];
            }

            text = text.Replace("\r\n", "\n");
            text = text.Trim();
            if (text.StartsWith("<!"))
            {
                startingIndex = text.IndexOf('>') + 1;
            }

            for (var i = startingIndex; i < text.Length; i++)
            {
                var c = text[i];
                var lastChar = i == text.Length - 1;
                string innerText = "";
                if (c == '<')
                {
                    if (lastChar) break;

                    var isClosingTag = text[i + 1] == '/';
                    var selfClosed = false;

                    string tagContent = "";
                    i += isClosingTag ? 2 : 1;
                    if (i >= text.Length) break;
                    while (text[i] != '>')
                    {
                        if (text[i] == '/' && text[i + 1] == '>')
                        {
                            selfClosed = true;
                            i++;
                            break;
                        }
                        tagContent += text[i];
                        i++;
                    }

                    var split = SplitOutsideQuotes(tagContent);
                    var tag = split[0];

                    if (isClosingTag)
                    {
                        if (IsSingleton(tag)) continue;
                        Pop();
                    }
                    else
                    {
                        var element = Element.FromTag(tag);

                        for (var j = 1; j < split.Length; j++)
                        {
                            var s = split[j];
                            if (string.IsNullOrWhiteSpace(s)) continue;
                            if (s.Contains('='))
                            {
                                string[] kv = s.Split('=');
                                if (element.Attributes.ContainsKey(kv[0])) continue;
                                element.Attributes[kv[0]] = kv[1].Trim('"');
                            }
                            else
                            {
                                if (element.Attributes.ContainsKey(s)) continue;
                                element.Attributes.Add(s, "");
                            }
                        }

                        Push(element);
                        if (IsSingleton(tag) || selfClosed)
                        {
                            Pop();
                        }
                    }
                }
                else
                {
                    while (text[i] != '<')
                    {
                        innerText += text[i];
                        i++;
                        if (i == text.Length) break;
                    }
                    i--;

                    if (innerText != string.Empty)
                    {
                        innerText = DecodeEntities(innerText);
                        var textNode = new TextNode(innerText);
                        Push(textNode);
                        Pop();
                    }
                }
            }

            return root;
        }
    }
}
