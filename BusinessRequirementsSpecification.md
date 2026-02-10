# Business Requirements Specification (BRS): Fluent Email HTML Builder (Version 2.1)

## 1. Vision & Purpose
To provide C# developers with a **dream developer experience (DX)** for programmatically building responsive, email-client-compatible HTML. The library will eliminate boilerplate and the fragility of string manipulation, enabling the rapid and reliable creation of common email content structures through a fluent, expression-based API.

## 2. Guiding Principles
*   **Fluent & Discoverable:** An intuitive, chainable API that developers can explore through IDE IntelliSense.
*   **Near-Zero Boilerplate:** Common tasks should require the absolute minimum amount of code.
*   **DRY (Don't Repeat Yourself) via Theming:** A robust theming system will be the primary mechanism for reusing styles and attributes, ensuring consistency and easy maintenance.
*   **Self-Documenting:** The C# code itself should be readable enough to clearly describe the resulting HTML structure.

## 3. MVP Scope

### 3.1 In-Scope Elements:
The MVP will focus on the following foundational elements:
*   **Text:** Paragraphs, headings, and other block-level text elements (`<div>`, `<span>`, etc.). Includes inline formatting like `<strong>` and `<em>`.
*   **Links:** `<a>` tags with `href` attributes.
*   **Images:** `<img>` tags with `src` and `alt` attributes.
*   **Tables:** The complete structure of `<table>`, `<tr>`, `<th>`, and `<td>`.
*   **Lists:** Ordered (`<ol>`) and unordered (`<ul>`) lists with `<li>` items.

### 3.2 Explicitly Out-of-Scope for MVP:
*   **Complex Nesting:** Nesting structural elements is disallowed. For example, a `TableCell` can contain text, a link, or an image, but it cannot contain another `Table` or a `List`.
*   **Unit Testing:** The delivery of a unit test suite is not a requirement for the MVP.

## 4. Key Features & Capabilities

### 4.1 Advanced Theming System
*   **Theme Definition:** The system must allow for the creation of `Theme` objects where styles and attributes are mapped to element types (e.g., `h1`, `table`) or custom classifications (e.g., `.invoice-header`).
*   **Class-based Styling:** Developers must be able to apply a named theme classification to an element using a fluent `.Class("name")` method.
*   **Attribute Merging Logic:**
    *   For the `class` attribute, local and theme values will be **appended**.
    *   For all other attributes, a locally-set value will **override** the theme value.

### 4.2 Fluent HTML Builders
*   **Core Builders:** Provide dedicated, fluent builders for all in-scope elements. Builders must support both declarative (lambda-based) and imperative (standalone object) construction.
*   **Flexible Text Block Creation:** The API must provide a generic method to create any block-level text element (e.g., `p`, `h1`-`h6`, `div`) by specifying the tag name as a string. Convenience methods for common tags (`.Paragraph()`, `.Heading1()`) must also be available.
*   **Advanced Text Composition:** A dedicated `TextContentBuilder` must be provided to allow for the fluent composition of mixed inline content within a single text block. This includes mixing plain text, `<strong>`, `<em>`, and `<a>` elements, including nested combinations (e.g., bold and italic text).
*   **Local Style Overrides:** Developers must be able to define arbitrary, one-off CSS styles on any element instance using a `.Style("key", "value")` method.

## 5. Build Process & Validation
*   **Self-Validation & Fail-Fast:** Each node **must** validate itself during the build step. An invalid state must throw a descriptive exception.
*   **Style Resolution Logic:** The final inline style attribute must be computed by merging styles in the following order of precedence: Local Styles > Theme Styles.
*   **Mandatory Style Inlining:** The build process must generate a single HTML string where all CSS styles are fully resolved and inlined into the `style=""` attribute of each respective element.

## 6. Crucial Non-Functional Requirements
*   **Security:** All user-provided content must be aggressively HTML-encoded by default.
*   **Clear Error Handling:** The build process must provide clear, actionable error messages for any issues encountered.

## 7. Project Deliverables
*   **Documentation & Examples:** The final deliverable must include clear API documentation with practical usage examples.