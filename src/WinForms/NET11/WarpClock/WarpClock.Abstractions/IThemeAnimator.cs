namespace WarpClock.Abstractions;

/// <summary>
///  Optional per-tick driver for a theme's dynamic effects. The engine invokes
///  <see cref="OnTick"/> at a fixed cadence (tenths of a second) on the UI thread.
///  Implementations mutate element parameters and the face rotation through the
///  supplied context; they must not block.
/// </summary>
public interface IThemeAnimator
{
    /// <summary>Called once when the theme becomes active, before the first tick.</summary>
    void Initialize(IClockTickContext context) { }

    /// <summary>Advances the theme's effects for one tick.</summary>
    void OnTick(IClockTickContext context);
}
