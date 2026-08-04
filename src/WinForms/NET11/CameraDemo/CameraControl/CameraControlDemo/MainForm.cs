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

    /// <summary>
    ///  Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();

        _capture = new CameraCapture(_cameraView);
        _capture.Error += Capture_Error;

        _cameraView.KeepAspectRatio = _keepAspectRatioCheckBox.Checked;
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
    ///  Closes the window when Esc is pressed.
    /// </summary>
    /// <param name="msg">The window message.</param>
    /// <param name="keyData">The pressed keys.</param>
    /// <returns><see langword="true"/> when the key was handled.</returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            Close();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    ///  Tears the capture session down before the window goes away.
    /// </summary>
    /// <param name="e">The closing event data.</param>
    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_closing)
        {
            _closing = true;
            e.Cancel = true;

            await _capture.DisposeAsync();

            Close();
            return;
        }

        base.OnFormClosing(e);
    }

    /// <summary>
    ///  Re-enumerates the cameras and restarts the preview.
    /// </summary>
    /// <returns>A task that completes once the preview is running.</returns>
    private async Task ReloadDevicesAsync()
    {
        SetBusy(true);

        try
        {
            IReadOnlyList<CameraDevice> devices = await CameraCapture.GetDevicesAsync();

            _suppressSelectionChanged = true;

            try
            {
                _cameraComboBox.DataSource = null;
                _cameraComboBox.DataSource = devices.ToList();
                _cameraComboBox.SelectedIndex = devices.Count > 0 ? 0 : -1;

                _formatComboBox.DataSource = null;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }

            if (devices.Count == 0)
            {
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

            if (!started)
            {
                return;
            }

            if (reloadFormats)
            {
                _suppressSelectionChanged = true;

                try
                {
                    _formatComboBox.DataSource = null;
                    _formatComboBox.DataSource = _capture.SupportedFormats.ToList();
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
    /// <param name="sender">The check box.</param>
    /// <param name="e">Unused event data.</param>
    private void KeepAspectRatioCheckBox_CheckedChanged(object? sender, EventArgs e)
        => _cameraView.KeepAspectRatio = _keepAspectRatioCheckBox.Checked;

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
    ///  Disables the control bar while the camera is being (re-)started.
    /// </summary>
    /// <param name="busy"><see langword="true"/> while an operation is running.</param>
    private void SetBusy(bool busy)
    {
        _controlBar.Enabled = !busy;
        UseWaitCursor = busy;
    }

    private void button1_Click(object sender, EventArgs e)
    {
        kioskModeComponent1.ToggleFullScreen();
    }
}
