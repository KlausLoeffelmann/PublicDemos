using WarpClock.Abstractions;

namespace WarpClock.App;

public partial class FormMain
{
    // ── Theme catalog (dynamic: built-ins + discovered plug-ins) ──

    private sealed class ThemeEntry(IClockTheme theme, string display, string source)
    {
        public IClockTheme Theme { get; } = theme;
        public string Display { get; } = display;
        public string Source { get; } = source;
    }
}
