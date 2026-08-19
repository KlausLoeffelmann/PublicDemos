namespace WarpClock.Abstractions;

/// <summary>
///  Immutable host-supplied indexed image metadata or references.
/// </summary>
public readonly record struct ClockIndexedImageSnapshot
{
    /// <summary>The image slot index.</summary>
    public required int Index { get; init; }

    /// <summary>The host-defined image source or asset key.</summary>
    public string? Source { get; init; }

    /// <summary>An optional human-readable description.</summary>
    public string? Description { get; init; }
}
