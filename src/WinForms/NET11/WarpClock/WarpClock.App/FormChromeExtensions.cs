namespace WarpClock.App;

/// <summary>
///  Provides the demo's taskbar-safe, borderless window mode.
/// </summary>
internal static class FormChromeExtensions
{
    /// <summary>
    ///  Removes the non-client window chrome while keeping the form inside the screen's
    ///  working area, so the taskbar, menu strip, and status strip remain available.
    /// </summary>
    /// <param name="form">The form to display without Windows chrome.</param>
    /// <param name="topMost">Whether the borderless form should stay above other windows.</param>
    /// <returns>The window state needed to restore the original chrome.</returns>
    internal static FormChromeState HideWindowsChrome(this Form form, bool topMost)
    {
        ArgumentNullException.ThrowIfNull(form);

        Rectangle windowedBounds = form.WindowState == FormWindowState.Normal
            ? form.Bounds
            : form.RestoreBounds;

        FormChromeState state = new(
            form.FormBorderStyle,
            form.WindowState,
            windowedBounds,
            form.TopMost);

        Rectangle workingArea = Screen.FromControl(form).WorkingArea;
        form.SuspendLayout();
        form.WindowState = FormWindowState.Normal;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Bounds = workingArea;
        form.TopMost = topMost;
        form.ResumeLayout(performLayout: true);

        return state;
    }

    /// <summary>
    ///  Restores window chrome and bounds captured by <see cref="HideWindowsChrome"/>.
    /// </summary>
    /// <param name="form">The form whose Windows chrome should be restored.</param>
    /// <param name="state">The state captured when the chrome was hidden.</param>
    internal static void RestoreWindowsChrome(this Form form, FormChromeState state)
    {
        ArgumentNullException.ThrowIfNull(form);

        FormWindowState windowState = state.WindowState == FormWindowState.Minimized
            ? FormWindowState.Normal
            : state.WindowState;

        form.SuspendLayout();
        form.TopMost = state.TopMost;
        form.WindowState = FormWindowState.Normal;
        form.FormBorderStyle = state.BorderStyle;
        form.Bounds = state.Bounds;
        form.WindowState = windowState;
        form.ResumeLayout(performLayout: true);
    }
}

/// <summary>
///  Captures the form properties changed by the no-chrome demo mode.
/// </summary>
/// <param name="BorderStyle">The original border style.</param>
/// <param name="WindowState">The original non-minimized window state.</param>
/// <param name="Bounds">The original normal window bounds.</param>
/// <param name="TopMost">The original topmost setting.</param>
internal readonly record struct FormChromeState(
    FormBorderStyle BorderStyle,
    FormWindowState WindowState,
    Rectangle Bounds,
    bool TopMost);
