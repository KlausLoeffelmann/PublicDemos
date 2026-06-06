# Layout Tester (LayoutTests)

A WinForms .NET 10 prototype for poking at **High-DPI cascaded layout** — how
auto-scaling propagates through nested `UserControl` containers when the host
form moves between monitors with different DPI under `PerMonitorV2`.

The app lets you build up a **probe set** of nested *carrier containers*,
configure their scaling parameters individually, save the set to disk, and
then launch a separate **carrier form** that actually instantiates the
hierarchy. You can then drag that carrier form between monitors at different
DPI and observe how each level of the cascade behaves.

## What's in a probe set

- A `ProbeFormDefinition` (the outer form: design size, scale settings).
- A tree of `ContainerDefinition`s — each one is a `UserControl` that gets
  hosted in the carrier form.

Each container has independent `ContainerParameters`:

- `ContainerKind`
  - **CTor** — the container is constructed and added to its parent during
    `InitializeComponent`-style setup.
  - **Lazy** — the container is added after the form's `OnLoad` via
    `BeginInvoke`, i.e. *after* the message loop has processed the initial
    layout pass.
- `DesignResolution` — `VGA_640x480`, `SVGA_800x600`, `WXGA_1280x800`.
- `ScalePercent`, `AutoScaleMode` (`Font` / `Dpi` / `None` / `Inherit`),
  `ApplyPhase` (`InCtor` vs `AfterOnLoad`).
- Font family / size / style.

The combination of *kind*, *apply phase*, and *auto-scale mode* is the whole
point of the demo — these are the knobs that decide whether a child container
inherits the parent's effective scale correctly or gets re-scaled (or
double-scaled) on a DPI change.

## How to use it

1. Open the solution `src\Prototypes\LayoutTests\LayoutTests.slnx` and run
   `LayoutTests.App` (F5).
2. Use the toolbar / menu to **Add Container** under the form root (or under
   another container) — pick CTor or Lazy.
3. Tweak parameters in the right-hand property panel.
4. **Save** the probe set; **Load** it later.
5. Hit the **action** button to launch a `CarrierForm` that materializes the
   tree and shows the result.
6. Drag the carrier form between monitors at different scaling factors to
   watch the cascade.

UI state (window size, splitter, last layout) is persisted via WARP's
`IUserSettingsService`.

## Project layout

```
LayoutTests.App/
├─ Program.cs                 # WinFormsApplication bootstrap (PerMonitorV2)
├─ MainForm.cs                # Probe-set editor + tree + property panel
├─ Carrier/                   # Containers + the runtime CarrierForm
│  ├─ CarrierContainerBase.cs
│  ├─ CTorContainerControl.cs
│  ├─ LazyContainerControl.cs
│  └─ CarrierForm.cs
├─ Designer/                  # Editor UI (tree, property panel, add-dialog)
└─ Models/                    # ProbeSet, ContainerDefinition, parameters, enums
```

## Dependencies

ProjectReferences (not NuGet) into the local WARP toolkit, so you can step
through WARP itself while debugging layout behavior:

- `WarpToolkit.WinForms`
- `WarpToolkit.WinForms.Extensions`
- `WarpToolkit.WinForms.AppServices`

Expects WARP checked out at `..\..\..\..\WARP\src` relative to the repo root.
