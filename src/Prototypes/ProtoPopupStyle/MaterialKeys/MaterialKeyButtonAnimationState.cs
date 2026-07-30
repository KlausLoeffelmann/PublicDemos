namespace MaterialKeys;

/// <summary>
///  Immutable snapshot of the animation progress of a <see cref="MaterialKeyButton"/>.
/// </summary>
/// <remarks>
///  <para>
///   The control owns the animation <em>timing</em>; the renderer only consumes the resulting
///   progress values. This keeps <see cref="MaterialKeyButtonRenderer"/> usable outside a live
///   control (designer surfaces, preview bitmaps, adornments) where no timer exists — simply
///   pass fixed progress values.
///  </para>
/// </remarks>
/// <param name="hoverProgress">Hover progress, <c>0</c> (not hovered) to <c>1</c> (fully hovered).</param>
/// <param name="pressProgress">Press progress, <c>0</c> (released) to <c>1</c> (fully pressed).</param>
public readonly struct MaterialKeyButtonAnimationState(float hoverProgress, float pressProgress)
{
    /// <summary>
    ///  Gets a state with no hover and no press applied.
    /// </summary>
    public static MaterialKeyButtonAnimationState None
        => default;

    /// <summary>
    ///  Gets the hover progress in the range <c>0</c>–<c>1</c>.
    /// </summary>
    public float HoverProgress { get; } = Math.Clamp(hoverProgress, 0f, 1f);

    /// <summary>
    ///  Gets the press progress in the range <c>0</c>–<c>1</c>.
    /// </summary>
    public float PressProgress { get; } = Math.Clamp(pressProgress, 0f, 1f);
}
