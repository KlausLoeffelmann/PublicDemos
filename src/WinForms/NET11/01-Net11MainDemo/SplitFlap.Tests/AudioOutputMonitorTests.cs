using WinForms.Audio.Core;
using WinForms.Audio.Synthesis;
using System.Threading.Channels;

namespace SplitFlap.Tests;

/// <summary>
///  Exercises actual final-output capture, bounded-history coherence, and nonblocking visualization drops.
/// </summary>
public sealed class AudioOutputMonitorTests
{
    /// <summary>
    ///  Captures exactly the submitted, interleaved, post-gain/clamp PCM and its conversion inputs.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Capture_ContainsActualSubmittedPcmAndPreClampValues(int channels)
    {
        using SpectrumTestSink sink = new(channels);
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        engine.MasterVolume = 0.75f;
        engine.Reverb = ReverbSettings.Off;
        AudioOutputMonitor monitor = engine.AttachOutputMonitor(sink.FramesPerBuffer);
        float[] samples = [-2, -1, -0.25f, 0, 0.25f, 1, 2];
        Task playback = engine.Play(new SpectrumSequenceVoice(samples));
        sink.Advance();
        short[] submitted = await sink.ReadAsync();

        short[] copied = new short[sink.FramesPerBuffer * channels];
        float[] preClamp = new float[sink.FramesPerBuffer];
        long endFrame = sink.FramesPerBuffer * 2;
        Assert.Equal(endFrame, engine.RenderedFrames);
        Assert.Equal(sink.FramesPerBuffer, engine.SubmittedFrames);
        Assert.False(monitor.TryCopyWindow(endFrame, copied, preClamp, out _));

        sink.Advance();
        await sink.ReadAsync();
        Assert.True(monitor.TryCopyWindow(endFrame, copied, preClamp, out AudioOutputWindow window));
        Assert.Equal(submitted, copied);
        Assert.Equal(endFrame, window.EndFrame);
        Assert.Equal(0, window.DroppedBlocks);

        for (int i = 0; i < sink.FramesPerBuffer; i++)
        {
            float expected = i < samples.Length ? samples[i] * (0.75f * short.MaxValue) : 0;
            Assert.Equal(expected, preClamp[i]);
            short pcm = (short)Math.Clamp(expected, short.MinValue, short.MaxValue);
            for (int channel = 0; channel < channels; channel++)
            {
                Assert.Equal(pcm, copied[i * channels + channel]);
            }
        }

        await playback.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
    }

    /// <summary>
    ///  Does not change a single PCM sample when optional monitoring is enabled.
    /// </summary>
    [Fact]
    public async Task Capture_LeavesPcmIdenticalWhenEnabled()
    {
        short[] unmonitored = await RenderBlock(false);
        short[] monitored = await RenderBlock(true);
        Assert.Equal(unmonitored, monitored);
    }

    /// <summary>
    ///  Does not publish a block rejected by the sink.
    /// </summary>
    [Fact]
    public async Task Capture_ExcludesFailedWritesAndStopsWithTheEngine()
    {
        using SpectrumTestSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        AudioOutputMonitor monitor = engine.AttachOutputMonitor(sink.FramesPerBuffer);
        sink.Advance();
        await sink.ReadAsync();
        InvalidOperationException failure = new("Output rejected.");
        sink.WriteFailure = failure;
        sink.Advance();

        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(
            () => engine.Completion.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken)));
        Assert.Equal(sink.FramesPerBuffer, engine.SubmittedFrames);
        Assert.Equal(sink.FramesPerBuffer * 2, engine.RenderedFrames);
        Assert.True(monitor.IsStopped);
        Assert.False(monitor.TryCopyWindow(
            sink.FramesPerBuffer * 2, new short[sink.FramesPerBuffer], new float[sink.FramesPerBuffer], out _));
    }

    /// <summary>
    ///  Starts a new subscription at its real absolute frame instead of reusing an old monitor's history.
    /// </summary>
    [Fact]
    public async Task Capture_AttachesAndDetachesMidstream()
    {
        using SpectrumTestSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        AudioOutputMonitor first = engine.AttachOutputMonitor(sink.FramesPerBuffer);
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();
        short[] pcm = new short[sink.FramesPerBuffer];
        float[] preClamp = new float[sink.FramesPerBuffer];
        Assert.True(first.TryCopyWindow(sink.FramesPerBuffer * 2, pcm, preClamp, out _));

        engine.DetachOutputMonitor(first);
        AudioOutputMonitor second = engine.AttachOutputMonitor(sink.FramesPerBuffer);
        Assert.True(first.IsStopped);
        Assert.False(second.TryCopyWindow(sink.FramesPerBuffer * 2, pcm, preClamp, out _));
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();

        Assert.True(second.TryCopyWindow(sink.FramesPerBuffer * 4, pcm, preClamp, out _));
        engine.Dispose();
        Assert.True(second.IsStopped);
        Assert.False(second.TryCopyWindow(sink.FramesPerBuffer * 4, pcm, preClamp, out _));
    }

    /// <summary>
    ///  Drops visualization rather than waiting for a reader and rejects windows spanning that gap.
    /// </summary>
    [Fact]
    public async Task History_SlowReaderDropsBlocksWithoutTornWindows()
    {
        AudioOutputMonitor monitor = new(AudioFormat.Default, 4, 0, 8);
        short[] block = [1, 1, 1, 1];
        float[] beforeClamp = [1, 1, 1, 1];
        Assert.True(monitor.TryWrite(0, block, beforeClamp));
        Assert.True(monitor.TryWrite(4, block, beforeClamp));
        using ManualResetEventSlim held = new(false);
        using ManualResetEventSlim release = new(false);
        Task reader = Task.Run(() =>
        {
            lock (monitor.CopySynchronization)
            {
                held.Set();
                release.Wait(TimeSpan.FromSeconds(5));
            }
        }, TestContext.Current.CancellationToken);

        try
        {
            Assert.True(held.Wait(TimeSpan.FromSeconds(5)));
            Assert.False(monitor.TryWrite(8, block, beforeClamp));
            Assert.Equal(1, monitor.DroppedBlocks);
        }
        finally
        {
            release.Set();
            await reader.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        }

        short[] pcm = Enumerable.Repeat((short)-123, 8).ToArray();
        float[] preClamp = new float[8];
        Array.Fill(block, (short)4);
        Assert.True(monitor.TryWrite(12, block, beforeClamp));
        Assert.False(monitor.TryCopyWindow(16, pcm, preClamp, out _));
        Assert.All(pcm, sample => Assert.Equal((short)-123, sample));
        Array.Fill(block, (short)5);
        Assert.True(monitor.TryWrite(16, block, beforeClamp));
        Assert.True(monitor.TryCopyWindow(20, pcm, preClamp, out AudioOutputWindow window));
        Assert.Equal(new short[] { 4, 4, 4, 4, 5, 5, 5, 5 }, pcm);
        Assert.Equal(1, window.DroppedBlocks);
    }

    /// <summary>
    ///  Copies through a non-block-aligned ring wrap and rejects old or future playback positions.
    /// </summary>
    [Fact]
    public void History_HandlesWrapOverwriteAndOutOfRangeCursors()
    {
        AudioOutputMonitor monitor = new(AudioFormat.Default, 3, 5, 8);
        Assert.Equal(19, monitor.CapacityFrames);
        short[] block = new short[3];
        float[] preClampBlock = new float[3];
        for (int start = 0; start < 30; start += 3)
        {
            for (int i = 0; i < block.Length; i++)
            {
                block[i] = (short)(start + i);
                preClampBlock[i] = start + i;
            }

            Assert.True(monitor.TryWrite(start, block, preClampBlock));
        }

        short[] pcm = new short[8];
        float[] preClamp = new float[8];
        Assert.False(monitor.TryCopyWindow(9, pcm, preClamp, out _));
        Assert.False(monitor.TryCopyWindow(31, pcm, preClamp, out _));
        Assert.False(monitor.TryCopyWindow(long.MinValue, pcm, preClamp, out _));
        Assert.True(monitor.TryCopyWindow(24, pcm, preClamp, out _));
        Assert.Equal(Enumerable.Range(16, 8).Select(value => (short)value), pcm);
        Assert.Equal(Enumerable.Range(16, 8).Select(value => (float)value), preClamp);
    }

    /// <summary>
    ///  Keeps both producer copies and coherent history reads allocation-free after warm-up.
    /// </summary>
    [Fact]
    public void History_SteadyCopiesDoNotAllocate()
    {
        AudioOutputMonitor monitor = new(AudioFormat.Default, 64, 192, 64);
        short[] pcm = new short[64];
        float[] preClamp = new float[64];
        short[] copied = new short[64];
        float[] copiedPreClamp = new float[64];
        long frame = 0;
        for (int i = 0; i < 256; i++)
        {
            monitor.TryWrite(frame, pcm, preClamp);
            frame += 64;
            monitor.TryCopyWindow(frame, copied, copiedPreClamp, out _);
        }

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1024; i++)
        {
            monitor.TryWrite(frame, pcm, preClamp);
            frame += 64;
            monitor.TryCopyWindow(frame, copied, copiedPreClamp, out _);
        }

        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, allocated);
    }

    private static async Task<short[]> RenderBlock(bool monitoring)
    {
        using SpectrumTestSink sink = new(2);
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        engine.MasterVolume = 0.63f;
        engine.Reverb = ReverbSettings.Off;
        if (monitoring)
        {
            engine.AttachOutputMonitor(sink.FramesPerBuffer);
        }

        _ = engine.Play(new SpectrumSequenceVoice([-2, -0.7f, -0.125f, 0.333f, 0.8f, 2]));
        sink.Advance();
        return await sink.ReadAsync();
    }
}

/// <summary>
///  Gates real engine writes before they succeed, with endpoint-free observations for monitor tests.
/// </summary>
internal class SpectrumTestSink(int channels = 1) : IAudioSink
{
    private readonly Channel<short[]> _blocks = Channel.CreateUnbounded<short[]>();
    private readonly SemaphoreSlim _advance = new(0);
    private readonly CancellationTokenSource _stop = new();
    private readonly Lock _writeSync = new();
    private readonly Lock _disposeSync = new();
    private bool _stopping;
    private bool _disposed;

    /// <summary>
    ///  Gets the test's mono or duplicated-stereo output format.
    /// </summary>
    public AudioFormat Format { get; } = new(48_000, channels);

    /// <summary>
    ///  Gets a small complete block so deterministic tests do not need audio-duration delays.
    /// </summary>
    public int FramesPerBuffer => 64;

    /// <summary>
    ///  Selects a failure for the currently gated submission.
    /// </summary>
    internal Exception? WriteFailure;

    /// <summary>
    ///  Observes submitted PCM but does not accept it until the test releases the gate.
    /// </summary>
    public void Write(ReadOnlySpan<short> pcm)
    {
        lock (_writeSync)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _stopping), this);
            _blocks.Writer.TryWrite(pcm.ToArray());
            try
            {
                _advance.Wait(_stop.Token);
            }
            catch (OperationCanceledException)
            {
                throw new ObjectDisposedException(nameof(SpectrumTestSink));
            }

            if (WriteFailure is not null)
            {
                throw WriteFailure;
            }
        }
    }

    /// <summary>
    ///  Releases exactly one pending submission.
    /// </summary>
    internal void Advance() => _advance.Release();

    /// <summary>
    ///  Waits for the pump to reach its next real sink write.
    /// </summary>
    internal Task<short[]> ReadAsync()
        => _blocks.Reader.ReadAsync(TestContext.Current.CancellationToken).AsTask()
            .WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

    /// <summary>
    ///  Wakes and drains the writer before releasing the test's synchronization resources.
    /// </summary>
    public void Dispose()
    {
        lock (_disposeSync)
        {
            if (_disposed)
            {
                return;
            }

            Volatile.Write(ref _stopping, true);
            _stop.Cancel();
            lock (_writeSync)
            {
                _advance.Dispose();
                _stop.Dispose();
                _blocks.Writer.TryComplete();
                _disposed = true;
            }
        }
    }
}

/// <summary>
///  Adds an explicitly controlled cached playback cursor without involving an audio device.
/// </summary>
internal sealed class SpectrumProgressTestSink : SpectrumTestSink, IAudioPlaybackProgress
{
    private long _completedFrames;

    /// <summary>
    ///  Configures a progress-reader failure to verify that analyzer failure does not stop the engine.
    /// </summary>
    internal Exception? ProgressFailure;

    /// <summary>
    ///  Gets or sets the observed played cursor.
    /// </summary>
    public long CompletedFrames
    {
        get => ProgressFailure is null ? Volatile.Read(ref _completedFrames) : throw ProgressFailure;
        set => Volatile.Write(ref _completedFrames, value);
    }

    /// <summary>
    ///  Gets the simulated queue depth used to size bounded history.
    /// </summary>
    public int BufferCapacityFrames => FramesPerBuffer * 3;
}

/// <summary>
///  Supplies deterministic mono samples to the real engine, not to a duplicate mixer.
/// </summary>
internal sealed class SpectrumSequenceVoice(float[] samples) : IVoice
{
    private int _position;

    /// <summary>
    ///  Gets whether every supplied sample has rendered.
    /// </summary>
    public bool IsFinished => _position >= samples.Length;

    /// <summary>
    ///  Produces the next supplied value, then silence.
    /// </summary>
    public float Next() => IsFinished ? 0 : samples[_position++];

    /// <summary>
    ///  Ends this test voice.
    /// </summary>
    public void Release() => _position = samples.Length;
}
