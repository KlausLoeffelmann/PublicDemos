# Procedural CR-78-style percussion

These are **original digital sound models**, not recordings, transistor-level
simulation, a bit-exact clone, or a claim of listening-verified hardware matching.
No commercial drum samples, captured oscillator tables, song transcription, or
manufacturer preset rhythm table is included. The host supplies its own original
score. The model and sequencing types themselves have no UI dependency.

## Palette and historical scope

[Roland's technical specifications](https://support.roland.com/hc/en-us/articles/201934399-CR-78-Technical-Specifications)
list eleven main sources: BD, SD, RS, HH, CY, maracas, claves, cowbell, high/low
bongo, and low conga. The panel also has tambourine and guiro, and an adjustable
CY/HH metallic sound. Accordingly:

- `Cr78Kit.Instruments` contains **13 percussion sounds**.
- `MetallicBeat` is a **layer and independent audition option**, not a fourteenth
  stock programmer track. `PercussionScore` deliberately rejects it as a track.
- A hi-hat or cymbal hit excites the layer at the selected `MetallicLevel`.
  Simultaneous scored HH/CY hits excite it once, at the greater velocity.
- HH and CY remain independent noise decays, not an 808-style closed/open-hat
  choke pair. Exact original cancel-button behavior is not emulated.
- The [owner's manual, page 2](https://www.manualslib.com/manual/3326086/Roland-Compurhythm-Cr-78.html?page=2#manual)
  describes the layer; [page 5](https://www.manualslib.com/manual/3326086/Roland-Compurhythm-Cr-78.html?page=5#manual)
  describes four voices per programmed pattern and four two-bar memories.
  Those programmer limits are **not general hardware polyphony limits** and are
  intentionally not imposed on this editor.

## Factory targets and decay interpretation

Source: **Roland CR-78 Service Notes, June 20, 1979**, printed pages **13 and 15**,
[archived original](https://archive.org/details/synthmanual-roland-cr-78-service-notes)
([PDF](https://archive.org/download/synthmanual-roland-cr-78-service-notes/rolandcr-78servicenotes.pdf)).
The collection includes VG-11 and revised VG-11A voicing diagrams; page 13
identifies VG-11A for serial 780700 and higher. The common factory adjustment
table on page 15 is the tuning reference here. This is **not** a claim to model
every component change between those board revisions.

The actual page-15 scan was inspected, including the bottom-left diagram. Its
decay interval is from amplitude **V to V/10**: **D20**, or minus 20 dB in voltage
amplitude. It is **not** exponential tau, a minus-60-dB reverb decay, or time to
absolute silence. For D samples, the digital multiplier is:

```text
r^D = 0.1
r = exp(log(0.1) / D)
envelope[n] = r^n
```

Models clear their state after five D20 intervals (minus 100 dB of envelope),
well below useful 16-bit output at these gains. Guiro instead holds during its
gate, then decays. The short excitation ramp and secondary modes mean a measured
whole-signal peak/envelope need not exactly equal the ideal primary envelope.

| Sound | Documented nominal anchor | Digital interpretation / limitation |
|---|---|---|
| Bass drum | 16 ms period = 62.5 Hz; D20 100 ms | Dominant damped 62.5 Hz mode, short 135 Hz mode and muted excitation. Not a modern pitch-swept kick. |
| Snare drum | Body about 340 Hz; D20 60 ms | Body modes plus filtered noise. The scope drawing also distinguishes an initial roughly 20 ms high transient; its exact nonlinear build-up is not circuit-simulated. |
| Rim shot | Frequency column 1480 Hz; D20 5 ms | Very short 1480/2430 Hz modal strike. **Printed** period column says 6.67 ms, inconsistent with 1480 Hz; the frequency column is chosen, not silently “corrected” OCR. Secondary mode is approximate. |
| Hi-hat | Noise; D20 60 ms | Bright filtered noise; filter corners are digital approximations. Optional separate metallic layer. |
| Cymbal | Noise; D20 350 ms | Broader, longer filtered noise, optionally with the same metallic layer. |
| Maracas | Noise; D20 20 ms | Shorter, lower-colored shaker noise. |
| Claves | Frequency column 2630 Hz; D20 18 ms | Damped wood mode with a short harmonic transient. **Printed** 0.43 ms period disagrees with 2630 Hz; the frequency column is chosen. |
| Cowbell | Components 800 and 555 Hz; D20 60 ms | Two filtered, finite-harmonic square-like carriers. Exact transistor pulse shape and analog saturation are not reproduced. |
| High bongo | 600 Hz; D20 40 ms | Damped primary plus short inharmonic membrane mode. |
| Low bongo | 400 Hz; D20 40 ms | Same membrane family at distinct tuning/excitation. |
| Low conga | 208 Hz; D20 150 ms | Lower, longer membrane, not merely a renamed bongo event. |
| Tambourine | D20 220 ms | Noise plus three inharmonic jingle carriers; their pitches are original approximations, not factory-specified targets. |
| Guiro | Two adjustment settings about 125 and 77 Hz | A smooth scraping pulse train excites noise/wood components. This model sweeps 125→77 Hz over the supplied gate; that continuous sweep and the 35 ms D20 release are **design choices**, not measured factory behavior. |
| Metallic layer | 6170, 5620, 4080 Hz; D20 50 ms | Three inharmonic, band-limited square-like carriers with their own filter/envelope. |

`Cr78Preset` centralizes the numbers. Secondary modal pitches, filter corners,
attack ramps, noise/body balance, and software levels are explicitly approximate.
Factory voltage amplitudes inform the relative character but are **not dBFS
calibration values**. Conservative digital peak bounds sum to **0.905** for the
complete bank; ordinary simultaneous hits need no master limiter to fit.

No permissioned isolated reference recording was supplied, so resonance and
decay tests verify these documented targets, **not perceptual equivalence**.
Future lawful reference measurements can refine parameters without adding the
recordings to this repository.

## DSP and lifetime choices

- Supported rates are **32–192 kHz**, including 44.1/48 kHz. Lower rates are
  rejected rather than silently losing the high-frequency palette.
- Bright carriers are procedural Fourier-series tables, not sampled instruments.
  Odd harmonics are tapered and excluded above 45% of sample rate. Interpolation
  and smooth envelopes reduce, but do not claim mathematically zero, residual
  table/modulation images. Noise is shaped by stable high/low-pass stages.
- Membranes/wood use weighted, differently damped modal carriers. Snare adds
  noise; tambourine combines noise and metallic partials; guiro uses a smooth
  repeated scrape. The palette is not fourteen generic sine-note presets.
- Two strike states per channel share prepared tables. Normal retriggers use a
  **5 ms crossfade**, preserving a short old tail without new DSP allocations.
  Faster-than-fade retriggers retain a decaying output correction rather than
  accumulating unbounded voices. This deliberately bounded behavior is not an
  analog component-energy simulation.
- Noise streams continue across triggers. Internal seeded seams make tests
  repeatable without making every real-time hit an identical noise burst.
- Release requests cross onto the audio thread; exact silence clears recursive
  state, and filters also flush inaudible feedback before subnormal values occur.
- Models have **no baked-in reverb**. The player admits a dry engine voice.

## Score/player behavior

`PercussionScore` copies and indexes its input before audio admission. Duplicate
cells, invalid bars/steps, non-finite velocities, invalid gates, and undefined
instruments are rejected. Its 4096-bar resource limit is not a hardware-memory
restriction. `WithStep` returns a new immutable score.

`DrumMachinePlayer` uses one persistent engine voice. It captures
`AudioEngine.RenderedFrames` on first rendering, advances once per sample, and
retains fractional sixteenth-note positions (`sampleRate * 15 / BPM`). It never
uses UI timers or `Task.Delay` for onsets. Supported tempo is 1–1000 BPM.

- Score/tempo requests coalesce and apply together at the next rendered bar
  boundary. Stop accepts them on the next block instead; edits cannot remain
  permanently pending just because transport is stopped.
- A changed bar count continues to the next bar modulo the new score; it does
  not use the UI's selected bar as a seek request. Existing sound tails and
  already-started guiro gates keep their sample durations.
- Start/Stop retain order in a bounded 128-command mailbox. Duplicate starts
  and pending auditions coalesce. An excessive transport flood throws instead
  of silently losing Stop/Start order. Audio uses a nonblocking try-lock **once
  per block**, never a per-sample lock.
- Stop cancels future score events and fades current sounds; audition still
  works. Disposal releases the persistent voice, not the caller-owned engine.
  The host must observe `AudioEngine.Completion` for endpoint/render failures.
- `IsPlaying` describes accepted transport intent/natural completion.
  `GetPlaybackSnapshot()` describes **played** output. Therefore a stopped
  transport can correctly have a playing snapshot while old buffers drain.
- A preallocated 4096-entry history retains bars and transport changes, including
  old tempos and score revisions. At 1000 BPM this holds over sixteen minutes
  of normal bars; default device queues are only a fraction of a second.
  Same-frame changes coalesce. Overwritten or contended history explicitly
  disables synchronization rather than presenting the newest rendered bar.
- Playback progress is completed-buffer progress, not an acoustic speaker
  measurement. A custom sink without a device clock is explicitly marked as
  submitted-stream approximation; buffer, device, and UI-refresh latency remain.
