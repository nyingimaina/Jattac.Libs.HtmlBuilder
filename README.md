# Fluent HTML Builder (v2)

A C# library for programmatically building responsive, email-client-compatible HTML with a focus on a dream developer experience (DX). This library uses a fluent, expression-based API to create readable, maintainable, and boilerplate-free HTML structures.

- **Fluent & Discoverable:** A chainable, context-aware API that guides you via IntelliSense.
- **Readable & Self-Documenting:** Your C# code visually mirrors the HTML structure it creates.
- **DRY (Don't Repeat Yourself):** A powerful theming system allows you to define styles and attributes once and reuse them everywhere.
- **Flexible:** Supports both a declarative, lambda-based syntax for clean structure and an imperative, object-based syntax for dynamic, data-driven content.
- **Safe & Robust:** All content is HTML-encoded by default, and a fail-fast validation system ensures you never generate broken HTML.

---

## 🚀 Two Ways to Build: Declarative vs. Imperative

This library supports two powerful design patterns.

**1. Declarative Style (Recommended for most uses)**
Use a clean, lambda-based syntax to define the structure of your document. This is highly readable and great for static or semi-static content.

```csharp
var doc = new HtmlDocument(myTheme, doc =>
{
    doc.Heading1("Welcome!")
       .Paragraph("This document was built declaratively.");
});

Console.WriteLine(doc.Build());
```

**2. Imperative Style (For dynamic content)**
Instantiate builders directly to generate content in loops or complex conditional logic. This gives you maximum control and flexibility.

```csharp
// Standalone builder usage
var listBuilder = new ListBuilder(myTheme);
foreach (var item in myData)
{
    listBuilder.Item(item.Name);
}

// Add the finished node to the document
var doc = new HtmlDocument(myTheme);
doc.Add(listBuilder.GetNode());

Console.WriteLine(doc.Build());
```

---

## ⚙️ Declarative Usage Examples

### Basic Elements

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Heading1("This is a Heading 1");
    doc.Heading2("This is a Heading 2");
    doc.Paragraph("This is a standard paragraph of text.");
    doc.Image("https://via.placeholder.com/150", alt: "A placeholder image");
});
```

### Lists
The `List` builder creates `<ul>` lists, and `OrderedList` creates `<ol>` lists.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.List(list => 
    {
        list.Item("First item")
            .Item("Second item");
    });
    
    doc.OrderedList(list => 
    {
        list.Item("Step 1")
            .Item("Step 2");
    });
});
```

### Tables
Chain `Table`, `Row`, `HeaderCell`, and `Cell` to build tables fluently.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Table(table =>
    {
        table.Row(row =>
        {
            row.HeaderCell("ID");
            row.HeaderCell("Name");
        });
        table.Row(row =>
        {
            row.Cell("1");
            row.Cell("John Doe");
        });
    });
});
```

---

## ⚙️ Imperative Usage Example

The imperative style is perfect for when you need to generate HTML from a dynamic data source.

```csharp
// 1. Imagine you have some data
var products = new[]
{
    new { ID = "A1", Name = "Laptop", Price = 1200.00 },
    new { ID = "A2", Name = "Mouse", Price = 25.00 },
};

// 2. Instantiate a builder directly
var tableBuilder = new TableBuilder(myTheme); // Pass in a theme if needed

// 3. Build the header
tableBuilder.Row(row =>
{
    row.HeaderCell("ID");
    row.HeaderCell("Name");
    row.HeaderCell("Price");
});

// 4. Loop over your data to build the body
foreach (var p in products)
{
    tableBuilder.Row(row =>
    {
        row.Cell(p.ID);
        row.Cell(p.Name);
        row.Cell(p.Price.ToString("C")); // Format as currency
    });
}

// 5. Add the completed node to a document
var doc = new HtmlDocument(myTheme);
doc.Add(tableBuilder.GetNode()); // Get the final IHtmlNode from the builder

// The final document now contains your dynamic table
Console.WriteLine(doc.Build());
```

---

## ✨ Styling & Theming

Apply styles and attributes to any element using the fluent methods on its builder.

- `.Style("key", "value")`: Adds a single CSS style.
- `.Attr("key", "value")`: Adds a single HTML attribute.
- `.Class("name")`: Adds a CSS class name (for theme matching).

**Precedence Rule:** Local styles/attributes **always override** theme styles/attributes.

### Example: Combining Theming and Local Overrides

```csharp
// 1. Define a Theme
var myTheme = new Theme();
myTheme.AddStyle("h1", new ElementStyle().SetStyle("color", "navy"));
myTheme.AddStyle(".special-paragraph", new ElementStyle().SetStyle("font-style", "italic"));

// 2. Build the document using the theme
var doc = new HtmlDocument(myTheme, doc =>
{
    // This h1 will automatically be navy from the theme
    doc.Heading1("Themed Heading");

    // This paragraph gets a theme style via its class
    doc.Paragraph("This is a special paragraph from the theme.")
       .Class("special-paragraph");

    // This paragraph overrides the theme with a local style
    doc.Paragraph("This paragraph has a local style override.")
       .Class("special-paragraph") // It matches the theme...
       .Style("font-style", "normal") // ...but this local style wins!
       .Style("color", "red");
});

// Output:
// <h1 style="color:navy">Themed Heading</h1>
// <p class="special-paragraph" style="font-style:italic">This is a special paragraph from the theme.</p>
// <p class="special-paragraph" style="font-style:normal;color:red">This paragraph has a local style override.</p>
```

---

## ⚠️ Validation & Error Handling

The library uses a **"fail-fast"** principle. If you try to build an invalid node (e.g., an `Image` without a `src`), the `.Build()` method will throw an `InvalidOperationException` with a clear error message, preventing broken HTML from being generated.