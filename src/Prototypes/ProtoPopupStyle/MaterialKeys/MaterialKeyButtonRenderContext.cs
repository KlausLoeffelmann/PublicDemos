namespace MaterialKeys;

/// <summary>
///  Carries every piece of information <see cref="MaterialKeyButtonRenderer"/> needs to render a
///  material key, without any dependency on a live <see cref="MaterialKeyButton"/> instance.
/// </summary>
/// <remarks>
///  <para>
///   Because the renderer receives all state through this context, it can be driven from other
///   controls, a designer surface, a preview-image generator or a design-time adornment. A
///   caller only needs a <see cref="Graphics"/> target and this context.
///  </para>
/// </remarks>
public sealed class MaterialKeyButtonRenderContext
{
    /// <summary>
    ///  Gets the bounds to render into, in device pixels of the target <see cref="Graphics"/>.
    /// </summary>
    public required Rectangle Bounds { get; init; }

    /// <summary>
    ///  Gets the caption text. May be <see langword="null"/> or empty.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    ///  Gets the font used for the caption.
    /// </summary>
    public required Font Font { get; init; }

    /// <summary>
    ///  Gets the effective key face color.
    /// </summary>
    public Color BackColor { get; init; } = SystemColors.Control;

    /// <summary>
    ///  Gets the effective caption color.
    /// </summary>
    public Color ForeColor { get; init; } = SystemColors.ControlText;

    /// <summary>
    ///  Gets the border color of the key body.
    /// </summary>
    public Color BorderColor { get; init; } = SystemColors.ControlDark;

    /// <summary>
    ///  Gets the border width in device pixels. <c>0</c> renders no border.
    /// </summary>
    public int BorderWidth { get; init; } = 1;

    /// <summary>
    ///  Gets a value indicating whether the key is enabled.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    ///  Gets a value indicating whether the key has keyboard focus and should show a focus cue.
    /// </summary>
    public bool Focused { get; init; }

    /// <summary>
    ///  Gets a value indicating whether the key is the default button of its dialog.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    ///  Gets the interactive render state.
    /// </summary>
    public MaterialKeyButtonRenderState RenderState { get; init; }

    /// <summary>
    ///  Gets the animation progress snapshot.
    /// </summary>
    public MaterialKeyButtonAnimationState AnimationState { get; init; }

    /// <summary>
    ///  Gets the caption alignment within the keytop.
    /// </summary>
    public ContentAlignment TextAlign { get; init; } = ContentAlignment.MiddleCenter;

    /// <summary>
    ///  Gets the right-to-left setting for text rendering.
    /// </summary>
    public RightToLeft RightToLeft { get; init; } = RightToLeft.No;

    /// <summary>
    ///  Gets the padding applied around the caption inside the bowl.
    /// </summary>
    public Padding Padding { get; init; }

    /// <summary>
    ///  Gets the DPI of the target device. Used to scale all chrome metrics.
    /// </summary>
    public int DeviceDpi { get; init; } = 96;

    /// <summary>
    ///  Gets the caption relief effect.
    /// </summary>
    public MaterialKeyButtonTextEffect TextEffect { get; init; } = MaterialKeyButtonTextEffect.Raised;

    /// <summary>
    ///  Gets a value indicating whether keyboard cues (mnemonic underlines) should be shown.
    /// </summary>
    public bool ShowKeyboardCues { get; init; } = true;

    /// <summary>
    ///  Gets a value indicating whether a high-contrast accessibility theme is active.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   When <see langword="true"/>, the renderer falls back to a flat, higher-contrast style
    ///   without material emulation.
    ///  </para>
    /// </remarks>
    public bool HighContrast { get; init; } = SystemInformation.HighContrast;

    /// <summary>
    ///  Gets the rendering options to use. Defaults to the process-wide shared options.
    /// </summary>
    public MaterialKeyButtonRendererOptions Options { get; init; } = MaterialKeyButtonRendererOptions.Shared;
}
