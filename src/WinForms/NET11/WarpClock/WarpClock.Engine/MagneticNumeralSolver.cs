using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Drives the engine's <c>MagneticNumerals</c> mode: instead of pointing at fixed
///  dial positions, every hand "finds" hour numerals wherever the theme has placed
///  them on the canvas while preserving the hand's authoritative progress through
///  the current numeral interval.
/// </summary>
/// <remarks>
///  <para>
///   Each hand aims only at its current numeral anchor, then adds the normal clockwise
///   elapsed-time angle within that numeral's 30-degree interval. Hour hands use one
///   numeral per hour; minute and second hands use one numeral per five units.
///   The later numeral is never part of the target calculation.
///   Themes opt a hand in with <see cref="ClockHandTargetMode.MagneticNumerals"/> and out
///   with <see cref="ClockHandTargetMode.Radial"/>; the host's global magnetic switch
///   decides for every hand that states neither.
///  </para>
///  <para>
///   Numerals carry a tri-state <see cref="ClockNumeralVisibility"/>:
///   <see cref="ClockNumeralVisibility.Visible"/> and
///   <see cref="ClockNumeralVisibility.Transparent"/> are valid targets;
///   <see cref="ClockNumeralVisibility.Invisible"/> (or a missing numeral) is skipped —
///   the hand simply stays on its previous target.
///  </para>
///  <para>
///   Exact hour/five-minute/five-second boundaries have zero compensation and therefore
///   point directly at the new numeral. Between boundaries the live anchor is tracked
///   continuously while the compensation advances clockwise.
///  </para>
/// </remarks>
internal sealed class MagneticNumeralSolver
{
    private const int NumeralCount = 12;

    private sealed class HandState
    {
        public int TargetIndex = -1;
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
    /// <param name="visibilityOf">Returns a numeral's visibility, or <see langword="null"/> if it does not exist.</param>
    /// <param name="anchorOf">Returns a numeral's current (live) anchor in pixels.</param>
    /// <returns>The magnetic angle, or <see langword="null"/> when no valid target has ever been available.</returns>
    public float? Solve(
        ClockHandKind hand,
        PointF pivot,
        ClockTimeSnapshot time,
        Func<int, ClockNumeralVisibility?> visibilityOf,
        Func<int, PointF> anchorOf)
    {
        MagneticNumeralPosition position = MagneticNumeralPosition.Resolve(hand, time);

        if (!_hands.TryGetValue(hand, out HandState? state))
        {
            state = new HandState();
            _hands[hand] = state;
        }

        if (visibilityOf(position.NumeralIndex)
            is ClockNumeralVisibility.Visible or ClockNumeralVisibility.Transparent)
        {
            state.TargetIndex = position.NumeralIndex;
        }

        if (state.TargetIndex < 0)
        {
            return null;
        }

        int skippedNumerals = ((position.NumeralIndex - state.TargetIndex) % NumeralCount + NumeralCount) % NumeralCount;
        float compensation = (skippedNumerals * 30f) + position.CompensationDegrees;
        float anchorAngle = ClockMath.AngleTo(pivot, anchorOf(state.TargetIndex));
        return ClockMath.Normalize360(anchorAngle + compensation);
    }
}
