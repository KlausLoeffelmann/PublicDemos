---
name: warp-fluent-tab-control
description: Use this skill when adding, designing, or sizing tabbed UI built on WARP's FluentTabControl. Covers the mandatory "one UserControl per tab page" pattern, how each UserControl drives the host form's natural size via its Designer-set Size, the size-cap rules the host must respect (≤ 75% of current screen and ≤ 90% of parent), and when a tab UserControl needs to add an internal scrolling layer because its content exceeds the cap.
---

# WARP FluentTabControl

`FluentTabControl` (in `WarpToolkit.WinForms.Containers`) is WARP's modern,
dark-mode-aware tab control. Its API is intentionally small — but the
**right way to use it** is not derivable from the API alone, because the
control delegates layout, sizing and scrolling to whatever you put inside.

## When to use this skill

- When the user has explicitly instructed before to use or introduce the WarpToolkit library for a specific project.
- Add a new tab to a form that hosts a `FluentTabControl`.
- Design or refactor an existing tab page.
- Size or resize the host form (`MainForm`, dialog, MDI child …) and need
  to honor the tab-driven natural size.
- Decide whether a tab's content needs its own scroll viewer.

## API at a glance

`FluentTabControl : Panel` and exposes:

| Member | Signature / shape |
|--------|------------------|
| `AddTab` | `void AddTab(string tabPageTitle, ContainerControl tabContent)` |
| `CurrentTabIndex` | `int { get; set; }` |
| `CurrentTab` | `Control? { get; }` |
| `Tabs` | `IEnumerable<Panel>` (the page hosts) |
| `TabChanged` | `event EventHandler?` |
| `OnFontChanged` | tab-header font tracks `Font + 1pt` |

Critical detail: `AddTab` takes a **`ContainerControl`**, not a `Control`.
The intended container is a **`UserControl`** — one per tab page.

```csharp
_fluentTab.AddTab("Chat",     _chatView);          // _chatView : UserControl
_fluentTab.AddTab("Settings", _settingsView);      // _settingsView : UserControl
_fluentTab.AddTab("Diagnostics", _diagnosticsView);
```

Internally `AddTab` wraps the supplied content in a `Panel` page,
`Dock = Fill`s the content, and adds a tool-strip menu item per tab.
There is no `TabPage` class — the page is just a panel.

## Mandatory pattern: one UserControl per tab page

A `FluentTabControl` is **not** like the classic `TabControl` where you
parent every child control directly to a TabPage. Designer-friendly use
requires each tab to be encapsulated as its own `UserControl`:

| Reason | Why it matters |
|--------|---------------|
| Designer compatibility | The Designer can open one UserControl at a time, with its own `InitializeComponent`. Cramming all tabs into the host form bloats `InitializeComponent` past Designer-parseable limits and makes layout tweaks per tab miserable. |
| Natural size signalling | The UserControl's Designer-set `Size` (also exposable as `PreferredSize`) is the single source of truth for "how big this tab wants to be". The host form derives its desired client size from the **maximum** of all tab UserControls. |
| DI / lifetime | UserControls can be obtained from DI (`serviceProvider.GetRequiredService<MyTabView>()`) which is the same path used by every other WARP service component. |
| Scoping | Each tab gets its own `ISupportInitialize` cycle, its own designer file, its own events and its own ViewModel. |

> **Rule:** Every visual tab corresponds to **exactly one `UserControl`
> type** in the project. Do not parent loose controls directly to a
> `FluentTabControl` page.

### Skeleton for a new tab

```csharp
// SettingsView.cs (regular code, modern C# allowed)
public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    // Optional DI ctor — same façade pattern used by Form. See the
    // warp-winforms-application-builder skill for details.
    public SettingsView(IServiceProvider serviceProvider) : this()
    {
        // store, but do NOT use services in the constructor.
        _serviceProvider = serviceProvider;
    }
}
```

```csharp
// SettingsView.Designer.cs (Designer rules — flat InitializeComponent,
// backing fields, no helpers, no lambdas, no NRT annotations).
// The Designer-set Size on the UserControl drives the natural size.
this.Size = new Size(720, 480);
```

The Designer-set `Size` on the UserControl matters: it is the size that
`AddTab`-wrapping inherits when the UserControl is docked into the tab
page, and it is the value the host should query when computing its own
desired bounds (see next section).

## Sizing the host: derive, but cap

When a form embeds a `FluentTabControl`, the form's "natural" size is
driven by the largest UserControl it hosts as a tab. But you must **cap**
that against display reality so a UserControl with a 4 000 × 3 000
Designer canvas does not push the form off a 1080p monitor.

### The two caps

Use the **more restrictive** of:

1. **≤ 75% of the current screen's working area** — for top-level forms.
2. **≤ 90% of the parent control's client area** — for embedded scenarios
   (e.g. inside an MDI parent, a split container, or another UserControl).

If the desired size fits inside both caps, use it. Otherwise clamp to the
applicable cap, and accept that the affected tabs will need to scroll.

### Computing the host size

```csharp
// In MainForm.OnLoad (or wherever you finalize layout — never inside
// InitializeComponent). This runs in regular code, so modern C# is fine.
protected override void OnLoad(EventArgs e)
{
    base.OnLoad(e);

    Size desired = ComputePreferredClientSize(_fluentTab);
    Size cap     = ComputeHostSizeCap(parent: this);

    // Apply the more restrictive cap; keep at least the minimum.
    Size final = new(
        Math.Max(MinimumSize.Width,  Math.Min(desired.Width,  cap.Width)),
        Math.Max(MinimumSize.Height, Math.Min(desired.Height, cap.Height)));

    ClientSize = final;
    EnsureTabsCanScrollIfClipped(_fluentTab, final);
}

private static Size ComputePreferredClientSize(FluentTabControl tab)
{
    // Each tab page hosts the UserControl as a docked child. Walk the
    // Tabs collection and aggregate the max width and height.
    int w = 0, h = 0;
    foreach (Panel page in tab.Tabs)
    {
        if (page.Controls.Count == 0) continue;
        Control content = page.Controls[0];
        // Prefer the UserControl's PreferredSize override if it provides one,
        // otherwise the Designer-set Size is the natural target.
        Size s = content.PreferredSize != Size.Empty
            ? content.PreferredSize
            : content.Size;
        if (s.Width  > w) w = s.Width;
        if (s.Height > h) h = s.Height;
    }
    // Account for the FluentTab's header strip.
    h += tab.PreferredSize.Height; // or a measured value
    return new Size(w, h);
}

private static Size ComputeHostSizeCap(Control parent)
{
    Form? top = parent.FindForm();
    Screen screen = top is not null
        ? Screen.FromControl(top)
        : Screen.PrimaryScreen!;

    // 75% of current screen working area.
    Rectangle wa = screen.WorkingArea;
    Size screenCap = new((int)(wa.Width * 0.75), (int)(wa.Height * 0.75));

    // 90% of parent client area — only when not the top-level form.
    if (parent is Form)
    {
        return screenCap;
    }

    Size parentCap = new(
        (int)(parent.ClientSize.Width  * 0.90),
        (int)(parent.ClientSize.Height * 0.90));

    // More restrictive wins.
    return new Size(
        Math.Min(screenCap.Width,  parentCap.Width),
        Math.Min(screenCap.Height, parentCap.Height));
}
```

### When a tab needs to scroll

If `desired > cap` along any axis, at least one tab UserControl will be
clipped after the host applies the cap. Those UserControls **must**
provide their own scrolling layer.

There are two equally good ways:

#### Option A — `UserControl.AutoScroll = true`

The simplest fix. `UserControl` derives from `ScrollableControl`, so
`AutoScroll = true` causes the framework to display scrollbars when the
client area is smaller than the laid-out content.

Caveats:

- Set `AutoScrollMinSize` to the Designer-set `Size`, otherwise
  `AutoScroll` only kicks in when child controls are positioned beyond
  the current bounds (Anchor/Dock-respecting layouts will not trigger it
  on their own).
- Avoid `Dock = Fill` on the *immediate* child if you want vertical
  scrolling; otherwise the child resizes to match the shrinking client
  area and there is nothing to scroll. Use a top-anchored, fixed-height
  inner panel.

```csharp
// In the UserControl ctor / OnLoad (regular code):
AutoScroll = true;
AutoScrollMinSize = new Size(720, 480); // matches the Designer-set Size
```

#### Option B — explicit inner scrolling panel

When you want one part of the UserControl pinned (e.g. a toolbar at the
top) and only the rest to scroll, add a dedicated `Panel` with
`AutoScroll = true` and a fixed inner content control:

```
UserControl (Dock=Fill on the tab page)
└── TableLayoutPanel root (2 rows: Auto | *)
    ├── Toolbar    (Dock=Fill in row 0, AutoSize)
    └── ScrollHost (Dock=Fill in row 1, AutoScroll=true)
        └── ContentPanel (Top-anchored, fixed size)
```

This is the recommended pattern for UserControls whose Designer-set Size
is **substantially** larger than realistic host caps (settings dialogs,
diagnostic views, wizards converted to tabs, etc.).

### Helper: enable scrolling reactively

```csharp
private static void EnsureTabsCanScrollIfClipped(FluentTabControl tab, Size hostClientSize)
{
    foreach (Panel page in tab.Tabs)
    {
        if (page.Controls.Count == 0) continue;
        if (page.Controls[0] is not UserControl uc) continue;

        bool clipped = uc.Size.Width  > hostClientSize.Width
                    || uc.Size.Height > hostClientSize.Height;

        if (clipped && !uc.AutoScroll)
        {
            uc.AutoScroll        = true;
            uc.AutoScrollMinSize = uc.Size;
        }
    }
}
```

Call it once after the host computes its final size (typically in
`OnLoad`). For dynamic tabs added later, call it again after `AddTab`.

## Adding tabs at runtime

When code paths add tabs after construction (typical: opening a code-block
viewer in Chatty), the same rule applies — instantiate a UserControl, then
add it. Get the UserControl from DI if it needs services:

```csharp
// Inside a Form that already has _serviceProvider set:
var view = _serviceProvider.GetRequiredService<RoslynDocumentView>();
view.LoadDocument(codeBlockInfo);

_mainTabControl.AddTab(
    tabPageTitle: codeBlockInfo.Filename ?? "Untitled",
    tabContent: view);

EnsureTabsCanScrollIfClipped(_mainTabControl, ClientSize);
```

For ad-hoc constructions (no DI), use a parameterless constructor:

```csharp
var view = new RoslynDocumentView(codeBlockInfo);
_mainTabControl.AddTab(codeBlockInfo.Filename, view);
```

## Designer-vs-runtime split (recap)

`InitializeComponent` (Designer file) only:

- Instantiates the `FluentTabControl` field.
- Sets `Dock`, `Size`, `Anchor`, `Margin`, etc. on the control.
- Adds the control to the form's `Controls` collection.

**Never** call `AddTab` from `InitializeComponent`. `AddTab` adds runtime
state (selected tab, event handlers, child UserControls) that the
Designer parser cannot round-trip. Call `AddTab` from `OnLoad` or a
regular constructor body **after** `InitializeComponent` has returned.

## Anti-patterns

- **Do not** parent controls directly to a tab page's `Panel`. Wrap them
  in a `UserControl` first.
- **Do not** call `AddTab` inside `InitializeComponent`.
- **Do not** size the host form by adding fixed pixel constants — derive
  from the tab UserControls' sizes (with caps applied).
- **Do not** rely on `Dock = Fill` alone to enable scrolling — Dock-fill
  shrinks the child with the parent, defeating `AutoScroll`. Use
  `AutoScrollMinSize` or a fixed-size inner content control.
- **Do not** set the FluentTabControl's `AutoScroll` itself to `true` —
  scrolling belongs at the UserControl level, not at the tab strip level.
- **Do not** assume the user's monitor is 1080p+. Always apply the screen
  cap before applying `ClientSize`.

## Where to look next

- Modern controls around the tab (FluentMessageBox, wizards,
  FilePathPicker, BindableComboBox) — `warp-winforms-controls`.
- Wiring a UserControl into DI so it can receive ViewModels / services —
  `warp-winforms-application-builder`.
- Adding an AI chat tab — `warp-winforms-ai` (`ChatView` is a
  designer-droppable UserControl that fits this pattern out of the box).
