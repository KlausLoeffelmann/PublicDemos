using LayoutTests.App.Models;

namespace LayoutTests.App.Designer;

public partial class AddContainerDialog : Form
{
    public AddContainerDialog()
    {
        InitializeComponent();
        ctorRadio.Checked = true;
        nameTextBox.Text = "Container";
    }

    public ContainerKind Kind => lazyRadio.Checked ? ContainerKind.Lazy : ContainerKind.CTor;

    public string ContainerName => string.IsNullOrWhiteSpace(nameTextBox.Text) ? "Container" : nameTextBox.Text.Trim();
}
