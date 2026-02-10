using System;
using System.Collections.Generic;
using System.Net;

namespace HtmlBuilding
{
    // Node for simple, raw text that will be HTML encoded
    public class RawText : IHtmlNode
    {
        private readonly string _text;
        public RawText(string text) { _text = text; }
        public string Build(Theme theme) => WebUtility.HtmlEncode(_text);
    }

    // Node for paragraphs, headings, etc.
    public class Text : HtmlNode
    {
        public Text(string text, string asTag = "p") : base(asTag)
        {
            Add(new RawText(text));
        }

        public Text(params IHtmlNode[] children) : this("p", children) { }
        public Text(string asTag = "p", params IHtmlNode[] children) : base(asTag)
        {
            foreach(var child in children) Add(child);
        }

        public override string Build(Theme theme) => BuildTag(theme);
    }

    public class Bold : HtmlNode
    {
        public Bold(string text) : base("strong")
        {
            Add(new RawText(text));
        }
        public override string Build(Theme theme) => BuildTag(theme);
    }
    
    public class Italics : HtmlNode
    {
        public Italics(string text) : base("em")
        {
            Add(new RawText(text));
        }
        public override string Build(Theme theme) => BuildTag(theme);
    }

    public class Image : HtmlNode
    {
        public Image(string src, string alt = "") : base("img")
        {
            Attr<Image>("src", src);
            if (!string.IsNullOrEmpty(alt))
            {
                Attr<Image>("alt", alt);
            }
        }
        protected override void Validate()
        {
            if (!_localAttributes.ContainsKey("src") || string.IsNullOrWhiteSpace(_localAttributes["src"]))
            {
                throw new InvalidOperationException("Image 'src' attribute cannot be empty.");
            }
        }
        public override string Build(Theme theme) => BuildTag(theme, selfClosing: true);
    }

    public class Link : HtmlNode
    {
        public Link(string href, IHtmlNode content) : base("a")
        {
            Attr<Link>("href", href);
            Add(content);
        }
        public Link(string href, string textContent) : this(href, new RawText(textContent)) {}
        
        protected override void Validate()
        {
            if (!_localAttributes.ContainsKey("href") || string.IsNullOrWhiteSpace(_localAttributes["href"]))
            {
                throw new InvalidOperationException("Link 'href' attribute cannot be empty.");
            }
        }
        public override string Build(Theme theme) => BuildTag(theme);
    }

    public class Table : HtmlNode
    {
        public Table() : base("table") { }
        public Table AddRow(params TableCell[] cells)
        {
            var row = new TableRow();
            foreach (var cell in cells) row.Add(cell);
            Add(row);
            return this;
        }
        public override string Build(Theme theme) => BuildTag(theme);
    }

    public class TableRow : HtmlNode
    {
        public TableRow() : base("tr") { }
        public override string Build(Theme theme) => BuildTag(theme);
    }

    public class TableCell : HtmlNode
    {
        public TableCell(IHtmlNode content, bool isHeader = false) : base(isHeader ? "th" : "td")
        {
            Add(content);
        }
        public TableCell(string textContent, bool isHeader = false) : this(new RawText(textContent), isHeader) {}
        public override string Build(Theme theme) => BuildTag(theme);
    }

    public class List : HtmlNode
    {
        public List(bool ordered = false) : base(ordered ? "ol" : "ul") { }
        public List AddItem(IHtmlNode content)
        {
            Add(new ListItem(content));
            return this;
        }
        public List AddItem(string textContent) => AddItem(new RawText(textContent));
        public override string Build(Theme theme) => BuildTag(theme);
    }

    public class ListItem : HtmlNode
    {
        public ListItem(IHtmlNode content) : base("li")
        {
            Add(content);
        }
        public override string Build(Theme theme) => BuildTag(theme);
    }
}
