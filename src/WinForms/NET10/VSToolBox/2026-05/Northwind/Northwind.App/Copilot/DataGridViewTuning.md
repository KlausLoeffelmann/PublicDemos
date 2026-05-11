# Custom DoubleBuffered, DarkMode-Aware DataGridView

Create a derived `DataGridView` class named `ThemedDataGridView` in the WinForms App project.

### Requirements

- **Double buffering** enabled via the protected `DoubleBuffered` property in the constructor (or by setting `ControlStyles.OptimizedDoubleBuffer`).
- **Dark mode detection** using `Application.IsDarkModeEnabled`.
- On creation **and** whenever the theme changes, apply the appropriate palette:
  - **Light mode**: use system defaults (do not override).
  - **Dark mode**: explicitly set `ColumnHeadersDefaultCellStyle`, `DefaultCellStyle`, `RowsDefaultCellStyle`, `AlternatingRowsDefaultCellStyle`, `RowHeadersDefaultCellStyle`, `BackgroundColor`, and `GridColor` to a dark palette with readable foreground colors.
  - Also set `EnableHeadersVisualStyles = false` in dark mode so header colors actually take effect.

### Theme-change hook

Re-apply the palette when the system theme changes at runtime — handle `SystemEvents.UserPreferenceChanged` (category `General`) and re-evaluate `Application.IsDarkModeEnabled`. Detach the handler in `Dispose(bool)`.

### Suggested Dark Palette

- Background: `Color.FromArgb(32, 32, 32)`
- Cells: `Color.FromArgb(45, 45, 48)` / Foreground `Color.WhiteSmoke`
- Alternating rows: `Color.FromArgb(55, 55, 58)`
- Headers: `Color.FromArgb(28, 28, 28)` / Foreground `Color.Gainsboro`
- Grid lines: `Color.FromArgb(70, 70, 70)`
- Selection: `Color.FromArgb(0, 120, 215)` / Foreground `Color.White`

Use this `ThemedDataGridView` in place of the default `DataGridView` in the main form.
