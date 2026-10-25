namespace DrumMachine.Demo.Controls;

/// <summary>
///  Owns one cached, current symbol bitmap per toolbar or menu item.
/// </summary>
/// <remarks>
///  Use on the UI thread. This owner never disposes the supplied factory, caller-owned
///  items, or pre-existing images. Do not share its item images with other controls.
/// </remarks>
internal sealed class ToolbarIconSet : IDisposable
{
    private readonly SymbolIconFactory _factory;
    private readonly Dictionary<ToolStripItem, OwnedIcon> _icons = new();
    private bool _disposed;

    /// <summary>
    ///  Creates an image owner using a separately owned, verified symbol-font factory.
    /// </summary>
    internal ToolbarIconSet(SymbolIconFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factory = factory;
    }

    /// <summary>
    ///  Installs or reuses an item's icon, disposing its previous owned image only after replacement.
    /// </summary>
    /// <remarks>
    ///  The logical size is independent per item; pass 16 for menus regardless of the toolbar preference.
    ///  Invoke after size, DPI, or foreground changes, not during painting.
    /// </remarks>
    internal void Apply(ToolStripItem item, ToolbarSymbol symbol, int logicalSize, int dpi, Color foreground)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(item);
        ObjectDisposedException.ThrowIf(item.IsDisposed, item);

        IconKey key = new(symbol, logicalSize, dpi, foreground.ToArgb());
        if (_icons.TryGetValue(item, out OwnedIcon? current) && current.Key == key)
        {
            item.ImageScaling = ToolStripItemImageScaling.None;
            if (!ReferenceEquals(item.Image, current.Image))
            {
                item.Image = current.Image;
            }

            return;
        }

        Bitmap replacement = _factory.Create(symbol, logicalSize, dpi, foreground);
        try
        {
            item.ImageScaling = ToolStripItemImageScaling.None;
            item.Image = replacement;
        }
        catch
        {
            if (!ReferenceEquals(item.Image, replacement))
            {
                replacement.Dispose();
            }
            else
            {
                // A synchronous layout handler can throw after the Image setter took effect.
                // Retain ownership so neither an attached image nor its predecessor leaks.
                _icons[item] = new(key, replacement);
                if (current is null)
                {
                    item.Disposed += Item_Disposed;
                }

                current?.Image.Dispose();
            }

            throw;
        }

        _icons[item] = new(key, replacement);
        if (current is null)
        {
            item.Disposed += Item_Disposed;
        }

        current?.Image.Dispose();
    }

    /// <summary>
    ///  Detaches and releases every owned bitmap, without disposing the items or factory.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        foreach ((ToolStripItem item, OwnedIcon icon) in _icons)
        {
            item.Disposed -= Item_Disposed;
            if (ReferenceEquals(item.Image, icon.Image))
            {
                item.Image = null;
            }

            icon.Image.Dispose();
        }

        _icons.Clear();
    }

    private void Item_Disposed(object? sender, EventArgs e)
    {
        if (sender is ToolStripItem item && _icons.Remove(item, out OwnedIcon? icon))
        {
            item.Disposed -= Item_Disposed;
            if (ReferenceEquals(item.Image, icon.Image))
            {
                item.Image = null;
            }

            icon.Image.Dispose();
        }
    }

    private readonly record struct IconKey(ToolbarSymbol Symbol, int LogicalSize, int Dpi, int ForegroundArgb);

    private sealed record OwnedIcon(IconKey Key, Bitmap Image);
}
