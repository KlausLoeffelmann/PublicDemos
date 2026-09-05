namespace SplitFlap.Audio.Percussion;

/// <summary>
///  Owns one preallocated retriggerable channel per percussion sound, plus the metallic layer.
/// </summary>
internal sealed class Cr78VoiceBank
{
    private readonly Cr78Generator[] _generators = new Cr78Generator[14];

    /// <summary>
    ///  Prepares the whole palette outside the audio thread with independently continuing noise streams.
    /// </summary>
    internal Cr78VoiceBank(int sampleRate, uint noiseSeed)
    {
        Cr78Kit.ValidateSampleRate(sampleRate);
        ArgumentOutOfRangeException.ThrowIfZero(noiseSeed);
        for (int i = 0; i < _generators.Length; i++)
        {
            uint seed = (noiseSeed ^ (0x9E3779B9u * (uint)(i + 1))) | 1u;
            _generators[i] = new Cr78Generator(sampleRate, (Cr78Instrument)i, seed);
        }
    }

    /// <summary>
    ///  Gets whether any audible strike or release remains in the bank.
    /// </summary>
    internal bool IsActive
    {
        get
        {
            foreach (Cr78Generator generator in _generators)
            {
                if (generator.IsActive)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    ///  Excites a single prepared sound; the caller decides when to trigger the shared metallic layer.
    /// </summary>
    internal void Trigger(Cr78Instrument instrument, float velocity, long gateSamples)
        => _generators[(int)instrument].Trigger(velocity, gateSamples);

    /// <summary>
    ///  Gracefully cancels every current strike without making future auditions impossible.
    /// </summary>
    internal void ReleaseAll()
    {
        foreach (Cr78Generator generator in _generators)
        {
            generator.Release();
        }
    }

    /// <summary>
    ///  Sums the fixed channels; their configured peak bounds together leave ten percent headroom.
    /// </summary>
    internal float Next()
    {
        float sample = 0;
        foreach (Cr78Generator generator in _generators)
        {
            sample += generator.Next();
        }

        return sample;
    }
}
