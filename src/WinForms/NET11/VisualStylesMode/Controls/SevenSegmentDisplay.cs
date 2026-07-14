// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using VisualStylesModeDemo.Views;

namespace VisualStylesModeDemo.Controls;

/// <summary>
///  A DPI-aware simulated seven-segment currency display.
/// </summary>
internal sealed class SevenSegmentDisplay : Control
{
    private static readonly byte[] s_segmentMasks =
    [
        0b_0011_1111,
        0b_0000_0110,
        0b_0101_1011,
        0b_0100_1111,
        0b_0110_0110,
        0b_0110_1101,
        0b_0111_1101,
        0b_0000_0111,
        0b_0111_1111,
        0b_0110_1111,
    ];

    private decimal _value;
    private RegisterDisplayMode _displayMode;

    public SevenSegmentDisplay()
    {
        SetStyle(
            ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
            true);

        BackColor = Color.FromArgb(18, 24, 22);
        ForeColor = Color.FromArgb(255, 118, 35);
        AccessibleRole = AccessibleRole.StaticText;
        TabStop = false;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Value
    {
        get => _value;
        set
        {
            if (_value == value)
            {
                return;
            }

            _value = value;
            Invalidate();
            NotifyAccessibleValueChanged();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RegisterDisplayMode DisplayMode
    {
        get => _displayMode;
        set
        {
            if (_displayMode == value)
            {
                return;
            }

            _displayMode = value;
            Invalidate();
            NotifyAccessibleValueChanged();
        }
    }

    protected override Size DefaultSize => new(640, 112);

    protected override AccessibleObject CreateAccessibilityInstance() => new DisplayAccessibleObject(this);

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(BackColor);

        float scale = DeviceDpi / 96F;
        int padding = Math.Max(6, (int)Math.Round(10 * scale));
        int labelHeight = Math.Max(Font.Height, (int)Math.Round(18 * scale));

        Rectangle labelBounds = new(
            padding,
            padding / 2,
            Math.Max(0, ClientSize.Width - (padding * 2)),
            labelHeight);
        TextRenderer.DrawText(
            graphics,
            DisplayMode.ToString().ToUpperInvariant(),
            Font,
            labelBounds,
            Color.FromArgb(150, ForeColor),
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

        Rectangle digitBounds = new(
            padding,
            labelBounds.Bottom,
            Math.Max(0, ClientSize.Width - (padding * 2)),
            Math.Max(0, ClientSize.Height - labelBounds.Bottom - padding));

        if (digitBounds.Width <= 0 || digitBounds.Height <= 0)
        {
            return;
        }

        const int digitCount = 10;
        string amountText = Math.Abs(Value).ToString("0.00", CultureInfo.InvariantCulture);
        string digits = amountText.Replace(".", string.Empty, StringComparison.Ordinal);
        if (digits.Length > digitCount)
        {
            digits = new string('9', digitCount);
        }

        digits = digits.PadLeft(digitCount);
        float cellWidth = digitBounds.Width / (float)digitCount;
        float thickness = Math.Max(2F, Math.Min(cellWidth, digitBounds.Height) * 0.12F);

        using SolidBrush litBrush = new(ForeColor);
        using SolidBrush dimBrush = new(Color.FromArgb(38, ForeColor));

        for (int index = 0; index < digitCount; index++)
        {
            RectangleF cell = new(
                digitBounds.Left + (index * cellWidth),
                digitBounds.Top,
                cellWidth,
                digitBounds.Height);
            DrawDigit(graphics, cell, digits[index], thickness, litBrush, dimBrush);
        }

        float dotSize = Math.Max(3F, thickness * 0.8F);
        float dotCellRight = digitBounds.Left + ((digitCount - 2) * cellWidth);
        graphics.FillEllipse(
            litBrush,
            dotCellRight - dotSize,
            digitBounds.Bottom - dotSize,
            dotSize,
            dotSize);
    }

    private static void DrawDigit(
        Graphics graphics,
        RectangleF bounds,
        char character,
        float thickness,
        Brush litBrush,
        Brush dimBrush)
    {
        byte mask = character is >= '0' and <= '9'
            ? s_segmentMasks[character - '0']
            : (byte)0;

        float horizontalInset = thickness * 0.8F;
        float left = bounds.Left + horizontalInset;
        float right = bounds.Right - horizontalInset;
        float top = bounds.Top + (thickness * 0.25F);
        float middle = bounds.Top + (bounds.Height / 2F);
        float bottom = bounds.Bottom - (thickness * 0.25F);
        float upperBottom = middle - (thickness * 0.35F);
        float lowerTop = middle + (thickness * 0.35F);

        DrawHorizontalSegment(graphics, left, right, top, thickness, IsLit(mask, 0), litBrush, dimBrush);
        DrawVerticalSegment(graphics, right, top, upperBottom, thickness, IsLit(mask, 1), litBrush, dimBrush);
        DrawVerticalSegment(graphics, right, lowerTop, bottom, thickness, IsLit(mask, 2), litBrush, dimBrush);
        DrawHorizontalSegment(graphics, left, right, bottom, thickness, IsLit(mask, 3), litBrush, dimBrush);
        DrawVerticalSegment(graphics, left, lowerTop, bottom, thickness, IsLit(mask, 4), litBrush, dimBrush);
        DrawVerticalSegment(graphics, left, top, upperBottom, thickness, IsLit(mask, 5), litBrush, dimBrush);
        DrawHorizontalSegment(graphics, left, right, middle, thickness, IsLit(mask, 6), litBrush, dimBrush);
    }

    private static bool IsLit(byte mask, int segment) => (mask & (1 << segment)) != 0;

    private static void DrawHorizontalSegment(
        Graphics graphics,
        float left,
        float right,
        float y,
        float thickness,
        bool isLit,
        Brush litBrush,
        Brush dimBrush)
    {
        float half = thickness / 2F;
        PointF[] points =
        [
            new(left + half, y - half),
            new(right - half, y - half),
            new(right, y),
            new(right - half, y + half),
            new(left + half, y + half),
            new(left, y),
        ];
        graphics.FillPolygon(isLit ? litBrush : dimBrush, points);
    }

    private static void DrawVerticalSegment(
        Graphics graphics,
        float x,
        float top,
        float bottom,
        float thickness,
        bool isLit,
        Brush litBrush,
        Brush dimBrush)
    {
        float half = thickness / 2F;
        PointF[] points =
        [
            new(x - half, top + half),
            new(x, top),
            new(x + half, top + half),
            new(x + half, bottom - half),
            new(x, bottom),
            new(x - half, bottom - half),
        ];
        graphics.FillPolygon(isLit ? litBrush : dimBrush, points);
    }

    private void NotifyAccessibleValueChanged()
    {
        if (IsHandleCreated)
        {
            AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
        }
    }

    private sealed class DisplayAccessibleObject(SevenSegmentDisplay owner)
        : ControlAccessibleObject(owner)
    {
        public override string? Value
        {
            get => $"{owner.DisplayMode}: {owner.Value:0.00}";
            set
            {
            }
        }
    }
}
