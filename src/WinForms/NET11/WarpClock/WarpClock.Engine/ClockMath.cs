using System.Drawing;

namespace WarpClock.Engine;

/// <summary>
///  Small angle / geometry helpers shared by the engine. All clock angles are in
///  degrees, measured clockwise from the 12 o'clock position (straight up).
/// </summary>
internal static class ClockMath
{
    public const float DegreesToRadians = MathF.PI / 180f;
    public const float RadiansToDegrees = 180f / MathF.PI;

    /// <summary>Normalizes an angle into the [0, 360) range.</summary>
    public static float Normalize360(float degrees)
    {
        degrees %= 360f;
        return degrees < 0f ? degrees + 360f : degrees;
    }

    /// <summary>Returns the signed shortest difference (−180, 180] from <paramref name="from"/> to <paramref name="to"/>.</summary>
    public static float ShortestDelta(float from, float to)
    {
        float delta = Normalize360(to - from);
        if (delta > 180f)
        {
            delta -= 360f;
        }

        return delta;
    }

    /// <summary>
    ///  Returns the point at clock-angle <paramref name="degrees"/> and the given
    ///  pixel <paramref name="radius"/> from <paramref name="center"/>.
    /// </summary>
    public static PointF PointOnDial(PointF center, float radius, float degrees)
    {
        float rad = degrees * DegreesToRadians;
        return new PointF(
            center.X + MathF.Sin(rad) * radius,
            center.Y - MathF.Cos(rad) * radius);
    }

    /// <summary>
    ///  Returns the clock-angle (clockwise from 12, in [0,360)) of the vector from
    ///  <paramref name="from"/> to <paramref name="to"/>.
    /// </summary>
    public static float AngleTo(PointF from, PointF to)
    {
        float dx = to.X - from.X;
        float dy = to.Y - from.Y;
        // Clock 0° is up (−Y); clockwise positive.
        float degrees = MathF.Atan2(dx, -dy) * RadiansToDegrees;
        return Normalize360(degrees);
    }

    /// <summary>Linearly interpolates between two points.</summary>
    public static PointF Lerp(PointF a, PointF b, float t)
        => new(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);

    /// <summary>Cubic ease-in-out on a normalized [0,1] value.</summary>
    public static float EaseInOut(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return t < 0.5f ? 4f * t * t * t : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
    }

    /// <summary>Rotates <paramref name="point"/> about <paramref name="center"/> by a clockwise angle.</summary>
    public static PointF RotateAbout(PointF point, PointF center, float degrees)
    {
        if (degrees == 0f)
        {
            return point;
        }

        float rad = degrees * DegreesToRadians;
        float cos = MathF.Cos(rad);
        float sin = MathF.Sin(rad);
        float dx = point.X - center.X;
        float dy = point.Y - center.Y;

        // Screen-space clockwise rotation.
        return new PointF(
            center.X + dx * cos - dy * sin,
            center.Y + dx * sin + dy * cos);
    }
}
