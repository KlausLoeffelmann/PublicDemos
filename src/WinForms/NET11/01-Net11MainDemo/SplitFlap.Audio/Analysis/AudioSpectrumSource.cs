namespace SplitFlap.Audio.Analysis;

/// <summary>
///  Analyzes a supplied engine's final output on a separate worker without owning the engine.
/// </summary>
/// <remarks>
///  Completed-buffer timing avoids displaying the engine's render-ahead queue. The FFT window,
///  whole-buffer progress, refresh interval, and device still introduce display/acoustic latency.
/// </remarks>
public sealed class AudioSpectrumSource : IDisposable
{
    private readonly AudioEngine _engine;
    private readonly AudioOutputMonitor _monitor;
    private readonly HannSpectrumAnalyzer _analyzer;
    private readonly short[] _pcm;
    private readonly float[] _preClamp;
    private readonly float[] _workingSpectrum;
    private readonly float[] _publishedSpectrum;
    private readonly object _snapshotSync = new();
    private readonly Lock _disposeSync = new();
    private readonly ManualResetEventSlim _stop = new(false);
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Thread _worker;
    private readonly TimeSpan _refreshInterval;
    private AudioSpectrumFrame _frame;
    private bool _hasFrame;
    private bool _disposed;

    /// <summary>
    ///  Attaches bounded monitoring and starts an analyzer worker; the caller still owns the engine.
    /// </summary>
    public AudioSpectrumSource(AudioEngine engine, AudioSpectrumOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _engine = engine;
        Options = options ?? new();
        Options.Validate();
        _analyzer = new(Options);
        _pcm = new short[checked(Options.FftSize * engine.Format.Channels)];
        _preClamp = new float[Options.FftSize];
        _workingSpectrum = new float[BinCount];
        _publishedSpectrum = new float[BinCount];
        _refreshInterval = TimeSpan.FromSeconds(1d / Options.RefreshRate);
        _monitor = engine.AttachOutputMonitor(Options.FftSize);
        try
        {
            _worker = new Thread(Analyze)
            {
                Name = "Audio spectrum",
                IsBackground = true
            };
            _worker.Start();
        }
        catch
        {
            engine.DetachOutputMonitor(_monitor);
            _stop.Dispose();
            throw;
        }
    }

    /// <summary>
    ///  Gets the number of one-sided FFT bins, including both DC and Nyquist.
    /// </summary>
    public int BinCount
        => Options.FftSize / 2 + 1;

    /// <summary>
    ///  Gets the immutable configuration of this source.
    /// </summary>
    public AudioSpectrumOptions Options { get; }

    /// <summary>
    ///  Completes on disposal or engine shutdown and faults on analyzer failure, independently of playback.
    /// </summary>
    public Task Completion
        => _completion.Task;

    /// <summary>
    ///  Copies one coherent finished spectrum, or returns false until a complete usable window exists.
    /// </summary>
    /// <param name="decibels">Caller-owned storage for at least BinCount calibrated dBFS values.</param>
    /// <param name="frame">Metadata describing exactly the copied bins.</param>
    public bool TryCopySpectrum(Span<float> decibels, out AudioSpectrumFrame frame)
    {
        if (decibels.Length < BinCount)
        {
            throw new ArgumentException($"Spectrum storage needs at least {BinCount} bins.", nameof(decibels));
        }

        lock (_snapshotSync)
        {
            frame = default;
            if (!_hasFrame || _monitor.IsStopped)
            {
                return false;
            }

            _publishedSpectrum.AsSpan().CopyTo(decibels);
            frame = _frame;
            return true;
        }
    }

    /// <summary>
    ///  Stops this analyzer and detaches its history without stopping the supplied audio engine.
    /// </summary>
    public void Dispose()
    {
        lock (_disposeSync)
        {
            if (_disposed)
            {
                return;
            }

            _engine.DetachOutputMonitor(_monitor);
            _stop.Set();
            if (!_worker.Join(TimeSpan.FromSeconds(2)))
            {
                // Keep the signal alive for a retry; a custom progress getter must not block.
                throw new TimeoutException("The spectrum analyzer did not stop. Playback-progress getters must be nonblocking.");
            }

            _stop.Dispose();
            _disposed = true;
        }
    }

    private void Analyze()
    {
        Exception? failure = null;
        long sequence = 0;
        long lastEndFrame = -1;
        try
        {
            while (!_stop.IsSet && !_monitor.IsStopped)
            {
                bool synchronized = _engine.TryGetPlaybackPosition(out long endFrame);
                if (endFrame != lastEndFrame)
                {
                    if (_monitor.TryCopyWindow(endFrame, _pcm, _preClamp, out AudioOutputWindow window))
                    {
                        SpectrumLevels levels = _analyzer.Analyze(_pcm, _preClamp, _monitor.Format, _workingSpectrum);
                        AudioSpectrumFrame frame = new(
                            ++sequence,
                            window.EndFrame,
                            _monitor.Format.SampleRate,
                            Options.FftSize,
                            levels.PeakFrequency,
                            levels.PeakLevel,
                            levels.RmsLevel,
                            levels.ClippedSamples,
                            window.DroppedBlocks,
                            synchronized);

                        // Only these two short copies are locked. No FFT work runs under the
                        // history gate or the UI snapshot gate, so a slow painter cannot stall audio.
                        lock (_snapshotSync)
                        {
                            _workingSpectrum.CopyTo(_publishedSpectrum, 0);
                            _frame = frame;
                            _hasFrame = true;
                        }

                        lastEndFrame = endFrame;
                    }
                    else
                    {
                        lock (_snapshotSync)
                        {
                            _hasFrame = false;
                        }
                    }
                }

                _stop.Wait(_refreshInterval);
            }
        }
        catch (Exception error)
        {
            // Monitoring failure must remain observable, but must never escape onto the
            // healthy audio pump. Detaching also removes its optional per-frame statistics work.
            failure = error;
        }
        finally
        {
            _engine.DetachOutputMonitor(_monitor);
            lock (_snapshotSync)
            {
                _hasFrame = false;
            }

            if (failure is null)
            {
                _completion.TrySetResult();
            }
            else
            {
                _completion.TrySetException(failure);
            }
        }
    }
}
