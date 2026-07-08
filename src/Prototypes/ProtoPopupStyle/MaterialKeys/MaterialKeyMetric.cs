namespace MaterialKeys;

/// <summary>
///  A DPI-neutral, generic magnitude used by the internal static settings of
///  <see cref="MaterialKeyButton"/> (corner radius, concavity depth, highlight/shadow strength).
/// </summary>
/// <remarks>
///  <para>
///   Using generic magnitudes instead of pixel values allows the renderer to resolve the actual
///   device values per call, which keeps HighDPI scenarios trivially correct.
///  </para>
/// </remarks>
public enum MaterialKeyMetric
{
    /// <summary>
    ///  A small magnitude.
    /// </summary>
    Small = 0,

    /// <summary>
    ///  A medium magnitude. This is the default.
    /// </summary>
    Medium = 1,

    /// <summary>
    ///  A large magnitude.
    /// </summary>
    Large = 2
}
