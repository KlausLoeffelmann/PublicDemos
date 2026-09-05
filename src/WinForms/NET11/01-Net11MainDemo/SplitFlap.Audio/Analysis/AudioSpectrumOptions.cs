namespace SplitFlap.Audio.Analysis;

/// <summary>
///  Configures a spectrum source; a larger window resolves lower frequencies but spans more audio time.
/// </summary>
public sealed record AudioSpectrumOptions
{
    /// <summary>
    ///  Gets the power-of-two frame count, from 64 through 65,536; 4096 spans about 85 ms at 48 kHz.
    /// </summary>
    public int FftSize { get; init; } = 4096;

    /// <summary>
    ///  Gets the analyzer refresh rate, from one through 120 updates per second.
    /// </summary>
    public int RefreshRate { get; init; } = 30;

    /// <summary>
    ///  Gets the finite silence floor in dBFS, from -300 through -1 dBFS.
    /// </summary>
    public float MinimumDecibels { get; init; } = -90;

    /// <summary>
    ///  Validates configuration before a source allocates history or attaches to an engine.
    /// </summary>
    internal void Validate()
    {
        if (FftSize is < 64 or > 65_536 || (FftSize & (FftSize - 1)) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(FftSize), "FFT size must be a power of two from 64 through 65,536.");
        }

        if (RefreshRate is < 1 or > 120)
        {
            throw new ArgumentOutOfRangeException(nameof(RefreshRate), "Refresh rate must be between one and 120 Hz.");
        }

        if (!float.IsFinite(MinimumDecibels) || MinimumDecibels is < -300 or > -1)
        {
            throw new ArgumentOutOfRangeException(nameof(MinimumDecibels), "The silence floor must be finite and between -300 and -1 dBFS.");
        }
    }
}
