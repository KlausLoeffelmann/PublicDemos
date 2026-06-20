namespace WarpClock.Abstractions;

/// <summary>
///  Marks a concrete <see cref="IClockTheme"/> implementation as discoverable by the
///  plug-in loader and carries optional display metadata. Applying the attribute is
///  optional — the loader also discovers public, parameterless
///  <see cref="IClockTheme"/> implementations — but it lets a plug-in opt a type out
///  (via <see cref="Discoverable"/>) or annotate it.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ClockThemeExportAttribute : Attribute
{
    /// <summary>Whether the loader should surface this theme. Defaults to <see langword="true"/>.</summary>
    public bool Discoverable { get; init; } = true;

    /// <summary>Optional display name override; falls back to <see cref="IClockTheme.Name"/>.</summary>
    public string? DisplayName { get; init; }
}
