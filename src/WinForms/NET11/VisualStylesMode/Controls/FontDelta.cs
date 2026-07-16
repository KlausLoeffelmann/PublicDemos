// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace VisualStylesModeDemo.Controls;

/// <summary>
///  Describes font size and style changes relative to an existing font.
/// </summary>
[TypeConverter(typeof(FontDeltaConverter))]
public sealed partial class FontDelta : IEquatable<FontDelta>
{
    private const float MinimumFontSizeInPoints = 1F;
    private const FontStyle ValidFontStyles =
        FontStyle.Bold | FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout;

    private readonly ConditionalWeakTable<Font, CachedFont> _fontCache = [];
    private readonly List<WeakReference<Font>> _cachedFonts = [];
    private readonly Lock _fontCacheLock = new();
    private float _sizeDeltaInPoints;
    private FontStyle _addedStyle;
    private FontStyle _removedStyle;

    /// <summary>
    ///  Initializes a relative font without any size or style changes.
    /// </summary>
    public FontDelta()
    {
    }

    /// <summary>
    ///  Initializes a relative font with the specified size and style changes.
    /// </summary>
    /// <param name="sizeDeltaInPoints">The number of points to add to the source font size.</param>
    /// <param name="addedStyle">The font styles to add after removing <paramref name="removedStyle"/>.</param>
    /// <param name="removedStyle">The font styles to remove from the source font.</param>
    public FontDelta(
        float sizeDeltaInPoints,
        FontStyle addedStyle,
        FontStyle removedStyle)
    {
        ValidateSizeDelta(sizeDeltaInPoints);
        ValidateFontStyle(addedStyle);
        ValidateFontStyle(removedStyle);

        _sizeDeltaInPoints = sizeDeltaInPoints;
        _addedStyle = addedStyle;
        _removedStyle = removedStyle;
    }

    /// <summary>
    ///  Gets or sets the number of points to add to the source font size.
    /// </summary>
    [DefaultValue(0F)]
    [NotifyParentProperty(true)]
    public float SizeDeltaInPoints
    {
        get => _sizeDeltaInPoints;
        set
        {
            ValidateSizeDelta(value);
            if (_sizeDeltaInPoints == value)
            {
                return;
            }

            _sizeDeltaInPoints = value;
            OnChanged();
        }
    }

    /// <summary>
    ///  Gets or sets the font styles to add after applying <see cref="RemovedStyle"/>.
    /// </summary>
    [DefaultValue(FontStyle.Regular)]
    [NotifyParentProperty(true)]
    public FontStyle AddedStyle
    {
        get => _addedStyle;
        set
        {
            ValidateFontStyle(value);
            if (_addedStyle == value)
            {
                return;
            }

            _addedStyle = value;
            OnChanged();
        }
    }

    /// <summary>
    ///  Gets or sets the font styles to remove from the source font.
    /// </summary>
    [DefaultValue(FontStyle.Regular)]
    [NotifyParentProperty(true)]
    public FontStyle RemovedStyle
    {
        get => _removedStyle;
        set
        {
            ValidateFontStyle(value);
            if (_removedStyle == value)
            {
                return;
            }

            _removedStyle = value;
            OnChanged();
        }
    }

    internal event EventHandler? Changed;

    /// <summary>
    ///  Gets a font produced by applying this delta to <paramref name="sourceFont"/>.
    /// </summary>
    /// <param name="sourceFont">The font whose size and styles provide the baseline.</param>
    /// <returns>
    ///  A weakly cached font owned by this <see cref="FontDelta"/>. The caller must not dispose it.
    /// </returns>
    public Font GetFont(Font sourceFont)
    {
        ArgumentNullException.ThrowIfNull(sourceFont);

        lock (_fontCacheLock)
        {
            if (_fontCache.TryGetValue(sourceFont, out CachedFont? cachedFont)
                && cachedFont.Reference.TryGetTarget(out Font? font))
            {
                return font;
            }

            _fontCache.Remove(sourceFont);

            Font createdFont = CreateFont(sourceFont);
            _fontCache.Add(sourceFont, new CachedFont(createdFont));
            _cachedFonts.RemoveAll(static reference => !reference.TryGetTarget(out _));
            _cachedFonts.Add(new WeakReference<Font>(createdFont));

            return createdFont;
        }
    }

    /// <inheritdoc/>
    public bool Equals(FontDelta? other) =>
        other is not null
        && SizeDeltaInPoints == other.SizeDeltaInPoints
        && AddedStyle == other.AddedStyle
        && RemovedStyle == other.RemovedStyle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FontDelta other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(SizeDeltaInPoints, AddedStyle, RemovedStyle);

    /// <inheritdoc/>
    public override string ToString() =>
        string.Create(
            CultureInfo.CurrentCulture,
            $"{SizeDeltaInPoints:+0.##;-0.##;0} pt, +{AddedStyle}, -{RemovedStyle}");

    private static void ValidateSizeDelta(float value)
    {
        if (!float.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "The font size adjustment must be finite.");
        }
    }

    private static void ValidateFontStyle(FontStyle value)
    {
        if ((value & ~ValidFontStyles) != 0)
        {
            throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FontStyle));
        }
    }

    private Font CreateFont(Font sourceFont)
    {
        FontStyle style = (sourceFont.Style & ~RemovedStyle) | AddedStyle;
        float sizeInPoints = Math.Max(
            MinimumFontSizeInPoints,
            sourceFont.SizeInPoints + SizeDeltaInPoints);

        if ((style & FontStyle.Bold) != 0
            && TryCreateSemiboldFont(sourceFont, sizeInPoints, style, out Font? semiboldFont))
        {
            return semiboldFont!;
        }

        return CreateSupportedFont(sourceFont, sizeInPoints, style);
    }

    private static Font CreateSupportedFont(Font sourceFont, float sizeInPoints, FontStyle requestedStyle)
    {
        try
        {
            return new Font(
                family: sourceFont.FontFamily,
                emSize: sizeInPoints,
                style: requestedStyle,
                unit: GraphicsUnit.Point,
                gdiCharSet: sourceFont.GdiCharSet,
                gdiVerticalFont: sourceFont.GdiVerticalFont);
        }
        catch (ArgumentException) when (requestedStyle != FontStyle.Regular)
        {
            FontStyle decorations = requestedStyle & (FontStyle.Underline | FontStyle.Strikeout);

            return new Font(
                family: sourceFont.FontFamily,
                emSize: sizeInPoints,
                style: decorations,
                unit: GraphicsUnit.Point,
                gdiCharSet: sourceFont.GdiCharSet,
                gdiVerticalFont: sourceFont.GdiVerticalFont);
        }
    }

    private void InvalidateFontCache()
    {
        lock (_fontCacheLock)
        {
            foreach (WeakReference<Font> reference in _cachedFonts)
            {
                if (reference.TryGetTarget(out Font? font))
                {
                    font.Dispose();
                }
            }

            _cachedFonts.Clear();
            _fontCache.Clear();
        }
    }

    private void OnChanged()
    {
        InvalidateFontCache();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private static bool TryCreateSemiboldFont(
        Font sourceFont,
        float sizeInPoints,
        FontStyle requestedStyle,
        out Font? semiboldFont)
    {
        // System.Drawing has no font-weight API, so a separately installed named face is the
        // only way to request semibold without dropping down to native GDI.
        string semiboldFamilyName = $"{sourceFont.FontFamily.Name} Semibold";
        FontStyle semiboldStyle = requestedStyle & ~FontStyle.Bold;

        try
        {
            Font candidate = new(
                familyName: semiboldFamilyName,
                emSize: sizeInPoints,
                style: semiboldStyle,
                unit: GraphicsUnit.Point,
                gdiCharSet: sourceFont.GdiCharSet,
                gdiVerticalFont: sourceFont.GdiVerticalFont);

            if (string.Equals(candidate.Name, semiboldFamilyName, StringComparison.OrdinalIgnoreCase))
            {
                semiboldFont = candidate;
                return true;
            }

            candidate.Dispose();
        }
        catch (ArgumentException)
        {
            // A named semibold face is optional; fall back to the source family's bold face.
        }

        semiboldFont = null;
        return false;
    }
}
