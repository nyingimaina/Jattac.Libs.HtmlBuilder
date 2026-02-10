# Fluent HTML Builder (v2.2)

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

## 🧱 Building Elements

### Generic & Basic Text
The most flexible way to create a text element is with the generic `.Text()` method. You can also use the convenient `.Paragraph()`, `.Heading1()`, etc. methods.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Heading1("This is a Heading 1");
    doc.Paragraph("This is a standard paragraph of text.");
    
    // Use the generic method for any tag
    doc.Text("This is a div.", asTag: "div");
    doc.Text("This is a span.", asTag: "span");
});
```

### Advanced Text Formatting
For text with mixed formatting (bold, italics, links), use the `Action` overload to get a `TextContentBuilder`. This allows you to compose inline elements fluently.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Paragraph(p =>
    {
        p.Raw("This text is normal. ")
         .Bold(b => {
             b.Raw("This part is bold, but ");
             b.Italic(i => i.Raw("this part is bold and italic."));
         })
         .Raw(" Now back to normal, and here is a ")
         .Link("https://example.com", l => l.Raw("link!"));
    });
});

// Output:
// <p>This text is normal. <strong>This part is bold, but <em>this part is bold and italic.</em></strong> Now back to normal, and here is a <a href="https://example.com">link!</a></p>
```

### Images
Create self-closing `<img>` tags.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Image("https://via.placeholder.com/150", alt: "A placeholder image");
});
```

### Lists
Create ordered (`<ol>`) or unordered (`<ul>`) lists.

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
Construct tables fluently with `Table`, `Header` (for `<thead>`), and `Row` (for `<tbody>`).

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Table(table =>
    {
        // Define your header row
        table.Header(row =>
        {
            row.HeaderCell("Product ID");
            row.HeaderCell("Product Name");
            row.HeaderCell("Price");
        });
        
        // Add data rows
        table.Row(row =>
        {
            row.Cell("P101");
            row.Cell("Laptop");
            row.Cell("$1200");
        });
        table.Row(row =>
        {
            row.Cell("P102");
            row.Cell("Mouse");
            row.Cell("$25");
        });
    });
});
```

#### Tables with Rowspan and Colspan
You can specify `rowSpan` and `colSpan` for individual cells.

**Important Note on Validation:** If any cell within a table uses `rowSpan` or `colSpan`, the library's automatic column count validation is **skipped**. It becomes the developer's responsibility to ensure the structural correctness of the table when using these advanced spanning attributes.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Table(table =>
    {
        table.Header(row =>
        {
            row.HeaderCell("Details", colSpan: 2); // Header spans 2 columns
            row.HeaderCell("Status");
        });
        table.Row(row =>
        {
            row.Cell("Item A");
            row.Cell("Value 1");
            row.Cell("Active");
        });
        table.Row(row =>
        {
            row.Cell("Summary", rowSpan: 2); // This cell spans 2 rows
            row.Cell("Item B");
            row.Cell("Pending");
        });
        table.Row(row =>
        {
            // This row effectively starts under the "Summary" cell
            row.Cell("Item C");
            row.Cell("Completed");
        });
    });
});
```

---

## ✨ Styling & Theming

Apply styles and attributes to any element using the fluent methods on its builder.

- `.Style("key", "value")`: Adds a single CSS style.
- `.Attr("key", "value")`: Adds a single HTML attribute.
- `.Class("name")`: Adds a CSS class name (for theme matching).

```csharp
// This example uses the new TextContentBuilder
var doc = new HtmlDocument(doc => 
{
    doc.Paragraph(p => 
    {
        p.Raw("This is a ")
         .Bold("bold statement")
         .Style("color", "red") // Applies style to the <strong> tag
         .Attr("data-id", "123");
    });
});

// Output:
// <p>This is a <strong style="color:red" data-id="123">bold statement</strong></p>
```
**Note:** You can't chain fluent style methods directly after `Raw()`. They must be chained after an element-creating method like `Bold()`, `Italic()`, or `Link()`.

---

## ⚠️ Validation & Error Handling

The library uses a **"fail-fast"** principle. If you try to build an invalid node (e.g., an `Image` without a `src`), the `.Build()` method will throw an `InvalidOperationException` with a clear error message, preventing broken HTML from being generated.
For tables, automatic column count validation is applied to ensure all rows match the header (or first data row) count. However, this validation is **skipped if any cell in the table uses `rowSpan` or `colSpan` attributes**, placing the responsibility for structural correctness on the developer.