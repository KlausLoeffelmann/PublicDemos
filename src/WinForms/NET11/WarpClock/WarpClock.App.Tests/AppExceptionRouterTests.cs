using Microsoft.Extensions.Logging.Abstractions;
using WarpToolkit.Desktop.AppServices;

namespace WarpClock.App.Tests;

public sealed class AppExceptionRouterTests
{
    [Fact]
    public void RecoverableUiException_IsReportedThroughStatusCallback()
    {
        FakeExceptionService exceptionService = new();
        AppExceptionRouter router = new(
            exceptionService,
            new AppPaths(),
            StartupOptions.Empty,
            NullLogger<AppExceptionRouter>.Instance);

        string? reportedStatus = null;
        router.Start(owner: null, statusReporter: message => reportedStatus = message);

        exceptionService.Raise(new InvalidOperationException("Synthetic UI failure"));

        Assert.NotNull(reportedStatus);
        Assert.Contains("Synthetic UI failure", reportedStatus, StringComparison.Ordinal);
    }

    private sealed class FakeExceptionService : IWinFormsAppExceptionService
    {
        private System.Threading.ThreadExceptionEventHandler? _handler;

        public void RegisterExceptionHandler(System.Threading.ThreadExceptionEventHandler threadExceptionEventHandler)
            => _handler = threadExceptionEventHandler;

        public void UnregisterExceptionHandler(System.Threading.ThreadExceptionEventHandler threadExceptionEventHandler)
        {
            if (_handler == threadExceptionEventHandler)
            {
                _handler = null;
            }
        }

        public void Raise(Exception exception)
            => _handler?.Invoke(this, new System.Threading.ThreadExceptionEventArgs(exception));
    }
}
