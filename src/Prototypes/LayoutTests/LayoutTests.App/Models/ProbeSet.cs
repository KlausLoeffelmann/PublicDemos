namespace LayoutTests.App.Models;

public sealed class ProbeSet
{
    public string Name { get; set; } = "Untitled";
    public ProbeFormDefinition Form { get; set; } = new();
    public List<ContainerDefinition> Roots { get; set; } = new();
}
