# Build Prompt — WinForms Tax Demo (Perf-Test Harness)

## Purpose

A deliberately structured WinForms application used as a **performance / form-load test harness**.
The overview form is **not** the subject of the test — it is a cheap first form that guarantees
the process, JIT, and all assemblies are warm before the *next* form is measured. The form under
test is the **giant Einkommensteuererklärung editor** (a single, scrollable, GroupBox-dense form
with View-menu bookmarks).

Target: **.NET 9+ WinForms**, C# 13/14, NRTs enabled, PerMonitorV2 DPI. Double-buffered everywhere
it matters. Use the existing `tax-demo-data.json` (schema: `tax-demo-data.schema.json`) as the data
source — load it at startup into in-memory models.

> Build plain WinForms unless told otherwise. Keep it Designer-compatible.

---

## Data

- Deserialize `tax-demo-data.json` into models matching the schema. Money fields are `decimal`,
  dates are `DateOnly`. `Status` is an enum (`Offen`, `Beglichen`, `Gestundet`).
- Keep the dataset in a singleton repository the forms read from.

---

## Localization (DE / EN)

- All user-facing strings via a resource lookup (`.resx` per culture, neutral = EN, `de` = German),
  or a simple `ILocalizer` keyed dictionary if you want to avoid resx churn.
- A language switch (e.g. **View → Language → English / Deutsch**) re-applies strings to open forms
  live, or at minimum on next form open. Persist the choice.
- Data values (city names, status) stay as-is; only **UI chrome** (labels, menus, captions, column
  headers) is localized.

---

## Reusable components (build once, reuse)

### 1. `ThemedDataGridView : DataGridView`
- Double-buffered (set `DoubleBuffered = true` via protected ctor).
- Subtle alternating row colors.
- Two color schemes: **Classic** and **Dark**, switchable, both legible.
- Used in **at least three** places: tax-payer grid, declarations grid, and any list view.

### 2. `IconFactory`
- Produces **36×36** toolbar icons by rendering **Segoe Fluent Icons** glyphs (fall back to
  Segoe MDL2 Assets) to bitmaps at the current DPI. No bitmap resources.
- One method: `Image GetIcon(int glyph, int size, Color color)` (or a glyph enum).

---

## Forms

### A. Overview Form (`MainForm`) — *warm-up form, not the test*
- `SplitContainer`, **Orientation = Horizontal**.
- **Top panel:** `ThemedDataGridView` listing all tax payers. Columns: Tax Number, Title,
  Name(s), Birth Date/Place, current city, maiden name, mother, father — the more the merrier.
- **Bottom panel:** "Details at a glance" — **no nested grids**.
  - Header row: **Tax Number left**, **Name right**, 2pt larger + bold.
  - Left: key detail fields of the selected payer.
  - Right: a second `ThemedDataGridView` (sibling, not nested) listing that payer's
    **Einkommensteuererklärungen** — columns: Year, Bemessungsgrundlage, zu zahlende Steuer,
    Fälligkeit, ausstehender Betrag, Status (color-coded by status).
- **Single click** on a top row → both bottom regions update immediately (single source of truth).
- **Toolbar** (36×36 icons):
  - *Edit person* → opens **PersonForm** (modal).
  - *Open declaration* → opens the giant **DeclarationForm** (modeless) for the selected year.
- **Double click** on a declaration row → opens the giant **DeclarationForm** (modeless).

### B. `PersonForm` — **modal**
- Full personal data of the selected tax payer, editable. Save / Close.
- Only one instance at a time (hence modal).

### C. `DeclarationForm` — **modeless / free-floating** (THE FORM UNDER TEST)
- **One single, tall, scrollable form** (`AutoScroll = true` host panel) packed with **GroupBox
  sections** — one per Anlage. Make it genuinely dense (many child controls per section):
  - Mantelbogen / Stammdaten
  - Anlage N (nichtselbständige Arbeit)
  - Anlage KAP (Kapitalerträge)
  - Anlage V (Vermietung & Verpachtung)
  - Anlage G (Gewerbebetrieb)
  - Anlage S (selbständige Arbeit)
  - Anlage Vorsorgeaufwand
  - Anlage Kind
  - Anlage Sonderausgaben / außergewöhnliche Belastungen
  - (add more sections to increase density)
- **View menu = bookmarks:** one entry per section; selecting it scrolls that GroupBox into view
  (`ScrollControlIntoView` / set `AutoScrollPosition`).
- Several instances may be open at once (clerk opens multiple years/payers).
- **Own menu + toolbar:**
  - **File:** Export, Close
  - **Edit:** *Edit Tax Form* (toggles the form from read-only → editable),
    Save changes, Save and Close, Close without saving.
- Form opens **read-only**; controls only become editable after *Edit Tax Form*.

---

## Conventions

- Globally imported namespaces assumed; no `using` directives needed in answers.
- `var` only for long/obvious types; primitive types always spelled out.
- Blank line before `return` and after a new block.
- Pattern matching / `switch` expressions / `is`/`and`/`or` preferred.
- Collection initializers (`List<string> x = [];`).
- Expression-bodied members for one-liners, formatted with the `=>` on its own line.
- XML doc comments on public members.

---

## Acceptance

1. App starts, `MainForm` loads the 20 payers from JSON.
2. Clicking a payer updates both bottom regions with zero nesting.
3. *Edit person* opens the modal `PersonForm`; double-click a declaration opens a modeless
   `DeclarationForm`; multiple `DeclarationForm`s can coexist.
4. `DeclarationForm` is one giant scrollable GroupBox form; the View menu jumps to sections.
5. Read-only until *Edit Tax Form*; Save/Save&Close/Close-without-saving behave correctly.
6. Language switch flips all UI chrome between EN and DE.
7. `ThemedDataGridView` and `IconFactory` are each reused in multiple places; dark + classic
   themes both legible.
