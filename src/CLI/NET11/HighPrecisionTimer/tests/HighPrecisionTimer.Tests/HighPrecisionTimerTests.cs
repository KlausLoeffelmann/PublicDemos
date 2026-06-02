// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms.Animation;

namespace HighPrecisionTimer.Tests;

/// <summary>
/// A test synchronization context that executes posted callbacks immediately
/// on the thread pool, simulating a UI message pump for testing purposes.
/// </summary>
internal sealed class TestSynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback d, object? state)
    {
        ThreadPool.QueueUserWorkItem(_ => d(state));
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        d(state);
    }
}

public sealed class HighPrecisionTimerTests : IDisposable
{
    private readonly SynchronizationContext? _originalContext;

    public HighPrecisionTimerTests()
    {
        _originalContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
    }

    public void Dispose()
    {
        System.Windows.Forms.Animation.HighPrecisionTimer.Reset();
        SynchronizationContext.SetSynchronizationContext(_originalContext);
    }

    [Fact]
    public async Task SingleConsumer_ReceivesTicksAtExpectedRate()
    {
        // Arrange
        var intervals = new ConcurrentBag<double>();
        var sw = Stopwatch.StartNew();
        double lastTick = 0;
        int tickCount = 0;
        const int targetTicks = 60;

        using var registration = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                double now = sw.Elapsed.TotalMilliseconds;
                if (lastTick > 0)
                {
                    intervals.Add(now - lastTick);
                }

                lastTick = now;
                Interlocked.Increment(ref tickCount);
                return ValueTask.CompletedTask;
            });

        // Act: let it run for ~1 second worth of ticks.
        while (tickCount < targetTicks)
        {
            await Task.Delay(50);
        }

        // Assert
        var sortedIntervals = intervals.OrderBy(x => x).ToList();
        double targetMs = System.Windows.Forms.Animation.HighPrecisionTimer.TargetFrameTimeMs;

        double p95 = GetPercentile(sortedIntervals, 0.95);
        double p99 = GetPercentile(sortedIntervals, 0.99);

        // P95 should be within 30% of target frame time.
        Assert.True(
            p95 < targetMs * 1.30,
            $"P95 interval ({p95:F2}ms) exceeds 130% of target ({targetMs:F2}ms)");

        // P99 should be within 50% of target frame time.
        Assert.True(
            p99 < targetMs * 1.50,
            $"P99 interval ({p99:F2}ms) exceeds 150% of target ({targetMs:F2}ms)");
    }

    [Fact]
    public async Task MultipleConsumers_AllReceiveTicksIndependently()
    {
        // Arrange
        const int consumerCount = 5;
        const int targetTicks = 30;
        var tickCounts = new int[consumerCount];
        var registrations = new System.Windows.Forms.Animation.HighPrecisionTimer.TimerRegistration[consumerCount];

        for (int i = 0; i < consumerCount; i++)
        {
            int idx = i;
            registrations[i] = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
                (tick, ct) =>
                {
                    Interlocked.Increment(ref tickCounts[idx]);
                    return ValueTask.CompletedTask;
                });
        }

        // Act
        while (tickCounts.Min() < targetTicks)
        {
            await Task.Delay(50);
        }

        // Cleanup
        foreach (var reg in registrations)
        {
            reg.Dispose();
        }

        // Assert: all consumers received at least target ticks.
        for (int i = 0; i < consumerCount; i++)
        {
            Assert.True(
                tickCounts[i] >= targetTicks,
                $"Consumer {i} received only {tickCounts[i]} ticks (expected >= {targetTicks})");
        }
    }

    [Fact]
    public async Task SlowConsumer_DropsFramesInsteadOfQueuing()
    {
        // Arrange: a consumer that takes longer than one frame to process.
        var ticks = new ConcurrentBag<HighPrecisionTimerTick>();
        int tickCount = 0;

        using var registration = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
            async (tick, ct) =>
            {
                ticks.Add(tick);
                Interlocked.Increment(ref tickCount);
                // Simulate slow rendering (2x frame time).
                await Task.Delay((int)(System.Windows.Forms.Animation.HighPrecisionTimer.TargetFrameTimeMs * 2));
            });

        // Act: let it run for ~2 seconds.
        await Task.Delay(2000);

        // Assert: some frames should have been dropped.
        var tickList = ticks.ToList();
        int totalDropped = tickList.Sum(t => t.DroppedFrames);

        Assert.True(
            totalDropped > 0,
            "Expected dropped frames for a slow consumer, but none were reported.");
    }

    [Fact]
    public async Task MixedConsumers_FastAndSlowDoNotInterfere()
    {
        // Arrange
        const int targetTicks = 40;
        var fastIntervals = new ConcurrentBag<double>();
        var fastSw = Stopwatch.StartNew();
        double fastLastTick = 0;
        int fastTickCount = 0;
        int slowTickCount = 0;

        // Fast consumer: ~1ms work.
        using var fastReg = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
            async (tick, ct) =>
            {
                double now = fastSw.Elapsed.TotalMilliseconds;
                if (fastLastTick > 0)
                {
                    fastIntervals.Add(now - fastLastTick);
                }

                fastLastTick = now;
                Interlocked.Increment(ref fastTickCount);
                await Task.Delay(1); // Simulate fast rendering.
            });

        // Slow consumer: ~40ms work.
        using var slowReg = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
            async (tick, ct) =>
            {
                Interlocked.Increment(ref slowTickCount);
                await Task.Delay(40); // Simulate slow rendering.
            });

        // Act
        while (fastTickCount < targetTicks)
        {
            await Task.Delay(50);
        }

        // Assert: the fast consumer should not be impacted by the slow one.
        var sortedFastIntervals = fastIntervals.OrderBy(x => x).ToList();
        double targetMs = System.Windows.Forms.Animation.HighPrecisionTimer.TargetFrameTimeMs;
        double p95 = GetPercentile(sortedFastIntervals, 0.95);

        Assert.True(
            p95 < targetMs * 1.40,
            $"Fast consumer P95 ({p95:F2}ms) degraded by slow consumer. Target: {targetMs:F2}ms");

        // Slow consumer should have significantly fewer ticks due to frame dropping.
        Assert.True(
            slowTickCount < fastTickCount,
            $"Slow consumer ({slowTickCount} ticks) should have fewer ticks than fast ({fastTickCount})");
    }

    [Fact]
    public async Task VariableWorkload_MaintainsSteadyFrameRate()
    {
        // Arrange: consumer that varies between 1ms and 8ms of work.
        var intervals = new ConcurrentBag<double>();
        var sw = Stopwatch.StartNew();
        double lastTick = 0;
        int tickCount = 0;
        var rng = new Random(42);
        const int targetTicks = 60;

        using var registration = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
            async (tick, ct) =>
            {
                double now = sw.Elapsed.TotalMilliseconds;
                if (lastTick > 0)
                {
                    intervals.Add(now - lastTick);
                }

                lastTick = now;
                Interlocked.Increment(ref tickCount);

                // Variable workload: 1-8ms.
                int workMs = rng.Next(1, 9);
                await Task.Delay(workMs);
            });

        // Act
        while (tickCount < targetTicks)
        {
            await Task.Delay(50);
        }

        // Assert
        var sortedIntervals = intervals.OrderBy(x => x).ToList();
        double targetMs = System.Windows.Forms.Animation.HighPrecisionTimer.TargetFrameTimeMs;
        double p95 = GetPercentile(sortedIntervals, 0.95);
        double p99 = GetPercentile(sortedIntervals, 0.99);
        double mean = sortedIntervals.Average();

        // Mean should be close to target.
        Assert.True(
            Math.Abs(mean - targetMs) < targetMs * 0.25,
            $"Mean interval ({mean:F2}ms) deviates too much from target ({targetMs:F2}ms)");

        Assert.True(
            p95 < targetMs * 1.30,
            $"P95 interval ({p95:F2}ms) exceeds 130% of target ({targetMs:F2}ms)");

        Assert.True(
            p99 < targetMs * 1.50,
            $"P99 interval ({p99:F2}ms) exceeds 150% of target ({targetMs:F2}ms)");
    }

    [Fact]
    public async Task ManyConsumers_ScalesWithoutExcessiveJitter()
    {
        // Arrange: 20 consumers with varying workloads.
        const int consumerCount = 20;
        const int targetTicks = 30;
        var allIntervals = new ConcurrentBag<double>[consumerCount];
        var tickCounts = new int[consumerCount];
        var registrations = new System.Windows.Forms.Animation.HighPrecisionTimer.TimerRegistration[consumerCount];
        var stopwatches = new Stopwatch[consumerCount];
        var lastTicks = new double[consumerCount];

        for (int i = 0; i < consumerCount; i++)
        {
            allIntervals[i] = new ConcurrentBag<double>();
            stopwatches[i] = Stopwatch.StartNew();
            int idx = i;

            registrations[i] = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
                async (tick, ct) =>
                {
                    double now = stopwatches[idx].Elapsed.TotalMilliseconds;
                    if (lastTicks[idx] > 0)
                    {
                        allIntervals[idx].Add(now - lastTicks[idx]);
                    }

                    lastTicks[idx] = now;
                    Interlocked.Increment(ref tickCounts[idx]);

                    // Simulate workload proportional to index (0-4ms).
                    int workMs = idx % 5;
                    if (workMs > 0)
                    {
                        await Task.Delay(workMs);
                    }
                });
        }

        // Act
        while (tickCounts.Min() < targetTicks)
        {
            await Task.Delay(100);
        }

        // Cleanup
        foreach (var reg in registrations)
        {
            reg.Dispose();
        }

        // Assert: aggregate P95 across all consumers should be reasonable.
        var allIntervalsFlat = allIntervals.SelectMany(x => x).OrderBy(x => x).ToList();
        double targetMs = System.Windows.Forms.Animation.HighPrecisionTimer.TargetFrameTimeMs;
        double p95 = GetPercentile(allIntervalsFlat, 0.95);

        Assert.True(
            p95 < targetMs * 1.50,
            $"Aggregate P95 ({p95:F2}ms) across {consumerCount} consumers exceeds " +
            $"150% of target ({targetMs:F2}ms)");
    }

    [Fact]
    public async Task Registration_Disposal_StopsCallbacks()
    {
        // Arrange
        int tickCount = 0;

        var registration = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                Interlocked.Increment(ref tickCount);
                return ValueTask.CompletedTask;
            });

        // Let it run a bit.
        await Task.Delay(200);
        int ticksBeforeDispose = tickCount;

        // Act
        registration.Dispose();
        await Task.Delay(200);

        // Assert: no (or very few) additional ticks after dispose.
        int ticksAfterDispose = tickCount - ticksBeforeDispose;
        Assert.True(
            ticksAfterDispose <= 2,
            $"Received {ticksAfterDispose} ticks after unregistration (expected <= 2 in-flight)");
    }

    [Fact]
    public void Registration_WithoutSyncContext_Throws()
    {
        // Arrange: remove sync context.
        var original = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);

        try
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                System.Windows.Forms.Animation.HighPrecisionTimer.Register(
                    (tick, ct) => ValueTask.CompletedTask));
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }
    }

    [Fact]
    public async Task TimerTick_ProvidesAccurateElapsedTime()
    {
        // Arrange
        var elapsedValues = new ConcurrentBag<double>();
        int tickCount = 0;
        const int targetTicks = 30;

        using var registration = System.Windows.Forms.Animation.HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                if (tick.FrameIndex > 0)
                {
                    elapsedValues.Add(tick.Elapsed.TotalMilliseconds);
                }

                Interlocked.Increment(ref tickCount);
                return ValueTask.CompletedTask;
            });

        // Act
        while (tickCount < targetTicks)
        {
            await Task.Delay(50);
        }

        // Assert: elapsed values should be close to target frame time.
        var sorted = elapsedValues.OrderBy(x => x).ToList();
        double targetMs = System.Windows.Forms.Animation.HighPrecisionTimer.TargetFrameTimeMs;
        double median = GetPercentile(sorted, 0.50);

        Assert.True(
            Math.Abs(median - targetMs) < targetMs * 0.25,
            $"Median elapsed ({median:F2}ms) deviates too much from target ({targetMs:F2}ms)");
    }

    private static double GetPercentile(List<double> sortedValues, double percentile)
    {
        if (sortedValues.Count == 0)
        {
            return 0;
        }

        int index = (int)Math.Ceiling(percentile * sortedValues.Count) - 1;
        return sortedValues[Math.Max(0, index)];
    }
}
