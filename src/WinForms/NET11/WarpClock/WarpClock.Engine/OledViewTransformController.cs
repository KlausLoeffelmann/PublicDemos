using System.Drawing;

namespace WarpClock.Engine;

internal sealed class OledViewTransformController
{
    private const float EdgeInsetFactor = 0.015f;
    private const float MinEdgeInsetPixels = 4f;
    private const float MaxEdgeInsetPixels = 16f;
    private const float TravelUsage = 0.80f;
    private const float TransitionSeconds = 1.5f;

    private static readonly double HorizontalSweepPeriodSeconds = TimeSpan.FromMinutes(18).TotalSeconds;
    private static readonly double VerticalSweepPeriodSeconds = TimeSpan.FromMinutes(24).TotalSeconds;

    private double _elapsedSeconds;
    private float _blend;

    public OledSceneTransform Current { get; private set; } = OledSceneTransform.Identity;

    public OledSceneTransform Advance(TimeSpan frameDelta, SizeF hostSurface, OledViewMode mode)
    {
        float dt = MathF.Max(0f, (float)frameDelta.TotalSeconds);

        if (mode == OledViewMode.General || _blend > 0f)
        {
            _elapsedSeconds += dt;
        }

        float targetBlend = mode == OledViewMode.General ? 1f : 0f;
        _blend = MoveTowards(_blend, targetBlend, dt / TransitionSeconds);

        if (_blend <= 0f || hostSurface.Width < 2f || hostSurface.Height < 2f)
        {
            Current = OledSceneTransform.Identity;
            return Current;
        }

        OledSceneTransform general = SampleGeneralTransform(hostSurface, TimeSpan.FromSeconds(_elapsedSeconds));
        Current = new OledSceneTransform(
            1f - ((1f - general.Scale) * _blend),
            new Point(
                (int)MathF.Round(general.Offset.X * _blend, MidpointRounding.AwayFromZero),
                (int)MathF.Round(general.Offset.Y * _blend, MidpointRounding.AwayFromZero)));

        return Current;
    }

    internal static OledSceneTransform SampleGeneralTransform(SizeF hostSurface, TimeSpan elapsed)
    {
        if (hostSurface.Width < 2f || hostSurface.Height < 2f)
        {
            return OledSceneTransform.Identity;
        }

        float minDimension = MathF.Min(hostSurface.Width, hostSurface.Height);
        float requestedInset = Math.Clamp(minDimension * EdgeInsetFactor, MinEdgeInsetPixels, MaxEdgeInsetPixels);
        float minScale = Math.Clamp(1f - (2f * requestedInset / MathF.Max(minDimension, 1f)), 0.90f, 1f);
        float safeInset = minDimension * (1f - minScale) / 2f;
        int maxOffset = Math.Max(0, (int)MathF.Floor(safeInset * TravelUsage));

        if (maxOffset == 0)
        {
            return OledSceneTransform.Identity;
        }

        float xWave = TriangleWave(elapsed.TotalSeconds / HorizontalSweepPeriodSeconds);
        float yWave = TriangleWave(elapsed.TotalSeconds / VerticalSweepPeriodSeconds + 0.5d);
        float excursion = MathF.Max(MathF.Abs(xWave), MathF.Abs(yWave));
        float scale = 1f - ((1f - minScale) * excursion);

        return new OledSceneTransform(
            scale,
            new Point(
                (int)MathF.Round(xWave * maxOffset, MidpointRounding.AwayFromZero),
                (int)MathF.Round(yWave * maxOffset, MidpointRounding.AwayFromZero)));
    }

    private static float MoveTowards(float current, float target, float step)
    {
        if (step <= 0f || current == target)
        {
            return current;
        }

        float delta = target - current;
        if (MathF.Abs(delta) <= step)
        {
            return target;
        }

        return current + (MathF.Sign(delta) * step);
    }

    private static float TriangleWave(double cycles)
    {
        cycles -= Math.Floor(cycles);

        if (cycles < 0.25d)
        {
            return (float)(cycles * 4d);
        }

        if (cycles < 0.75d)
        {
            return (float)(2d - (cycles * 4d));
        }

        return (float)((cycles * 4d) - 4d);
    }
}
