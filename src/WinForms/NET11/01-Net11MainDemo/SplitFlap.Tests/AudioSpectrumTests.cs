using WinForms.Audio.Analysis;
using WinForms.Audio.Core;
using WinForms.Audio.Synthesis;
using WinForms.Audio.WinForms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace SplitFlap.Tests;

/// <summary>
///  Verifies calibrated FFT math, played-versus-submitted timing, and independent analyzer/UI lifetimes.
/// </summary>
public sealed class AudioSpectrumTests
{
    /// <summary>
    ///  Checks Hann coherent gain and a bin-centred sine's peak, RMS, and frequency.
    /// </summary>
    [Theory]
    [InlineData(44_100)]
    [InlineData(48_000)]
    public void Fft_BinCentredSineHasCalibratedLevel(int sampleRate)
    {
        const int size = 4096;
        const int bin = 85;
        AudioSpectrumOptions options = new();
        HannSpectrumAnalyzer analyzer = new(options);
        short[] pcm = new short[size];
        float[] preClamp = new float[size];
        float[] decibels = new float[size / 2 + 1];
        for (int i = 0; i < size; i++)
        {
            pcm[i] = (short)Math.Round(16384 * Math.Sin(2 * Math.PI * bin * i / size));
            preClamp[i] = pcm[i];
        }

        SpectrumLevels levels = analyzer.Analyze(pcm, preClamp, new AudioFormat(sampleRate, 1), decibels);

        Assert.InRange(decibels[bin], -6.023f, -6.018f);
        Assert.InRange(decibels[bin - 1], -12.05f, -12.03f);
        Assert.InRange(levels.PeakFrequency, (float)bin * sampleRate / size - 0.01f, (float)bin * sampleRate / size + 0.01f);
        Assert.InRange(levels.RmsLevel, -9.04f, -9.02f);
        Assert.InRange(levels.PeakLevel, -6.03f, -6.01f);
        Assert.Equal(0, levels.ClippedSamples);
        Assert.All(decibels, value => Assert.True(float.IsFinite(value)));
    }

    /// <summary>
    ///  Gives actual silence a finite floor, and does not double DC or Nyquist magnitudes.
    /// </summary>
    [Fact]
    public void Fft_SilenceDcAndNyquistHaveDefinedLevels()
    {
        const int size = 64;
        AudioSpectrumOptions options = new() { FftSize = size };
        HannSpectrumAnalyzer analyzer = new(options);
        short[] pcm = new short[size];
        float[] preClamp = new float[size];
        float[] decibels = new float[size / 2 + 1];
        SpectrumLevels silence = analyzer.Analyze(pcm, preClamp, AudioFormat.Default, decibels);
        Assert.All(decibels, value => Assert.Equal(options.MinimumDecibels, value));
        Assert.Equal(options.MinimumDecibels, silence.PeakLevel);
        Assert.Equal(options.MinimumDecibels, silence.RmsLevel);
        Assert.Equal(0, silence.PeakFrequency);

        Array.Fill(pcm, (short)8192);
        Array.Fill(preClamp, 8192);
        SpectrumLevels dc = analyzer.Analyze(pcm, preClamp, AudioFormat.Default, decibels);
        Assert.InRange(decibels[0], -12.042f, -12.040f);
        Assert.Equal(0, dc.PeakFrequency);

        for (int i = 0; i < size; i++)
        {
            pcm[i] = (short)(i % 2 == 0 ? 8192 : -8192);
            preClamp[i] = pcm[i];
        }

        SpectrumLevels nyquist = analyzer.Analyze(pcm, preClamp, AudioFormat.Default, decibels);
        Assert.InRange(decibels[^1], -12.042f, -12.040f);
        Assert.Equal(24_000, nyquist.PeakFrequency);
        Assert.Equal(size / 2 + 1, decibels.Length);
    }

    /// <summary>
    ///  Defines stereo as a channel average rather than accidentally reading interleaved samples as time.
    /// </summary>
    [Fact]
    public void Fft_AveragesStereoChannels()
    {
        const int size = 64;
        AudioSpectrumOptions options = new() { FftSize = size };
        HannSpectrumAnalyzer analyzer = new(options);
        short[] pcm = new short[size * 2];
        for (int i = 0; i < size; i++)
        {
            pcm[i * 2] = 8192;
            pcm[i * 2 + 1] = -8192;
        }

        float[] decibels = new float[size / 2 + 1];
        SpectrumLevels levels = analyzer.Analyze(pcm, new float[size], new AudioFormat(48_000, 2), decibels);
        Assert.All(decibels, value => Assert.Equal(options.MinimumDecibels, value));
        Assert.Equal(options.MinimumDecibels, levels.RmsLevel);
    }

    /// <summary>
    ///  Reports pre-clamp overload while the FFT and RMS still describe the final clamped PCM.
    /// </summary>
    [Fact]
    public void Fft_ClipStatisticsDoNotReplaceTheActualOutputSpectrum()
    {
        const int size = 64;
        HannSpectrumAnalyzer analyzer = new(new AudioSpectrumOptions { FftSize = size });
        short[] pcm = Enumerable.Repeat(short.MaxValue, size * 2).ToArray();
        float[] preClamp = Enumerable.Repeat(short.MaxValue * 2f, size).ToArray();
        float[] decibels = new float[size / 2 + 1];

        SpectrumLevels levels = analyzer.Analyze(pcm, preClamp, new AudioFormat(48_000, 2), decibels);

        Assert.InRange(decibels[0], -0.001f, 0);
        Assert.InRange(levels.RmsLevel, -0.001f, 0);
        Assert.InRange(levels.PeakLevel, 6.01f, 6.03f);
        Assert.Equal(size * 2, levels.ClippedSamples);
    }

    /// <summary>
    ///  Rejects invalid sizes, refresh rates, and non-finite or nonnegative floors before attachment.
    /// </summary>
    [Fact]
    public void Options_ValidateTheirBoundedConfiguration()
    {
        Assert.Equal(4096, new AudioSpectrumOptions().FftSize);
        Assert.Equal(30, new AudioSpectrumOptions().RefreshRate);
        foreach (int size in new[] { 0, 32, 65, 131072 })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioSpectrumOptions { FftSize = size }.Validate());
        }

        foreach (int rate in new[] { 0, 121 })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioSpectrumOptions { RefreshRate = rate }.Validate());
        }

        foreach (float floor in new[] { 0, -301, float.NaN, float.NegativeInfinity })
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AudioSpectrumOptions { MinimumDecibels = floor }.Validate());
        }
    }

    /// <summary>
    ///  Reuses the FFT's permutation, twiddle, window, and work storage for every analysis.
    /// </summary>
    [Fact]
    public void Fft_SteadyAnalysisDoesNotAllocate()
    {
        const int size = 64;
        HannSpectrumAnalyzer analyzer = new(new AudioSpectrumOptions { FftSize = size });
        short[] pcm = new short[size];
        float[] preClamp = new float[size];
        float[] decibels = new float[size / 2 + 1];
        for (int i = 0; i < 256; i++)
        {
            analyzer.Analyze(pcm, preClamp, AudioFormat.Default, decibels);
        }

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1024; i++)
        {
            analyzer.Analyze(pcm, preClamp, AudioFormat.Default, decibels);
        }

        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, allocated);
    }

    /// <summary>
    ///  Aligns the window with completed device frames rather than newer rendered or submitted PCM.
    /// </summary>
    [Fact]
    public async Task Source_AnalyzesPlayedFramesInsteadOfRenderAhead()
    {
        using SpectrumProgressTestSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        engine.MasterVolume = 1;
        engine.Reverb = ReverbSettings.Off;
        using AudioSpectrumSource source = new(engine, new() { FftSize = 64, RefreshRate = 120 });
        _ = engine.Play(new SpectrumConstantVoice(0.5f));
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();
        float[] bins = new float[source.BinCount];
        Assert.False(source.TryCopySpectrum(bins, out _));
        Assert.True(engine.TryGetPlaybackPosition(out long position));
        Assert.Equal(0, position);

        sink.CompletedFrames = 128;
        AudioSpectrumFrame first = WaitForFrame(source, bins, 128);
        Assert.True(first.IsPlaybackSynchronized);
        Assert.Equal(64, first.FftSize);
        Assert.Equal(48_000, first.SampleRate);
        Assert.InRange(bins[0], -6.03f, -6.01f);

        engine.MasterVolume = 0;
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();
        Assert.Equal(256, engine.SubmittedFrames);
        Assert.True(source.TryCopySpectrum(bins, out AudioSpectrumFrame stillPlayed));
        Assert.Equal(128, stillPlayed.EndFrame);
        Assert.InRange(bins[0], -6.03f, -6.01f);

        sink.CompletedFrames = 256;
        AudioSpectrumFrame silent = WaitForFrame(source, bins, 256);
        Assert.True(silent.Sequence > first.Sequence);
        Assert.All(bins, value => Assert.Equal(source.Options.MinimumDecibels, value));
        Assert.Equal(source.Options.MinimumDecibels, silent.RmsLevel);
    }

    /// <summary>
    ///  Labels custom sinks without a device clock as submitted-stream mode.
    /// </summary>
    [Fact]
    public async Task Source_FallbackUsesExplicitSubmittedMetadata()
    {
        using SpectrumTestSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        AudioSpectrumOptions options = new() { FftSize = 64, RefreshRate = 120 };
        using AudioSpectrumSource source = new(engine, options);
        Assert.Same(options, source.Options);
        Assert.Equal(33, source.BinCount);
        float[] bins = new float[source.BinCount];
        Assert.False(source.TryCopySpectrum(bins, out _));
        Assert.Throws<ArgumentException>(() => source.TryCopySpectrum(new float[1], out _));
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();

        AudioSpectrumFrame frame = WaitForFrame(source, bins, 128);
        Assert.False(frame.IsPlaybackSynchronized);
        Assert.False(engine.TryGetPlaybackPosition(out long position));
        Assert.Equal(frame.EndFrame, position);
        Assert.All(bins, value => Assert.Equal(options.MinimumDecibels, value));
    }

    /// <summary>
    ///  Lets independent sources detach and complete without taking ownership of the engine.
    /// </summary>
    [Fact]
    public async Task Source_DisposalAndEngineShutdownHaveIndependentLifetimes()
    {
        using SpectrumTestSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        using AudioSpectrumSource first = new(engine, new() { FftSize = 64, RefreshRate = 120 });
        using AudioSpectrumSource second = new(engine, new() { FftSize = 64, RefreshRate = 120 });

        Parallel.Invoke(first.Dispose, first.Dispose);
        Assert.True(first.Completion.IsCompletedSuccessfully);
        Assert.False(first.TryCopySpectrum(new float[first.BinCount], out _));
        Assert.False(engine.Completion.IsCompleted);
        sink.Advance();
        await sink.ReadAsync();
        sink.Advance();
        await sink.ReadAsync();
        WaitForFrame(second, new float[second.BinCount], 128);

        engine.Dispose();
        await second.Completion.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        Assert.True(second.Completion.IsCompletedSuccessfully);
        Assert.False(second.TryCopySpectrum(new float[second.BinCount], out _));
        Assert.Throws<ObjectDisposedException>(() => new AudioSpectrumSource(engine));
    }

    /// <summary>
    ///  Faults only the analyzer's Completion if a custom progress reader fails.
    /// </summary>
    [Fact]
    public async Task Source_AnalyzerFailureDoesNotStopHealthyPlayback()
    {
        InvalidOperationException failure = new("Bad progress reader.");
        using SpectrumProgressTestSink sink = new() { ProgressFailure = failure };
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        using AudioSpectrumSource source = new(engine, new() { FftSize = 64, RefreshRate = 120 });

        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(
            () => source.Completion.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken)));
        Assert.False(source.TryCopySpectrum(new float[source.BinCount], out _));
        Assert.False(engine.Completion.IsCompleted);
        sink.Advance();
        await sink.ReadAsync();
        Assert.Equal(64, engine.SubmittedFrames);
        Assert.False(engine.Completion.IsCompleted);
    }

    /// <summary>
    ///  Keeps control construction Designer-safe and leaves a caller-owned source alive after control disposal.
    /// </summary>
    [Fact]
    public async Task Control_IsDesignerSafeAndDoesNotOwnItsSource()
    {
        using SpectrumTestSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        await sink.ReadAsync();
        using AudioSpectrumSource source = new(engine, new() { FftSize = 64 });
        RunSta(() =>
        {
            using PaintableSpectrumControl control = new();
            Assert.Null(control.Source);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.UsesDoubleBuffer);
            Assert.True(control.RedrawsOnResize);
            PropertyDescriptor property = TypeDescriptor.GetProperties(control)[nameof(AudioSpectrumControl.Source)]!;
            Assert.False(property.IsBrowsable);
            Assert.Equal(DesignerSerializationVisibility.Hidden,
                ((DesignerSerializationVisibilityAttribute)property.Attributes[typeof(DesignerSerializationVisibilityAttribute)]!).Visibility);

            control.Source = source;
            using Bitmap bitmap = new(640, 280);
            using Graphics graphics = Graphics.FromImage(bitmap);
            control.PaintTo(graphics);
            control.Size = new Size(40, 30);
            control.PaintTo(graphics);
            control.Dispose();
            Assert.False(source.Completion.IsCompleted);
        });

        Assert.False(engine.Completion.IsCompleted);
    }

    private static AudioSpectrumFrame WaitForFrame(AudioSpectrumSource source, float[] bins, long endFrame)
    {
        AudioSpectrumFrame result = default;
        Assert.True(SpinWait.SpinUntil(() =>
        {
            if (source.Completion.IsFaulted)
            {
                return true;
            }

            return source.TryCopySpectrum(bins, out result) && result.EndFrame == endFrame;
        }, TimeSpan.FromSeconds(5)), "The analyzer did not publish the requested complete window.");
        Assert.False(source.Completion.IsFaulted, source.Completion.Exception?.ToString());
        Assert.Equal(endFrame, result.EndFrame);
        return result;
    }

    private static void RunSta(Action action)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception error)
            {
                failure = error;
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(5)));
        if (failure is not null)
        {
            ExceptionDispatchInfo.Throw(failure);
        }
    }

    private sealed class SpectrumConstantVoice(float sample) : IVoice
    {
        /// <summary>
        ///  Keeps producing until engine shutdown.
        /// </summary>
        public bool IsFinished => false;

        /// <summary>
        ///  Produces a deterministic level for playback-alignment tests.
        /// </summary>
        public float Next() => sample;

        /// <summary>
        ///  Leaves shutdown cancellation to the test engine.
        /// </summary>
        public void Release() { }
    }

    private sealed class PaintableSpectrumControl : AudioSpectrumControl
    {
        /// <summary>
        ///  Exposes the inherited buffering style without creating a window handle.
        /// </summary>
        internal bool UsesDoubleBuffer => DoubleBuffered;

        /// <summary>
        ///  Exposes the resize-redraw style for a constructor-only check.
        /// </summary>
        internal bool RedrawsOnResize => GetStyle(ControlStyles.ResizeRedraw);

        /// <summary>
        ///  Paints offscreen to check both normal and undersized geometry without a device or message loop.
        /// </summary>
        internal void PaintTo(Graphics graphics)
        {
            using PaintEventArgs arguments = new(graphics, ClientRectangle);
            OnPaint(arguments);
        }
    }
}
