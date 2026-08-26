using WarpClock.Abstractions;
using WarpClock.Engine;

namespace WarpClock.App;

public partial class HandsOptionsView : UserControl
{
    public HandsOptionsView()
    {
        InitializeComponent();

        AutoScroll = true;
        AutoScrollMinSize = Size;
        _graceNumericUpDown.AccessibleName = "Hand grace seconds";
        _graceNumericUpDown.AccessibleDescription = "Shared hand catch-up window in seconds.";
    }

    public void LoadFrom(HandOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        SetCheckedMotion(options.HourMotion, _hourCrawlingRadioButton, _hourSweepRadioButton, _hourFastTickRadioButton, _hourTickRadioButton);
        SetCheckedMotion(options.MinuteMotion, _minuteCrawlingRadioButton, _minuteSweepRadioButton, _minuteFastTickRadioButton, _minuteTickRadioButton);
        SetCheckedMotion(options.SecondMotion, _secondCrawlingRadioButton, _secondSweepRadioButton, _secondFastTickRadioButton, _secondTickRadioButton);
        _graceNumericUpDown.Value = Math.Clamp(options.GraceSeconds, (int)_graceNumericUpDown.Minimum, (int)_graceNumericUpDown.Maximum);
    }

    public HandOptions CreateOptions()
    {
        var options = new HandOptions
        {
            HourMotion = GetCheckedMotion(_hourCrawlingRadioButton, _hourSweepRadioButton, _hourFastTickRadioButton, _hourTickRadioButton),
            MinuteMotion = GetCheckedMotion(_minuteCrawlingRadioButton, _minuteSweepRadioButton, _minuteFastTickRadioButton, _minuteTickRadioButton),
            SecondMotion = GetCheckedMotion(_secondCrawlingRadioButton, _secondSweepRadioButton, _secondFastTickRadioButton, _secondTickRadioButton),
            GraceSeconds = (int)_graceNumericUpDown.Value,
        };

        options.Normalize();
        return options;
    }

    private static ClockHandMotion GetCheckedMotion(
        RadioButton crawlingRadioButton,
        RadioButton sweepRadioButton,
        RadioButton fastTickRadioButton,
        RadioButton tickRadioButton)
    {
        if (sweepRadioButton.Checked)
        {
            return ClockHandMotion.Sweep;
        }

        if (fastTickRadioButton.Checked)
        {
            return ClockHandMotion.FastTick;
        }

        if (tickRadioButton.Checked)
        {
            return ClockHandMotion.Tick;
        }

        return ClockHandMotion.Crawling;
    }

    private static void SetCheckedMotion(
        ClockHandMotion motion,
        RadioButton crawlingRadioButton,
        RadioButton sweepRadioButton,
        RadioButton fastTickRadioButton,
        RadioButton tickRadioButton)
    {
        crawlingRadioButton.Checked = motion == ClockHandMotion.Crawling;
        sweepRadioButton.Checked = motion == ClockHandMotion.Sweep;
        fastTickRadioButton.Checked = motion == ClockHandMotion.FastTick;
        tickRadioButton.Checked = motion == ClockHandMotion.Tick;
    }
}
