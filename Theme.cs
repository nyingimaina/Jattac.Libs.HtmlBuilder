using System.Collections.Generic;

namespace HtmlBuilding
{
    /// <summary>
    /// Defines a collection of styles and attributes that can be applied to HTML elements.
    /// </summary>
    public class Theme
    {
        private readonly Dictionary<string, ElementStyle> _styles = new Dictionary<string, ElementStyle>(System.StringComparer.OrdinalIgnoreCase);

        public void AddStyle(string selector, ElementStyle style)
        {
            _styles[selector] = style;
        }

        public ElementStyle GetStyleFor(string selector)
        {
            return _styles.TryGetValue(selector, out var style) ? style : ElementStyle.Empty;
        }
    }

    /// <summary>
    /// Represents the styles and attributes for a specific HTML element or class.
    /// </summary>
    public class ElementStyle
    {
        public Dictionary<string, string> Styles { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();

        public static ElementStyle Empty => new ElementStyle();

        public ElementStyle SetStyle(string key, string value)
        {
            Styles[key] = value;
            return this;
        }

        public ElementStyle SetAttribute(string key, string value)
        {
            Attributes[key] = value;
            return this;
        }
    }
}
