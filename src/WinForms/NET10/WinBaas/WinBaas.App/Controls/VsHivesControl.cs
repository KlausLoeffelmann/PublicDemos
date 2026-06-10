using System.Diagnostics;
using WinBaas.Models;

namespace WinBaas.Controls;

/// <summary>
///  The Visual Studio hive list with clickable paths and copy buttons.
/// </summary>
public sealed partial class VsHivesControl : UserControl
{
    public VsHivesControl()
    {
        InitializeComponent();
    }

    /// <summary>Raised when hive interaction should update the status strip text.</summary>
    public event EventHandler<string>? StatusTextChanged;

    /// <summary>Replaces the displayed hive list.</summary>
    public void SetItems(IEnumerable<VsHive> hives)
    {
        ArgumentNullException.ThrowIfNull(hives);

        while (_table.RowCount > 1)
        {
            int rowIndex = _table.RowCount - 1;
            for (int col = 0; col < _table.ColumnCount; col++)
            {
                Control? control = _table.GetControlFromPosition(col, rowIndex);
                if (control is not null)
                {
                    _table.Controls.Remove(control);
                    control.Dispose();
                }
            }

            _table.RowStyles.RemoveAt(rowIndex);
            _table.RowCount--;
        }

        foreach (VsHive hive in hives)
        {
            int row = _table.RowCount;
            _table.RowCount++;
            _table.RowStyles.Add(new RowStyle());

            var label = new Label
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Margin = new Padding(3, 6, 12, 6),
                Text = hive.Name,
            };

            var link = new LinkLabel
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = true,
                Margin = new Padding(3, 6, 12, 6),
                Text = hive.FullPath,
                Tag = hive.FullPath,
            };
            link.LinkClicked += Link_LinkClicked;

            var button = new Button
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Margin = new Padding(3, 3, 3, 3),
                Text = "Copy",
                Tag = hive.FullPath,
            };
            button.Click += CopyButton_Click;

            _table.Controls.Add(label, 0, row);
            _table.Controls.Add(link, 1, row);
            _table.Controls.Add(button, 2, row);
        }
    }

    private void Link_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (sender is not LinkLabel { Tag: string path })
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
            });
            StatusTextChanged?.Invoke(this, path);
        }
        catch (Exception)
        {
            StatusTextChanged?.Invoke(this, $"Could not open {path}");
        }
    }

    private void CopyButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button { Tag: string path })
        {
            return;
        }

        try
        {
            Clipboard.SetText(path);
            StatusTextChanged?.Invoke(this, path);
        }
        catch (Exception)
        {
            StatusTextChanged?.Invoke(this, $"Could not copy {path}");
        }
    }
}
