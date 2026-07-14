This project is an exploratory test bed for the .NET 11 WinForms API surface tracked in
https://github.com/dotnet/winforms/issues/14694 (VisualStylesMode, Command/CommandParameter,
KioskModeManager, Application.SystemTextSize, TreeView.NodeLeading, etc.).

Each scenario lives in its own UserControl under Views\, implementing IScenarioView, and is
registered once in MainForm.CreateViews(). The View menu switches which scenario is shown in
Panel1 of the SplitContainer; Panel2 always hosts the same PropertyGrid.

Use Edit > Edit mode (Ctrl+E) to place a transparent, non-activating adorner over the unchanged
active view. Double-click the deepest visible control to select it; Ctrl+double-click adds or
removes a control from the current selection. Edit > Select All and Edit > Deselect All operate on
the visible hit-testable controls. Leaving Edit mode clears the selection and removes the adorner.
The icon-only ToolStrip mirrors Save, Load, Edit mode, Select All, and Deselect All.

The StatusStrip reports the current window DPI/display scaling, the .NET 11 accessibility text
scale, and the Windows accent color. SystemTextSizeChanged updates the menu, status, and active-view
fonts immediately; a five-second timer refreshes the accent color and its swatch.

The Cash Register view is a functional mechanical-register simulation built from standard WinForms
Button controls. Its six denomination columns add exact place values from $9,000 through $0.01;
twenty department keys register the current input, and the receipt panel records items, VOID audit
corrections, 8.25% tax, subtotal snapshots, and a locked final total. The next denomination after
Total starts a new sale.

The bottom of the View menu applies VisualStylesMode.Classic or VisualStylesMode.Net11 recursively
to every active scenario. Standard, Flat, Popup, and System apply only to the Cash Register buttons;
System intentionally uses authentic native rendering and may therefore ignore the register's
category colors.

Keep individual scenario views focused and simple - the host shell (MainForm) is the only place
that should grow "framework" logic.