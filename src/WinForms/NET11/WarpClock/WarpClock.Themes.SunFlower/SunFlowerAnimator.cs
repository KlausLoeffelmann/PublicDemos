using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.SunFlower;

/// <summary>
///  Buzzes the bees: whenever a hand sweeps across an hour position, that bee spins one
///  full, eased 360° turn in place. Spins are driven through the non-hand element's
///  <see cref="ClockElementParameters.ExtraRotationDegrees"/>, which the engine applies
///  verbatim to non-hand visuals (so a full turn is allowed here).
/// </summary>
internal sealed class SunFlowerAnimator : IThemeAnimator
{
    private const float SpinSeconds = 0.7f;       // duration of one full buzz-spin
    private const float TriggerDegrees = 5f;      // a hand this close to a bee triggers it
    private const float ReleaseDegrees = 12f;     // the hand must leave this zone to re-arm

    private readonly bool[] _spinning = new bool[12];
    private readonly bool[] _armed = new bool[12];
    private readonly float[] _elapsed = new float[12];

    public void Initialize(IClockTickContext context)
    {
        // Every bee starts ready to be triggered.
        for (int i = 0; i < _armed.Length; i++)
        {
            _armed[i] = true;
        }
    }

    public void OnTick(IClockTickContext context)
    {
        float dt = (float)context.FrameDelta.TotalSeconds;

        // The three authoritative hand angles; whichever is nearest a bee can trigger it.
        float hourAngle = context.Time.HourAngle;
        float minuteAngle = context.Time.MinuteAngle;
        float secondAngle = context.Time.SecondAngle;

        for (int i = 0; i < 12; i++)
        {
            float beeAngle = i * 30f;
            float nearest = Min3(
                AngularDistance(hourAngle, beeAngle),
                AngularDistance(minuteAngle, beeAngle),
                AngularDistance(secondAngle, beeAngle));

            // Re-arm once all hands have moved away from the bee.
            if (nearest > ReleaseDegrees)
            {
                _armed[i] = true;
            }

            // Fire a fresh spin when a hand enters the trigger zone of an armed, idle bee.
            if (!_spinning[i] && _armed[i] && nearest <= TriggerDegrees)
            {
                _spinning[i] = true;
                _armed[i] = false;
                _elapsed[i] = 0f;
            }

            ClockElementParameters parameters = context.GetParameters(ClockElementId.HourMarker(i));

            if (_spinning[i])
            {
                _elapsed[i] += dt;
                float t = Math.Clamp(_elapsed[i] / SpinSeconds, 0f, 1f);

                if (t >= 1f)
                {
                    _spinning[i] = false;
                    parameters.ExtraRotationDegrees = 0f;
                }
                else
                {
                    // One eased full turn (accelerate then settle).
                    parameters.ExtraRotationDegrees = 360f * EaseInOut(t);
                }
            }
        }
    }

    private static float AngularDistance(float a, float b)
    {
        float d = MathF.Abs((a - b) % 360f);
        return MathF.Min(d, 360f - d);
    }

    private static float Min3(float a, float b, float c) => MathF.Min(a, MathF.Min(b, c));

    private static float EaseInOut(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return t < 0.5f ? 4f * t * t * t : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
