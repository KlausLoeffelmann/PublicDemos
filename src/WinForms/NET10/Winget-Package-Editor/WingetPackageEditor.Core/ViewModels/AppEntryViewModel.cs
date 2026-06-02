using CommunityToolkit.Mvvm.ComponentModel;
using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.ViewModels;

public sealed partial class AppEntryViewModel : ObservableObject
{
    public AppEntryViewModel(AppEntry model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public AppEntry Model { get; }

    public string EntryType => Model switch
    {
        VisualStudioEntry => "Visual Studio",
        VSCodeEntry => "VS Code",
        GenericAppEntry => "Generic",
        _ => Model.GetType().Name
    };

    public string Id
    {
        get => Model.Id;
        set => SetProperty(Model.Id, value, Model, static (model, newValue) => model.Id = newValue);
    }

    public string DisplayName
    {
        get => Model.DisplayName;
        set
        {
            if (SetProperty(Model.DisplayName, value, Model, static (model, newValue) => model.DisplayName = newValue))
            {
                OnPropertyChanged(nameof(TreeText));
            }
        }
    }

    public AppAction Action
    {
        get => Model.Action;
        set => SetProperty(Model.Action, value, Model, static (model, newValue) => model.Action = newValue);
    }

    public AppSource Source
    {
        get => Model.Source;
        set => SetProperty(Model.Source, value, Model, static (model, newValue) => model.Source = newValue);
    }

    public string? Version
    {
        get => Model.Version;
        set => SetProperty(Model.Version, value, Model, static (model, newValue) => model.Version = newValue);
    }

    public AppScope Scope
    {
        get => Model.Scope;
        set => SetProperty(Model.Scope, value, Model, static (model, newValue) => model.Scope = newValue);
    }

    public bool AllowPrerelease
    {
        get => Model.AllowPrerelease;
        set => SetProperty(Model.AllowPrerelease, value, Model, static (model, newValue) => model.AllowPrerelease = newValue);
    }

    public string TreeText => string.IsNullOrWhiteSpace(DisplayName) ? Id : DisplayName;

    public string ExtensionsSummary => Model switch
    {
        VSCodeEntry code => $"{code.Extensions.Count} VS Code extension(s)",
        VisualStudioEntry visualStudio => $"{visualStudio.Extensions.Count} VSIX extension(s)",
        _ => ""
    };
}
