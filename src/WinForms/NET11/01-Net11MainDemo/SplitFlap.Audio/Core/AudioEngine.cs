using System.Collections.Concurrent;

namespace SplitFlap.Audio.Core;

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
    private readonly ConcurrentQueue<ActiveVoice> _incoming = new();
    private readonly List<ActiveVoice> _active = [];
    private readonly float[] _dry;
    private readonly float[] _wet;
    private readonly short[] _pcm;
    private readonly Reverb _reverb;
    private Thread? _pump;
    private volatile bool _stopping;
    private bool _disposed;

    private AudioEngine(IAudioSink sink)
    {
        _sink = sink;
        int frames = sink.FramesPerBuffer;
        _dry = new float[frames];
        _wet = new float[frames];
        _pcm = new short[frames * sink.Format.Channels];
        _reverb = new Reverb(sink.Format.SampleRate);
    }

    /// <summary>The output format. Voices are built for <see cref="AudioFormat.SampleRate"/>.</summary>
    public AudioFormat Format
        => _sink.Format;

    /// <summary>Sample rate shortcut for voice constructors.</summary>
    public int SampleRate
        => _sink.Format.SampleRate;

    /// <summary>Upper bound on simultaneous voices. Beyond that, the oldest voice is stolen.</summary>
    public int MaxPolyphony { get; set; } = 48;

    /// <summary>Master gain before the 16-bit conversion. Leave headroom; 40 clacks add up.</summary>
    public float MasterVolume { get; set; } = 0.8f;

    /// <summary>Reverb bus settings. Voices contribute via their send level.</summary>
    public ReverbSettings Reverb { get; set; } = ReverbSettings.Room;

    /// <summary>Number of voices currently sounding.</summary>
    public int ActiveVoices
        => _active.Count;

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
        ArgumentNullException.ThrowIfNull(voice);
        ObjectDisposedException.ThrowIf(_disposed, this);

        ActiveVoice entry = new(voice, Math.Clamp(reverbSend, 0f, 1f));
        _incoming.Enqueue(entry);

        return entry.Completion.Task;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _stopping = true;
        _pump?.Join(1000);
        _sink.Dispose();

        foreach (ActiveVoice entry in _active)
        {
            entry.Completion.TrySetCanceled();
        }

        while (_incoming.TryDequeue(out ActiveVoice? pending))
        {
            pending.Completion.TrySetCanceled();
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
        int frames = _dry.Length;
        int channels = Format.Channels;

        while (!_stopping)
        {
            AdmitIncoming();

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
                    entry.Completion.TrySetResult();
                }
            }

            _reverb.Process(Reverb, _wet, _dry);

            float gain = MasterVolume * short.MaxValue;

            for (int i = 0; i < frames; i++)
            {
                short value = (short)Math.Clamp(_dry[i] * gain, short.MinValue, short.MaxValue);

                for (int c = 0; c < channels; c++)
                {
                    _pcm[i * channels + c] = value;
                }
            }

            try
            {
                _sink.Write(_pcm);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
        }
    }

    private void AdmitIncoming()
    {
        while (_incoming.TryDequeue(out ActiveVoice? entry))
        {
            while (_active.Count >= MaxPolyphony)
            {
                // Voice stealing: the oldest goes, immediately. Real synths do the same.
                ActiveVoice stolen = _active[0];
                _active.RemoveAt(0);
                stolen.Completion.TrySetResult();
            }

            _active.Add(entry);
        }
    }

    private sealed class ActiveVoice(IVoice voice, float reverbSend)
    {
        public IVoice Voice { get; } = voice;
        public float ReverbSend { get; } = reverbSend;
        public TaskCompletionSource Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
