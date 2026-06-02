using WingetPackageEditor.Core.Models;
using WingetPackageEditor.Core.Services;

namespace Winget_Package_Editor;

/// <summary>
///  WinForms implementation of <see cref="IPackageEditorDialogService"/>. Dialogs are owned by the
///  currently active form so they center over the main window.
/// </summary>
internal sealed class WinFormsPackageEditorDialogService : IPackageEditorDialogService
{
    public NewFromExistingResult? AskNewFromExisting(IReadOnlyList<WingetPackage> existingPackages)
    {
        ArgumentNullException.ThrowIfNull(existingPackages);

        using NewFromExistingDialog dialog = new(existingPackages);
        if (dialog.ShowDialog(Form.ActiveForm) != DialogResult.OK || dialog.SourcePackage is null)
        {
            return null;
        }

        return new NewFromExistingResult(dialog.NewName, dialog.SourcePackage);
    }

    public bool ConfirmRemovePackage(string packageName)
    {
        return MessageBox.Show(
            Form.ActiveForm,
            $"Remove package '{packageName}'?\r\n\r\nA backup will be written to the AppData backups folder.",
            "Remove package",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2) == DialogResult.Yes;
    }

    public AppEntry? PickAndConfigureApp(IReadOnlyList<AppEntry> wellKnownApps)
    {
        ArgumentNullException.ThrowIfNull(wellKnownApps);

        using AddAppDialog dialog = new(wellKnownApps);
        return dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK ? dialog.Result : null;
    }
}
