# "WinBaas" - WinForms Backup Assisst Application Prompt

Use the following prompt as the implementation specification for a WinForms application to be created in `src/WinForms/NET10/WinBaas`.

## Prompt

Create a new .NET 10 WinForms desktop application in the current branch "WinBaas" and said directory, that helps users discover and back up common “easy to forget” folders, files, and local SQL Server artifacts on a machine.

The name of the App and the namespace is "WinBaas" - the title in the Main Form is "WinBaas - WinForms Backup Assist"

The application must use the WARP WinForms hosting model and `IUserSettingsService`:
- Use `WinFormsApplication.CreateBuilder(args)` / `WinFormsApplication`.
- Use `AddWinFormsUserSettingsService(...)` for settings persistence.
- Use the WARP app services pattern for dialogs/exception handling if appropriate.
- Do not use MVVM and do not use DirectX.
- Follow the patterns described in the repo skills under `.github/skills/` (especially `WinFormsApplicationBuilder`, `winforms-development`, `winforms-designer-code`, and `AppServices`).

## Goal and scope

Build a small backup tool with the following behavior:
- A MenuStrip at the top.
- A ToolStrip below the MenuStrip. This should contain the most frequently used features. Set the Icon size to 36x36 Pixel. Use the WARP library and the ToolStripItem skill.
- A SplitContainer with a TreeView on the left and a ListView on the right.
- A StatusStrip at the bottom.
- The TreeView holds object sources only. It must never directly hold the actual objects that later get backed up.
- The ListView holds the actual discovered objects for the selected source node.
- The tool should know a broad predefined catalog of common “forgotten” backup locations and file-type combinations.
- The catalog must support custom user-defined entries.
- The tool must support discovery of LocalDB and SQL Express database instances and list their attached databases.
- The tool must allow backup of selected objects to either a folder or a ZIP archive.

## Predefined catalog behavior

Seed the catalog with a rich set of predefined entries for common machine-local backup spots, for example (use your knowledge to add to the list!)
- Markdown, Word, Excel, PDF, PowerPoint, and other office-like files from the Downloads folder.
- Settings files from custom apps in `AppData` subfolders.
- Screenshots, photos, videos, and recordings.
- Files (not links) on the Desktop and in Desktop subfolders.
- Visual Studio projects that are not inside GitHub repositories.
- Edge favorites.
- Camtasia raw recordings.
- Windows voice or screen recordings.
- Environment variables that are not standard path definitions (for example Copilot keys or other secrets).
- Oh My Posh definitions and Lua files.
- LocalDB and SQL Server Express instances holding databases.
- Additional common “forgot to back up” spots as appropriate.

Each catalog entry must include:
- Path
- List of file extensions
- List of known file names
- Name
- Description
- Kind (folder, file, environment variable, SQL server, etc.)
- Whether subfolders should be included
- Whether the entry is user-defined or built-in

The predefined catalog should be extensive, alright, but practical rather than exhaustive, and should favor common, 
well-known locations over obscure edge cases.

## TreeView and selection model

The left TreeView must follow these rules:
- The TreeView contains only source nodes (root entries and their discovered source folders / instances / groups).
- It must not directly contain every backup candidate object.
- A click on a node in the TreeView must always refresh the ListView on the right side.
- Selecting a parent node must select all child nodes and the corresponding ListView items.
- Clearing a parent node must clear all child nodes and the corresponding ListView items.
- Selecting a child node must select the corresponding ListView items.
- Clearing a child node must clear the corresponding ListView items.
- Selection and checked state between the TreeView, child nodes, and ListView items must stay synchronized reliably.
- The source node selection should be the authoritative state for the ListView contents.

### Tree structure expectations

The TreeView should have root entries such as:
- `C:\`
  - folders in drive C that contain discoverable backup candidates
- `N:\boxes`
  - folders in drive N that contain discoverable backup candidates
- additional drives or user folders as appropriate
- `SQL Server`
  - `LocalDB`
  - `SQL Express`

Only show tree branches that actually contain discoverable items.

## ListView model and columns

The right ListView must show the actual object candidates for the selected source node.

Use a ListView with checkboxes.

The ListView column headers must be:
- `[Filename]` — the file name including extension
- `[File type]` — a user-friendly label for the file extension, not the raw extension
- `[Changed]` — last changed date in `yyyy-MM-dd HH:mm`
- `[Created]` — creation date in `yyyy-MM-dd`
- `[Size]` — IEC size with 1024-byte-based units (KiB, MiB, GiB, TiB, …)

Implement a well-known file-type map with common extensions, for example:
- `.sln` → `VS Solution`
- `.slnx` → `VS V17+ Solution`
- `.cs` → `C# Code Files`
- `.vb` → `VB Code Files`
- `.md` → `Markdown files`
- `.jpeg`, `.jpg` → `JPEG Image`
- `.png` → `PNG Image`
- `.gif` → `GIF Image`
- `.bmp` → `BMP Image`
- `.heic`, `.heif` → `HEIF Image`
- `.mp4`, `.mov`, `.avi`, `.webm` → `Video File`
- `.wav`, `.m4a` → `Audio File`
- `.json`, `.xml`, `.config`, `.ini`, `.toml`, `.yaml`, `.yml` → `Configuration File`
- `.url` → `Edge Favorite`
- `.camproj`, `.camrec`, `.trec` → `Camtasia Recording`
- `.lua` → `Lua Script`
- `.ps1` → `PowerShell Script`
- `.sql` → `SQL Script`
- `.bak` → `SQL Backup`
- `.pdf` → `PDF Document`
- `.doc`, `.docx` → `Word Document`
- `.xls`, `.xlsx` → `Excel Workbook`
- `.ppt`, `.pptx` → `PowerPoint Presentation`
- `.zip` → `ZIP Archive`

The mapping should be reasonably large, but should not include esoteric file types. 
The most common extension should win over a less common known extension.

When an item is selected in the ListView:
- Show the SI size and abbreviation in the ToolStrip status area.
- Show the formatted bytes in parentheses using `###,###,###,###,###,##0`.
- Example: `1.23 MiB (1,290,000 bytes)`.

## Folder and file discovery behavior

The discovery engine must be best-effort and non-fatal:
- Missing folders, inaccessible paths, missing SQL tools, and SQL connection failures must not crash the application.
- The app should continue and surface warnings or messages when needed.

Discovery rules:
- If the resulting ListView content would not reveal files but folders, the ListView must show only folders recursively.
- If the source node represents a folder, the ListView should be populated with matching files and folders.
- If the source node represents a file, the ListView should show that single file or the matching file entry.
- For folders, the size of folders must be calculated asynchronously and updated in the UI.
- Do not block the UI while computing folder sizes.
- The TreeView must only show branches that contain at least one discoverable backup candidate.

## SQL discovery behavior

SQL discovery should be limited to LocalDB and SQL Express only.
- Discover LocalDB instances and SQL Express instances when available.
- Discover attached databases for those instances.
- Provide a best-effort discovery toggle in the options dialog.
- Include database discovery in the catalog by default.

## Menu actions and dialogs

Implement the following menu actions:
- `Discover objects to backup...`
  - Starts the discovery process.
  - Shows status and progress in the StatusStrip.
  - Uses a progress bar during the discovery operation.
- `Backup selected object...`
  - Enabled only when discovery is not running and at least one object is selected.
  - Supports copy-to-folder and ZIP archive output.
  - Lets the user choose the destination.
- `Add Object...`
  - Opens a dialog that lets the user pick a folder or file.
  - If the user selects a folder, the dialog must allow choosing file extensions and whether subfolder backup is recursive.
  - Support user-defined folder, file, environment-variable, and SQL entries as appropriate.
- `Delete Object...`
  - Removes a user-defined object from the catalog.
  - Built-in catalog entries should not be deleted directly.
- `Restore definition...`
  - Restores the catalog to the default built-in definition after confirmation.
- `Options...`
  - Allows the user to configure the SQL discovery toggle.
  - Allows the user to configure a roaming catalog path.
  - Allows the user to choose the backup mode (copy-to-folder vs ZIP).

## Persistence and settings

Persist application state and user settings through `IUserSettingsService`.
- The catalog should be persistable.
- The roaming catalog path option should be honored in the actual catalog storage behavior when configured.
- Restoring defaults must reset the catalog to the built-in definition.
- The app should remember form bounds and state.
- The options dialog must save settings immediately on OK.

## UI and layout requirements

The controls and layout must be implemented so that the form behaves cleanly:
- MenuStrip, ToolStrip, and StatusStrip must remain visible at all times.
- The Z-order of docked controls must be correct.
- Use proper docking so the MenuStrip, ToolStrip, and StatusStrip remain fixed while the SplitContainer fills the remaining area.
- The base font of all forms should be `11 pt`.
- Do not use the application-wide base font feature.
- Set the ToolStrip font explicitly and do not rely on ambient inheritance.
- The ToolStrip and StatusStrip should use dedicated font assignments, not ambient control fonts.

## Designer and code-behind requirements

Follow the WinForms Designer rules in the repo skills:
- Keep Designer-generated code in `.Designer.cs` files only.
- Keep application logic, event handlers, and service interaction in regular `.cs` files.
- Keep the code-behind clean and separate from the Designer file.
- Keep the main form compatible with the WinForms Designer.
- Make sure the DI-aware constructor assigns `_serviceProvider` before `InitializeComponent()`.
- Do not put business logic inside `InitializeComponent()`.
- Avoid Designer-incompatible C# constructs inside `.Designer.cs` files.

## Validation expectations

Before considering the work complete:
- Build the solution.
- Run a smoke test of discovery and backup flow.
- Verify the TreeView/ListView synchronization logic.
- Verify the ListView column formatting and size display behavior.
- Verify that the roaming catalog option and restore-to-default behavior work.
- Commit meaningful milestones and push them to the `BackupWorkmachine` branch.

## Output expectations

When implementing, keep the current project layout and follow the existing per-demo structure under:
- `src/WinForms/NET10/VSToolBox/2026-05/BackupWorkmachine`

Implement the application in a way that preserves the current architecture, but fully satisfies the requirements above.
