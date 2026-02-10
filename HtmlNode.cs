using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace HtmlBuilding
{
    /// <summary>
    /// Abstract base class for all HTML elements, providing common functionality for styling, attributes, and building.
    /// </summary>
    public abstract class HtmlNode : IHtmlNode
    {
        protected readonly string _tag;
        protected readonly List<string> _localClasses = new List<string>();
        protected readonly Dictionary<string, string> _localStyles = new Dictionary<string, string>();
        protected readonly Dictionary<string, string> _localAttributes = new Dictionary<string, string>();
        
        // Content can be simple text or other nodes. A list of nodes is more generic.
        protected readonly List<IHtmlNode> _children = new List<IHtmlNode>();

        protected HtmlNode(string tag)
        {
            _tag = tag;
        }

        public T Class<T>(string className) where T : HtmlNode
        {
            if (!string.IsNullOrWhiteSpace(className))
            {
                _localClasses.Add(className);
            }
            return (T)this;
        }

        public T Style<T>(string key, string value) where T : HtmlNode
        {
            _localStyles[key] = value;
            return (T)this;
        }

        public T Attr<T>(string key, string value) where T : HtmlNode
        {
            _localAttributes[key] = value;
            return (T)this;
        }

        protected void Add(IHtmlNode child)
        {
            _children.Add(child);
        }

        public abstract string Build(Theme theme);

        protected virtual void Validate()
        {
            // Base validation (can be overridden by subclasses)
        }
        
        protected string BuildTag(Theme theme, bool selfClosing = false)
        {
            Validate();

            // --- Attribute and Style Resolution ---

            var finalAttributes = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            var finalStyles = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            var finalClasses = new HashSet<string>();

            // 1. Apply theme styles for the tag
            var tagThemeStyle = theme.GetStyleFor(_tag);
            MergeStyles(finalStyles, tagThemeStyle.Styles);
            MergeAttributes(finalAttributes, tagThemeStyle.Attributes, finalClasses);

            // 2. Apply theme styles for each class added via .Class()
            foreach (var localClass in _localClasses)
            {
                var classThemeStyle = theme.GetStyleFor("." + localClass);
                MergeStyles(finalStyles, classThemeStyle.Styles);
                MergeAttributes(finalAttributes, classThemeStyle.Attributes, finalClasses);
            }

            // 3. Apply local styles and attributes (these have precedence)
            MergeStyles(finalStyles, _localStyles);
            MergeAttributes(finalAttributes, _localAttributes, finalClasses);
            
            // Add classes from .Class() method
            foreach(var cls in _localClasses) finalClasses.Add(cls);


            // --- String Building ---

            var sb = new StringBuilder();
            sb.Append($"<{_tag}");

            // Combine all classes and add to attributes
            if (finalClasses.Any())
            {
                finalAttributes["class"] = string.Join(" ", finalClasses);
            }

            // Append all final attributes
            foreach (var attr in finalAttributes)
            {
                sb.Append($" {attr.Key}=\"{WebUtility.HtmlEncode(attr.Value)}\"");
            }

            // Append all final styles
            if (finalStyles.Any())
            {
                var styleString = string.Join(";", finalStyles.Select(kv => $"{kv.Key}:{kv.Value}"));
                sb.Append($" style=\"{styleString}\"");
            }

            if (selfClosing)
            {
                sb.Append(" />");
            }
            else
            {
                sb.Append(">");
                foreach (var child in _children)
                {
                    sb.Append(child.Build(theme));
                }
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
                if (kv.Key.Equals("class", System.StringComparison.OrdinalIgnoreCase))
                {
                    var splitClasses = kv.Value.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s));
                    foreach(var cls in splitClasses) classes.Add(cls);
                }
                else
                {
                    target[kv.Key] = kv.Value;
                }
            }
        }
    }
}