using System.Collections.Generic;
using System.Text;

namespace HtmlBuilding
{
    /// <summary>
    /// Represents the root of an HTML document, holding the theme and top-level elements.
    /// </summary>
    public class HtmlDocument
    {
        private readonly List<IHtmlNode> _rootNodes = new List<IHtmlNode>();
        private readonly Theme _theme;

        /// <summary>
        /// Initializes a new document with an optional theme.
        /// </summary>
        /// <param name="theme">The theme to apply to all elements. If null, an empty theme is used.</param>
        public HtmlDocument(Theme theme = null)
        {
            _theme = theme ?? new Theme();
        }

        /// <summary>
        /// Adds a top-level HTML node to the document.
        /// </summary>
        /// <param name="node">The node to add (e.g., a Text, Table, or List).</param>
        public HtmlDocument Add(IHtmlNode node)
        {
            _rootNodes.Add(node);
            return this;
        }

        /// <summary>
        /// Builds all nodes in the document into a single HTML string.
        /// </summary>
        /// <returns>The final, complete HTML string.</returns>
        public string Build()
        {
            var sb = new StringBuilder();
            foreach (var node in _rootNodes)
            {
                sb.Append(node.Build(_theme));
            }
            return sb.ToString();
        }
    }
}
