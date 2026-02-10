using System;
using System.Collections.Generic;
using System.Text;

namespace Jattac.Libs.HtmlBuilder
{
    /// <summary>
    /// Represents the root of an HTML document, holding the theme and top-level elements.
    /// This is the main entry point for creating HTML.
    /// </summary>
    public class HtmlDocument
    {
        private readonly List<IHtmlNode> _rootNodes = new List<IHtmlNode>();
        private readonly Theme _theme;

        /// <summary>
        /// Initializes a new document, optionally configuring it with a theme and an expression-based builder.
        /// </summary>
        /// <param name="theme">The theme to apply to all elements. If null, an empty theme is used.</param>
        /// <param name="configure">An action that uses a DocumentBuilder to fluently construct the HTML.</param>
        public HtmlDocument(Theme? theme = null, Action<DocumentBuilder>? configure = null) // Changed theme to nullable
        {
            _theme = theme ?? new Theme();
            
            if (configure != null)
            {
                var builder = new DocumentBuilder(this, _theme);
                configure(builder);
            }
        }

        /// <summary>
        /// Imperatively adds a pre-built node to the document. Useful for standalone builder usage.
        /// </summary>
        public void Add(IHtmlNode node)
        {
            _rootNodes.Add(node);
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
