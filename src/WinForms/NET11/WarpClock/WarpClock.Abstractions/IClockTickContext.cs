namespace WarpClock.Abstractions;

/// <summary>
///  Context handed to a theme's <see cref="IThemeAnimator"/> each tick. It exposes
///  the authoritative time (read-only) and the limited set of levers a theme may
///  pull: per-element parameters and the global face rotation.
/// </summary>
public interface IClockTickContext
{
    /// <summary>The authoritative time/angles for this tick.</summary>
    ClockTimeSnapshot Time { get; }

    /// <summary>Elapsed time since the previous tick.</summary>
    TimeSpan FrameDelta { get; }

    /// <summary>The full set of elements the active theme declared.</summary>
    IReadOnlyList<ClockElementDescriptor> Elements { get; }

    /// <summary>
    ///  The global face rotation in degrees (clockwise). Rotates non-hand elements
    ///  about the dial center; whether hands rotate with it is governed by
    ///  <see cref="ThemeCapabilities.HandsFollowFaceRotation"/>.
    /// </summary>
    float FaceRotationDegrees { get; set; }

    /// <summary>Gets the mutable parameters for the given element.</summary>
    ClockElementParameters GetParameters(ClockElementId id);
}
