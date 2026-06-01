using System.Collections.ObjectModel;

namespace WingetPackageEditor.Core.ViewModels;

public sealed class NavigationNodeViewModel
{
    public NavigationNodeViewModel(string text, NavigationNodeKind kind, object value, string key)
    {
        Text = text;
        Kind = kind;
        Value = value;
        Key = key;
    }

    public string Text { get; }

    public NavigationNodeKind Kind { get; }

    public object Value { get; }

    public string Key { get; }

    public ObservableCollection<NavigationNodeViewModel> Children { get; } = [];

    public override string ToString() => Text;
}

public enum NavigationNodeKind
{
    Package,
    App,
    Extension,
    VisualStudioRoot,
    VisualStudioVersion,
    VisualStudioSkuCombo,
    VisualStudioInstance
}
