# BranchComposer

A WinForms .NET 10 demo / tool for **composing a single working branch from a
chosen set of feature branches** on top of a fetched base branch — built on
top of the WARP Git services.

The typical workflow it's designed for: you have a `main` (or other base)
plus several in-flight feature branches, and you want to assemble a temporary
"branch set" that combines a chosen subset of them, replaying their commits
on top of the base. BranchComposer drives that end-to-end with a UI that
shows the repo state, the candidate branches, the composed set, and a live
Git console.

## What it does

- Lists local Git repositories (and GitHub repos via WARP services).
- Validates that each candidate branch is currently based on the fetched
  base branch before composing — refusing to operate on stale state.
- Composes a **branch set** by replaying commits onto the base. Conflict
  handling is **explicit** rather than blanket `-X theirs`, so cumulative
  append-only files aren't silently truncated.
- Surfaces all `git` commands and output in an embedded console view
  (`GitConsoleView`) for full visibility.
- Persists window/grid/splitter state via WARP's user settings service.

## Build

```powershell
dotnet build src\WinForms\NET10\BranchComposer\BranchComposer.slnx
```

Run `BranchComposer.App` from the solution to launch the UI.

## Project layout

```
BranchComposer.App/
├─ Program.cs                       # WinFormsApplication + DI bootstrap
├─ MainForm.cs                      # Repos / branches / branch-set UI
├─ BranchSetEditorDialog.cs         # Edit a single branch set
├─ BranchSelectionDataGridView.cs   # Candidate branches grid
├─ BranchSetDataGridView.cs         # Composed branch-set grid
├─ GitConsoleView.cs                # Embedded git output console
├─ Models/                          # AppState etc.
└─ Services/                        # GitConsoleService, AppStateStore, ...
```

## Dependencies

ProjectReferences into the local WARP toolkit (expected at
`..\..\..\..\..\WARP\src` relative to this folder):

- `WarpToolkit.WinForms`
- `WarpToolkit.WinForms.Extensions`
- `WarpToolkit.WinForms.AppServices`
- `WarpToolkit.WinForms.Github` (Git + GitHub services)
- `WarpToolkit.WinForms.Specialized`

## Design notes

UI behavior lives in the demo; reusable Git/composition behavior lives in
WARP. The app deliberately avoids approaches that would hide a Git conflict
or discard cumulative data (e.g. blanket cherry-pick `-X theirs`) — known-safe
append-only / resource-cleanup conflicts are automated, real code conflicts
are surfaced for the user to resolve.
