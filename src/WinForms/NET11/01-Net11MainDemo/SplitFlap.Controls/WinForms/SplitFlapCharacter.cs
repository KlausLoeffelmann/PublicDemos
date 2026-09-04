namespace SplitFlap.WinForms;

/// <summary>
///  One split-flap character as a control. Put four of them next to each other and you have a
///  1970s clock radio. Everything else (font, colors, speed, jams) is inherited from
///  <see cref="SplitFlapCharacterDisplay"/>; the grid is fixed at 1x1.
/// </summary>
[ToolboxItem(true)]
[DefaultProperty(nameof(Character))]
[Description("A single retro split-flap character.")]
public class SplitFlapCharacter : SplitFlapCharacterDisplay
{
    /// <summary>
    ///  Initializes a single character showing a blank.
    /// </summary>
    public SplitFlapCharacter()
    {
        base.Rows = 1;
        base.Columns = 1;
    }

    /// <summary>
    ///  The character to show. Characters not on the drum resolve to blank.
    /// </summary>
    [Category("Appearance")]
    [Description("The character to show.")]
    [DefaultValue(' ')]
    public char Character
    {
        get => Text is { Length: > 0 } text ? text[0] : ' ';
        set => Text = value.ToString();
    }

    /// <summary>
    ///  The visual behind this control, for sound hookups.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SplitFlapCharacterVisual Visual
        => GetVisual(0, 0);

    /// <summary>
    ///  Always 1.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int Rows
    {
        get => 1;
        set { }
    }

    /// <summary>
    ///  Always 1.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int Columns
    {
        get => 1;
        set { }
    }

    /// <summary>
    ///  Use <see cref="Character"/> instead.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value is { Length: > 0 } ? value[..1] : string.Empty;
    }

    /// <inheritdoc/>
    protected override Padding DefaultPadding
        => Padding.Empty;
}
