using System.Collections.ObjectModel;

namespace WingetPackageEditor.Core.ViewModels;

public sealed class NavigationNodeViewModel
{
    public NavigationNodeViewModel(string text, NavigationNodeKind kind, object value)
    {
        Text = text;
        Kind = kind;
        Value = value;
    }

    public string Text { get; }

    public NavigationNodeKind Kind { get; }

    public object Value { get; }

    public ObservableCollection<NavigationNodeViewModel> Children { get; } = [];

    public override string ToString() => Text;
}

public enum NavigationNodeKind
{
    Package,
    App,
    Extension
}
