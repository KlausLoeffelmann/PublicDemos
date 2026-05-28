using System.ComponentModel;
using WingetPackageEditor.Core.ViewModels;

namespace Winget_Package_Editor;

internal sealed class GridSelectionBinder : IDisposable
{
    private readonly DataGridView _gridView;
    private readonly MainViewModel _viewModel;
    private bool _updating;

    public GridSelectionBinder(DataGridView gridView, MainViewModel viewModel)
    {
        _gridView = gridView ?? throw new ArgumentNullException(nameof(gridView));
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _gridView.SelectionChanged += OnSelectionChanged;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public void Dispose()
    {
        _gridView.SelectionChanged -= OnSelectionChanged;
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        if (_updating || _gridView.CurrentRow?.DataBoundItem is not AppEntryViewModel app)
        {
            return;
        }

        _viewModel.SelectedApp = app;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainViewModel.SelectedApp))
        {
            return;
        }

        _updating = true;
        try
        {
            foreach (DataGridViewRow row in _gridView.Rows)
            {
                if (ReferenceEquals(row.DataBoundItem, _viewModel.SelectedApp))
                {
                    row.Selected = true;
                    _gridView.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }
        finally
        {
            _updating = false;
        }
    }
}
