This project is an exploratory test bed for the .NET 11 WinForms API surface tracked in
https://github.com/dotnet/winforms/issues/14694 (VisualStylesMode, Command/CommandParameter,
KioskModeManager, Application.SystemVisualSettings, TreeView.NodeLeading, etc.).

Each scenario lives in its own UserControl under Views\, implementing IScenarioView, and is
registered once in MainForm.CreateViews(). The View menu switches which scenario is shown in
Panel1 of the SplitContainer; Panel2 always hosts the same PropertyGrid.

Use Edit > Edit mode (Ctrl+E) to place a transparent, non-activating adorner over the unchanged
active view. Double-click the deepest visible control to select it; Ctrl+double-click adds or
removes a control from the current selection. Edit > Select All and Edit > Deselect All operate on
the visible hit-testable controls. Leaving Edit mode clears the selection and removes the adorner.
The icon-only ToolStrip mirrors Save, Load, Edit mode, Select All, and Deselect All.

The StatusStrip reports the current window DPI/display scaling, the .NET 11 accessibility text
scale, and the Windows accent color. SystemVisualSettingsChanged updates the menu, status, and active-view
fonts immediately; a five-second timer refreshes the accent color and its swatch.

The Cash Register view is a functional mechanical-register simulation built from standard WinForms
Button controls. Its six denomination columns add exact place values from $9,000 through $0.01;
eighteen department keys register the current input, and the receipt panel records items, VOID audit
corrections, 8.25% tax, subtotal snapshots, and a locked final total. The next denomination after
Total starts a new sale.

The bottom of the View menu applies VisualStylesMode.Classic or VisualStylesMode.Net11 recursively
to every active scenario. Standard, Flat, Popup, and System apply only to the Cash Register buttons;
System intentionally uses authentic native rendering and may therefore ignore the register's
category colors.

Keep individual scenario views focused and simple - the host shell (MainForm) is the only place
that should grow "framework" logic.

Customer-entry dates in both views use RegionalDateTextBox rather than DateTimePicker. Input is
parsed and normalized using the current regional short-date format. Invalid or empty dates stay
visible with an ErrorProvider message, without trapping keyboard focus. The Customer Entry notes
toolbar is docked above the RichTextBox as a sibling inside the Customer notes GroupBox.

DemoConstants.cs controls startup: StartupVisualStylesMode selects Classic or Net11, StartupView
selects a DemoView (Customer by default), and StartupFormRevealMode selects Classic or Deferred.
Restart the application after changing these compile-time switches.

For the reveal comparison, start with the Customer view and set StartupFormRevealMode to Classic
to watch its controls paint progressively. Switch to Deferred and restart to see only the completed
form. CustomerRevealSlowdown delays each existing child window's first WM_PAINT by up to 60 ms,
within a shared 1800 ms startup budget. It does not change control bounds, force extra layouts, or
pump messages. Each hook detaches after its first paint; later edits, resizing, and style changes
are not slowed. Set CustomerFirstPaintDelayMilliseconds to zero to disable the experiment.
The customer screen is a UserControl hosted by MainForm; the reveal policy belongs to that form.

The optional Windows 11 custom-chrome experiment is isolated on the Win11FormChrome branch,
based on main. Its usage notes are in Win11Chrome.txt on that branch.
