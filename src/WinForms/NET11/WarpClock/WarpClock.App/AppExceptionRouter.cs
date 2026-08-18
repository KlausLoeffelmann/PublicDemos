using Microsoft.Extensions.Logging;
using WarpToolkit.Desktop.AppServices;

namespace WarpClock.App;

/// <summary>
///  Centralizes exception logging and replaces the stock WinForms exception dialog.
/// </summary>
public sealed class AppExceptionRouter : IDisposable
{
    private readonly IWinFormsAppExceptionService _exceptionService;
    private readonly AppPaths _paths;
    private readonly StartupOptions _startupOptions;
    private readonly ILogger<AppExceptionRouter> _logger;
    private IWin32Window? _owner;
    private Action<string>? _statusReporter;
    private bool _started;

    public AppExceptionRouter(
        IWinFormsAppExceptionService exceptionService,
        AppPaths paths,
        StartupOptions startupOptions,
        ILogger<AppExceptionRouter> logger)
    {
        ArgumentNullException.ThrowIfNull(exceptionService);
        ArgumentNullException.ThrowIfNull(paths);
        ArgumentNullException.ThrowIfNull(startupOptions);
        ArgumentNullException.ThrowIfNull(logger);

        _exceptionService = exceptionService;
        _paths = paths;
        _startupOptions = startupOptions;
        _logger = logger;
    }

    public void Start(IWin32Window? owner, Action<string>? statusReporter = null)
    {
        if (_started)
        {
            return;
        }

        _owner = owner;
        _statusReporter = statusReporter;
        _exceptionService.RegisterExceptionHandler(OnThreadException);
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        _started = true;
    }

    public void Stop()
    {
        if (!_started)
        {
            return;
        }

        _exceptionService.UnregisterExceptionHandler(OnThreadException);
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        _statusReporter = null;
        _started = false;
    }

    public void Dispose() => Stop();

    private void OnThreadException(object? sender, System.Threading.ThreadExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Unhandled UI exception.");

        if (_startupOptions.DebugRunSeconds is not null)
        {
            return;
        }

        try
        {
            _statusReporter?.Invoke(
                $"Recovered from an unexpected error: {e.Exception.Message} (details in {_paths.LogDirectory})");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not report the recoverable exception on the UI status surface.");
        }
    }

    private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            _logger.LogCritical(exception, "Unhandled non-UI exception. IsTerminating={IsTerminating}", e.IsTerminating);
        }
        else
        {
            _logger.LogCritical("Unhandled non-UI exception object: {ExceptionObject}. IsTerminating={IsTerminating}", e.ExceptionObject, e.IsTerminating);
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Unobserved task exception.");
        e.SetObserved();
    }
}
