# Analog Rhythm Lab

A vanilla .NET 11 WinForms demo of procedural CR-78-style percussion, an editable
original score, and a reusable frequency-spectrum control from `WinForms.Audio`.
Stock playback requires no recorded drum samples or WARP dependency. Optional
user-imported WAVs are kept in portable sound presets, separately from scores.

The [development journal](DEVELOPMENT-JOURNAL.md) preserves the prompts, design
decisions, and sound/UI iterations leading to this version.

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

The editor strip and Edit menu change the score. **Add bar** inserts an empty bar
after the target bar on every track. **Delete bar** removes that bar from every
track; the last bar cannot be deleted. **Copy bar** inserts a duplicate, but only
tracks checked in **Use** are copied. Fill and clear honor **Current bar** or
**All bars** and skip unchecked tracks. Fill writes 16th, 8th, quarter, half, or
whole notes, clears the other steps in that scope, and keeps the velocity and gate
of hits that are already on the grid. The target bar is the selected step's bar,
or the first visible bar. Click the Use header, **All tracks**, or **No tracks**
to change every row. Eight named track-selection presets are application
preferences, not part of the loop file or Undo history.

**Play** starts or resumes, **Pause** holds the musical position, and **Stop**
releases sounds and resets to the beginning. The audio device and spectrum stay
running while paused, so audition still works. Score/tempo edits take effect at a
bar boundary. Volume controls target Master or an individual percussion channel
and also affect decaying tails. The shared metallic layer has its own enable
button and remembered amount.

The fixed playback baseline is **+12.04 dB** (`PercussionOutputGain = 4f` in
`WinForms.Audio\Percussion\Cr78VoiceBank.cs`), for both synthesized and imported
sounds. Dense mixes use peak-gain reduction with a 0.98 full-scale ceiling and
50 ms recovery to avoid hard clipping; below limiting, existing channel balance
and the master fader are unchanged apart from the fourfold baseline gain.

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
Version 2 loop files retain all seventeen fixed tracks. Version 1 files remain
readable: maracas/claves migrate to tambourine 2/guiro 2, and the new accent and
metallic-hi-hat tracks begin empty with default levels.

## Export a WAV for listening comparisons

**File -> Export WAV** renders the current bar, the complete loop, or a chosen
number of complete loops as **16-bit stereo PCM WAV**. The current bar is the
editor's target bar; full-loop export starts with bar 1 regardless of the live
playhead. The repetition count is independent of the document's Loop switch.

Exports use the current tempo, master/per-track levels, metallic-layer settings,
and imported sounds, including the current fixed output calibration and peak limiter.
No normalization, dither, background noise, or extra reverb is added. Both stereo
channels carry the same mono mix. The sample rate matches the audio engine when
available, otherwise 48 kHz. Stock noise is seeded for reproducible exports;
its exact waveform can differ from randomized live strikes.

Rendering is offline and does not record the audio device or change playback,
the loop document, or its unsaved state. Multiple loops render continuously:
decays and fractional timing carry across boundaries, rather than repeating a
separately rounded/clipped WAV chunk. A selected bar begins without the preceding
bar's tails. By default, the file ends at the exact musical endpoint. Enable
**Include final sound tail** to let the last sounds finish naturally.

The save dialog starts in the configured recordings folder. Export is limited
to 10,000 repetitions and one hour including any appended tail. Data is streamed
to a temporary file; a failed/cancelled export does not replace an existing file.

For the HDMI/TV/soundbar investigation, export several loops and play the WAV in
the same media player and output configuration as the comparison recording.
If attacks disappear there too, signal-dependent downstream muting becomes more
likely. If only live playback loses them, investigate its output path instead.
This test provides evidence; it does not by itself prove a particular noise gate.

## Recorded-sound presets

Select a percussion row and use **Import WAV** to replace only that track's
sound. **Synthesis** (Tools -> Reset current track to synthesis) restores its
algorithmic model. Importing never adds or
removes tracks, changes score events, or changes the eight track-selection presets.
PCM 8/16/24/32-bit and 32-bit floating-point WAVs are supported, downmixed and
resampled before playback. Individual imports are limited to 30 seconds/64 MiB;
malformed or unsupported files report an error without replacing the current kit.

**Load sounds / Save sounds** (also in Tools as Load/Save sound preset) use `.drumkit.json` manifests with
adjacent WAV asset folders. Save copies the original WAV bytes, validates the
copies, then atomically publishes relative paths. Move the manifest **and its
asset folder together** to transfer a preset. Stock-only tracks need no files.
Saving over a preset creates a new asset generation; older asset folders are
retained rather than risking deletion of files used by another copy of a kit.
Sound mappings are independent of musical Undo/Redo and loop files.

## Options and symbol icons

Tools -> Options provides Classic/Dark mode/System, default loop, recordings and
sound-preset folders,
Small (32x32), Medium (48x48), or Large (64x64) toolbar icons, and four relative
font sizes. Small keeps the current WinForms standard; Normal, Large, and XXL add
2, 4, or 6 points without changing the selected font family or style. Icon and
folder changes apply immediately; theme and font changes apply after restart.
Icon sizes are at 96 DPI and scale per monitor. System reads the Windows color
mode at launch.

`SymbolIconFactory` renders installed Segoe Fluent Icons glyphs into transparent
bitmaps at their target size, with an explicit Segoe MDL2 Assets fallback.
No WARP symbol library, downloaded font, or enlarged raster icon is involved.
The toolbar owns/replaces its images; normal menu icon sizes remain independent.

The WAV picker remembers its last successful import folder across launches.
The double-buffered track grid uses restrained family/beat accents in Classic
and Dark modes and respects Windows high-contrast colors.

Preferences, recent files, the eight track-selection presets, window placement,
splitter position and column settings live in
`%LocalAppData%\DrumMachine.Demo\settings.json`, not inside loop documents.

The stock palette matches the seventeen supplied sound names, in musical order:
kick/accented kick, snare/accented snare, rim shot, hi-hat/accented hi-hat/metallic
hi-hat, cymbal, high/low bongo, low conga, cowbell, tambourine 1/2 and guiro 1/2.
The independent cymbal/hi-hat metallic layer remains available as well.
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

The control is `WinForms.Audio.WinForms.AudioSpectrumControl`, in the existing
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

The new stock algorithms fit the temporary WAVs' damped resonances, pitch drift,
noise coloration, amplitude envelopes and scrape rates. Only compact parameters
are embedded; removing `Sounds` does not affect playback. Accent variants are
independent fits rather than simple gain changes.

Both kicks retain the fitted attack, then use a single fixed 60 Hz body with
a short exponential decay. The kick/accent finish at 160/140 ms, without the
previous detuned, wavering sustain or a reverb-like tail.

The offline tools in `DrumMachine.Analysis` regenerate models and compare actual
.NET renders with the source WAVs. Spectral/temporal regression measurements are
not a claim of perceptual, transistor-level or bit-exact equivalence.
Implementation details are documented in `WinForms.Audio\Percussion\README.md`.
