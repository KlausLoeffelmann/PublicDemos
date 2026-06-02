using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Result of the "New from existing package" dialog: the chosen new name and the source package
///  whose definition should be cloned.
/// </summary>
public sealed record NewFromExistingResult(string NewName, WingetPackage SourcePackage);

/// <summary>
///  Abstracts the modal dialogs the editor needs so view-model logic stays UI-free and testable.
/// </summary>
public interface IPackageEditorDialogService
{
    /// <summary>
    ///  Prompts for a new package name and a source package to clone. Returns <see langword="null"/>
    ///  when the user cancels.
    /// </summary>
    NewFromExistingResult? AskNewFromExisting(IReadOnlyList<WingetPackage> existingPackages);

    /// <summary>
    ///  Asks the user to confirm removal of the named package.
    /// </summary>
    bool ConfirmRemovePackage(string packageName);

    /// <summary>
    ///  Lets the user pick a well-known app and configure how it installs. Returns a configured
    ///  <see cref="AppEntry"/>, or <see langword="null"/> when cancelled.
    /// </summary>
    AppEntry? PickAndConfigureApp(IReadOnlyList<AppEntry> wellKnownApps);
}
