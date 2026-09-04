namespace SplitFlap.Demo;

/// <summary>
///  Glue between the animator and the audio engine: every fallen flap becomes a clack, every jam a
///  short buzz. Lives entirely on the animator and engine threads; the UI never hears about it.
/// </summary>
internal sealed class BoardSound : IDisposable
{
    private const int MaxClacksPerFrame = 12;

    private readonly SplitFlapAnimator _animator;
    private readonly AudioEngine _engine;
    private readonly VoiceChannel _clacks;
    private readonly VoiceChannel _buzz;
    private long _lastFrameTick;
    private int _clacksThisFrame;

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

    public VoiceChannel CreateMelodyChannel(VoicePatch patch)
        => Melody = _engine.CreateChannel(patch);

    private void OnFlapFell(object? sender, FlapEventArgs e)
    {
        // Forty flaps falling in the same 16 ms frame are perceived as one big clatter anyway.
        // Beyond a dozen voices the mix only gets louder, not busier, so we cap per frame.
        long now = Environment.TickCount64;

        if (now - _lastFrameTick > 8)
        {
            _lastFrameTick = now;
            _clacksThisFrame = 0;
        }

        if (++_clacksThisFrame <= MaxClacksPerFrame)
        {
            _clacks.Trigger(new ClackVoice(_engine.SampleRate, volume: 0.28f));
        }
    }

    private void OnJammed(object? sender, FlapEventArgs e)
        => _ = _buzz.PlaySoundAsync(Sound.Of(55, 220));

    public void Dispose()
    {
        _animator.FlapFell -= OnFlapFell;
        _animator.Jammed -= OnJammed;
        _engine.Dispose();
    }
}
