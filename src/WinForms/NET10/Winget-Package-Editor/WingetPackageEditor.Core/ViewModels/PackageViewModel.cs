using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.ViewModels;

public sealed partial class PackageViewModel : ObservableObject
{
    public PackageViewModel(WingetPackage model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Apps = new ObservableCollection<AppEntryViewModel>(
            model.Apps.Select(app => new AppEntryViewModel(app)));
    }

    public WingetPackage Model { get; }

    public ObservableCollection<AppEntryViewModel> Apps { get; }

    public string Name
    {
        get => Model.Name;
        set
        {
            if (SetProperty(Model.Name, value, Model, static (model, newValue) => model.Name = newValue))
            {
                OnPropertyChanged(nameof(TreeText));
            }
        }
    }

    public string? Description
    {
        get => Model.Description;
        set => SetProperty(Model.Description, value, Model, static (model, newValue) => model.Description = newValue);
    }

    public string? Author
    {
        get => Model.Author;
        set => SetProperty(Model.Author, value, Model, static (model, newValue) => model.Author = newValue);
    }

    public string Version
    {
        get => Model.Version;
        set => SetProperty(Model.Version, value, Model, static (model, newValue) => model.Version = newValue);
    }

    public string TreeText => string.IsNullOrWhiteSpace(Name) ? "(Untitled package)" : Name;

    public AppEntryViewModel AddApp(AppEntry app)
    {
        ArgumentNullException.ThrowIfNull(app);
        Model.Apps.Add(app);
        AppEntryViewModel viewModel = new(app);
        Apps.Add(viewModel);
        return viewModel;
    }

    public bool RemoveApp(AppEntryViewModel app)
    {
        ArgumentNullException.ThrowIfNull(app);
        bool removedFromModel = Model.Apps.Remove(app.Model);
        bool removedFromView = Apps.Remove(app);
        return removedFromModel || removedFromView;
    }
}
