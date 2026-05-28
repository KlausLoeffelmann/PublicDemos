using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WingetPackageEditor.Core.ViewModels;

namespace Winget_Package_Editor;

internal sealed class TreeViewBinder : IDisposable
{
    private readonly TreeView _treeView;
    private readonly ObservableCollection<NavigationNodeViewModel> _roots;
    private bool _updatingTree;

    public TreeViewBinder(TreeView treeView, ObservableCollection<NavigationNodeViewModel> roots)
    {
        _treeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
        _roots = roots ?? throw new ArgumentNullException(nameof(roots));
        _roots.CollectionChanged += OnRootsChanged;
        _treeView.AfterSelect += OnAfterSelect;
        Rebuild();
    }

    public event EventHandler<NavigationNodeViewModel?>? SelectedNodeChanged;

    public void SelectNode(NavigationNodeViewModel? selectedNode)
    {
        if (selectedNode is null)
        {
            return;
        }

        TreeNode? node = FindNode(_treeView.Nodes, selectedNode);
        if (node is null)
        {
            return;
        }

        _updatingTree = true;
        try
        {
            _treeView.SelectedNode = node;
            node.EnsureVisible();
        }
        finally
        {
            _updatingTree = false;
        }
    }

    public void Dispose()
    {
        _roots.CollectionChanged -= OnRootsChanged;
        _treeView.AfterSelect -= OnAfterSelect;
    }

    private void OnRootsChanged(object? sender, NotifyCollectionChangedEventArgs e) => Rebuild();

    private void Rebuild()
    {
        _updatingTree = true;
        try
        {
            _treeView.BeginUpdate();
            _treeView.Nodes.Clear();
            foreach (NavigationNodeViewModel root in _roots)
            {
                _treeView.Nodes.Add(CreateNode(root));
            }

            _treeView.ExpandAll();
            if (_treeView.Nodes.Count > 0 && _treeView.SelectedNode is null)
            {
                _treeView.SelectedNode = _treeView.Nodes[0];
            }
        }
        finally
        {
            _treeView.EndUpdate();
            _updatingTree = false;
        }
    }

    private static TreeNode CreateNode(NavigationNodeViewModel viewModel)
    {
        TreeNode node = new(viewModel.Text)
        {
            Tag = viewModel
        };

        foreach (NavigationNodeViewModel child in viewModel.Children)
        {
            node.Nodes.Add(CreateNode(child));
        }

        return node;
    }

    private void OnAfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (_updatingTree)
        {
            return;
        }

        SelectedNodeChanged?.Invoke(this, e.Node?.Tag as NavigationNodeViewModel);
    }

    private static TreeNode? FindNode(TreeNodeCollection nodes, NavigationNodeViewModel selectedNode)
    {
        foreach (TreeNode node in nodes)
        {
            if (ReferenceEquals(node.Tag, selectedNode))
            {
                return node;
            }

            TreeNode? childNode = FindNode(node.Nodes, selectedNode);
            if (childNode is not null)
            {
                return childNode;
            }
        }

        return null;
    }
}
