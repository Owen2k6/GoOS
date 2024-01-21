#nullable enable
using GoOS.GUI.Apps.GoWeb.Render;
using PrismAPI.Graphics;
using PrismAPI.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoOS.GUI.Apps.GoWeb.Html
{
    public abstract class Element
    {
        private static readonly string[] _blockLevelTags = new string[]
        {
            "address",
            "article",
            "aside",
            "blockquote",
            "canvas",
            "dd",
            "div",
            "dl",
            "dt",
            "fieldset",
            "figcaption",
            "figure",
            "footer",
            "form",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "header",
            "hgroup",
            "hr",
            "li",
            "main",
            "nav",
            "noscript",
            "ol",
            "output",
            "p",
            "pre",
            "section",
            "table",
            "tfoot",
            "ul",
            "video"
        };

        public static Element FromTag(string tag)
        {
            tag = tag.ToLower();
            return tag switch
            {
                "html" => new HtmlElement(),
                "body" => new BodyElement(),
                "h1" => new HeadingElement(HeadingElement.HeadingLevel.H1),
                "h2" => new HeadingElement(HeadingElement.HeadingLevel.H2),
                "h3" => new HeadingElement(HeadingElement.HeadingLevel.H3),
                "h4" => new HeadingElement(HeadingElement.HeadingLevel.H4),
                "h5" => new HeadingElement(HeadingElement.HeadingLevel.H5),
                "h6" => new HeadingElement(HeadingElement.HeadingLevel.H6),
                "title" => new TitleElement(),
                "br" => new BreakElement(),
                "a" => new AnchorElement(),
                "pre" => new PreElement(),
                "hr" => new HrElement(),
                "style" => new StyleElement(),
                "script" => new ScriptElement(),
                "u" => new UnderlineElement(),
                _ => new UnknownElement(tag),
            };
        }

        public bool IsBlockLevel() => _blockLevelTags.Contains(GetTag());

        public abstract string GetTag();

        public bool Selector(string selector)
        {
            selector = selector.Trim();

            if (selector == string.Empty) return false;
            if (selector == GetTag()) return true;
            if (selector == "*") return true;

            if (selector[0] == '.')
            {
                var checkClass = selector.Substring(1);
                Attributes.TryGetValue("class", out var classes);
                if (classes != null)
                {
                    foreach (string @class in classes.Split(null))
                    {
                        if (@class == checkClass)
                        {
                            return true;
                        }
                    }
                }
            }

            if (selector[0] == '#')
            {
                var checkId = selector.Substring(1);
                Attributes.TryGetValue("id", out var id);
                if (id == null) return false;
                if (id == checkId)
                {
                    return true;
                }
            }

            return false;
        }

        public Element? QuerySelector(string selector)
        {
            if (Selector(selector)) return this;
            foreach (var child in Children)
            {
                var result = child.QuerySelector(selector);
                if (result is not null) return result;
            }
            return null;
        }

        public virtual string TextContent
        {
            get
            {
                var result = string.Empty;
                foreach (var child in Children)
                {
                    result += child.TextContent;
                }
                return result;
            }
        }

        public void Add(Element element)
        {
            if (element == this || Children.Contains(element))
            {
                return;
            }
            Children.Add(element);
            element.Parent = this;
        }

        public virtual Font GetFont()
        {
            if (Parent != null)
            {
                return Parent.GetFont();
            }
            return Resources.Font_1x;
        }

        public virtual Color GetColor()
        {
            if (Parent != null)
            {
                return Parent.GetColor();
            }
            return Color.Black;
        }

        public virtual bool GetUnderline()
        {
            if (Parent != null)
            {
                return Parent.GetUnderline();
            }
            return false;
        }

        public virtual bool IsPreformatted()
        {
            if (Parent != null)
            {
                return Parent.IsPreformatted();
            }
            return false;
        }

        public virtual bool IsVisible()
        {
            if (Parent != null)
            {
                return Parent.IsVisible();
            }
            return true;
        }

        public virtual void Render(RenderContext ctx)
        {
            if (!IsVisible())
            {
                return;
            }
            foreach (Element child in Children)
            {
                child.Render(ctx);
            }
        }

        public int X = 0;
        public int Y = 0;

        public bool Contains(Element element) => Children.Contains(element);

        public Element? Parent { get; private set; }

        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public List<Element> Children { get; init; } = new List<Element>();
    }
}
