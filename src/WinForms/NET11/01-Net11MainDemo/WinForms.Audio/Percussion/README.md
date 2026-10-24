# Measured procedural percussion

The stock kit is synthesized from **compact fitted parameters**, not played from
recordings. The temporary WAV references in `DrumMachine.Demo\Sounds` informed
these parameters, but are not embedded, copied to the output, or required at
runtime. There is no transistor-level, bit-exact, or listening-verified hardware
emulation claim.

## Fixed palette

`Cr78Kit.Instruments` has seventeen tracks in musical order:

| Family | Tracks |
|---|---|
| Drums | Kick drum, accented kick drum, snare drum, accented snare drum, rim shot |
| Hats/cymbal | Hi-hat, accented hi-hat, metallic hi-hat, cymbal |
| Tuned percussion | High bongo, low bongo, low conga, cowbell |
| Shaken/scraped | Tambourine 1, tambourine 2, guiro 1, guiro 2 |

Names expand the source filenames rather than exposing abbreviations such as
`Tamb`. Accent and second-articulation tracks have their own fitted models; they
are not just louder or renamed copies.

Stable enum values remain intact. Legacy maracas/claves DSP remains available
for callers using those enum values; the demo migrates their old score tracks
and selection presets to tambourine 2/guiro 2. `MetallicBeat` remains an auxiliary
HH/CY layer and audition, not another score track. The new metallic hi-hat is a
separate fixed track.

## How the models were derived

`DrumMachine.Analysis\analyze.py` fits each normalized reference as:

```text
sum(amplitude * exp(-t / decay) *
    cos(phase + 2*pi*(frequency*t + chirp*t*t/2)))
+ colored_noise(t) * interpolated_RMS_envelope(t) * scrape_modulation(t)
```

The fitter uses variable-projection least squares for up to eight damped modes,
then measures the remaining noise's smoothed power spectrum and RMS envelope.
Guiro models additionally fit scrape rate, rate-change time, phase and pulse
width. The long guiro changes from about 74.65 to 124.89 scrapes/second near
546 ms; this replaces the old, unmeasured continuous 125-to-77 Hz sweep.

`MeasuredModels.json` contains only those parameters: no PCM, recorded noise,
recorded oscillator tables, or stored FFT phases. `MeasuredPercussionModel`
constructs fresh randomized colored noise and analytic modal strikes before
audio admission. Two prepared strikes per channel make successive noise hits
different while retaining deterministic seeded test seams.

Measured reference anchors include approximately 60 Hz for kick, 308 Hz for
snare body, 1408 Hz for rim shot, 590/389 Hz for bongos, 196 Hz for conga, and
546/775 Hz for cowbell. These supersede the previous service-manual nominal
tunings for the stock palette. The legacy auxiliary metallic layer still uses
6170/5620/4080 Hz carriers.

The kicks retain their fitted initial attack, crossfading from 4-12 ms into one
fixed 60 Hz carrier. Its exponential decay is 43 ms (kick) or 37 ms (accent);
a smooth final fade ends the complete strike at 160/140 ms. Detuned/chirped
modes and randomized bass noise never continue into the body. This replaces
the original multi-mode tails, whose beating sounded like vibrato or ambience.
The revised bodies retain approximately 53/45 ms T90 without adding reverb.

Comparison uses full-signal spectral centroids, ninety-percent-energy times
(T90), 48 smoothed spectral bands, and 2 ms RMS envelopes. These are meaningful
spectral/temporal measurements, **not perceptual-equivalence scores**. The
analysis tool can render and compare the actual .NET implementation, not just
its Python approximation. Regression tests retain scalar measurements, not
reference recordings.

A 48 kHz comparison **before the dry-kick revision** of actual .NET stock renders (seed 42, no metallic
layer) against all seventeen references gave spectral-band cosine similarities
of **0.981-1.000** and RMS-envelope cosine similarities of **0.920-1.000**.
Spectral centroids stayed within 12% and T90 within 18% of the references.
These describe this reproducible render, not a guarantee for every noise strike
or a substitute for listening. The two guiros were fitted again after their
initial envelope comparison exposed overly narrow scrape pulses. These historical
comparison ranges do not describe the revised kick tails; the temporary source
WAVs are no longer available for a fresh comparison.

## DSP and sample overrides

- Supported synthesis rates remain 32-192 kHz. The noise spectrum is generated
  at the destination rate; modal carriers above the Nyquist safety margin are
  excluded.
- Preparation, decoding, resampling and PCM copies happen outside rendering.
  Hits, retriggers, fader ramps and mapping changes allocate no per-hit DSP.
- Two strike states and a 5 ms crossfade/release preserve retrigger continuity.
  Rapid retriggers use a bounded correction instead of accumulating voices.
- Instrument gains preserve relative balance. `Cr78VoiceBank.PercussionOutputGain`
  is the single static player-output baseline: `4f`, approximately **+12.04 dB**.
  It follows channel/master faders and applies equally to stock sounds, imported
  WAVs and the auxiliary layer. A sample-peak limiter reduces overloaded mixes
  to `OutputPeakCeiling = 0.98f`, with immediate attenuation and 50 ms exponential
  recovery, rather than relying on PCM hard clipping. Below limiting, the gain
  is exactly fourfold. Master mute remains exact silence.
- Standalone `Cr78Kit.CreateVoice` strikes retain their raw instrument levels;
  calibration/limiting belongs to the player mix, not each individual channel.
  The kit is dry; no recorded ambience is added.
- Stock guiros respect score gates, extending their sustain with a crossfaded
  loop when necessary, then releasing. Default auditions use 250/800 ms gates.
  Imported WAVs instead play their complete natural one-shot.
- `DrumMachinePlayer.SetSamples` replaces the full override mapping at the next
  audio block. Missing entries restore synthesis. Owned PCM copies prevent
  subsequent caller mutations from racing playback, and active tails keep
  their source until the next retrigger.
- Import supports PCM 8/16/24/32-bit and IEEE 32-bit float, including standard
  extensible headers and multichannel downmixing. Invalid chunks, non-finite
  audio, unsupported formats and excessive allocations fail explicitly.
  Percussion presets limit individual WAVs to 30 seconds and 64 MiB.

## Score/player behavior

`PercussionScore` copies, validates, sorts and indexes its events before audio
admission. Duplicate cells, invalid positions, velocities, gates and undefined
instruments are rejected. The 4096-bar bound is a software resource limit,
not the original hardware programmer's memory restriction.

`DrumMachinePlayer` advances from the engine's sample clock, never a UI timer.
Fractional sixteenth-note positions are retained (`sampleRate * 15 / BPM`),
supporting 1-1000 BPM. Score/tempo edits coalesce at bar boundaries; mixer,
loop and sample mapping changes apply at the next block. Stop accepts pending
musical edits without requiring playback to resume.

The bounded 128-command mailbox preserves transport/configuration order.
Auditions coalesce per sound, and overflow throws rather than losing commands.
The audio thread takes a nonblocking try-lock once per block, not per sample.
Pause holds musical position and releases current tails; Stop also resets
position. Both leave audition and the audio endpoint available.

`GetPlaybackSnapshot()` follows completed device buffers, not rendered-ahead
output. Retained bar/tempo history resolves delayed playheads; missing or
overwritten history is explicitly unsynchronized. Device buffering, UI refresh
and acoustic latency remain. `IsPlaying` describes transport intent, not the
separately delayed playhead. The caller owns and observes the engine's completion.
