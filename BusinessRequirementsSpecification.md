# Business Requirements Specification (BRS): Fluent Email HTML Builder (Version 2.3)

## 1. Vision & Purpose
To provide C# developers with a **dream developer experience (DX)** for programmatically building responsive, email-client-compatible HTML. The library will eliminate boilerplate and the fragility of string manipulation, enabling the rapid and reliable creation of common email content structures through a fluent, expression-based API.

## 2. Guiding Principles
*   **Fluent & Discoverable:** An intuitive, chainable API that developers can explore through IDE IntelliSense.
*   **Readable & Self-Documenting:** Your C# code visually mirrors the HTML structure it creates.
*   **DRY (Don't Repeat Yourself):** A powerful theming system will be the primary mechanism for reusing styles and attributes, ensuring consistency and easy maintenance.
*   **Flexible:** Supports both a declarative, lambda-based syntax for clean structure and an imperative, object-based syntax for dynamic, data-driven content.
*   **Safe & Robust:** All content is HTML-encoded by default, and a fail-fast validation system ensures you never generate broken HTML.

## 3. MVP Scope

### 3.1 In-Scope Elements:
The MVP will focus on the following foundational elements:
*   **Text:** Paragraphs, headings, and other block-level text elements (`<div>`, `<span>`, etc.). Includes inline formatting like `<strong>` and `<em>`.
*   **Links:** `<a>` tags with `href` attributes.
*   **Images:** `<img>` tags with `src` and `alt` attributes.
*   **Tables:** The complete structure of `<table>`, `<tr>`, `<th>`, and `<td>`, including support for `rowspan` and `colspan` attributes.
*   **Lists:** Ordered (`<ol>`) and unordered (`<ul>`) lists with `<li>` items.

### 3.2 Explicitly Out-of-Scope for MVP:
*   **Complex Nesting:** Nesting structural elements is disallowed. For example, a `TableCell` can contain text, a link, or an image, but it cannot contain another `Table` or a `List`.
*   **Unit Testing:** The delivery of a unit test suite is not a requirement for the MVP.

## 4. Key Features & Capabilities

### 4.1 Advanced Theming System
*   **Theme Definition:** The system must allow for the creation of `Theme` objects where styles and attributes are mapped to element types (e.g., `h1`, `table`) or custom classifications (e.g., `.invoice-header`).
*   **Detailed ElementStyle:** `ElementStyle` definitions will support setting both CSS styles and HTML attributes.
*   **Class-based Styling:** Developers must be able to apply a named theme classification to an element using a fluent `.Class("name")` method. The selector for classes must start with a `.` (e.g., `.button`).
*   **Attribute Merging Logic:**
    *   For the `class` attribute, local and theme values will be **appended**.
    *   For all other attributes, a locally-set value will **override** the theme value.

### 4.2 Fluent HTML Builders
*   **Core Builders:** Provide dedicated, fluent builders for all in-scope elements. Builders must support both declarative (lambda-based) and imperative (standalone object) construction.
*   **Flexible Text Block Creation:** The API must provide a generic method to create any block-level text element (e.g., `p`, `h1`-`h6`, `div`, `span`) by specifying the tag name as a string (`asTag` parameter). Convenience methods for common tags (`.Paragraph()`, `.Heading1()`) must also be available.
*   **Advanced Text Composition:** A dedicated `TextContentBuilder` must be provided to allow for the fluent composition of mixed inline content within a single text block. This includes mixing plain text (`Raw`), `<strong>` (`Bold`), `<em>` (`Italic`), and `<a>` (`Link`) elements, including nested combinations (e.g., bold and italic text, bold text inside a link).
*   **Table Enhancements:** The `TableBuilder` will provide a `Header()` method for explicit header row definition. `RowBuilder`'s `Cell()` and `HeaderCell()` methods will support optional `rowspan` and `colspan` attributes.
*   **Local Style Overrides:** Developers must be able to define arbitrary, one-off CSS styles (`.Style("key", "value")`) and HTML attributes (`.Attr("key", "value")`) on any element builder. These local definitions will always take precedence over any conflicting theme styles or attributes.

## 5. Build Process & Validation
*   **Self-Validation & Fail-Fast:** Each node **must** validate itself during the build step. An invalid state must throw a descriptive exception.
*   **Table Structure Validation:** The library will perform column count validation for tables. If a table contains any cells with `rowspan` or `colspan` attributes, this automatic column count validation will be **skipped**, and the developer assumes responsibility for ensuring the structural correctness of the table.
*   **Style Resolution Logic:** The final inline style attribute must be computed by merging styles in the following order of precedence: Local Styles > Theme Styles.
*   **Mandatory Style Inlining:** The build process must generate a single HTML string where all CSS styles are fully resolved and inlined into the `style=""` attribute of each respective element.

## 6. Crucial Non-Functional Requirements
*   **Security:** All user-provided content must be aggressively HTML-encoded by default.
*   **Clear Error Handling:** The build process must provide clear, actionable error messages for any issues encountered.

## 7. Project Deliverables
*   **Documentation & Examples:** The final deliverable must include clear API documentation with practical usage examples, especially for setting up and using the Theming system and advanced text composition.