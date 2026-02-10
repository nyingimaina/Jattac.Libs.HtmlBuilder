# Business Requirements Specification (BRS): Fluent Email HTML Builder (Version 2.0)

## 1. Vision & Purpose
To provide C# developers with a **dream developer experience (DX)** for programmatically building responsive, email-client-compatible HTML. The library will eliminate boilerplate and the fragility of string manipulation, enabling the rapid and reliable creation of common email content structures through a fluent, discoverable, and self-documenting API.

## 2. Guiding Principles
*   **Fluent & Discoverable:** An intuitive, chainable API that developers can explore through IDE IntelliSense.
*   **Near-Zero Boilerplate:** Common tasks should require the absolute minimum amount of code.
*   **DRY (Don't Repeat Yourself) via Theming:** A robust theming system will be the primary mechanism for reusing styles and attributes, ensuring consistency and easy maintenance.
*   **Self-Documenting:** The C# code itself should be readable enough to clearly describe the resulting HTML structure.

## 3. MVP Scope

### 3.1 In-Scope Elements:
The MVP will focus on the following foundational elements:
*   **Text:** Paragraphs, headings, `<strong>`, `<em>`.
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
    *   For the `class` attribute, local and theme values will be **appended** (e.g., theme `class="a"` + local `.Class("b")` results in `class="a b"`).
    *   For all other attributes, a locally-set value will **override** the theme value.

### 4.2 Fluent HTML Builders
*   **Core Builders:** Provide dedicated, fluent builders for all in-scope elements (`Text`, `Link`, `Image`, `Table`, `List`).
*   **Local Style Overrides:** Developers must be able to define arbitrary, one-off CSS styles on any element instance using a `.Style("key", "value")` method. These styles are not part of a theme.

## 5. Build Process & Validation

*   **Self-Validation & Fail-Fast:** During the build step, each node **must** validate itself. An invalid state (e.g., an `Image` node without a `src` attribute, or a `Link` without an `href`) must throw a descriptive exception. This ensures errors are caught early and never result in broken HTML.
*   **Style Resolution Logic:** For each element, the final inline style attribute must be computed by merging styles in the following order of precedence (higher number wins):
    1.  Styles from the applied `Theme` (for the element type and any applied classes).
    2.  Custom styles applied locally via the `.Style()` method.
*   **Mandatory Style Inlining:** The build process must generate a single HTML string where all CSS styles are fully resolved and inlined into the `style=""` attribute of each respective element, ensuring maximum email client compatibility.

## 6. Crucial Non-Functional Requirements

*   **Security:** All user-provided content (text, attribute values, URLs) must be aggressively HTML-encoded by default to prevent Cross-Site Scripting (XSS) vulnerabilities. Raw HTML insertion should be an explicit and clearly marked capability.
*   **Clear Error Handling:** In addition to self-validation, the build process must provide clear, actionable error messages for any issues encountered.

## 7. Project Deliverables

*   **Documentation & Examples:** The final deliverable must include clear API documentation with practical usage examples, especially for setting up and using the Theming system.
