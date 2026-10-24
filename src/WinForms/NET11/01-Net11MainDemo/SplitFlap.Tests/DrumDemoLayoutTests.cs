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
            Assert.Equal(35, grid.Columns.Count);
            Assert.Equal(19, grid.Columns.Cast<DataGridViewColumn>().Count(column => column.Visible));
            Assert.Equal(Cr78Kit.Instruments.Count, grid.Rows.Count);
            Assert.Equal(17, grid.Rows.Count);
            Assert.IsType<DrumMachine.Demo.Controls.BeatGridView>(grid);
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
            Assert.Equal(35, grid.Columns.Cast<DataGridViewColumn>().Count(column => column.Visible));
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
            Assert.Equal(original.RecordingsFolder, dialog.Result.RecordingsFolder);
            Assert.Equal(original.SoundPresetFolder, dialog.Result.SoundPresetFolder);
            Assert.Equal(original.LastWavImportFolder, dialog.Result.LastWavImportFolder);
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

    [Fact]
    public void BarEditor_ScopesSelectedTracksWithoutAudioOrUserSettings()
        => OnStaThread(() =>
        {
            Assert.True(StartupOptions.TryParse(["--no-settings"], out StartupOptions options, out string? error), error);
            using MainForm form = new(options);
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            ToolStrip editor = Find<ToolStrip>(form, "_editStrip");
            Assert.All(grid.Rows.Cast<DataGridViewRow>(), row => Assert.Equal(true, row.Cells["_trackColumn"].Value));
            Assert.Equal(8, EditItem<ToolStripComboBox>(editor, "_presetSelector").Items.Count);

            Row(grid, Cr78Instrument.SnareDrum).Cells["_trackColumn"].Value = false;
            EditItem<ToolStripButton>(editor, "_fillButton").PerformClick();
            Assert.Equal(true, Row(grid, Cr78Instrument.BassDrum).Cells["_step02"].Value);
            Assert.Equal(true, Row(grid, Cr78Instrument.SnareDrum).Cells["_step05"].Value);
            Assert.Equal(false, Row(grid, Cr78Instrument.SnareDrum).Cells["_step01"].Value);
            Assert.Contains("*", form.Text);

            FindItem<ToolStripComboBox>(form, "_barSelector").SelectedIndex = 1;
            Assert.Equal(false, Row(grid, Cr78Instrument.BassDrum).Cells["_step02"].Value);
            FindItem<ToolStripComboBox>(form, "_barSelector").SelectedIndex = 0;

            EditItem<ToolStripButton>(editor, "_selectNoneButton").PerformClick();
            Assert.False(EditItem<ToolStripButton>(editor, "_fillButton").Enabled);
            Row(grid, Cr78Instrument.Cowbell).Cells["_trackColumn"].Value = true;
            EditItem<ToolStripComboBox>(editor, "_scopeSelector").SelectedIndex = 1;
            EditItem<ToolStripButton>(editor, "_clearButton").PerformClick();
            Assert.Equal(false, Row(grid, Cr78Instrument.Cowbell).Cells["_step10"].Value);
            Assert.Equal(true, Row(grid, Cr78Instrument.HiHat).Cells["_step01"].Value);

            EditItem<ToolStripButton>(editor, "_selectNoneButton").PerformClick();
            Row(grid, Cr78Instrument.BassDrum).Cells["_trackColumn"].Value = true;
            EditItem<ToolStripButton>(editor, "_copyBarButton").PerformClick();
            Assert.Equal(3, FindItem<ToolStripComboBox>(form, "_barSelector").Items.Count);
            Assert.Equal(true, Row(grid, Cr78Instrument.BassDrum).Cells["_step02"].Value);
            Assert.Equal(false, Row(grid, Cr78Instrument.SnareDrum).Cells["_step05"].Value);

            EditItem<ToolStripButton>(editor, "_deleteBarButton").PerformClick();
            Assert.Equal(2, FindItem<ToolStripComboBox>(form, "_barSelector").Items.Count);
            EditItem<ToolStripButton>(editor, "_addBarButton").PerformClick();
            Assert.Equal(3, FindItem<ToolStripComboBox>(form, "_barSelector").Items.Count);
            Assert.Equal(false, Row(grid, Cr78Instrument.BassDrum).Cells["_step01"].Value);

            MenuStrip menu = Find<MenuStrip>(form, "_menuStrip");
            ToolStripMenuItem edit = Assert.IsType<ToolStripMenuItem>(menu.Items[1]);
            Assert.IsType<ToolStripMenuItem>(edit.DropDownItems[2]).PerformClick();
            Assert.Equal(2, FindItem<ToolStripComboBox>(form, "_barSelector").Items.Count);

            EditItem<ToolStripButton>(editor, "_selectNoneButton").PerformClick();
            Row(grid, Cr78Instrument.HiHat).Cells["_trackColumn"].Value = true;
            Row(grid, Cr78Instrument.Cymbal).Cells["_trackColumn"].Value = true;
            EditItem<ToolStripTextBox>(editor, "_presetName").Text = "Hats";
            EditItem<ToolStripButton>(editor, "_savePresetButton").PerformClick();
            Assert.Contains("Hats", EditItem<ToolStripComboBox>(editor, "_presetSelector").Text, StringComparison.Ordinal);
            EditItem<ToolStripButton>(editor, "_selectAllButton").PerformClick();
            EditItem<ToolStripButton>(editor, "_recallPresetButton").PerformClick();
            Assert.Equal(true, Row(grid, Cr78Instrument.HiHat).Cells["_trackColumn"].Value);
            Assert.Equal(true, Row(grid, Cr78Instrument.Cymbal).Cells["_trackColumn"].Value);
            Assert.Equal(false, Row(grid, Cr78Instrument.BassDrum).Cells["_trackColumn"].Value);
            Assert.Null(Find<AudioSpectrumControl>(form, "_spectrumControl").Source);

            static DataGridViewRow Row(DataGridView grid, Cr78Instrument instrument)
                => grid.Rows.Cast<DataGridViewRow>().Single(row => row.Tag is Cr78Instrument value && value == instrument);

            static T EditItem<T>(ToolStrip strip, string name) where T : ToolStripItem
                => Assert.IsType<T>(strip.Items[name]);
        });

    [Fact]
    public void Options_OkKeepsTrackSelectionPresets()
        => OnStaThread(() =>
        {
            AppSettings original = new AppSettings { DefaultFolder = Path.GetTempPath() }
                .WithTrackPreset(3, "Kicks", [Cr78Instrument.BassDrum]);
            using OptionsDialog dialog = new(original);
            MethodInfo? confirm = typeof(OptionsDialog).GetMethod(
                "Ok_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(confirm);
            confirm.Invoke(dialog, [null, EventArgs.Empty]);

            Assert.Equal(DialogResult.OK, dialog.DialogResult);
            Assert.Equal("Kicks", dialog.Result.TrackPresets[3].Name);
            Assert.Equal([Cr78Instrument.BassDrum], dialog.Result.TrackPresets[3].Instruments);
            Assert.Equal(8, dialog.Result.TrackPresets.Count);
            Assert.Equal(Path.GetFullPath(Path.GetTempPath()), dialog.Result.DefaultFolder);
        });

    [Fact]
    public void SoundCommands_TargetTheFixedKitAndDoNotAddTracks()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            Assert.Equal(
                Cr78Kit.Instruments.ToArray(),
                grid.Rows.Cast<DataGridViewRow>().Select(row => Assert.IsType<Cr78Instrument>(row.Tag)));
            MenuStrip menu = Find<MenuStrip>(form, "_menuStrip");
            ToolStripMenuItem tools = Assert.IsType<ToolStripMenuItem>(menu.Items[3]);
            Assert.Contains(tools.DropDownItems.Cast<ToolStripItem>(), item => item.Name == "_importWavMenuItem");
            Assert.Contains(tools.DropDownItems.Cast<ToolStripItem>(), item => item.Name == "_saveSoundPresetMenuItem");
            Assert.DoesNotContain(
                tools.DropDownItems.Cast<ToolStripItem>(),
                item => item.Text?.Contains("Add track", StringComparison.OrdinalIgnoreCase) == true);
            ToolStripItem soundItem = Find<StatusStrip>(form, "_statusStrip").Items["_soundStatusLabel"]!;
            Assert.Equal("Sounds: Stock percussion", soundItem.Text);
        });

    [Fact]
    public void SplitterConstraints_ApplyOnlyAfterTheContentHasRoom()
        => OnStaThread(() =>
        {
            Assert.True(StartupOptions.TryParse(["--no-settings"], out StartupOptions options, out string? error), error);
            using MainForm form = new(options);
            form.ClientSize = new System.Drawing.Size(1280, 900);
            form.CreateControl();
            SplitContainer split = Find<SplitContainer>(form, "_contentSplit");
            Assert.Equal(100, split.Panel1MinSize);
            Assert.Equal(140, split.Panel2MinSize);
            Assert.InRange(split.SplitterDistance, split.Panel1MinSize, split.Height - split.Panel2MinSize - split.SplitterWidth);
            Assert.True(split.Panel1.Controls[0].Width > 0);
            Assert.True(split.Panel2.Controls[0].Height > 0);
        });

    [Theory]
    [InlineData(1f)]
    [InlineData(1.5f)]
    [InlineData(2f)]
    public void SoundControls_FitAndSeventeenRowsStayReachableAt96_144_192Scale(float scale)
        => OnStaThread(() =>
        {
            Assert.True(StartupOptions.TryParse(["--no-settings"], out StartupOptions options, out string? error), error);
            using MainForm form = new(options);
            form.ClientSize = new System.Drawing.Size((int)(800 * scale), (int)(700 * scale));
            if (scale != 1f)
            {
                form.Scale(new System.Drawing.SizeF(scale, scale));
            }

            form.CreateControl();
            AssertSoundCommandsFit(form);
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            Assert.Equal(17, grid.Rows.Count);
            Assert.Equal(Cr78Kit.Instruments.ToArray(), grid.Rows.Cast<DataGridViewRow>().Select(row => Assert.IsType<Cr78Instrument>(row.Tag)));
            Assert.All(grid.Rows.Cast<DataGridViewRow>(), row => Assert.True(row.Visible));
            int contentHeight = grid.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + grid.ColumnHeadersHeight;
            Assert.True(contentHeight > grid.ColumnHeadersHeight);
            if (grid.ClientSize.Height < contentHeight)
            {
                Assert.True(grid.ScrollBars is ScrollBars.Vertical or ScrollBars.Both);
            }
            SplitContainer split = Find<SplitContainer>(form, "_contentSplit");
            Assert.InRange(split.SplitterDistance, split.Panel1MinSize, split.Height - split.Panel2MinSize - split.SplitterWidth);
            Assert.True(split.Height > split.SplitterWidth);
        });

    [Fact]
    public void LayoutRestore_AppliesColumnsSplitterAndRejectsOffscreenWindows()
        => OnStaThread(() =>
        {
            Rectangle area = Screen.PrimaryScreen!.WorkingArea;
            AppSettings settings = new()
            {
                Theme = AppTheme.Classic,
                Window = new WindowPlacement
                {
                    X = area.Left + 24,
                    Y = area.Top + 24,
                    Width = Math.Min(980, Math.Max(800, area.Width - 48)),
                    Height = Math.Min(720, Math.Max(620, area.Height - 48)),
                    SavedDpi = 96
                },
                SpectrumSplitterDistanceDip = 180,
                StepGridColumns = ColumnPreferences(instrumentDip: 200, stepDip: 48, swapFirstSteps: true)
            };
            using MainForm form = new(StartupOptions.Interactive, settings);
            form.ClientSize = new System.Drawing.Size(1280, 900);
            form.CreateControl();
            InvokeOnLoad(form);

            Rectangle expected = settings.Window.ScaleToDpi(form.DeviceDpi);
            if (WindowPlacement.IsUsableOnScreen(expected))
            {
                Assert.Equal(expected, form.Bounds);
                Assert.NotEqual(FormWindowState.Maximized, form.WindowState);
            }

            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            int instrumentWidth = Math.Max(160, WindowPlacement.ScaleFromDip(200, form.DeviceDpi));
            int stepWidth = Math.Max(32, WindowPlacement.ScaleFromDip(48, form.DeviceDpi));
            Assert.Equal(instrumentWidth, grid.Columns["_instrumentColumn"]!.Width);
            Assert.Equal(stepWidth, grid.Columns["_step01"]!.Width);
            Assert.Equal(grid.Columns["_step02"]!.DisplayIndex + 1, grid.Columns["_step01"]!.DisplayIndex);
            SplitContainer split = Find<SplitContainer>(form, "_contentSplit");
            int desired = WindowPlacement.ScaleFromDip(180, form.DeviceDpi);
            int maximum = split.Height - split.SplitterWidth - split.Panel2MinSize;
            Assert.Equal(Math.Clamp(desired, split.Panel1MinSize, maximum), split.SplitterDistance);

            AppSettings offscreen = settings with
            {
                Window = new WindowPlacement { X = -20_000, Y = -20_000, Width = 900, Height = 700, SavedDpi = 96 }
            };
            using MainForm rejected = new(StartupOptions.Interactive, offscreen);
            rejected.CreateControl();
            InvokeOnLoad(rejected);
            Assert.NotEqual(-20_000, rejected.Location.X);
            Assert.Equal(FormStartPosition.CenterScreen, rejected.StartPosition);
        });

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void OptionsDialog_KeepsThemeChoiceAndCenteredBoundsOnScreen(int index)
        => OnStaThread(() =>
        {
            AppTheme theme = index == 0 ? AppTheme.Classic : AppTheme.Dark;
            AppSettings settings = new() { Theme = theme };
            using MainForm owner = new(StartupOptions.Interactive, settings);
            owner.ClientSize = new System.Drawing.Size(1100, 760);
            owner.CreateControl();
            using OptionsDialog dialog = new(settings);
            using NewLoopDialog loop = new();
            dialog.CreateControl();
            loop.CreateControl();
            ComboBox themeList = Find<ComboBox>(dialog, "_theme");
            Assert.Equal(index, themeList.SelectedIndex);
            Assert.Equal(theme == AppTheme.Classic ? "Classic" : "Dark mode", themeList.Items[index]?.ToString());
            AssertCenteredOnScreen(dialog);
            AssertCenteredOnScreen(loop);
            Assert.Equal(SystemColors.Window, Find<DataGridView>(owner, "_stepGrid").BackgroundColor);
        });

    [Theory]
    [InlineData(96)]
    [InlineData(144)]
    [InlineData(192)]
    public void ColumnPreferences_ScaleWidthsWithoutAMonitorAtThatDpi(int dpi)
        => OnStaThread(() =>
        {
            using MainForm form = new();
            DataGridView grid = Find<DataGridView>(form, "_stepGrid");
            StepGridColumnPreference[] columns = ColumnPreferences(180, 36, swapFirstSteps: false);
            Assert.True(StepGridColumnPreference.TryApply(grid, columns, dpi));
            Assert.Equal(Math.Max(160, WindowPlacement.ScaleFromDip(180, dpi)), grid.Columns["_instrumentColumn"]!.Width);
            Assert.Equal(Math.Max(32, WindowPlacement.ScaleFromDip(36, dpi)), grid.Columns["_step16"]!.Width);
            Assert.Equal(36, WindowPlacement.ScaleToDip(WindowPlacement.ScaleFromDip(36, dpi), dpi));
        });

    private static void AssertSoundCommandsFit(MainForm form)
    {
        ToolStrip edit = Find<ToolStrip>(form, "_editStrip");
        edit.PerformLayout();
        string[] names = ["_importWavButton", "_resetSoundButton", "_loadSoundPresetButton", "_saveSoundPresetButton"];
        foreach (string name in names)
        {
            ToolStripItem item = edit.Items[name]!;
            Assert.True(item.Available);
            Assert.InRange(item.Bounds.Right, 1, edit.ClientSize.Width + 2);
            Assert.InRange(item.Bounds.Bottom, 1, edit.ClientSize.Height + 2);
        }

        Assert.Equal("Import WAV", edit.Items["_importWavButton"]!.Text);
        Assert.Equal("Synthesis", edit.Items["_resetSoundButton"]!.Text);
        Assert.Equal("Load sounds", edit.Items["_loadSoundPresetButton"]!.Text);
        Assert.Equal("Save sounds", edit.Items["_saveSoundPresetButton"]!.Text);
    }

    private static void AssertCenteredOnScreen(Form dialog)
    {
        Assert.Equal(FormStartPosition.CenterParent, dialog.StartPosition);
        Rectangle area = Screen.FromControl(dialog).WorkingArea;
        Assert.True(dialog.MinimumSize.Width <= area.Width);
        Assert.True(dialog.MinimumSize.Height <= area.Height);
        Rectangle centered = new(
            area.Left + Math.Max(0, (area.Width - dialog.Width) / 2),
            area.Top + Math.Max(0, (area.Height - dialog.Height) / 2),
            dialog.Width,
            dialog.Height);
        Assert.True(WindowPlacement.IsUsableOnScreen(centered));
    }

    private static StepGridColumnPreference[] ColumnPreferences(int instrumentDip, int stepDip, bool swapFirstSteps)
    {
        return StepGridColumnPreference.Names.Select((name, index) => new StepGridColumnPreference
        {
            Name = name,
            DisplayIndex = swapFirstSteps && name == "_step01" ? index + 1
                : swapFirstSteps && name == "_step02" ? index - 1
                : index,
            WidthDip = name == "_instrumentColumn" ? instrumentDip : name.StartsWith("_step", StringComparison.Ordinal) ? stepDip : 48
        }).ToArray();
    }

    private static void InvokeOnLoad(MainForm form)
    {
        MethodInfo onLoad = typeof(MainForm).GetMethod("OnLoad", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMethodException(nameof(MainForm), "OnLoad");
        onLoad.Invoke(form, [EventArgs.Empty]);
    }

    private static T Find<T>(Control root, string name) where T : Control
        => Assert.IsAssignableFrom<T>(Assert.Single(root.Controls.Find(name, true)));

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
