# Analog Rhythm Lab

A vanilla .NET 11 WinForms demo of procedural CR-78-style percussion, an editable
original score, and a reusable frequency-spectrum control from `SplitFlap.Audio`.
It contains no recorded drum samples, commercial song transcription, or WARP dependency.

## Run

From the solution directory:

```powershell
dotnet run --project .\DrumMachine.Demo\DrumMachine.Demo.csproj -c Release
```

The MenuStrip and ToolStrip share New/Open/Save commands. **New** asks for a blank
1-, 2-, or 4-bar loop. **View -> 1 Bar / 2 Bars** only changes the visible range;
the range selector navigates longer scores without changing their length or
seeking playback. Toggle steps with the mouse or Space. Each row retains its
instrument audition button.

**Play** starts or resumes, **Pause** holds the musical position, and **Stop**
releases sounds and resets to the beginning. The audio device and spectrum stay
running while paused, so audition still works. Score/tempo edits take effect at a
bar boundary. Volume controls target Master or an individual percussion channel
and also affect decaying tails. The shared metallic layer has its own enable
button and remembered amount.

## Loop documents and editor history

Open and save versioned `.drumloop.json` files containing track definitions,
hit velocities/gates, tempo, master and percussion levels, Loop, and metallic
enable/amount. Save As changes the document path only after success. File writes
use a temporary file and atomic replacement; invalid files do not replace the
current loop. New/Open/Quit protect unsaved changes.

Undo/Redo covers musical document changes, including mixer settings, with one
history entry per slider gesture. Saving keeps history; undoing back to the saved
state clears the title's unsaved marker. Transport, viewport, and app options are
not document edits. File -> Recent lists the last five successful opens/saves.
`Examples\OriginalBallad.drumloop.json` contains the original demonstration groove.

## Options and symbol icons

Tools -> Options provides Classic/Dark mode/System, a default loop-file folder,
and Small (32x32), Medium (48x48), or Large (64x64) toolbar icons. Sizes are at
96 DPI and scale per monitor. Icon and folder changes apply immediately; theme
changes require a restart. System reads the Windows color mode at launch.

`SymbolIconFactory` renders installed Segoe Fluent Icons glyphs into transparent
bitmaps at their target size, with an explicit Segoe MDL2 Assets fallback.
No WARP symbol library, downloaded font, or enlarged raster icon is involved.
The toolbar owns/replaces its images; normal menu icon sizes remain independent.

Preferences and recent files live in
`%LocalAppData%\DrumMachine.Demo\settings.json`, not inside loop documents.

The stock palette comprises 11 primary voices, tambourine and guiro, and a
metallic layer associated with cymbal/hi-hat. This models the sound character,
not the original machine's programmer or four-track memory restrictions.
The score is original demo material, not a manufacturer's preset or a named song.
The kit plays dry by default.

## What the spectrum shows

The spectrum reads final output PCM, after mixing, master level, and clipping.
It is not a second oscillator pretending to show the audio. A separate worker
performs Hann-windowed FFT analysis; the control only copies and paints complete
snapshots. Capture and visualization cannot make the audio producer wait.

With WinMM, the analysis window follows completed output buffers instead of the
newest rendered-ahead block. Buffer granularity, the FFT window, UI refresh, and
the device still add latency; this is not a sample-exact acoustic measurement.
A custom sink without a playback clock is explicitly identified as submitted
audio rather than played audio.

The control is `SplitFlap.Audio.WinForms.AudioSpectrumControl`, in the existing
audio assembly. That assembly now references the WinForms framework but adds
no external DSP package. The runtime Source property is not Designer-serialized,
and opening a form in the Designer does not open an audio device.

## Timed scenarios

```powershell
dotnet run --project .\DrumMachine.Demo\DrumMachine.Demo.csproj -c Release -- --scenario spectrum --run-for 8
dotnet run --project .\DrumMachine.Demo\DrumMachine.Demo.csproj -c Release -- --scenario all --run-for 30
```

| Scenario | Purpose |
|---|---|
| `kit` | Audition all instruments and the metallic sound. |
| `score` | Play the original two-bar score once. |
| `spectrum` | Confirm that a known output tone reaches the playback-aligned analyzer. |
| `document` | Exercise file round-trip, Undo/Redo, bar views, Pause/Resume, and Stop/Reset without dialogs. |
| `all` | Run all scenarios. |

Exit codes are 0 for success, 1 for failure or an interrupted requested scenario,
and 2 for invalid arguments. A short timer is not a successful scenario merely
because it closed the window. No scenario failure opens a blocking error dialog.
`--run-for` without a scenario simply closes an interactive session normally.
`--no-settings` bypasses preference and recent-file I/O. Automated scenarios also
isolate user preferences automatically and use temporary files, never the user's loops.

Logs are in `%LocalAppData%\DrumMachine.Demo\Logs`, separate from the split-flap
demo's logs. If another demo instance locks the usual build output, use a separate
MSBuild `OutDir` rather than terminating that instance.

## Model fidelity

The procedural models use original Roland specifications and service-note
frequency/decay targets where available, with model-specific approximations
documented alongside `SplitFlap.Audio\Percussion`. They are not a claim of
transistor-level or bit-exact hardware emulation. The original adjustment table's
decay definition matters; it is not automatically an exponential time constant.

Reference material:

- [Roland CR-78 specifications](https://support.roland.com/hc/en-us/articles/201934399-CR-78-Technical-Specifications)
- [Original service notes](https://archive.org/details/synthmanual-roland-cr-78-service-notes)

Any later reference recordings must be lawfully usable and remain outside the
repository. They may inform measured parameters, not become bundled sound assets.
