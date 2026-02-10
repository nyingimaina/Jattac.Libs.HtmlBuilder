using System;
using HtmlBuilding.HtmlModel;

namespace HtmlBuilding
{
    // A generic builder to provide common fluent methods for element nodes.
    public abstract class Builder<TBuilder, TNode> where TBuilder : class where TNode : ElementNode
    {
        protected readonly TNode _node;
        protected readonly Theme _theme;

        internal Builder(TNode node, Theme theme)
        {
            _node = node;
            _theme = theme;
        }

        public TBuilder Style(string key, string value)
        {
            _node.AddStyle(key, value);
            return (this as TBuilder)!;
        }

        public TBuilder Class(string className)
        {
            _node.AddClass(className);
            return (this as TBuilder)!;
        }

        public TBuilder Attr(string key, string value)
        {
            _node.AddAttribute(key, value);
            return (this as TBuilder)!;
        }

        public IHtmlNode GetNode() => _node;
    }

    // Builder for the root of the document.
    public class DocumentBuilder
    {
        private readonly HtmlDocument _doc;
        private readonly Theme _theme;
        internal DocumentBuilder(HtmlDocument doc, Theme theme) { _doc = doc; _theme = theme; }

        public DocumentBuilder AddNode(IHtmlNode node) { _doc.Add(node); return this; }
        
        // --- New Generic Text Methods ---
        public DocumentBuilder Text(string text, string asTag = "p") => AddNode(new TextNode(asTag).With(n => n.AddChild(new RawTextNode(text))));
        public DocumentBuilder Text(Action<TextContentBuilder> config, string asTag = "p")
        {
            var textNode = new TextNode(asTag);
            var builder = new TextContentBuilder(textNode, _theme);
            config(builder);
            return AddNode(textNode);
        }

        // --- Convenience Text Methods ---
        public DocumentBuilder Heading1(string text) => Text(text, "h1");
        public DocumentBuilder Heading2(string text) => Text(text, "h2");
        public DocumentBuilder Paragraph(string text) => Text(text, "p");
        public DocumentBuilder Paragraph(Action<TextContentBuilder> config) => Text(config, "p");

        public DocumentBuilder Image(string src, string alt = "") => AddNode(new ImageNode().With(n => { n.AddAttribute("src", src); n.AddAttribute("alt", alt); }));
        
        public DocumentBuilder List(Action<ListBuilder> config)
        {
            var builder = new ListBuilder(new ListNode(false), _theme);
            config(builder);
            return AddNode(builder.GetNode());
        }

        public DocumentBuilder OrderedList(Action<ListBuilder> config)
        {
            var builder = new ListBuilder(new ListNode(true), _theme);
            config(builder);
            return AddNode(builder.GetNode());
        }

        public DocumentBuilder Table(Action<TableBuilder> config)
        {
            var builder = new TableBuilder(new TableNode(), _theme);
            config(builder);
            return AddNode(builder.GetNode());
        }
    }

    // --- New builder for inline text content ---
    public class TextContentBuilder
    {
        private readonly ElementNode _parentNode;
        private readonly Theme _theme;
        internal TextContentBuilder(ElementNode parentNode, Theme theme) { _parentNode = parentNode; _theme = theme; }

        public TextContentBuilder Raw(string text) { _parentNode.AddChild(new RawTextNode(text)); return this; }
        
        public TextContentBuilder Bold(string text) => Bold(b => b.Raw(text));
        public TextContentBuilder Bold(Action<TextContentBuilder> config)
        {
            var strongNode = new StrongNode();
            var builder = new TextContentBuilder(strongNode, _theme);
            config(builder);
            _parentNode.AddChild(strongNode);
            return this;
        }

        public TextContentBuilder Italic(string text) => Italic(i => i.Raw(text));
        public TextContentBuilder Italic(Action<TextContentBuilder> config)
        {
            var emNode = new EmNode();
            var builder = new TextContentBuilder(emNode, _theme);
            config(builder);
            _parentNode.AddChild(emNode);
            return this;
        }

        public TextContentBuilder Link(string href, string text) => Link(href, l => l.Raw(text));
        public TextContentBuilder Link(string href, Action<TextContentBuilder> config)
        {
            var linkNode = new LinkNode();
            linkNode.AddAttribute("href", href);

            var builder = new TextContentBuilder(linkNode, _theme);
            config(builder);
            _parentNode.AddChild(linkNode);
            return this;
        }
    }

    public class ListBuilder : Builder<ListBuilder, ListNode>
    {
        internal ListBuilder(ListNode node, Theme theme) : base(node, theme) { }
        public ListBuilder Item(string text) => Item(new RawTextNode(text));
        public ListBuilder Item(IHtmlNode content)
        {
            _node.AddChild(new ListItemNode().With(n => n.AddChild(content)));
            return this;
        }
    }

    public class TableBuilder : Builder<TableBuilder, TableNode>
    {
        internal TableBuilder(TableNode node, Theme theme) : base(node, theme) { }
        public TableBuilder Row(Action<RowBuilder> config)
        {
            var builder = new RowBuilder(new TableRowNode(), _theme);
            config(builder);
            _node.AddChild(builder.GetNode());
            return this;
        }
    }

    public class RowBuilder : Builder<RowBuilder, TableRowNode>
    {
        internal RowBuilder(TableRowNode node, Theme theme) : base(node, theme) { }
        public RowBuilder Cell(string text) => Cell(new RawTextNode(text));
        public RowBuilder Cell(IHtmlNode content)
        {
            _node.AddChild(new TableCellNode(false).With(n => n.AddChild(content)));
            return this;
        }
        public RowBuilder HeaderCell(string text) => HeaderCell(new RawTextNode(text));
        public RowBuilder HeaderCell(IHtmlNode content)
        {
            _node.AddChild(new TableCellNode(true).With(n => n.AddChild(content)));
            return this;
        }
    }

    internal static class NodeExtensions
    {
        internal static T With<T>(this T node, Action<T> configure) where T : ElementNode
        {
            configure(node);
            return node;
        }
    }
}
