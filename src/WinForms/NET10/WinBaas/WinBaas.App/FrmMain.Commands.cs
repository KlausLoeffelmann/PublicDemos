using Microsoft.Extensions.Logging;
using WarpToolkit.WinForms.Extensions.UI;
using WarpToolkit.WinForms.Symbols;
using WinBaas.Dialogs;
using WinBaas.Models;
using WinBaas.Services;

namespace WinBaas;

/// <summary>
///  Wires the MenuStrip and ToolStrip commands to icons (via WARP's
///  <see cref="ToolStripExtensions.ConfigureItem"/>) and click handlers.
/// </summary>
public sealed partial class FrmMain
{
    private CancellationTokenSource? _discoveryCts;
    private bool _discovering;

    /// <summary>
    ///  Called from <c>OnLoad</c> after <see cref="InitializeComponent"/> has run
    ///  and each <see cref="ToolStripItem"/> has its <c>Owner</c> set, per the
    ///  WARP <see cref="ToolStripExtensions.ConfigureItem"/> contract.
    /// </summary>
    private void ConfigureCommands()
    {
        _tsbDiscover.ConfigureItem(
            symbol: FluentSymbols.AllSymbols.Refresh,
            eventHandler: (DiscoverCommand, removeBeforeAdd: true),
            tooltipText: "Discover objects to backup");

        _tsbBackup.ConfigureItem(
            symbol: FluentSymbols.AllSymbols.Save,
            eventHandler: (BackupCommand, removeBeforeAdd: true),
            tooltipText: "Backup selected objects");

        _tsbAdd.ConfigureItem(
            symbol: FluentSymbols.AllSymbols.AddBold,
            eventHandler: (AddObjectCommand, removeBeforeAdd: true),
            tooltipText: "Add object\u2026");

        _tsbDelete.ConfigureItem(
            symbol: FluentSymbols.AllSymbols.Delete,
            eventHandler: (DeleteObjectCommand, removeBeforeAdd: true),
            tooltipText: "Delete object\u2026");

        _tsbOptions.ConfigureItem(
            symbol: FluentSymbols.AllSymbols.Settings,
            eventHandler: (OptionsCommand, removeBeforeAdd: true),
            tooltipText: "Options\u2026");

        _menuFileDiscover.Click += DiscoverCommand;
        _menuFileBackup.Click += BackupCommand;
        _menuCatalogAdd.Click += AddObjectCommand;
        _menuCatalogDelete.Click += DeleteObjectCommand;
        _menuCatalogRestore.Click += RestoreDefaultsCommand;
        _menuToolsOptions.Click += OptionsCommand;

        UpdateCommandStates();
    }

    private void UpdateCommandStates()
    {
        bool hasSelection = _nodeItems.Values.Any(items => items.Any(i => i.IsChecked));
        _tsbBackup.Enabled = !_discovering && hasSelection;
        _menuFileBackup.Enabled = _tsbBackup.Enabled;
        _tsbDiscover.Enabled = !_discovering;
        _menuFileDiscover.Enabled = !_discovering;

        bool userDefinedSelected =
            _treeSources.SelectedNode?.Tag is CatalogEntry entry && entry.IsUserDefined;
        _menuCatalogDelete.Enabled = userDefinedSelected;
        _tsbDelete.Enabled = userDefinedSelected;
    }

    private async void DiscoverCommand(object? sender, EventArgs e)
    {
        if (_discovering)
        {
            return;
        }

        _discovering = true;
        _statusProgress.Visible = true;
        _statusProgress.Style = ProgressBarStyle.Marquee;
        _statusInfo.Text = "Discovering\u2026";
        UpdateCommandStates();

        _discoveryCts?.Dispose();
        _discoveryCts = new CancellationTokenSource();
        CancellationToken ct = _discoveryCts.Token;

        try
        {
            await Task.Yield();
            foreach (TreeNode leaf in EnumerateLeafNodes(_treeSources.Nodes).ToArray())
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                if (leaf.Tag is not CatalogEntry entry)
                {
                    continue;
                }

                _statusInfo.Text = $"Discovering {entry.Name}\u2026";
                IReadOnlyList<DiscoveredItem> items = await _discovery.DiscoverAsync(entry, ct);
                _nodeItems[leaf] = [.. items];

                leaf.Text = items.Count == 0 ? entry.Name : $"{entry.Name} ({items.Count})";
                leaf.ForeColor = items.Count == 0 ? SystemColors.GrayText : SystemColors.ControlText;
            }

            _statusInfo.Text = $"Discovered {_nodeItems.Values.Sum(v => v.Count)} item(s).";
            _logger.LogInformation("Discovery complete: {Count} item(s).",
                _nodeItems.Values.Sum(v => v.Count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Discovery failed.");
            _statusInfo.Text = $"Discovery failed: {ex.Message}";
        }
        finally
        {
            _discovering = false;
            _statusProgress.Visible = false;
            _statusProgress.Style = ProgressBarStyle.Continuous;
            UpdateCommandStates();
        }
    }

    private async void BackupCommand(object? sender, EventArgs e)
    {
        var selected = _nodeItems.Values
            .SelectMany(list => list)
            .Where(item => item.IsChecked)
            .ToList();

        if (selected.Count == 0)
        {
            return;
        }

        var options = ChooseBackupDestination();
        if (options is null)
        {
            return;
        }

        _statusInfo.Text = $"Backing up {selected.Count} item(s)\u2026";
        _statusProgress.Visible = true;
        _statusProgress.Style = ProgressBarStyle.Marquee;
        try
        {
            var progress = new Progress<string>(msg => _statusInfo.Text = msg);
            BackupResult result = await _backup.BackupAsync(selected, options, progress);
            _statusInfo.Text = $"Backup complete: {result.FinalDestination}";
            _logger.LogInformation(
                "Backup written to {Destination}. Report: {ReportPath}",
                result.FinalDestination,
                result.ReportPath);

            var viewer = new DlgReportViewer(result.ReportPath) { Owner = this };
            viewer.Show(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup failed.");
            _statusInfo.Text = $"Backup failed: {ex.Message}";
        }
        finally
        {
            _statusProgress.Visible = false;
            _statusProgress.Style = ProgressBarStyle.Continuous;
        }
    }

    private BackupOptions? ChooseBackupDestination()
    {
        var stored = _settings.Get<BackupMode>("WinBaas.BackupMode", BackupMode.CopyToFolder);
        if (stored == BackupMode.ZipArchive)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Select root folder for the WinBaas .zip backup",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
            };
            return dlg.ShowDialog(this) == DialogResult.OK
                ? new BackupOptions { Mode = BackupMode.ZipArchive, Destination = dlg.SelectedPath }
                : null;
        }

        using var folderDlg = new FolderBrowserDialog
        {
            Description = "Select destination folder for the WinBaas backup",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true,
        };
        return folderDlg.ShowDialog(this) == DialogResult.OK
            ? new BackupOptions { Mode = BackupMode.CopyToFolder, Destination = folderDlg.SelectedPath }
            : null;
    }

    private void AddObjectCommand(object? sender, EventArgs e)
    {
        using var dialog = new DlgAddObject();
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Result is CatalogEntry entry)
        {
            _catalog.Add(entry);
            PopulateSourceTree();
        }
    }

    private void DeleteObjectCommand(object? sender, EventArgs e)
    {
        if (_treeSources.SelectedNode?.Tag is not CatalogEntry entry || !entry.IsUserDefined)
        {
            return;
        }

        if (MessageBox.Show(this,
                $"Delete user-defined catalog entry '{entry.Name}'?",
                "WinBaas",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        _catalog.Remove(entry.Id);
        PopulateSourceTree();
    }

    private void RestoreDefaultsCommand(object? sender, EventArgs e)
    {
        if (MessageBox.Show(this,
                "Discard all user-defined entries and restore the built-in catalog?",
                "WinBaas",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        _catalog.RestoreDefaults();
        PopulateSourceTree();
    }

    private void OptionsCommand(object? sender, EventArgs e)
    {
        using var dialog = new DlgOptions(_settings);
        dialog.ShowDialog(this);
    }
}
