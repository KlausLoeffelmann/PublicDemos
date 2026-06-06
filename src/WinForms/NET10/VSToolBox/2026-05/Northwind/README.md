# Northwind (Visual Studio Toolbox, May 2026)

The companion demo from the **May 2026 Visual Studio Toolbox** episode: a
WinForms .NET 10 app over the classic **Northwind** sample database via
**Entity Framework Core**.

## What's in here

- **`Northwind.DataLayer`** — EF Core data layer for Northwind
  (`NorthwindContext` + entity types). Includes a couple of seeded resources
  used by the data layer.
- **`Northwind.App`** — WinForms front-end (`FrmMain`). On startup it runs a
  small `TestDatabaseConnection()` smoke test against LocalDB and prints the
  first 10 customers to the Debug output, so a missing/misconfigured DB is
  caught immediately with a clear error dialog.
- **`Northwind.App/Copilot`** — the actual prompts used to drive the
  Copilot-assisted parts of the demo (`AppUserStoryPrompt.md`, `DTuning.md`),
  kept alongside the code so the episode is reproducible.

## Build / run

```powershell
dotnet build src\WinForms\NET10\VSToolBox\2026-05\Northwind\Northwind.slnx
```

Then run `Northwind.App`. Requires:

1. SQL Server **LocalDB** installed.
2. The **Northwind** database present.
3. A matching connection string in the data layer's configuration.

If the connection fails, the app shows an explanatory `MessageBox` instead of
crashing on the first query.
