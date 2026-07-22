// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

/// <summary>
///  Conceptual prototype scenario that explores how to draw a "proper" rounded rectangle with GDI+.
///  A control strip tunes the shared parameters live while a grid renders the same rectangle with
///  several techniques (columns) over dark, classic and colorful backgrounds (rows), making it easy
///  to see how each technique's corner arcs blend with the straight edges across colors and themes.
/// </summary>
/// <remarks>
///  The prototype UI is built entirely in <see cref="BuildPrototypeUi"/> rather than the Designer,
///  because it is a data-driven grid of custom preview panels (techniques x themes) rather than a
///  fixed control layout.
/// </remarks>
public partial class ScratchView : UserControl, IScenarioView
{
    private static readonly RoundedRectangleTechnique[] s_techniques = Enum.GetValues<RoundedRectangleTechnique>();
    private static readonly PreviewBackgroundTheme[] s_themes = Enum.GetValues<PreviewBackgroundTheme>();

    private readonly RoundedRectanglePrototypeParameters _parameters = new();
    private Panel? _colorIndicator;

    public ScratchView()
    {
        InitializeComponent();
        BuildPrototypeUi();
    }

    public string DisplayName => "Rounded Rectangle Prototype";

    private void BuildPrototypeUi()
    {
        SuspendLayout();

        TableLayoutPanel root = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        root.Controls.Add(CreateDescriptionLabel(), 0, 0);
        root.Controls.Add(CreateControlStrip(), 0, 1);
        root.Controls.Add(CreatePreviewGrid(), 0, 2);

        Controls.Add(root);

        ResumeLayout(true);
    }

    private static Label CreateDescriptionLabel() => new()
    {
        Dock = DockStyle.Fill,
        AutoSize = true,
        Padding = new Padding(8, 8, 8, 2),
        Text = "GDI+ rounded-rectangle anti-aliasing prototype. Columns are drawing techniques, rows "
            + "are background themes. Tune the parameters below and watch how each technique's corner "
            + "arcs blend with the straight edges.",
    };

    private Control CreateControlStrip()
    {
        FlowLayoutPanel strip = new()
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(6, 2, 6, 6),
        };

        Label radiusValue = new() { AutoSize = true, Text = FormatDip(_parameters.CornerRadius) };
        TrackBar radius = CreateTrackBar(0, 60, (int)_parameters.CornerRadius);
        radius.ValueChanged += (_, _) =>
        {
            _parameters.CornerRadius = radius.Value;
            radiusValue.Text = FormatDip(radius.Value);
        };
        strip.Controls.Add(CreateCluster("Corner radius", radius, radiusValue));

        Label thicknessValue = new() { AutoSize = true, Text = FormatDip(_parameters.BorderThickness) };
        TrackBar thickness = CreateTrackBar(1, 12, (int)_parameters.BorderThickness);
        thickness.ValueChanged += (_, _) =>
        {
            _parameters.BorderThickness = thickness.Value;
            thicknessValue.Text = FormatDip(thickness.Value);
        };
        strip.Controls.Add(CreateCluster("Border thickness", thickness, thicknessValue));

        CheckBox fill = new() { Text = "Fill body", AutoSize = true, Checked = _parameters.FillEnabled };
        fill.CheckedChanged += (_, _) => _parameters.FillEnabled = fill.Checked;
        Label alphaValue = new() { AutoSize = true, Text = _parameters.FillAlpha.ToString() };
        TrackBar alpha = CreateTrackBar(0, 255, _parameters.FillAlpha);
        alpha.ValueChanged += (_, _) =>
        {
            _parameters.FillAlpha = alpha.Value;
            alphaValue.Text = alpha.Value.ToString();
        };
        strip.Controls.Add(CreateCluster("Fill / alpha", fill, alpha, alphaValue));

        NumericUpDown ssaa = new()
        {
            Minimum = 2,
            Maximum = 4,
            Value = _parameters.SupersamplingFactor,
            Width = 56,
        };
        ssaa.ValueChanged += (_, _) => _parameters.SupersamplingFactor = (int)ssaa.Value;
        strip.Controls.Add(CreateCluster("SSAA factor (col 5)", ssaa));

        strip.Controls.Add(CreateColorCluster());

        return strip;
    }

    private Control CreateColorCluster()
    {
        FlowLayoutPanel row = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = Padding.Empty,
        };

        _colorIndicator = new Panel
        {
            Width = 24,
            Height = 24,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = _parameters.StrokeColor,
            Margin = new Padding(0, 0, 8, 0),
        };
        _parameters.Changed += (_, _) =>
        {
            if (_colorIndicator is not null)
            {
                _colorIndicator.BackColor = _parameters.StrokeColor;
            }
        };
        row.Controls.Add(_colorIndicator);

        row.Controls.Add(CreateSwatch("Black", Color.FromArgb(20, 20, 20)));
        row.Controls.Add(CreateSwatch("White", Color.White));
        row.Controls.Add(CreateSwatch("Blue", Color.FromArgb(0, 120, 215)));
        row.Controls.Add(CreateSwatch("Red", Color.FromArgb(220, 50, 50)));
        row.Controls.Add(CreateSwatch("Green", Color.FromArgb(40, 170, 90)));

        Button custom = new() { Text = "Custom...", AutoSize = true, Margin = new Padding(8, 0, 0, 0) };
        custom.Click += (_, _) =>
        {
            using ColorDialog dialog = new() { Color = _parameters.StrokeColor, FullOpen = true };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _parameters.StrokeColor = dialog.Color;
            }
        };
        row.Controls.Add(custom);

        return CreateCluster("Stroke color", row);
    }

    private Button CreateSwatch(string name, Color color)
    {
        Button swatch = new()
        {
            Width = 28,
            Height = 28,
            BackColor = color,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(0, 0, 4, 0),
            AccessibleName = $"{name} stroke color",
            TabStop = true,
        };
        swatch.FlatAppearance.BorderColor = Color.Gray;
        swatch.Click += (_, _) => _parameters.StrokeColor = color;
        return swatch;
    }

    private Control CreatePreviewGrid()
    {
        TableLayoutPanel grid = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = s_techniques.Length + 1,
            RowCount = s_themes.Length + 1,
            Padding = new Padding(6),
        };

        grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        foreach (RoundedRectangleTechnique _ in s_techniques)
        {
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / s_techniques.Length));
        }

        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        foreach (PreviewBackgroundTheme _ in s_themes)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / s_themes.Length));
        }

        grid.Controls.Add(new Label { AutoSize = true, Anchor = AnchorStyles.None, Text = string.Empty }, 0, 0);

        for (int column = 0; column < s_techniques.Length; column++)
        {
            grid.Controls.Add(CreateHeaderLabel(RoundedRectangleRenderer.GetCaption(s_techniques[column])), column + 1, 0);
        }

        for (int rowIndex = 0; rowIndex < s_themes.Length; rowIndex++)
        {
            grid.Controls.Add(CreateHeaderLabel(s_themes[rowIndex].ToString()), 0, rowIndex + 1);

            for (int column = 0; column < s_techniques.Length; column++)
            {
                grid.Controls.Add(
                    new RoundedRectanglePreviewPanel(s_techniques[column], s_themes[rowIndex], _parameters),
                    column + 1,
                    rowIndex + 1);
            }
        }

        return grid;
    }

    private static Label CreateHeaderLabel(string text) => new()
    {
        AutoSize = true,
        Anchor = AnchorStyles.None,
        TextAlign = ContentAlignment.MiddleCenter,
        Margin = new Padding(4),
        Text = text,
    };

    private static Control CreateCluster(string caption, params Control[] controls)
    {
        FlowLayoutPanel cluster = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = new Padding(6, 4, 14, 4),
        };

        cluster.Controls.Add(new Label { Text = caption, AutoSize = true, Margin = new Padding(0, 0, 0, 2) });

        FlowLayoutPanel row = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = Padding.Empty,
        };

        foreach (Control control in controls)
        {
            row.Controls.Add(control);
        }

        cluster.Controls.Add(row);
        return cluster;
    }

    private static TrackBar CreateTrackBar(int minimum, int maximum, int value) => new()
    {
        Minimum = minimum,
        Maximum = maximum,
        Value = Math.Clamp(value, minimum, maximum),
        TickStyle = TickStyle.None,
        AutoSize = false,
        Width = 160,
        Height = 34,
        Margin = new Padding(0, 0, 6, 0),
    };

    private static string FormatDip(float dip) => $"{dip:0} px";
}
