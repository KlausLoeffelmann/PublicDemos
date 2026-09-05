using System.Diagnostics;
using System.Text.Json;
using DrumMachine.Demo;
using WinForms.Audio.Analysis;
using WinForms.Audio.Core;
using WinForms.Audio.Music;
using WinForms.Audio.Playback;
using WinForms.Audio.Sequencing;
using WinForms.Audio.Synthesis;

namespace SplitFlap.Tests;

/// <summary>
///  Opt-in measurements of the real audio pump, with no device, sleeps, or PCM copies in the sink.
/// </summary>
public sealed class AudioPerformanceTests(ITestOutputHelper output)
{
    /// <summary>
    ///  Reports repeatable Release workloads; timing values are observations, not test assertions.
    /// </summary>
    [Fact(Explicit = true)]
    [Trait("Category", "Performance")]
    public void MeasurePump()
    {
        Workload[] workloads =
        [
            new("idle-off", ReverbSettings.Off),
            new("idle-hall", ReverbSettings.Hall),
            new("single-sine", ReverbSettings.Off, Tones: 1),
            new("board-clacks", ReverbSettings.Hall, BoardBursts: true),
            new("board-and-melody", ReverbSettings.Hall, Tones: 1, BoardBursts: true),
            new("clacks-32", ReverbSettings.Hall, Clacks: 32),
            new("clacks-64", ReverbSettings.Hall, Clacks: 64),
            new("sines-64", ReverbSettings.Hall, Tones: 64),
            new("hall-tail", ReverbSettings.Hall, TailBursts: true)
        ];

        output.WriteLine(
            $"Runtime: {Environment.Version}; configuration: " +
#if DEBUG
            "Debug (use Release for comparisons).");
#else
            "Release.");
#endif

        // Warm all code paths before the measured rounds. Those rounds then use a fixed
        // warm-up block count, so every comparison sees the same seeded audio sequence.
        foreach (Workload workload in workloads)
        {
            Measure(workload, round: 0);
        }

        for (int round = 1; round <= 3; round++)
        {
            foreach (Workload workload in workloads)
            {
                TestContext.Current.CancellationToken.ThrowIfCancellationRequested();
                output.WriteLine("PERF " + JsonSerializer.Serialize(Measure(workload, round)));
            }
        }
    }

    /// <summary>
    ///  Measures long-idle reverb after a strike, not only freshly initialized silent buffers.
    /// </summary>
    [Fact(Explicit = true)]
    [Trait("Category", "Performance")]
    public void MeasureLongIdle()
    {
        // Timetable updates can be minutes apart. A tail that is no longer audible may still
        // leave tiny floating-point values in feedback filters, unlike a fresh silent engine.
        Workload[] workloads =
        [
            new("room-after-long-tail", ReverbSettings.Room, OneBurst: true, WarmupBlocks: 3_200),
            new("hall-after-long-tail", ReverbSettings.Hall, OneBurst: true, WarmupBlocks: 3_200)
        ];

        foreach (Workload workload in workloads)
        {
            output.WriteLine("PERF " + JsonSerializer.Serialize(Measure(workload, round: 1)));
        }
    }

    /// <summary>
    ///  Compares the actual drum-player pump with and without a concurrent spectrum worker.
    /// </summary>
    [Fact(Explicit = true)]
    [Trait("Category", "Performance")]
    public void MeasureRhythm()
    {
        Workload[] workloads =
        [
            new("rhythm-playing", ReverbSettings.Off, DrumPlayer: true),
            new("rhythm-spectrum", ReverbSettings.Off, DrumPlayer: true, Spectrum: true),
            new("rhythm-idle-spectrum", ReverbSettings.Off, DrumPlayer: true, Spectrum: true, Idle: true)
        ];

        foreach (Workload workload in workloads)
        {
            Measure(workload, round: 0);
        }

        // Production kit voices retain their normal variation. These elapsed-time observations
        // are not bit-exact A/B measurements or isolated analyzer CPU counters.
        for (int round = 1; round <= 3; round++)
        {
            foreach (Workload workload in workloads)
            {
                output.WriteLine("PERF " + JsonSerializer.Serialize(Measure(workload, round)));
            }
        }
    }

    private static Measurement Measure(Workload workload, int round)
    {
        // The fixture owns these gates. The sink signals shutdown; only after the engine joins
        // its worker do the surrounding using statements dispose the wait handles.
        using ManualResetEventSlim start = new();
        using ManualResetEventSlim finished = new();
        using ManualResetEventSlim stop = new();
        using MeasuringSink sink = new(workload, start, finished, stop, round == 0 ? 250 : 0);
        using (AudioEngine engine = AudioEngine.Create(sink))
        {
            engine.MaxPolyphony = 64;
            engine.Reverb = workload.Reverb;
            sink.Attach(engine);
            using DrumMachinePlayer? player = workload.DrumPlayer
                ? new DrumMachinePlayer(engine, DemoScores.OriginalBallad, new Tempo(120))
                : null;
            using AudioSpectrumSource? spectrum = workload.Spectrum ? new AudioSpectrumSource(engine) : null;
            if (player is not null && !workload.Idle)
            {
                player.Start();
            }
            start.Set();

            Assert.True(finished.Wait(TimeSpan.FromSeconds(20), TestContext.Current.CancellationToken));
            if (spectrum?.Completion.IsFaulted == true)
            {
                spectrum.Completion.GetAwaiter().GetResult();
            }
        }

        return sink.GetMeasurement(round);
    }

    private sealed record Workload(
        string Name,
        ReverbSettings Reverb,
        int Tones = 0,
        int Clacks = 0,
        bool BoardBursts = false,
        bool TailBursts = false,
        bool OneBurst = false,
        int WarmupBlocks = 128,
        bool DrumPlayer = false,
        bool Spectrum = false,
        bool Idle = false);

    private sealed record Measurement(
        string Workload,
        int Round,
        int Blocks,
        double AudioSeconds,
        double MeanRenderMicroseconds,
        double P95RenderMicroseconds,
        double P99RenderMicroseconds,
        double MaxRenderMicroseconds,
        double RenderMillisecondsPerAudioSecond,
        double RenderBytesPerBlock,
        double ProducerBytesPerVoice,
        int Gen0Collections,
        long PcmChecksum);

    private sealed class MeasuringSink(
        Workload workload,
        ManualResetEventSlim start,
        ManualResetEventSlim finished,
        ManualResetEventSlim stop,
        int minimumWarmupMilliseconds) : IAudioSink
    {
        private const int MeasuredBlocks = 512;
        private readonly long[] _renderTicks = new long[MeasuredBlocks];
        private AudioEngine _engine = null!;
        private VoiceChannel _channel = null!;
        private long _renderStarted;
        private long _allocationStarted;
        private long _warmupStarted;
        private long _renderBytes;
        private long _producerBytes;
        private long _producerVoices;
        private long _pcmChecksum;
        private int _measured;
        private int _block;
        private int _nextBurst;
        private int _voiceSequence;
        private int _gen0AtStart;
        private int _gen0Collections;
        private bool _measuring;

        public AudioFormat Format => AudioFormat.Default;

        public int FramesPerBuffer => 960;

        public void Attach(AudioEngine engine)
        {
            _engine = engine;
            _channel = engine.CreateChannel();
            _channel.ReverbSend = 0.35f;
        }

        public void Write(ReadOnlySpan<short> pcm)
        {
            long now = Stopwatch.GetTimestamp();
            long allocated = GC.GetAllocatedBytesForCurrentThread();

            if (_measuring)
            {
                _renderTicks[_measured++] = now - _renderStarted;
                _renderBytes += allocated - _allocationStarted;
                _pcmChecksum += pcm[0];
            }

            if (_measured == MeasuredBlocks)
            {
                _gen0Collections = GC.CollectionCount(0) - _gen0AtStart;
                finished.Set();
                stop.Wait();
                throw new ObjectDisposedException(nameof(MeasuringSink));
            }

            start.Wait();
            ObjectDisposedException.ThrowIf(stop.IsSet, this);

            if (_warmupStarted == 0)
            {
                _warmupStarted = Stopwatch.GetTimestamp();
            }

            if (!_measuring
                && _block >= workload.WarmupBlocks
                && Stopwatch.GetElapsedTime(_warmupStarted).TotalMilliseconds >= minimumWarmupMilliseconds)
            {
                _measuring = true;
                _gen0AtStart = GC.CollectionCount(0);
            }

            // Scheduling runs outside the timed render interval. Its allocations are counted
            // separately even though this deterministic fixture drives it on the pump thread.
            long beforeProducer = GC.GetAllocatedBytesForCurrentThread();
            int voices = ScheduleNextBlock();
            if (_measuring)
            {
                _producerBytes += GC.GetAllocatedBytesForCurrentThread() - beforeProducer;
                _producerVoices += voices;
            }

            _allocationStarted = GC.GetAllocatedBytesForCurrentThread();
            _renderStarted = Stopwatch.GetTimestamp();
        }

        public void Dispose()
            => stop.Set();

        public Measurement GetMeasurement(int round)
        {
            double totalTicks = _renderTicks.Sum();
            double microsecondsPerTick = 1_000_000d / Stopwatch.Frequency;
            double audioSeconds = MeasuredBlocks * FramesPerBuffer / (double)Format.SampleRate;
            Array.Sort(_renderTicks);

            return new(
                workload.Name,
                round,
                MeasuredBlocks,
                audioSeconds,
                totalTicks / MeasuredBlocks * microsecondsPerTick,
                _renderTicks[(int)(MeasuredBlocks * 0.95)] * microsecondsPerTick,
                _renderTicks[(int)(MeasuredBlocks * 0.99)] * microsecondsPerTick,
                _renderTicks[^1] * microsecondsPerTick,
                totalTicks / Stopwatch.Frequency * 1000 / audioSeconds,
                _renderBytes / (double)MeasuredBlocks,
                _producerVoices == 0 ? 0 : _producerBytes / (double)_producerVoices,
                _gen0Collections,
                _pcmChecksum);
        }

        private int ScheduleNextBlock()
        {
            int voices = 0;
            if (_block == 0)
            {
                for (int i = 0; i < workload.Tones; i++)
                {
                    _channel.Trigger(new ToneVoice(Format.SampleRate, VoicePatch.Default, 220 + i * 7));
                    voices++;
                }
            }

            while (_engine.ActiveVoices + voices < workload.Clacks)
            {
                TriggerClack(TimeSpan.Zero);
                voices++;
            }

            if (workload.BoardBursts)
            {
                long blockStart = (long)_block * FramesPerBuffer;
                long blockEnd = blockStart + FramesPerBuffer;
                long burstSample = (long)_nextBurst * Format.SampleRate / 60;

                while (burstSample < blockEnd)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        double offsetSeconds = (burstSample - blockStart) / (double)Format.SampleRate;
                        TriggerClack(TimeSpan.FromSeconds(offsetSeconds + (0.1 + i * 0.5) / 1000));
                        voices++;
                    }

                    burstSample = (long)++_nextBurst * Format.SampleRate / 60;
                }
            }

            if ((workload.TailBursts && _block % 250 == 0) || (workload.OneBurst && _block == 0))
            {
                for (int i = 0; i < 12; i++)
                {
                    TriggerClack(TimeSpan.FromMilliseconds(i * 0.5));
                    voices++;
                }
            }

            _block++;
            return voices;
        }

        private void TriggerClack(TimeSpan delay)
        {
            uint seed = (uint)++_voiceSequence;
            float variance = 0.85f + (seed % 31) / 100f;
            _channel.Trigger(new ClackVoice(
                Format.SampleRate, 0.25f, delay, 1.5f, seed, variance));
        }
    }
}
