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
        _tickerBand = new TickerBandControl();
        _propertyGrid = new PropertyGrid();
        _menuStrip = new MenuStrip();
        _fileMenu = new ToolStripMenuItem();
        _miCreateNewThemeSet = new ToolStripMenuItem();
        _miEditCurrentThemeSet = new ToolStripMenuItem();
        _miLoadThemeSet = new ToolStripMenuItem();
        _miSaveThemeSet = new ToolStripMenuItem();
        _fileReloadSeparator = new ToolStripSeparator();
        _fileExitSeparator = new ToolStripSeparator();
        _themeMenu = new ToolStripMenuItem();
        _speedMenu = new ToolStripMenuItem();
        _miSpeed1 = new ToolStripMenuItem();
        _miSpeed10 = new ToolStripMenuItem();
        _miSpeed60 = new ToolStripMenuItem();
        _miSpeed600 = new ToolStripMenuItem();
        _viewMenu = new ToolStripMenuItem();
        _toolsMenu = new ToolStripMenuItem();
        _miOptions = new ToolStripMenuItem();
        _miKiosk = new ToolStripMenuItem();
        _miOledView = new ToolStripMenuItem();
        _miRecordFramerate = new ToolStripMenuItem();
        _miMagnetic = new ToolStripMenuItem();
        _miVSync = new ToolStripMenuItem();
        _handMovementMenu = new ToolStripMenuItem();
        _miMotionCrawl = new ToolStripMenuItem();
        _miMotionGlide = new ToolStripMenuItem();
        _miMotionFastTick = new ToolStripMenuItem();
        _miMotionTick = new ToolStripMenuItem();
        _themeInfoMenu = new ToolStripMenuItem();
        _miInfoNever = new ToolStripMenuItem();
        _miInfoFixed = new ToolStripMenuItem();
        _miInfoFadeFixed = new ToolStripMenuItem();
        _miInfoFadeSides = new ToolStripMenuItem();
        _infoPlacementSeparator = new ToolStripSeparator();
        _placementMenu = new ToolStripMenuItem();
        _miPlaceLeft = new ToolStripMenuItem();
        _miPlaceRight = new ToolStripMenuItem();
        _miPlaceFace = new ToolStripMenuItem();
        _miProperties = new ToolStripMenuItem();
        _miExit = new ToolStripMenuItem();
        _kioskMenu = new ToolStripMenuItem();
        _fullScreenToggleKeysMenu = new ToolStripMenuItem();
        _miToggleControlEnter = new ToolStripMenuItem();
        _miToggleControlShiftEnter = new ToolStripMenuItem();
        _miToggleF11 = new ToolStripMenuItem();
        _miToggleF12 = new ToolStripMenuItem();
        _miAlwaysOn = new ToolStripMenuItem();
        _miAllowEscape = new ToolStripMenuItem();
        _mousePointerHideDelayMenu = new ToolStripMenuItem();
        _miMouseHideNever = new ToolStripMenuItem();
        _miMouseHide1000 = new ToolStripMenuItem();
        _miMouseHide2000 = new ToolStripMenuItem();
        _miMouseHide5000 = new ToolStripMenuItem();
        _miMouseHide10000 = new ToolStripMenuItem();
        _miTopMostInFullScreen = new ToolStripMenuItem();
        _kioskChromeSeparator = new ToolStripSeparator();
        _miHideWindowsChrome = new ToolStripMenuItem();
        _miReloadPlugins = new ToolStripMenuItem();
        _statusStrip = new StatusStrip();
        _statusInfo = new ToolStripStatusLabel();
        _statusFps = new ToolStripStatusLabel();
        _statusMode = new ToolStripStatusLabel();
        _kioskModeManager = new KioskModeManager(components);
        _fpsTimer = new System.Windows.Forms.Timer(components);
        toolStripSeparator1 = new ToolStripSeparator();
        ((ISupportInitialize)_splitContainer).BeginInit();
        _splitContainer.Panel1.SuspendLayout();
        _splitContainer.SuspendLayout();
        _menuStrip.SuspendLayout();
        _statusStrip.SuspendLayout();
        ((ISupportInitialize)_kioskModeManager).BeginInit();
        SuspendLayout();
        // 
        // _splitContainer
        // 
        _splitContainer.Dock = DockStyle.Fill;
        _splitContainer.FixedPanel = FixedPanel.Panel2;
        _splitContainer.Location = new Point(0, 44);
        _splitContainer.Margin = new Padding(6, 6, 6, 6);
        _splitContainer.Name = "_splitContainer";
        // 
        // _splitContainer.Panel1
        // 
        _splitContainer.Panel1.Controls.Add(_clock);
        _splitContainer.Panel2Collapsed = true;
        _splitContainer.Size = new Size(1411, 1535);
        _splitContainer.SplitterDistance = 966;
        _splitContainer.SplitterWidth = 7;
        _splitContainer.TabIndex = 0;
        // 
        // _clock
        // 
        _clock.BackColor = Color.Black;
        _clock.Dock = DockStyle.Fill;
        _clock.Location = new Point(0, 0);
        _clock.Margin = new Padding(6, 6, 6, 6);
        _clock.Name = "_clock";
        _clock.Size = new Size(1411, 1535);
        _clock.TabIndex = 0;
        _clock.TabStop = false;
        // 
        // _propertyGrid
        // 
        _propertyGrid.Dock = DockStyle.Fill;
        _propertyGrid.Location = new Point(0, 0);
        _propertyGrid.Name = "_propertyGrid";
        _propertyGrid.Size = new Size(236, 714);
        _propertyGrid.TabIndex = 0;
        //
        // _tickerBand
        //
        _tickerBand.Dock = DockStyle.Bottom;
        _tickerBand.Location = new Point(0, 1537);
        _tickerBand.Name = "_tickerBand";
        _tickerBand.Size = new Size(1411, 42);
        _tickerBand.TabIndex = 3;
        _tickerBand.Visible = false;
        // 
        // _menuStrip
        // 
        _menuStrip.ImageScalingSize = new Size(32, 32);
        _menuStrip.Items.AddRange(new ToolStripItem[] { _fileMenu, _themeMenu, _viewMenu, _speedMenu, _toolsMenu, _kioskMenu });
        _menuStrip.Location = new Point(0, 0);
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Padding = new Padding(11, 4, 0, 4);
        _menuStrip.Size = new Size(1411, 44);
        _menuStrip.TabIndex = 1;
        //
        // _fileMenu
        //
        _fileMenu.DropDownItems.AddRange(new ToolStripItem[] { _miCreateNewThemeSet, _miEditCurrentThemeSet, _miLoadThemeSet, _miSaveThemeSet, _fileReloadSeparator, _miReloadPlugins, _fileExitSeparator, _miExit });
        _fileMenu.Name = "_fileMenu";
        _fileMenu.Size = new Size(73, 36);
        _fileMenu.Text = "&File";
        //
        // _miCreateNewThemeSet
        //
        _miCreateNewThemeSet.Name = "_miCreateNewThemeSet";
        _miCreateNewThemeSet.Size = new Size(393, 44);
        _miCreateNewThemeSet.Text = "&New Themeset...";
        _miCreateNewThemeSet.Click += OnCreateNewThemeSetClick;
        //
        // _miEditCurrentThemeSet
        //
        _miEditCurrentThemeSet.Name = "_miEditCurrentThemeSet";
        _miEditCurrentThemeSet.Size = new Size(393, 44);
        _miEditCurrentThemeSet.Text = "&Edit Current Themeset...";
        _miEditCurrentThemeSet.Click += OnEditCurrentThemeSetClick;
        //
        // _miLoadThemeSet
        //
        _miLoadThemeSet.Name = "_miLoadThemeSet";
        _miLoadThemeSet.Size = new Size(393, 44);
        _miLoadThemeSet.Text = "&Load Themeset...";
        _miLoadThemeSet.Click += OnLoadThemeSetClick;
        //
        // _miSaveThemeSet
        //
        _miSaveThemeSet.Name = "_miSaveThemeSet";
        _miSaveThemeSet.Size = new Size(393, 44);
        _miSaveThemeSet.Text = "&Save Themeset";
        _miSaveThemeSet.Click += OnSaveThemeSetClick;
        //
        // _fileReloadSeparator
        //
        _fileReloadSeparator.Name = "_fileReloadSeparator";
        _fileReloadSeparator.Size = new Size(390, 6);
        // 
        // _themeMenu
        // 
        _themeMenu.Name = "_themeMenu";
        _themeMenu.Size = new Size(108, 36);
        _themeMenu.Text = "&Theme";
        // 
        // _speedMenu
        // 
        _speedMenu.DropDownItems.AddRange(new ToolStripItem[] { _miSpeed1, _miSpeed10, _miSpeed60, _miSpeed600 });
        _speedMenu.Name = "_speedMenu";
        _speedMenu.Size = new Size(101, 36);
        _speedMenu.Text = "&Speed";
        // 
        // _miSpeed1
        // 
        _miSpeed1.Name = "_miSpeed1";
        _miSpeed1.Size = new Size(309, 44);
        _miSpeed1.Tag = 1D;
        _miSpeed1.Text = "Real time (1x)";
        _miSpeed1.Click += OnSpeedClick;
        // 
        // _miSpeed10
        // 
        _miSpeed10.Name = "_miSpeed10";
        _miSpeed10.Size = new Size(309, 44);
        _miSpeed10.Tag = 10D;
        _miSpeed10.Text = "Fast (10x)";
        _miSpeed10.Click += OnSpeedClick;
        // 
        // _miSpeed60
        // 
        _miSpeed60.Name = "_miSpeed60";
        _miSpeed60.Size = new Size(309, 44);
        _miSpeed60.Tag = 60D;
        _miSpeed60.Text = "Faster (60x)";
        _miSpeed60.Click += OnSpeedClick;
        // 
        // _miSpeed600
        // 
        _miSpeed600.Name = "_miSpeed600";
        _miSpeed600.Size = new Size(309, 44);
        _miSpeed600.Tag = 600D;
        _miSpeed600.Text = "Very fast (600x)";
        _miSpeed600.Click += OnSpeedClick;
        // 
        // _viewMenu
        // 
        _viewMenu.DropDownItems.AddRange(new ToolStripItem[] { _miProperties, _miKiosk, toolStripSeparator1, _miOledView, _miRecordFramerate, _miVSync, _miMagnetic, _handMovementMenu, _themeInfoMenu });
        _viewMenu.Name = "_viewMenu";
        _viewMenu.Size = new Size(85, 36);
        _viewMenu.Text = "&View";
        //
        // _toolsMenu
        //
        _toolsMenu.DropDownItems.AddRange(new ToolStripItem[] { _miOptions });
        _toolsMenu.Name = "_toolsMenu";
        _toolsMenu.Size = new Size(86, 36);
        _toolsMenu.Text = "&Tools";
        //
        // _miOptions
        //
        _miOptions.Name = "_miOptions";
        _miOptions.Size = new Size(187, 44);
        _miOptions.Text = "&Options";
        _miOptions.Click += OnOptionsClick;
        // 
        // _miKiosk
        // 
        _miKiosk.Name = "_miKiosk";
        _miKiosk.Size = new Size(352, 44);
        _miKiosk.Text = "&Toggle full screen";
        _miKiosk.Click += OnKioskClick;
        //
        // _miOledView
        //
        _miOledView.Name = "_miOledView";
        _miOledView.Size = new Size(352, 44);
        _miOledView.Text = "&OLED view";
        _miOledView.Click += OnOledViewClick;
        //
        // _miRecordFramerate
        //
        _miRecordFramerate.Name = "_miRecordFramerate";
        _miRecordFramerate.Size = new Size(352, 44);
        _miRecordFramerate.Text = "&Record frame rate";
        _miRecordFramerate.Click += OnRecordFramerateClick;
        // 
        // _miMagnetic
        // 
        _miMagnetic.Name = "_miMagnetic";
        _miMagnetic.Size = new Size(352, 44);
        _miMagnetic.Text = "&Magnetic numerals";
        _miMagnetic.Click += OnMagneticClick;
        // 
        // _miVSync
        // 
        _miVSync.Name = "_miVSync";
        _miVSync.Size = new Size(352, 44);
        _miVSync.Text = "&VSync";
        _miVSync.Click += OnVSyncClick;
        //
        // _handMovementMenu
        //
        _handMovementMenu.DropDownItems.AddRange(new ToolStripItem[] { _miMotionCrawl, _miMotionGlide, _miMotionFastTick, _miMotionTick });
        _handMovementMenu.Name = "_handMovementMenu";
        _handMovementMenu.Size = new Size(352, 44);
        _handMovementMenu.Text = "&Hand movement";
        //
        // _miMotionCrawl
        //
        _miMotionCrawl.Name = "_miMotionCrawl";
        _miMotionCrawl.Size = new Size(352, 44);
        _miMotionCrawl.Tag = "Crawling";
        _miMotionCrawl.Text = "&Crawl (move and pause)";
        _miMotionCrawl.Click += OnHandMovementClick;
        //
        // _miMotionGlide
        //
        _miMotionGlide.Name = "_miMotionGlide";
        _miMotionGlide.Size = new Size(352, 44);
        _miMotionGlide.Tag = "Sweep";
        _miMotionGlide.Text = "&Glide continuously";
        _miMotionGlide.Click += OnHandMovementClick;
        //
        // _miMotionFastTick
        //
        _miMotionFastTick.Name = "_miMotionFastTick";
        _miMotionFastTick.Size = new Size(352, 44);
        _miMotionFastTick.Tag = "FastTick";
        _miMotionFastTick.Text = "&Fast tick";
        _miMotionFastTick.Click += OnHandMovementClick;
        //
        // _miMotionTick
        //
        _miMotionTick.Name = "_miMotionTick";
        _miMotionTick.Size = new Size(352, 44);
        _miMotionTick.Tag = "Tick";
        _miMotionTick.Text = "&Tick";
        _miMotionTick.Click += OnHandMovementClick;
        // 
        // _themeInfoMenu
        // 
        _themeInfoMenu.DropDownItems.AddRange(new ToolStripItem[] { _miInfoNever, _miInfoFixed, _miInfoFadeFixed, _miInfoFadeSides, _infoPlacementSeparator, _placementMenu });
        _themeInfoMenu.Name = "_themeInfoMenu";
        _themeInfoMenu.Size = new Size(352, 44);
        _themeInfoMenu.Text = "Theme &info";
        _themeInfoMenu.DropDownOpening += OnThemeInfoOpening;
        // 
        // _miInfoNever
        // 
        _miInfoNever.Name = "_miInfoNever";
        _miInfoNever.Size = new Size(359, 44);
        _miInfoNever.Text = "Never";
        _miInfoNever.Click += OnThemeInfoModeClick;
        // 
        // _miInfoFixed
        // 
        _miInfoFixed.Name = "_miInfoFixed";
        _miInfoFixed.Size = new Size(359, 44);
        _miInfoFixed.Text = "Fixed position";
        _miInfoFixed.Click += OnThemeInfoModeClick;
        // 
        // _miInfoFadeFixed
        // 
        _miInfoFadeFixed.Name = "_miInfoFadeFixed";
        _miInfoFadeFixed.Size = new Size(359, 44);
        _miInfoFadeFixed.Text = "Fade in/out (fixed)";
        _miInfoFadeFixed.Click += OnThemeInfoModeClick;
        // 
        // _miInfoFadeSides
        // 
        _miInfoFadeSides.Name = "_miInfoFadeSides";
        _miInfoFadeSides.Size = new Size(359, 44);
        _miInfoFadeSides.Text = "Fade alternate sides";
        _miInfoFadeSides.Click += OnThemeInfoModeClick;
        // 
        // _infoPlacementSeparator
        // 
        _infoPlacementSeparator.Name = "_infoPlacementSeparator";
        _infoPlacementSeparator.Size = new Size(356, 6);
        // 
        // _placementMenu
        // 
        _placementMenu.DropDownItems.AddRange(new ToolStripItem[] { _miPlaceLeft, _miPlaceRight, _miPlaceFace });
        _placementMenu.Name = "_placementMenu";
        _placementMenu.Size = new Size(359, 44);
        _placementMenu.Text = "Placement";
        _placementMenu.DropDownOpening += OnThemeInfoPlacementOpening;
        // 
        // _miPlaceLeft
        // 
        _miPlaceLeft.Name = "_miPlaceLeft";
        _miPlaceLeft.Size = new Size(329, 44);
        _miPlaceLeft.Text = "Left screen side";
        _miPlaceLeft.Click += OnThemeInfoPlacementClick;
        // 
        // _miPlaceRight
        // 
        _miPlaceRight.Name = "_miPlaceRight";
        _miPlaceRight.Size = new Size(329, 44);
        _miPlaceRight.Text = "Right screen side";
        _miPlaceRight.Click += OnThemeInfoPlacementClick;
        // 
        // _miPlaceFace
        // 
        _miPlaceFace.Name = "_miPlaceFace";
        _miPlaceFace.Size = new Size(329, 44);
        _miPlaceFace.Text = "On clock face";
        _miPlaceFace.Click += OnThemeInfoPlacementClick;
        // 
        // _miProperties
        // 
        _miProperties.Name = "_miProperties";
        _miProperties.Size = new Size(352, 44);
        _miProperties.Text = "&Properties panel";
        _miProperties.Click += OnTogglePropertiesClick;
        // 
        // _miExit
        // 
        _miExit.Name = "_miExit";
        _miExit.ShortcutKeys = Keys.Alt | Keys.F4;
        _miExit.Size = new Size(393, 44);
        _miExit.Text = "E&xit";
        _miExit.Click += OnExitClick;
        // 
        // _kioskMenu
        // 
        _kioskMenu.DropDownItems.AddRange(new ToolStripItem[] { _fullScreenToggleKeysMenu, _miAlwaysOn, _miAllowEscape, _mousePointerHideDelayMenu, _miTopMostInFullScreen, _kioskChromeSeparator, _miHideWindowsChrome });
        _kioskMenu.Name = "_kioskMenu";
        _kioskMenu.Size = new Size(159, 36);
        _kioskMenu.Text = "&Kiosk mode";
        _kioskMenu.DropDownOpening += OnKioskMenuOpening;
        // 
        // _fullScreenToggleKeysMenu
        // 
        _fullScreenToggleKeysMenu.DropDownItems.AddRange(new ToolStripItem[] { _miToggleControlEnter, _miToggleControlShiftEnter, _miToggleF11, _miToggleF12 });
        _fullScreenToggleKeysMenu.Name = "_fullScreenToggleKeysMenu";
        _fullScreenToggleKeysMenu.Size = new Size(531, 44);
        _fullScreenToggleKeysMenu.Text = "Full-screen &toggle keys";
        // 
        // _miToggleControlEnter
        // 
        _miToggleControlEnter.Name = "_miToggleControlEnter";
        _miToggleControlEnter.Size = new Size(320, 44);
        _miToggleControlEnter.Tag = Keys.Control | Keys.Enter;
        _miToggleControlEnter.Text = "Ctrl+Enter";
        _miToggleControlEnter.Click += OnFullScreenToggleKeysClick;
        // 
        // _miToggleControlShiftEnter
        // 
        _miToggleControlShiftEnter.Name = "_miToggleControlShiftEnter";
        _miToggleControlShiftEnter.Size = new Size(320, 44);
        _miToggleControlShiftEnter.Tag = Keys.Control | Keys.Shift | Keys.Enter;
        _miToggleControlShiftEnter.Text = "Ctrl+Shift+Enter";
        _miToggleControlShiftEnter.Click += OnFullScreenToggleKeysClick;
        // 
        // _miToggleF11
        // 
        _miToggleF11.Name = "_miToggleF11";
        _miToggleF11.Size = new Size(320, 44);
        _miToggleF11.Tag = Keys.F11;
        _miToggleF11.Text = "F11";
        _miToggleF11.Click += OnFullScreenToggleKeysClick;
        // 
        // _miToggleF12
        // 
        _miToggleF12.Name = "_miToggleF12";
        _miToggleF12.Size = new Size(320, 44);
        _miToggleF12.Tag = Keys.F12;
        _miToggleF12.Text = "F12";
        _miToggleF12.Click += OnFullScreenToggleKeysClick;
        // 
        // _miAlwaysOn
        // 
        _miAlwaysOn.Name = "_miAlwaysOn";
        _miAlwaysOn.Size = new Size(531, 44);
        _miAlwaysOn.Text = "&Always on (avoids sleep / hibernate)";
        _miAlwaysOn.Click += OnAlwaysOnClick;
        // 
        // _miAllowEscape
        // 
        _miAllowEscape.Name = "_miAllowEscape";
        _miAllowEscape.Size = new Size(531, 44);
        _miAllowEscape.Text = "Allow &Escape to exit kiosk mode";
        _miAllowEscape.Click += OnAllowEscapeClick;
        // 
        // _mousePointerHideDelayMenu
        // 
        _mousePointerHideDelayMenu.DropDownItems.AddRange(new ToolStripItem[] { _miMouseHideNever, _miMouseHide1000, _miMouseHide2000, _miMouseHide5000, _miMouseHide10000 });
        _mousePointerHideDelayMenu.Name = "_mousePointerHideDelayMenu";
        _mousePointerHideDelayMenu.Size = new Size(531, 44);
        _mousePointerHideDelayMenu.Text = "Hide mouse &pointer after";
        // 
        // _miMouseHideNever
        // 
        _miMouseHideNever.Name = "_miMouseHideNever";
        _miMouseHideNever.Size = new Size(294, 44);
        _miMouseHideNever.Tag = 0;
        _miMouseHideNever.Text = "Don't hide (0)";
        _miMouseHideNever.Click += OnMousePointerHideDelayClick;
        // 
        // _miMouseHide1000
        // 
        _miMouseHide1000.Name = "_miMouseHide1000";
        _miMouseHide1000.Size = new Size(294, 44);
        _miMouseHide1000.Tag = 1000;
        _miMouseHide1000.Text = "1,000 ms";
        _miMouseHide1000.Click += OnMousePointerHideDelayClick;
        // 
        // _miMouseHide2000
        // 
        _miMouseHide2000.Name = "_miMouseHide2000";
        _miMouseHide2000.Size = new Size(294, 44);
        _miMouseHide2000.Tag = 2000;
        _miMouseHide2000.Text = "2,000 ms";
        _miMouseHide2000.Click += OnMousePointerHideDelayClick;
        // 
        // _miMouseHide5000
        // 
        _miMouseHide5000.Name = "_miMouseHide5000";
        _miMouseHide5000.Size = new Size(294, 44);
        _miMouseHide5000.Tag = 5000;
        _miMouseHide5000.Text = "5,000 ms";
        _miMouseHide5000.Click += OnMousePointerHideDelayClick;
        // 
        // _miMouseHide10000
        // 
        _miMouseHide10000.Name = "_miMouseHide10000";
        _miMouseHide10000.Size = new Size(294, 44);
        _miMouseHide10000.Tag = 10000;
        _miMouseHide10000.Text = "10,000 ms";
        _miMouseHide10000.Click += OnMousePointerHideDelayClick;
        // 
        // _miTopMostInFullScreen
        // 
        _miTopMostInFullScreen.Name = "_miTopMostInFullScreen";
        _miTopMostInFullScreen.Size = new Size(531, 44);
        _miTopMostInFullScreen.Text = "&Topmost in full screen";
        _miTopMostInFullScreen.Click += OnTopMostInFullScreenClick;
        // 
        // _kioskChromeSeparator
        // 
        _kioskChromeSeparator.Name = "_kioskChromeSeparator";
        _kioskChromeSeparator.Size = new Size(528, 6);
        // 
        // _miHideWindowsChrome
        // 
        _miHideWindowsChrome.Name = "_miHideWindowsChrome";
        _miHideWindowsChrome.Size = new Size(531, 44);
        _miHideWindowsChrome.Text = "&Hide Windows chrome (ESC exits)";
        _miHideWindowsChrome.Click += OnHideWindowsChromeClick;
        // 
        // _miReloadPlugins
        // 
        _miReloadPlugins.Name = "_miReloadPlugins";
        _miReloadPlugins.Size = new Size(393, 44);
        _miReloadPlugins.Text = "&Reload Plug-Ins";
        _miReloadPlugins.Click += OnReloadPluginsClick;
        // 
        // _fileExitSeparator
        // 
        _fileExitSeparator.Name = "_fileExitSeparator";
        _fileExitSeparator.Size = new Size(390, 6);
        // 
        // _statusStrip
        // 
        _statusStrip.ImageScalingSize = new Size(32, 32);
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusInfo, _statusFps, _statusMode });
        _statusStrip.Location = new Point(0, 1579);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Padding = new Padding(2, 0, 56, 0);
        _statusStrip.Size = new Size(1411, 42);
        _statusStrip.TabIndex = 2;
        // 
        // _statusInfo
        // 
        _statusInfo.Name = "_statusInfo";
        _statusInfo.Size = new Size(1135, 32);
        _statusInfo.Spring = true;
        _statusInfo.Text = "Ready.";
        _statusInfo.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _statusFps
        // 
        _statusFps.AutoSize = false;
        _statusFps.Name = "_statusFps";
        _statusFps.Size = new Size(90, 32);
        _statusFps.Text = "— fps";
        _statusFps.TextAlign = ContentAlignment.MiddleRight;
        // 
        // _statusMode
        // 
        _statusMode.Name = "_statusMode";
        _statusMode.Size = new Size(128, 32);
        _statusMode.Text = "Windowed";
        // 
        // _kioskModeManager
        // 
        _kioskModeManager.ContainerControl = this;
        _kioskModeManager.MousePointerAutoHideDelay = 5000;
        _kioskModeManager.ToggleFullScreenKeys = Keys.Control | Keys.Shift | Keys.Enter;
        _kioskModeManager.TopMostInFullScreen = true;
        _kioskModeManager.FullScreenChanged += OnKioskFullScreenChanged;
        // 
        // _fpsTimer
        // 
        _fpsTimer.Interval = 200;
        _fpsTimer.Tick += OnFpsTimerTick;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(349, 6);
        // 
        // FormMain
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1411, 1621);
        Controls.Add(_splitContainer);
        Controls.Add(_tickerBand);
        Controls.Add(_statusStrip);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;
        Margin = new Padding(6, 6, 6, 6);
        MinimumSize = new Size(609, 645);
        Name = "FormMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "WARP DirectX Wall Clock";
        _splitContainer.Panel1.ResumeLayout(false);
        ((ISupportInitialize)_splitContainer).EndInit();
        _splitContainer.ResumeLayout(false);
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        ((ISupportInitialize)_kioskModeManager).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private SplitContainer _splitContainer;
    private WarpClockControl _clock;
    private PropertyGrid _propertyGrid;
    private TickerBandControl _tickerBand;

    private MenuStrip _menuStrip;
    private ToolStripMenuItem _fileMenu;
    private ToolStripMenuItem _miCreateNewThemeSet;
    private ToolStripMenuItem _miEditCurrentThemeSet;
    private ToolStripMenuItem _miLoadThemeSet;
    private ToolStripMenuItem _miSaveThemeSet;
    private ToolStripSeparator _fileReloadSeparator;
    private ToolStripSeparator _fileExitSeparator;
    private ToolStripMenuItem _themeMenu;

    private ToolStripMenuItem _speedMenu;
    private ToolStripMenuItem _miSpeed1;
    private ToolStripMenuItem _miSpeed10;
    private ToolStripMenuItem _miSpeed60;
    private ToolStripMenuItem _miSpeed600;

    private ToolStripMenuItem _viewMenu;
    private ToolStripMenuItem _toolsMenu;
    private ToolStripMenuItem _miOptions;
    private ToolStripMenuItem _miKiosk;
    private ToolStripMenuItem _miOledView;
    private ToolStripMenuItem _miRecordFramerate;
    private ToolStripMenuItem _miMagnetic;
    private ToolStripMenuItem _miVSync;
    private ToolStripMenuItem _handMovementMenu;
    private ToolStripMenuItem _miMotionCrawl;
    private ToolStripMenuItem _miMotionGlide;
    private ToolStripMenuItem _miMotionFastTick;
    private ToolStripMenuItem _miMotionTick;
    private ToolStripMenuItem _themeInfoMenu;
    private ToolStripMenuItem _miInfoNever;
    private ToolStripMenuItem _miInfoFixed;
    private ToolStripMenuItem _miInfoFadeFixed;
    private ToolStripMenuItem _miInfoFadeSides;
    private ToolStripSeparator _infoPlacementSeparator;
    private ToolStripMenuItem _placementMenu;
    private ToolStripMenuItem _miPlaceLeft;
    private ToolStripMenuItem _miPlaceRight;
    private ToolStripMenuItem _miPlaceFace;
    private ToolStripMenuItem _miProperties;
    private ToolStripMenuItem _miExit;

    private ToolStripMenuItem _kioskMenu;
    private ToolStripMenuItem _fullScreenToggleKeysMenu;
    private ToolStripMenuItem _miToggleControlEnter;
    private ToolStripMenuItem _miToggleControlShiftEnter;
    private ToolStripMenuItem _miToggleF11;
    private ToolStripMenuItem _miToggleF12;
    private ToolStripMenuItem _miAlwaysOn;
    private ToolStripMenuItem _miAllowEscape;
    private ToolStripMenuItem _mousePointerHideDelayMenu;
    private ToolStripMenuItem _miMouseHideNever;
    private ToolStripMenuItem _miMouseHide1000;
    private ToolStripMenuItem _miMouseHide2000;
    private ToolStripMenuItem _miMouseHide5000;
    private ToolStripMenuItem _miMouseHide10000;
    private ToolStripMenuItem _miTopMostInFullScreen;
    private ToolStripSeparator _kioskChromeSeparator;
    private ToolStripMenuItem _miHideWindowsChrome;
    private ToolStripMenuItem _miReloadPlugins;

    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _statusInfo;
    private ToolStripStatusLabel _statusFps;
    private ToolStripStatusLabel _statusMode;

    private KioskModeManager _kioskModeManager;
    private System.Windows.Forms.Timer _fpsTimer;
    private ToolStripSeparator toolStripSeparator1;
}
