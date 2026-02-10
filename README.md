# Fluent HTML Builder (Jattac.Libs.HtmlBuilder v1.0.0)

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
// Ensure you have a 'using Jattac.Libs.HtmlBuilder;' statement
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
// Ensure you have a 'using Jattac.Libs.HtmlBuilder;' statement
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
    
    // Use the generic method for any tag (e.g., div, span)
    doc.Text("This is a generic div block.", asTag: "div");
    doc.Text("This is an inline span block.", asTag: "span");
});
```

### Advanced Text Formatting & Links
For text with mixed formatting (bold, italics, links), use the `Action` overload to get a `TextContentBuilder`. This allows you to compose inline elements fluently.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Paragraph(p =>
    {
        p.Raw("This text is normal. ")
         .Bold(b => {
             b.Raw("This part is bold, but ");
             b.Italic(i => i.Raw("this part is bold and italic.")); // Nested inline
         })
         .Raw(" Now back to normal, and here is a ")
         .Link("https://example.com/more-info", linkContent => 
         {
             linkContent.Raw("link to ");
             linkContent.Bold("more information"); // Bold text inside a link
         })
         .Attr("target", "_blank"); // Add an attribute to the link
    });
});

// Output (attributes/styles on link will be nested under <a>)
// <p>This text is normal. <strong>This part is bold, but <em>this part is bold and italic.</em></strong> Now back to normal, and here is a <a href="https://example.com/more-info" target="_blank">link to <strong>more information</strong></a></p>
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

## ✨ Styling & Theming: A Comprehensive Walkthrough

This library emphasizes **DRY** (Don't Repeat Yourself) principles through its powerful theming system. All styles are ultimately inlined into the HTML for maximum email client compatibility.

### 1. Defining a Theme
A `Theme` object is a collection of `ElementStyle` definitions. `ElementStyle` allows you to define both CSS styles and HTML attributes for a given selector.

```csharp
// Ensure you have a 'using Jattac.Libs.HtmlBuilder;' statement
var myTheme = new Theme();

// --- Tag-Based Styling ---
// Styles all <h1> tags
myTheme.AddStyle("h1", new ElementStyle()
    .SetStyle("font-family", "Arial, sans-serif")
    .SetStyle("color", "#333333")
    .SetStyle("margin-bottom", "10px")
);

// Styles all <p> tags
myTheme.AddStyle("p", new ElementStyle()
    .SetStyle("font-family", "Verdana, sans-serif")
    .SetStyle("font-size", "14px")
    .SetStyle("line-height", "1.5")
);

// Styles all <table> tags and sets attributes
myTheme.AddStyle("table", new ElementStyle()
    .SetAttribute("width", "100%")
    .SetAttribute("cellspacing", "0")
    .SetAttribute("cellpadding", "5")
    .SetStyle("border-collapse", "collapse")
    .SetStyle("margin-top", "15px")
);

// Styles all <th> (table header) tags
myTheme.AddStyle("th", new ElementStyle()
    .SetStyle("background-color", "#f2f2f2")
    .SetStyle("color", "#333333")
    .SetStyle("font-weight", "bold")
    .SetStyle("padding", "8px")
    .SetStyle("border", "1px solid #ddd")
    .SetStyle("text-align", "left")
);

// Styles all <td> (table data) tags
myTheme.AddStyle("td", new ElementStyle()
    .SetStyle("padding", "8px")
    .SetStyle("border", "1px solid #ddd")
    .SetStyle("text-align", "left")
);

// --- Class-Based Styling ---
// Styles elements with the class "button" (selector must start with '.')
myTheme.AddStyle(".button", new ElementStyle()
    .SetStyle("display", "inline-block")
    .SetStyle("padding", "10px 20px")
    .SetStyle("background-color", "#007bff")
    .SetStyle("color", "white")
    .SetStyle("text-decoration", "none")
    .SetStyle("border-radius", "5px")
);

// Styles elements with the class "highlight"
myTheme.AddStyle(".highlight", new ElementStyle()
    .SetStyle("background-color", "yellow")
    .SetStyle("font-weight", "bold")
);
```

### 2. Applying a Theme to Your Document
Pass your `Theme` object to the `HtmlDocument` constructor. All elements added to this document will automatically inherit the theme's definitions.

```csharp
var doc = new HtmlDocument(myTheme, docBuilder =>
{
    docBuilder.Heading1("Welcome to Our Newsletter"); // Will be styled by h1 theme
    docBuilder.Paragraph("This is the main content of your email."); // Will be styled by p theme
});
```

### 3. Applying Classes to Elements
Use the `.Class("name")` method on any element builder to apply class-based theme styles.

```csharp
var doc = new HtmlDocument(myTheme, docBuilder =>
{
    docBuilder.Paragraph(p =>
    {
        p.Raw("Click here to ")
         .Link("https://example.com/subscribe", "Subscribe Now")
         .Class("button"); // This link will get ".button" theme styles
    });
});
```

### 4. Local Style Overrides
You can always apply one-off styles or attributes directly to an element using `.Style("key", "value")` and `.Attr("key", "value")`. These local definitions **always take precedence** over any conflicting theme styles or attributes.

```csharp
var doc = new HtmlDocument(myTheme, docBuilder =>
{
    docBuilder.Heading1("Important Announcement") // Gets default h1 theme style
              .Style("color", "red"); // Local style overrides theme 'color'
              
    docBuilder.Paragraph("This is a special notice.")
              .Class("highlight") // Gets ".highlight" theme styles
              .Style("background-color", "lightgreen"); // Local style overrides theme 'background-color'
});
```

### 5. Attribute Merging Logic
*   For the `class` attribute, local and theme values are **appended** (e.g., theme `class="a"` + local `.Class("b")` results in `class="a b"`).
*   For all other attributes (e.g., `width`, `target`), a locally-set value will **override** any theme-provided value.

### 6. Comprehensive Theming Walkthrough Example

```csharp
// 1. Define the Theme (as shown above)
// ... myTheme definition ...

// 2. Create the Document with the Theme
var doc = new HtmlDocument(myTheme, docBuilder =>
{
    docBuilder.Heading1("Order Confirmation"); // Uses h1 theme style

    docBuilder.Paragraph(p => 
    {
        p.Raw("Dear Customer, thank you for your order. Your order #")
         .Bold("XYZ123")
         .Raw(" has been placed successfully. You can view your order details ")
         .Link("https://example.com/order/xyz123", "here")
         .Attr("target", "_blank") // Local attribute on link
         .Raw(".");
    });

    docBuilder.Table(table =>
    {
        // Table and th/td cells will use their respective theme styles
        table.Header(row =>
        {
            row.HeaderCell("Item Description");
            row.HeaderCell("Qty");
            row.HeaderCell("Price");
            row.HeaderCell("Total");
        });
        table.Row(row =>
        {
            row.Cell("Wireless Mouse");
            row.Cell("1");
            row.Cell("$25.00");
            row.Cell("$25.00").Style("font-weight", "bold"); // Local style override
        });
        table.Row(row =>
        {
            row.Cell("Mechanical Keyboard");
            row.Cell("1");
            row.Cell("$75.00");
            row.Cell("$75.00");
        });
        table.Row(row =>
        {
            row.Cell("Subtotal", colSpan: 3).Style("text-align", "right"); // Span and local style
            row.Cell("$100.00");
        });
    });

    docBuilder.Paragraph(p =>
    {
        p.Raw("Need help? Contact our support team ")
         .Link("mailto:support@example.com", "support@example.com")
         .Raw(".");
    }).Class("highlight"); // Paragraph with a themed class
});

Console.WriteLine(doc.Build());
```

---

## ⚠️ Validation & Error Handling

The library uses a **"fail-fast"** principle. If you try to build an invalid node (e.g., an `Image` without a `src`), the `.Build()` method will throw an `InvalidOperationException` with a clear error message, preventing broken HTML from being generated.
For tables, automatic column count validation is applied to ensure all rows match the header (or first data row) count. However, this validation is **skipped if any cell in the table uses `rowSpan` or `colSpan` attributes**, placing the responsibility for structural correctness on the developer.