namespace SplitFlap.Audio.Percussion;

/// <summary>
///  Centralizes nominal tuning, D20 decay times, filter choices, and conservative output gains.
/// </summary>
internal readonly record struct Cr78Preset
{
    private Cr78Preset(
        double frequency,
        double secondFrequency,
        double thirdFrequency,
        double decayMilliseconds,
        double attackMilliseconds,
        float level,
        double highPassHz,
        double lowPassHz)
    {
        Frequency = frequency;
        SecondFrequency = secondFrequency;
        ThirdFrequency = thirdFrequency;
        DecayMilliseconds = decayMilliseconds;
        AttackMilliseconds = attackMilliseconds;
        Level = level;
        HighPassHz = highPassHz;
        LowPassHz = lowPassHz;
    }

    /// <summary>
    ///  Gets the principal resonance or first metallic oscillator in hertz.
    /// </summary>
    internal double Frequency { get; }

    /// <summary>
    ///  Gets the secondary membrane, wood, or metallic component in hertz.
    /// </summary>
    internal double SecondFrequency { get; }

    /// <summary>
    ///  Gets the third metallic component in hertz, or zero when not used.
    /// </summary>
    internal double ThirdFrequency { get; }

    /// <summary>
    ///  Gets the time to one-tenth voltage amplitude, equivalent to minus 20 dB, in milliseconds.
    /// </summary>
    internal double DecayMilliseconds { get; }

    /// <summary>
    ///  Gets the brief digital excitation ramp in milliseconds, not a measured hardware value.
    /// </summary>
    internal double AttackMilliseconds { get; }

    /// <summary>
    ///  Gets a peak bound; the fourteen bounds sum to 0.905, leaving summing headroom.
    /// </summary>
    internal float Level { get; }

    /// <summary>
    ///  Gets the approximate noise/output high-pass cutoff in hertz.
    /// </summary>
    internal double HighPassHz { get; }

    /// <summary>
    ///  Gets the approximate noise/output low-pass cutoff in hertz.
    /// </summary>
    internal double LowPassHz { get; }

    /// <summary>
    ///  Selects factory tuning and D20 targets, with explicitly approximate secondary/filter values.
    /// </summary>
    internal static Cr78Preset For(Cr78Instrument instrument)
        => instrument switch
        {
            Cr78Instrument.BassDrum => new(62.5, 135, 0, 100, 0.6, 0.140f, 35, 1_100),
            Cr78Instrument.SnareDrum => new(340, 185, 0, 60, 0.5, 0.100f, 1_000, 9_500),
            Cr78Instrument.RimShot => new(1_480, 2_430, 0, 5, 0.1, 0.120f, 600, 8_000),
            Cr78Instrument.HiHat => new(0, 0, 0, 60, 0.15, 0.050f, 6_500, 10_500),
            Cr78Instrument.Cymbal => new(0, 0, 0, 350, 0.4, 0.050f, 4_000, 10_000),
            Cr78Instrument.Maracas => new(0, 0, 0, 20, 0.2, 0.040f, 2_800, 9_500),
            Cr78Instrument.Claves => new(2_630, 5_260, 0, 18, 0.15, 0.040f, 800, 9_000),
            Cr78Instrument.Cowbell => new(800, 555, 0, 60, 0.2, 0.060f, 300, 5_000),
            Cr78Instrument.HighBongo => new(600, 978, 0, 40, 0.45, 0.040f, 500, 4_000),
            Cr78Instrument.LowBongo => new(400, 648, 0, 40, 0.6, 0.040f, 400, 3_500),
            Cr78Instrument.LowConga => new(208, 338, 0, 150, 0.7, 0.080f, 200, 3_000),
            Cr78Instrument.Tambourine => new(2_800, 4_170, 6_300, 220, 0.25, 0.055f, 3_500, 10_000),
            Cr78Instrument.Guiro => new(2_110, 3_470, 0, 35, 0.6, 0.055f, 1_200, 8_000),
            Cr78Instrument.MetallicBeat => new(6_170, 5_620, 4_080, 50, 0.15, 0.035f, 3_000, 11_000),
            _ => throw new ArgumentOutOfRangeException(nameof(instrument))
        };
}
