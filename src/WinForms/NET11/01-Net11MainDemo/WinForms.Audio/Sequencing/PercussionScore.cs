using System.Collections.ObjectModel;
using WinForms.Audio.Percussion;

namespace WinForms.Audio.Sequencing;

/// <summary>
///  An immutable percussion event in zero-based bars and sixteenth-note steps.
/// </summary>
public readonly record struct PercussionHit
{
    /// <summary>
    ///  Creates a validated event; gate length affects the guiro, while other sounds decay naturally.
    /// </summary>
    public PercussionHit(
        int Bar,
        int Step,
        Cr78Instrument Instrument,
        float Velocity = 1f,
        int GateSteps = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Bar);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(Bar, PercussionScore.MaximumBars);
        ArgumentOutOfRangeException.ThrowIfNegative(Step);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(Step, PercussionScore.StepsPerBar);
        Cr78Kit.ValidateInstrument(Instrument, allowMetallic: false);
        Cr78Kit.ValidateLevel(Velocity, nameof(Velocity));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(GateSteps);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(GateSteps, PercussionScore.MaximumGateSteps);
        this.Bar = Bar;
        this.Step = Step;
        this.Instrument = Instrument;
        this.Velocity = Velocity;
        this.GateSteps = GateSteps;
    }

    /// <summary>
    ///  Gets the zero-based bar containing the strike.
    /// </summary>
    public int Bar { get; }

    /// <summary>
    ///  Gets the zero-based sixteenth-note step within that bar.
    /// </summary>
    public int Step { get; }

    /// <summary>
    ///  Gets the percussion sound; the metallic layer is controlled separately.
    /// </summary>
    public Cr78Instrument Instrument { get; }

    /// <summary>
    ///  Gets finite strike strength from zero to one.
    /// </summary>
    public float Velocity { get; }

    /// <summary>
    ///  Gets the guiro gate in steps, which may cross a bar boundary.
    /// </summary>
    public int GateSteps { get; }
}

/// <summary>
///  A validated, immutable 4/4 score with its per-step event lookup prepared on the caller's thread.
/// </summary>
/// <remarks>
///  This is not the hardware programmer: all thirteen sounds may share a step and scores may
///  exceed four two-bar memories. The limits below are generous software resource bounds only.
/// </remarks>
public sealed class PercussionScore
{
    private readonly PercussionHit[] _hits;
    private readonly ReadOnlyCollection<PercussionHit> _readOnlyHits;
    private readonly int[] _stepOffsets;

    /// <summary>
    ///  The number of sixteenth-note steps in each 4/4 bar.
    /// </summary>
    public const int StepsPerBar = 16;

    /// <summary>
    ///  The software score-size bound, unrelated to the original programmer's memory limits.
    /// </summary>
    public const int MaximumBars = 4_096;

    /// <summary>
    ///  The maximum supported gate length, allowing sustained scrapes across multiple bars.
    /// </summary>
    public const int MaximumGateSteps = MaximumBars * StepsPerBar;

    /// <summary>
    ///  Copies, validates, sorts, and indexes events without retaining a mutable caller collection.
    /// </summary>
    public PercussionScore(int barCount, IEnumerable<PercussionHit> hits)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(barCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(barCount, MaximumBars);
        ArgumentNullException.ThrowIfNull(hits);
        BarCount = barCount;
        _hits = hits.ToArray();
        foreach (PercussionHit hit in _hits)
        {
            // A default record bypasses its explicit constructor, so validate again here.
            ValidatePosition(hit.Bar, hit.Instrument, hit.Step);
            Cr78Kit.ValidateLevel(hit.Velocity, nameof(hits));
            if (hit.GateSteps <= 0 || hit.GateSteps > MaximumGateSteps)
            {
                throw new ArgumentOutOfRangeException(nameof(hits), "GateSteps must be a supported positive length.");
            }
        }

        Array.Sort(_hits, static (left, right) => Key(left).CompareTo(Key(right)));
        for (int i = 1; i < _hits.Length; i++)
        {
            if (Key(_hits[i - 1]) == Key(_hits[i]))
            {
                throw new ArgumentException("An instrument may occur only once in a bar/step cell.", nameof(hits));
            }
        }

        _readOnlyHits = Array.AsReadOnly(_hits);
        _stepOffsets = new int[barCount * StepsPerBar + 1];
        int eventIndex = 0;
        for (int step = 0; step < _stepOffsets.Length - 1; step++)
        {
            _stepOffsets[step] = eventIndex;
            while (eventIndex < _hits.Length &&
                _hits[eventIndex].Bar * StepsPerBar + _hits[eventIndex].Step == step)
            {
                eventIndex++;
            }
        }

        _stepOffsets[^1] = eventIndex;
    }

    /// <summary>
    ///  Gets the number of bars, including empty bars.
    /// </summary>
    public int BarCount { get; }

    /// <summary>
    ///  Gets events in bar, step, then instrument order through a non-mutable collection.
    /// </summary>
    public IReadOnlyList<PercussionHit> Hits
        => _readOnlyHits;

    /// <summary>
    ///  Tests one valid grid cell without exposing or allocating mutable event storage.
    /// </summary>
    public bool HasHit(int bar, Cr78Instrument instrument, int step)
    {
        ValidatePosition(bar, instrument, step);
        foreach (PercussionHit hit in GetStepHits(bar, step))
        {
            if (hit.Instrument == instrument)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Returns an edited score, preserving other cells and an existing hit's gate length.
    /// </summary>
    public PercussionScore WithStep(int bar, Cr78Instrument instrument, int step, bool enabled, float velocity = 1f)
    {
        ValidatePosition(bar, instrument, step);
        Cr78Kit.ValidateLevel(velocity, nameof(velocity));
        List<PercussionHit> edited = new(_hits.Length + 1);
        bool found = false;
        foreach (PercussionHit hit in _hits)
        {
            if (hit.Bar == bar && hit.Step == step && hit.Instrument == instrument)
            {
                found = true;
                if (enabled)
                {
                    if (hit.Velocity == velocity)
                    {
                        return this;
                    }

                    edited.Add(new PercussionHit(bar, step, instrument, velocity, hit.GateSteps));
                }
            }
            else
            {
                edited.Add(hit);
            }
        }

        if (!found)
        {
            if (!enabled)
            {
                return this;
            }

            edited.Add(new PercussionHit(bar, step, instrument, velocity));
        }

        return new PercussionScore(BarCount, edited);
    }

    /// <summary>
    ///  Gets a preindexed step without constructing arrays, enumerators, or DSP graphs.
    /// </summary>
    internal ReadOnlySpan<PercussionHit> GetStepHits(int bar, int step)
    {
        int index = bar * StepsPerBar + step;
        int start = _stepOffsets[index];
        return _hits.AsSpan(start, _stepOffsets[index + 1] - start);
    }

    /// <summary>
    ///  Compares caller-side document snapshots without treating a mixer-only edit as a new score.
    /// </summary>
    internal bool ContentEquals(PercussionScore other)
        => ReferenceEquals(this, other) ||
            (BarCount == other.BarCount && _hits.AsSpan().SequenceEqual(other._hits));

    private void ValidatePosition(int bar, Cr78Instrument instrument, int step)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bar);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(bar, BarCount);
        ArgumentOutOfRangeException.ThrowIfNegative(step);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(step, StepsPerBar);
        Cr78Kit.ValidateInstrument(instrument, allowMetallic: false);
    }

    private static int Key(PercussionHit hit)
        => (hit.Bar * StepsPerBar + hit.Step) * 13 + (int)hit.Instrument;
}
