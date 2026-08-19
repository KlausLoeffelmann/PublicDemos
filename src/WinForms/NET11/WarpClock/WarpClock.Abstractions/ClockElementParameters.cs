using System.Drawing;

namespace WarpClock.Abstractions;

/// <summary>
///  Mutable per-element parameters a theme animator may influence each tick.
/// </summary>
/// <remarks>
///  These are the <b>only</b> levers a theme has over an element at runtime. None
///  of them can change which time a hand points at — hand rotation is derived by
///  the engine from the element's target anchor.
/// </remarks>
public sealed class ClockElementParameters
{
    /// <summary>Whether the element's visual is shown.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    ///  Tri-state numeral visibility used by the engine's magnetic-numeral aiming.
    ///  <see cref="ClockNumeralVisibility.Transparent"/> hides the numeral but keeps it
    ///  as a target; <see cref="ClockNumeralVisibility.Invisible"/> hides it and makes the
    ///  hands skip it. Defaults to <see cref="ClockNumeralVisibility.Visible"/> and has no
    ///  effect on non-magnetic themes beyond hiding the visual when not
    ///  <see cref="ClockNumeralVisibility.Visible"/>.
    /// </summary>
    public ClockNumeralVisibility Visibility { get; set; } = ClockNumeralVisibility.Visible;

    /// <summary>Offset (design units) added to the element's layout anchor.</summary>
    public PointF AnchorOffset { get; set; }

    /// <summary>Uniform scale applied to the element's visual about its pivot.</summary>
    public float Scale { get; set; } = 1f;

    /// <summary>Horizontal skew in degrees applied about the pivot.</summary>
    public float SkewDegrees { get; set; }

    /// <summary>
    ///  Extra rotation (degrees, clockwise) applied about the pivot. For
    ///  non-hands this spins the element freely; for hands it is layered on top
    ///  of the engine-derived pointing rotation and is intended only for small
    ///  stylistic wobble — it cannot be used to misrepresent the time because the
    ///  engine clamps hand wobble to a small bound.
    /// </summary>
    public float ExtraRotationDegrees { get; set; }

    /// <summary>Opacity 0..1. Honored by renderers that support it (forces a redraw when changed).</summary>
    public float Opacity { get; set; } = 1f;

    /// <summary>
    ///  Per-hand override for how the engine chooses the time target this element points at.
    ///  Non-hand elements ignore it; unsafe requests fall back to the engine's safe default.
    /// </summary>
    public ClockHandTargetMode HandTargetMode { get; set; } = ClockHandTargetMode.ThemeDefault;

    /// <summary>Optional text the renderer may use (e.g. a numeral override during a blend).</summary>
    public string? Text { get; set; }

    /// <summary>Blend / transition progress 0..1 a renderer may use for cross-fades.</summary>
    public float Progress { get; set; }

    /// <summary>When set by the animator, the engine redraws the element's content this frame.</summary>
    public bool RedrawRequested { get; set; }

    /// <summary>Theme scratch state associated with this element.</summary>
    public object? Tag { get; set; }
}
