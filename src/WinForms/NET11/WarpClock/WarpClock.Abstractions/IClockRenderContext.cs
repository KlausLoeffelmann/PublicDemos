using System.Drawing;

namespace WarpClock.Abstractions;

/// <summary>
///  Context handed to a theme's <see cref="IClockElementRenderer"/> when (re)drawing
///  one element's cached content. Drawing happens in element-local pixel space with
///  the origin at the top-left of <see cref="ContentSize"/>.
/// </summary>
public interface IClockRenderContext
{
    /// <summary>The element being drawn.</summary>
    ClockElementId Id { get; }

    /// <summary>The element's content size in pixels (already scaled by the engine).</summary>
    SizeF ContentSize { get; }

    /// <summary>The element's pivot point in pixels within <see cref="ContentSize"/>.</summary>
    PointF Pivot { get; }

    /// <summary>The element's current parameters (opacity, text, progress, etc.).</summary>
    ClockElementParameters Parameters { get; }

    /// <summary>The authoritative time snapshot, for time-dependent content.</summary>
    ClockTimeSnapshot Time { get; }

    /// <summary>
    ///  The displayed time-zone snapshot for the current render. Legacy
    ///  implementations remain compatible through this default interface member.
    /// </summary>
    ClockTimeZoneSnapshot TimeZone => ClockTimeZoneSnapshot.Create(TimeZoneInfo.Local, Time.Now);

    /// <summary>
    ///  Immutable host ambient content, such as overlay messages or indexed images.
    ///  Legacy implementations remain compatible through this default interface member.
    /// </summary>
    ClockAmbientSnapshot Ambient => ClockAmbientSnapshot.Empty;

    /// <summary>The scale factor from design units to pixels the engine applied.</summary>
    float Scale { get; }
}
