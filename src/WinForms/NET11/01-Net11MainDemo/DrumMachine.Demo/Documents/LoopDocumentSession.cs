namespace DrumMachine.Demo.Documents;

/// <summary>
///  Owns editor snapshots, a saved baseline, and bounded musical Undo/Redo independently of the UI.
/// </summary>
internal sealed class LoopDocumentSession
{
    private const int HistoryLimit = 100;
    private readonly List<HistoryEntry> _undo = [];
    private readonly List<HistoryEntry> _redo = [];
    private LoopDocument _saved;
    private LoopDocument? _gestureStart;
    private string? _gestureDescription;

    /// <summary>
    ///  Starts a clean session, optionally associated with an already opened file.
    /// </summary>
    public LoopDocumentSession(LoopDocument initial, string? path = null)
    {
        ArgumentNullException.ThrowIfNull(initial);
        FilePath = NormalizeOptionalPath(path);
        Current = initial;
        _saved = initial;
    }

    /// <summary>
    ///  Gets the live editor snapshot, including previews not yet committed as a history action.
    /// </summary>
    public LoopDocument Current { get; private set; }

    /// <summary>
    ///  Gets the successfully opened or saved path, which is never rewound by Undo.
    /// </summary>
    public string? FilePath { get; private set; }

    /// <summary>
    ///  Gets whether the current musical contents differ from the last successfully written snapshot.
    /// </summary>
    public bool IsDirty => !Current.ValueEquals(_saved);

    /// <summary>
    ///  Gets whether committing a pending gesture would leave an action available to undo.
    /// </summary>
    public bool CanUndo => HasGestureChange || _undo.Count != 0;

    /// <summary>
    ///  Gets whether Redo is available without discarding a new pending edit.
    /// </summary>
    public bool CanRedo => !HasGestureChange && _redo.Count != 0;

    /// <summary>
    ///  Gets the pending gesture or most recent committed action's label.
    /// </summary>
    public string? UndoDescription
        => HasGestureChange ? _gestureDescription : _undo.Count == 0 ? null : _undo[^1].Description;

    /// <summary>
    ///  Gets the next redo action's label, or null when a new edit has replaced that branch.
    /// </summary>
    public string? RedoDescription => CanRedo ? _redo[^1].Description : null;

    /// <summary>
    ///  Applies a semantic change immediately, recording one action unless a gesture is in progress.
    /// </summary>
    public bool Apply(LoopDocument document, string description)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (Current.ValueEquals(document))
        {
            return false;
        }

        if (_gestureStart is null)
        {
            AddUndo(new HistoryEntry(Current, document, description));
            _redo.Clear();
        }

        Current = document;
        return true;
    }

    /// <summary>
    ///  Finishes any previous gesture and starts a single grouped slider or keyboard action.
    /// </summary>
    public void BeginGesture(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        CommitGesture();
        _gestureStart = Current;
        _gestureDescription = description;
    }

    /// <summary>
    ///  Records the gesture's net change once; returning to its initial value creates no action.
    /// </summary>
    public void CommitGesture()
    {
        LoopDocument? before = _gestureStart;
        string? description = _gestureDescription;
        _gestureStart = null;
        _gestureDescription = null;
        if (before is not null && !before.ValueEquals(Current))
        {
            AddUndo(new HistoryEntry(before, Current, description!));
            _redo.Clear();
        }
    }

    /// <summary>
    ///  Commits any preview and restores the preceding musical snapshot without changing the file path.
    /// </summary>
    public bool Undo()
    {
        CommitGesture();
        if (_undo.Count == 0)
        {
            return false;
        }

        HistoryEntry entry = _undo[^1];
        _undo.RemoveAt(_undo.Count - 1);
        _redo.Add(entry);
        Current = entry.Before;
        return true;
    }

    /// <summary>
    ///  Commits any preview and reapplies the next action if no intervening edit replaced it.
    /// </summary>
    public bool Redo()
    {
        CommitGesture();
        if (_redo.Count == 0)
        {
            return false;
        }

        HistoryEntry entry = _redo[^1];
        _redo.RemoveAt(_redo.Count - 1);
        AddUndo(entry);
        Current = entry.After;
        return true;
    }

    /// <summary>
    ///  Establishes a clean New/Open baseline and discards the previous document's complete history.
    /// </summary>
    public void Replace(LoopDocument document, string? path)
    {
        ArgumentNullException.ThrowIfNull(document);
        string? normalizedPath = NormalizeOptionalPath(path);
        Current = document;
        _saved = document;
        FilePath = normalizedPath;
        _undo.Clear();
        _redo.Clear();
        _gestureStart = null;
        _gestureDescription = null;
    }

    /// <summary>
    ///  Marks the actually written snapshot as saved, retaining newer edits and the entire Undo history.
    /// </summary>
    public void MarkSaved(LoopDocument writtenSnapshot, string path)
    {
        ArgumentNullException.ThrowIfNull(writtenSnapshot);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string normalizedPath = Path.GetFullPath(path);
        _saved = writtenSnapshot;
        FilePath = normalizedPath;
    }

    private bool HasGestureChange => _gestureStart is not null && !_gestureStart.ValueEquals(Current);

    private void AddUndo(HistoryEntry entry)
    {
        if (_undo.Count == HistoryLimit)
        {
            _undo.RemoveAt(0);
        }

        _undo.Add(entry);
    }

    private static string? NormalizeOptionalPath(string? path)
    {
        if (path is null)
        {
            return null;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Path.GetFullPath(path);
    }

    private sealed record HistoryEntry(LoopDocument Before, LoopDocument After, string Description);
}
