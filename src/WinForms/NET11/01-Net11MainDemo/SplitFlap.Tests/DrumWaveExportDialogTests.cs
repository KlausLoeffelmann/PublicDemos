using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using DrumMachine.Demo;
using DrumMachine.Demo.Documents;
using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Exercises export choices, limits, command availability and modal layout without an audio device.
/// </summary>
[Collection("Rhythm UI")]
public sealed class DrumWaveExportDialogTests
{
    /// <summary>
    ///  Default, current-bar and repeated-loop choices expose the requested immutable render range.
    /// </summary>
    [Fact]
    public void ScopeChoices_MapToTheCorrectRangeAndRepetitionCount()
        => OnStaThread(() =>
        {
            using ExportWaveDialog dialog = new(currentBar: 2, barCount: 4, tempoBpm: 120);
            NumericUpDown repetitions = Find<NumericUpDown>(dialog, "_repetitions");
            Assert.Equal(WaveExportScope.FullLoop, dialog.Scope);
            Assert.Equal(0, dialog.FirstBar);
            Assert.Null(dialog.BarCount);
            Assert.Equal(1, dialog.Repetitions);
            Assert.Equal(8m, dialog.MusicalSeconds);
            Assert.False(dialog.IncludeTail);
            Assert.False(repetitions.Enabled);
            Assert.Equal(1, repetitions.Minimum);
            Assert.Equal(PercussionWaveExporter.MaximumRepetitions, repetitions.Maximum);

            Find<RadioButton>(dialog, "_repeated").Checked = true;
            repetitions.Value = 3;
            Assert.True(repetitions.Enabled);
            Assert.Equal(WaveExportScope.RepeatedLoop, dialog.Scope);
            Assert.Equal(3, dialog.Repetitions);
            Assert.Equal(24m, dialog.MusicalSeconds);

            Find<RadioButton>(dialog, "_currentBar").Checked = true;
            Assert.Equal(2, dialog.FirstBar);
            Assert.Equal(1, dialog.BarCount);
            Assert.Equal(1, dialog.Repetitions);
            Assert.Equal(2m, dialog.MusicalSeconds);
            Assert.False(repetitions.Enabled);
            Find<CheckBox>(dialog, "_includeTail").Checked = true;
            Assert.True(dialog.IncludeTail);
            Assert.Equal(2m, dialog.MusicalSeconds);
        });

    /// <summary>
    ///  One hour is accepted, but a longer musical sequence cannot open the export save workflow.
    /// </summary>
    [Fact]
    public void DurationLimit_DisablesExportAndRecoversWhenScopeChanges()
        => OnStaThread(() =>
        {
            using ExportWaveDialog dialog = new(0, 2, 120);
            Find<RadioButton>(dialog, "_repeated").Checked = true;
            NumericUpDown repetitions = Find<NumericUpDown>(dialog, "_repetitions");
            Button export = Find<Button>(dialog, "_export");
            repetitions.Value = 900;
            Assert.Equal(3_600m, dialog.MusicalSeconds);
            Assert.False(dialog.ExceedsMusicalLimit);
            Assert.True(export.Enabled);
            repetitions.Value = 901;
            Assert.True(dialog.ExceedsMusicalLimit);
            Assert.False(export.Enabled);
            Assert.Contains("over 1 hour", Find<Label>(dialog, "_duration").Text);
            Find<RadioButton>(dialog, "_currentBar").Checked = true;
            Assert.True(export.Enabled);
            Assert.False(dialog.ExceedsMusicalLimit);
            Assert.Equal(2m, dialog.MusicalSeconds);
        });

    /// <summary>
    ///  Export works without a running audio endpoint but respects the existing busy command guard.
    /// </summary>
    [Fact]
    public void FileCommand_IsAvailableWithoutAudioAndDisabledWhileBusy()
        => OnStaThread(() =>
        {
            using MainForm form = new();
            MenuStrip menu = Find<MenuStrip>(form, "_menuStrip");
            ToolStripMenuItem file = Assert.IsType<ToolStripMenuItem>(menu.Items[0]);
            ToolStripMenuItem export = Assert.IsType<ToolStripMenuItem>(file.DropDownItems["_exportWavMenuItem"]);
            Assert.True(export.Enabled);
            FieldInfo busy = typeof(MainForm).GetField("_busy", BindingFlags.Instance | BindingFlags.NonPublic)!;
            MethodInfo update = typeof(MainForm).GetMethod("UpdateCommandState", BindingFlags.Instance | BindingFlags.NonPublic)!;
            busy.SetValue(form, true);
            update.Invoke(form, null);
            Assert.False(export.Enabled);
            busy.SetValue(form, false);
            update.Invoke(form, null);
            Assert.True(export.Enabled);
            Assert.DoesNotContain("*", form.Text);
        });

    /// <summary>
    ///  A loaded recording cannot accidentally become the destination for its own rendered output.
    /// </summary>
    [Fact]
    public void ExportDestination_ProtectsImportedWavSources()
        => OnStaThread(() =>
        {
            using LoopTestFiles files = new();
            using MainForm form = new();
            string source = files.File("source.wav");
            PercussionSoundPreset preset = PercussionSoundPreset.Stock.WithWaveFile(Cr78Instrument.BassDrum, source);
            typeof(MainForm).GetField("_soundPreset", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(form, preset);
            MethodInfo protect = typeof(MainForm).GetMethod(
                "IsProtectedExportDestination", BindingFlags.Instance | BindingFlags.NonPublic)!;
            Assert.Equal(true, protect.Invoke(form, [source]));
            Assert.Equal(true, protect.Invoke(form, [source.ToUpperInvariant()]));
            Assert.Equal(false, protect.Invoke(form, [files.File("export.wav")]));
        });

    /// <summary>
    ///  The actual modal dialog loads, exposes keyboard buttons and keeps its controls within their containers.
    /// </summary>
    [Theory]
    [InlineData(1f)]
    [InlineData(1.5f)]
    [InlineData(2f)]
    public void ModalDialog_ExposesCompleteChoicesAtScaledSizes(float scale)
        => OnStaThread(() =>
        {
            using ExportWaveDialog dialog = new(1, 4, 120);
            if (scale != 1f)
            {
                dialog.Scale(new SizeF(scale, scale));
            }

            ExceptionDispatchInfo? failure = null;
            dialog.Shown += (_, _) =>
            {
                try
                {
                    Assert.Equal(AutoScaleMode.Dpi, dialog.AutoScaleMode);
                    Assert.Equal(FormStartPosition.CenterParent, dialog.StartPosition);
                    Assert.Same(Find<Button>(dialog, "_export"), dialog.AcceptButton);
                    Assert.Same(Find<Button>(dialog, "_cancel"), dialog.CancelButton);
                    foreach (Control control in Descendants(dialog).Where(control => control.Visible))
                    {
                        Assert.True(control.Width > 0 && control.Height > 0, control.Name);
                        Assert.NotNull(control.Parent);
                        Assert.True(control.Right <= control.Parent.ClientSize.Width + 1, control.Name);
                        Assert.True(control.Bottom <= control.Parent.ClientSize.Height + 1, control.Name);
                    }

                    Find<RadioButton>(dialog, "_currentBar").Checked = true;
                    Find<Button>(dialog, "_export").PerformClick();
                }
                catch (Exception ex)
                {
                    failure = ExceptionDispatchInfo.Capture(ex);
                    dialog.DialogResult = DialogResult.Cancel;
                }
            };
            DialogResult result = dialog.ShowDialog();
            failure?.Throw();
            Assert.Equal(DialogResult.OK, result);
            Assert.Equal(1, dialog.FirstBar);
            Assert.Equal(1, dialog.BarCount);
        });

    private static IEnumerable<Control> Descendants(Control root)
    {
        foreach (Control control in root.Controls)
        {
            yield return control;
            foreach (Control child in Descendants(control))
            {
                yield return child;
            }
        }
    }

    private static T Find<T>(Control root, string name) where T : Control
        => Assert.IsAssignableFrom<T>(Assert.Single(root.Controls.Find(name, true)));

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
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "The export dialog operation did not finish.");
        failure?.Throw();
    }
}
