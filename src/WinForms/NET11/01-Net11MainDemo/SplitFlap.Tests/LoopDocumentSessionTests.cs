using DrumMachine.Demo.Documents;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Checks semantic dirty tracking, bounded Undo/Redo, gesture grouping, and asynchronous save snapshots.
/// </summary>
public sealed class LoopDocumentSessionTests
{
    /// <summary>
    ///  Starts an untitled blank loop clean while retaining the absent path needed to enable its first Save.
    /// </summary>
    [Fact]
    public void Session_StartsWithACleanUntitledBaseline()
    {
        LoopDocument initial = LoopDocument.CreateEmpty(4);
        LoopDocumentSession session = new(initial);
        Assert.Same(initial, session.Current);
        Assert.Null(session.FilePath);
        Assert.False(session.IsDirty);
        Assert.False(session.CanUndo);
        Assert.False(session.CanRedo);
        Assert.Null(session.UndoDescription);
        Assert.Null(session.RedoDescription);
        Assert.False(session.Undo());
        Assert.False(session.Redo());

        LoopDocument equivalent = new(new PercussionScore(4, []),
            percussionVolumes: new Dictionary<Cr78Instrument, int>(initial.PercussionVolumes));
        Assert.False(session.Apply(equivalent, "No musical change"));
        Assert.False(session.IsDirty);
        Assert.False(session.CanUndo);
    }

    /// <summary>
    ///  Makes every musical value undoable, including remembered metallic amounts while disabled.
    /// </summary>
    [Theory]
    [InlineData("Score")]
    [InlineData("Tempo")]
    [InlineData("Master")]
    [InlineData("Percussion")]
    [InlineData("Loop")]
    [InlineData("Metallic enable")]
    [InlineData("Metallic amount")]
    public void Session_UndoesAndRedoesEachMusicalField(string description)
    {
        LoopDocument initial = LoopDocument.CreateEmpty(4);
        LoopDocumentSession session = new(initial);
        LoopDocument edited = description switch
        {
            "Score" => initial.WithScore(new PercussionScore(4, [new(2, 14, Cr78Instrument.Guiro, 0.37f, 23)])),
            "Tempo" => initial.WithTempo(137),
            "Master" => initial.WithMasterVolume(0),
            "Percussion" => initial.WithInstrumentVolume(Cr78Instrument.Cowbell, 0),
            "Loop" => initial.WithLoop(false),
            "Metallic enable" => initial.WithMetallic(true, 0),
            "Metallic amount" => initial.WithMetallic(false, 73),
            _ => throw new ArgumentException(nameof(description))
        };

        Assert.True(session.Apply(edited, description));
        Assert.Equal(edited, session.Current);
        Assert.True(session.IsDirty);
        Assert.True(session.CanUndo);
        Assert.Equal(description, session.UndoDescription);
        Assert.False(session.CanRedo);

        Assert.True(session.Undo());
        Assert.Equal(initial, session.Current);
        Assert.False(session.IsDirty);
        Assert.Equal(description, session.RedoDescription);
        Assert.True(session.Redo());
        Assert.Equal(edited, session.Current);
        Assert.True(session.IsDirty);
        Assert.False(session.CanRedo);
    }

    /// <summary>
    ///  Tracks clean state by contents after Save and keeps Save As paths out of Undo history.
    /// </summary>
    [Fact]
    public void Save_PreservesHistoryAndChangesTheSemanticCleanPoint()
    {
        using LoopTestFiles files = new();
        LoopDocument initial = LoopDocument.CreateEmpty(2);
        LoopDocumentSession session = new(initial, files.File("opened.drumloop.json"));
        session.Apply(initial.WithTempo(120), "Tempo");
        LoopDocument saved = session.Current;
        string savedAs = files.File("saved-as.drumloop.json");
        session.MarkSaved(saved, savedAs);

        Assert.False(session.IsDirty);
        Assert.True(session.CanUndo);
        Assert.True(session.Undo());
        Assert.Equal(initial, session.Current);
        Assert.True(session.IsDirty);
        Assert.Equal(savedAs, session.FilePath);
        Assert.True(session.Redo());
        Assert.Equal(saved, session.Current);
        Assert.False(session.IsDirty);
        Assert.Equal(savedAs, session.FilePath);

        session.Apply(session.Current.WithLoop(false), "Loop");
        Assert.True(session.IsDirty);
        Assert.True(session.Undo());
        Assert.False(session.IsDirty);
    }

    /// <summary>
    ///  Marks only the snapshot actually passed to asynchronous I/O as saved, even after a newer editor change.
    /// </summary>
    [Fact]
    public async Task Save_WhileEditingRetainsTheNewerDirtyStateAndUndoBaseline()
    {
        using LoopTestFiles files = new();
        string path = files.File("saved.drumloop.json");
        LoopDocumentSession session = new(LoopDocument.CreateEmpty(4));
        session.Apply(session.Current.WithTempo(120), "Tempo");
        LoopDocument writtenSnapshot = session.Current;
        Task save = LoopDocumentStore.SaveAsync(writtenSnapshot, path, TestContext.Current.CancellationToken);
        session.Apply(session.Current.WithInstrumentVolume(Cr78Instrument.Guiro, 0), "Guiro volume");
        await save;
        session.MarkSaved(writtenSnapshot, path);

        Assert.Equal(writtenSnapshot, await LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken));
        Assert.Equal(0, session.Current.PercussionVolumes[Cr78Instrument.Guiro]);
        Assert.Equal(100, writtenSnapshot.PercussionVolumes[Cr78Instrument.Guiro]);
        Assert.True(session.IsDirty);
        Assert.Equal(path, session.FilePath);
        Assert.True(session.Undo());
        Assert.Equal(writtenSnapshot, session.Current);
        Assert.False(session.IsDirty);
        Assert.True(session.Undo());
        Assert.Equal(92, session.Current.TempoBpm);
        Assert.True(session.IsDirty);
        Assert.True(session.Redo());
        Assert.False(session.IsDirty);
        Assert.True(session.Redo());
        Assert.True(session.IsDirty);
    }

    /// <summary>
    ///  Previews every slider change live but records one action with the gesture's own description.
    /// </summary>
    [Fact]
    public void Gesture_GroupsPreviewsAndUndoCommitsItAutomatically()
    {
        LoopDocument initial = LoopDocument.CreateEmpty(1);
        LoopDocumentSession session = new(initial);
        session.BeginGesture("Master volume");
        for (int percent = 64; percent >= 0; percent--)
        {
            Assert.True(session.Apply(session.Current.WithMasterVolume(percent), "Individual preview"));
            Assert.Equal(percent, session.Current.MasterVolumePercent);
        }

        Assert.True(session.IsDirty);
        Assert.True(session.CanUndo);
        Assert.Equal("Master volume", session.UndoDescription);
        Assert.True(session.Undo());
        Assert.Equal(initial, session.Current);
        Assert.False(session.CanUndo);
        Assert.False(session.IsDirty);
        Assert.Equal("Master volume", session.RedoDescription);
        Assert.True(session.Redo());
        Assert.Equal(0, session.Current.MasterVolumePercent);
        Assert.True(session.Undo());
        Assert.False(session.Undo());
    }

    /// <summary>
    ///  Treats a gesture's return to its original value as a no-op without destroying an existing redo branch.
    /// </summary>
    [Fact]
    public void Gesture_WithNoNetChangeAddsNoActionOrDirtyState()
    {
        LoopDocument initial = LoopDocument.CreateEmpty(1);
        LoopDocumentSession session = new(initial);
        session.Apply(initial.WithTempo(120), "Tempo");
        session.Undo();
        Assert.True(session.CanRedo);

        session.BeginGesture("Cancelled volume adjustment");
        session.Apply(session.Current.WithMasterVolume(0), "Preview");
        Assert.False(session.CanRedo);
        Assert.Null(session.RedoDescription);
        session.Apply(initial, "Return to original value");
        session.CommitGesture();
        session.CommitGesture();

        Assert.False(session.IsDirty);
        Assert.False(session.CanUndo);
        Assert.True(session.CanRedo);
        Assert.Equal("Tempo", session.RedoDescription);
        Assert.True(session.Redo());
        Assert.Equal(120, session.Current.TempoBpm);
    }

    /// <summary>
    ///  Commits a previous gesture before a second begins and keeps the two fader targets independently undoable.
    /// </summary>
    [Fact]
    public void Gesture_StartingAnotherGestureFinishesThePreviousAction()
    {
        LoopDocumentSession session = new(LoopDocument.CreateEmpty(1));
        session.BeginGesture("Tempo");
        session.Apply(session.Current.WithTempo(100), "Preview");
        session.BeginGesture("Master");
        session.Apply(session.Current.WithMasterVolume(20), "Preview");
        session.CommitGesture();
        session.CommitGesture();

        Assert.Equal("Master", session.UndoDescription);
        Assert.True(session.Undo());
        Assert.Equal(100, session.Current.TempoBpm);
        Assert.Equal(65, session.Current.MasterVolumePercent);
        Assert.Equal("Tempo", session.UndoDescription);
        Assert.True(session.Undo());
        Assert.Equal(92, session.Current.TempoBpm);
        Assert.False(session.CanUndo);
    }

    /// <summary>
    ///  Invalidates an undone branch on a new committed edit, including a gesture committed by Redo itself.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NewEdit_ClearsRedo(bool gesture)
    {
        LoopDocumentSession session = new(LoopDocument.CreateEmpty(1));
        session.Apply(session.Current.WithTempo(120), "Tempo");
        session.Undo();
        if (gesture)
        {
            session.BeginGesture("Loop");
        }

        session.Apply(session.Current.WithLoop(false), "Loop");
        Assert.False(session.CanRedo);
        Assert.False(session.Redo());
        Assert.Equal(92, session.Current.TempoBpm);
        Assert.False(session.Current.Loop);
        Assert.Equal("Loop", session.UndoDescription);
        Assert.True(session.Undo());
        Assert.False(session.IsDirty);
        Assert.Equal("Loop", session.RedoDescription);
    }

    /// <summary>
    ///  Discards the previous document's pending gesture, saved point, and both history branches on New/Open.
    /// </summary>
    [Fact]
    public void Replace_StartsAFreshCleanDocument()
    {
        using LoopTestFiles files = new();
        LoopDocumentSession session = new(LoopDocument.CreateEmpty(1), files.File("old.drumloop.json"));
        session.Apply(session.Current.WithTempo(120), "Tempo");
        session.Apply(session.Current.WithLoop(false), "Loop");
        session.Undo();
        session.BeginGesture("Master");
        session.Apply(session.Current.WithMasterVolume(0), "Preview");
        LoopDocument replacement = new(new PercussionScore(4, [new(3, 15, Cr78Instrument.Guiro, 0.22f, 17)]));
        string path = files.File("new.drumloop.json");

        session.Replace(replacement, path);

        Assert.Same(replacement, session.Current);
        Assert.Equal(path, session.FilePath);
        Assert.False(session.IsDirty);
        Assert.False(session.CanUndo);
        Assert.False(session.CanRedo);
        Assert.Null(session.UndoDescription);
        Assert.Null(session.RedoDescription);
        session.CommitGesture();
        Assert.False(session.CanUndo);

        session.Replace(LoopDocument.CreateEmpty(2), null);
        Assert.Null(session.FilePath);
        Assert.False(session.IsDirty);
    }

    /// <summary>
    ///  Keeps at most one hundred musical actions while retaining their full redo sequence.
    /// </summary>
    [Fact]
    public void History_IsBoundedToOneHundredActions()
    {
        LoopDocumentSession session = new(LoopDocument.CreateEmpty(1));
        for (int index = 0; index < 121; index++)
        {
            session.Apply(session.Current.WithTempo(40 + index), $"Tempo {index}");
        }

        for (int index = 0; index < 100; index++)
        {
            Assert.True(session.Undo());
        }

        Assert.Equal(60, session.Current.TempoBpm);
        Assert.False(session.CanUndo);
        Assert.False(session.Undo());
        for (int index = 0; index < 100; index++)
        {
            Assert.True(session.Redo());
        }

        Assert.Equal(160, session.Current.TempoBpm);
        Assert.False(session.CanRedo);
        Assert.False(session.Redo());
    }

    /// <summary>
    ///  Keeps save completion independent of a newer live gesture and recognizes a separately cloned saved value.
    /// </summary>
    [Fact]
    public void MarkSaved_DoesNotCommitOrLoseANewerGesture()
    {
        using LoopTestFiles files = new();
        LoopDocumentSession session = new(LoopDocument.CreateEmpty(1));
        LoopDocument written = session.Current;
        session.BeginGesture("Metallic amount");
        session.Apply(session.Current.WithMetallic(false, 37), "Preview");
        session.MarkSaved(new LoopDocument(new PercussionScore(1, [])), files.File("saved.drumloop.json"));
        Assert.True(session.IsDirty);
        Assert.Equal("Metallic amount", session.UndoDescription);
        session.Apply(session.Current.WithMetallic(false, 73), "Preview");
        session.CommitGesture();
        Assert.True(session.Undo());
        Assert.Equal(written, session.Current);
        Assert.False(session.IsDirty);
        Assert.False(session.CanUndo);
    }

    /// <summary>
    ///  Rejects bad command inputs before altering the current snapshot, path, or pending action.
    /// </summary>
    [Fact]
    public void Session_InvalidArgumentsDoNotPartiallyMutateState()
    {
        using LoopTestFiles files = new();
        LoopDocumentSession session = new(LoopDocument.CreateEmpty(1), files.File("original.drumloop.json"));
        session.Apply(session.Current.WithTempo(120), "Tempo");
        LoopDocument current = session.Current;
        string? path = session.FilePath;

        Assert.Throws<ArgumentNullException>(() => session.Apply(null!, "Invalid"));
        Assert.Throws<ArgumentException>(() => session.Apply(current.WithTempo(121), ""));
        Assert.Throws<ArgumentException>(() => session.BeginGesture(" "));
        Assert.Throws<ArgumentNullException>(() => session.Replace(null!, null));
        Assert.Throws<ArgumentException>(() => session.Replace(LoopDocument.CreateEmpty(2), ""));
        Assert.Throws<ArgumentException>(() => session.MarkSaved(current, " "));
        Assert.Same(current, session.Current);
        Assert.Equal(path, session.FilePath);
        Assert.True(session.IsDirty);
        Assert.Equal("Tempo", session.UndoDescription);
    }
}
