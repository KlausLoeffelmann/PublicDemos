using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Winget_Package_Editor;

internal sealed class ObservableBindingList<T> : BindingList<T>, IDisposable
{
    private ObservableCollection<T>? _source;

    public ObservableBindingList(ObservableCollection<T> source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        foreach (T item in source)
        {
            Add(item);
        }

        source.CollectionChanged += OnCollectionChanged;
    }

    public void Dispose()
    {
        if (_source is not null)
        {
            _source.CollectionChanged -= OnCollectionChanged;
            _source = null;
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    foreach (T item in e.NewItems)
                    {
                        Add(item);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is not null)
                {
                    foreach (T item in e.OldItems)
                    {
                        Remove(item);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
                Reload();
                break;
        }
    }

    private void Reload()
    {
        RaiseListChangedEvents = false;
        try
        {
            Clear();
            if (_source is null)
            {
                return;
            }

            foreach (T item in _source)
            {
                Add(item);
            }
        }
        finally
        {
            RaiseListChangedEvents = true;
            ResetBindings();
        }
    }
}
