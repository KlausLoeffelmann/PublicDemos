namespace WarpClock.Abstractions;

/// <summary>
///  Declares what a theme needs from the engine. The engine validates these and
///  enforces the time-correctness invariant (for example, it disables the
///  <c>Crawling</c> second-hand motion when <see cref="FreeFloating"/> is set,
///  because a crawling sweep is incompatible with aiming a hand at relocated
///  target anchors).
/// </summary>
public sealed record ThemeCapabilities
{
    /// <summary>
    ///  When <see langword="true"/>, the theme may place element anchors anywhere
    ///  (not just on the dial circle). Hands then aim at their interpolated target
    ///  anchors and continuous crawling is disabled in favor of grace catch-up.
    /// </summary>
    public bool FreeFloating { get; init; }

    /// <summary>
    ///  When <see langword="true"/>, a global face rotation also rotates the hand
    ///  pivots/targets, so the time stays correct relative to the rotating face.
    ///  When <see langword="false"/>, the face can spin while the hands keep aiming
    ///  at their absolute targets.
    /// </summary>
    public bool HandsFollowFaceRotation { get; init; } = true;

    /// <summary>The default themes' capabilities (radial, hands follow face).</summary>
    public static ThemeCapabilities Default { get; } = new();
}
