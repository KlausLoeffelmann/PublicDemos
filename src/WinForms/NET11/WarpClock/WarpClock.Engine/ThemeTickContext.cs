using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  The engine's implementation of <see cref="IClockTickContext"/>. Reused each tick;
///  the control refreshes <see cref="Time"/>/<see cref="FrameDelta"/> before invoking
///  the theme animator.
/// </summary>
internal sealed class ThemeTickContext : IClockTickContext
{
    private readonly Func<ClockElementId, ClockElementParameters> _parametersOf;
    private readonly IReadOnlyList<ClockElementDescriptor> _elements;

    public ThemeTickContext(
        IReadOnlyList<ClockElementDescriptor> elements,
        Func<ClockElementId, ClockElementParameters> parametersOf)
    {
        _elements = elements;
        _parametersOf = parametersOf;
    }

    public ClockTimeSnapshot Time { get; set; }

    public TimeSpan FrameDelta { get; set; }

    public IReadOnlyList<ClockElementDescriptor> Elements => _elements;

    public float FaceRotationDegrees { get; set; }

    public ClockElementParameters GetParameters(ClockElementId id) => _parametersOf(id);
}
