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
        root.Controls.Add(CreatePreviewFlows(), 0, 2);

        Controls.Add(root);

        ResumeLayout(true);
    }

    private static Label CreateDescriptionLabel() => new()
    {
        Dock = DockStyle.Fill,
        AutoSize = true,
        Padding = new Padding(8, 8, 8, 2),
        Text = "GDI+ rounded-rectangle anti-aliasing prototype. The top row shows how a rounded "
            + "rectangle is drawn today (the built-in Graphics.DrawRoundedRectangle and the equivalent "
            + "manual arc path); the bottom row shows improved techniques. Tune the parameters and "
            + "watch how each technique's corner arcs blend with the straight edges.",
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
        TrackBar radius = CreateTrackBar(0, 80, (int)_parameters.CornerRadius);
        radius.ValueChanged += (_, _) =>
        {
            _parameters.CornerRadius = radius.Value;
            radiusValue.Text = FormatDip(radius.Value);
        };
        strip.Controls.Add(CreateCluster("Size (corner radius)", radius, radiusValue));

        Label widthValue = new() { AutoSize = true, Text = FormatDip(_parameters.RectWidth) };
        TrackBar width = CreateTrackBar(40, 400, (int)_parameters.RectWidth);
        width.ValueChanged += (_, _) =>
        {
            _parameters.RectWidth = width.Value;
            widthValue.Text = FormatDip(width.Value);
        };
        strip.Controls.Add(CreateCluster("Width", width, widthValue));

        Label heightValue = new() { AutoSize = true, Text = FormatDip(_parameters.RectHeight) };
        TrackBar height = CreateTrackBar(40, 300, (int)_parameters.RectHeight);
        height.ValueChanged += (_, _) =>
        {
            _parameters.RectHeight = height.Value;
            heightValue.Text = FormatDip(height.Value);
        };
        strip.Controls.Add(CreateCluster("Height", height, heightValue));

        Label thicknessValue = new() { AutoSize = true, Text = FormatDip(_parameters.BorderThickness) };
        TrackBar thickness = CreateTrackBar(1, 12, (int)_parameters.BorderThickness);
        thickness.ValueChanged += (_, _) =>
        {
            _parameters.BorderThickness = thickness.Value;
            thicknessValue.Text = FormatDip(thickness.Value);
        };
        strip.Controls.Add(CreateCluster("Border thickness", thickness, thicknessValue));

        CheckBox antiAlias = new() { Text = "Anti-alias", AutoSize = true, Checked = _parameters.AntiAliasEnabled };
        antiAlias.CheckedChanged += (_, _) => _parameters.AntiAliasEnabled = antiAlias.Checked;
        strip.Controls.Add(CreateCluster("Smoothing", antiAlias));

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
        strip.Controls.Add(CreateCluster("SSAA factor", ssaa));

        ComboBox background = new()
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 110,
        };
        background.Items.AddRange([.. Enum.GetNames<PreviewBackgroundTheme>()]);
        background.SelectedItem = _parameters.BackgroundTheme.ToString();
        background.SelectedIndexChanged += (_, _) =>
        {
            if (background.SelectedItem is string name && Enum.TryParse(name, out PreviewBackgroundTheme theme))
            {
                _parameters.BackgroundTheme = theme;
            }
        };
        strip.Controls.Add(CreateCluster("Background", background));

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

    private Control CreatePreviewFlows()
    {
        TableLayoutPanel groups = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(6),
        };
        groups.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        groups.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
        groups.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

        RoundedRectangleTechnique[] current = [.. s_techniques.Where(RoundedRectangleRenderer.IsCurrentTechnique)];
        RoundedRectangleTechnique[] improved = [.. s_techniques.Where(technique => !RoundedRectangleRenderer.IsCurrentTechnique(technique))];

        groups.Controls.Add(CreateTechniqueGroup("Current / standard ways", current), 0, 0);
        groups.Controls.Add(CreateTechniqueGroup("Improved ways", improved), 0, 1);

        return groups;
    }

    private Control CreateTechniqueGroup(string caption, RoundedRectangleTechnique[] techniques)
    {
        GroupBox group = new()
        {
            Text = caption,
            Dock = DockStyle.Fill,
            Padding = new Padding(6),
        };

        FlowLayoutPanel flow = new()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight,
        };

        foreach (RoundedRectangleTechnique technique in techniques)
        {
            flow.Controls.Add(new RoundedRectanglePreviewPanel(technique, _parameters));
        }

        group.Controls.Add(flow);
        return group;
    }

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
