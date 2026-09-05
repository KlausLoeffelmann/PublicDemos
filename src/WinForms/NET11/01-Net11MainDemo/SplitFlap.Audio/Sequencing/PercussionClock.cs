namespace SplitFlap.Audio.Sequencing;

/// <summary>
///  Keeps fractional frame positions so rounding a sixteenth note cannot accumulate loop drift.
/// </summary>
internal sealed class PercussionClock
{
    private readonly int _sampleRate;

    /// <summary>
    ///  Prepares a clock for a supported rate and a validated positive tempo.
    /// </summary>
    internal PercussionClock(int sampleRate, Tempo tempo)
    {
        _sampleRate = sampleRate;
        SetTempo(tempo);
    }

    /// <summary>
    ///  Gets the exact decimal position of the next step, retaining the sub-sample remainder.
    /// </summary>
    internal decimal ExactFrame { get; private set; }

    /// <summary>
    ///  Gets the fractional duration of one sixteenth note.
    /// </summary>
    internal decimal FramesPerStep { get; private set; }

    /// <summary>
    ///  Gets the nearest output sample for the next step.
    /// </summary>
    internal long NextFrame
        => RoundFrame(ExactFrame);

    /// <summary>
    ///  Starts a musical origin at an absolute engine frame.
    /// </summary>
    internal void Reset(long frame)
        => ExactFrame = frame;

    /// <summary>
    ///  Changes future step durations without rounding or discarding the current phase.
    /// </summary>
    internal void SetTempo(Tempo tempo)
    {
        ValidateTempo(tempo);
        // 60 seconds / BPM / four sixteenths per beat = 15 / BPM seconds per step.
        // Decimal arithmetic happens once per step, not per sample. Its retained fraction
        // avoids integer-step drift and stays far below one sample over years of playback.
        FramesPerStep = _sampleRate * 15m / tempo.BeatsPerMinute;
    }

    /// <summary>
    ///  Advances a step while retaining its fractional sample remainder across every bar and loop.
    /// </summary>
    internal void Advance()
        => ExactFrame += FramesPerStep;

    /// <summary>
    ///  Measures a gate from this same origin so both ends use the same nearest-sample rule.
    /// </summary>
    internal long GateFrames(int gateSteps)
        => Math.Max(1, RoundFrame(ExactFrame + FramesPerStep * gateSteps) - NextFrame);

    /// <summary>
    ///  Uses one explicit rounding convention for rendered onsets and playback-history lookup.
    /// </summary>
    internal static long RoundFrame(decimal frame)
        => decimal.ToInt64(decimal.Round(frame, 0, MidpointRounding.AwayFromZero));

    /// <summary>
    ///  Rejects invalid Tempo defaults and bounds real-time event density to a practical range.
    /// </summary>
    internal static void ValidateTempo(Tempo tempo)
    {
        if (tempo.BeatsPerMinute < 1 || tempo.BeatsPerMinute > 1_000)
        {
            throw new ArgumentOutOfRangeException(nameof(tempo), "The drum player supports 1–1,000 beats per minute.");
        }
    }
}
