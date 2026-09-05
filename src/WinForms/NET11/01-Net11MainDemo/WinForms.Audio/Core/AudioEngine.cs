using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace WinForms.Audio.Core;

/// <summary>
///  The pump. One dedicated thread asks every active voice for its next sample, sums them in
///  <see langword="float"/>, runs the reverb bus, converts to 16-bit once, and hands the block to
///  the sink, which blocks until the device wants more. That blocking is the clock.
/// </summary>
/// <remarks>
///  Nothing in here is ever "played". Voices are enqueued from any thread, picked up at the next
///  block, and dropped when they report <see cref="IVoice.IsFinished"/>. The task returned by
///  <see cref="Play"/> completes at that moment, i.e. after the release stage, not at note-off.
/// </remarks>
public sealed class AudioEngine : IDisposable
{
    private readonly IAudioSink _sink;
    private readonly Lock _admissionSync = new();
    private readonly Lock _disposeSync = new();
    private readonly ConcurrentQueue<ActiveVoice> _incoming = new();
    private readonly List<ActiveVoice> _active = [];
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly float[] _dry;
    private readonly float[] _wet;
    private readonly short[] _pcm;
    private readonly float[] _preClampPcm;
    private readonly Reverb _reverb;
    private Thread? _pump;
    private Exception? _pumpFailure;
    private volatile bool _stopping;
    private bool _disposed;
    private bool _shutdownComplete;
    private int _maxPolyphony = 48;
    private long _renderedFrames;
    private long _submittedFrames;
    private AudioOutputMonitor[] _outputMonitors = [];

    private AudioEngine(IAudioSink sink)
    {
        _sink = sink;
        int frames = sink.FramesPerBuffer;
        _dry = new float[frames];
        _wet = new float[frames];
        _pcm = new short[frames * sink.Format.Channels];
        _preClampPcm = new float[frames];
        _reverb = new Reverb(sink.Format.SampleRate);
    }

    /// <summary>
    ///  The output format. Voices are built for <see cref="AudioFormat.SampleRate"/>.
    /// </summary>
    public AudioFormat Format
        => _sink.Format;

    /// <summary>
    ///  Sample rate shortcut for voice constructors.
    /// </summary>
    public int SampleRate
        => _sink.Format.SampleRate;

    /// <summary>
    ///  Upper bound on simultaneous voices. Beyond that, the oldest voice is stolen.
    /// </summary>
    public int MaxPolyphony
    {
        get => Volatile.Read(ref _maxPolyphony);
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            Volatile.Write(ref _maxPolyphony, value);
        }
    }

    /// <summary>
    ///  Master gain before the 16-bit conversion. Leave headroom; 40 clacks add up.
    /// </summary>
    public float MasterVolume { get; set; } = 0.8f;

    /// <summary>
    ///  Reverb bus settings. Voices contribute via their send level.
    /// </summary>
    public ReverbSettings Reverb { get; set; } = ReverbSettings.Room;

    /// <summary>
    ///  Number of voices currently sounding.
    /// </summary>
    public int ActiveVoices
        => _active.Count;

    /// <summary>
    ///  Completes when the pump stops, or faults if rendering or device output fails.
    /// </summary>
    /// <remarks>
    ///  Observe this task even when using only one-shot triggers. It reports a terminal failure
    ///  once per engine, without allocating a task for each clack. Normal disposal completes it
    ///  successfully; per-voice playback tasks are cancelled when disposal interrupts them.
    /// </remarks>
    public Task Completion
        => _completion.Task;

    /// <summary>
    ///  Gets the number of generated frames. While voices render a block, this is its starting frame.
    /// </summary>
    public long RenderedFrames
        => Volatile.Read(ref _renderedFrames);

    /// <summary>
    ///  Gets the number of frames successfully handed to the sink.
    /// </summary>
    public long SubmittedFrames
        => Volatile.Read(ref _submittedFrames);

    /// <summary>
    ///  Reads the completed device-buffer position, or the submitted position when no device clock exists.
    /// </summary>
    /// <param name="frames">Absolute frame position since this engine started.</param>
    /// <returns>True for a device-backed position; false for the explicitly approximate submitted position.</returns>
    public bool TryGetPlaybackPosition(out long frames)
    {
        if (_sink is IAudioPlaybackProgress progress)
        {
            frames = Math.Min(progress.CompletedFrames, SubmittedFrames);
            return true;
        }

        frames = SubmittedFrames;
        return false;
    }

    /// <summary>
    ///  Attaches a preallocated output history without inserting analysis or callbacks into the pump.
    /// </summary>
    internal AudioOutputMonitor AttachOutputMonitor(int windowSize)
    {
        int capacity = _sink is IAudioPlaybackProgress progress ? progress.BufferCapacityFrames : 0;
        AudioOutputMonitor monitor = new(Format, _dry.Length, capacity, windowSize);
        lock (_admissionSync)
        {
            ObjectDisposedException.ThrowIf(_disposed || _stopping, this);
            if (_pumpFailure is not null)
            {
                throw new InvalidOperationException("Cannot monitor an audio engine whose pump has failed.", _pumpFailure);
            }

            // Subscription changes allocate on the caller, not once per output block.
            Volatile.Write(ref _outputMonitors, [.. _outputMonitors, monitor]);
        }

        return monitor;
    }

    /// <summary>
    ///  Removes only this history subscription; it never stops the engine or another analyzer.
    /// </summary>
    internal void DetachOutputMonitor(AudioOutputMonitor monitor)
    {
        monitor.Stop();
        lock (_admissionSync)
        {
            int index = Array.IndexOf(_outputMonitors, monitor);
            if (index < 0)
            {
                return;
            }

            AudioOutputMonitor[] remaining = new AudioOutputMonitor[_outputMonitors.Length - 1];
            Array.Copy(_outputMonitors, 0, remaining, 0, index);
            Array.Copy(_outputMonitors, index + 1, remaining, index, remaining.Length - index);
            Volatile.Write(ref _outputMonitors, remaining);
        }
    }

    /// <summary>
    ///  Creates an engine over a sink and starts pumping. <see langword="null"/> opens the default
    ///  device through <see cref="WaveOutSink"/>.
    /// </summary>
    public static AudioEngine Create(IAudioSink? sink = null)
    {
        AudioEngine engine = new(sink ?? new WaveOutSink());
        engine.Start();

        return engine;
    }

    /// <summary>
    ///  Creates a channel: a patch, a volume, a reverb send, and the Play* methods.
    /// </summary>
    public VoiceChannel CreateChannel(VoicePatch? patch = null)
        => new(this, patch ?? VoicePatch.Default);

    /// <summary>
    ///  Adds a voice to the mix.
    /// </summary>
    /// <param name="voice">Any <see cref="IVoice"/>.</param>
    /// <param name="reverbSend">0..1 how much of the voice goes to the reverb bus.</param>
    /// <returns>A task that completes when the voice has finished, including its release.</returns>
    public Task Play(IVoice voice, float reverbSend = 0f)
    {
        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(voice, reverbSend, completion);
        return completion.Task;
    }

    /// <summary>
    ///  Queues a one-shot voice without allocating a playback completion task.
    /// </summary>
    /// <remarks>
    ///  Terminal playback failures are reported through <see cref="Completion"/>.
    /// </remarks>
    internal void Trigger(IVoice voice, float reverbSend)
        => Enqueue(voice, reverbSend, completion: null);

    /// <summary>
    ///  Closes admission, wakes the sink, and waits for the pump to release its voices.
    /// </summary>
    /// <remarks>
    ///  Call from the owning thread, not from a voice or sink callback. If native cleanup fails,
    ///  admission stays closed but another disposal attempt can retry the retained resources.
    /// </remarks>
    public void Dispose()
    {
        if (Thread.CurrentThread == _pump)
        {
            throw new InvalidOperationException("Dispose the audio engine from its owner, not its rendering thread.");
        }

        lock (_disposeSync)
        {
            if (_shutdownComplete)
            {
                return;
            }

            lock (_admissionSync)
            {
                _disposed = true;
                _stopping = true;
                StopOutputMonitors();
            }

            // A sink may be blocked waiting for a device buffer. Disposal is its wake-up signal,
            // so release it before joining the pump rather than paying a guaranteed timeout.
            Exception? disposalFailure = null;
            try
            {
                _sink.Dispose();
            }
            catch (Exception ex)
            {
                // Cleanup errors must reach the caller, but must not prevent joining the worker.
                disposalFailure = ex;
            }

            if (_pump is not null && !_pump.Join(1000))
            {
                TimeoutException timeout = new("The audio sink did not stop the rendering thread during disposal.");
                if (disposalFailure is not null)
                {
                    throw new AggregateException(disposalFailure, timeout);
                }

                throw timeout;
            }

            if (disposalFailure is not null)
            {
                ExceptionDispatchInfo.Throw(disposalFailure);
            }

            _shutdownComplete = true;
        }
    }

    private void Enqueue(IVoice voice, float reverbSend, TaskCompletionSource? completion)
    {
        ArgumentNullException.ThrowIfNull(voice);

        // This short lock protects admission and terminal state only, never synthesis or I/O.
        // Closing admission before draining the queue prevents a concurrent Play from leaving
        // a task stranded after shutdown or failure.
        lock (_admissionSync)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_pumpFailure is not null)
            {
                completion?.TrySetException(_pumpFailure);
                // A trigger has no individual task; Completion already reports this failure.
                return;
            }

            _incoming.Enqueue(new ActiveVoice(voice, Math.Clamp(reverbSend, 0f, 1f), completion));
        }
    }

    private void Start()
    {
        _pump = new Thread(Pump)
        {
            Name = "AudioEngine",
            IsBackground = true,
            Priority = ThreadPriority.Highest
        };

        _pump.Start();
    }

    private void Pump()
    {
        Exception? failure = null;
        try
        {
            int frames = _dry.Length;
            int channels = Format.Channels;

            while (!_stopping)
            {
                AdmitIncoming();

                // Mix in floating point so voices can add without integer overflow. Clamping is
                // deliberately postponed until the one final conversion to signed 16-bit PCM.
                Array.Clear(_dry);
                Array.Clear(_wet);

                for (int v = _active.Count - 1; v >= 0; v--)
                {
                    ActiveVoice entry = _active[v];
                    IVoice voice = entry.Voice;
                    float send = entry.ReverbSend;

                    for (int i = 0; i < frames; i++)
                    {
                        float sample = voice.Next();
                        _dry[i] += sample;
                        _wet[i] += sample * send;

                        if (voice.IsFinished)
                        {
                            break;
                        }
                    }

                    if (voice.IsFinished)
                    {
                        _active.RemoveAt(v);
                        entry.Completion?.TrySetResult();
                    }
                }

                _reverb.Process(Reverb, _wet, _dry);

                AudioOutputMonitor[] monitors = Volatile.Read(ref _outputMonitors);
                float gain = MasterVolume * short.MaxValue;

                for (int i = 0; i < frames; i++)
                {
                    float scaled = _dry[i] * gain;
                    short value = (short)Math.Clamp(scaled, short.MinValue, short.MaxValue);
                    if (monitors.Length != 0)
                    {
                        // Record the actual conversion input only when somebody needs it.
                        // Peak/clip calculations belong to the analyzer, not this audio loop.
                        _preClampPcm[i] = scaled;
                    }

                    for (int c = 0; c < channels; c++)
                    {
                        _pcm[i * channels + c] = value;
                    }
                }

                // The blocking sink write is the engine's clock: when the device consumes one
                // block, room becomes available for exactly the next block.
                long startFrame = _renderedFrames;
                Volatile.Write(ref _renderedFrames, startFrame + frames);
                _sink.Write(_pcm);
                Volatile.Write(ref _submittedFrames, _renderedFrames);
                foreach (AudioOutputMonitor monitor in monitors)
                {
                    // A failed sink write never reaches capture. Readers see the same PCM
                    // that was submitted, not a second synthesis or a render-ahead estimate.
                    monitor.TryWrite(startFrame, _pcm, _preClampPcm);
                }
            }
        }
        catch (ObjectDisposedException) when (_stopping)
        {
            // Disposal wakes a sink that is waiting for a native buffer.
        }
        catch (Exception ex)
        {
            failure = ex;
            lock (_admissionSync)
            {
                _pumpFailure = ex;
            }
        }
        finally
        {
            // Only the pump touches the active list, including during shutdown. The owner
            // waits for it instead of attempting concurrent cleanup after a join timeout.
            foreach (ActiveVoice entry in _active)
            {
                CompleteInterruptedVoice(entry, failure);
            }

            _active.Clear();
            while (_incoming.TryDequeue(out ActiveVoice? pending))
            {
                CompleteInterruptedVoice(pending, failure);
            }

            lock (_admissionSync)
            {
                StopOutputMonitors();
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

    private void StopOutputMonitors()
    {
        foreach (AudioOutputMonitor monitor in _outputMonitors)
        {
            monitor.Stop();
        }

        Volatile.Write(ref _outputMonitors, []);
    }

    private static void CompleteInterruptedVoice(ActiveVoice entry, Exception? failure)
    {
        if (failure is null)
        {
            entry.Completion?.TrySetCanceled();
        }
        else
        {
            entry.Completion?.TrySetException(failure);
        }
    }

    private void AdmitIncoming()
    {
        int maxPolyphony = MaxPolyphony;
        while (_incoming.TryDequeue(out ActiveVoice? entry))
        {
            while (_active.Count >= maxPolyphony)
            {
                // Voice stealing: the oldest goes, immediately. Real synths do the same.
                ActiveVoice stolen = _active[0];
                _active.RemoveAt(0);
                stolen.Completion?.TrySetResult();
            }

            _active.Add(entry);
        }
    }

    private sealed class ActiveVoice(IVoice voice, float reverbSend, TaskCompletionSource? completion)
    {
        public IVoice Voice { get; } = voice;
        public float ReverbSend { get; } = reverbSend;
        public TaskCompletionSource? Completion { get; } = completion;
    }
}
