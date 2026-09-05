using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using DrumMachine.Demo;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.WinForms;

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
            Assert.Equal(18, grid.Columns.Count);
            Assert.Equal(13, grid.Rows.Count);
            Assert.Null(spectrum.Source);
            Assert.False(grid.Enabled);
            Assert.Equal(2, Find<ComboBox>(form, "_barSelector").Items.Count);
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

            Find<ComboBox>(form, "_barSelector").SelectedIndex = 1;
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

    private static T Find<T>(Control root, string name) where T : Control
        => Assert.IsType<T>(Assert.Single(root.Controls.Find(name, true)));

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
