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
- This means **rendering** has finished, not that the device has played the last block.
  Device buffering and reverb tails can outlive that task.
- Observe `AudioEngine.Completion` once per engine. It faults on a terminal rendering/device
  failure and completes normally after disposal. `VoiceChannel.Trigger` deliberately does
  not create a task per clack; lifetime observation is how those failures remain visible.
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
- `BoardSound` serializes event admission against its own disposal. Unsubscribing an event
  does not cancel a callback that the animator is already executing.

## Performance and readable DSP

- The dry, wet, and PCM arrays belong to the engine and are reused. Array-to-span conversion
  does not copy or allocate, but simply replacing array indexing with span indexing is not
  itself a speed improvement.
- `OnePoleFilter` computes coefficients when a cutoff changes, not for every sample.
  Configure its cutoffs on its owning thread; each filter's delay state is also thread-owned.
- `Oscillator` publishes pitch atomically and caches its phase increment on the rendering
  thread. This retains live pitch changes without resetting phase or adding per-sample locks.
- `ClackVoice` evaluates the half-sine only during its attack (72 samples at 48 kHz/1.5 ms).
  The multiplier is one for the remainder of its approximately 35-48 ms sounding decay.
- `Trigger` still allocates a voice and a small admission entry. It is **not allocation-free**;
  it avoids only the unnecessary completion source/task. Awaited playback retains both.
- Internal seeded noise/clack constructors permit repeatable reference comparisons.
  Normal playback still uses random variation.
- The existing `IVoice.Next` contract remains deliberately simple. Consider a block-oriented
  `Span<float>` contract only if repeatable measurements justify its additional code and copy
  pass. Do not add SIMD intrinsics, unsafe DSP, task-per-voice mixing, or speculative pools.
- Enabled reverb must continue processing zero input while its delay lines contain a tail.
  An empty voice list is not permission to discard or freeze that tail.
- Feedback storage is flushed to exact zero below `1e-20f`, far below one 16-bit PCM step.
  Otherwise subnormal floating-point residue can consume extra CPU long after the sound
  is inaudible. This is not a gate on audible tails and needs no processor intrinsics.
- Four default 20 ms WinMM buffers hold up to 80 ms of queued audio, and the pump can have
  another rendered block waiting. This is queue capacity, not measured speaker latency.
  Smaller buffers increase wakeups and scheduling pressure; preserve defaults unless
  representative real-device results justify a change.

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

The sink uses safe C# `IntPtr`/`Marshal` interop, not C# pointers. Keep native headers and
PCM storage alive at stable addresses for their entire prepared/queued lifetime; passing a
temporary managed header by reference is not sufficient. Its reusable staging array lets the
span-based sink contract use `Marshal.Copy` without allocating on each write.

Construction must record allocations immediately and unwind all prior work if open/prepare
fails. Disposal wakes blocked writers, resets the device, unprepares returned headers,
releases native storage, closes the device, and disposes events after all users have left.
Never free a queued/prepared block while WinMM still owns it, including on cleanup failures.
Header scanning, copying, and submission must be serialized against teardown. Signal shutdown
before waiting for the writer lock so a blocked writer can leave.

Every checked native call should report:

- operation name;
- numeric `MMRESULT`;
- text from `waveOutGetErrorText`;
- relevant format/buffer context in the application log.

## Error propagation

The pump must not lose an exception on a background thread. Store the pump failure, fault
active/incoming playback tasks and `AudioEngine.Completion`, and make later `Play` calls
return a faulted task. During expected disposal, `ObjectDisposedException` from a woken sink
is normal. Only the pump cleans up its active voice list; admission is closed before draining
so a concurrent producer cannot strand a completion task.

The demo observes the engine lifetime task from the UI thread, writes one AppData diagnostic,
and disables the failed sound instance. Do not do logging, dialogs, or UI dispatch inside the
per-sample loop.

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

For opt-in Release measurements, use `AudioPerformanceTests` as documented in the test-harness
skill. The existing correctness `MemorySink` copies PCM and sleeps; it is not a performance
baseline. Include silence after a long tail, not just newly initialized silent buffers.
Compare the seeded reference equations and the same workload before/after changing DSP.

## Documentation standard

This library is demonstration material. Explain why PCM and synthesis operations work, not merely what a line assigns. Public contracts use:

```csharp
/// <summary>
///  Description.
/// </summary>
```

Add threading, ownership, range, completion, and exception details where callers need them.
