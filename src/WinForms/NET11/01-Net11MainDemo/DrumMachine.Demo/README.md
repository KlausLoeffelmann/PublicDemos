# Analog Rhythm Lab

A vanilla .NET 11 WinForms demo of procedural CR-78-style percussion, an editable
original score, and a reusable frequency-spectrum control from `SplitFlap.Audio`.
It contains no recorded drum samples, commercial song transcription, or WARP dependency.

## Run

From the solution directory:

```powershell
dotnet run --project .\DrumMachine.Demo\DrumMachine.Demo.csproj -c Release
```

Use **Play/Stop**, **Loop**, tempo, and master volume for transport. Select a bar
and toggle its 16 steps with the mouse or Space. Each instrument row has a Play
button for audition; the metallic layer also has a separate audition button.
Changing the visible bar does not seek the audio. Score and tempo edits are
applied at a bar boundary; Reset pattern restores the original two-bar score.
Edits are in-memory and intentionally do not overwrite a settings file.

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
| `all` | Run all three scenarios. |

Exit codes are 0 for success, 1 for failure or an interrupted requested scenario,
and 2 for invalid arguments. A short timer is not a successful scenario merely
because it closed the window. No scenario failure opens a blocking error dialog.
`--run-for` without a scenario simply closes an interactive session normally.

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
