// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace VisualStylesModeDemo.Controls;

/// <summary>
///  A GroupBoxEx that demonstrates mode-aware custom rendering for the .NET 11 visual styles.
/// </summary>
public class GroupBoxEx : GroupBox, ISupportInitialize
{
    private const int LogicalPadding = 8;
    private const int LogicalCaptionTopPadding = 6;
    private const int LogicalCaptionBottomPadding = 2;
    private const int CaptionLeadingPadding = 1;

    private FontTemplate _captionFontTemplate = new(
        1F, 
        FontStyle.Bold, 
        FontStyle.Regular);

    private Color _captionBackColor = SystemColors.Control;
    private int _captionHeight;
    private int _measuredCaptionWidth = -1;
    private int _initializationCount;
    private bool _initialized;

    /// <summary>
    ///  Initializes a new instance of the <see cref="GroupBoxEx"/> class.
    /// </summary>
    public GroupBoxEx()
    {
        SetStyle(
            ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
            true);

        _captionFontTemplate.Changed += CaptionFontDelta_Changed;
        BackColor = SystemColors.ControlLight;
        _initialized = true;
    }

    /// <inheritdoc/>
    [DefaultValue(typeof(Color), "ControlLight")]
    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    /// <summary>
    ///  Gets or sets the color of the caption band used by the .NET 11 visual style.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(typeof(Color), "Control")]
    public Color CaptionBackColor
    {
        get => _captionBackColor;
        set
        {
            if (_captionBackColor == value)
            {
                return;
            }

            _captionBackColor = value;
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the size and style changes applied to <see cref="Control.Font"/> for the caption.
    /// </summary>
    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [RefreshProperties(RefreshProperties.All)]
    public FontTemplate CaptionFontTemplate
    {
        get => _captionFontTemplate;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (_captionFontTemplate.Equals(value))
            {
                return;
            }

            _captionFontTemplate.Changed -= CaptionFontDelta_Changed;
            _captionFontTemplate = value;
            _captionFontTemplate.Changed += CaptionFontDelta_Changed;
            UpdateCaptionMetrics(forceLayout: true);
        }
    }

    /// <inheritdoc/>
    public void BeginInit()
    {
        if (_initializationCount++ == 0)
        {
            SuspendLayout();
        }
    }

    /// <inheritdoc/>
    public void EndInit()
    {
        if (_initializationCount == 0 || --_initializationCount > 0)
        {
            return;
        }

        try
        {
            MeasureCaptionHeight();
        }
        finally
        {
            ResumeLayout(performLayout: false);
        }

        PerformLayout();
        Invalidate();
    }

    /// <inheritdoc/>
    public override Rectangle DisplayRectangle
    {
        get
        {
            if (!UsesNet11VisualStyles)
            {
                return base.DisplayRectangle;
            }

            int captionHeight = GetCaptionHeight();

            return new Rectangle(
                Padding.Left,
                captionHeight + Padding.Top,
                Math.Max(0, ClientSize.Width - Padding.Horizontal),
                Math.Max(0, ClientSize.Height - captionHeight - Padding.Vertical));
        }
    }

    /// <inheritdoc/>
    protected override Padding DefaultPadding
        => new(LogicalPadding);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _captionFontTemplate.Changed -= CaptionFontDelta_Changed;
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateCaptionMetrics(forceLayout: true);
    }

    /// <inheritdoc/>
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        UpdateCaptionMetrics(forceLayout: true);
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (!UsesNet11VisualStyles)
        {
            base.OnPaint(e);
            return;
        }

        Graphics graphics = e.Graphics;
        graphics.Clear(BackColor);

        int captionHeight = GetCaptionHeight();
        Rectangle captionBand = new(
            x: 0,
            y: 0,
            width: ClientSize.Width,
            height: Math.Min(ClientSize.Height, captionHeight));

        using (SolidBrush captionBrush = new(CaptionBackColor))
        {
            graphics.FillRectangle(captionBrush, captionBand);
        }

        TextRenderer.DrawText(
            dc: graphics,
            text: Text,
            font: CaptionFont,
            bounds: GetCaptionTextBounds(captionBand),
            foreColor: Enabled ? ForeColor : SystemColors.GrayText,
            flags: GetCaptionTextFormatFlags());

        if (SystemInformation.HighContrast && ClientSize.Width > 0 && ClientSize.Height > captionHeight)
        {
            graphics.DrawRectangle(
                pen: SystemPens.WindowText,
                x: 0,
                y: captionHeight,
                width: ClientSize.Width - 1,
                height: ClientSize.Height - captionHeight - 1);
        }
    }

    /// <inheritdoc/>
    protected override void OnVisualStylesModeChanged(EventArgs e)
    {
        base.OnVisualStylesModeChanged(e);
        UpdateCaptionMetrics(forceLayout: true);
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        if (UsesNet11VisualStyles && GetAvailableCaptionWidth() != _measuredCaptionWidth)
        {
            UpdateCaptionMetrics(forceLayout: false);
        }
    }

    /// <inheritdoc/>
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);

        if (UsesNet11VisualStyles)
        {
            UpdateCaptionMetrics(forceLayout: false);
        }
    }

    private Font CaptionFont
        => _captionFontTemplate.GetFont(Font);

    private bool IsInitializing
        => _initializationCount > 0;

    private bool UsesNet11VisualStyles
        => _initialized
            && (VisualStylesMode
                is VisualStylesMode.Net11
                or VisualStylesMode.Latest);

    private void CaptionFontDelta_Changed(object? sender, EventArgs e)
        => UpdateCaptionMetrics(forceLayout: true);

    private int GetAvailableCaptionWidth()
        => Math.Max(
            1,
            ClientSize.Width - CaptionLeadingPadding - ScaleLogical(LogicalPadding));

    private int GetCaptionHeight()
    {
        int availableWidth = GetAvailableCaptionWidth();
        if (_captionHeight == 0 || availableWidth != _measuredCaptionWidth)
        {
            MeasureCaptionHeight();
        }

        return _captionHeight;
    }

    private Rectangle GetCaptionTextBounds(Rectangle captionBand)
    {
        int trailingPadding = ScaleLogical(LogicalPadding);

        int left = RightToLeft == RightToLeft.Yes
            ? captionBand.Left + trailingPadding
            : captionBand.Left + CaptionLeadingPadding;

        int right = RightToLeft == RightToLeft.Yes
            ? captionBand.Right - CaptionLeadingPadding
            : captionBand.Right - trailingPadding;

        int top = Math.Min(
            captionBand.Bottom,
            captionBand.Top + ScaleLogical(LogicalCaptionTopPadding));

        int bottom = Math.Max(
            top,
            captionBand.Bottom - ScaleLogical(LogicalCaptionBottomPadding));

        return Rectangle.FromLTRB(
            Math.Min(left, captionBand.Right),
            top,
            Math.Max(Math.Min(left, captionBand.Right), right),
            bottom);
    }

    private TextFormatFlags GetCaptionTextFormatFlags()
        => TextFormatFlags.NoPadding
            | TextFormatFlags.WordBreak
            | (RightToLeft == RightToLeft.Yes
                ? TextFormatFlags.Right | TextFormatFlags.RightToLeft
                : TextFormatFlags.Left);

    private int MeasureCaptionHeight()
    {
        int availableWidth = GetAvailableCaptionWidth();
        Font captionFont = CaptionFont;

        int textHeight = string.IsNullOrEmpty(Text)
            ? captionFont.Height
            : TextRenderer.MeasureText(
                text: Text,
                font: captionFont,
                proposedSize: new Size(availableWidth, int.MaxValue),
                flags: GetCaptionTextFormatFlags()).Height;

        _measuredCaptionWidth = availableWidth;

        _captionHeight = Math.Max(captionFont.Height, textHeight)
            + ScaleLogical(LogicalCaptionTopPadding)
            + ScaleLogical(LogicalCaptionBottomPadding);

        return _captionHeight;
    }

    private void UpdateCaptionMetrics(bool forceLayout)
    {
        if (!_initialized)
        {
            return;
        }

        if (IsInitializing)
        {
            _captionHeight = 0;
            _measuredCaptionWidth = -1;
            Invalidate();

            return;
        }

        int previousHeight = _captionHeight;
        int measuredHeight = MeasureCaptionHeight();

        if (forceLayout || measuredHeight != previousHeight)
        {
            PerformLayout();
        }

        Invalidate();
    }

    private void ResetCaptionFontDelta()
        => CaptionFontTemplate = new FontTemplate(
            sizeDeltaInPoints: 1F,
            addedStyle: FontStyle.Bold,
            removedStyle: FontStyle.Regular);

    private int ScaleLogical(int logicalValue)
        => (int)Math.Round(logicalValue * DeviceDpi / 96F, MidpointRounding.AwayFromZero);

    private bool ShouldSerializeCaptionFontDelta()
        => _captionFontTemplate.SizeDeltaInPoints != 1F
            || _captionFontTemplate.AddedStyle != FontStyle.Bold
            || _captionFontTemplate.RemovedStyle != FontStyle.Regular;
}
