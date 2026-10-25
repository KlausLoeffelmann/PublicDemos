using System.Collections.ObjectModel;

namespace WinForms.Audio.Percussion;

/// <summary>
///  Creates dry, procedural CR-78-style sounds without recordings or an audio-device dependency.
/// </summary>
/// <remarks>
///  These are fitted modal/colored-noise models, not circuit emulation or recorded samples.
///  The adjacent README describes the measured targets and remaining approximations.
/// </remarks>
public static class Cr78Kit
{
    private static readonly ReadOnlyCollection<Cr78Instrument> s_instruments =
        Array.AsReadOnly(new[]
        {
            Cr78Instrument.BassDrum, Cr78Instrument.BassDrumAccent,
            Cr78Instrument.SnareDrum, Cr78Instrument.SnareDrumAccent, Cr78Instrument.RimShot,
            Cr78Instrument.HiHat, Cr78Instrument.HiHatAccent, Cr78Instrument.MetallicHiHat,
            Cr78Instrument.Cymbal, Cr78Instrument.HighBongo, Cr78Instrument.LowBongo,
            Cr78Instrument.LowConga, Cr78Instrument.Cowbell, Cr78Instrument.Tambourine,
            Cr78Instrument.Tambourine2, Cr78Instrument.Guiro, Cr78Instrument.Guiro2
        });

    /// <summary>
    ///  The lowest supported rate; lower rates cannot retain the full bright-noise palette.
    /// </summary>
    public const int MinimumSampleRate = 32_000;

    /// <summary>
    ///  The highest supported rate, bounding the size of prepared synthesis resources.
    /// </summary>
    public const int MaximumSampleRate = 192_000;

    /// <summary>
    ///  The seventeen fixed stock tracks, excluding legacy sounds and the auxiliary metallic layer.
    /// </summary>
    public static IReadOnlyList<Cr78Instrument> Instruments
        => s_instruments;

    /// <summary>
    ///  Gets the human-readable name of a percussion sound or the metallic-layer audition.
    /// </summary>
    public static string GetDisplayName(Cr78Instrument instrument)
        => instrument switch
        {
            Cr78Instrument.BassDrum => "Kick drum",
            Cr78Instrument.BassDrumAccent => "Accented kick drum",
            Cr78Instrument.SnareDrum => "Snare drum",
            Cr78Instrument.SnareDrumAccent => "Accented snare drum",
            Cr78Instrument.RimShot => "Rim shot",
            Cr78Instrument.HiHat => "Hi-hat",
            Cr78Instrument.HiHatAccent => "Accented hi-hat",
            Cr78Instrument.MetallicHiHat => "Metallic hi-hat",
            Cr78Instrument.Cymbal => "Cymbal",
            Cr78Instrument.Maracas => "Maracas",
            Cr78Instrument.Claves => "Claves",
            Cr78Instrument.Cowbell => "Cowbell",
            Cr78Instrument.HighBongo => "High bongo",
            Cr78Instrument.LowBongo => "Low bongo",
            Cr78Instrument.LowConga => "Low conga",
            Cr78Instrument.Tambourine => "Tambourine 1",
            Cr78Instrument.Tambourine2 => "Tambourine 2",
            Cr78Instrument.Guiro => "Guiro 1",
            Cr78Instrument.Guiro2 => "Guiro 2",
            Cr78Instrument.MetallicBeat => "Metallic beat (CY/HH layer)",
            _ => throw new ArgumentOutOfRangeException(nameof(instrument))
        };

    /// <summary>
    ///  Creates an independent one-shot audition. The guiro's default gate is a quarter second.
    /// </summary>
    /// <param name="sampleRate">A supported engine rate, normally 44,100 or 48,000 Hz.</param>
    /// <param name="instrument">A percussion sound or the metallic-only audition.</param>
    /// <param name="velocity">Finite strike strength from zero to one.</param>
    /// <param name="metallicLevel">Finite layer level from zero to one, used only by hi-hat/cymbal.</param>
    /// <returns>A voice to pass to an engine, or render directly without an endpoint.</returns>
    public static IVoice CreateVoice(
        int sampleRate,
        Cr78Instrument instrument,
        float velocity = 1f,
        float metallicLevel = 0f)
        => CreateVoice(sampleRate, instrument, velocity, metallicLevel,
            (uint)Random.Shared.Next(1, int.MaxValue));

    /// <summary>
    ///  Creates a repeatable one-shot for tests; subsequent retriggers never reseed its noise.
    /// </summary>
    internal static IVoice CreateVoice(
        int sampleRate,
        Cr78Instrument instrument,
        float velocity,
        float metallicLevel,
        uint noiseSeed)
    {
        ValidateSampleRate(sampleRate);
        ValidateInstrument(instrument);
        ValidateLevel(velocity, nameof(velocity));
        ValidateLevel(metallicLevel, nameof(metallicLevel));
        ArgumentOutOfRangeException.ThrowIfZero(noiseSeed);
        return new OneShot(sampleRate, instrument, velocity, metallicLevel, noiseSeed);
    }

    /// <summary>
    ///  Rejects rates for which this full-palette model has not been prepared.
    /// </summary>
    internal static void ValidateSampleRate(int sampleRate)
    {
        if (sampleRate < MinimumSampleRate || sampleRate > MaximumSampleRate)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleRate),
                $"The full palette supports {MinimumSampleRate:N0}–{MaximumSampleRate:N0} Hz.");
        }
    }

    /// <summary>
    ///  Rejects undefined enum values, optionally excluding the audition-only metallic entry.
    /// </summary>
    internal static void ValidateInstrument(Cr78Instrument instrument, bool allowMetallic = true)
    {
        if (!Enum.IsDefined(instrument) || (!allowMetallic && instrument == Cr78Instrument.MetallicBeat))
        {
            throw new ArgumentOutOfRangeException(nameof(instrument),
                allowMetallic ? "Unknown percussion sound." : "The auxiliary metallic layer is not a score track.");
        }
    }

    /// <summary>
    ///  Maps retired stock track identifiers to their replacements when reading older documents.
    /// </summary>
    public static Cr78Instrument GetCurrentInstrument(Cr78Instrument instrument)
        => instrument switch
        {
            Cr78Instrument.Maracas => Cr78Instrument.Tambourine2,
            Cr78Instrument.Claves => Cr78Instrument.Guiro2,
            _ => instrument
        };

    /// <summary>
    ///  Gets a stock fader's position independently of stable enum identifiers.
    /// </summary>
    internal static int GetInstrumentIndex(Cr78Instrument instrument)
    {
        int index = s_instruments.IndexOf(GetCurrentInstrument(instrument));
        return index >= 0 ? index : throw new ArgumentOutOfRangeException(nameof(instrument));
    }

    /// <summary>
    ///  Rejects non-finite or out-of-range velocity and level values rather than hiding bad input.
    /// </summary>
    internal static void ValidateLevel(float value, string parameterName)
    {
        if (!float.IsFinite(value) || value < 0f || value > 1f)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The value must be finite and between zero and one.");
        }
    }

    private sealed class OneShot : IVoice
    {
        private readonly Cr78Generator _sound;
        private readonly Cr78Generator? _metallic;
        private int _releaseRequested;
        private bool _releasing;

        /// <summary>
        ///  Prepares a single audition and, when requested, its independently decaying metallic layer.
        /// </summary>
        internal OneShot(int sampleRate, Cr78Instrument instrument, float velocity, float metallicLevel, uint seed)
        {
            _sound = new Cr78Generator(sampleRate, instrument, seed);
            _sound.Trigger(velocity, instrument == Cr78Instrument.Guiro2 ? sampleRate * 4 / 5 : sampleRate / 4);
            if (metallicLevel > 0f && instrument is Cr78Instrument.HiHat or Cr78Instrument.Cymbal)
            {
                _metallic = new Cr78Generator(sampleRate, Cr78Instrument.MetallicBeat, (seed ^ 0x9E3779B9u) | 1u);
                _metallic.Trigger(velocity * metallicLevel, sampleRate / 4);
            }
        }

        /// <summary>
        ///  Gets whether both the sound and its optional metallic tail have reached exact silence.
        /// </summary>
        public bool IsFinished
            => !_sound.IsActive && (_metallic is null || !_metallic.IsActive);

        /// <summary>
        ///  Renders the next sample, accepting cross-thread release only on the rendering thread.
        /// </summary>
        public float Next()
        {
            if (!_releasing && Volatile.Read(ref _releaseRequested) != 0)
            {
                _releasing = true;
                _sound.Release();
                _metallic?.Release();
            }

            return _sound.Next() + (_metallic?.Next() ?? 0f);
        }

        /// <summary>
        ///  Requests a short click-free release without modifying DSP state on the caller's thread.
        /// </summary>
        public void Release()
            => Volatile.Write(ref _releaseRequested, 1);
    }
}
