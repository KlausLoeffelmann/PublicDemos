---
name: splitflap-audio
description: Develop, diagnose, document, and test the vanilla SplitFlap.Audio synthesis and WinMM playback library. Use this for tones, notes, samples, mixing, reverb, waveOut failures, or board sound effects.
---

# Split-Flap Audio Library

## Architecture

`SplitFlap.Audio` is dependency-free and has four layers:

| Layer | Responsibility |
|---|---|
| `Core` | PCM format, voice/sink contracts, mixing pump, and WinMM `waveOut` sink |
| `Synthesis` | Oscillators, noise, filter, ADSR envelope, reverb, and voice implementations |
| `Music` | Notes, accidentals, values, articulation, ornaments, tempo, and notation parsing |
| `Playback` | Instrument patches, channels, sequencing, samples, WAV loading, and resampling |

`SplitFlap.Demo\BoardSound.cs` is application glue. It converts animator events into clacks and jams while sharing one engine with the melody channel.

## Audio model

- The engine produces mono floating-point samples and converts once to signed 16-bit PCM.
- A sink may duplicate mono values across output channels.
- `IAudioSink.Write` is blocking. Device buffer availability is the engine clock.
- Voices are admitted from any thread and mixed on one dedicated pump thread.
- A voice task completes after the voice reports `IsFinished`, including envelope release.
- Reverb uses a per-voice send into a wet bus; the dry and processed wet signals meet before PCM conversion.
- `MasterVolume` needs headroom because many flap clacks can overlap.
- Cancellation requests a graceful voice release; sequence cancellation stops scheduling additional notes.
- Flap events are raised on the dedicated animator worker, not the UI thread. `BoardSound`
  only constructs and enqueues voices there; `AudioEngine` generates and mixes every sample
  on its own high-priority pump thread.
- Same-frame flap events need sub-buffer offsets. `ClackVoice.startDelay` emits exact silence
  for the requested sample count, allowing `BoardSound` to stagger a group without timers,
  sleeps, extra tasks, or UI-thread involvement.
- Keep the clack attack short but nonzero. The half-sine attack preserves the mechanical
  transient while avoiding a full-amplitude random noise sample at the first sample boundary.

## Struct defaults

Do not assume `new SomeRecordStruct()` applies optional primary-constructor values. Value types can be all-zero initialized. Use explicit semantic presets such as:

```csharp
AudioFormat.Default
EnvelopeSettings.Default
```

This invariant is critical for device formats and envelope behavior.

## WinMM ownership

`WaveOutSink` owns:

- the `waveOut` device handle;
- the callback event;
- each unmanaged `WAVEHDR`;
- each unmanaged PCM block associated with a header.

Construction must record allocations immediately and unwind all prior work if open/prepare fails. Disposal wakes blocked writers, resets the device, unprepares headers, frees PCM/header memory, closes the device, and disposes events. Never free a queued block before `waveOutReset` returns ownership.

Every checked native call should report:

- operation name;
- numeric `MMRESULT`;
- text from `waveOutGetErrorText`;
- relevant format/buffer context in the application log.

## Error propagation

The pump must not lose an exception on a background thread. Store the pump failure, fault active and incoming playback tasks, and make later `Play` calls fail immediately. During expected disposal, `ObjectDisposedException` from a woken sink is normal.

## Testing

Never make unit tests depend on an installed or functioning audio endpoint.

Implement `IAudioSink` fakes to:

- capture PCM and verify non-silent finite output;
- pace the pump with a short deterministic delay;
- throw from `Write` and verify playback task failure;
- validate exact frame/channel block lengths;
- verify cancellation and disposal completion.

Use the real endpoint only through the timed smoke harness:

```powershell
dotnet run --project .\SplitFlap.Demo\SplitFlap.Demo.csproj --no-build -- --scenario sound --run-for 3 --no-settings
```

Then inspect `%LocalAppData%\SplitFlap.Demo\Logs`.

## Documentation standard

This library is demonstration material. Explain why PCM and synthesis operations work, not merely what a line assigns. Public contracts use:

```csharp
/// <summary>
///  Description.
/// </summary>
```

Add threading, ownership, range, completion, and exception details where callers need them.
