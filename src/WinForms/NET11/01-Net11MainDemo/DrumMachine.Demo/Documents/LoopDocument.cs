using System.Collections.ObjectModel;
using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;

namespace DrumMachine.Demo.Documents;

/// <summary>
///  Captures an immutable editor score and its musical settings, independently of playback and UI state.
/// </summary>
internal sealed class LoopDocument : IEquatable<LoopDocument>
{
    /// <summary>
    ///  Bounds document events independently of the score's bar limit and the JSON byte limit.
    /// </summary>
    public const int MaximumHits = 65_536;

    /// <summary>
    ///  Creates a complete document; an omitted mixer defaults all thirteen levels to one hundred percent.
    /// </summary>
    public LoopDocument(
        PercussionScore score,
        int tempoBpm = 92,
        int masterVolumePercent = 65,
        IReadOnlyDictionary<Cr78Instrument, int>? percussionVolumes = null,
        bool loop = true,
        bool metallicEnabled = false,
        int metallicVolumePercent = 0)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(score.Hits.Count, MaximumHits);
        ValidateTempo(tempoBpm);
        ValidatePercent(masterVolumePercent, nameof(masterVolumePercent));
        ValidatePercent(metallicVolumePercent, nameof(metallicVolumePercent));

        Dictionary<Cr78Instrument, int> volumes = [];
        if (percussionVolumes is null)
        {
            foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
            {
                volumes.Add(instrument, 100);
            }
        }
        else
        {
            foreach ((Cr78Instrument instrument, int percent) in percussionVolumes)
            {
                ValidateInstrument(instrument);
                ValidatePercent(percent, nameof(percussionVolumes));
                volumes.Add(instrument, percent);
            }

            if (volumes.Count != Cr78Kit.Instruments.Count)
            {
                throw new ArgumentException(
                    "Percussion levels must include each of the thirteen score instruments.", nameof(percussionVolumes));
            }
        }

        // PercussionScore already owns and validates its events. Sharing it preserves empty bars,
        // exact velocities, and cross-bar gates without copying a score on every fader preview.
        Score = score;
        TempoBpm = tempoBpm;
        MasterVolumePercent = masterVolumePercent;
        PercussionVolumes = new ReadOnlyDictionary<Cr78Instrument, int>(volumes);
        Loop = loop;
        MetallicEnabled = metallicEnabled;
        MetallicVolumePercent = metallicVolumePercent;
    }

    /// <summary>
    ///  Gets the immutable score, including its empty bars and unquantized event parameters.
    /// </summary>
    public PercussionScore Score { get; }

    /// <summary>
    ///  Gets the editor tempo in whole beats per minute, from forty through two hundred forty.
    /// </summary>
    public int TempoBpm { get; }

    /// <summary>
    ///  Gets the master output level from zero through one hundred percent.
    /// </summary>
    public int MasterVolumePercent { get; }

    /// <summary>
    ///  Gets a defensive, read-only snapshot of all thirteen percussion levels.
    /// </summary>
    public IReadOnlyDictionary<Cr78Instrument, int> PercussionVolumes { get; }

    /// <summary>
    ///  Gets whether the score repeats after its final bar.
    /// </summary>
    public bool Loop { get; }

    /// <summary>
    ///  Gets whether the independent cymbal/hi-hat metallic layer is enabled.
    /// </summary>
    public bool MetallicEnabled { get; }

    /// <summary>
    ///  Gets the metallic amount remembered even while the layer is disabled.
    /// </summary>
    public int MetallicVolumePercent { get; }

    /// <summary>
    ///  Creates a blank score within the score's explicit one-to-4096-bar software bound.
    /// </summary>
    public static LoopDocument CreateEmpty(int bars)
        => new(new PercussionScore(bars, []));

    /// <summary>
    ///  Replaces only the score, retaining all mixer and transport choices.
    /// </summary>
    public LoopDocument WithScore(PercussionScore score)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(score.Hits.Count, MaximumHits);
        return ScoresEqual(Score, score)
            ? this
            : new(score, TempoBpm, MasterVolumePercent, PercussionVolumes, Loop, MetallicEnabled, MetallicVolumePercent);
    }

    /// <summary>
    ///  Replaces the tempo without modifying any note parameter.
    /// </summary>
    public LoopDocument WithTempo(int tempoBpm)
    {
        ValidateTempo(tempoBpm);
        return tempoBpm == TempoBpm
            ? this
            : new(Score, tempoBpm, MasterVolumePercent, PercussionVolumes, Loop, MetallicEnabled, MetallicVolumePercent);
    }

    /// <summary>
    ///  Replaces the master level without rewriting note velocities.
    /// </summary>
    public LoopDocument WithMasterVolume(int percent)
    {
        ValidatePercent(percent, nameof(percent));
        return percent == MasterVolumePercent
            ? this
            : new(Score, TempoBpm, percent, PercussionVolumes, Loop, MetallicEnabled, MetallicVolumePercent);
    }

    /// <summary>
    ///  Replaces one primary instrument's level, excluding the separately controlled metallic layer.
    /// </summary>
    public LoopDocument WithInstrumentVolume(Cr78Instrument instrument, int percent)
    {
        ValidateInstrument(instrument);
        ValidatePercent(percent, nameof(percent));
        if (PercussionVolumes[instrument] == percent)
        {
            return this;
        }

        Dictionary<Cr78Instrument, int> volumes = new(PercussionVolumes)
        {
            [instrument] = percent
        };
        return new(Score, TempoBpm, MasterVolumePercent, volumes, Loop, MetallicEnabled, MetallicVolumePercent);
    }

    /// <summary>
    ///  Replaces the repeat choice without saving a playing, paused, or stopped state.
    /// </summary>
    public LoopDocument WithLoop(bool loop)
        => loop == Loop
            ? this
            : new(Score, TempoBpm, MasterVolumePercent, PercussionVolumes, loop, MetallicEnabled, MetallicVolumePercent);

    /// <summary>
    ///  Replaces the metallic enable flag and remembered amount as one musical edit.
    /// </summary>
    public LoopDocument WithMetallic(bool enabled, int percent)
    {
        ValidatePercent(percent, nameof(percent));
        return enabled == MetallicEnabled && percent == MetallicVolumePercent
            ? this
            : new(Score, TempoBpm, MasterVolumePercent, PercussionVolumes, Loop, enabled, percent);
    }

    /// <summary>
    ///  Compares score contents and every musical value rather than collection identities.
    /// </summary>
    public bool ValueEquals(LoopDocument? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null ||
            TempoBpm != other.TempoBpm ||
            MasterVolumePercent != other.MasterVolumePercent ||
            Loop != other.Loop ||
            MetallicEnabled != other.MetallicEnabled ||
            MetallicVolumePercent != other.MetallicVolumePercent ||
            !ScoresEqual(Score, other.Score))
        {
            return false;
        }

        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            if (PercussionVolumes[instrument] != other.PercussionVolumes[instrument])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Implements typed semantic equality for independently loaded or cloned documents.
    /// </summary>
    public bool Equals(LoopDocument? other) => ValueEquals(other);

    /// <summary>
    ///  Implements object equality using the same complete musical snapshot.
    /// </summary>
    public override bool Equals(object? obj) => obj is LoopDocument other && ValueEquals(other);

    /// <summary>
    ///  Hashes the same scalar values, ordered score events, and instrument levels used by equality.
    /// </summary>
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Score.BarCount);
        foreach (PercussionHit hit in Score.Hits)
        {
            hash.Add(hit);
        }

        hash.Add(TempoBpm);
        hash.Add(MasterVolumePercent);
        hash.Add(Loop);
        hash.Add(MetallicEnabled);
        hash.Add(MetallicVolumePercent);
        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            hash.Add(PercussionVolumes[instrument]);
        }

        return hash.ToHashCode();
    }

    private static bool ScoresEqual(PercussionScore left, PercussionScore right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left.BarCount != right.BarCount || left.Hits.Count != right.Hits.Count)
        {
            return false;
        }

        for (int index = 0; index < left.Hits.Count; index++)
        {
            if (left.Hits[index] != right.Hits[index])
            {
                return false;
            }
        }

        return true;
    }

    private static void ValidateTempo(int tempoBpm)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(tempoBpm, 40);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(tempoBpm, 240);
    }

    private static void ValidatePercent(int percent, string parameterName)
    {
        if (percent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Levels must be between zero and one hundred percent.");
        }
    }

    private static void ValidateInstrument(Cr78Instrument instrument)
    {
        if (!Cr78Kit.Instruments.Contains(instrument))
        {
            throw new ArgumentOutOfRangeException(nameof(instrument), "A primary percussion instrument is required.");
        }
    }
}
