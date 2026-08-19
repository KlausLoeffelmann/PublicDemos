using WarpToolkit.WinForms.Containers;

namespace WarpClock.App;

public partial class OptionsDialog : Form
{
    private readonly WarpClockOptions _sourceOptions;
    private readonly HandsOptionsView _handsView;
    private readonly TimeZonesOptionsView _timeZonesView;
    private readonly DisplayOptionsView _displayView;
    private readonly FoldersOptionsView _foldersView;
    private bool _sizeInitialized;

    public OptionsDialog(WarpClockOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _sourceOptions = options.Clone();
        _sourceOptions.Normalize();

        InitializeComponent();

        _handsView = new HandsOptionsView();
        _timeZonesView = new TimeZonesOptionsView();
        _displayView = new DisplayOptionsView();
        _foldersView = new FoldersOptionsView();

        AddTabs();
        LoadFromOptions(_sourceOptions);
        EditedOptions = _sourceOptions.Clone();
    }

    public WarpClockOptions EditedOptions { get; private set; }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_sizeInitialized)
        {
            return;
        }

        _sizeInitialized = true;

        Size preferred = ComputePreferredClientSize(_tabs);
        Rectangle workingArea = Screen.FromControl(this).WorkingArea;
        Size cap = new(
            Math.Max(MinimumSize.Width, (int)(workingArea.Width * 0.75)),
            Math.Max(MinimumSize.Height, (int)(workingArea.Height * 0.75)));

        ClientSize = new Size(
            Math.Min(preferred.Width, cap.Width),
            Math.Min(preferred.Height, cap.Height));

        Size availableTabSize = new(
            Math.Max(0, _tabs.ClientSize.Width),
            Math.Max(0, _tabs.ClientSize.Height));
        EnsureTabsCanScrollIfClipped(_tabs, availableTabSize);
    }

    private void AddTabs()
    {
        _tabs.AddTab("Hands", _handsView);
        _tabs.AddTab("Timezones", _timeZonesView);
        _tabs.AddTab("Display", _displayView);
        _tabs.AddTab("Folders", _foldersView);
    }

    private void LoadFromOptions(WarpClockOptions options)
    {
        _handsView.LoadFrom(options.Hands);
        _timeZonesView.LoadFrom(options.TimeZones);
        _displayView.LoadFrom(options.Display);
        _foldersView.LoadFrom(options.Folders);
    }

    private void OnOkClick(object? sender, EventArgs e)
    {
        if (!_timeZonesView.TryCreateOptions(out TimeZoneOptions? timeZoneOptions, out string? validationMessage))
        {
            AppMessageDialog.ShowMessage(
                this,
                "WarpClock Options",
                "Options validation failed",
                validationMessage ?? "Review the highlighted values and try again.");
            return;
        }

        WarpClockOptions edited = _sourceOptions.Clone();
        edited.Hands = _handsView.CreateOptions();
        edited.TimeZones = timeZoneOptions;
        edited.Display = _displayView.CreateOptions();
        edited.Folders = _foldersView.CreateOptions();
        edited.Normalize();

        EditedOptions = edited;
        DialogResult = DialogResult.OK;
        Close();
    }

    private static Size ComputePreferredClientSize(FluentTabControl tabs)
    {
        int width = 0;
        int height = 0;

        foreach (Panel page in tabs.Tabs)
        {
            if (page.Controls.Count == 0)
            {
                continue;
            }

            Control content = page.Controls[0];
            Size naturalSize = content.Size;
            width = Math.Max(width, naturalSize.Width);
            height = Math.Max(height, naturalSize.Height);
        }

        return new Size(width + 96, height + 140);
    }

    private static void EnsureTabsCanScrollIfClipped(FluentTabControl tabs, Size availableSize)
    {
        foreach (Panel page in tabs.Tabs)
        {
            if (page.Controls.Count == 0 || page.Controls[0] is not UserControl view)
            {
                continue;
            }

            bool clipped = view.Size.Width > availableSize.Width || view.Size.Height > availableSize.Height;
            if (clipped)
            {
                view.AutoScroll = true;
                view.AutoScrollMinSize = view.Size;
            }
        }
    }
}
