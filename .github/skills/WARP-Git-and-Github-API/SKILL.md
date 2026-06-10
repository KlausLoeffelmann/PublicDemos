---
name: warp-github-git
description: Use this skill when working with WARP's GitHub and local Git APIs, including authenticated Octokit clients, GitHub repo list views, local repository inspection, branch metadata, and branch composition/cherry-pick workflows in WarpToolkit.WinForms.Github.
---

# WARP GitHub and Git APIs

This skill covers `WarpToolkit.WinForms.Github`, which currently contains two
related areas:

| Area | Entry points | Purpose |
|------|--------------|---------|
| GitHub API/auth | `IGitHubClientFactoryService`, `AddGitHubGcmClientServiceFactory`, `GitHubLoginManager`, `GitHubDeviceLogin`, `RepoListView` | Authenticated Octokit client creation and GitHub repository UI helpers. |
| Local Git APIs | `ILocalGitRepositoryService`, `IGitBranchCompositionService`, `AddGitServices` | Inspect local git repos, read branch/commit metadata, and compose target branches from selected source branches. |

## When to use this skill

- The user wants to inspect a local GitHub repository from a WinForms or CLI app.
- The user wants to list local/remote branches or show latest commit metadata.
- The user wants to compose a target branch by replaying/cherry-picking commits
  from one or more source branches.
- The user needs an authenticated `GitHubClient` through Git Credential Manager
  or the existing GitHub device-login flow.
- The user wants to display GitHub repositories in a WinForms `ListView`.

## Package / project reference

For apps that can use the package:

```xml
<PackageReference Include="WarpToolkit.WinForms.Github" Version="0.9.79-preview.g5a320ad311" />
```

If the API is still being evolved inside the WARP repo, reference the project:

```xml
<ProjectReference Include="..\..\..\warp\src\WarpToolkit.WinForms.Github\WarpToolkit.WinForms.Github.csproj" />
```

Adjust the relative path for the consuming project.

## DI registration

Register local Git services with:

```csharp
using WarpToolkit.WinForms.Github.Services;

builder.Services.AddGitServices();
```

This registers:

- `ILocalGitRepositoryService`
- `IGitBranchCompositionService`

Register the GitHub/Octokit client factory separately when needed:

```csharp
using WarpToolkit.WinForms.Github.Services;

builder.Services.AddGitHubGcmClientServiceFactory();
```

## Local repository inspection

Use `ILocalGitRepositoryService` for non-destructive repo/branch metadata:

```csharp
using WarpToolkit.WinForms.Github.Git;

ILocalGitRepositoryService git = new LocalGitRepositoryService();

LocalGitRepositoryInfo repo = await git.GetRepositoryInfoAsync(path);
IReadOnlyList<GitBranchInfo> branches = await git.GetBranchesAsync(repo.RootPath);
GitCommitInfo mainTip = await git.GetBranchTipAsync(repo.RootPath, repo.DefaultBranch ?? "main");
```

Important model types:

| Type | Purpose |
|------|---------|
| `LocalGitRepositoryInfo` | Root path, parsed origin remote, current branch, default branch. |
| `GitRemoteInfo` | Remote host, owner, repository name, URL, stable `RepositoryKey`. |
| `GitBranchInfo` | Branch name, local/remote flag, latest commit if available. |
| `GitCommitInfo` | SHA, abbreviated SHA, author date, committer date, subject, parent count. |

## Branch composition

Use `IGitBranchCompositionService` when creating an integration branch from
multiple source branches:

```csharp
using WarpToolkit.WinForms.Github.Git;

IGitBranchCompositionService composer =
    new GitBranchCompositionService(new LocalGitRepositoryService());

BranchCompositionResult result = await composer.ComposeAsync(new BranchCompositionRequest
{
    RepositoryPath = repo.RootPath,
    BaseBranch = "main",
    SourceBranches = ["feature-a", "feature-b"],
    TargetOptions = new BranchTargetOptions
    {
        BranchSetName = "release-test",
        TargetBranchName = "combined",
        NamingMode = TargetBranchNamingMode.NumberedSuffix,
        NumberWidth = 2,
        OverwriteExisting = false
    }
});
```

Composition behavior:

- Runs in a temporary `git worktree`; it does **not** mutate the user's active checkout.
- Replays commits sorted by author date, then committer date, then SHA.
- Rejects merge commits for now instead of guessing a mainline parent.
- Aborts and reports cherry-pick conflicts.
- Uses normal `git push` for new target branches.
- Uses `--force-with-lease=<ref>:<oldSha>` when overwriting an existing remote branch.

## Target branch naming

`BranchTargetOptions` always places the target branch under the branch-set folder:

| `TargetBranchNamingMode` | Shape |
|--------------------------|-------|
| `Fixed` | `<branch-set>/<target-name>` |
| `DateFolder` | `<branch-set>/<yyyy-MM-dd>/<target-name>` |
| `NumberedSuffix` | `<branch-set>/<target-name>-01` or `<branch-set>/<target-name>-001` |

Call `ResolveTargetBranchNameAsync` when you only need the computed name:

```csharp
string target = await composer.ResolveTargetBranchNameAsync(
    repo.RootPath,
    options);
```

## Error handling

Catch the specific exception types first:

```csharp
try
{
    BranchCompositionResult result = await composer.ComposeAsync(request);
}
catch (BranchCompositionException ex)
{
    // ex.Commit may identify the failed commit.
    // ex.ConflictedFiles lists unresolved paths for cherry-pick conflicts.
}
catch (GitCommandException ex)
{
    // ex.ExitCode, ex.StandardOutput, and ex.StandardError contain git details.
}
```

Do not swallow these exceptions. Surface the git stderr/stdout or conflicted
files to the user so they can fix the repository state or branch selection.

## GitHub client APIs

Use `IGitHubClientFactoryService.GetClient()` when a feature needs Octokit:

```csharp
using Octokit;
using WarpToolkit.WinForms.Github.Services;

GitHubClient client = gitHubClientFactory.GetClient();
IReadOnlyList<Repository> repos = await client.Repository.GetAllForCurrent();
```

Current auth options:

- `AddGitHubGcmClientServiceFactory()` reads the token from Git Credential Manager.
- `GitHubLoginManager.GetClientAsync()` uses the existing device-code login flow.

For local Git operations such as push/fetch/cherry-pick, prefer the local Git
services above. They shell out to `git`, so they reuse the user's normal Git
authentication setup, including SSH keys, GCM, or `gh auth setup-git`.

## Anti-patterns to avoid

- Do not compose branches in the user's active working tree. Use
  `IGitBranchCompositionService`, which isolates work in a temporary worktree.
- Do not use unconditional force-push for overwrite flows. Preserve the
  service's `--force-with-lease` behavior.
- Do not try to auto-resolve cherry-pick conflicts in v1. Report the failed
  commit and conflicted files.
- Do not assume every local git remote is GitHub.com. `GitRemoteInfo` can parse
  GitHub Enterprise-style hosts; local git workflows should still work without
  Octokit.

