namespace SplitFlap.Visuals;

/// <summary>
///  How fast a single flap falls. This is the time per <em>flap</em>, not per transition:
///  a transition from 'A' to 'Z' walks the whole drum and takes 25 flaps.
/// </summary>
public enum FlipAnimationSpeed
{
    /// <summary>~220 ms per flap. Good for very large characters or slow-motion demos.</summary>
    VerySlow,

    /// <summary>~150 ms per flap.</summary>
    Slow,

    /// <summary>~90 ms per flap. Roughly what a well-maintained 1980s board did.</summary>
    Medium,

    /// <summary>~55 ms per flap.</summary>
    Fast,

    /// <summary>~32 ms per flap. Nervous.</summary>
    VeryFast
}

/// <summary>
///  Helpers for <see cref="FlipAnimationSpeed"/>.
/// </summary>
public static class FlipAnimationSpeedExtensions
{
    /// <summary>
    ///  Converts the speed into the duration of one flap fall in milliseconds.
    /// </summary>
    public static double ToMillisecondsPerFlap(this FlipAnimationSpeed speed)
        => speed switch
        {
            FlipAnimationSpeed.VerySlow => 220,
            FlipAnimationSpeed.Slow => 150,
            FlipAnimationSpeed.Fast => 55,
            FlipAnimationSpeed.VeryFast => 32,
            _ => 90
        };
}

/// <summary>
///  Event data for flap events. Raised on the animator thread, never on the UI thread.
/// </summary>
/// <param name="visual">The visual that raised the event.</param>
/// <param name="character">The character now showing on the visual.</param>
public sealed class FlapEventArgs(SplitFlapCharacterVisual visual, char character) : EventArgs
{
    /// <summary>The visual that raised the event.</summary>
    public SplitFlapCharacterVisual Visual { get; } = visual;

    /// <summary>The character now showing on the visual.</summary>
    public char Character { get; } = character;
}
