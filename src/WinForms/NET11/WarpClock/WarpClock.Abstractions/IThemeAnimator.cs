namespace WarpClock.Abstractions;

/// <summary>
///  Optional per-tick driver for a theme's dynamic effects. The engine invokes
///  <see cref="OnTick"/> once per rendered frame on the UI thread; read
///  <see cref="IClockTickContext.FrameDelta"/> for the elapsed time and integrate against
///  it so effects are frame-rate independent. Implementations mutate element parameters
///  and the face rotation through the supplied context; they must not block.
/// </summary>
public interface IThemeAnimator
{
    /// <summary>Called once when the theme becomes active, before the first tick.</summary>
    void Initialize(IClockTickContext context) { }

    /// <summary>Advances the theme's effects for one tick.</summary>
    void OnTick(IClockTickContext context);
}
