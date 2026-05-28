using System.Text.Json.Serialization;

namespace WingetPackageEditor.Core.Models;

public sealed class WingetPackage
{
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public string? Author { get; set; }

    public string Version { get; set; } = "1.0.0";

    public List<AppEntry> Apps { get; set; } = [];
}

public enum AppAction { Ensure, Install, Upgrade }

public enum AppScope { User, Machine }

public enum AppSource { Winget, MSStore }

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(GenericAppEntry), "generic")]
[JsonDerivedType(typeof(VisualStudioEntry), "vs")]
[JsonDerivedType(typeof(VSCodeEntry), "vscode")]
public abstract class AppEntry
{
    public string Id { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public AppAction Action { get; set; } = AppAction.Ensure;

    public AppSource Source { get; set; } = AppSource.Winget;

    public string? Version { get; set; }

    public AppScope Scope { get; set; } = AppScope.Machine;

    public bool AllowPrerelease { get; set; }

    public Dictionary<string, string> ExtraSettings { get; set; } = [];
}

public sealed class GenericAppEntry : AppEntry;

public sealed class VisualStudioEntry : AppEntry
{
    public VSEdition Edition { get; set; }

    public VSChannel Channel { get; set; }

    public string? VSConfigPath { get; set; }

    public string? VSConfigInline { get; set; }

    public string? InstanceNickname { get; set; }

    public List<VsixReference> Extensions { get; set; } = [];
}

public sealed class VsixReference
{
    public string Identifier { get; set; } = "";

    public bool Admin { get; set; } = true;
}

public sealed class VSCodeEntry : AppEntry
{
    public List<string> Extensions { get; set; } = [];
}

public enum VSEdition { Community, Professional, Enterprise, BuildTools }

public enum VSChannel { Release, Preview }
