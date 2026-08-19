using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  The engine's implementation of <see cref="IClockRenderContext"/>, passed to the
///  theme renderer when (re)drawing one element's cached content.
/// </summary>
internal sealed class ElementRenderContext : IClockRenderContext
{
    public ClockElementId Id { get; set; }

    public SizeF ContentSize { get; set; }

    public PointF Pivot { get; set; }

    public ClockElementParameters Parameters { get; set; } = new();

    public ClockTimeSnapshot Time { get; set; }

    public ClockTimeZoneSnapshot TimeZone { get; set; } = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Local, DateTime.Now);

    public ClockAmbientSnapshot Ambient { get; set; } = ClockAmbientSnapshot.Empty;

    public float Scale { get; set; } = 1f;
}
