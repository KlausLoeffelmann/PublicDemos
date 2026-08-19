namespace WarpClock.App;

public partial class DisplayOptionsView : UserControl
{
    public DisplayOptionsView()
    {
        InitializeComponent();

        AutoScroll = true;
        AutoScrollMinSize = Size;
        _customTextTextBox.AccessibleName = "Custom ticker text";
        _sourcesListView.AccessibleName = "Ticker source order";
        _moveUpButton.AccessibleName = "Move ticker source up";
        _moveDownButton.AccessibleName = "Move ticker source down";
        UpdateTickerState();
        UpdateMoveButtons();
    }

    public void LoadFrom(DisplayOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _tickerEnabledCheckBox.Checked = options.TickerEnabled;
        _customTextTextBox.Text = options.CustomTickerMessage;
        _showThemeTickerVisualCheckBox.Checked = options.ShowThemeTickerVisual;
        _showFractionSecondVisualCheckBox.Checked = options.ShowFractionSecondVisual;

        _sourcesListView.BeginUpdate();
        _sourcesListView.Items.Clear();
        foreach (TickerSourceEditorItem item in OptionsDialogModelMapper.CreateTickerItems(options))
        {
            var listViewItem = new ListViewItem(item.DisplayName)
            {
                Checked = item.Enabled,
                Tag = item,
            };
            _sourcesListView.Items.Add(listViewItem);
        }

        if (_sourcesListView.Items.Count > 0)
        {
            _sourcesListView.Items[0].Selected = true;
        }

        _sourcesListView.EndUpdate();
        UpdateTickerState();
        UpdateMoveButtons();
    }

    public DisplayOptions CreateOptions()
    {
        List<TickerSourceEditorItem> items = [];
        foreach (ListViewItem item in _sourcesListView.Items)
        {
            if (item.Tag is TickerSourceEditorItem editorItem)
            {
                editorItem.Enabled = item.Checked;
                items.Add(editorItem);
            }
        }

        return OptionsDialogModelMapper.CreateDisplayOptions(
            _tickerEnabledCheckBox.Checked,
            _customTextTextBox.Text,
            items,
            _showThemeTickerVisualCheckBox.Checked,
            _showFractionSecondVisualCheckBox.Checked);
    }

    private void OnTickerEnabledCheckedChanged(object? sender, EventArgs e)
        => UpdateTickerState();

    private void OnSourcesListViewSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateMoveButtons();

    private void OnSourcesListViewItemChecked(object? sender, ItemCheckedEventArgs e)
    {
        if (e.Item.Tag is TickerSourceEditorItem editorItem)
        {
            editorItem.Enabled = e.Item.Checked;
        }
    }

    private void OnMoveUpButtonClick(object? sender, EventArgs e)
        => MoveSelectedItem(-1);

    private void OnMoveDownButtonClick(object? sender, EventArgs e)
        => MoveSelectedItem(1);

    private void MoveSelectedItem(int delta)
    {
        if (_sourcesListView.SelectedIndices.Count == 0)
        {
            return;
        }

        int currentIndex = _sourcesListView.SelectedIndices[0];
        int nextIndex = currentIndex + delta;
        if (nextIndex < 0 || nextIndex >= _sourcesListView.Items.Count)
        {
            return;
        }

        ListViewItem currentItem = _sourcesListView.Items[currentIndex];
        _sourcesListView.Items.RemoveAt(currentIndex);
        _sourcesListView.Items.Insert(nextIndex, currentItem);
        currentItem.Selected = true;
        currentItem.Focused = true;
        UpdateMoveButtons();
    }

    private void UpdateTickerState()
    {
        bool enabled = _tickerEnabledCheckBox.Checked;
        _customTextLabel.Enabled = enabled;
        _customTextTextBox.Enabled = enabled;
        _sourcesLabel.Enabled = enabled;
        _sourcesListView.Enabled = enabled;
        _moveUpButton.Enabled = enabled && _sourcesListView.SelectedIndices.Count > 0 && _sourcesListView.SelectedIndices[0] > 0;
        _moveDownButton.Enabled = enabled
            && _sourcesListView.SelectedIndices.Count > 0
            && _sourcesListView.SelectedIndices[0] < _sourcesListView.Items.Count - 1;
    }

    private void UpdateMoveButtons()
        => UpdateTickerState();
}
