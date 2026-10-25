# Analog Rhythm Lab - Development Journal

**Checkpoint: October 24, 2026**

Project: `src\WinForms\NET11\01-Net11MainDemo\DrumMachine.Demo\DrumMachine.Demo.csproj`

This records the prompts, implementation approach, technical decisions, course
corrections, and results of this iteration. It starts with the existing drum
machine, not an empty project, and ends at the version described as "almost
PERFECT" in the request to preserve this history.

The prompts below preserve the user's wording. The engineering account
summarizes the approaches and their rationale rather than presenting a literal
transcript of every tool call. Historical measurements and checks are identified
separately from those performed after the final audio changes.

## 1. The starting point

The application already had a WinForms rhythm editor, procedural percussion,
score playback and audition, channel/master mixing, loop documents, and an
output-spectrum display. The existing percussion catalog and sound-generation
algorithms were the starting point.

The aim was to improve that application without replacing its musical workflow:
give the fixed tracks meaningful sounds and names, make the UI more usable,
support personal recorded-sound kits, remember the working environment, and
bring the synthesized stock sounds closer to the supplied examples.

Existing working-tree changes were retained. In particular, the pre-existing
bar editor and other earlier audio/editor work were not treated as newly created
features of this iteration.

## 2. The prompts

### Prompt 1 - Expand the sounds, UI, presets, and persistence

```text
I want to add a few features to the Drum machine in @src\WinForms\NET11\01-Net11MainDemo\DrumMachine.Demo\DrumMachine.Demo.csproj .

Tenet: For the UI Work, please spin-up Grok 4.7 as the Agent in charge.

* We have added a series of WAV files in @src\WinForms\NET11\01-Net11MainDemo\DrumMachine.Demo\Sounds\. These are temporary, and will not stay. But - I want you to use them
  - For setting up a new Percussion list, which should equal these percussion sounds (just the naming) in a meaning full order, and de-abbrivate them, if they were.
  - I figure, we're using a DataGridView for the track visualization. Please make sure, we are sub-classing the DataGridView, enabling double buffering, and are coming up with bit more colorful (not Japanese-Webpage-colorful, though!) implementation of the UI. Please take DarkMode and Classic mode for the theming into account.

* Allow the creation of new Percussion presets by adding/import of real WAV-Files for a particular Percussion track. Keep in mind: We are NOT allowing to add or remove any tracks here. We are just allowing to map to different presets based on additional wav files the user can add, and the user can then also save those presets along with the important WAV files to a Preset folder.

* In Tools/Option, let us define default paths for the folder to the recordings and the folder to the presets. When the user important wav files, please let's remember where the files came from, and let's open the same folder by default the next time they want to import wav files.

* Can you make sure, the Window position, Splitter and column settings are serialized on program end, and deserialized on start?

* And now the real work:
  Please, Analyze the Wav Forms of the @src\WinForms\NET11\01-Net11MainDemo\DrumMachine.Demo\Sounds\ folder, and try to come up with an algorthmic way to mimic the sound as close as possible. That - change the existing algorithms out and replace them by the algorithms you figured out. This way, we also then have a close match to the new list of stock percussion sounds.
 To that end, feel free to use extra budget to create a command line tool for WAV file analysis, should that be helpful, so that coming up with an algorithm for that particular sound is easier or/and more precise. Alternatively, add those analysis functionality to the App itself, and introduce command line args to call the app from the command line - your choice. Also, feel free to consult with Gemini 4 or Grok 4.7 for this task.
```

### Prompt 2 - Investigate the quiet playback

```text
Can you please check, why the playback of the samples is pretty quiet?
Even if the master volume is 100% and then the played samples are still like 50%, if at all, of the volume of other sounds, like, the notification sounds.

Is there a baseline value somewhere defined as a const, which we could pick to adjust the baseline but still have it staticly coded?
```

### Prompt 3 - Raise the baseline and correct the kicks

```text
OK, then let's make that change, so that we start at around +12db.

In that context, could you please overthink the algorithm for the kicks? They are starting comparatively well - but the sustain/decay with that vibrato sounds completely off. Both sound are more or less immediatly over and actually do not have any reverb-like effects whatsoever.
```

### Prompt 4 - Preserve this iteration

```text
Could you please preserve the iteration to this point, by creating a Markdown document, which contain my prompts, and your approaches in thinking and setting up the app from the version we had to the almost PERFECT version, we got now?
```

## 3. First implementation pass: separate the responsibilities

The work was divided along the app's existing boundaries:

| Area | Approach |
|---|---|
| UI and preferences | Grok 4.7 led the grid, theme, commands, options, and layout-persistence work, as requested. |
| Audio and measurement | The coordinating assistant implemented the sound models, analysis tools, audio preparation, and sample handoff. |
| Portable sound presets | Keep file storage and validation separate from both the UI and the real-time renderer. |
| Integration | Reconcile the expanded catalog with enum compatibility, score indexing, faders, documents, settings, and tests. |

This allowed the UI work and audio/preset work to proceed independently before
integration. Grok 4.7 was actually used for the UI work; the permission to consult
other models for synthesis did not imply that Gemini was also used.

### A fixed palette, not a track-management feature

The seventeen supplied sound names became seventeen fixed tracks in musical
order, with abbreviations expanded:

| Family | Track order |
|---|---|
| Drums | Kick drum, accented kick drum, snare drum, accented snare drum, rim shot |
| Hats and cymbal | Hi-hat, accented hi-hat, metallic hi-hat, cymbal |
| Tuned percussion | High bongo, low bongo, low conga, cowbell |
| Shaken and scraped | Tambourine 1, tambourine 2, guiro 1, guiro 2 |

Importing a WAV changes the sound assigned to one of these tracks. It does not
create or delete tracks, alter score events, or redefine the existing
track-selection presets.

Existing enum values were preserved rather than renumbered. New sounds were
appended, while legacy maracas/claves references migrate to tambourine 2/guiro 2
in demo documents and settings. The existing auxiliary metallic layer remains
separate from the metallic-hi-hat track.

### A more polished grid without abandoning WinForms conventions

`BeatGridView` subclasses `DataGridView` and enables double buffering. Restrained
instrument-family and beat accents provide more visual structure without
turning the grid into an overly colorful display.

Classic, Dark, and high-contrast behavior were taken into account. UI behavior
stays in regular code files; Designer-compatible declarations remain in the
Designer files. Sound commands and options integrate with the existing editor
rather than introducing a second workflow.

Window placement, splitter position, and column settings are persisted and
restored with DPI-aware handling. Heavy player preparation is asynchronous so
that constructing/showing the form does not require blocking audio preparation.

### Three kinds of saved state

Keeping these separate was an important design choice:

| State | Storage and responsibility |
|---|---|
| Musical loop | `.drumloop.json`: score, tempo, levels, and musical playback settings. |
| Sound preset | `.drumkit.json` plus adjacent WAV assets: mappings from fixed tracks to recorded sounds. |
| Application preferences | `settings.json`: default folders, remembered import folder, theme, layout, and other app preferences. |

Loop documents advanced to version 2 while retaining version 1 migration.
Preferences advanced to schema version 4.

The WAV picker remembers its last successful import location. Tools -> Options
provides default recordings and sound-preset folders. Sound-preset changes have
their own unsaved-change protection instead of being confused with score edits.

### Portable presets must own their dependencies

Saving only absolute paths to the original imports would produce fragile kits:
they would stop working after the user moved or deleted those files.

Instead, saving a sound preset copies the original WAV bytes into an adjacent
asset folder and publishes a manifest with relative paths. The copies are
validated before the manifest is atomically replaced.

A failed save preserves the existing preset and removes only assets owned by
the unsuccessful save. Older published asset generations are retained rather
than risking deletion of files used by another copy of a kit. Portability means
moving the manifest and its asset folder together.

Imports support PCM 8/16/24/32-bit and IEEE float32 WAVs, including supported
extensible headers, multichannel downmixing, and sample-rate conversion. Per-track
imports are bounded to 30 seconds and 64 MiB, with explicit failures for invalid
or unsupported input.

Preparation and defensive PCM copies happen off the rendering thread. A complete,
immutable mapping is admitted at the next audio block; existing tails keep their
source, and unmapped tracks revert to stock synthesis.

## 4. Deriving stock sounds without shipping the reference recordings

The temporary WAVs were measurement inputs, not runtime dependencies.

An offline Python tool using NumPy and SciPy fits compact sound parameters.
A companion .NET command-line project renders the actual stock synthesis and
validates WAVs through the same decoder used for imports.

The general model combines damped resonances with spectrally colored noise:

```text
sum(amplitude * exp(-t / decay) *
    cos(phase + 2*pi*(frequency*t + chirp*t*t/2)))
+ colored_noise(t) * RMS_envelope(t) * scrape_modulation(t)
```

The fit captures resonant frequencies, decay constants, pitch drift, analytic
mode amplitudes/phases, noise coloration, and amplitude envelopes. Guiros also
need scrape timing and pulse shape. Their first envelope comparison exposed
overly narrow scrape pulses, so the guiro models were fitted again rather than
accepting the first plausible result.

Only parameters are embedded in `MeasuredModels.json`: no source PCM, recorded
noise, or recorded FFT phases. The runtime synthesizes in-memory one-shots before
audio admission. Two prepared strike states and the existing 5 ms
retrigger/release handling keep the audio path allocation-free per hit.

Measured anchors included approximately 60 Hz for the kick, 308 Hz for the snare
body, 1408 Hz for the rim shot, 590/389 Hz for the bongos, 196 Hz for conga, and
546/775 Hz for cowbell.

### Integration issues that needed real fixes

Expanding the palette was not just a matter of adding names:

- Score indexing still used a hardcoded stride of 13. New enum values could
  collide with the following step's keys; the stride was corrected to cover the
  complete enum range, currently 20.
- Stable enum IDs no longer matched the displayed track/fader order. Explicit
  kit-index mapping replaced assumptions that an enum value was a fader index.
- Older tests and fixtures assumed thirteen primary tracks and needed to reflect
  the expanded fixed palette.
- Floating-point resampling-length arithmetic could turn 480 frames at 48 kHz
  into 440 rather than 441 frames at 44.1 kHz. Integer-rational length arithmetic
  fixed the off-by-one instead of weakening the expected result.

### What the first measurements established, and what they did not

Before the later kick revision, fixed-seed .NET renders compared with all
seventeen references achieved:

| Measurement | Initial comparison |
|---|---|
| Smoothed spectral-band cosine similarity | 0.981-1.000 |
| RMS-envelope cosine similarity | 0.920-1.000 |
| Spectral centroid deviation | Within 12% |
| T90 deviation | Within 18% |

T90 is the time containing ninety percent of the sound's energy. These
measurements describe spectral and temporal shape, not perceived likeness.
They did not prove that the sounds were perceptually correct. The next listening
feedback, particularly about the kicks, was an important correction to that
initial assessment.

## 5. Second pass: explain the low playback level

The quiet-playback report prompted an investigation before changing anything.
The master control was not secretly capped at fifty percent: the demo sets the
engine master to unity, and a UI master of 100% means unity gain.

The attenuation was already inside the percussion channels:

```csharp
_level = preset.Level * (Cr78Kit.Instruments.Contains(instrument) ? 0.63f : 1f);
```

For example:

| Sound | Channel amplitude bound before velocity and faders | Relative to full scale |
|---|---|---|
| Kick | `0.140 * 0.63 = 0.0882` | About -21.1 dBFS |
| Hi-hat | `0.050 * 0.63 = 0.0315` | About -30 dBFS |

These are bounds, not measured RMS loudness or guaranteed peak levels.
Imported WAVs pass through the same channel calibration, so replacing synthesis
with a recording did not bypass the attenuation.

The recommendation was to preserve the established relative instrument balance
and introduce one named, statically coded gain at the final percussion mix.
This diagnostic pass did not itself change the code. The following prompt
approved the proposed approximately +12 dB increase.

## 6. Third pass: louder playback and short, dry kicks

### One explicit baseline, with overload protection

The player now defines:

```csharp
internal const float PercussionOutputGain = 4f;
internal const float OutputPeakCeiling = 0.98f;
```

Fourfold amplitude gives `20 * log10(4)`, approximately **+12.04 dB**.
This is not a claim of fourfold perceived loudness.

The calibration is applied after channel and master faders, at the
`Cr78VoiceBank` output. It covers stock sounds, imported samples, and the
auxiliary metallic layer without retuning each instrument or changing the
shared audio engine.

Simply multiplying and letting PCM conversion hard-clip dense patterns was
rejected. A lightweight sample-peak limiter instead applies immediate gain
reduction when needed and recovers exponentially with a 50 ms time constant.
Its 0.98 ceiling leaves a small PCM conversion margin. Limiter state uses
double precision so recovery does not stall short of unity at higher sample
rates.

Below limiting, the output is exactly fourfold; overload protection can reduce
that gain during dense passages. Muting remains exact silence, and rendering
does not acquire per-hit allocations.

Standalone `Cr78Kit.CreateVoice` sounds and the analysis exports intentionally
retain raw channel calibration. The gain and limiter belong to the player mix,
so a loudness adjustment does not invalidate timbre-analysis comparisons.

### Why the original kick fit could sound wrong

Both kicks used four fitted modes plus a colored-noise residual. Some modes
had long decay constants and different pitch drifts. Even quiet late components
could interfere, producing beating and a wandering tail. There was no reverb
processor to switch off: the reverb-like impression came from the model.

The reference files were no longer present by this point. The revision therefore
used retained measurements, the earlier modal parameters, and the user's
listening feedback. It was not presented as a new comparison with unavailable
original WAVs.

### Preserve the attack; replace the problematic body

The initial attack was already described as good. Rather than discard it, the
revised model keeps the fitted attack and crossfades from 4-12 ms into a single
fixed-frequency body:

| Property | Kick | Accented kick |
|---|---|---|
| Body frequency | 60 Hz | 60 Hz |
| Exponential decay time constant | 43 ms | 37 ms |
| Complete strike duration | About 160 ms | About 140 ms |
| Final fade | 12 ms | 12 ms |

After the attack transition, no detuned/chirped modes or random bass residual
remain. The body has a stable pitch and monotonically decreasing peaks, followed
by a smooth end rather than extended ringing.

Body amplitude and phase were projected from the retained early modal fits.
The offline fitter was updated to generate the same dedicated kick-body
structure, preventing later regeneration from silently restoring the old tail.
Regeneration also accounts for source clips shorter than the generated body.

Final measurements from the actual .NET exports were:

| Measurement | Kick | Accented kick |
|---|---|---|
| T90 | 52.6 ms | 44.8 ms |
| Retained original reference T90 | About 53 ms | About 45 ms |
| Spectral centroid | 90.6 Hz | 107.0 Hz |
| Retained original reference centroid | About 92 Hz | About 108 Hz |

The useful early energy characteristics survived, while the objectionable late
behavior was removed. Those scalar checks support the change; they do not
replace the user's listening judgment.

## 7. Verification at the milestones

The evidence belongs to two distinct stages:

| Stage | Recorded result |
|---|---|
| First feature integration | Full suite: 534 passed, 3 skipped, 537 total. |
| First feature integration | Release solution build: zero warnings and errors. |
| UI pass | Classic/Dark checks and simulated 96/144/192 DPI scaling; not physical multi-monitor testing. |
| Final gain/kick revision | 90 targeted mixer, synthesis, stock-model, and sample-preset regressions passed. |
| Final gain/kick revision | 60 sequencing and output-integration regressions passed. |
| Final gain/kick revision | Release solution build: zero warnings and errors. |
| Final gain/kick revision | Actual app was responsive and `--scenario all --run-for 35 --no-settings` exited 0. |
| Final gain/kick revision | Actual .NET kick exports and Python fitting/rendering checks passed. |

The final 150 targeted checks are not described as another full-suite run.
They cover exact fourfold gain below limiting, overloaded positive/negative
sample mixes, limiter recovery across sample rates, exact mute, existing
faders/transport behavior, and rendering allocations. Kick-specific checks
cover short duration, bounded late energy, stable 60 Hz zero crossings, and
declining peaks at 32/48/96/192 kHz.

The original all-seventeen-reference similarity ranges remain historical:
the later kick design changed, and the source WAVs are gone.

## 8. Where the implementation lives

Paths here are relative to `src\WinForms\NET11\01-Net11MainDemo`.

| File or area | Responsibility |
|---|---|
| `DrumMachine.Demo\Controls\BeatGridView.cs` | Buffered, themed track grid. |
| `DrumMachine.Demo\MainForm.Sounds.cs` | Import, reset, load/save, and sound-preset workflow. |
| `DrumMachine.Demo\MainForm.Layout.cs` | Window, splitter, and column restoration/persistence. |
| `DrumMachine.Demo\AppSettings.cs`, `AppSettingsStore.cs`, `OptionsDialog.cs` | Application preferences and default folders. |
| `DrumMachine.Demo\Documents\PercussionSoundPreset.cs`, `PercussionSoundPresetStore.cs` | Sound mappings and portable preset storage. |
| `WinForms.Audio\Percussion\Cr78Kit.cs` | Fixed catalog and explicit instrument indexing. |
| `WinForms.Audio\Percussion\Cr78Generator.cs` | Prepared strikes, source selection, and retrigger/release continuity. |
| `WinForms.Audio\Percussion\MeasuredModels.json`, `MeasuredPercussionModel.cs` | Parameterized stock synthesis and dedicated dry kick bodies. |
| `WinForms.Audio\Percussion\PercussionSampleSet.cs` | Validated, owned sample mappings. |
| `WinForms.Audio\Percussion\Cr78VoiceBank.cs` | Channel mixing, static output baseline, and peak protection. |
| `WinForms.Audio\Sequencing` | Block-boundary mapping/configuration handoff and sample-clock sequencing. |
| `WinForms.Audio\Playback\Sample.cs` | WAV decoding, downmixing, and resampling. |
| `DrumMachine.Analysis` | Offline fitting, actual .NET renders, decoder validation, and comparison tools. |
| `SplitFlap.Tests` | Existing and added regression coverage for this solution. |

For normal use, see [the app README](README.md). The sibling
`WinForms.Audio\Percussion\README.md` and `DrumMachine.Analysis\README.md`
explain the synthesis and analysis interfaces in more detail.

## 9. What this checkpoint preserves

The result is the existing rhythm editor with a coherent seventeen-track
palette, optional portable recorded-sound kits, a buffered and more expressive
UI, remembered working folders/layout, reference-informed stock synthesis,
an explicit +12.04 dB playback baseline, and short, stable kick bodies.

The key lesson of the iteration is that measurements, architecture, and
listening feedback answer different questions. Good spectral/envelope
similarity did not excuse the kick tail. Listening exposed a specific defect;
the next change corrected that defect while retaining the successful attack and
early energy profile.

"Almost PERFECT" records the user's assessment at this point, not a claim of
bit-exact hardware emulation or universal perceptual equivalence.

This document preserves the development story and decisions. It is not a
backup of the original WAVs or a substitute for versioning the implementation.
