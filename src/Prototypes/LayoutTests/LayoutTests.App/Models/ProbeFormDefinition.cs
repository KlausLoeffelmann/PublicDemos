namespace LayoutTests.App.Models;

public sealed class ProbeFormDefinition
{
    public string Title { get; set; } = "Probe Carrier";
    public Size InitialClientSize { get; set; } = new(900, 700);
    public string FontFamily { get; set; } = "Segoe UI";
    public float FontSizePt { get; set; } = 9f;
    public FontStyle FontStyle { get; set; } = FontStyle.Regular;
    public AutoScaleMode AutoScaleMode { get; set; } = AutoScaleMode.Font;
}
