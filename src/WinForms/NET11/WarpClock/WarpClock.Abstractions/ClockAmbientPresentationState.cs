namespace WarpClock.Abstractions;

/// <summary>
///  Identifies whether host ambient content is in its default or alternate state.
/// </summary>
public enum ClockAmbientPresentationState
{
    /// <summary>The default ambient/time-zone state.</summary>
    Default,

    /// <summary>An alternate ambient/time-zone state.</summary>
    Alternate
}
