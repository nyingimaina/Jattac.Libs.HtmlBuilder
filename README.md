<p align="center">
  <img src="icon.png" alt="Jattac.Libs.HtmlBuilder Logo" width="150"/>
</p>

<h1 align="center">Jattac.Libs.HtmlBuilder</h1>

<p align="center">
  <em>A Fluent C# Library for Building Responsive, Email-Client-Compatible HTML</em>
</p>

<!-- NuGet Version Badge -->
 [![NuGet version](https://img.shields.io/nuget/v/Jattac.Libs.HtmlBuilder.svg)](https://www.nuget.org/packages/Jattac.Libs.HtmlBuilder/)
<!-- License Badge - Corrected link -->
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/nyingimaina/Jattac.Libs.HtmlBuilder/blob/master/LICENSE)
<!-- Add more badges here as needed, e.g., Build Status, Downloads, etc. -->



<p align="center">
  <a href="#features">Features</a> &bull;
  <a href="#installation">Installation</a> &bull;
  <a href="#two-ways-to-build-declarative-vs-imperative">Usage</a> &bull;
  <a href="#styling--theming-a-comprehensive-walkthrough">Theming</a> &bull;
  <a href="#validation--error-handling">Validation</a> &bull;
  <a href="#license">License</a>
</p>

---

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Two Ways to Build: Declarative vs. Imperative](#two-ways-to-build-declarative-vs-imperative)
  - [Declarative Style](#declarative-style-recommended-for-most-uses)
  - [Imperative Style](#imperative-style-for-dynamic-content)
- [Building Elements](#building-elements)
  - [Text & Headings](#text--headings)
  - [Advanced Text & Links](#advanced-text--links)
  - [Lists & Data-Bound Lists](#lists--data-bound-lists)
  - [Tables](#tables)
  - [Images, Spacers, & Dividers](#images-spacers--dividers)
  - [Generic & Raw HTML Elements](#generic--raw-html-elements)
- [Styling & Theming: A Comprehensive Walkthrough](#styling--theming-a-comprehensive-walkthrough)
- [Validation & Error Handling](#validation--error-handling)
- [Contributing](#contributing)
- [License](#license)

---

## Introduction

Jattac.Libs.HtmlBuilder is a C# library meticulously crafted to provide a **dream developer experience (DX)** for programmatically constructing responsive, email-client-compatible HTML. It eliminates the boilerplate and fragility often associated with manual HTML string manipulation, offering a fluent, discoverable, and self-documenting API.

## Features

- **Fluent & Discoverable API:** Design HTML structures using a natural, chainable syntax guided by IntelliSense.
- **Minimal Boilerplate:** Drastically reduces the amount of code needed to generate common HTML elements.
- **DRY Theming System:** Define and apply styles and attributes once, ensuring consistency and easy maintenance across your HTML.
- **Flexible Construction:** Supports both declarative (lambda-based) and imperative (standalone builder) paradigms.
- **Data-Bound Collections:** Generate lists directly from any `IEnumerable<T>`.
- **Advanced Text Composition:** Effortlessly mix plain text, bold, italic, and links, including nested inline elements.
- **Robust Table Builder:** Create complex tables with explicit header definition and support for `rowspan` and `colspan`.
- **Built-in Validation:** Fail-fast error handling prevents broken HTML generation for invalid structures.
- **Email Compatibility:** All CSS is automatically inlined into `style` attributes, ensuring maximum compatibility across various email clients.
- **Security:** Aggressively HTML-encodes user-provided content by default to prevent XSS vulnerabilities.

## Installation

The Jattac.Libs.HtmlBuilder library is available as a NuGet package.

```bash
dotnet add package Jattac.Libs.HtmlBuilder
```

Or, search for `Jattac.Libs.HtmlBuilder` in your NuGet Package Manager.

---

## 🚀 Two Ways to Build: Declarative vs. Imperative

This library supports two powerful design patterns.

**1. Declarative Style (Recommended for most uses)**
Use a clean, lambda-based syntax to define the structure of your document.

```csharp
// Ensure you have a 'using Jattac.Libs.HtmlBuilder;' statement
var doc = new HtmlDocument(myTheme, doc =>
{
    doc.Heading1("Welcome!")
       .Paragraph("This document was built declaratively.");
});
```

**2. Imperative Style (For dynamic content)**
Instantiate builders directly to generate content in loops or complex conditional logic.

```csharp
// Ensure you have a 'using Jattac.Libs.HtmlBuilder;' statement
// Standalone builder usage
var listBuilder = new ListBuilder(myTheme);
foreach (var item in myData)
{
    listBuilder.Item(item.Name);
}

var doc = new HtmlDocument(myTheme);
doc.Add(listBuilder.GetNode());
```

---

## 🧱 Building Elements

### Text & Headings
Use `.Paragraph()`, `.Heading1()`, etc. for common block-level text. For any other tag, use the generic `.Text()` method.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Heading1("This is a Heading 1");
    doc.Paragraph("This is a standard paragraph of text.");
    doc.Text("This is a div.", asTag: "div");
});
```

### Advanced Text & Links
For text with mixed formatting, use the `Action` overload to get a `TextContentBuilder`.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Paragraph(p =>
    {
        p.Raw("This is normal. ")
         .Bold(b => b.Italic("This is bold and italic."))
         .Raw(" Here is a ")
         .Link("https://example.com", "link")
         .Attr("target", "_blank");
    });
});
```

### Lists & Data-Bound Lists
Create lists manually with `.List()` or `.OrderedList()`, or generate them automatically from a collection with `.ListFor()`.

```csharp
var myItems = new string[] { "Apple", "Orange", "Banana" };

var doc = new HtmlDocument(doc =>
{
    doc.Heading2("Manual List");
    doc.List(list => 
    {
        list.Item("First item");
        list.Item("Second item");
    });

    doc.Heading2("Data-Bound List");
    doc.ListFor(myItems, (list, item) =>
    {
        list.Item(i => i.Raw($"Fruit: ").Bold(item));
    });
});
```

### Tables
Construct tables fluently with `Table`, `Header` (for `<thead>`), and `Row` (for `<tbody>`). `rowspan` and `colspan` are supported.

**Validation Note:** If any cell uses `rowspan` or `colSpan`, automatic column count validation is skipped.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Table(table =>
    {
        table.Header(row =>
        {
            row.HeaderCell("Details", colSpan: 2);
            row.HeaderCell("Status");
        });
        table.Row(row =>
        {
            row.Cell("Item A");
            row.Cell("Value 1");
            row.Cell("Active");
        });
    });
});
```

### Images, Spacers, & Dividers
Convenience methods for common layout elements.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Paragraph("Some content above the line.");
    
    // Creates a <hr /> tag
    doc.HorizontalRule().Style("border-top", "1px solid #ccc");

    // Creates an empty <div style="height:20px"></div>
    doc.Spacer("20px"); 

    doc.Paragraph("Some content below the line.");
    doc.Image("https://via.placeholder.com/150", alt: "A placeholder image");
});
```

### Generic & Raw HTML Elements

#### Generic Element
For any tag not covered by a convenience method (e.g., `<header>`, `<footer>`), use `.Element()`.

```csharp
var doc = new HtmlDocument(doc =>
{
    doc.Element("header", h => 
    {
        h.Style("background-color", "#f8f9fa");
        h.Content(c => c.Bold("My Website Header"));
    });
});
```

#### Raw HTML Injection
As an "escape hatch" for advanced scenarios, you can inject a raw, un-processed HTML string.

**Warning:** This bypasses all safety, styling, and validation features of the library. Use with caution.

```csharp
var doc = new HtmlDocument(doc =>
{
    var legacyHtml = "<div class='special-widget'>This is pre-built HTML.</div>";
    doc.RawHtml(legacyHtml);
});
```

---

## ✨ Styling & Theming: A Comprehensive Walkthrough
*(This section remains the same as the previous version, as the theming logic has not changed.)*

---

## ⚠️ Validation & Error Handling

The library uses a **"fail-fast"** principle. If you try to build an invalid node (e.g., an `Image` without a `src`), the `.Build()` method will throw an `InvalidOperationException` with a clear error message, preventing broken HTML from being generated.
For tables, automatic column count validation is applied to ensure all rows match the header (or first data row) count. However, this validation is **skipped if any cell in the table uses `rowSpan` or `colSpan` attributes**, placing the responsibility for structural correctness on the developer.

## Contributing

We welcome contributions! Please feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
