using WarpClock.Abstractions;

namespace WarpClock.Themes.EmberClock;

/// <summary>
///  Toggles each hour's ember as the second hand crosses it, then eases the flame and its numeral
///  to lit/out. Only a still-easing element requests a redraw. Never sets a hand angle.
/// </summary>
internal sealed class EmberClockAnimator : IThemeAnimator
{
    private const float DegreesPerHour = 360f / EmberClockTheme.HourCount;
    private const float EaseRatePerSecond = 3.2f;   // reaches its target in ~0.3s

    private readonly bool[] _lit = new bool[EmberClockTheme.HourCount];
    private readonly float[] _burn = new float[EmberClockTheme.HourCount];
    private int _coveredHour = -1;

    public void Initialize(IClockTickContext ctx)
    {
        for (int i = 0; i < EmberClockTheme.HourCount; i++)
        {
            _lit[i] = true;
            _burn[i] = 1f;
            SetBurn(ctx, i, 1f);
        }

        _coveredHour = HourUnder(ctx.Time.SecondAngle);
    }

    public void OnTick(IClockTickContext ctx)
    {
        int hour = HourUnder(ctx.Time.SecondAngle);
        if (hour != _coveredHour)
        {
            _lit[hour] = !_lit[hour];   // toggle on entering a new hour
            _coveredHour = hour;
        }

        float maxStep = EaseRatePerSecond * (float)ctx.FrameDelta.TotalSeconds;

        for (int i = 0; i < EmberClockTheme.HourCount; i++)
        {
            float target = _lit[i] ? 1f : 0f;
            float diff = target - _burn[i];
            if (diff == 0f)
            {
                continue;   // settled: no redraw
            }

            _burn[i] = MathF.Abs(diff) <= maxStep ? target : _burn[i] + MathF.Sign(diff) * maxStep;
            SetBurn(ctx, i, _burn[i]);
        }
    }

    /// <summary>Writes an hour's burn to its flame and numeral, and requests a redraw.</summary>
    private static void SetBurn(IClockTickContext ctx, int hour, float burn)
    {
        ClockElementParameters flame = ctx.GetParameters(ClockElementId.HourMarker(hour));
        flame.Progress = burn;
        flame.RedrawRequested = true;

        ClockElementParameters mark = ctx.GetParameters(ClockElementId.CustomElement(hour));
        mark.Progress = burn;
        mark.RedrawRequested = true;
    }

    /// <summary>Nearest hour (0..11) to the given clock angle; 0 = 12 o'clock.</summary>
    private static int HourUnder(float angleDegrees)
    {
        int idx = (int)MathF.Round(angleDegrees / DegreesPerHour);
        return ((idx % EmberClockTheme.HourCount) + EmberClockTheme.HourCount) % EmberClockTheme.HourCount;
    }
}
