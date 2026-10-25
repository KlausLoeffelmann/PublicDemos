using DrumMachine.Demo.Controls;

namespace DrumMachine.Demo;

partial class MainForm
{
    private Rectangle _normalBounds;
    private bool _applyingSplitterConstraints;

    /// <inheritdoc/>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ApplySplitterConstraints();
        if (DesignMode || IgnoreSettings)
        {
            return;
        }

        RestoreWindowPlacement();
        RestoreSplitter();
        RestoreColumnLayout();
    }

    /// <inheritdoc/>
    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        if (!DesignMode && _contentSplit is not null)
        {
            ApplySplitterConstraints();
        }
    }

    /// <summary>
    ///  Applies splitter floors only after the control is tall enough to satisfy both panels.
    /// </summary>
    private void ApplySplitterConstraints()
    {
        if (_applyingSplitterConstraints || _contentSplit is null || _contentSplit.IsDisposed)
        {
            return;
        }

        const int panel1Min = 100;
        const int panel2Min = 140;
        if (_contentSplit.Panel1MinSize == panel1Min && _contentSplit.Panel2MinSize == panel2Min)
        {
            return;
        }

        int available = _contentSplit.Height - _contentSplit.SplitterWidth;
        if (available < panel1Min + panel2Min)
        {
            return;
        }

        int distance = _contentSplit.SplitterDistance;
        _applyingSplitterConstraints = true;
        try
        {
            _contentSplit.Panel1MinSize = panel1Min;
            _contentSplit.Panel2MinSize = panel2Min;
            int maximum = available - panel2Min;
            _contentSplit.SplitterDistance = Math.Clamp(distance, panel1Min, Math.Max(panel1Min, maximum));
        }
        finally
        {
            _applyingSplitterConstraints = false;
        }
    }

    /// <inheritdoc/>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (WindowState == FormWindowState.Normal && IsHandleCreated && Bounds.Width > 0 && Bounds.Height > 0)
        {
            _normalBounds = Bounds;
        }
    }

    private void RestoreWindowPlacement()
    {
        WindowPlacement placement = _settings.Window;
        if (!placement.HasBounds)
        {
            return;
        }

        Rectangle bounds = placement.ScaleToDpi(DeviceDpi);
        if (!WindowPlacement.IsUsableOnScreen(bounds))
        {
            AppLogger.Information("Settings", "Saved window bounds were off-screen and were not restored.");
            return;
        }

        StartPosition = FormStartPosition.Manual;
        Bounds = bounds;
        _normalBounds = bounds;
        if (placement.IsMaximized)
        {
            WindowState = FormWindowState.Maximized;
        }
    }

    private void RestoreSplitter()
    {
        if (_settings.SpectrumSplitterDistanceDip <= 0 || _contentSplit.Height <= 0)
        {
            return;
        }

        int desired = WindowPlacement.ScaleFromDip(_settings.SpectrumSplitterDistanceDip, DeviceDpi);
        int maximum = _contentSplit.Height - _contentSplit.SplitterWidth - _contentSplit.Panel2MinSize;
        if (maximum < _contentSplit.Panel1MinSize)
        {
            return;
        }

        _contentSplit.SplitterDistance = Math.Clamp(desired, _contentSplit.Panel1MinSize, maximum);
    }

    private void RestoreColumnLayout()
    {
        if (_settings.StepGridColumns.Count == 0)
        {
            return;
        }

        _stepGrid.UseExplicitStepWidths();
        if (!StepGridColumnPreference.TryApply(_stepGrid, _settings.StepGridColumns, DeviceDpi))
        {
            AppLogger.Information("Settings", "Saved score-column layout was ignored because it no longer matches the grid.");
        }
    }

    private void CaptureWindowLayout()
    {
        if (IgnoreSettings || !IsHandleCreated || _contentSplit is null || _stepGrid is null)
        {
            return;
        }

        Rectangle bounds = _normalBounds.Width > 0
            ? _normalBounds
            : WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        if (bounds.Width >= 200 && bounds.Height >= 150 && DeviceDpi is >= 96 and <= 480)
        {
            _settings = _settings.WithWindowPlacement(bounds, WindowState == FormWindowState.Maximized, DeviceDpi);
        }
        if (_contentSplit.Height > _contentSplit.SplitterWidth)
        {
            int distanceDip = WindowPlacement.ScaleToDip(_contentSplit.SplitterDistance, DeviceDpi);
            _settings = _settings.WithSpectrumSplitterDistanceDip(Math.Clamp(distanceDip, 40, 4_000));
        }

        _settings = _settings.WithStepGridColumns(StepGridColumnPreference.Capture(_stepGrid, DeviceDpi));
    }
}
