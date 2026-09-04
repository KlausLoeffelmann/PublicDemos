using System.Diagnostics;

namespace SplitFlap.Visuals;

/// <summary>
///  The controller that drives a whole board: one dedicated thread, one clock, any number of
///  <see cref="SplitFlapCharacterVisual"/> instances. Advances state and renders back buffers
///  off the UI thread; the UI thread only blits.
/// </summary>
/// <remarks>
///  <para>
///   Most code never needs to create one. Controls register with <see cref="Default"/>, which
///   starts on first use, so several boards in one process share a single clock, just like the
///   real thing had one controller for the whole hall.
///  </para>
///  <para>
///   Events forwarded from visuals are raised on the animator thread. Do not touch controls in
///   those handlers; use <c>Control.InvokeAsync</c>.
///  </para>
/// </remarks>
public sealed class SplitFlapAnimator : IAsyncDisposable, IDisposable
{
    private static readonly Lazy<SplitFlapAnimator> s_default = new(() => new SplitFlapAnimator());

    private readonly Lock _sync = new();
    private readonly List<SplitFlapCharacterVisual> _visuals = [];
    private readonly List<TaskCompletionSource> _pendingRegistrations = [];
    private CancellationTokenSource? _cts;
    private Task? _loop;
    private bool _disposed;

    private SplitFlapAnimator(int framesPerSecond = 60)
        => FramesPerSecond = Math.Clamp(framesPerSecond, 10, 240);

    /// <summary>
    ///  The process-wide animator. Started lazily on first registration.
    /// </summary>
    public static SplitFlapAnimator Default
        => s_default.Value;

    /// <summary>
    ///  Raised on the animator thread after a frame in which at least one visual changed.
    /// </summary>
    public event EventHandler? FrameRendered;

    /// <summary>
    ///  Forwarded from every registered visual. Animator thread.
    /// </summary>
    public event EventHandler<FlapEventArgs>? FlapFell;

    /// <summary>
    ///  Forwarded from every registered visual. Animator thread.
    /// </summary>
    public event EventHandler<FlapEventArgs>? Jammed;

    /// <summary>
    ///  Forwarded from every registered visual. Animator thread.
    /// </summary>
    public event EventHandler<FlapEventArgs>? Settled;

    /// <summary>
    ///  Target frame rate.
    /// </summary>
    public int FramesPerSecond { get; }

    /// <summary>
    ///  <see langword="true"/> while the animator thread runs.
    /// </summary>
    public bool IsRunning
        => _loop is { IsCompleted: false };

    /// <summary>
    ///  Number of visuals currently registered.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_sync)
            {
                return _visuals.Count;
            }
        }
    }

    /// <summary>
    ///  Creates an independent animator, e.g. to run one board at a different frame rate.
    /// </summary>
    public static SplitFlapAnimator Create(int framesPerSecond = 60)
        => new(framesPerSecond);

    /// <summary>
    ///  Registers a visual and starts the animator if necessary.
    /// </summary>
    /// <returns>A task that completes once the visual has been rendered for the first time.</returns>
    public Task RegisterAsync(SplitFlapCharacterVisual visual)
    {
        ArgumentNullException.ThrowIfNull(visual);

        return RegisterAsync([visual]);
    }

    /// <summary>
    ///  Registers several visuals at once and starts the animator if necessary.
    /// </summary>
    /// <returns>A task that completes once all of them have been rendered for the first time.</returns>
    public Task RegisterAsync(IEnumerable<SplitFlapCharacterVisual> visuals)
    {
        ArgumentNullException.ThrowIfNull(visuals);
        ObjectDisposedException.ThrowIf(_disposed, this);

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        lock (_sync)
        {
            foreach (SplitFlapCharacterVisual visual in visuals)
            {
                if (_visuals.Contains(visual))
                {
                    continue;
                }

                _visuals.Add(visual);
                visual.FlapFell += OnVisualFlapFell;
                visual.Jammed += OnVisualJammed;
                visual.Settled += OnVisualSettled;
                visual.Invalidate();
            }

            _pendingRegistrations.Add(completion);
        }

        Start();

        return completion.Task;
    }

    /// <summary>
    ///  Removes a visual. The visual is not disposed.
    /// </summary>
    public void Unregister(SplitFlapCharacterVisual visual)
    {
        ArgumentNullException.ThrowIfNull(visual);

        lock (_sync)
        {
            if (_visuals.Remove(visual))
            {
                visual.FlapFell -= OnVisualFlapFell;
                visual.Jammed -= OnVisualJammed;
                visual.Settled -= OnVisualSettled;
            }
        }
    }

    /// <summary>
    ///  Removes several visuals. The visuals are not disposed.
    /// </summary>
    public void Unregister(IEnumerable<SplitFlapCharacterVisual> visuals)
    {
        ArgumentNullException.ThrowIfNull(visuals);

        foreach (SplitFlapCharacterVisual visual in visuals)
        {
            Unregister(visual);
        }
    }

    /// <summary>
    ///  Starts the animator thread. Idempotent.
    /// </summary>
    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_sync)
        {
            if (IsRunning)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            _loop = Task.Factory.StartNew(
                () => Loop(token),
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }
    }

    /// <summary>
    ///  Starts the animator and returns a task that completes when it stops, either through the
    ///  token or through disposal.
    /// </summary>
    public Task RunAsync(CancellationToken cancellationToken = default)
    {
        Start();

        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() => _cts?.Cancel());
        }

        return _loop!;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _cts?.Cancel();

        if (_loop is not null)
        {
            try
            {
                await _loop.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected.
            }
        }

        _cts?.Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void Loop(CancellationToken token)
    {
        Thread.CurrentThread.Name = "SplitFlapAnimator";
        Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

        TimeSpan frame = TimeSpan.FromSeconds(1.0 / FramesPerSecond);
        TimeSpan hiccup = TimeSpan.FromMilliseconds(250);
        Stopwatch clock = Stopwatch.StartNew();
        TimeSpan last = clock.Elapsed;

        while (!token.IsCancellationRequested)
        {
            TimeSpan now = clock.Elapsed;
            TimeSpan elapsed = now - last;
            last = now;

            // Debugger break, sleep/resume, GC pause: don't let the board catch up in one frame.
            if (elapsed > hiccup)
            {
                elapsed = frame;
            }

            SplitFlapCharacterVisual[] visuals;
            TaskCompletionSource[] pending;

            lock (_sync)
            {
                visuals = [.. _visuals];
                pending = [.. _pendingRegistrations];
                _pendingRegistrations.Clear();
            }

            bool anyChanged = false;

            foreach (SplitFlapCharacterVisual visual in visuals)
            {
                try
                {
                    if (visual.Advance(elapsed))
                    {
                        visual.RenderFrame();
                        anyChanged = true;
                    }
                }
                catch (ObjectDisposedException)
                {
                    Unregister(visual);
                }
                catch (ArgumentException)
                {
                    // A font was replaced under us mid-frame; the next frame will use the new one.
                }
            }

            foreach (TaskCompletionSource completion in pending)
            {
                completion.TrySetResult();
            }

            if (anyChanged || pending.Length > 0)
            {
                FrameRendered?.Invoke(this, EventArgs.Empty);
            }

            TimeSpan budget = frame - (clock.Elapsed - now);

            if (budget > TimeSpan.FromMilliseconds(1))
            {
                Thread.Sleep(budget);
            }
        }

        lock (_sync)
        {
            foreach (TaskCompletionSource completion in _pendingRegistrations)
            {
                completion.TrySetCanceled(token);
            }

            _pendingRegistrations.Clear();
        }
    }

    private void OnVisualFlapFell(object? sender, FlapEventArgs e)
        => FlapFell?.Invoke(this, e);

    private void OnVisualJammed(object? sender, FlapEventArgs e)
        => Jammed?.Invoke(this, e);

    private void OnVisualSettled(object? sender, FlapEventArgs e)
        => Settled?.Invoke(this, e);
}
