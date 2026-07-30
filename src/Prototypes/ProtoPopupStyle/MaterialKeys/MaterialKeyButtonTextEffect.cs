namespace MaterialKeys;

/// <summary>
///  Determines how the caption of a <see cref="MaterialKeyButton"/> is physically integrated
///  into the key surface.
/// </summary>
public enum MaterialKeyButtonTextEffect
{
    /// <summary>
    ///  The caption appears slightly raised above the key surface (embossed).
    /// </summary>
    Raised = 0,

    /// <summary>
    ///  The caption appears slightly recessed into the key surface (engraved/letterpress).
    /// </summary>
    Engraved = 1,

    /// <summary>
    ///  The caption is drawn flat, without any relief effect.
    /// </summary>
    Flat = 2
}
