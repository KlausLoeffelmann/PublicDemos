using System.ComponentModel;
using WarpClock.Engine;

namespace WarpClock.App;

partial class FormMain
{
    private IContainer components;

    /// <summary>Clean up any resources being used.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        _splitContainer = new SplitContainer();
        _clock = new WarpClockControl();
        _propertyGrid = new PropertyGrid();
        _menuStrip = new MenuStrip();
        _themeMenu = new ToolStripMenuItem();
        _motionMenu = new ToolStripMenuItem();
        _secondMotionMenu = new ToolStripMenuItem();
        _miSecondCrawling = new ToolStripMenuItem();
        _miSecondSweep = new ToolStripMenuItem();
        _miSecondFastTick = new ToolStripMenuItem();
        _miSecondTick = new ToolStripMenuItem();
        _minuteMotionMenu = new ToolStripMenuItem();
        _miMinuteCrawling = new ToolStripMenuItem();
        _miMinuteSweep = new ToolStripMenuItem();
        _miMinuteFastTick = new ToolStripMenuItem();
        _miMinuteTick = new ToolStripMenuItem();
        _hourMotionMenu = new ToolStripMenuItem();
        _miHourCrawling = new ToolStripMenuItem();
        _miHourSweep = new ToolStripMenuItem();
        _miHourFastTick = new ToolStripMenuItem();
        _miHourTick = new ToolStripMenuItem();
        _graceMenu = new ToolStripMenuItem();
        _miGrace1 = new ToolStripMenuItem();
        _miGrace5 = new ToolStripMenuItem();
        _miGrace10 = new ToolStripMenuItem();
        _miGrace20 = new ToolStripMenuItem();
        _miGrace30 = new ToolStripMenuItem();
        _speedMenu = new ToolStripMenuItem();
        _miSpeed1 = new ToolStripMenuItem();
        _miSpeed10 = new ToolStripMenuItem();
        _miSpeed60 = new ToolStripMenuItem();
        _miSpeed600 = new ToolStripMenuItem();
        _viewMenu = new ToolStripMenuItem();
        _miKiosk = new ToolStripMenuItem();
        _miHideTaskbar = new ToolStripMenuItem();
        _miPreventSleep = new ToolStripMenuItem();
        _miMagnetic = new ToolStripMenuItem();
        _miProperties = new ToolStripMenuItem();
        _viewSeparator = new ToolStripSeparator();
        _miExit = new ToolStripMenuItem();
        _pluginsMenu = new ToolStripMenuItem();
        _miReloadPlugins = new ToolStripMenuItem();
        _miOpenPluginsFolder = new ToolStripMenuItem();
        _statusStrip = new StatusStrip();
        _statusInfo = new ToolStripStatusLabel();
        _statusMode = new ToolStripStatusLabel();
        _kioskModeManager = new KioskModeManager(components);

        ((ISupportInitialize)_splitContainer).BeginInit();
        _splitContainer.Panel1.SuspendLayout();
        _splitContainer.SuspendLayout();
        _menuStrip.SuspendLayout();
        _statusStrip.SuspendLayout();
        ((ISupportInitialize)_kioskModeManager).BeginInit();
        SuspendLayout();

        // _splitContainer
        _splitContainer.Dock = DockStyle.Fill;
        _splitContainer.FixedPanel = FixedPanel.Panel2;
        _splitContainer.Location = new Point(0, 24);
        _splitContainer.Name = "_splitContainer";
        _splitContainer.Panel1.Controls.Add(_clock);
        _splitContainer.Panel2Collapsed = true;
        _splitContainer.Size = new Size(760, 714);
        _splitContainer.SplitterDistance = 520;
        _splitContainer.TabIndex = 0;

        // _clock
        _clock.BackColor = Color.Black;
        _clock.Dock = DockStyle.Fill;
        _clock.Location = new Point(0, 0);
        _clock.Name = "_clock";
        _clock.Size = new Size(520, 714);
        _clock.TabIndex = 0;

        // _propertyGrid
        _propertyGrid.Dock = DockStyle.Fill;
        _propertyGrid.Location = new Point(0, 0);
        _propertyGrid.Name = "_propertyGrid";
        _propertyGrid.Size = new Size(236, 714);
        _propertyGrid.TabIndex = 0;

        // _menuStrip
        _menuStrip.Items.AddRange(new ToolStripItem[]
        {
            _themeMenu,
            _motionMenu,
            _graceMenu,
            _speedMenu,
            _viewMenu,
            _pluginsMenu
        });

        _menuStrip.Location = new Point(0, 0);
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Size = new Size(760, 24);
        _menuStrip.TabIndex = 1;

        // _themeMenu
        _themeMenu.Name = "_themeMenu";
        _themeMenu.Text = "&Theme";

        // _motionMenu
        _motionMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _secondMotionMenu,
            _minuteMotionMenu,
            _hourMotionMenu
        });
        _motionMenu.Name = "_motionMenu";
        _motionMenu.Text = "&Motion";

        // _secondMotionMenu
        _secondMotionMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _miSecondCrawling,
            _miSecondSweep,
            _miSecondFastTick,
            _miSecondTick
        });
        _secondMotionMenu.Name = "_secondMotionMenu";
        _secondMotionMenu.Text = "&Second hand";
        _secondMotionMenu.DropDownOpening += OnSecondMotionOpening;

        // _miSecondCrawling
        _miSecondCrawling.Name = "_miSecondCrawling";
        _miSecondCrawling.Text = "Crawling";
        _miSecondCrawling.Click += OnSecondMotionClick;

        // _miSecondSweep
        _miSecondSweep.Name = "_miSecondSweep";
        _miSecondSweep.Text = "Sweep (glide)";
        _miSecondSweep.Click += OnSecondMotionClick;

        // _miSecondFastTick
        _miSecondFastTick.Name = "_miSecondFastTick";
        _miSecondFastTick.Text = "Fast Tick";
        _miSecondFastTick.Click += OnSecondMotionClick;

        // _miSecondTick
        _miSecondTick.Name = "_miSecondTick";
        _miSecondTick.Text = "Tick";
        _miSecondTick.Click += OnSecondMotionClick;

        // _minuteMotionMenu
        _minuteMotionMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _miMinuteCrawling,
            _miMinuteSweep,
            _miMinuteFastTick,
            _miMinuteTick
        });
        _minuteMotionMenu.Name = "_minuteMotionMenu";
        _minuteMotionMenu.Text = "&Minute hand";
        _minuteMotionMenu.DropDownOpening += OnMinuteMotionOpening;

        // _miMinuteCrawling
        _miMinuteCrawling.Name = "_miMinuteCrawling";
        _miMinuteCrawling.Text = "Crawling";
        _miMinuteCrawling.Click += OnMinuteMotionClick;

        // _miMinuteSweep
        _miMinuteSweep.Name = "_miMinuteSweep";
        _miMinuteSweep.Text = "Sweep (glide)";
        _miMinuteSweep.Click += OnMinuteMotionClick;

        // _miMinuteFastTick
        _miMinuteFastTick.Name = "_miMinuteFastTick";
        _miMinuteFastTick.Text = "Fast Tick";
        _miMinuteFastTick.Click += OnMinuteMotionClick;

        // _miMinuteTick
        _miMinuteTick.Name = "_miMinuteTick";
        _miMinuteTick.Text = "Tick";
        _miMinuteTick.Click += OnMinuteMotionClick;

        // _hourMotionMenu
        _hourMotionMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _miHourCrawling,
            _miHourSweep,
            _miHourFastTick,
            _miHourTick
        });
        _hourMotionMenu.Name = "_hourMotionMenu";
        _hourMotionMenu.Text = "&Hour hand";
        _hourMotionMenu.DropDownOpening += OnHourMotionOpening;

        // _miHourCrawling
        _miHourCrawling.Name = "_miHourCrawling";
        _miHourCrawling.Text = "Crawling";
        _miHourCrawling.Click += OnHourMotionClick;

        // _miHourSweep
        _miHourSweep.Name = "_miHourSweep";
        _miHourSweep.Text = "Sweep (glide)";
        _miHourSweep.Click += OnHourMotionClick;

        // _miHourFastTick
        _miHourFastTick.Name = "_miHourFastTick";
        _miHourFastTick.Text = "Fast Tick";
        _miHourFastTick.Click += OnHourMotionClick;

        // _miHourTick
        _miHourTick.Name = "_miHourTick";
        _miHourTick.Text = "Tick";
        _miHourTick.Click += OnHourMotionClick;

        // _graceMenu
        _graceMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _miGrace1,
            _miGrace5,
            _miGrace10,
            _miGrace20,
            _miGrace30
        });
        _graceMenu.Name = "_graceMenu";
        _graceMenu.Text = "&Grace";

        // _miGrace1
        _miGrace1.Name = "_miGrace1";
        _miGrace1.Tag = 1;
        _miGrace1.Text = "1 second";
        _miGrace1.Click += OnGraceClick;

        // _miGrace5
        _miGrace5.Name = "_miGrace5";
        _miGrace5.Tag = 5;
        _miGrace5.Text = "5 seconds";
        _miGrace5.Click += OnGraceClick;

        // _miGrace10
        _miGrace10.Name = "_miGrace10";
        _miGrace10.Tag = 10;
        _miGrace10.Text = "10 seconds";
        _miGrace10.Click += OnGraceClick;

        // _miGrace20
        _miGrace20.Name = "_miGrace20";
        _miGrace20.Tag = 20;
        _miGrace20.Text = "20 seconds";
        _miGrace20.Click += OnGraceClick;

        // _miGrace30
        _miGrace30.Name = "_miGrace30";
        _miGrace30.Tag = 30;
        _miGrace30.Text = "30 seconds";
        _miGrace30.Click += OnGraceClick;

        // _speedMenu
        _speedMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _miSpeed1,
            _miSpeed10,
            _miSpeed60,
            _miSpeed600
        });
        _speedMenu.Name = "_speedMenu";
        _speedMenu.Text = "&Speed";

        // _miSpeed1
        _miSpeed1.Name = "_miSpeed1";
        _miSpeed1.Tag = 1d;
        _miSpeed1.Text = "Real time (1x)";
        _miSpeed1.Click += OnSpeedClick;

        // _miSpeed10
        _miSpeed10.Name = "_miSpeed10";
        _miSpeed10.Tag = 10d;
        _miSpeed10.Text = "Fast (10x)";
        _miSpeed10.Click += OnSpeedClick;

        // _miSpeed60
        _miSpeed60.Name = "_miSpeed60";
        _miSpeed60.Tag = 60d;
        _miSpeed60.Text = "Faster (60x)";
        _miSpeed60.Click += OnSpeedClick;

        // _miSpeed600
        _miSpeed600.Name = "_miSpeed600";
        _miSpeed600.Tag = 600d;
        _miSpeed600.Text = "Very fast (600x)";
        _miSpeed600.Click += OnSpeedClick;

        // _viewMenu
        _viewMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _miKiosk,
            _miHideTaskbar,
            _miPreventSleep,
            _miMagnetic,
            _miProperties,
            _viewSeparator,
            _miExit
        });
        _viewMenu.Name = "_viewMenu";
        _viewMenu.Text = "&View";

        // _miKiosk
        _miKiosk.Name = "_miKiosk";
        _miKiosk.ShortcutKeys = Keys.F11;
        _miKiosk.Text = "&Kiosk mode";
        _miKiosk.Click += OnKioskClick;

        // _miHideTaskbar
        _miHideTaskbar.Name = "_miHideTaskbar";
        _miHideTaskbar.Text = "Hide &taskbar in kiosk";
        _miHideTaskbar.Click += OnHideTaskbarClick;

        // _miPreventSleep
        _miPreventSleep.Name = "_miPreventSleep";
        _miPreventSleep.Text = "Prevent &sleep / screensaver";
        _miPreventSleep.Click += OnPreventSleepClick;

        // _miMagnetic
        _miMagnetic.Name = "_miMagnetic";
        _miMagnetic.Text = "&Magnetic numerals";
        _miMagnetic.Click += OnMagneticClick;

        // _miProperties
        _miProperties.Name = "_miProperties";
        _miProperties.Text = "&Properties panel";
        _miProperties.Click += OnTogglePropertiesClick;

        // _miExit
        _miExit.Name = "_miExit";
        _miExit.ShortcutKeys = Keys.Alt | Keys.F4;
        _miExit.Text = "E&xit";
        _miExit.Click += OnExitClick;

        // _pluginsMenu
        _pluginsMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            _miReloadPlugins,
            _miOpenPluginsFolder
        });

        _pluginsMenu.Name = "_pluginsMenu";
        _pluginsMenu.Text = "&Plug-ins";

        // _miReloadPlugins
        _miReloadPlugins.Name = "_miReloadPlugins";
        _miReloadPlugins.Text = "&Reload plug-ins";
        _miReloadPlugins.Click += OnReloadPluginsClick;

        // _miOpenPluginsFolder
        _miOpenPluginsFolder.Name = "_miOpenPluginsFolder";
        _miOpenPluginsFolder.Text = "&Open plug-ins folder";
        _miOpenPluginsFolder.Click += OnOpenPluginsFolderClick;

        // _statusStrip
        _statusStrip.Items.AddRange(new ToolStripItem[]
        {
            _statusInfo,
            _statusMode
        });
        _statusStrip.Location = new Point(0, 738);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Size = new Size(760, 22);
        _statusStrip.TabIndex = 2;

        // _statusInfo
        _statusInfo.Name = "_statusInfo";
        _statusInfo.Spring = true;
        _statusInfo.Text = "Ready.";
        _statusInfo.TextAlign = ContentAlignment.MiddleLeft;

        // _statusMode
        _statusMode.Name = "_statusMode";
        _statusMode.Text = "Windowed";

        // _kioskModeManager
        _kioskModeManager.ContainerControl = this;
        _kioskModeManager.EscapeExitsFullScreen = true;
        _kioskModeManager.HideTaskbar = true;
        _kioskModeManager.ToggleFullScreenKey = Keys.F11;
        _kioskModeManager.FullScreenChanged += OnKioskFullScreenChanged;
        _kioskModeManager.Wakeup += OnKioskWakeup;

        // FormMain
        AutoScaleDimensions = new SizeF(7f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(760, 760);
        Controls.Add(_splitContainer);
        Controls.Add(_statusStrip);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;
        MinimumSize = new Size(340, 340);
        Name = "FormMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "WARP DirectX Wall Clock";

        ((ISupportInitialize)_kioskModeManager).EndInit();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        _splitContainer.Panel1.ResumeLayout(false);
        ((ISupportInitialize)_splitContainer).EndInit();
        _splitContainer.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private SplitContainer _splitContainer;
    private WarpClockControl _clock;
    private PropertyGrid _propertyGrid;

    private MenuStrip _menuStrip;
    private ToolStripMenuItem _themeMenu;

    private ToolStripMenuItem _motionMenu;
    private ToolStripMenuItem _secondMotionMenu;
    private ToolStripMenuItem _miSecondCrawling;
    private ToolStripMenuItem _miSecondSweep;
    private ToolStripMenuItem _miSecondFastTick;
    private ToolStripMenuItem _miSecondTick;
    private ToolStripMenuItem _minuteMotionMenu;
    private ToolStripMenuItem _miMinuteCrawling;
    private ToolStripMenuItem _miMinuteSweep;
    private ToolStripMenuItem _miMinuteFastTick;
    private ToolStripMenuItem _miMinuteTick;
    private ToolStripMenuItem _hourMotionMenu;
    private ToolStripMenuItem _miHourCrawling;
    private ToolStripMenuItem _miHourSweep;
    private ToolStripMenuItem _miHourFastTick;
    private ToolStripMenuItem _miHourTick;

    private ToolStripMenuItem _graceMenu;
    private ToolStripMenuItem _miGrace1;
    private ToolStripMenuItem _miGrace5;
    private ToolStripMenuItem _miGrace10;
    private ToolStripMenuItem _miGrace20;
    private ToolStripMenuItem _miGrace30;

    private ToolStripMenuItem _speedMenu;
    private ToolStripMenuItem _miSpeed1;
    private ToolStripMenuItem _miSpeed10;
    private ToolStripMenuItem _miSpeed60;
    private ToolStripMenuItem _miSpeed600;

    private ToolStripMenuItem _viewMenu;
    private ToolStripMenuItem _miKiosk;
    private ToolStripMenuItem _miHideTaskbar;
    private ToolStripMenuItem _miPreventSleep;
    private ToolStripMenuItem _miMagnetic;
    private ToolStripMenuItem _miProperties;
    private ToolStripSeparator _viewSeparator;
    private ToolStripMenuItem _miExit;

    private ToolStripMenuItem _pluginsMenu;
    private ToolStripMenuItem _miReloadPlugins;
    private ToolStripMenuItem _miOpenPluginsFolder;

    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _statusInfo;
    private ToolStripStatusLabel _statusMode;

    private KioskModeManager _kioskModeManager;
}
