namespace SplitFlap.Demo;

/// <summary>
///  Glue between the animator and the audio engine: every fallen flap becomes a clack, every jam a
///  short buzz. Voices are queued on the animator thread and rendered on the engine thread;
///  the UI observes only the engine's lifetime task for terminal failures.
/// </summary>
internal sealed class BoardSound : IDisposable
{
    private const int MaxClacksPerFrame = 12;

    private readonly SplitFlapAnimator _animator;
    private readonly Lock _eventSync = new();
    private readonly AudioEngine _engine;
    private readonly VoiceChannel _clacks;
    private readonly VoiceChannel _buzz;
    private long _lastFrameTick;
    private int _clacksThisFrame;
    private bool _disposed;

    /// <summary>
    ///  Connects board events to a shared synthesis engine.
    /// </summary>
    public BoardSound(SplitFlapAnimator animator)
    {
        _animator = animator;
        _engine = AudioEngine.Create();
        _engine.Reverb = ReverbSettings.Hall;
        _engine.MaxPolyphony = 64;

        _clacks = _engine.CreateChannel();
        _clacks.ReverbSend = 0.35f;

        _buzz = _engine.CreateChannel(VoicePatch.Lead with { Volume = 0.15f });
        _buzz.ReverbSend = 0.1f;

        _animator.FlapFell += OnFlapFell;
        _animator.Jammed += OnJammed;
    }

    /// <summary>
    ///  A melody channel for the "play a tune" button. Same engine, different instrument.
    /// </summary>
    public VoiceChannel Melody { get; private set; } = null!;

    /// <summary>
    ///  Gets the output sample rate used to build voices.
    /// </summary>
    public int SampleRate
        => _engine.SampleRate;

    /// <summary>
    ///  Completes on normal shutdown, or faults if audio rendering or device output fails.
    /// </summary>
    public Task Completion
        => _engine.Completion;

    /// <summary>
    ///  Creates the instrument channel used by the tune button.
    /// </summary>
    public VoiceChannel CreateMelodyChannel(VoicePatch patch)
        => Melody = _engine.CreateChannel(patch);

    private void OnFlapFell(object? sender, FlapEventArgs e)
    {
        // Unsubscribing does not cancel a handler already in progress on the animator.
        // Finish that short admission before disposal closes the engine.
        lock (_eventSync)
        {
            if (!_disposed)
            {
                TriggerClack();
            }
        }
    }

    private void TriggerClack()
    {
        // The animator advances every visual in one frame, so without an offset all resulting
        // clacks would enter the next audio block on exactly the same sample. Real flap shafts
        // and solenoids have small mechanical tolerances. Spread this frame's strikes across
        // roughly six milliseconds, with a little jitter, to reproduce that loose clatter.
        long now = Environment.TickCount64;

        if (now - _lastFrameTick > 8)
        {
            _lastFrameTick = now;
            _clacksThisFrame = 0;
        }

        int clackIndex = _clacksThisFrame++;

        // Beyond a dozen voices the mix only gets louder, not busier, so retain the cap.
        if (clackIndex < MaxClacksPerFrame)
        {
            double delayMilliseconds =
                clackIndex * 0.5
                + 0.1
                + Random.Shared.NextDouble() * 0.3;

            _clacks.Trigger(
                new ClackVoice(
                    _engine.SampleRate,
                    volume: 0.25f,
                    startDelay: TimeSpan.FromMilliseconds(delayMilliseconds),
                    attackMilliseconds: 1.5f));
        }
    }

    private void OnJammed(object? sender, FlapEventArgs e)
    {
        lock (_eventSync)
        {
            if (!_disposed)
            {
                _buzz.Trigger(new ToneVoice(
                    _engine.SampleRate,
                    _buzz.Patch,
                    55,
                    TimeSpan.FromMilliseconds(220),
                    _buzz.Volume));
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_eventSync)
        {
            if (!_disposed)
            {
                _disposed = true;
                _animator.FlapFell -= OnFlapFell;
                _animator.Jammed -= OnJammed;
            }
        }

        _engine.Dispose();
    }
}
