namespace MaterialKeys;

/// <summary>
///  Tunable rendering options for <see cref="MaterialKeyButtonRenderer"/>.
/// </summary>
/// <remarks>
///  <para>
///   All geometric magnitudes are expressed as <see cref="MaterialKeyMetric"/> values and only
///   resolved to device pixels inside the renderer, based on the DPI carried by the render
///   context. <see cref="Shared"/> is the process-wide instance behind the internal static
///   settings on <see cref="MaterialKeyButton"/>, used to experiment with the best all-purpose
///   defaults.
///  </para>
/// </remarks>
public sealed class MaterialKeyButtonRendererOptions
{
    /// <summary>
    ///  Gets the process-wide shared options instance.
    /// </summary>
    public static MaterialKeyButtonRendererOptions Shared { get; } = new();

    /// <summary>
    ///  Gets or sets the corner radius magnitude of the key body.
    /// </summary>
    public MaterialKeyMetric CornerRadius { get; set; } = MaterialKeyMetric.Medium;

    /// <summary>
    ///  Gets or sets how deep the concave bowl of the keytop appears.
    /// </summary>
    public MaterialKeyMetric ConcavityDepth { get; set; } = MaterialKeyMetric.Medium;

    /// <summary>
    ///  Gets or sets the strength of edge highlights.
    /// </summary>
    public MaterialKeyMetric HighlightStrength { get; set; } = MaterialKeyMetric.Medium;

    /// <summary>
    ///  Gets or sets the strength of edge and bowl shadows.
    /// </summary>
    public MaterialKeyMetric ShadowStrength { get; set; } = MaterialKeyMetric.Medium;

    /// <summary>
    ///  Gets or sets the base duration of state-change animations.
    /// </summary>
    public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromMilliseconds(160);

    /// <summary>
    ///  Resolves <see cref="CornerRadius"/> to device-independent pixels (96 DPI).
    /// </summary>
    internal float GetCornerRadiusDip()
        => CornerRadius switch
        {
            MaterialKeyMetric.Small => 4f,
            MaterialKeyMetric.Large => 9f,
            _ => 6f
        };

    /// <summary>
    ///  Resolves <see cref="ConcavityDepth"/> to a fractional shading depth.
    /// </summary>
    internal float GetConcavityDepth()
        => ConcavityDepth switch
        {
            MaterialKeyMetric.Small => 0.07f,
            MaterialKeyMetric.Large => 0.17f,
            _ => 0.11f
        };

    /// <summary>
    ///  Resolves <see cref="HighlightStrength"/> to a multiplier.
    /// </summary>
    internal float GetHighlightMultiplier()
        => HighlightStrength switch
        {
            MaterialKeyMetric.Small => 0.6f,
            MaterialKeyMetric.Large => 1.5f,
            _ => 1f
        };

    /// <summary>
    ///  Resolves <see cref="ShadowStrength"/> to a multiplier.
    /// </summary>
    internal float GetShadowMultiplier()
        => ShadowStrength switch
        {
            MaterialKeyMetric.Small => 0.6f,
            MaterialKeyMetric.Large => 1.5f,
            _ => 1f
        };
}
