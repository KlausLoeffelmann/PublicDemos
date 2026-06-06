namespace LayoutTests.App.Models;

public sealed class ContainerParameters
{
    public DesignResolution DesignResolution { get; set; } = DesignResolution.VGA_640x480;
    public ScalePercent ScalePercent { get; set; } = ScalePercent.P100;
    public AutoScaleMode AutoScaleMode { get; set; } = AutoScaleMode.Font;
    public ScaleApplyPhase ApplyPhase { get; set; } = ScaleApplyPhase.InCtor;
    public string FontFamily { get; set; } = "Segoe UI";
    public float FontSizePt { get; set; } = 9f;
    public FontStyle FontStyle { get; set; } = FontStyle.Regular;

    public ContainerParameters Clone() => new()
    {
        DesignResolution = DesignResolution,
        ScalePercent = ScalePercent,
        AutoScaleMode = AutoScaleMode,
        ApplyPhase = ApplyPhase,
        FontFamily = FontFamily,
        FontSizePt = FontSizePt,
        FontStyle = FontStyle,
    };

    public static Size GetDesignSize(DesignResolution res) => res switch
    {
        DesignResolution.VGA_640x480 => new Size(640, 480),
        DesignResolution.SVGA_800x600 => new Size(800, 600),
        DesignResolution.WXGA_1280x800 => new Size(1280, 800),
        _ => new Size(640, 480),
    };
}
