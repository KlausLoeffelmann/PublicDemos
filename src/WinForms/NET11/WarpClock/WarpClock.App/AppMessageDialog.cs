namespace WarpClock.App;

public partial class AppMessageDialog : Form
{
    public AppMessageDialog(string title, string headline, string message)
    {
        InitializeComponent();

        Text = title;
        _headlineLabel.Text = headline;
        _messageTextBox.Text = message;
    }

    public static DialogResult ShowMessage(
        IWin32Window? owner,
        string title,
        string headline,
        string message)
    {
        using AppMessageDialog dialog = new(title, headline, message);
        return owner is null
            ? dialog.ShowDialog()
            : dialog.ShowDialog(owner);
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }
}
