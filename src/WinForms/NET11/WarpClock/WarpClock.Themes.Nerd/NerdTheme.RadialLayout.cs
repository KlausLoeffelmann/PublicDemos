using WarpClock.Abstractions;

namespace WarpClock.Themes.Nerd;

public sealed partial class NerdTheme
{
    private sealed class RadialLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            anchor = default;
            return false;
        }
    }
}
