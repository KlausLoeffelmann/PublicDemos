using WinForms.Audio.Percussion;

namespace DrumMachine.Demo.Controls;

/// <summary>
///  A double-buffered score grid whose beat and track colors stay readable in Classic, Dark, and high contrast.
/// </summary>
internal class BeatGridView : DataGridView
{
    /// <summary>
    ///  Creates a grid that paints from <see cref="SystemColors"/> and never adds or removes rows itself.
    /// </summary>
    public BeatGridView()
    {
        DoubleBuffered = true;
        EnableHeadersVisualStyles = false;
        BackgroundColor = SystemColors.Window;
        GridColor = SystemColors.ControlDark;
        BorderStyle = BorderStyle.None;
        ApplyChrome();
    }

    /// <summary>
    ///  Repaints headers and the empty background when Windows changes Classic, Dark, or high-contrast colors.
    /// </summary>
    protected override void OnSystemColorsChanged(EventArgs e)
    {
        base.OnSystemColorsChanged(e);
        BackgroundColor = SystemColors.Window;
        GridColor = SystemInformation.HighContrast ? SystemColors.WindowFrame : SystemColors.ControlDark;
        ApplyChrome();
    }

    /// <summary>
    ///  Applies a step-column color. Playback and high contrast stay on system highlight and window colors.
    /// </summary>
    public void ApplyStepColumnStyle(DataGridViewColumn column, int stepIndex, bool playing, bool missing)
    {
        ArgumentNullException.ThrowIfNull(column);
        Color back = StepBackColor(stepIndex, missing, playing);
        Color fore = playing
            ? SystemColors.HighlightText
            : missing ? SystemColors.GrayText : SystemColors.WindowText;
        column.DefaultCellStyle.BackColor = back;
        column.DefaultCellStyle.ForeColor = fore;
        column.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
        column.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
        column.HeaderCell.Style.BackColor = HeaderBackColor(stepIndex, playing);
        column.HeaderCell.Style.ForeColor = SystemColors.ControlText;
        column.HeaderCell.Style.SelectionBackColor = column.HeaderCell.Style.BackColor;
        column.HeaderCell.Style.SelectionForeColor = SystemColors.ControlText;
    }

    /// <summary>
    ///  Tints only the instrument name, leaving checkboxes on the window color so they remain obvious.
    /// </summary>
    public static void ApplyTrackNameStyle(DataGridViewCell cell, Cr78Instrument instrument, bool selected)
    {
        ArgumentNullException.ThrowIfNull(cell);
        cell.Style.BackColor = SystemInformation.HighContrast
            ? SystemColors.Window
            : Blend(SystemColors.Window, TrackAccent(instrument), selected ? 0.14f : 0.06f);
        cell.Style.ForeColor = selected ? SystemColors.WindowText : SystemColors.GrayText;
        cell.Style.SelectionBackColor = SystemColors.Highlight;
        cell.Style.SelectionForeColor = SystemColors.HighlightText;
    }

    /// <summary>
    ///  Gets the background for one sixteenth-note column.
    /// </summary>
    public static Color StepBackColor(int stepIndex, bool missing, bool playing)
    {
        if (playing)
        {
            return SystemColors.Highlight;
        }

        if (missing || SystemInformation.HighContrast)
        {
            return missing ? SystemColors.Control : SystemColors.Window;
        }

        float amount = stepIndex % 4 == 0 ? 0.12f : stepIndex % 4 == 2 ? 0.05f : 0f;
        return amount == 0f
            ? SystemColors.Window
            : Blend(SystemColors.Window, BeatAccent(stepIndex), amount);
    }

    /// <summary>
    ///  Lets saved widths stick. Fill mode would discard them on the next layout.
    /// </summary>
    public void UseExplicitStepWidths()
    {
        foreach (DataGridViewColumn column in Columns)
        {
            if (column.Name.StartsWith("_step", StringComparison.Ordinal))
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }
    }

    private void ApplyChrome()
    {
        ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
        ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
        ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
        ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
        DefaultCellStyle.BackColor = SystemColors.Window;
        DefaultCellStyle.ForeColor = SystemColors.WindowText;
        DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
        DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
        RowHeadersDefaultCellStyle.BackColor = SystemColors.Control;
        RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
    }

    private static Color HeaderBackColor(int stepIndex, bool playing)
    {
        if (playing || SystemInformation.HighContrast || stepIndex % 4 != 0)
        {
            return SystemColors.Control;
        }

        return Blend(SystemColors.Control, BeatAccent(stepIndex), 0.16f);
    }

    private static Color BeatAccent(int stepIndex)
        => (stepIndex / 4 % 4) switch
        {
            0 => Color.FromArgb(168, 92, 72),
            1 => Color.FromArgb(70, 116, 158),
            2 => Color.FromArgb(78, 132, 96),
            _ => Color.FromArgb(146, 116, 62)
        };

    private static Color TrackAccent(Cr78Instrument instrument)
    {
        string name = instrument.ToString();
        if (name.StartsWith("BassDrum", StringComparison.Ordinal))
        {
            return Color.FromArgb(164, 78, 68);
        }

        if (name.StartsWith("SnareDrum", StringComparison.Ordinal) || name == nameof(Cr78Instrument.RimShot))
        {
            return Color.FromArgb(166, 122, 62);
        }

        if (name.StartsWith("HiHat", StringComparison.Ordinal) || name == "MetallicHiHat" || name == nameof(Cr78Instrument.Cymbal))
        {
            return Color.FromArgb(62, 122, 142);
        }

        if (name.Contains("Bongo", StringComparison.Ordinal) || name.Contains("Conga", StringComparison.Ordinal))
        {
            return Color.FromArgb(96, 128, 72);
        }

        if (name.StartsWith("Tambourine", StringComparison.Ordinal) || name == nameof(Cr78Instrument.Maracas))
        {
            return Color.FromArgb(150, 96, 72);
        }

        if (name.StartsWith("Guiro", StringComparison.Ordinal) || name == nameof(Cr78Instrument.Claves))
        {
            return Color.FromArgb(78, 104, 148);
        }

        return Color.FromArgb(140, 118, 64);
    }

    private static Color Blend(Color basis, Color accent, float amount)
    {
        float keep = 1f - amount;
        int channel(byte from, byte toward) => (int)Math.Round(from * keep + toward * amount);
        return Color.FromArgb(
            channel(basis.R, accent.R),
            channel(basis.G, accent.G),
            channel(basis.B, accent.B));
    }
}
