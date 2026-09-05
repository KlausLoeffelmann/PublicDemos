using System.Threading.Channels;
using SplitFlap.Audio.Core;
using SplitFlap.Audio.Playback;
using SplitFlap.Audio.Synthesis;

namespace SplitFlap.Tests;

public sealed class AudioEngineTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Mixer_WritesExactSamplesAndZeroFillsFinalBlock(int channels)
    {
        using StepSink sink = new(channels);
        using AudioEngine engine = AudioEngine.Create(sink);
        engine.Reverb = ReverbSettings.Off;
        engine.MasterVolume = 1;
        await sink.ReadAsync();

        Task playback = engine.Play(new SequenceVoice([0.25f, -0.25f]));
        sink.Advance();
        short[] block = await sink.ReadAsync();

        short[] mono = [8191, -8191, 0, 0];
        for (int frame = 0; frame < mono.Length; frame++)
        {
            for (int channel = 0; channel < channels; channel++)
            {
                Assert.Equal(mono[frame], block[frame * channels + channel]);
            }
        }

        await playback.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        Assert.False(engine.Completion.IsCompleted);
    }

    [Fact]
    public async Task Trigger_ProducesPcmAndReportsFailureThroughEngineCompletion()
    {
        using StepSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        engine.Reverb = ReverbSettings.Off;
        await sink.ReadAsync();

        engine.CreateChannel().Trigger(new SequenceVoice([0.5f]));
        sink.Advance();
        Assert.Contains(await sink.ReadAsync(), sample => sample != 0);

        InvalidOperationException failure = new("render failure");
        engine.CreateChannel().Trigger(new FailingVoice(failure));
        sink.Advance();
        Exception observed = await Assert.ThrowsAsync<InvalidOperationException>(
            () => engine.Completion.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken));
        Assert.Same(failure, observed);

        // Later awaited requests see the same terminal failure. A one-shot does not allocate
        // another unobserved faulted task; its owner has already received Completion's fault.
        Exception later = await Assert.ThrowsAsync<InvalidOperationException>(
            () => engine.Play(new SequenceVoice([1f])));
        Assert.Same(failure, later);
        engine.CreateChannel().Trigger(new SequenceVoice([1f]));
        Assert.Equal(0, engine.ActiveVoices);
    }

    [Fact]
    public async Task Disposal_CancelsActiveAndQueuedVoicesAndCompletesEngine()
    {
        using StepSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();

        Task active = engine.Play(new ConstantVoice());
        sink.Advance();
        await sink.ReadAsync();
        Task queued = engine.Play(new ConstantVoice());
        engine.Dispose();

        Assert.True(active.IsCanceled);
        Assert.True(queued.IsCanceled);
        Assert.True(engine.Completion.IsCompletedSuccessfully);
        Assert.Equal(0, engine.ActiveVoices);
        Assert.Throws<ObjectDisposedException>(() => { _ = engine.Play(new ConstantVoice()); });
        Assert.Throws<ObjectDisposedException>(() => engine.CreateChannel().Trigger(new ConstantVoice()));
    }

    [Fact]
    public async Task VoiceStealing_CompletesTheOldestAwaitedVoice()
    {
        using StepSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        engine.MaxPolyphony = 1;
        await sink.ReadAsync();

        Task oldest = engine.Play(new ConstantVoice());
        engine.CreateChannel().Trigger(new ConstantVoice());
        sink.Advance();
        await sink.ReadAsync();

        Assert.True(oldest.IsCompletedSuccessfully);
        Assert.Equal(1, engine.ActiveVoices);
    }

    [Fact]
    public async Task Cancellation_RendersTheReleaseInsteadOfCuttingTheVoice()
    {
        using StepSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        using CancellationTokenSource cancellation = new();
        engine.Reverb = ReverbSettings.Off;
        engine.MasterVolume = 1;
        await sink.ReadAsync();
        VoiceChannel channel = engine.CreateChannel(new VoicePatch(
            Waveform.Square, new EnvelopeSettings(0, 0, 1, 0.125f), Volume: 0.5f));

        Task playback = channel.PlaySoundAsync(440, cancellation.Token);
        sink.Advance();
        await sink.ReadAsync();
        cancellation.Cancel();
        sink.Advance();
        short[] release = await sink.ReadAsync();

        Assert.True(release[0] > release[^1]);
        Assert.True(release[^1] > 0);
        Assert.False(playback.IsCompleted);

        sink.Advance();
        short[] final = await sink.ReadAsync();
        await playback.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        Assert.True(playback.IsCompletedSuccessfully);
        Assert.Equal(0, final[^1]);
    }

    [Fact]
    public async Task PumpFailure_FaultsEveryAwaitedVoice()
    {
        using StepSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        Task active = engine.Play(new ConstantVoice());
        InvalidOperationException failure = new("failure while mixing");
        Task failing = engine.Play(new FailingVoice(failure));

        sink.Advance();
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(
            () => engine.Completion.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken)));
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => active));
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => failing));
    }

    [Fact]
    public async Task ConcurrentAdmissionAndDisposal_LeaveNoPlaybackTasksPending()
    {
        using StepSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        List<Task> playbacks = [];

        Task producer = Task.Run(() =>
        {
            for (int i = 0; i < 1_000; i++)
            {
                try
                {
                    playbacks.Add(engine.Play(new ConstantVoice()));
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                if (i == 16)
                {
                    started.SetResult();
                }
            }
        }, TestContext.Current.CancellationToken);

        await started.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        engine.Dispose();
        await producer.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        Assert.All(playbacks, task => Assert.True(task.IsCanceled));
        Assert.True(engine.Completion.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task CleanupFailure_StillJoinsThePumpAndCanBeRetried()
    {
        using StepSink steps = new();
        FailingDisposeSink sink = new(steps);
        using AudioEngine engine = AudioEngine.Create(sink);
        await steps.ReadAsync();
        Task queued = engine.Play(new ConstantVoice());

        Assert.Throws<InvalidOperationException>(engine.Dispose);
        Assert.True(queued.IsCanceled);
        Assert.True(engine.Completion.IsCompletedSuccessfully);
        Assert.Equal(1, sink.DisposeCount);

        engine.Dispose();
        engine.Dispose();
        Assert.Equal(2, sink.DisposeCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Polyphony_MustBePositive(int maximum)
    {
        using StepSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        Assert.Throws<ArgumentOutOfRangeException>(() => engine.MaxPolyphony = maximum);
    }

    private sealed class SequenceVoice(float[] samples) : IVoice
    {
        private int _position;
        public bool IsFinished => _position >= samples.Length;
        public float Next() => IsFinished ? 0 : samples[_position++];
        public void Release() => _position = samples.Length;
    }

    private sealed class ConstantVoice : IVoice
    {
        public bool IsFinished => false;
        public float Next() => 0.1f;
        public void Release() { }
    }

    private sealed class FailingVoice(Exception failure) : IVoice
    {
        public bool IsFinished => false;
        public float Next() => throw failure;
        public void Release() { }
    }

    private sealed class FailingDisposeSink(StepSink inner) : IAudioSink
    {
        public AudioFormat Format => inner.Format;
        public int FramesPerBuffer => inner.FramesPerBuffer;
        public int DisposeCount { get; private set; }
        public void Write(ReadOnlySpan<short> pcm) => inner.Write(pcm);

        public void Dispose()
        {
            inner.Dispose();
            if (++DisposeCount == 1)
            {
                throw new InvalidOperationException("The device could not close on the first attempt.");
            }
        }
    }

    private sealed class StepSink(int channels = 1) : IAudioSink
    {
        private readonly Channel<short[]> _blocks = Channel.CreateUnbounded<short[]>();
        private readonly SemaphoreSlim _advance = new(0);
        private int _disposed;

        public AudioFormat Format => new(48_000, channels);
        public int FramesPerBuffer => 4;

        public async Task<short[]> ReadAsync()
            => await _blocks.Reader.ReadAsync(TestContext.Current.CancellationToken)
                .AsTask().WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        public void Advance() => _advance.Release();

        public void Write(ReadOnlySpan<short> pcm)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            Assert.Equal(FramesPerBuffer * Format.Channels, pcm.Length);
            Assert.True(_blocks.Writer.TryWrite(pcm.ToArray()));
            _advance.Wait();
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _advance.Release();
            }
        }
    }
}
