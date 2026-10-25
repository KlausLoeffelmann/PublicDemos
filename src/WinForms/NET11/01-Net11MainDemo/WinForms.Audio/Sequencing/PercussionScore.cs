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
///  This is not the hardware programmer: all stock sounds may share a step and scores may
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
    ///  Inserts an empty bar at <paramref name="index"/>, shifting later events forward.
    /// </summary>
    /// <param name="index">The zero-based insertion point, or <see cref="BarCount"/> to append.</param>
    /// <returns>A new score; the original is unchanged.</returns>
    public PercussionScore InsertBar(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, BarCount);
        if (BarCount == MaximumBars)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The score is already at its bar limit.");
        }

        List<PercussionHit> edited = new(_hits.Length);
        foreach (PercussionHit hit in _hits)
        {
            edited.Add(hit.Bar < index ? hit : Shift(hit, 1));
        }

        return new PercussionScore(BarCount + 1, edited);
    }

    /// <summary>
    ///  Removes one bar and the events it contains, then closes the gap.
    /// </summary>
    /// <param name="index">The zero-based bar to remove.</param>
    /// <returns>A new score; the original is unchanged.</returns>
    /// <remarks>
    ///  Gate lengths are durations, not end-bar references, so a scrape that started in an
    ///  earlier bar keeps its length.
    /// </remarks>
    public PercussionScore RemoveBar(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, BarCount);
        if (BarCount == 1)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "A score must keep at least one bar.");
        }

        List<PercussionHit> edited = new(_hits.Length);
        foreach (PercussionHit hit in _hits)
        {
            if (hit.Bar == index)
            {
                continue;
            }

            edited.Add(hit.Bar < index ? hit : Shift(hit, -1));
        }

        return new PercussionScore(BarCount - 1, edited);
    }

    /// <summary>
    ///  Inserts a copy of <paramref name="bar"/> immediately after it.
    /// </summary>
    /// <param name="bar">The zero-based source bar.</param>
    /// <param name="instruments">Instruments whose hits are copied. Other instruments are empty in the new bar.</param>
    /// <returns>A new score; the original is unchanged.</returns>
    public PercussionScore DuplicateBar(int bar, IEnumerable<Cr78Instrument> instruments)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bar);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(bar, BarCount);
        HashSet<Cr78Instrument> selected = ValidateInstruments(instruments);
        if (BarCount == MaximumBars)
        {
            throw new ArgumentOutOfRangeException(nameof(bar), "The score is already at its bar limit.");
        }

        List<PercussionHit> edited = new(_hits.Length + StepsPerBar);
        foreach (PercussionHit hit in _hits)
        {
            if (hit.Bar < bar)
            {
                edited.Add(hit);
            }
            else if (hit.Bar == bar)
            {
                edited.Add(hit);
                if (selected.Contains(hit.Instrument))
                {
                    edited.Add(Shift(hit, 1));
                }
            }
            else
            {
                edited.Add(Shift(hit, 1));
            }
        }

        return new PercussionScore(BarCount + 1, edited);
    }

    /// <summary>
    ///  Turns on a regular subdivision in the named bars and instruments, and clears the other steps there.
    /// </summary>
    /// <param name="bars">Bars to fill. Each bar may be listed only once.</param>
    /// <param name="instruments">Instruments to fill. The metallic layer is not a score track.</param>
    /// <param name="stepInterval">1, 2, 4, 8, or 16 sixteenth-note steps.</param>
    /// <returns>A new score. Existing on-grid hits keep their velocity and gate.</returns>
    public PercussionScore FillBars(IEnumerable<int> bars, IEnumerable<Cr78Instrument> instruments, int stepInterval)
    {
        ArgumentNullException.ThrowIfNull(bars);
        HashSet<int> barSet = ValidateBars(bars);
        HashSet<Cr78Instrument> selected = ValidateInstruments(instruments);
        if (stepInterval is not (1 or 2 or 4 or 8 or 16))
        {
            throw new ArgumentOutOfRangeException(nameof(stepInterval), "The fill interval must be 1, 2, 4, 8, or 16 steps.");
        }

        List<PercussionHit> edited = new(_hits.Length + barSet.Count * selected.Count * (StepsPerBar / stepInterval));
        HashSet<(int Bar, int Step, Cr78Instrument Instrument)> present = [];
        foreach (PercussionHit hit in _hits)
        {
            bool targeted = barSet.Contains(hit.Bar) && selected.Contains(hit.Instrument);
            if (targeted && hit.Step % stepInterval != 0)
            {
                continue;
            }

            edited.Add(hit);
            if (targeted)
            {
                present.Add((hit.Bar, hit.Step, hit.Instrument));
            }
        }

        foreach (int bar in barSet)
        {
            foreach (Cr78Instrument instrument in selected)
            {
                for (int step = 0; step < StepsPerBar; step += stepInterval)
                {
                    if (present.Add((bar, step, instrument)))
                    {
                        edited.Add(new PercussionHit(bar, step, instrument));
                    }
                }
            }
        }

        return new PercussionScore(BarCount, edited);
    }

    /// <summary>
    ///  Removes hits for the named instruments in the named bars, leaving every other cell unchanged.
    /// </summary>
    /// <param name="bars">Bars to clear. Each bar may be listed only once.</param>
    /// <param name="instruments">Instruments to clear.</param>
    /// <returns>A new score; the original is unchanged.</returns>
    public PercussionScore ClearBars(IEnumerable<int> bars, IEnumerable<Cr78Instrument> instruments)
    {
        ArgumentNullException.ThrowIfNull(bars);
        HashSet<int> barSet = ValidateBars(bars);
        HashSet<Cr78Instrument> selected = ValidateInstruments(instruments);
        List<PercussionHit> edited = new(_hits.Length);
        foreach (PercussionHit hit in _hits)
        {
            if (barSet.Contains(hit.Bar) && selected.Contains(hit.Instrument))
            {
                continue;
            }

            edited.Add(hit);
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
        => (hit.Bar * StepsPerBar + hit.Step) * ((int)Cr78Instrument.Guiro2 + 1) + (int)hit.Instrument;

    private static PercussionHit Shift(PercussionHit hit, int bars)
        => new(hit.Bar + bars, hit.Step, hit.Instrument, hit.Velocity, hit.GateSteps);

    private HashSet<int> ValidateBars(IEnumerable<int> bars)
    {
        HashSet<int> set = [];
        bool any = false;
        foreach (int bar in bars)
        {
            any = true;
            ArgumentOutOfRangeException.ThrowIfNegative(bar);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(bar, BarCount);
            if (!set.Add(bar))
            {
                throw new ArgumentException("A bar may be listed only once.", nameof(bars));
            }
        }

        if (!any)
        {
            throw new ArgumentException("At least one bar is required.", nameof(bars));
        }

        return set;
    }

    private static HashSet<Cr78Instrument> ValidateInstruments(IEnumerable<Cr78Instrument> instruments)
    {
        ArgumentNullException.ThrowIfNull(instruments);
        HashSet<Cr78Instrument> set = [];
        foreach (Cr78Instrument instrument in instruments)
        {
            Cr78Kit.ValidateInstrument(instrument, allowMetallic: false);
            if (!set.Add(instrument))
            {
                throw new ArgumentException("An instrument may be listed only once.", nameof(instruments));
            }
        }

        if (set.Count == 0)
        {
            throw new ArgumentException("At least one percussion instrument is required.", nameof(instruments));
        }

        return set;
    }
}
