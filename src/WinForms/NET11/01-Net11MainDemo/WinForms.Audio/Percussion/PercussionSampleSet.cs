namespace WinForms.Audio.Percussion;

/// <summary>
///  Owns validated mono sample copies prepared before an atomic audio-block handoff.
/// </summary>
internal sealed class PercussionSampleSet
{
    private readonly float[]?[] _samples = new float[Enum.GetValues<Cr78Instrument>().Length][];

    /// <summary>
    ///  The immutable stock mapping, with no recorded samples.
    /// </summary>
    internal static PercussionSampleSet Stock { get; } = new();

    private PercussionSampleSet()
    {
    }

    /// <summary>
    ///  Copies caller-owned PCM and rejects unsupported tracks, rates, lengths and amplitudes.
    /// </summary>
    internal PercussionSampleSet(IReadOnlyDictionary<Cr78Instrument, Sample> samples, int sampleRate)
    {
        ArgumentNullException.ThrowIfNull(samples);
        foreach ((Cr78Instrument instrument, Sample sample) in samples)
        {
            if (!Cr78Kit.Instruments.Contains(instrument))
            {
                throw new ArgumentOutOfRangeException(nameof(samples), "Only fixed stock tracks can be remapped.");
            }

            ArgumentNullException.ThrowIfNull(sample);
            if (sample.SampleRate != sampleRate || sample.Data.Length == 0 || sample.Data.Length > sampleRate * 30)
            {
                throw new ArgumentException("Percussion WAVs must match the engine rate and last at most thirty seconds.", nameof(samples));
            }

            float[] copy = (float[])sample.Data.Clone();
            foreach (float value in copy)
            {
                if (!float.IsFinite(value) || Math.Abs(value) > 1)
                {
                    throw new ArgumentException("Percussion samples must contain finite audio within -1..1.", nameof(samples));
                }
            }

            _samples[(int)instrument] = copy;
        }
    }

    /// <summary>
    ///  Gets owned PCM without allocating or exposing it publicly.
    /// </summary>
    internal float[]? this[Cr78Instrument instrument] => _samples[(int)instrument];
}
