namespace MaterialKeys;

/// <summary>
///  Describes the interactive render state of a <see cref="MaterialKeyButton"/>.
/// </summary>
/// <remarks>
///  <para>
///   The states are flags so that reasonable combinations (e.g. <see cref="Hover"/> together with
///   <see cref="Focused"/>, or <see cref="Pressed"/> together with <see cref="Focused"/>) can be
///   expressed. <see cref="Disabled"/> overrides all interactive states during rendering.
///  </para>
/// </remarks>
[Flags]
public enum MaterialKeyButtonRenderState
{
    /// <summary>
    ///  The key is idle.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///  The pointer hovers over the key.
    /// </summary>
    Hover = 1,

    /// <summary>
    ///  The key is pressed (mouse, touch or keyboard).
    /// </summary>
    Pressed = 2,

    /// <summary>
    ///  The key is disabled.
    /// </summary>
    Disabled = 4,

    /// <summary>
    ///  The key has the keyboard focus.
    /// </summary>
    Focused = 8,

    /// <summary>
    ///  The key is the default button of its dialog.
    /// </summary>
    Default = 16
}
