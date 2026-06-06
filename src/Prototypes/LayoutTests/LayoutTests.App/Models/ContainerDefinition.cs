namespace LayoutTests.App.Models;

public sealed class ContainerDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Container";
    public ContainerKind Kind { get; set; } = ContainerKind.CTor;
    public ContainerParameters Parameters { get; set; } = new();
    public List<ContainerDefinition> Children { get; set; } = new();
}
