---
name: splitflap-test-harness
description: Run and extend the SplitFlap.Demo xUnit v3 tests and timed command-line smoke scenarios. Use this when verifying changes, reproducing startup/display/audio failures, or adding a new automated demo scenario.
---

# Split-Flap Test Harness

## Use this skill when

- validating a change to `SplitFlap.Controls`, `SplitFlap.Audio`, or `SplitFlap.Demo`;
- reproducing a failure that happens only after the WinForms message loop starts;
- adding a repeatable smoke path for a new stage feature;
- inspecting AppData logs from an automated run.

## Unit tests

Run the complete .NET 11 xUnit v3 project from the demo folder:

```powershell
dotnet test .\SplitFlap.Tests\SplitFlap.Tests.csproj -nologo
```

The test project is an xUnit v3 executable integrated with Microsoft Testing Platform. Keep tests deterministic. Do not require speakers, an audio endpoint, a particular monitor, installed optional fonts, or persisted user settings.

Use a fake `IAudioSink` for engine tests. A fake sink controls pacing, captures PCM, and can deliberately throw to verify pump error propagation.

## Command-line smoke runs

The WinForms executable accepts:

| Option | Meaning |
|---|---|
| `--scenario display` | Update the board, animate characters, force a jam, and exercise display behavior. |
| `--scenario sound` | Open the default audio endpoint and play a short deterministic melody. |
| `--scenario all` | Run display and sound scenarios in one process. |
| `--run-for <seconds>` | Close after a positive duration no greater than 3600 seconds. |
| `--no-settings` | Do not load or save the user's settings. |

Example:

```powershell
dotnet run --project .\SplitFlap.Demo\SplitFlap.Demo.csproj --no-build -- --scenario all --run-for 3 --no-settings
```

The first `--` belongs to `dotnet run`; options after it go to the demo.

## Results and logs

- Exit code `0`: startup and the selected scenario completed successfully.
- Exit code `1`: the scenario or application failed; inspect the log.
- Exit code `2`: invalid command-line input; usage is written to standard error.

Logs are in:

```text
%LocalAppData%\SplitFlap.Demo\Logs
```

The daily file is named `splitflap-yyyyMMdd.log`. Search the final run for `ERROR` or `CRITICAL`, then use its stack trace and category to identify the failing layer.

## Extending the harness

1. Add a scenario value only when the behavior needs the WinForms message loop or real platform integration.
2. Parse it strictly in `StartupOptions`; reject missing/unknown values.
3. Start it from `MainForm` only after `OnLoad` and handle creation.
4. Honor the lifetime cancellation token.
5. Log scenario start and completion.
6. Set a nonzero process exit code on failure.
7. Add parser tests and narrow unit tests for extracted logic.
8. Preserve normal no-argument interactive startup.
