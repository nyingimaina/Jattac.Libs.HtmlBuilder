namespace HtmlBuilding
{
    /// <summary>
    /// Represents any object that can be built into an HTML string.
    /// </summary>
    public interface IHtmlNode
    {
        /// <summary>
        /// Builds the node into an HTML string using the provided theme.
        /// </summary>
        /// <param name="theme">The theme to apply during the build.</param>
        /// <returns>A string of HTML.</returns>
        string Build(Theme theme);
    }
}
