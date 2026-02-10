using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HtmlBuilding.HtmlModel
{
    // A simple node for raw text content, which will be HTML encoded.
    public class RawTextNode : IHtmlNode
    {
        private readonly string _text;
        public RawTextNode(string text) { _text = text; }
        public string Build(Theme theme) => WebUtility.HtmlEncode(_text);
    }

    // Base class for all element-based nodes.
    public abstract class ElementNode : IHtmlNode
    {
        protected readonly string _tag;
        protected readonly List<string> _classes = new List<string>();
        protected readonly Dictionary<string, string> _styles = new Dictionary<string, string>();
        protected readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();
        protected readonly List<IHtmlNode> _children = new List<IHtmlNode>();

        public ElementNode(string tag) { _tag = tag; }

        public void AddChild(IHtmlNode child) => _children.Add(child);
        public void AddClass(string className) => _classes.Add(className);
        public void AddStyle(string key, string value) => _styles[key] = value;
        public void AddAttribute(string key, string value) => _attributes[key] = value;
        
        protected virtual void Validate() { }

        public string Build(Theme theme)
        {
            Validate();

            var finalAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var finalStyles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var finalClasses = new HashSet<string>();

            // 1. Apply theme styles
            var tagThemeStyle = theme.GetStyleFor(_tag);
            MergeStyles(finalStyles, tagThemeStyle.Styles);
            MergeAttributes(finalAttributes, tagThemeStyle.Attributes, finalClasses);

            foreach (var cls in _classes)
            {
                var classThemeStyle = theme.GetStyleFor("." + cls);
                MergeStyles(finalStyles, classThemeStyle.Styles);
                MergeAttributes(finalAttributes, classThemeStyle.Attributes, finalClasses);
            }

            // 2. Apply local styles and attributes
            MergeStyles(finalStyles, _styles);
            MergeAttributes(finalAttributes, _attributes, finalClasses);
            
            foreach(var cls in _classes) finalClasses.Add(cls);

            // 3. Build string
            var sb = new StringBuilder();
            sb.Append($"<{_tag}");

            if (finalClasses.Any()) finalAttributes["class"] = string.Join(" ", finalClasses);

            foreach (var attr in finalAttributes) sb.Append($" {attr.Key}=\"{WebUtility.HtmlEncode(attr.Value)}\"");
            
            if (finalStyles.Any())
            {
                var styleParts = finalStyles.Select(kv => $"{kv.Key}:{kv.Value}");
                var styleString = string.Join(";", styleParts);
                sb.Append($" style=\"{styleString}\"");
            }
            
            bool selfClosing = _tag == "img";
            if (selfClosing)
            {
                sb.Append(" />");
            }
            else
            {
                sb.Append(">");
                foreach (var child in _children) sb.Append(child.Build(theme));
                sb.Append($"</{_tag}>");
            }

            return sb.ToString();
        }

        private void MergeStyles(Dictionary<string, string> target, Dictionary<string, string> source)
        {
            foreach (var kv in source) target[kv.Key] = kv.Value;
        }

        private void MergeAttributes(Dictionary<string, string> target, Dictionary<string, string> source, HashSet<string> classes)
        {
            foreach (var kv in source)
            {
                if (kv.Key.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    foreach(var cls in kv.Value.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s))) classes.Add(cls);
                }
                else
                {
                    target[kv.Key] = kv.Value;
                }
            }
        }
    }

    // Concrete model nodes
    public class TextNode : ElementNode { public TextNode(string tag) : base(tag) { } }
    public class StrongNode : ElementNode { public StrongNode() : base("strong") { } } // New
    public class EmNode : ElementNode { public EmNode() : base("em") { } } // New
    public class LinkNode : ElementNode 
    { 
        public LinkNode() : base("a") { }
        protected override void Validate() 
        {
            if (!_attributes.ContainsKey("href") || string.IsNullOrWhiteSpace(_attributes["href"]))
                throw new InvalidOperationException("Link 'href' attribute cannot be empty.");
        }
    }
    public class ImageNode : ElementNode 
    {
        public ImageNode() : base("img") { }
        protected override void Validate() 
        {
            if (!_attributes.ContainsKey("src") || string.IsNullOrWhiteSpace(_attributes["src"]))
                throw new InvalidOperationException("Image 'src' attribute cannot be empty.");
        }
    }
    public class TableNode : ElementNode { public TableNode() : base("table") { } }
    public class TableRowNode : ElementNode { public TableRowNode() : base("tr") { } }
    public class TableCellNode : ElementNode { public TableCellNode(bool isHeader) : base(isHeader ? "th" : "td") { } }
    public class ListNode : ElementNode { public ListNode(bool ordered) : base(ordered ? "ol" : "ul") { } }
    public class ListItemNode : ElementNode { public ListItemNode() : base("li") { } }
}