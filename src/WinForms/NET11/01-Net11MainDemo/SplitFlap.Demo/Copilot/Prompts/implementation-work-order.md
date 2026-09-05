# Split-Flap WinForms .NET 11 Demo Work Order

## Goal

Turn the current split-flap departure-board prototype into a reliable, stage-ready demonstration of .NET 11 WinForms features.

## Boundaries

- Use vanilla .NET 11 WinForms APIs only.
- Do not add or use WARP libraries.
- Keep Forms and UserControls compatible with the WinForms Designer.
- Put control/component construction and static property serialization in `*.Designer.cs`.
- Put behavior, calculations, persistence, and event handlers in regular `.cs` files.

## Documentation and comments

- Explain non-obvious design decisions and algorithms with useful inline comments.
- Give the sound code additional teaching-oriented comments covering PCM, buffering, sample rates, channels, oscillators, envelopes, mixing, reverb, voice lifetime, cancellation, WAV data, native WinMM ownership, and thread boundaries.
- Document public and protected APIs. Use valid lowercase XML tags with one leading space on content lines:

```csharp
/// <summary>
///  Description.
/// </summary>
```

- Add parameter, return-value, exception, remarks, or inheritance tags when they clarify the contract.
- Do not add comments that merely restate an assignment.

## Required project history

When starting from an untracked copy of the prototype, preserve it in this order before later edits:

1. Commit `SplitFlap.Controls`.
2. Commit `WinForms.Audio`.
3. Commit `SplitFlap.Demo` and `WinFormsNet11Demo.slnx`.

Use descriptive subjects and bodies. Keep later documentation, diagnostics/tests, audio fixes, UI/settings, and Copilot guidance in focused commits.

## Diagnostics and testability

- Write daily rolling logs beneath `%LocalAppData%\SplitFlap.Demo\Logs`.
- Include timestamps, severity, category, full exceptions, startup options, settings activity, presentation changes, and audio failures.
- Capture both WinForms UI-thread exceptions and non-UI unhandled exceptions.
- Maintain a .NET 11 xUnit v3 test project with deterministic tests.
- Audio unit tests must use fake `IAudioSink` implementations and must not require physical audio hardware.
- Support:

```text
--scenario display|sound|all
--run-for <seconds>
--no-settings
```

- Automated failures must be logged and return a nonzero exit code.
- A normal launch without options must remain interactive.

## Sound reliability

- Reproduce sound failures through the command-line scenario and inspect the AppData log.
- Report the failing WinMM operation, `MMRESULT`, translated error text, PCM format, and buffer configuration.
- Unwind partially allocated native resources when construction fails.
- Observe audio-pump failures and propagate them to playback tasks.
- Dispose the sink, buffers, events, and pending voices deterministically.
- Add a regression test for every fixed root cause.

## Stage-ready UI

The main form must include the .NET 11 `KioskModeManager` component and these menus:

```text
File
  Auto-Save Settings
  Save Settings
  ------------------
  Quit

View
  Full Screen (Kiosk Mode)
  Full Screen (Window)
  -------------------------
  Font Name and Size...
  Keep Aspect Ratio
  Define Lines/Column Count...
  Fit Screen Size
```

- Auto-save is enabled by default.
- Load existing settings in `OnLoad`.
- Persist safe normal window bounds/state, display font/grid/aspect settings, animation speed, sound, and board-sizing behavior.
- `--no-settings` disables both loading and saving.
- Kiosk fullscreen is borderless/topmost and managed by `KioskModeManager`.
- Window fullscreen is a normal maximized window with title bar and menu.
- Restore only usable on-screen bounds and never restore minimized.
- Use accessible, DPI-aware modal dialogs with keyboard mnemonics and OK/Cancel semantics.
- Preserve the configured display ratio during normal-window resizing when requested.
- Fit Screen Size should choose a font/grid fit that uses at least 80% of the active screen where feasible while retaining sufficient padding and avoiding clipping.

## Acceptance criteria

- The solution builds with the repository's .NET 11 SDK.
- All xUnit v3 tests pass.
- Display, sound, and combined timed scenarios exit successfully.
- Invalid command lines return a nonzero exit code and usage text.
- Settings round-trip and malformed settings fall back safely with diagnostics.
- Sound initializes and plays through the default Windows device.
- Kiosk and maximized-window modes enter and exit predictably.
- Designer files retain standard generated structure.
- No WARP reference, generated build output, or temporary file is committed.
