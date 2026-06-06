using System.ComponentModel;
using LayoutTests.App.Models;
using LayoutTests.App.Services;

namespace LayoutTests.App.Carrier;

public class CarrierContainerBase : UserControl
{
    private static UselessFacts? s_facts;

    private ContainerParameters _parameters = new();
    private string _displayName = "Container";
    private bool _parametersApplied;

    public CarrierContainerBase()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw, true);
        Padding = new Padding(8, 86, 8, 8);
        BorderStyle = BorderStyle.FixedSingle;
        BackColor = SystemColors.Window;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new AutoScaleMode AutoScaleMode
    {
        get => base.AutoScaleMode;
        set => base.AutoScaleMode = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new SizeF AutoScaleDimensions
    {
        get => base.AutoScaleDimensions;
        set => base.AutoScaleDimensions = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ContainerParameters Parameters => _parameters;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DisplayName
    {
        get => _displayName;
        set => _displayName = value ?? string.Empty;
    }

    /// <summary>
    ///  Stashes the parameters and display name. When <see cref="ContainerParameters.ApplyPhase"/>
    ///  is <see cref="ScaleApplyPhase.InCtor"/>, <see cref="Apply"/> is invoked immediately;
    ///  otherwise it is deferred to the first <see cref="OnLoad"/>.
    /// </summary>
    public void Configure(ContainerParameters parameters, string displayName)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        _parameters = parameters.Clone();
        _displayName = displayName ?? string.Empty;
        _parametersApplied = false;

        if (_parameters.ApplyPhase == ScaleApplyPhase.InCtor)
        {
            Apply(_parameters);
        }
    }

    public void Apply(ContainerParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        _parameters = parameters.Clone();

        Font = new Font(parameters.FontFamily, parameters.FontSizePt, parameters.FontStyle);
        AutoScaleDimensions = ComputeAutoScaleDimensions(parameters);
        AutoScaleMode = parameters.AutoScaleMode;
        Size = ContainerParameters.GetDesignSize(parameters.DesignResolution);
        _parametersApplied = true;
        Invalidate();
    }

    protected static UselessFacts Facts => s_facts ??= new UselessFacts();

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_parameters.ApplyPhase == ScaleApplyPhase.AfterOnLoad && !_parametersApplied)
        {
            Apply(_parameters);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        PaintParameterOverlay(e.Graphics);
    }

    private void PaintParameterOverlay(Graphics g)
    {
        const int overlayHeight = 78;
        Rectangle rect = new(0, 0, Width, overlayHeight);
        using var bg = new SolidBrush(Color.FromArgb(220, 32, 32, 32));
        g.FillRectangle(bg, rect);

        using var titleFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        using var bodyFont = new Font("Segoe UI", 8.25F, FontStyle.Regular);

        TextRenderer.DrawText(
            g,
            $"{_displayName}  ({(_parametersApplied ? "Applied" : "PENDING")})",
            titleFont,
            new Point(8, 6),
            Color.White);

        string line1 = $"Design: {ContainerParameters.GetDesignSize(_parameters.DesignResolution).Width}×{ContainerParameters.GetDesignSize(_parameters.DesignResolution).Height}   " +
                       $"Scale: {(int)_parameters.ScalePercent}%   " +
                       $"Mode: {_parameters.AutoScaleMode}   " +
                       $"Phase: {_parameters.ApplyPhase}";

        string line2 = $"Font: {_parameters.FontFamily} {_parameters.FontSizePt:0.##}pt {_parameters.FontStyle}";

        string line3 = $"Actual: {Width}×{Height}   AutoScaleDims: {AutoScaleDimensions.Width:0.##}, {AutoScaleDimensions.Height:0.##}   DeviceDpi: {DeviceDpi}";

        TextRenderer.DrawText(g, line1, bodyFont, new Point(8, 26), Color.White);
        TextRenderer.DrawText(g, line2, bodyFont, new Point(8, 42), Color.White);
        TextRenderer.DrawText(g, line3, bodyFont, new Point(8, 58), Color.LightGreen);
    }

    private static SizeF ComputeAutoScaleDimensions(ContainerParameters p)
    {
        const float baseWidth = 7F;
        const float baseHeight = 15F;
        float factor = (int)p.ScalePercent / 100F;
        return new SizeF(baseWidth * factor, baseHeight * factor);
    }
}
