# Main Form Layout & Behavior

Modify the Form in the WinForms App to implement a Northwind 
customer editor against the existing EFCore DataLayer.

### Layout (top-to-bottom)

1. **MenuStrip**
   Set the MenuStrip Font to 11pt Segoe UI. Menu items:
   - **File**: `Export as CSV...`, separator, `Quit`
   - **Edit**: `Add new Customer`, `Edit selected Customer`, `Cancel`, separator, `Save changes`

2. **ToolStrip**: `Add`, `Edit`, `Cancel`, `Save changes`.
   Please implement a simple Image-Factory class based on rendering the Segoe Fluent Icons,
   with which you can generate the required icons for the ToolStrip buttons 
   (e.g. `Add` = `AddRegular`, `Edit` = `EditRegular`, `...)
   dynamically at runtime, without needing to reference any image files.

   Make sure, the ToolStrip's' `ImageScalingSize` is set to 36x36, and that the rendered icons
   are taking a sufficient amount of padding into account when being generated. Make also sure,
   that the `ToolStripItemsImageScaling` property is also set to `SizeToFit`.
   And finally, we want the text of the ToolStripBotton beneath the Image.

3. **SplitContainer** (important: _horizontal_ splitter, 10pt Font)
   - **Panel1**: Top area: `DataGridView` bound to Northwind customers
   - **Panel2**: Bottom area: 
     Detail view
     * A bit of a padding to the splitter.
     * In bigger, bold letters {CustomerID - CompanyName}
     * Then: Label/TextBox pairs for the selected customer's fields

4. **StatusStrip** (left-to-right, also with 11pt Segoe UI font):
   - `Customers:` label
   - `{customerCount}` label
   - `Last changed Customer:` label
   - `{id} {name} {contact}` label
   - `Select` button — jumps to that customer in the grid
   - `{Date} {Time}` label (spring, right-aligned)

### Grid Behavior

- Columns shown: `CustomerId`, `CompanyName`, `ContactName`, `ContactTitle`
- Read-only (no direct cell editing)
- Full-row selection, multi-select enabled

### Edit Modes

Three states: **View**, **Add**, **Edit**.

- **View** (default): grid enabled, detail TextBoxes disabled, `Save changes` disabled.
- **Add / Edit**: triggered by ToolStrip button or Edit menu. Grid disabled, detail TextBoxes enabled, `Save changes` enabled *only when dirty*.

### Enable/Disable Rules

| Control | Enabled when |
|---|---|
| `Edit` (button + menu) | Exactly 1 row selected, in View mode |
| `Add` (button + menu) | In View mode |
| `Cancel` (button + menu) | In Add/Edit mode |
| `Save changes` (button + menu) | In Add/Edit mode **and** dirty |
| `Export as CSV...` | More than 1 row selected, in View mode |
| `Select` (status bar) | In View mode |

### Dirty Tracking

Track changes to detail TextBoxes against the original entity values. 
`Save changes` only enables once an actual change exists.

### Save

`Save changes` commits the Add/Edit through the EFCore DataLayer, returns to View mode, 
refreshes the grid, and updates the "Last changed Customer" status label.

### Export

`Export as CSV...` exports the currently selected rows (the shown columns) to a CSV file
via `SaveFileDialog`.

## Scope of work

Do not implement any other features from another Markdown prompt files
without explicit instruction. 

Focus solely on the above requirements for the Form, 
but take available agents and/or skills into account.
