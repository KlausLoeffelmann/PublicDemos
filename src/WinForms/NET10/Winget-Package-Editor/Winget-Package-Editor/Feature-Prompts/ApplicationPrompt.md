# WinGet Package Editor — Implementation Prompt for Copilot

## Project Summary

Build a Windows Forms application targeting **.NET 10** that acts as a visual editor for **WinGet Configuration packages** (DSC YAML consumed by `winget configure`). It is a single-user personal tool for managing ~8 development machines from one curated, repeatable provisioning surface.

The app uses the **WARP.Toolkit** WinForms library. WARP and WinForms conventions are already understood by the agent; follow standard WARP patterns and WinForms idioms. Be precise where this document is precise (WinGet YAML, data model, process behavior) and conventional everywhere else.

---

## Scope — v1 only

**In scope:**

- Create / edit / delete / save / load Package documents (JSON on disk).
- Edit polymorphic app entries via a master-detail UI.
- Emit a `config.yaml` + `install.ps1` pair from a Package.
- "Run Now" — shell out to `winget configure`, stream output to a console pane.
- Settings file in `%APPDATA%`, designed to roam via OneDrive Known-Folder Move.
- Hardcoded curated catalog.

**Explicitly out of scope (v2 candidates — do not implement):**

- Scheduled Task creation.
- Self-contained .NET exe bundle generation.
- Catalog editor / user-extensible catalog.

---

## Build Order — Engine First

The interesting risk in this project is not the UI. Build in this order:

1. **Data model** (POCOs with `System.Text.Json` polymorphism).
2. **YAML emitter** (model → `winget configure` YAML).
3. **Process runner** for `winget configure` with async stdout/stderr streaming.
4. **One hardcoded test package** that round-trips through #1–#3 from a **console host** (a small `Program.cs` or xUnit test).
5. **WinForms editor** wrapping the engine.

**Do not start on the WinForms UI until step 4 produces YAML that `winget configure` accepts and runs end-to-end.** The engine is where the real risk lives; the UI is the last 30% of the work, not the first.

---

## Data Model

```csharp
public sealed class WingetPackage
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string Version { get; set; } = "1.0.0";
    public List<AppEntry> Apps { get; set; } = [];
}

public enum AppAction { Ensure, Install, Upgrade }
public enum AppScope  { User, Machine }
public enum AppSource { Winget, MSStore }

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(GenericAppEntry),  "generic")]
[JsonDerivedType(typeof(VisualStudioEntry), "vs")]
[JsonDerivedType(typeof(VSCodeEntry),       "vscode")]
public abstract class AppEntry
{
    public string Id { get; set; } = "";          // WinGet package id
    public string DisplayName { get; set; } = "";
    public AppAction Action { get; set; } = AppAction.Ensure;
    public AppSource Source { get; set; } = AppSource.Winget;
    public string? Version { get; set; }          // null = latest
    public AppScope Scope { get; set; } = AppScope.Machine;
    public bool AllowPrerelease { get; set; }
    public Dictionary<string, string> ExtraSettings { get; set; } = [];
}

public sealed class GenericAppEntry : AppEntry { }

public sealed class VisualStudioEntry : AppEntry
{
    public VSEdition Edition { get; set; }         // Community | Professional | Enterprise | BuildTools
    public VSChannel Channel { get; set; }         // Release | Preview
    public string?   VSConfigPath { get; set; }
    public string?   VSConfigInline { get; set; }  // alternative to path
    public string?   InstanceNickname { get; set; }
    public List<VsixReference> Extensions { get; set; } = [];
}

public sealed class VsixReference
{
    public string Identifier { get; set; } = "";   // marketplace ItemName, .vsix URL, or local path
    public bool   Admin { get; set; } = true;
}

public sealed class VSCodeEntry : AppEntry
{
    public List<string> Extensions { get; set; } = []; // publisher.name format
}

public enum VSEdition { Community, Professional, Enterprise, BuildTools }
public enum VSChannel { Release, Preview }
```

Serialize Packages to JSON as the document format.

---

## Curated Catalog (v1, hardcoded)

Each catalog item provides defaults (Id, DisplayName, default action, polymorphic entry type). Adding a catalog item to a package creates a fresh editable copy.

| DisplayName | WinGet Id | Entry Type | Notes |
|---|---|---|---|
| PowerShell 7 | `Microsoft.PowerShell` | Generic | |
| Visual Studio Code | `Microsoft.VisualStudioCode` | VSCode | Has Extensions list |
| Visual Studio | (see VS section) | VS | Custom flow |
| Clink | `chrisant996.Clink` | Generic | |
| Oh My Posh | `JanDeDobbeleer.OhMyPosh` | Generic | |
| Sysinternals Suite | `Microsoft.Sysinternals.Suite` | Generic | Verify id |
| 7-Zip | `7zip.7zip` | Generic | |
| ScreenToGif | `NickeManarin.ScreenToGif` | Generic | |
| WinMerge | `WinMerge.WinMerge` | Generic | |
| Windows App | `Microsoft.WindowsApp` | Generic | Cloud-PC client, formerly Remote Desktop |
| GitHub CLI | `GitHub.cli` | Generic | |
| GitHub Copilot CLI | `GitHub.cli` + extension | Generic | Post-step: `gh extension install github/gh-copilot` |
| Paint.NET | `dotPDN.PaintDotNet` | Generic | |
| PowerToys | `Microsoft.PowerToys` | Generic | |
| .NET 8 SDK | `Microsoft.DotNet.SDK.8` | Generic | |
| .NET 10 SDK | `Microsoft.DotNet.SDK.10` | Generic | |
| .NET 11 SDK (Preview) | `Microsoft.DotNet.SDK.Preview` | Generic | Set `AllowPrerelease = true`; verify id |

**Important:** WinGet IDs in this table are best-effort. Several may be stale. Before locking the catalog, run `winget search <name>` for each entry and capture the canonical id. Do **not** ship the catalog with unverified IDs.

Add a `CatalogValidator` that, in DEBUG builds, calls `winget show --id <id> --exact` for each catalog entry at startup and logs any misses.

---

## WinGet Configuration YAML — Precise Spec

**This is the section to be exact about. Read carefully.**

The output format is **WinGet Configuration**, which uses **DSC v3** semantics over a **WinGet Configuration v0.2** schema.

### File header

```yaml
# yaml-language-server: $schema=https://aka.ms/configuration-dsc-schema/0.2
properties:
  configurationVersion: 0.2
  assertions: []
  resources:
    # ... resource entries
```

Verify the current canonical schema URL before locking it in (Microsoft has occasionally moved DSC schema endpoints).

### The two `id` fields — DO NOT CONFUSE

Each resource block has two distinct `id` values:

```yaml
- resource: Microsoft.WinGet.DSC/WinGetPackage
  id: microsoft-powershell        # ← YAML-level anchor, for dependsOn references
  settings:
    id: Microsoft.PowerShell      # ← WinGet package id (the "real" id)
```

- The **YAML-level `id`** is a stable anchor for `dependsOn`. Derive from the package id: lowercase, dots → hyphens, no other special chars. Must be unique within the document.
- The **`settings.id`** is the literal WinGet package id passed to the package manager.

### Generic app — `Microsoft.WinGet.DSC/WinGetPackage`

```yaml
- resource: Microsoft.WinGet.DSC/WinGetPackage
  id: <yaml-anchor>
  directives:
    description: <DisplayName>
    allowPrerelease: <true|false>
  settings:
    id: <WinGet package id>
    source: winget               # or "msstore"
    Ensure: Present              # DSC convention; capitalized; values are Present | Absent
    UseLatest: true              # when entry.Version is null
    # OR — never both:
    Version: "1.2.3"             # when entry.Version is pinned
```

Critical:

- `Ensure` is **capitalized**, values `Present` | `Absent`. Do not lowercase.
- `UseLatest` and `Version` are **mutually exclusive**. Emit exactly one.
- v1 always emits `Ensure: Present` (Install/Upgrade/Ensure on the model side all collapse to `Present`; the model's distinction matters only for documentation and a future Absent verb).
- Omit `allowPrerelease` when false; include only when true.

### VS Code

VSCode itself uses `WinGetPackage`. Extensions install via a follow-up Script resource that depends on the VSCode resource:

```yaml
- resource: PSDscResources/Script
  id: vscode-extensions
  dependsOn:
    - microsoft-visualstudiocode    # the WinGetPackage YAML anchor
  directives:
    description: Install VS Code extensions
  settings:
    GetScript:  "return @{ Result = '' }"
    TestScript: "return $false"     # always re-run; idempotent because --install-extension no-ops if present
    SetScript: |
      $exts = @('ms-dotnettools.csharp', 'github.copilot')
      foreach ($e in $exts) {
        & code --install-extension $e --force
      }
```

`dependsOn` references the **YAML anchor**, not the WinGet id.

### Visual Studio

VS has its own DSC module: **`Microsoft.VisualStudio.DSC`**. Resource surface: `VSSetup` (install / edition / channel / vsconfig), `VSComponents` (workloads + components after install). **Module evolves quickly — verify resource names and supported settings at implementation time** via:

```powershell
Get-DscResource -Module Microsoft.VisualStudio.DSC
```

Minimal VS install via VSConfig:

```yaml
- resource: Microsoft.VisualStudio.DSC/VSSetup
  id: vs-pro-2022
  directives:
    description: Visual Studio 2022 Professional
  settings:
    productId: Microsoft.VisualStudio.Product.Professional
    channelId: VisualStudio.17.Release       # or VisualStudio.17.Preview
    configFile: C:\path\to\package.vsconfig  # required when configuring components
    # nickname / instance: as supported by the current module version
```

If the model has `VSConfigInline` set, the emitter writes that content to a temp `.vsconfig` next to the YAML and points `configFile` at it.

#### VSIX — reliable v1 approach

DSC resource coverage for arbitrary marketplace VSIXes is incomplete. Use a `PSDscResources/Script` resource that discovers VS via `vswhere` and loops through `VSIXInstaller.exe`:

```yaml
- resource: PSDscResources/Script
  id: vs-extensions
  dependsOn:
    - vs-pro-2022
  directives:
    description: Install VSIX extensions
  settings:
    GetScript:  "return @{ Result = '' }"
    TestScript: "return $false"
    SetScript: |
      $vsRoot = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
        -latest -property installationPath
      $installer = Join-Path $vsRoot 'Common7\IDE\VSIXInstaller.exe'
      $vsixes = @('<id-or-url-or-path-1>', '<id-or-url-or-path-2>')
      foreach ($v in $vsixes) {
        $p = Start-Process $installer -ArgumentList "/quiet /admin `"$v`"" -Wait -PassThru
        # exit 1001 means already installed — treat as success
        if ($p.ExitCode -ne 0 -and $p.ExitCode -ne 1001) {
          throw "VSIXInstaller failed for $v with code $($p.ExitCode)"
        }
      }
```

### Sources

- `winget` — public community repo (default).
- `msstore` — Microsoft Store source. Different license-acceptance flow; `winget configure` handles it but the user must accept agreements on first contact.

### Emitter hygiene

- No BOM in the YAML file. Write UTF-8 without BOM.
- Do not emit empty `directives:` or `settings:` blocks — omit the key.
- Do not emit `dependsOn` arrays for resources with no real dependency.
- YAML keys are case-sensitive in DSC. `Ensure`, `UseLatest`, `Version`, `Present`, `Absent` — preserve case exactly as documented above.
- Stable ordering: sort entries deterministically (by YAML anchor) so identical models produce byte-identical YAML — important for diffing and source control.

---

## `install.ps1` Companion Script

Emit alongside the YAML in the same output folder:

```powershell
#Requires -Version 7.0
[CmdletBinding()] param()

$ErrorActionPreference = 'Stop'
$here   = Split-Path -Parent $MyInvocation.MyCommand.Path
$config = Join-Path $here 'config.yaml'

# Verify winget
if (-not (Get-Command winget -ErrorAction SilentlyContinue)) {
    throw "winget not found. Install App Installer from the Microsoft Store and retry."
}

# Ensure required DSC modules
$modules = @('Microsoft.WinGet.DSC', 'PSDscResources')
# Emitter appends 'Microsoft.VisualStudio.DSC' if any VS resource is present in the YAML.

foreach ($m in $modules) {
    if (-not (Get-Module -ListAvailable -Name $m)) {
        Install-Module -Name $m -Scope CurrentUser -Force -AcceptLicense
    }
}

winget configure --file $config --accept-configuration-agreements --verbose
exit $LASTEXITCODE
```

The emitter mutates the `$modules` list at emit-time based on which resources are present in the YAML.

---

## Process Runner — Console Pane Specifics

The console pane streams output from `winget configure`. Required behavior:

1. **Async, line-buffered.** Use `Process.StandardOutput.ReadLineAsync` in a loop. **Never** `ReadToEnd()` — installs take minutes and the UI must remain responsive.
2. **UI thread marshaling.** Output writes must go through `Control.Invoke` / `BeginInvoke` or a captured `SynchronizationContext`. The reader runs on a `Task`, not the UI thread.
3. **ANSI escape handling.** `winget configure --verbose` emits cursor-control escapes for progress bars: `\x1B[2K`, `\x1B[?25l`, `\x1B[<n>;<m>H`, color escapes, etc. Strip with this regex before display:
   ```
   \x1B\[[\d;?]*[a-zA-Z]
   ```
   Or, optionally, parse colors into spans if colored output is wanted. v1 = strip is fine.
4. **Unbounded growth defense.** Do **not** use a plain `RichTextBox` that grows without bound — long installs produce thousands of lines and the control will stall. Use either a virtualized list/grid or a capped ring buffer (e.g. last 5000 lines, drop oldest).
5. **stderr handling.** Capture stderr separately. Tag stderr lines visually (dim red foreground / italic / "[err]" prefix — agent's call).

Process invocation:

```
winget configure --file "<path>" --accept-configuration-agreements --verbose
```

Smoke-test the YAML before running. `winget configure` supports subcommands like `show` and `test` for inspection / Test-phase dry-runs — check `winget configure --help` and use the appropriate one as the emitter's validation step. Do not assume `validate` exists; verify.

---

## Elevation

`winget configure` needs elevation for machine-scope installs (most of the catalog).

**v1: use `requireAdministrator` in the app manifest.** The whole app runs elevated. This is a single-user personal tool; do not over-engineer split-privilege models.

```xml
<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
```

---

## MVVM Architecture

- Create a dedicated NET10 class library which holds the ViewModels and the Business Logic with every App related WinGet Feature.
- Create Unit Tests for the ViewModels with fake WinGet runners.
- Use Warp.Toolkits DI and Warp.Toolkit UI Services for Dialog control.

### UI-Helper classes

- Not every Control in WinForms is suited for Binding in MVVM Fashion. For those cases, create adapter classes in the UI Project.
- Call the Forms/UserControls 'View'. E.g. FrmMainView (Form), DetailsView (UserControl)

## Layout (brief — WARP conventions apply)

- Top: MenuStrip
- Below: ToolStrip - use WARP ToolStrip skill for Icons.
- Below: Nested SplitContainer -- Left->TreeView node of packages->node of Apps->node of Extensions/Plug-ins, where it applies. Right->SplitContainer -- Top:WarpDataGridView with list of Apps of package/List of Extensions/Plug-ins where it applies->List of properties/settings of App/Extensions/Plug-in. Bottom (Panel2): Console control for debug and procces stdinout.
- `StatusStrip`. Status strip surfaces details of the current selection.
- VS and VSCode are the apps currently needing extension support.
- Menus: **File** (New, Open, Save, Save As, Export YAML+Script, Quit), **Edit** (Add App, Remove App, Properties), **Action** (Apply Now, Generate Bundle Folder), **Tools** (Options), **Help**.

---

## Settings File

`%APPDATA%\WingetPackageEditor\settings.json` — single JSON file, roamable via OneDrive Known-Folder Move:

```json
{
  "PackageStorePath": "%OneDrive%\\WingetPackages",
  "LastOpenedPackage": "...",
  "WindowState": { "Width": 1400, "Height": 900, "Maximized": true }
}
```

Resolve `%OneDrive%` and other environment variables at load time via `Environment.ExpandEnvironmentVariables`.

---
## Definition of Done - V0

WinForms and MVVM is still a work-approach to optimize. For that reason:
* We start building a Solution skeleton, with a minimum ViewModel wired up to
  - A few MenuItems and ToolStripButtons
  - The TreeView
  - The WarpDataGridView
  - The Console
  - The StatusStrip

The ViewModel should 
* contain the real base structure and already include the data model.
* Ensure the correct Command roundtripping (MenuItems, ToolStripButtons, Buttons)
* Ensure the proper functioning of Relay Commands.
* Correct propergation of ViewModels via the Control's DataContext Property
* Ensure the correct rountripping of the TreView
* Ensure the correct roundtripping of the WarpDataGridView
* Introduce a system to communicate (Messages?) with the Console control.
* Proof, that the ViewModel build for V0 scope is unit testable.

In AutoPilot, the V0 build task is done after those features have been created.
Further continuation with V1 needs exploritory testing by human interaction, and a manual new triggering for building the next V1 milestone.

## Definition of Done — v1

1. Create a new package, add 5 mixed entries (including a Visual Studio Professional entry with a VSConfig and 2 VSIXes, plus a VS Code entry with 3 extensions). Save as JSON.
2. Reopen the saved package — round-trips losslessly (assert via golden-file test).
3. Emit `config.yaml` + `install.ps1` to a folder; YAML is deterministic, BOM-free, UTF-8.
4. The emitted YAML loads without error via `winget configure show --file config.yaml` (or whichever inspection subcommand exists at implementation time).
5. "Run Now" executes `winget configure` end-to-end with live output streamed to the console pane; UI remains responsive; ANSI escapes are stripped; long output does not stall the UI.
6. App runs elevated via manifest; no per-action UAC prompts during normal use.

---

## Final Notes for the Implementing Agent

- **Verify before locking.** WinGet IDs, DSC module resource surfaces (`Microsoft.WinGet.DSC`, `Microsoft.VisualStudio.DSC`, `PSDscResources`), and the canonical schema URL change. Validate at implementation time against the actual installed environment; do not trust this document over a freshly-run `winget search` or `Get-DscResource`.
- **Engine-first build order is non-negotiable.** A working YAML emitter + process runner driven by a console host must exist before any WinForms code is written.
- **Determinism matters.** Two runs of the emitter on the same model must produce byte-identical YAML. Source control will thank the user.
- **No invented features.** v2 items (schedule, exe bundle, catalog editor) are not v1. If a v1 feature pulls in v2-shaped scope, stop and ask.
