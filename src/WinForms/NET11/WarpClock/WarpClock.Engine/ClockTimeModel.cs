using System.Diagnostics;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  The single, authoritative source of clock time. Converts the real wall clock
///  (optionally shifted by a demo offset and accelerated by a speed multiplier) into
///  a <see cref="ClockTimeSnapshot"/> of continuous hand angles. No theme can alter
///  what this produces, which is what guarantees the displayed time is always right.
/// </summary>
public sealed class ClockTimeModel
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private double _lastElapsedSeconds;
    private TimeSpan _accumulatedOffset = TimeSpan.Zero;

    /// <summary>An additional fixed offset (e.g. a time-zone demo offset).</summary>
    public TimeSpan TimeOffset { get; set; } = TimeSpan.Zero;

    /// <summary>A speed multiplier for demos. 1.0 is real time.</summary>
    public double SpeedMultiplier { get; set; } = 1.0;

    /// <summary>Resets the accumulated fast-forward offset (call when speed returns to 1×).</summary>
    public void ResetAccumulatedOffset() => _accumulatedOffset = TimeSpan.Zero;

    /// <summary>Computes the authoritative snapshot for the current instant.</summary>
    public ClockTimeSnapshot CreateSnapshot()
    {
        double elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;
        DateTime realNow = DateTime.Now;

        if (SpeedMultiplier != 1.0)
        {
            double deltaSeconds = elapsedSeconds - _lastElapsedSeconds;
            _accumulatedOffset += TimeSpan.FromSeconds(deltaSeconds * (SpeedMultiplier - 1.0));
        }

        _lastElapsedSeconds = elapsedSeconds;

        DateTime now = realNow + TimeOffset + _accumulatedOffset;

        float fractionalSecond = now.Millisecond / 1000f;
        float totalSeconds = now.Second + fractionalSecond;
        float totalMinutes = now.Minute + totalSeconds / 60f;
        float totalHours = (now.Hour % 12) + totalMinutes / 60f;

        return new ClockTimeSnapshot
        {
            Now = now,
            SecondAngle = ClockMath.Normalize360(totalSeconds * 6f),
            MinuteAngle = ClockMath.Normalize360(totalMinutes * 6f),
            HourAngle = ClockMath.Normalize360(totalHours * 30f),
            SubSecondAngle = ClockMath.Normalize360(fractionalSecond * 360f),
        };
    }
}
