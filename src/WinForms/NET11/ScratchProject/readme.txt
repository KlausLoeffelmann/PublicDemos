This project is an exploratory test bed for the .NET 11 WinForms API surface tracked in
https://github.com/dotnet/winforms/issues/14694 (VisualStylesMode, Command/CommandParameter,
KioskModeManager, Application.SystemTextSize, TreeView.NodeLeading, etc.).

Each scenario lives in its own UserControl under Views\, implementing IScenarioView, and is
registered once in MainForm.CreateViews(). The View menu switches which scenario is shown in
Panel1 of the SplitContainer; Panel2 always hosts the same PropertyGrid, driven by whichever
controls are checked (via CheckBoxes) in the active view. Use Edit > Select All / Reset Selection
to bulk (de)select controls for the PropertyGrid.

Keep individual scenario views focused and simple - the host shell (MainForm) is the only place
that should grow "framework" logic.