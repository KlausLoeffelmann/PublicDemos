namespace CameraControlDemo;

/// <summary>
///  Demo shell that lets the user pick a camera and a capture resolution and shows the
///  live feed in a <see cref="CameraView"/>.
/// </summary>
public partial class MainForm : Form
{
    /// <summary>
    ///  The capture engine driving <c>_cameraView</c>.
    /// </summary>
    private readonly CameraCapture _capture;

    /// <summary>
    ///  Set while a ComboBox <c>DataSource</c> is being filled, so the resulting
    ///  <c>SelectedIndexChanged</c> notifications do not restart the camera.
    /// </summary>
    private bool _suppressSelectionChanged;

    /// <summary>
    ///  Set while the form is shutting down, so no further restarts are attempted.
    /// </summary>
    private bool _closing;

    private bool _captureDisposed;

    /// <summary>
    ///  Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();

        _capture = new CameraCapture(_cameraView);
        _capture.Error += Capture_Error;

        _cameraView.KeepAspectRatio = _keepAspectRatioButton.Checked;
        _cameraView.StatusText = "Looking for cameras\u2026";
    }

    /// <summary>
    ///  Enumerates the available cameras and starts the first one.
    /// </summary>
    /// <param name="e">Unused event data.</param>
    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await ReloadDevicesAsync();
    }

    /// <summary>
    ///  Renders toolbar symbols once the form has its monitor DPI.
    /// </summary>
    /// <param name="e">Unused event data.</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateToolbarImages();
    }

    /// <inheritdoc/>
    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        UpdateToolbarImages();
    }

    /// <inheritdoc/>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        if (IsHandleCreated && _imageFactory is not null)
        {
            UpdateToolbarImages();
        }
    }

    /// <summary>
    ///  Tears the capture session down before the window goes away.
    /// </summary>
    /// <param name="e">The closing event data.</param>
    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (e.Cancel || _captureDisposed)
        {
            return;
        }

        e.Cancel = true;

        if (_closing)
        {
            return;
        }

        _closing = true;
        SetBusy(true);

        try
        {
            await _capture.DisposeAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"The camera could not be released: {ex.Message}",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _capture.Error -= Capture_Error;
            _captureDisposed = true;
            Close();
        }
    }

    /// <summary>
    ///  Re-enumerates the cameras and restarts the preview.
    /// </summary>
    /// <returns>A task that completes once the preview is running.</returns>
    private async Task ReloadDevicesAsync()
    {
        if (_closing)
        {
            return;
        }

        SetBusy(true);

        try
        {
            IReadOnlyList<CameraDevice> devices = await CameraCapture.GetDevicesAsync();

            if (_closing)
            {
                return;
            }

            _suppressSelectionChanged = true;

            try
            {
                _cameraComboBox.ComboBox.DataSource = null;
                _cameraComboBox.ComboBox.DataSource = devices.ToList();
                _cameraComboBox.SelectedIndex = devices.Count > 0 ? 0 : -1;

                _formatComboBox.ComboBox.DataSource = null;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            if (devices.Count == 0)
            {
                await _capture.StopAsync();
                if (_closing)
                {
                    return;
                }

                _cameraView.ClearFrame();
                _cameraView.StatusText = "No camera found.";

                return;
            }

            await RestartAsync(reloadFormats: true);
        }
        catch (Exception ex)
        {
            ShowError($"The camera list could not be read: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    /// <summary>
    ///  Restarts the preview with the currently selected device and format.
    /// </summary>
    /// <param name="reloadFormats">
    ///  <see langword="true"/> to refill the resolution list from the freshly opened
    ///  device (used when the device changed).
    /// </param>
    /// <returns>A task that completes once the preview is running.</returns>
    private async Task RestartAsync(bool reloadFormats)
    {
        if (_closing || _cameraComboBox.SelectedItem is not CameraDevice device)
        {
            return;
        }

        CameraFormat? format = reloadFormats
            ? null
            : _formatComboBox.SelectedItem as CameraFormat;

        SetBusy(true);

        try
        {
            _cameraView.ClearFrame();
            _cameraView.StatusText = $"Starting {device.DisplayName}\u2026";

            bool started = await _capture.StartAsync(device, format);

            if (!started || _closing)
            {
                return;
            }

            if (reloadFormats)
            {
                _suppressSelectionChanged = true;

                try
                {
                    _formatComboBox.ComboBox.DataSource = null;
                    _formatComboBox.ComboBox.DataSource = _capture.SupportedFormats.ToList();
                    _formatComboBox.SelectedIndex =
                        _capture.SupportedFormats.Count > 0 ? 0 : -1;
                }
                finally
                {
                    _suppressSelectionChanged = false;
                }
            }

            _cameraView.StatusText = "Waiting for the first frame\u2026";
        }
        catch (Exception ex)
        {
            ShowError($"The camera preview could not be restarted: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    /// <summary>
    ///  Restarts the preview on the newly selected camera.
    /// </summary>
    /// <param name="sender">The camera ComboBox.</param>
    /// <param name="e">Unused event data.</param>
    private async void CameraComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_suppressSelectionChanged)
        {
            return;
        }

        await RestartAsync(reloadFormats: true);
    }

    /// <summary>
    ///  Restarts the preview with the newly selected capture format.
    /// </summary>
    /// <param name="sender">The resolution ComboBox.</param>
    /// <param name="e">Unused event data.</param>
    private async void FormatComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_suppressSelectionChanged)
        {
            return;
        }

        await RestartAsync(reloadFormats: false);
    }

    /// <summary>
    ///  Switches the view between letterboxed and unscaled 1:1 rendering.
    /// </summary>
    /// <param name="sender">The fit-frame toolbar button.</param>
    /// <param name="e">Unused event data.</param>
    private void KeepAspectRatioButton_CheckedChanged(object? sender, EventArgs e)
        => _cameraView.KeepAspectRatio = _keepAspectRatioButton.Checked;

    /// <summary>
    ///  Re-enumerates the cameras.
    /// </summary>
    /// <param name="sender">The refresh button.</param>
    /// <param name="e">Unused event data.</param>
    private async void RefreshButton_Click(object? sender, EventArgs e)
        => await ReloadDevicesAsync();

    /// <summary>
    ///  Surfaces a capture failure, marshalling to the UI thread when necessary.
    /// </summary>
    /// <param name="sender">The capture engine.</param>
    /// <param name="e">The error data.</param>
    private void Capture_Error(object? sender, CameraErrorEventArgs e)
    {
        if (_closing || IsDisposed || !IsHandleCreated)
        {
            return;
        }

        if (InvokeRequired)
        {
            _ = InvokeAsync(() => ShowError(e.Message));
            return;
        }

        ShowError(e.Message);
    }

    /// <summary>
    ///  Shows an error both inside the view and as a message box.
    /// </summary>
    /// <param name="message">The message to show.</param>
    private void ShowError(string message)
    {
        if (_closing || IsDisposed)
        {
            return;
        }

        _cameraView.ClearFrame();
        _cameraView.StatusText = message;

        MessageBox.Show(
            this,
            message,
            Text,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    /// <summary>
    ///  Disables capture settings without blocking the presentation controls.
    /// </summary>
    /// <param name="busy"><see langword="true"/> while an operation is running.</param>
    private void SetBusy(bool busy)
    {
        if (_closing && !busy)
        {
            return;
        }

        _cameraComboBox.Enabled = !busy;
        _formatComboBox.Enabled = !busy;
        _refreshButton.Enabled = !busy;
        UseWaitCursor = busy;
    }

    private void FullScreenButton_Click(object? sender, EventArgs e)
        => _kioskModeManager.ToggleFullScreen();

    private void KioskModeManager_FullScreenChanged(object? sender, EventArgs e)
    {
        if (_closing)
        {
            return;
        }

        bool fullScreen = _kioskModeManager.FullScreen;
        _fullScreenButton.Checked = fullScreen;
        _fullScreenButton.Text = fullScreen ? "Exit f&ullscreen" : "F&ullscreen";
        _fullScreenButton.ToolTipText = fullScreen
            ? "Return to the window (F11 or Esc)"
            : "Enter fullscreen (F11); press Esc to return to the window";
        UpdateToolbarImages();
    }

    private void UpdateToolbarImages()
    {
        _imageFactory.SetImage(_cameraLabel, CameraSymbol.Camera, DeviceDpi);
        _imageFactory.SetImage(_formatLabel, CameraSymbol.Resolution, DeviceDpi);
        _imageFactory.SetImage(_keepAspectRatioButton, CameraSymbol.FitFrame, DeviceDpi);
        _imageFactory.SetImage(_refreshButton, CameraSymbol.Refresh, DeviceDpi);
        _imageFactory.SetImage(_fullScreenButton,
            _kioskModeManager.FullScreen ? CameraSymbol.BackToWindow : CameraSymbol.FullScreen,
            DeviceDpi);
    }
}
