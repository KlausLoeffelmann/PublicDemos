using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Derives the displayed rotation of each hand from the authoritative time and the
///  position of its target anchor, then applies a grace catch-up so a hand eases
///  toward its target whenever the target jumps (for example when a free-floating
///  theme relocates an hour visual).
/// </summary>
/// <remarks>
///  A theme can never set a hand angle directly: it can only move anchors. The hand
///  always ends up pointing at the engine-owned target, so the time is always right —
///  the grace window only governs <i>how</i> it gets there, never <i>where</i>.
/// </remarks>
public sealed class HandPointingSolver
{
    private readonly Dictionary<ClockHandKind, float> _displayed = new();

    /// <summary>Clears all grace state (call on theme change or surface reset).</summary>
    public void Reset() => _displayed.Clear();

    /// <summary>
    ///  Computes the radial target angle for a hand: the authoritative angle, possibly
    ///  quantized or glided by the chosen <paramref name="motion"/>.
    /// </summary>
    /// <param name="time">The authoritative time snapshot.</param>
    /// <param name="hand">The hand to aim.</param>
    /// <param name="motion">The motion style for this hand.</param>
    /// <param name="glideDurationSeconds">
    ///  The ease-in-out crawl duration (wall-clock seconds) used by <see cref="ClockHandMotion.Crawling"/>.
    /// </param>
    public static float RadialTargetAngle(
        ClockTimeSnapshot time,
        ClockHandKind hand,
        ClockHandMotion motion,
        float glideDurationSeconds = 0.5f)
    {
        // SubSecond has no quantized form; it always crawls.
        if (hand == ClockHandKind.SubSecond)
        {
            return time.SubSecondAngle;
        }

        float degreesPerUnit = GetContinuousUnitMetrics(time, hand).DegreesPerUnit;
        float unitPosition = SelectUnitPosition(time, hand, motion, glideDurationSeconds);
        return ClockMath.Normalize360(unitPosition * degreesPerUnit);
    }

    internal static float SelectUnitPosition(
        ClockTimeSnapshot time,
        ClockHandKind hand,
        ClockHandMotion motion,
        float glideDurationSeconds = 0.5f)
    {
        if (hand == ClockHandKind.SubSecond)
        {
            return FractionalSecond(time) % 1f;
        }

        (float continuousUnit, _, float secondsPerStep) = GetContinuousUnitMetrics(time, hand);
        float stepsPerUnit = hand switch
        {
            ClockHandKind.Hour => 60f,
            ClockHandKind.Minute => 60f,
            _ => 1f,
        };
        float stepPosition = continuousUnit * stepsPerUnit;

        return motion switch
        {
            ClockHandMotion.Crawling => CrawlUnit(stepPosition, secondsPerStep, glideDurationSeconds) / stepsPerUnit,
            ClockHandMotion.Tick => MathF.Floor(stepPosition) / stepsPerUnit,
            ClockHandMotion.FastTick => MathF.Floor(stepPosition * 4f) / (stepsPerUnit * 4f),
            ClockHandMotion.Sweep => continuousUnit,
            _ => continuousUnit,
        };
    }

    private static (float ContinuousUnit, float DegreesPerUnit, float SecondsPerUnit) GetContinuousUnitMetrics(
        ClockTimeSnapshot time,
        ClockHandKind hand)
        => hand switch
        {
            ClockHandKind.Hour => (FractionalHour(time), 30f, 60f),
            ClockHandKind.Minute => (FractionalMinute(time), 6f, 1f),
            _ => (FractionalSecond(time), 6f, 1f),
        };

    /// <summary>
    ///  Eases the hand from the previous mark to the next over the first
    ///  <paramref name="glideDurationSeconds"/> of each unit, then holds on the mark.
    /// </summary>
    private static float CrawlUnit(float continuousUnit, float secondsPerUnit, float glideDurationSeconds)
    {
        float whole = MathF.Floor(continuousUnit);
        float frac = continuousUnit - whole;
        float glideFraction = Math.Clamp(glideDurationSeconds / MathF.Max(secondsPerUnit, 1e-4f), 0f, 1f);
        float glideT = glideFraction <= 0f ? 1f : Math.Clamp(frac / glideFraction, 0f, 1f);
        float eased = ClockMath.EaseInOut(glideT);
        return whole - 1f + eased;
    }

    /// <summary>
    ///  Computes the free-floating target angle for a hand: the angle from the hand's
    ///  pivot anchor to the interpolated position of its target slot anchors. This is
    ///  what makes a hand's tip "follow" relocated hour / minute visuals.
    /// </summary>
    /// <param name="hand">The hand to aim.</param>
    /// <param name="pivot">The hand's pivot anchor (pixels).</param>
    /// <param name="time">The authoritative time.</param>
    /// <param name="anchorOf">Resolves the anchor (pixels) of any element id.</param>
    public static float FreeFloatingTargetAngle(
        ClockHandKind hand,
        PointF pivot,
        ClockTimeSnapshot time,
        Func<ClockElementId, PointF> anchorOf)
        => FreeFloatingTargetAngle(hand, pivot, time, ClockHandMotion.Crawling, 0.5f, anchorOf);

    /// <summary>
    ///  Computes the free-floating target angle for a hand while honoring the chosen
    ///  motion style when selecting or interpolating target slots.
    /// </summary>
    public static float FreeFloatingTargetAngle(
        ClockHandKind hand,
        PointF pivot,
        ClockTimeSnapshot time,
        ClockHandMotion motion,
        float glideDurationSeconds,
        Func<ClockElementId, PointF> anchorOf)
    {
        if (hand == ClockHandKind.SubSecond)
        {
            return time.SubSecondAngle;
        }

        float unitPosition = SelectUnitPosition(time, hand, motion, glideDurationSeconds);
        PointF target = hand switch
        {
            ClockHandKind.Hour => InterpolateSlot(
                ClockElementKind.HourMarker, 12, unitPosition, anchorOf),
            ClockHandKind.Minute => InterpolateSlot(
                ClockElementKind.MinuteTick, 60, unitPosition, anchorOf),
            ClockHandKind.Second => InterpolateSlot(
                ClockElementKind.MinuteTick, 60, unitPosition, anchorOf),
            _ => pivot,
        };

        if (target == pivot)
        {
            // Degenerate (e.g. sub-second) — fall back to the radial angle.
            return RadialTargetAngle(time, hand, motion, glideDurationSeconds);
        }

        return ClockMath.AngleTo(pivot, target);
    }

    /// <summary>
    ///  Applies grace catch-up: eases the hand's displayed angle toward
    ///  <paramref name="targetAngle"/>. When <paramref name="smooth"/> is false, the
    ///  hand tracks a continuously moving target exactly.
    /// </summary>
    /// <param name="hand">The hand.</param>
    /// <param name="targetAngle">The desired target angle (degrees).</param>
    /// <param name="graceSeconds">The catch-up window, 0..30 seconds.</param>
    /// <param name="smooth">Whether to ease (free-floating) or track exactly (radial crawl).</param>
    /// <param name="dtSeconds">Elapsed time since the previous frame.</param>
    public float Solve(ClockHandKind hand, float targetAngle, float graceSeconds, bool smooth, float dtSeconds)
    {
        targetAngle = ClockMath.Normalize360(targetAngle);

        if (!_displayed.TryGetValue(hand, out float displayed))
        {
            _displayed[hand] = targetAngle;
            return targetAngle;
        }

        if (!smooth || graceSeconds <= 0f || dtSeconds <= 0f)
        {
            _displayed[hand] = targetAngle;
            return targetAngle;
        }

        float delta = ClockMath.ShortestDelta(displayed, targetAngle);

        // Critically-damped follow: ~95% of the gap closes within graceSeconds.
        float tau = graceSeconds / 3f;
        float follow = 1f - MathF.Exp(-dtSeconds / MathF.Max(tau, 1e-4f));
        displayed = ClockMath.Normalize360(displayed + delta * follow);

        // Snap when essentially arrived to avoid infinite easing.
        if (MathF.Abs(ClockMath.ShortestDelta(displayed, targetAngle)) < 0.05f)
        {
            displayed = targetAngle;
        }

        _displayed[hand] = displayed;
        return displayed;
    }

    private static PointF InterpolateSlot(
        ClockElementKind kind,
        int count,
        float fractionalIndex,
        Func<ClockElementId, PointF> anchorOf)
    {
        float wrapped = fractionalIndex % count;
        if (wrapped < 0f)
        {
            wrapped += count;
        }

        int lower = (int)MathF.Floor(wrapped) % count;
        int upper = (lower + 1) % count;
        float t = wrapped - MathF.Floor(wrapped);

        PointF a = anchorOf(new ClockElementId(kind, lower));
        PointF b = anchorOf(new ClockElementId(kind, upper));
        return ClockMath.Lerp(a, b, t);
    }

    private static float FractionalHour(ClockTimeSnapshot time)
        => (time.Now.Hour % 12)
            + time.Now.Minute / 60f
            + (time.Now.Second + (time.Now.Millisecond / 1000f)) / 3600f;

    private static float FractionalMinute(ClockTimeSnapshot time)
        => time.Now.Minute + (time.Now.Second + time.Now.Millisecond / 1000f) / 60f;

    private static float FractionalSecond(ClockTimeSnapshot time)
        => time.Now.Second + time.Now.Millisecond / 1000f;
}
