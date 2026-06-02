---
name: warptoolkit-console-diagnostics
description: Guide for WarpToolkit.WinForms.Specialized developer-tool types — the interactive ConsoleControl, the HexAsciiDumper hex/ASCII formatter, and the FilenameDisambiguator collision-free filename generator. Use this when embedding an in-process console/terminal pane, dumping bytes as hex+ASCII, or generating unique export filenames.
---

# WarpToolkit.WinForms.Specialized Console & Diagnostics

Developer-tool controls and helpers from `WarpToolkit.WinForms.Specialized`
(namespace `WarpToolkit.WinForms.Specialized`).

> **Source of truth:** verified against
> `src/WarpToolkit.WinForms.Specialized/Console/`, `Tools/`, and `IO/`. Real
> usage: `samples/Self Managed/cs/WarpToolkit.GitScanner/UI/Console/ConsoleTab.cs`.

## When to Use This Skill

- Embedding an **interactive in-process console / terminal pane** that streams
  colored output and runs commands (`ConsoleControl`).
- Formatting binary data as a **hex + ASCII dump** (`HexAsciiDumper`).
- Generating **collision-free filenames** for save/export features
  (`FilenameDisambiguator`).

## ConsoleControl

`ConsoleControl : RichTextBox`. An interactive console surface with colored,
styled, async writes and command execution. Configure it like a `RichTextBox`
in the Designer, then write to it from regular (async) code.

Key public API (verified signatures):

```csharp
Task WriteAsync(string text, Color? textColor = null, CustomFontStyle? style = null, FontSize? size = null, bool keepStyles = false);
Task WriteLineAsync(string? text = null, Color? textColor = null, CustomFontStyle? style = null, FontSize? size = null, bool keepStyles = false);
Task WriteLineAsync(string? text, ConsoleChannel channel);   // channel picks the color
Task WriteMessageAsync(string text, ConsoleMessageKind kind = ConsoleMessageKind.Standard, bool includeTimestamp = false);
Task SetStyleAsync(Color? textColor = null, CustomFontStyle? style = null, FontSize? size = null, bool keepSetting = false);
Task ResetStylesAsync();
Task ResetAsync();
Task RunCommandAsync(string command, string arguments);
Task RunCommandAsync(string command, string arguments, ConsoleChannel channel);
TextWriter ConsoleOut { get; }       // a TextWriter that writes into the control
TextWriter ConsoleOutAsync { get; }
```

Supporting enums:
- `ConsoleChannel`: `Default = 0, Trace, Debug, Information, Warning, Error, Git, Sql`.
- `ConsoleMessageKind`: `Standard, Command, Output, Information, Success, Warning, Error`.
- `CustomFontStyle` `[Flags]`-style: `Normal = 0, Bold = 1, Italic = 2, Underline = 4, StrikeThrough = 8`.
- `FontSize`: `Normal = 0, Smaller, Small, Larger, Large`.
- `ConsoleChannelColors` (static) — predefined channel colors plus
  `Color GetStdOut(ConsoleChannel channel, Color fallback)` and
  `Color GetStdErr(ConsoleChannel channel)`.

```csharp
// Designer file — flat, Designer-compliant:
_console = new ConsoleControl();
_console.Dock = DockStyle.Fill;
_console.ReadOnly = true;
_console.BorderStyle = BorderStyle.None;
_console.WordWrap = false;
Controls.Add(_console);

// Regular async code:
await _console.SetStyleAsync(
    textColor: Color.LightSkyBlue,
    style: CustomFontStyle.Bold,
    size: FontSize.Normal,
    keepSetting: true);

await _console.WriteLineAsync("Running build…", ConsoleChannel.Information);
await _console.RunCommandAsync("dotnet", "build", ConsoleChannel.Git);
```

## HexAsciiDumper

`public class HexAsciiDumper`. Streams bytes/strings in and produces a formatted
hex (or octal/decimal) + optional text dump. Feed it incrementally and flush at
the end.

| Member | Signature / default |
|--------|---------------------|
| ctor | `HexAsciiDumper()` |
| `BytesPerRow` | `int { get; set; }` |
| `DataFormat` | `DataFormat { get; set; }` |
| `ShowText` | `ShowText { get; set; }` |
| `TryGetString` | `bool TryGetString(byte[] newData, out string? result)` |
| `TryGetString` | `bool TryGetString(string newData, out string? result)` |
| `GetRemaining()` | `string` |
| `Flush()` | `void` |

Enums: `DataFormat { Hex, Oct, Decimal }`, `ShowText { None, ASCII, Unicode }`.

```csharp
HexAsciiDumper dumper = new();
dumper.BytesPerRow = 16;
dumper.DataFormat = DataFormat.Hex;
dumper.ShowText = ShowText.ASCII;

if (dumper.TryGetString(buffer, out string? rows))
{
    await _console.WriteAsync(rows);
}
await _console.WriteAsync(dumper.GetRemaining()); // flush the partial last row
```

## FilenameDisambiguator

`public class FilenameDisambiguator`. Produces collision-free filenames using a
`GenerationStrategy` and optional base path/title/extension.

Constructors include:
```csharp
FilenameDisambiguator();
FilenameDisambiguator(string title, string extension, bool requestPathForFile = false);
FilenameDisambiguator(string title, string basePath, string extension, bool requestPathForFile = false);
FilenameDisambiguator(GenerationStrategy generationStrategy, string extension, bool requestPathForFile = false);
FilenameDisambiguator(string title, string extension, GenerationStrategy generationStrategy, bool combineUniquePath = false);
FilenameDisambiguator(string title, string basePath, string extension, GenerationStrategy generationStrategy, bool requestPathForFile = false);
```

Key members:

| Member | Signature |
|--------|-----------|
| `GenerationStrategy` | `GenerationStrategy { get; set; }` |
| `Title` | `string? { get; set; }` |
| `Extension` | `string { get; set; }` (non-null) |
| `BasePath` | `string? { get; set; }` |
| `CombineUniquePath` | `bool { get; set; }` |
| `DateFilenameAmendmentFormat` | `string { get; set; }` |
| `MaxLengthFilename` / `MaxLengthFolder` | `int { get; set; }` |
| `FilenameCandidate` / `FolderCandidate` / `FullFilename` | `string { get; }` |
| `GetResultingFilenameParts()` | `(string folder, string filename, string extension)` |
| `GetResultingFoldername()` | `string` |
| `TryDelete()` | `bool` |
| static `ExpandPath` / `ShrinkPath` | `string (string)` |
| static `FromFilename` | `FilenameDisambiguator FromFilename(string fullPathAndFilename)` / `(string basePath, string filename)` |

`GenerationStrategy` enum: `None, DateBased, GuidBase, DateAmended`.

```csharp
FilenameDisambiguator dis = new(
    title: "ChatExport",
    basePath: chatFolder,
    extension: ".md",
    generationStrategy: GenerationStrategy.DateAmended);

string fullPath = dis.FullFilename; // unique, non-colliding
```

## DarkMode & High-DPI notes

- `ConsoleControl` derives from `RichTextBox`; set `BackColor`/`ForeColor` (or a
  dark theme) and a monospace `Font` (e.g. `"Cascadia Mono"`) as in the
  GitScanner sample. Channel colors come from `ConsoleChannelColors`.
- `HexAsciiDumper` and `FilenameDisambiguator` are non-visual helpers — no DPI
  considerations.

## Common gotchas

- All `ConsoleControl` write/run/style methods are **async (`Task`)** — `await`
  them and marshal to the UI thread; don't fire-and-forget on a background
  thread without care.
- Choose the `WriteLineAsync(text, ConsoleChannel)` overload for channel-colored
  output; the `Color?`-based overloads are for ad-hoc colors.
- `HexAsciiDumper` buffers a partial final row — always emit `GetRemaining()` (or
  `Flush()`) so the last bytes appear.
- `FilenameDisambiguator.Extension` must be non-null (its setter throws on null).
