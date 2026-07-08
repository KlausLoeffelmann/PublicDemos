namespace MaterialKeys;

/// <summary>
///  Timing information delivered with each animation frame tick of <see cref="HighPrecisionTimer"/>.
/// </summary>
/// <param name="frame">The monotonically increasing frame counter.</param>
/// <param name="elapsedSinceLastTick">The time since the previous tick delivered to this registration.</param>
/// <param name="totalElapsed">The total time since the timer thread started.</param>
internal readonly struct HighPrecisionTimerTick(long frame, TimeSpan elapsedSinceLastTick, TimeSpan totalElapsed)
{
    /// <summary>
    ///  Gets the monotonically increasing frame counter.
    /// </summary>
    public long Frame { get; } = frame;

    /// <summary>
    ///  Gets the time elapsed since the previous tick delivered to this registration.
    /// </summary>
    public TimeSpan ElapsedSinceLastTick { get; } = elapsedSinceLastTick;

    /// <summary>
    ///  Gets the total time elapsed since the timer thread started.
    /// </summary>
    public TimeSpan TotalElapsed { get; } = totalElapsed;
}

/// <summary>
///  Handle for a <see cref="HighPrecisionTimer"/> registration. Dispose to unregister.
/// </summary>
internal sealed class TimerRegistration(long id) : IDisposable
{
    private long _id = id;

    /// <inheritdoc/>
    public void Dispose()
    {
        long id = Interlocked.Exchange(ref _id, 0);

        if (id != 0)
        {
            HighPrecisionTimer.Unregister(id);
        }
    }
}

/// <summary>
///  Shared high-resolution animation timer with a tick resolution of up to 10 ms.
/// </summary>
/// <remarks>
///  <para>
///   Stand-in implementation matching the registration contract of the runtime's
///   <c>HighPrecisionTimer</c>. If the real timer is available in the target library, delete
///   this file — the consuming code only relies on <see cref="Register"/>,
///   <see cref="HighPrecisionTimerTick"/> and <see cref="TimerRegistration"/>.
///  </para>
///  <para>
///   A single background thread drives all registrations. Callbacks are marshaled to the
///   <see cref="SynchronizationContext"/> captured at registration time, and a registration is
///   never re-entered: if a callback is still in flight when the next tick fires, that tick is
///   skipped for the registration (the following tick reports the accumulated elapsed time).
///  </para>
/// </remarks>
internal static class HighPrecisionTimer
{
    private const int TargetResolutionMilliseconds = 10;

    private static readonly ConcurrentDictionary<long, Registration> s_registrations = new();
    private static readonly Lock s_gate = new();

    private static long s_nextId;
    private static Thread? s_thread;

    /// <summary>
    ///  Registers a callback to be invoked on each animation frame tick.
    ///  The current <see cref="SynchronizationContext"/> is captured and used
    ///  to marshal the callback to the appropriate thread.
    /// </summary>
    /// <param name="callback">
    ///  The async callback invoked each frame. Receives timing information and a cancellation token.
    /// </param>
    /// <returns>A <see cref="TimerRegistration"/> that must be disposed to unregister.</returns>
    /// <exception cref="InvalidOperationException">
    ///  Thrown when no <see cref="SynchronizationContext"/> is available on the current thread.
    /// </exception>
    internal static TimerRegistration Register(Func<HighPrecisionTimerTick, CancellationToken, ValueTask> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        SynchronizationContext? syncContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException(
                "A SynchronizationContext must be available on the calling thread. " +
                "Ensure registration is performed from a UI thread.");

        long id = Interlocked.Increment(ref s_nextId);
        Registration registration = new(id, callback, syncContext);
        s_registrations.TryAdd(id, registration);

        EnsureRunning();

        return new TimerRegistration(id);
    }

    /// <summary>
    ///  Removes a registration and cancels any in-flight callback.
    /// </summary>
    internal static void Unregister(long id)
    {
        if (s_registrations.TryRemove(id, out Registration? registration))
        {
            registration.Cancel();
        }
    }

    private static void EnsureRunning()
    {
        lock (s_gate)
        {
            if (s_thread is { IsAlive: true })
            {
                return;
            }

            s_thread = new Thread(TimerLoop)
            {
                IsBackground = true,
                Name = "MaterialKeys.HighPrecisionTimer",
                Priority = ThreadPriority.AboveNormal
            };

            s_thread.Start();
        }
    }

    private static void TimerLoop()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        long frame = 0;
        TimeSpan lastTick = TimeSpan.Zero;

        while (true)
        {
            Thread.Sleep(TargetResolutionMilliseconds);

            TimeSpan now = stopwatch.Elapsed;
            frame++;

            foreach (Registration registration in s_registrations.Values)
            {
                registration.TryPost(frame, now, now - lastTick);
            }

            lastTick = now;

            if (s_registrations.IsEmpty)
            {
                lock (s_gate)
                {
                    // Re-check under the gate so a concurrent Register either sees the live
                    // thread or restarts it via EnsureRunning after we cleared the field.
                    if (s_registrations.IsEmpty)
                    {
                        s_thread = null;

                        return;
                    }
                }
            }
        }
    }

    private sealed class Registration(
        long id,
        Func<HighPrecisionTimerTick, CancellationToken, ValueTask> callback,
        SynchronizationContext syncContext)
    {
        private static readonly SendOrPostCallback s_onPosted =
            static state => ((Registration)state!).OnPosted();

        private readonly CancellationTokenSource _cts = new();

        private int _inFlight;
        private TimeSpan _accumulated;
        private HighPrecisionTimerTick _pendingTick;

        public long Id { get; } = id;

        public void Cancel()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        /// <summary>
        ///  Posts a tick to the captured context unless a previous callback is still in flight,
        ///  in which case the elapsed time is accumulated into the next delivered tick.
        /// </summary>
        public void TryPost(long frame, TimeSpan total, TimeSpan delta)
        {
            if (_cts.IsCancellationRequested)
            {
                return;
            }

            _accumulated += delta;

            if (Interlocked.CompareExchange(ref _inFlight, 1, 0) != 0)
            {
                return;
            }

            _pendingTick = new HighPrecisionTimerTick(frame, _accumulated, total);
            _accumulated = TimeSpan.Zero;

            syncContext.Post(s_onPosted, this);
        }

        private async void OnPosted()
        {
            try
            {
                if (!_cts.IsCancellationRequested)
                {
                    await callback(_pendingTick, _cts.Token).ConfigureAwait(true);
                }
            }
            catch (OperationCanceledException)
            {
                // Registration was disposed mid-flight -- expected.
            }
            catch (ObjectDisposedException)
            {
                // Target control went away mid-flight -- expected during teardown.
            }
            finally
            {
                Volatile.Write(ref _inFlight, 0);
            }
        }
    }
}
