using System.Runtime.ExceptionServices;
using System.Reflection;
using System.Windows.Forms;
using DrumMachine.Demo;
using WinForms.Audio.Percussion;
using WinForms.Audio.WinForms;

namespace SplitFlap.Tests;

[CollectionDefinition("Rhythm UI", DisableParallelization = true)]
public sealed class RhythmUiCollection
{
}

[Collection("Rhythm UI")]
public sealed class DrumDemoLayoutTests
{
    [Fact]
    public void Constructor_IsDeviceIndependentAndContainsTheCompleteEditor()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            AudioSpectrumControl spectrum = Find<AudioSpectrumControl>(form, "_spectrumControl");

            Assert.Equal(AutoScaleMode.Dpi, form.AutoScaleMode);
            Assert.Equal(34, grid.Columns.Count);
            Assert.Equal(18, grid.Columns.Cast<DataGridViewColumn>().Count(column => column.Visible));
            Assert.Equal(13, grid.Rows.Count);
            Assert.Null(spectrum.Source);
            Assert.True(grid.Enabled);
            Assert.False(FindItem<ToolStripButton>(form, "_playButton").Enabled);
            Assert.Equal(2, FindItem<ToolStripComboBox>(form, "_barSelector").Items.Count);
        });

    [Fact]
    public void SelectingABarChangesTheViewWithoutOpeningAudio()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            DataGridViewRow kick = grid.Rows.Cast<DataGridViewRow>()
                .Single(row => row.Tag is Cr78Instrument.BassDrum);
            Assert.Equal(false, kick.Cells["_step08"].Value);

            FindItem<ToolStripComboBox>(form, "_barSelector").SelectedIndex = 1;
            kick = grid.Rows.Cast<DataGridViewRow>().Single(row => row.Tag is Cr78Instrument.BassDrum);
            Assert.Equal(true, kick.Cells["_step08"].Value);
            Assert.Null(Find<AudioSpectrumControl>(form, "_spectrumControl").Source);
        });

    [Fact]
    public void LayoutAndEmptySpectrumSurviveResize()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            TableLayoutPanel layout = Find<TableLayoutPanel>(form, "_layout");
            AudioSpectrumControl spectrum = Find<AudioSpectrumControl>(form, "_spectrumControl");
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");

            foreach (System.Drawing.Size size in new[]
            {
                new System.Drawing.Size(800, 620),
                new System.Drawing.Size(1100, 820),
                new System.Drawing.Size(1600, 1000)
            })
            {
                form.ClientSize = size;
                form.PerformLayout();
                layout.PerformLayout();
                Assert.True(spectrum.Width > 0 && spectrum.Height > 0);
                Assert.True(grid.Width > 0 && grid.Height > 0);
                Assert.True(spectrum.Right <= layout.ClientSize.Width);
                Assert.True(grid.Right <= layout.ClientSize.Width);
                using System.Drawing.Bitmap image = new(spectrum.Width, spectrum.Height);
                spectrum.DrawToBitmap(image, spectrum.ClientRectangle);
            }
        });

    [Fact]
    public void MenusAndView_DoNotChangeTheDocument()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            MenuStrip menu = Find<MenuStrip>(form, "_menuStrip");
            Assert.Equal(["&File", "&Edit", "&View", "&Tools"], menu.Items.Cast<ToolStripItem>().Select(item => item.Text));
            ToolStripMenuItem view = Assert.IsType<ToolStripMenuItem>(menu.Items[2]);
            Assert.IsType<ToolStripMenuItem>(view.DropDownItems[1]).PerformClick();

            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            Assert.Equal(34, grid.Columns.Cast<DataGridViewColumn>().Count(column => column.Visible));
            Assert.Single(FindItem<ToolStripComboBox>(form, "_barSelector").Items.Cast<object>());
            Assert.DoesNotContain("*", form.Text);
            Assert.False(FindItem<ToolStripButton>(form, "_playButton").Enabled);
        });

    [Fact]
    public void ScoreEdits_AreUndoableWithoutAnAudioDevice()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            DataGridViewRow kick = grid.Rows.Cast<DataGridViewRow>().Single(row => row.Tag is Cr78Instrument.BassDrum);
            kick.Cells["_step01"].Value = false;
            Assert.Contains("*", form.Text);

            MenuStrip menu = Find<MenuStrip>(form, "_menuStrip");
            ToolStripMenuItem edit = Assert.IsType<ToolStripMenuItem>(menu.Items[1]);
            ToolStripMenuItem undo = Assert.IsType<ToolStripMenuItem>(edit.DropDownItems[2]);
            Assert.True(undo.Enabled);
            undo.PerformClick();
            kick = grid.Rows.Cast<DataGridViewRow>().Single(row => row.Tag is Cr78Instrument.BassDrum);
            Assert.Equal(true, kick.Cells["_step01"].Value);
            Assert.DoesNotContain("*", form.Text);
        });

    [Fact]
    public void NewDialog_OffersExactlyTheRequestedLengths()
        => OnStaThread(() =>
        {
            using NewLoopDialog dialog = new();
            ComboBox choices = Find<ComboBox>(dialog, "_bars");
            Assert.Equal(3, choices.Items.Count);
            Assert.Equal(2, dialog.BarCount);
            choices.SelectedIndex = 0;
            Assert.Equal(1, dialog.BarCount);
            choices.SelectedIndex = 2;
            Assert.Equal(4, dialog.BarCount);
        });

    [Fact]
    public void Options_CancelDoesNotApplyEdits()
        => OnStaThread(() =>
        {
            AppSettings original = new() { FontSize = AppFontSize.Normal };
            using OptionsDialog dialog = new(original);
            Find<ComboBox>(dialog, "_theme").SelectedIndex = 1;
            Find<ComboBox>(dialog, "_icons").SelectedIndex = 2;
            Find<ComboBox>(dialog, "_fontSize").SelectedIndex = 3;
            Find<TextBox>(dialog, "_folder").Text = "not a full folder path";
            dialog.DialogResult = DialogResult.Cancel;

            Assert.Equal(original.Theme, dialog.Result.Theme);
            Assert.Equal(original.IconSize, dialog.Result.IconSize);
            Assert.Equal(original.FontSize, dialog.Result.FontSize);
            Assert.Equal(original.DefaultFolder, dialog.Result.DefaultFolder);
        });

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void Options_MapsAllRelativeFontSizesWithoutOpeningAudio(int selectedIndex, int expected)
        => OnStaThread(() =>
        {
            using OptionsDialog dialog = new(new AppSettings { DefaultFolder = Path.GetTempPath() });
            ComboBox choices = Find<ComboBox>(dialog, "_fontSize");
            Assert.Equal(4, choices.Items.Count);
            choices.SelectedIndex = selectedIndex;
            MethodInfo? confirm = typeof(OptionsDialog).GetMethod(
                "Ok_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(confirm);
            confirm.Invoke(dialog, [null, EventArgs.Empty]);

            Assert.Equal(DialogResult.OK, dialog.DialogResult);
            Assert.Equal((AppFontSize)expected, dialog.Result.FontSize);
        });

    [Fact]
    public void TwoBarView_MapsEditsToTheSecondMusicalBar()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            MenuStrip menu = Find<MenuStrip>(form, "_menuStrip");
            ToolStripMenuItem view = Assert.IsType<ToolStripMenuItem>(menu.Items[2]);
            Assert.IsType<ToolStripMenuItem>(view.DropDownItems[1]).PerformClick();
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            DataGridViewRow kick = grid.Rows.Cast<DataGridViewRow>().Single(row => row.Tag is Cr78Instrument.BassDrum);
            kick.Cells["_step18"].Value = true;

            Assert.IsType<ToolStripMenuItem>(view.DropDownItems[0]).PerformClick();
            FindItem<ToolStripComboBox>(form, "_barSelector").SelectedIndex = 1;
            kick = grid.Rows.Cast<DataGridViewRow>().Single(row => row.Tag is Cr78Instrument.BassDrum);
            Assert.Equal(true, kick.Cells["_step02"].Value);
            FindItem<ToolStripComboBox>(form, "_barSelector").SelectedIndex = 0;
            kick = grid.Rows.Cast<DataGridViewRow>().Single(row => row.Tag is Cr78Instrument.BassDrum);
            Assert.Equal(false, kick.Cells["_step02"].Value);
        });

    [Theory]
    [InlineData(32)]
    [InlineData(48)]
    [InlineData(64)]
    public void Toolbar_UsesSelectedSymbolSizeAndWrapsWithoutOpeningAudio(int size)
        => OnStaThread(() =>
        {
            using MainForm form = new(StartupOptions.Interactive, new AppSettings { IconSize = (ToolbarIconSize)size });
            MethodInfo? initialize = typeof(MainForm).GetMethod("InitializeIcons", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(initialize);
            initialize.Invoke(form, null);
            ToolStrip strip = Find<ToolStrip>(form, "_toolStrip");
            ToolStripButton play = FindItem<ToolStripButton>(form, "_playButton");
            Assert.NotNull(play.Image);
            int pixels = (size * form.DeviceDpi + 48) / 96;
            Assert.Equal(new System.Drawing.Size(pixels, pixels), play.Image.Size);
            Assert.Equal(ToolStripItemImageScaling.None, play.ImageScaling);
            Assert.Equal(ToolStripLayoutStyle.Flow, strip.LayoutStyle);

            form.ClientSize = new System.Drawing.Size(800, 620);
            form.PerformLayout();
            strip.PerformLayout();
            Assert.True(strip.Height >= play.Height);
            Assert.Null(Find<AudioSpectrumControl>(form, "_spectrumControl").Source);
            Assert.DoesNotContain("*", form.Text);
        });

    private static T Find<T>(Control root, string name) where T : Control
        => Assert.IsType<T>(Assert.Single(root.Controls.Find(name, true)));

    private static T FindItem<T>(Control root, string name) where T : ToolStripItem
        => Assert.IsType<T>(Find<ToolStrip>(root, "_toolStrip").Items[name]);

    private static void OnStaThread(Action action)
    {
        ExceptionDispatchInfo? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The UI layout operation did not finish.");
        failure?.Throw();
    }
}
