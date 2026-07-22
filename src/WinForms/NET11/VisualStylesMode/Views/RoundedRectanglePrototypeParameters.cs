// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Shared, observable parameter bag for the Rounded Rectangle prototype. Every preview cell reads
///  from the same instance and re-paints whenever <see cref="Changed"/> is raised, so tuning a
///  control updates all techniques across all background themes simultaneously.
/// </summary>
/// <remarks>
///  All geometric values are expressed in device-independent pixels (DIPs); the preview panels scale
///  them by their own <see cref="Control.DeviceDpi"/> at paint time.
/// </remarks>
internal sealed class RoundedRectanglePrototypeParameters
{
    private float _cornerRadius = 18f;
    private float _borderThickness = 2f;
    private Color _strokeColor = Color.FromArgb(0, 120, 215);
    private bool _fillEnabled = true;
    private int _fillAlpha = 255;
    private int _supersamplingFactor = 4;

    /// <summary>Raised whenever any parameter changes so previews can invalidate.</summary>
    public event EventHandler? Changed;

    /// <summary>Corner radius in DIPs.</summary>
    public float CornerRadius
    {
        get => _cornerRadius;
        set => SetField(ref _cornerRadius, value);
    }

    /// <summary>Border (stroke) thickness in DIPs.</summary>
    public float BorderThickness
    {
        get => _borderThickness;
        set => SetField(ref _borderThickness, value);
    }

    /// <summary>Color of the stroked border.</summary>
    public Color StrokeColor
    {
        get => _strokeColor;
        set => SetField(ref _strokeColor, value);
    }

    /// <summary>Whether the body is filled beneath the border.</summary>
    public bool FillEnabled
    {
        get => _fillEnabled;
        set => SetField(ref _fillEnabled, value);
    }

    /// <summary>Alpha (0-255) applied to the theme-provided body fill color.</summary>
    public int FillAlpha
    {
        get => _fillAlpha;
        set => SetField(ref _fillAlpha, Math.Clamp(value, 0, 255));
    }

    /// <summary>Supersampling factor (2-4x) used by the SSAA technique only.</summary>
    public int SupersamplingFactor
    {
        get => _supersamplingFactor;
        set => SetField(ref _supersamplingFactor, Math.Clamp(value, 2, 4));
    }

    private void SetField<T>(ref T field, T value)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
