# Fluent HTML Builder

A C# library for programmatically building responsive, email-client-compatible HTML with a focus on a dream developer experience (DX).

This library is designed from the ground up to be:
- **Fluent & Discoverable:** An intuitive, chainable API that's easy to explore with IDE IntelliSense.
- **Readable & Self-Documenting:** Your C# code will look like the HTML structure it creates.
- **DRY (Don't Repeat Yourself):** A powerful theming system allows you to define styles and attributes once and reuse them everywhere.
- **Safe & Robust:** All content is HTML-encoded by default, and a fail-fast validation system ensures you never generate broken HTML.

---

## 🚀 Getting Started

Here's a simple "Hello, World" to get a feel for the library.

```csharp
var doc = new HtmlDocument();

doc.Add(new Text("Hello, World!", asTag: "h1"));

string html = doc.Build();

Console.WriteLine(html); 
// Output: <h1>Hello, World!</h1>
```

---

## ⚙️ Core Concepts

### 1. The Document
The `HtmlDocument` is the root container for your HTML. You add elements to it and call `.Build()` when you're ready to generate the final string.

### 2. Nodes
Everything you can add to the document is a "node" (e.g., `Text`, `Table`, `Image`). All nodes are built from the `IHtmlNode` interface.

### 3. Building
Calling the `.Build()` method on an `HtmlDocument` instance traverses the entire tree of nodes, resolves all styling and attributes, and renders the final, complete HTML string. All styles are automatically inlined into `style` attributes for maximum email client compatibility.

---

## 🧱 Basic Elements

### Text (Headings & Paragraphs)
Use the `Text` node to create headings and paragraphs. The default is `<p>`.

```csharp
var doc = new HtmlDocument()
    .Add(new Text("This is a heading.", asTag: "h1"))
    .Add(new Text("This is a standard paragraph of text."));

// Output:
// <h1>This is a heading.</h1><p>This is a standard paragraph of text.</p>
```

### Inline Formatting: Bold & Italics
You can easily create bold and italicized text. For use inside a `Text` node, pass them into the constructor.

```csharp
var doc = new HtmlDocument()
    .Add(new Text(
        new RawText("This text includes "), // Use RawText for plain text segments
        new Bold("bold text"),
        new RawText(" and "),
        new Italics("italic text.")
    ));

// Output:
// <p>This text includes <strong>bold text</strong> and <em>italic text.</em></p>
```

### Links
Create hyperlinks using the `Link` node.

```csharp
var doc = new HtmlDocument()
    .Add(new Text(
        new RawText("Please "),
        new Link("https://example.com", "click here"),
        new RawText(" to continue.")
    ));

// Output:
// <p>Please <a href="https://example.com">click here</a> to continue.</p>
```

### Images
Create self-closing `<img>` tags with the `Image` node. The `src` is required.

```csharp
var doc = new HtmlDocument()
    .Add(new Image("https://via.placeholder.com/150", alt: "A placeholder image"));

// Output:
// <img src="https://via.placeholder.com/150" alt="A placeholder image" />
```

---

## 🍽️ Complex Elements

### Lists
Create ordered (`<ol>`) or unordered (`<ul>`) lists.

```csharp
var doc = new HtmlDocument()
    .Add(new Text("Unordered List:", asTag: "h3"))
    .Add(new List() // Defaults to unordered
        .AddItem("First item")
        .AddItem("Second item")
        .AddItem(new Link("https://example.com", "A link item"))
    )
    .Add(new Text("Ordered List:", asTag: "h3"))
    .Add(new List(ordered: true)
        .AddItem("First step")
        .AddItem("Second step")
    );

// Output:
// <h3>Unordered List:</h3>
// <ul><li>First item</li><li>Second item</li><li><a href="https://example.com">A link item</a></li></ul>
// <h3>Ordered List:</h3>
// <ol><li>First step</li><li>Second step</li></ol>
```

### Tables
Construct tables fluently with `Table`, `TableRow`, and `TableCell`.

```csharp
var doc = new HtmlDocument()
    .Add(new Table()
        .AddRow(
            new TableCell("ID", isHeader: true),
            new TableCell("Name", isHeader: true),
            new TableCell("Status", isHeader: true)
        )
        .AddRow(
            new TableCell("1"),
            new TableCell("John Doe"),
            new TableCell(new Bold("Active"))
        )
        .AddRow(
            new TableCell("2"),
            new TableCell("Jane Smith"),
            new TableCell(new Italics("Inactive"))
        )
    );

// Output:
// <table>
//   <tr><th>ID</th><th>Name</th><th>Status</th></tr>
//   <tr><td>1</td><td>John Doe</td><td><strong>Active</strong></td></tr>
//   <tr><td>2</td><td>Jane Smith</td><td><em>Inactive</em></td></tr>
// </table>
```
**Note:** Per the MVP scope, complex nesting is disallowed (e.g., a `List` inside a `TableCell`).

---

## ✨ Styling & Theming

### Method 1: Local Overrides (The Simple Way)
You can apply styles and attributes directly to any node using a fluent interface. This is perfect for one-off modifications.

- `.Style("key", "value")`: Adds a single CSS style.
- `.Attr("key", "value")`: Adds a single HTML attribute.
- `.Class("name")`: Adds a CSS class name (used for theming).

```csharp
var doc = new HtmlDocument()
    .Add(new Link("https://example.com", "Click Me!")
        .Style<Link>("color", "white")
        .Style<Link>("text-decoration", "none")
        .Style<Link>("background-color", "blue")
        .Style<Link>("padding", "10px 15px")
        .Attr<Link>("target", "_blank")
        .Class<Link>("button") // Add a class for theme matching
    );

// Output (style order may vary):
// <a href="https://example.com" target="_blank" class="button" style="color:white;text-decoration:none;background-color:blue;padding:10px 15px">Click Me!</a>
```

### Method 2: Theming (The Powerful, DRY Way)
For consistent styling, the theming system is the recommended approach. You define styles once in a `Theme` object and it's applied automatically.

**Style Precedence Rule:** Local styles and attributes always **override** theme styles.

#### Full Theming Example:
```csharp
// 1. Define your Theme
var myTheme = new Theme();

// Add a style for all <h1> tags
myTheme.AddStyle("h1", new ElementStyle()
    .SetStyle("font-family", "Arial, sans-serif")
    .SetStyle("color", "#333333"));

// Add a style for all <table> tags
myTheme.AddStyle("table", new ElementStyle()
    .SetAttribute("cellspacing", "0")
    .SetAttribute("cellpadding", "5")
    .SetAttribute("width", "100%")
    .SetStyle("border-collapse", "collapse"));

// Add a reusable class style for ".header"
myTheme.AddStyle(".header", new ElementStyle()
    .SetStyle("background-color", "#f2f2f2")
    .SetStyle("font-weight", "bold"));

// 2. Create a document WITH the theme
var doc = new HtmlDocument(myTheme);

// 3. Create elements as usual
doc.Add(new Text("Themed Invoice", asTag: "h1")); // Will automatically get h1 styles

doc.Add(new Table()
    // This row gets the ".header" class styles
    .AddRow(
        new TableCell("Item").Class<TableCell>("header"),
        new TableCell("Price").Class<TableCell>("header")
    )
    .AddRow(
        new TableCell("Laptop"),
        // This specific cell gets a local override
        new TableCell("$1200").Style<TableCell>("font-weight", "bold")
    )
);

string html = doc.Build();
Console.WriteLine(html);
```

**Output of Theming Example:**
```html
<h1 style="font-family:Arial, sans-serif;color:#333333">Themed Invoice</h1>
<table cellspacing="0" cellpadding="5" width="100%" style="border-collapse:collapse">
  <tr>
    <th class="header" style="background-color:#f2f2f2;font-weight:bold">Item</th>
    <th class="header" style="background-color:#f2f2f2;font-weight:bold">Price</th>
  </tr>
  <tr>
    <td>Laptop</td>
    <td style="font-weight:bold">$1200</td>
  </tr>
</table>
```

---

## ⚠️ Validation & Error Handling

The library follows a **"fail-fast"** principle. If you try to build an invalid node, the `.Build()` method will throw an `InvalidOperationException` with a clear error message. This prevents you from ever generating broken or incomplete HTML.

```csharp
// This will throw an exception during .Build()
var brokenImage = new Image(""); // src is empty
var brokenLink = new Link("", "text"); // href is empty
```
