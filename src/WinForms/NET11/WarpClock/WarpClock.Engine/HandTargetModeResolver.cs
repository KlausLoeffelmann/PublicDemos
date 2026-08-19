using WarpClock.Abstractions;

namespace WarpClock.Engine;

internal static class HandTargetModeResolver
{
    public static ClockHandTargetMode Resolve(
        ClockHandKind hand,
        ClockHandTargetMode requestedMode,
        bool themeSupportsFreeFloating)
    {
        if (hand is ClockHandKind.None or ClockHandKind.SubSecond)
        {
            return ClockHandTargetMode.Radial;
        }

        return requestedMode switch
        {
            ClockHandTargetMode.Radial => ClockHandTargetMode.Radial,
            ClockHandTargetMode.FreeFloating when themeSupportsFreeFloating => ClockHandTargetMode.FreeFloating,
            ClockHandTargetMode.FreeFloating => ClockHandTargetMode.Radial,
            _ => themeSupportsFreeFloating
                ? ClockHandTargetMode.FreeFloating
                : ClockHandTargetMode.Radial,
        };
    }
}
