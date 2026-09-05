---
name: splitflap-test-harness
description: Run and extend the SplitFlap.Demo xUnit v3 tests and timed command-line smoke scenarios. Use this when verifying changes, reproducing startup/display/audio failures, or adding a new automated demo scenario.
---

# Split-Flap Test Harness

## Use this skill when

- validating a change to `SplitFlap.Controls`, `WinForms.Audio`, or `SplitFlap.Demo`;
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

## Opt-in audio performance measurements

The existing xUnit v3 runner includes an explicit, device-independent performance workload:

```powershell
dotnet run --project .\SplitFlap.Tests\SplitFlap.Tests.csproj -c Release --no-restore -- --filter-class SplitFlap.Tests.AudioPerformanceTests --explicit only --parallel none --show-live-output on --timeout 180s
```

It is excluded from ordinary runs. No benchmark package, audio endpoint, or unsafe code is
needed. Run without a debugger and compare the same machine/configuration.
If a running demo locks its output DLLs, build with an isolated
`-p:OutDir=<directory>` and run `SplitFlap.Tests.exe` from that directory rather than
closing the user's demo instance.

- `MeasurePump` warms all paths and reports three rounds with fixed seeded input and block counts.
- Workloads cover idle output, a sine, twelve-clack board bursts at nominal 60 Hz, a mixed board
  and melody, 32/64-clack stress, 64 sines, and a hall tail.
- `MeasureLongIdle` additionally measures room and hall output after 64 simulated seconds with
  no new strikes. This catches subnormal feedback costs that fresh-silence measurements miss.
- `MeasureRhythm` covers the reusable drum player with monitoring off/on and a concurrent
  spectrum worker, plus a stopped player's idle cost. It runs the actual pump without a
  device or UI; production kit variation is retained, so these are timing observations,
  not bit-exact sample comparisons or separate analyzer CPU measurements.
- `PERF` lines contain JSON with mean/p95/p99/maximum block-render elapsed time, render time
  per simulated audio second, bytes allocated per render block and per admitted voice,
  process-wide Gen0 counts, and a PCM checksum.
- The measuring sink neither copies PCM nor sleeps during rendering. It times the real pump
  between writes, excluding its own synthetic scheduling. Producer allocations are measured
  separately; this fixture schedules on the pump thread, not on a real concurrent animator.
- The timings are elapsed render intervals, not isolated CPU counters or speaker latency.
  They do not measure native WinMM copy/wait cost, producer contention, or driver buffering.
  Other work in the test process can contribute to Gen0 counts and scheduling outliers.
- There are no machine-dependent timing assertions. Keep ordinary regression tests deterministic.
  Reference-equation tests cover DSP output; a checksum is only a useful coarse comparison.
- `Span<T>` alone is not a performance result. Record repeatable before/after numbers and
  retain more complex or sound-changing techniques only when their benefit is substantial.

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

## Rhythm demo

The second application, `DrumMachine.Demo`, uses the same library and its spectrum control:

```powershell
dotnet run --project .\DrumMachine.Demo\DrumMachine.Demo.csproj -c Release -- --scenario spectrum --run-for 8
dotnet run --project .\DrumMachine.Demo\DrumMachine.Demo.csproj -c Release -- --scenario all --run-for 30
```

Its scenarios are `kit`, `score`, `spectrum`, `document`, and `all`. The kit scenario auditions
all percussion entries; score plays the original pattern once; spectrum checks
that a known output tone reaches the playback-aligned analyzer. A deadline or
manual close before a requested scenario completes is a failure, not a successful
test merely because the process exited.

The document scenario round-trips a temporary loop, exercises Undo/Redo and bar views,
and checks Pause/Resume and Stop/Reset. The editor saves `.drumloop.json` documents;
app preferences and five recent files use its own AppData `settings.json`.
`--no-settings` bypasses preference/recents I/O; automated scenarios also bypass it
automatically. Never let a scenario overwrite the user's loop files. Its separate logs are
`%LocalAppData%\DrumMachine.Demo\Logs\drummachine-yyyyMMdd.log`.
Keep original groove and model provenance documented; do not add recordings or
commercial song arrangements as test fixtures.

Use fake sinks and controlled playback-progress sources for sample timing, FFT
normalization, post-gain PCM, capture gaps, slow-reader behavior, and disposal.
Opening the rhythm form or spectrum control for a layout test must not open a
real audio endpoint; run WinForms layout/painting work on an STA thread.

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
