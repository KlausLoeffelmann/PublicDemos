using WarpToolkit.WinForms.Github.Git;

namespace BranchComposer.App.Models;

public sealed class AppState
{
    public List<RepositoryEntry> Repositories { get; set; } = [];

    public Dictionary<string, List<BranchSetDefinition>> BranchSetsByRepository { get; set; } = [];

    public string? LastSelectedRepositoryKey { get; set; }

    public string? LastSelectedBranchSetName { get; set; }
}

public sealed class RepositoryEntry
{
    public string RootPath { get; set; } = string.Empty;

    public string RepositoryKey { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string RemoteUrl { get; set; } = string.Empty;

    public string? DefaultBranch { get; set; }

    public string Key => !string.IsNullOrWhiteSpace(RepositoryKey) ? RepositoryKey : RootPath;
}

public sealed class BranchSetDefinition
{
    public string Name { get; set; } = string.Empty;

    public string RepositoryKey { get; set; } = string.Empty;

    public string BaseBranch { get; set; } = string.Empty;

    public List<string> SourceBranches { get; set; } = [];

    public string TargetBranchName { get; set; } = string.Empty;

    public TargetBranchNamingMode NamingMode { get; set; }

    public int NumberWidth { get; set; } = 2;

    public bool OverwriteExisting { get; set; }
}

