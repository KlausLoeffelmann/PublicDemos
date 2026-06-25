using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Drives the engine's <c>MagneticNumerals</c> mode: instead of pointing at fixed
///  dial positions, every hand "finds" hour numerals wherever the theme has placed
///  them on the canvas and swings to the next one at its own rate.
/// </summary>
/// <remarks>
///  <para>
///   Each hand steps once per its base unit — the second hand every second, the minute
///   hand every minute, the hour hand every hour — to the <i>next</i> hour numeral
///   (index <c>floor(unit) mod 12</c>). Because a theme may scatter the numerals
///   arbitrarily, consecutive targets can sit anywhere, so the hand can swing wildly
///   back and forth.
///  </para>
///  <para>
///   Numerals carry a tri-state <see cref="ClockNumeralVisibility"/>:
///   <see cref="ClockNumeralVisibility.Visible"/> and
///   <see cref="ClockNumeralVisibility.Transparent"/> are valid targets;
///   <see cref="ClockNumeralVisibility.Invisible"/> (or a missing numeral) is skipped —
///   the hand simply stays on its previous target.
///  </para>
///  <para>
///   On each step the hand glides (ease-in-out) from where it was to the new target over
///   <c>GlideDurationSeconds</c>; once arrived it <i>tracks</i> the target's live anchor,
///   so a numeral that keeps moving is followed and the hand never comes to rest.
///  </para>
/// </remarks>
internal sealed class MagneticNumeralSolver
{
    private const int NumeralCount = 12;

    private sealed class HandState
    {
        public bool Initialized;
        public long LastWhole = long.MinValue;
        public int TargetIndex = -1;
        public float DisplayedAngle;
        public bool Gliding;
        public float GlideElapsed;
        public float GlideStartAngle;
    }

    private readonly Dictionary<ClockHandKind, HandState> _hands = [];

    /// <summary>Clears all per-hand state (call on theme change or when toggling the mode).</summary>
    public void Reset() => _hands.Clear();

    /// <summary>
    ///  Computes the displayed angle for a magnetic hand this frame.
    /// </summary>
    /// <param name="hand">The hand to aim.</param>
    /// <param name="pivot">The hand's pivot anchor (pixels).</param>
    /// <param name="time">The authoritative time snapshot.</param>
    /// <param name="glideDurationSeconds">Ease-in-out glide duration in seconds.</param>
    /// <param name="dtSeconds">Elapsed seconds since the previous frame.</param>
    /// <param name="visibilityOf">Returns a numeral's visibility, or <see langword="null"/> if it does not exist.</param>
    /// <param name="anchorOf">Returns a numeral's current (live) anchor in pixels.</param>
    public float Solve(
        ClockHandKind hand,
        PointF pivot,
        ClockTimeSnapshot time,
        float glideDurationSeconds,
        float dtSeconds,
        Func<int, ClockNumeralVisibility?> visibilityOf,
        Func<int, PointF> anchorOf)
    {
        // The continuous unit value advances at the hand's natural rate; its integer part
        // increments exactly once per second / minute / hour.
        float unit = hand switch
        {
            ClockHandKind.Hour => time.HourAngle / 30f,    // 0..12
            ClockHandKind.Minute => time.MinuteAngle / 6f, // 0..60
            _ => time.SecondAngle / 6f,                    // 0..60
        };

        long whole = (long)MathF.Floor(unit);
        int candidate = (int)(((whole % NumeralCount) + NumeralCount) % NumeralCount);

        if (!_hands.TryGetValue(hand, out HandState? state))
        {
            state = new HandState();
            _hands[hand] = state;
        }

        bool IsTarget(int idx)
            => visibilityOf(idx) is ClockNumeralVisibility.Visible or ClockNumeralVisibility.Transparent;

        // First frame for this hand: snap straight onto a valid target without a glide.
        if (!state.Initialized)
        {
            state.TargetIndex = IsTarget(candidate) ? candidate : FirstValidTarget(IsTarget);
            state.LastWhole = whole;
            state.Initialized = true;
            state.Gliding = false;
            state.DisplayedAngle = state.TargetIndex >= 0
                ? ClockMath.AngleTo(pivot, anchorOf(state.TargetIndex))
                : 0f;
            return state.DisplayedAngle;
        }

        // A step occurs when the integer unit advances. Only then do we re-target.
        if (whole != state.LastWhole)
        {
            state.LastWhole = whole;

            if (IsTarget(candidate))
            {
                // Valid numeral: begin a fresh glide from wherever the hand currently is.
                state.TargetIndex = candidate;
                state.GlideStartAngle = state.DisplayedAngle;
                state.GlideElapsed = 0f;
                state.Gliding = true;
            }
            // Invisible / missing numeral: skip it — keep the previous target and stay put.
        }

        if (state.TargetIndex < 0)
        {
            return state.DisplayedAngle;
        }

        // The live target angle is recomputed every frame so a moving numeral is followed.
        float liveTarget = ClockMath.AngleTo(pivot, anchorOf(state.TargetIndex));

        if (state.Gliding)
        {
            state.GlideElapsed += MathF.Max(dtSeconds, 0f);
            float t = glideDurationSeconds <= 0f
                ? 1f
                : Math.Clamp(state.GlideElapsed / glideDurationSeconds, 0f, 1f);

            float eased = ClockMath.EaseInOut(t);
            float delta = ClockMath.ShortestDelta(state.GlideStartAngle, liveTarget);
            state.DisplayedAngle = ClockMath.Normalize360(state.GlideStartAngle + delta * eased);

            if (t >= 1f)
            {
                state.Gliding = false;
            }
        }
        else
        {
            // Arrived: keep pointing at the live target (tracks numerals that keep moving).
            state.DisplayedAngle = liveTarget;
        }

        return state.DisplayedAngle;
    }

    private static int FirstValidTarget(Func<int, bool> isTarget)
    {
        for (int i = 0; i < NumeralCount; i++)
        {
            if (isTarget(i))
            {
                return i;
            }
        }

        return -1;
    }
}
