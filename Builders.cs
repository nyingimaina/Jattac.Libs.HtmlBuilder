using System;
using HtmlBuilding.HtmlModel;

namespace HtmlBuilding
{
    // A generic builder to provide common fluent methods.
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
            return (this as TBuilder)!; // Added null-forgiving operator
        }

        public TBuilder Class(string className)
        {
            _node.AddClass(className);
            return (this as TBuilder)!; // Added null-forgiving operator
        }

        public TBuilder Attr(string key, string value)
        {
            _node.AddAttribute(key, value);
            return (this as TBuilder)!; // Added null-forgiving operator
        }

        /// <summary>
        /// For standalone usage, gets the underlying node to be added to a parent.
        /// </summary>
        public IHtmlNode GetNode() => _node;
    }

    public class DocumentBuilder
    {
        private readonly HtmlDocument _doc;
        private readonly Theme _theme;
        internal DocumentBuilder(HtmlDocument doc, Theme theme) { _doc = doc; _theme = theme; }

        public DocumentBuilder AddNode(IHtmlNode node) { _doc.Add(node); return this; }
        public DocumentBuilder Heading1(string text) => AddNode(new TextNode("h1").With(n => n.AddChild(new RawTextNode(text))));
        public DocumentBuilder Heading2(string text) => AddNode(new TextNode("h2").With(n => n.AddChild(new RawTextNode(text))));
        public DocumentBuilder Paragraph(string text) => AddNode(new TextNode("p").With(n => n.AddChild(new RawTextNode(text))));
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

    // Helper extension for cleaner internal syntax
    internal static class NodeExtensions
    {
        internal static T With<T>(this T node, Action<T> configure) where T : ElementNode
        {
            configure(node);
            return node;
        }
    }
}