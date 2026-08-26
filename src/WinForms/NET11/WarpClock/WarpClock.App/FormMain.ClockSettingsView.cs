using System.ComponentModel;

using WarpClock.Abstractions;
using WarpClock.Engine;

namespace WarpClock.App;

public partial class FormMain
{
    private sealed class ClockSettingsView(FormMain owner, WarpClockControl clock)
    {
        [Category("Animation")]
        [DisplayName("Second hand")]
        public ClockHandMotion SecondMotion
        {
            get => clock.SecondMotion;
            set => clock.SecondMotion = value;
        }

        [Category("Animation")]
        [DisplayName("Minute hand")]
        public ClockHandMotion MinuteMotion
        {
            get => clock.MinuteMotion;
            set => clock.MinuteMotion = value;
        }

        [Category("Animation")]
        [DisplayName("Hour hand")]
        public ClockHandMotion HourMotion
        {
            get => clock.HourMotion;
            set => clock.HourMotion = value;
        }

        [Category("Hand targeting")]
        [DisplayName("Second hand")]
        [Description("ThemeDefault uses the active theme's choice; otherwise forces the selected targeting mode.")]
        public ClockHandTargetMode SecondTargetMode
        {
            get => clock.SecondTargetMode;
            set => clock.SecondTargetMode = value;
        }

        [Category("Hand targeting")]
        [DisplayName("Minute hand")]
        [Description("ThemeDefault uses the active theme's choice; otherwise forces the selected targeting mode.")]
        public ClockHandTargetMode MinuteTargetMode
        {
            get => clock.MinuteTargetMode;
            set => clock.MinuteTargetMode = value;
        }

        [Category("Hand targeting")]
        [DisplayName("Hour hand")]
        [Description("ThemeDefault uses the active theme's choice; otherwise forces the selected targeting mode.")]
        public ClockHandTargetMode HourTargetMode
        {
            get => clock.HourTargetMode;
            set => clock.HourTargetMode = value;
        }

        [Category("Animation")]
        [DisplayName("Grace period")]
        [Description("Hand-to-target catch-up window, in seconds.")]
        public int GraceSeconds
        {
            get => clock.GraceSeconds;
            set => clock.GraceSeconds = value;
        }

        [Category("Animation")]
        [DisplayName("Crawl duration")]
        [Description("Ease-in-out crawl duration before the hand rests on its next step, in seconds.")]
        public float GlideDurationSeconds
        {
            get => clock.GlideDurationSeconds;
            set => clock.GlideDurationSeconds = value;
        }

        [Category("Animation")]
        public bool MagneticNumerals
        {
            get => clock.MagneticNumerals;
            set => clock.MagneticNumerals = value;
        }

        [Category("Demo time")]
        public TimeSpan TimeOffset
        {
            get => clock.TimeOffset;
            set => clock.TimeOffset = value;
        }

        [Category("Demo time")]
        public double SpeedMultiplier
        {
            get => clock.SpeedMultiplier;
            set
            {
                if (value == 1d)
                {
                    clock.ResetTimeAcceleration();
                }

                clock.SpeedMultiplier = value;
            }
        }

        [Category("Theme information")]
        public RenderThemeInfo RenderThemeInfo
        {
            get => owner._preferredThemeInfoMode;
            set
            {
                owner._preferredThemeInfoMode = value;
                owner.ApplyEffectiveThemeInfoMode();
            }
        }

        [Category("Theme information")]
        public ThemeInfoPlacement ThemeInfoPlacement
        {
            get => clock.ThemeInfoPlacement;
            set => clock.ThemeInfoPlacement = value;
        }

        [Category("Theme information")]
        [DisplayName("OLED view")]
        public bool OledView
        {
            get => owner.GetOledViewEnabled();
            set
            {
                owner.SetOledViewEnabled(value);
                owner.ApplyEffectiveThemeInfoMode();
            }
        }

        [Category("Rendering")]
        public bool VSyncEnabled
        {
            get => clock.VSyncEnabled;
            set => clock.VSyncEnabled = value;
        }

        [Category("Rendering")]
        [Description("Frame rate used when VSync is disabled.")]
        public double TargetFrameRate
        {
            get => clock.TargetFrameRate;
            set => clock.TargetFrameRate = value;
        }
    }
}
