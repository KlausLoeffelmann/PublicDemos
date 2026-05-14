# Main Form Layout & Behavior

Modify `Form1` in the WinForms App to implement a Northwind customer editor against the existing EFCore DataLayer.

### Layout (top-to-bottom)

1. **MenuStrip**
   - **File**: `Export as CSV...`, separator, `Quit`
   - **Edit**: `Add new Customer`, `Edit selected Customer`, separator, `Save changes`

2. **ToolStrip** (text buttons): `Add`, `Edit`, `Save changes`

3. **SplitContainer** (horizontal splitter)
   - **Panel1**: `DataGridView` bound to Northwind customers
   - **Panel2**: Detail view — Label/TextBox pairs for the selected customer's fields

4. **StatusStrip** (left-to-right):
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
| `Save changes` (button + menu) | In Add/Edit mode **and** dirty |
| `Export as CSV...` | More than 1 row selected, in View mode |
| `Select` (status bar) | In View mode |

### Dirty Tracking

Track changes to detail TextBoxes against the original entity values. `Save changes` only enables once an actual change exists.

### Save

`Save changes` commits the Add/Edit through the EFCore DataLayer, returns to View mode, refreshes the grid, and updates the "Last changed Customer" status label.

### Export

`Export as CSV...` exports the currently selected rows (the shown columns) to a CSV file via `SaveFileDialog`.
