using System.Drawing;
using System.Windows.Forms;
using WinForms.Audio.Percussion;

namespace DrumMachine.Demo;

/// <summary>
///  Selects the application's color mode at startup rather than changing a running form's theme.
/// </summary>
internal enum AppTheme
{
    /// <summary>
    ///  Uses the classic Windows Forms color mode.
    /// </summary>
    Classic,

    /// <summary>
    ///  Requests the Windows Forms dark color mode.
    /// </summary>
    Dark,

    /// <summary>
    ///  Uses the operating system's color preference at launch.
    /// </summary>
    System
}

/// <summary>
///  Defines toolbar glyph sizes in logical pixels at the 96-DPI design baseline.
/// </summary>
internal enum ToolbarIconSize
{
    /// <summary>
    ///  Renders toolbar icons at thirty-two logical pixels.
    /// </summary>
    Small = 32,

    /// <summary>
    ///  Renders toolbar icons at forty-eight logical pixels.
    /// </summary>
    Medium = 48,

    /// <summary>
    ///  Renders toolbar icons at sixty-four logical pixels.
    /// </summary>
    Large = 64
}

/// <summary>
///  Selects a point-size increment relative to the current WinForms default font.
/// </summary>
internal enum AppFontSize
{
    /// <summary>
    ///  Uses the current WinForms standard point size without modification.
    /// </summary>
    Small,

    /// <summary>
    ///  Adds two points to the current WinForms standard.
    /// </summary>
    Normal,

    /// <summary>
    ///  Adds four points to the current WinForms standard.
    /// </summary>
    Large,

    /// <summary>
    ///  Adds six points to the current WinForms standard.
    /// </summary>
    Xxl
}

/// <summary>
///  Keeps user-interface preferences and recent paths outside musical documents and their Undo history.
/// </summary>
internal sealed record AppSettings
{
    private const int MaximumPathCharacters = 4_096;
    private IReadOnlyList<string> _recentFiles = Array.Empty<string>();
    private IReadOnlyList<TrackSelectionPreset> _trackPresets = TrackSelectionPreset.CreateEmpty();

    /// <summary>
    ///  The maximum number of successfully opened or saved loop files retained in the menu.
    /// </summary>
    public const int MaximumRecentFiles = 5;

    /// <summary>
    ///  The number of named track-selection presets retained with application preferences.
    /// </summary>
    public const int TrackPresetCount = TrackSelectionPreset.SlotCount;

    /// <summary>
    ///  Gets the color mode to apply before constructing UI on the next launch.
    /// </summary>
    public AppTheme Theme { get; init; } = AppTheme.System;

    /// <summary>
    ///  Gets the immediately applicable toolbar icon size, independently of the menu glyph size.
    /// </summary>
    public ToolbarIconSize IconSize { get; init; } = ToolbarIconSize.Small;

    /// <summary>
    ///  Gets the point-size increment applied to the current WinForms default font at startup.
    /// </summary>
    public AppFontSize FontSize { get; init; } = AppFontSize.Small;

    /// <summary>
    ///  Gets the initial folder for Open and untitled Save As without changing a named document's path.
    /// </summary>
    public string DefaultFolder { get; init; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    ///  Gets a defensive snapshot of recent paths, ordered with the latest successful operation first.
    /// </summary>
    public IReadOnlyList<string> RecentFiles
    {
        get => _recentFiles;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _recentFiles = Array.AsReadOnly(value.ToArray());
        }
    }

    /// <summary>
    ///  Gets the one- or two-bar viewport size, which never changes a score's actual length.
    /// </summary>
    public int BarsPerView { get; init; } = 1;

    /// <summary>
    ///  Gets the default folder for imported percussion recordings.
    /// </summary>
    public string RecordingsFolder { get; init; } = SpecialFolderOrDocuments(Environment.SpecialFolder.MyMusic);

    /// <summary>
    ///  Gets the default folder for portable sound-preset manifests.
    /// </summary>
    public string SoundPresetFolder { get; init; } = SpecialFolderOrDocuments(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    ///  Gets the folder of the last successful WAV import. Dialogs start here before falling back to <see cref="RecordingsFolder"/>.
    /// </summary>
    public string LastWavImportFolder { get; init; } = SpecialFolderOrDocuments(Environment.SpecialFolder.MyMusic);

    /// <summary>
    ///  Gets the normal-window bounds, maximized flag, and DPI saved with those bounds. An empty placement is not restored.
    /// </summary>
    public WindowPlacement Window { get; init; } = WindowPlacement.Unset;

    /// <summary>
    ///  Gets the spectrum/score splitter distance in 96-DPI pixels. Zero keeps the Designer distance.
    /// </summary>
    public int SpectrumSplitterDistanceDip { get; init; }

    private IReadOnlyList<StepGridColumnPreference> _stepGridColumns = [];

    /// <summary>
    ///  Gets saved score-column widths and display order. An empty list keeps the Designer layout.
    /// </summary>
    public IReadOnlyList<StepGridColumnPreference> StepGridColumns
    {
        get => _stepGridColumns;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _stepGridColumns = Array.AsReadOnly(value.ToArray());
        }
    }

    /// <summary>
    ///  Gets the eight named track-selection presets, independent of the open loop document.
    /// </summary>
    public IReadOnlyList<TrackSelectionPreset> TrackPresets
    {
        get => _trackPresets;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _trackPresets = Array.AsReadOnly(value.ToArray());
        }
    }

    /// <summary>
    ///  Moves a normalized Windows path to the front, deduplicates case-insensitively, and retains at most five.
    /// </summary>
    public AppSettings WithRecentFile(string path)
    {
        string normalized = NormalizeFilePath(path);
        List<string> recent = [normalized];
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase) { normalized };
        foreach (string previous in RecentFiles)
        {
            string fullPath = NormalizeFilePath(previous);
            if (seen.Add(fullPath))
            {
                recent.Add(fullPath);
                if (recent.Count == MaximumRecentFiles)
                {
                    break;
                }
            }
        }

        return this with { RecentFiles = recent };
    }

    /// <summary>
    ///  Removes only the specified normalized recent path without probing unrelated files or folders.
    /// </summary>
    public AppSettings WithRemovedRecentFile(string path)
    {
        string normalized = NormalizeFilePath(path);
        return this with
        {
            RecentFiles = RecentFiles
                .Select(NormalizeFilePath)
                .Where(previous => !StringComparer.OrdinalIgnoreCase.Equals(previous, normalized))
                .ToArray()
        };
    }

    /// <summary>
    ///  Replaces one preset slot with a normalized name and instrument set, leaving the other seven unchanged.
    /// </summary>
    public AppSettings WithTrackPreset(int slot, string name, IEnumerable<Cr78Instrument> instruments)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(slot, TrackSelectionPreset.SlotCount);
        TrackSelectionPreset[] presets = TrackPresets.ToArray();
        presets[slot] = new TrackSelectionPreset
        {
            Name = TrackSelectionPreset.NormalizeName(name),
            Instruments = TrackSelectionPreset.NormalizeInstruments(instruments)
        };
        return this with { TrackPresets = presets };
    }

    /// <summary>
    ///  Remembers the directory of a successful WAV import without changing the musical document.
    /// </summary>
    public AppSettings WithLastWavImportFolder(string path)
        => this with { LastWavImportFolder = NormalizePath(path) };

    /// <summary>
    ///  Stores normal bounds and whether the window was maximized. Minimized state is not persisted.
    /// </summary>
    public AppSettings WithWindowPlacement(Rectangle restoreBounds, bool maximized, int dpi)
        => this with
        {
            Window = new WindowPlacement
            {
                X = restoreBounds.X,
                Y = restoreBounds.Y,
                Width = restoreBounds.Width,
                Height = restoreBounds.Height,
                IsMaximized = maximized,
                SavedDpi = dpi
            }.Validate()
        };

    /// <summary>
    ///  Stores the spectrum splitter distance in 96-DPI pixels.
    /// </summary>
    public AppSettings WithSpectrumSplitterDistanceDip(int distanceDip)
    {
        if (distanceDip is < 40 or > 4_000)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceDip), "The splitter distance is outside the supported range.");
        }

        return this with { SpectrumSplitterDistanceDip = distanceDip };
    }

    /// <summary>
    ///  Replaces the saved score-column layout. An empty list restores the Designer layout on the next launch.
    /// </summary>
    public AppSettings WithStepGridColumns(IReadOnlyList<StepGridColumnPreference> columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        return this with { StepGridColumns = StepGridColumnPreference.Validate(columns) };
    }

    /// <summary>
    ///  Validates persisted choices and paths without requiring removable or network folders to be online.
    /// </summary>
    internal AppSettings ValidateAndNormalize()
    {
        if (!Enum.IsDefined(Theme) || !Enum.IsDefined(IconSize) || !Enum.IsDefined(FontSize))
        {
            throw new ArgumentException("The theme, font size, or toolbar icon size is unsupported.");
        }

        if (BarsPerView is not (1 or 2))
        {
            throw new ArgumentOutOfRangeException(nameof(BarsPerView), "The view must show one or two bars.");
        }

        if (string.IsNullOrWhiteSpace(DefaultFolder) || !Path.IsPathFullyQualified(DefaultFolder))
        {
            throw new ArgumentException("The default folder must be a fully qualified Windows path.");
        }

        string folder = NormalizePath(DefaultFolder);
        if (RecentFiles.Count > MaximumRecentFiles)
        {
            throw new ArgumentException("At most five recent files may be persisted.");
        }

        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        List<string> recent = [];
        foreach (string path in RecentFiles)
        {
            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
            {
                throw new ArgumentException("Recent files must use fully qualified Windows paths.");
            }

            string fullPath = NormalizeFilePath(path);
            if (!seen.Add(fullPath))
            {
                throw new ArgumentException("The recent-file list contains a duplicate Windows path.");
            }

            recent.Add(fullPath);
        }

        if (TrackPresets.Count != TrackSelectionPreset.SlotCount)
        {
            throw new ArgumentException("Exactly eight track-selection presets are required.");
        }

        TrackSelectionPreset[] presets = new TrackSelectionPreset[TrackSelectionPreset.SlotCount];
        for (int slot = 0; slot < presets.Length; slot++)
        {
            presets[slot] = TrackPresets[slot].Validate();
        }

        return this with
        {
            DefaultFolder = folder,
            RecordingsFolder = NormalizeFolder(RecordingsFolder),
            SoundPresetFolder = NormalizeFolder(SoundPresetFolder),
            LastWavImportFolder = NormalizeFolder(LastWavImportFolder),
            RecentFiles = recent,
            TrackPresets = presets,
            Window = Window.Validate(),
            SpectrumSplitterDistanceDip = ValidateSplitter(SpectrumSplitterDistanceDip),
            StepGridColumns = StepGridColumnPreference.Validate(StepGridColumns)
        };
    }

    private static int ValidateSplitter(int distanceDip)
    {
        if (distanceDip is < 0 or > 4_000)
        {
            throw new ArgumentOutOfRangeException(nameof(SpectrumSplitterDistanceDip), "The splitter distance is outside the supported range.");
        }

        return distanceDip;
    }

    internal static string SpecialFolderOrDocuments(Environment.SpecialFolder folder)
    {
        string path = Environment.GetFolderPath(folder);
        if (IsUsableDefaultFolder(path))
        {
            return path;
        }

        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return IsUsableDefaultFolder(documents) ? documents : @"C:\Users\Public\Documents";
    }

    private static bool IsUsableDefaultFolder(string path)
        => !string.IsNullOrWhiteSpace(path) && Path.IsPathFullyQualified(path) && Directory.Exists(path);

    private static string NormalizeFilePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (Path.EndsInDirectorySeparator(path))
        {
            throw new ArgumentException("A recent-file path must include a filename.", nameof(path));
        }

        string fullPath = NormalizePath(path);
        if (Path.GetFileName(fullPath).Length == 0)
        {
            throw new ArgumentException("A recent-file path must include a filename.", nameof(path));
        }

        return fullPath;
    }

    private static string NormalizeFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("Folders must be fully qualified Windows paths.", nameof(path));
        }

        return NormalizePath(path);
    }

    private static string NormalizePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (path.Length > MaximumPathCharacters || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            throw new ArgumentException("The path is invalid or exceeds the supported length.", nameof(path));
        }

        string fullPath = Path.GetFullPath(path);
        if (fullPath.Length > MaximumPathCharacters)
        {
            throw new ArgumentException("The path exceeds the supported length.", nameof(path));
        }

        string root = Path.GetPathRoot(fullPath)!;
        char[] invalidNameCharacters = Path.GetInvalidFileNameChars();
        foreach (string part in fullPath[root.Length..].Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries))
        {
            if (part.IndexOfAny(invalidNameCharacters) >= 0 || part.EndsWith(' ') || part.EndsWith('.'))
            {
                throw new ArgumentException("A path component contains invalid or ambiguous Windows filename characters.", nameof(path));
            }
        }

        return Path.TrimEndingDirectorySeparator(fullPath);
    }
}

/// <summary>
///  A named selection of score tracks, stored with application preferences rather than a loop file.
/// </summary>
internal sealed record TrackSelectionPreset
{
    /// <summary>
    ///  The number of preset slots.
    /// </summary>
    public const int SlotCount = 8;

    /// <summary>
    ///  The maximum length of a preset name after trimming.
    /// </summary>
    public const int MaximumNameLength = 40;

    private IReadOnlyList<Cr78Instrument> _instruments = [];

    /// <summary>
    ///  Gets the user-chosen name. An empty name is displayed as "Preset N".
    /// </summary>
    public string Name { get; init; } = "";

    /// <summary>
    ///  Gets the selected primary instruments, copied so a caller cannot change a saved preset.
    /// </summary>
    public IReadOnlyList<Cr78Instrument> Instruments
    {
        get => _instruments;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _instruments = Array.AsReadOnly(value.ToArray());
        }
    }

    /// <summary>
    ///  Creates eight empty, unnamed presets.
    /// </summary>
    public static IReadOnlyList<TrackSelectionPreset> CreateEmpty()
        => Array.AsReadOnly(Enumerable.Repeat(new TrackSelectionPreset(), SlotCount).ToArray());

    /// <summary>
    ///  Gets the combo and status label for a zero-based slot.
    /// </summary>
    public string DisplayLabel(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(slot, SlotCount);
        return string.IsNullOrEmpty(Name) ? $"Preset {slot + 1}" : $"Preset {slot + 1}: {Name}";
    }

    /// <summary>
    ///  Trims a preset name and rejects control characters or names longer than forty characters.
    /// </summary>
    public static string NormalizeName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        string trimmed = name.Trim();
        if (trimmed.Length > MaximumNameLength)
        {
            throw new ArgumentException($"Preset names must be {MaximumNameLength} characters or fewer.");
        }

        foreach (char character in trimmed)
        {
            if (char.IsControl(character))
            {
                throw new ArgumentException("Preset names cannot contain control characters.");
            }
        }

        return trimmed;
    }

    /// <summary>
    ///  Copies a unique set of primary instruments in kit order.
    /// </summary>
    public static IReadOnlyList<Cr78Instrument> NormalizeInstruments(IEnumerable<Cr78Instrument> instruments)
    {
        ArgumentNullException.ThrowIfNull(instruments);
        HashSet<Cr78Instrument> selected = [];
        foreach (Cr78Instrument instrument in instruments)
        {
            if (!Cr78Kit.Instruments.Contains(instrument))
            {
                throw new ArgumentException("Track presets can contain only the fixed score instruments.", nameof(instruments));
            }

            if (!selected.Add(instrument))
            {
                throw new ArgumentException("A track preset cannot list an instrument more than once.", nameof(instruments));
            }
        }

        return Cr78Kit.Instruments.Where(selected.Contains).ToArray();
    }

    /// <summary>
    ///  Returns a preset with a trimmed name and kit-ordered instruments.
    /// </summary>
    public TrackSelectionPreset Validate()
        => new()
        {
            Name = NormalizeName(Name),
            Instruments = NormalizeInstruments(Instruments)
        };
}

/// <summary>
///  Normal window bounds saved in device pixels, plus the DPI used so a later session can scale them.
/// </summary>
internal sealed record WindowPlacement
{
    /// <summary> Gets a placement that leaves the Designer start position unchanged. </summary>
    public static WindowPlacement Unset { get; } = new();

    /// <summary> Gets the left edge in device pixels. </summary>
    public int X { get; init; }

    /// <summary> Gets the top edge in device pixels. </summary>
    public int Y { get; init; }

    /// <summary> Gets the normal width in device pixels. Zero means the placement is unset. </summary>
    public int Width { get; init; }

    /// <summary> Gets the normal height in device pixels. Zero means the placement is unset. </summary>
    public int Height { get; init; }

    /// <summary> Gets whether the window should open maximized after its normal bounds are restored. </summary>
    public bool IsMaximized { get; init; }

    /// <summary> Gets the DPI at which <see cref="Width"/> and <see cref="Height"/> were captured. </summary>
    public int SavedDpi { get; init; }

    /// <summary> Gets whether both dimensions were saved. </summary>
    public bool HasBounds => Width > 0 && Height > 0;

    /// <summary> Returns the saved rectangle without scaling. </summary>
    public Rectangle ToRectangle() => new(X, Y, Width, Height);

    /// <summary> Scales a 96-DPI length to the current device DPI. </summary>
    public static int ScaleFromDip(int dip, int dpi)
    {
        if (dip <= 0)
        {
            return 0;
        }

        return Math.Max(1, dip * NormalizeDpi(dpi) / 96);
    }

    /// <summary> Converts a device-pixel length to 96-DPI pixels. </summary>
    public static int ScaleToDip(int devicePixels, int dpi)
    {
        if (devicePixels <= 0)
        {
            return 0;
        }

        return Math.Max(1, devicePixels * 96 / NormalizeDpi(dpi));
    }

    /// <summary> Scales size, not position, when the current monitor DPI differs from the saved DPI. </summary>
    public Rectangle ScaleToDpi(int currentDpi)
    {
        if (!HasBounds || SavedDpi <= 0 || currentDpi <= 0 || SavedDpi == currentDpi)
        {
            return ToRectangle();
        }

        return new Rectangle(
            X,
            Y,
            Math.Max(1, Width * currentDpi / SavedDpi),
            Math.Max(1, Height * currentDpi / SavedDpi));
    }

    /// <summary>
    ///  Requires a visible title-bar area on a working area so a restored window can still be moved.
    /// </summary>
    public static bool IsUsableOnScreen(Rectangle bounds)
    {
        if (bounds.Width < 200 || bounds.Height < 150)
        {
            return false;
        }

        foreach (Screen screen in Screen.AllScreens)
        {
            Rectangle visible = Rectangle.Intersect(bounds, screen.WorkingArea);
            if (visible.Width >= 80 && visible.Height >= 48
                && bounds.Top >= screen.WorkingArea.Top - 8
                && bounds.Top < screen.WorkingArea.Bottom - 48)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary> Rejects partial or out-of-range placements. Off-screen bounds remain loadable and are skipped at restore. </summary>
    public WindowPlacement Validate()
    {
        if (!HasBounds)
        {
            if (X != 0 || Y != 0 || Width != 0 || Height != 0 || IsMaximized || SavedDpi != 0)
            {
                throw new ArgumentException("An unset window placement must contain only zeros.");
            }

            return Unset;
        }

        if (SavedDpi is < 96 or > 480)
        {
            throw new ArgumentOutOfRangeException(nameof(SavedDpi), "Window DPI must be between 96 and 480.");
        }

        if (Width is < 200 or > 10_000 || Height is < 150 or > 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(Width), "Saved window size is outside the supported range.");
        }

        if (X is < -32_000 or > 32_000 || Y is < -32_000 or > 32_000)
        {
            throw new ArgumentOutOfRangeException(nameof(X), "Saved window position is outside the supported range.");
        }

        return this;
    }

    private static int NormalizeDpi(int dpi) => dpi is < 96 or > 480 ? 96 : dpi;
}

/// <summary>
///  One persisted score-grid column, with width stored at 96 DPI.
/// </summary>
internal sealed record StepGridColumnPreference
{
    /// <summary> The Designer column names, in collection order. </summary>
    public static IReadOnlyList<string> Names { get; } = CreateNames();

    /// <summary> Gets the column name. </summary>
    public string Name { get; init; } = "";

    /// <summary> Gets the visual display index. </summary>
    public int DisplayIndex { get; init; }

    /// <summary> Gets the width in 96-DPI pixels. </summary>
    public int WidthDip { get; init; }

    /// <summary> Copies the live grid into DIP widths. </summary>
    public static StepGridColumnPreference[] Capture(DataGridView grid, int dpi)
    {
        ArgumentNullException.ThrowIfNull(grid);
        StepGridColumnPreference[] columns = new StepGridColumnPreference[grid.Columns.Count];
        for (int index = 0; index < columns.Length; index++)
        {
            DataGridViewColumn column = grid.Columns[index];
            columns[index] = new StepGridColumnPreference
            {
                Name = column.Name,
                DisplayIndex = column.DisplayIndex,
                WidthDip = WindowPlacement.ScaleToDip(Math.Max(column.MinimumWidth, column.Width), dpi)
            };
        }

        return Validate(columns);
    }

    /// <summary>
    ///  Applies widths and display order. Returns false when the saved set no longer matches the grid.
    /// </summary>
    public static bool TryApply(DataGridView grid, IReadOnlyList<StepGridColumnPreference> columns, int dpi)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(columns);
        if (!MatchesGrid(grid, columns))
        {
            return false;
        }

        foreach (StepGridColumnPreference preference in columns)
        {
            DataGridViewColumn? column = grid.Columns[preference.Name];
            if (column is null)
            {
                return false;
            }

            int width = WindowPlacement.ScaleFromDip(preference.WidthDip, dpi);
            column.Width = Math.Max(column.MinimumWidth, width);
        }

        try
        {
            foreach (StepGridColumnPreference preference in columns.OrderBy(column => column.DisplayIndex))
            {
                DataGridViewColumn? column = grid.Columns[preference.Name];
                if (column is null)
                {
                    return false;
                }

                column.DisplayIndex = preference.DisplayIndex;
            }
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    /// <summary> Accepts an empty list or one complete, unique column set. </summary>
    public static StepGridColumnPreference[] Validate(IReadOnlyList<StepGridColumnPreference> columns)
    {
        ArgumentNullException.ThrowIfNull(columns);
        if (columns.Count == 0)
        {
            return [];
        }

        if (columns.Count != Names.Count)
        {
            throw new ArgumentException("Saved score columns must include every grid column exactly once.");
        }

        HashSet<string> names = new(StringComparer.Ordinal);
        HashSet<int> indexes = [];
        StepGridColumnPreference[] copy = new StepGridColumnPreference[columns.Count];
        for (int index = 0; index < columns.Count; index++)
        {
            StepGridColumnPreference column = columns[index];
            if (!names.Add(column.Name) || !Names.Contains(column.Name))
            {
                throw new ArgumentException("Saved score columns contain an unknown or duplicate name.");
            }

            if (column.DisplayIndex < 0 || column.DisplayIndex >= Names.Count || !indexes.Add(column.DisplayIndex))
            {
                throw new ArgumentException("Saved score columns contain a duplicate or invalid display index.");
            }

            if (column.WidthDip is < 16 or > 2_000)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), "A saved column width is outside the supported range.");
            }

            copy[index] = column;
        }

        return copy;
    }

    private static bool MatchesGrid(DataGridView grid, IReadOnlyList<StepGridColumnPreference> columns)
    {
        if (columns.Count != grid.Columns.Count)
        {
            return false;
        }

        foreach (StepGridColumnPreference preference in columns)
        {
            if (grid.Columns[preference.Name] is null)
            {
                return false;
            }
        }

        return true;
    }

    private static string[] CreateNames()
    {
        string[] names = new string[35];
        names[0] = "_trackColumn";
        names[1] = "_instrumentColumn";
        names[2] = "_auditionColumn";
        for (int step = 1; step <= 32; step++)
        {
            names[step + 2] = $"_step{step:00}";
        }

        return names;
    }
}
