using WarpToolkit.WinForms.Typography.Controls;

namespace WinBaas.Dialogs;

/// <summary>
///  Modeless dialog that renders a WinBaas backup log Markdown file using
///  WARP's <see cref="MarkdownRenderControl"/>.
/// </summary>
public sealed class DlgReportViewer : Form
{
    private readonly MarkdownRenderControl _renderer;
    private readonly StatusStrip _status;
    private readonly ToolStripStatusLabel _pathLabel;
    private readonly ToolStripStatusLabel _spring;
    private readonly ToolStripDropDownButton _actions;

    public DlgReportViewer(string reportPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportPath);

        Font = new Font("Segoe UI", 11F);
        Text = $"WinBaas \u2013 Backup report ({Path.GetFileName(reportPath)})";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(900, 700);
        MinimumSize = new Size(560, 360);

        _renderer = new MarkdownRenderControl
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
        };

        _status = new StatusStrip
        {
            Dock = DockStyle.Bottom,
            Font = new Font("Segoe UI", 10F),
        };
        _pathLabel = new ToolStripStatusLabel(reportPath)
        {
            Spring = false,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        _spring = new ToolStripStatusLabel { Spring = true };
        _actions = new ToolStripDropDownButton("Actions");
        _actions.DropDownItems.Add("Open containing folder", null, (_, _) => OpenFolder(reportPath));
        _actions.DropDownItems.Add("Copy report path", null, (_, _) => Clipboard.SetText(reportPath));
        _status.Items.AddRange(new ToolStripItem[] { _pathLabel, _spring, _actions });

        Controls.Add(_renderer);
        Controls.Add(_status);

        try
        {
            _renderer.MarkdownText = File.ReadAllText(reportPath);
        }
        catch (Exception ex)
        {
            _renderer.MarkdownText = $"# Could not load report\n\n```\n{ex.Message}\n```";
        }
    }

    private static void OpenFolder(string reportPath)
    {
        try
        {
            string args = $"/select,\"{reportPath}\"";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer.exe", args)
            {
                UseShellExecute = true,
            });
        }
        catch
        {
            // best-effort; ignore failures
        }
    }
}
