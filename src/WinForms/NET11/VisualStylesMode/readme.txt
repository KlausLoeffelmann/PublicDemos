This project is an exploratory test bed for the .NET 11 WinForms API surface tracked in
https://github.com/dotnet/winforms/issues/14694 (VisualStylesMode, Command/CommandParameter,
KioskModeManager, Application.SystemTextSize, TreeView.NodeLeading, etc.).

Each scenario lives in its own UserControl under Views\, implementing IScenarioView, and is
registered once in MainForm.CreateViews(). The View menu switches which scenario is shown in
Panel1 of the SplitContainer; Panel2 always hosts the same PropertyGrid, driven by whichever
controls are selected in the active view. Selection is done by double-clicking a control (each
demo control is wrapped in a SelectablePanel that highlights when selected); Shift + double-click
selects a rectangular range across a TableLayoutPanel grid. Use Edit > Select All / Clear Selection
to bulk (de)select controls for the PropertyGrid.

Keep individual scenario views focused and simple - the host shell (MainForm) is the only place
that should grow "framework" logic.