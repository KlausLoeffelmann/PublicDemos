using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Single decision point for how a hand finds its target. Every hand goes through
///  here so a theme request, the host's global magnetic switch, and the safety rules
///  (sub-second hands never chase numerals; free-floating needs theme support) can
///  never disagree between code paths.
/// </summary>
internal static class HandTargetModeResolver
{
    public static ClockHandTargetMode Resolve(
        ClockHandKind hand,
        ClockHandTargetMode requestedMode,
        bool themeSupportsFreeFloating,
        bool magneticNumeralsEnabled)
    {
        if (hand is ClockHandKind.None or ClockHandKind.SubSecond)
        {
            return ClockHandTargetMode.Radial;
        }

        // An explicit opt-out always wins: a theme that wants a plain radial hand keeps
        // one even while the rest of the dial is magnetic.
        if (requestedMode == ClockHandTargetMode.Radial)
        {
            return ClockHandTargetMode.Radial;
        }

        // An explicit magnetic request expresses the theme's design, so it survives a
        // host that never switched the global magnetic mode on (for example because a
        // persisted user setting kept it off). The global switch magnetizes every other
        // hand that did not opt out.
        if (requestedMode == ClockHandTargetMode.MagneticNumerals || magneticNumeralsEnabled)
        {
            return ClockHandTargetMode.MagneticNumerals;
        }

        return themeSupportsFreeFloating
            ? ClockHandTargetMode.FreeFloating
            : ClockHandTargetMode.Radial;
    }
}
