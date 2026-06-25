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
    ///  The ease-in-out glide duration (wall-clock seconds) used by <see cref="ClockHandMotion.Sweep"/>.
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

        // The continuous authoritative angle and how many degrees one "unit" spans
        // (a second for the second hand, a minute for the minute hand, an hour for the
        // hour hand). The continuous unit value is recovered as angle / degreesPerUnit.
        (float continuousAngle, float degreesPerUnit, float secondsPerUnit) = hand switch
        {
            ClockHandKind.Hour => (time.HourAngle, 30f, 3600f),
            ClockHandKind.Minute => (time.MinuteAngle, 6f, 60f),
            _ => (time.SecondAngle, 6f, 1f),
        };

        return motion switch
        {
            ClockHandMotion.Crawling => continuousAngle,
            ClockHandMotion.Tick => MathF.Floor(continuousAngle / degreesPerUnit) * degreesPerUnit,
            ClockHandMotion.FastTick => QuantizeFast(continuousAngle, degreesPerUnit),
            ClockHandMotion.Sweep => Glide(continuousAngle, degreesPerUnit, secondsPerUnit, glideDurationSeconds),
            _ => continuousAngle,
        };
    }

    /// <summary>Four discrete steps per unit (the legacy "Sweep" feel).</summary>
    private static float QuantizeFast(float continuousAngle, float degreesPerUnit)
    {
        float step = degreesPerUnit / 4f;
        return MathF.Floor(continuousAngle / step) * step;
    }

    /// <summary>
    ///  Eases the hand from the previous mark to the next over the first
    ///  <paramref name="glideDurationSeconds"/> of each unit, then holds on the mark.
    ///  The hand reaches the mark at the half-way point of a one-second unit, leaving the
    ///  remaining time to rest on the numeral.
    /// </summary>
    private static float Glide(float continuousAngle, float degreesPerUnit, float secondsPerUnit, float glideDurationSeconds)
    {
        float unit = continuousAngle / degreesPerUnit;   // continuous units (e.g. 0..60 for seconds)
        float whole = MathF.Floor(unit);                 // the mark we are arriving at
        float frac = unit - whole;                       // progress (0..1) through the current unit

        // Convert the wall-clock glide duration into a fraction of this hand's unit.
        float glideFraction = Math.Clamp(glideDurationSeconds / MathF.Max(secondsPerUnit, 1e-4f), 0f, 1f);
        float glideT = glideFraction <= 0f ? 1f : Math.Clamp(frac / glideFraction, 0f, 1f);

        // Glide from the previous mark (whole - 1) up to the current mark (whole).
        float eased = ClockMath.EaseInOut(glideT);
        return (whole - 1f + eased) * degreesPerUnit;
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
    {
        PointF target = hand switch
        {
            ClockHandKind.Hour => InterpolateSlot(
                ClockElementKind.HourMarker, 12, FractionalHour(time), anchorOf),
            ClockHandKind.Minute => InterpolateSlot(
                ClockElementKind.MinuteTick, 60, FractionalMinute(time), anchorOf),
            ClockHandKind.Second => InterpolateSlot(
                ClockElementKind.MinuteTick, 60, FractionalSecond(time), anchorOf),
            _ => pivot,
        };

        if (target == pivot)
        {
            // Degenerate (e.g. sub-second) — fall back to the radial angle.
            return RadialTargetAngle(time, hand, ClockHandMotion.Crawling);
        }

        return ClockMath.AngleTo(pivot, target);
    }

    /// <summary>
    ///  Applies grace catch-up: eases the hand's displayed angle toward
    ///  <paramref name="targetAngle"/>. When <paramref name="smooth"/> is false (radial
    ///  crawl), the hand tracks the target exactly.
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
        => (time.Now.Hour % 12) + time.Now.Minute / 60f + time.Now.Second / 3600f;

    private static float FractionalMinute(ClockTimeSnapshot time)
        => time.Now.Minute + (time.Now.Second + time.Now.Millisecond / 1000f) / 60f;

    private static float FractionalSecond(ClockTimeSnapshot time)
        => time.Now.Second + time.Now.Millisecond / 1000f;
}
