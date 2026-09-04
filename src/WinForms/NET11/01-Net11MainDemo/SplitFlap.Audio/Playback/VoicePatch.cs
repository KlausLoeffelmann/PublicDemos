namespace SplitFlap.Audio.Playback;

/// <summary>
///  The parameter set of one tone generator: the "instrument". Immutable; use <c>with</c> to vary.
/// </summary>
/// <param name="Waveform">Wave shape.</param>
/// <param name="Envelope">ADSR.</param>
/// <param name="PulseWidth">Duty cycle for <see cref="Waveform.Pulse"/>.</param>
/// <param name="Volume">Base gain, 0..1.</param>
public sealed record VoicePatch(
    Waveform Waveform = Waveform.Sine,
    EnvelopeSettings Envelope = default,
    float PulseWidth = 0.25f,
    float Volume = 0.5f)
{
    /// <summary>
    ///  ADSR, defaulting to <see cref="EnvelopeSettings.Default"/> when unset.
    /// </summary>
    public EnvelopeSettings Envelope { get; init; } = Envelope == default ? EnvelopeSettings.Default : Envelope;

    /// <summary>
    ///  Plain sine with a snappy envelope.
    /// </summary>
    public static VoicePatch Default
        => new();

    /// <summary>
    ///  1981: a pulse wave with a slow swell and long tail. Play chords.
    /// </summary>
    public static VoicePatch Pad
        => new(Waveform.Pulse, EnvelopeSettings.Pad, 0.35f, 0.3f);

    /// <summary>
    ///  Plucked sawtooth. Bass lines, arpeggios.
    /// </summary>
    public static VoicePatch Pluck
        => new(Waveform.Sawtooth, EnvelopeSettings.Pluck, Volume: 0.4f);

    /// <summary>
    ///  Square-wave lead. The C64 in one patch.
    /// </summary>
    public static VoicePatch Lead
        => new(Waveform.Square, new EnvelopeSettings(5, 60, 0.6f, 100), Volume: 0.35f);

    /// <summary>
    ///  Triangle organ. Soft, no decay.
    /// </summary>
    public static VoicePatch Organ
        => new(Waveform.Triangle, EnvelopeSettings.Organ, Volume: 0.5f);
}

/// <summary>
///  A raw tone request: a frequency for a length. No music theory attached.
/// </summary>
/// <param name="Frequency">Pitch in Hz.</param>
/// <param name="Length">Gate length.</param>
public readonly record struct Sound(double Frequency, TimeSpan Length)
{
    /// <summary>
    ///  Convenience: frequency and milliseconds.
    /// </summary>
    public static Sound Of(double frequency, int milliseconds)
        => new(frequency, TimeSpan.FromMilliseconds(milliseconds));
}
