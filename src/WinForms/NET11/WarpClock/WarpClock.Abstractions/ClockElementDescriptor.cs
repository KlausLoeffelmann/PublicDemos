using System.Drawing;

namespace WarpClock.Abstractions;

/// <summary>
///  Immutable description of a clock element a theme wants the engine to
///  materialize as its own DirectComposition visual.
/// </summary>
/// <remarks>
///  <para>
///   Content is authored in an element-local design space sized
///   <see cref="ContentSize"/> (design units; the engine scales it to pixels).
///   <see cref="Pivot"/> is the point in that space that sits on the element's
///   layout anchor and acts as the rotation center.
///  </para>
///  <para>
///   Hands must be authored <b>pointing toward 12 o'clock</b> (straight up,
///   −Y) from the pivot. The engine rotates the visual by the authoritative
///   clock angle (or, in free-floating layouts, by the angle that aims the hand
///   at its target anchor), so the displayed time is always correct.
///  </para>
/// </remarks>
public sealed record ClockElementDescriptor
{
    /// <summary>The element identity.</summary>
    public required ClockElementId Id { get; init; }

    /// <summary>The element-local content size in design units.</summary>
    public required SizeF ContentSize { get; init; }

    /// <summary>The pivot / rotation-center / anchor point within the content.</summary>
    public PointF Pivot { get; init; }

    /// <summary>The time target a hand tracks; <see cref="ClockHandKind.None"/> for non-hands.</summary>
    public ClockHandKind Hand { get; init; } = ClockHandKind.None;

    /// <summary>Higher values render in front of lower ones.</summary>
    public int ZOrder { get; init; }

    /// <summary>
    ///  When <see langword="true"/>, the engine asks the renderer to redraw the
    ///  element's cached content every frame (for content that depends on the
    ///  current time, e.g. a binary read-out hand or a drum). When
    ///  <see langword="false"/>, content is drawn once and only re-drawn when a
    ///  parameter change requests it.
    /// </summary>
    public bool RedrawPerFrame { get; init; }

    /// <summary>Convenience flag: whether this descriptor represents a hand.</summary>
    public bool IsHand => Hand != ClockHandKind.None;
}
