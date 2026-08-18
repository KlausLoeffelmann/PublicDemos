using System.Drawing;

namespace WarpClock.Engine;

internal readonly record struct OledSceneTransform(float Scale, Point Offset)
{
    public static OledSceneTransform Identity { get; } = new(1f, Point.Empty);

    public bool IsIdentity => Scale == 1f && Offset == Point.Empty;
}
