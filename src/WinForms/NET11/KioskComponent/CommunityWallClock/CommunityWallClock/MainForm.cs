using WarpToolkit.WinForms.Extensions.UI;

namespace CommunityWallClock
{
    public partial class MainForm : Form, IServiceProvider
    {
        private static readonly string SettingsKey_MainFormBounds
            = nameof(SettingsKey_MainFormBounds);

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Bounds = _userSettingsService.Get(
                key: SettingsKey_MainFormBounds,
                defaultValue: this.CenterToScreen(
                    horizontalFillGrade: 70,
                    verticalFillGrade: 70));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            _userSettingsService.Set(
                key: SettingsKey_MainFormBounds,
                value: Bounds);

            _userSettingsService.Flush();
        }
    }
}
