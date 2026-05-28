#nullable enable

using Microsoft.Extensions.DependencyInjection;
using WarpToolkit.ComponentModel;
using WarpToolkit.WinForms.Tooling;
using WingetPackageEditor.Core.ViewModels;

namespace Winget_Package_Editor;

public partial class MainForm : Form, IServiceProvider
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null!;

    private IUserSettingsService? _userSettingsService;
    private IServiceProvider? _serviceProvider;
    private MainViewModel? _viewModel;
    private ObservableBindingList<AppEntryViewModel>? _appsBindingList;
    private TreeViewBinder? _treeViewBinder;
    private GridSelectionBinder? _gridSelectionBinder;

    /// <summary>
    ///  Initializes a new instance of the <see cref="MainForm"/> class with dependency injection support.
    /// </summary>
    /// <param name="serviceProvider">
    ///  The service provider that contains all registered services for dependency injection.
    ///  This parameter is used to resolve dependencies and configure the form with the required services.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///  Thrown when <paramref name="serviceProvider"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    ///  Thrown when the required <see cref="IUserSettingsService"/> is not registered in the service provider.
    /// </exception>
    /// <remarks>
    ///  This constructor overload is specifically designed to be used when the Form is instantiated 
    ///  through Dependency Injection (DI) using the <c>WinFormsApplication</c> class and the 
    ///  <c>WinFormsApplicationBuilder</c>. This approach provides the same infrastructure pattern 
    ///  as ASP.NET Core applications, enabling familiar service registration, configuration, 
    ///  and dependency injection patterns in WinForms applications.
    ///  <para>
    ///   When using this constructor, the Form acts as a ServiceProvider-aware component, 
    ///   allowing it to resolve and utilize services that have been registered in the 
    ///   application's service container. This enables loose coupling, testability, 
    ///   and modern application architecture patterns in WinForms development.
    ///  </para>
    ///  <para>
    ///   The constructor automatically assigns the service provider to the form using the 
    ///   <c>AssignServiceProvider</c> extension method and resolves the required 
    ///   <see cref="IUserSettingsService"/> from the container.
    ///  </para>
    /// </remarks>
    public MainForm(IServiceProvider serviceProvider) : this()
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        _serviceProvider = new DeferredServiceProvider(serviceProvider);

        _userSettingsService = serviceProvider.GetRequiredService<IUserSettingsService>();
        _viewModel = serviceProvider.GetRequiredService<MainViewModel>();

        if (_userSettingsService is null)
        {
            throw new NullReferenceException($"The service '{nameof(IUserSettingsService)}' is not registered.");
        }
    }

    object IServiceProvider.GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType, nameof(serviceType));

        if (_serviceProvider is null)
        {
            throw new InvalidOperationException("Service provider is not initialized.");
        }

        return _serviceProvider.GetService(serviceType)
            ?? throw new InvalidOperationException($"Service of type '{serviceType.Name}' is not registered.");
    }

#pragma warning disable WFOWARP9901
    private sealed class DeferredServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DeferredServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
#pragma warning restore WFOWARP9901

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            if (_viewModel is not null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.ConsoleMessages.CollectionChanged -= ConsoleMessages_CollectionChanged;
            }

            _appsBindingList?.Dispose();
            _treeViewBinder?.Dispose();
            _gridSelectionBinder?.Dispose();
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _mainMenuStrip = new MenuStrip();
        _fileMenuItem = new ToolStripMenuItem();
        _newMenuItem = new ToolStripMenuItem();
        _openMenuItem = new ToolStripMenuItem();
        _saveMenuItem = new ToolStripMenuItem();
        _saveAsMenuItem = new ToolStripMenuItem();
        _exportMenuItem = new ToolStripMenuItem();
        _quitMenuItem = new ToolStripMenuItem();
        _editMenuItem = new ToolStripMenuItem();
        _addAppMenuItem = new ToolStripMenuItem();
        _removeAppMenuItem = new ToolStripMenuItem();
        _propertiesMenuItem = new ToolStripMenuItem();
        _actionMenuItem = new ToolStripMenuItem();
        _applyNowMenuItem = new ToolStripMenuItem();
        _generateBundleFolderMenuItem = new ToolStripMenuItem();
        _toolsMenuItem = new ToolStripMenuItem();
        _optionsMenuItem = new ToolStripMenuItem();
        _helpMenuItem = new ToolStripMenuItem();
        _mainToolStrip = new ToolStrip();
        _newToolStripButton = new ToolStripButton();
        _openToolStripButton = new ToolStripButton();
        _saveToolStripButton = new ToolStripButton();
        _addAppToolStripButton = new ToolStripButton();
        _removeAppToolStripButton = new ToolStripButton();
        _exportToolStripButton = new ToolStripButton();
        _applyNowToolStripButton = new ToolStripButton();
        _mainSplitContainer = new SplitContainer();
        _packageTreeView = new TreeView();
        _rightSplitContainer = new SplitContainer();
        _gridHostPanel = new Panel();
        _gridButtonPanel = new FlowLayoutPanel();
        _addAppButton = new Button();
        _removeAppButton = new Button();
        _propertiesButton = new Button();
        _appsWarpDataGridView = new DataGridView();
        _consoleControl = new ConsoleControl();
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();
        _fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _newMenuItem, _openMenuItem, _saveMenuItem, _saveAsMenuItem, _exportMenuItem, _quitMenuItem });
        _editMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _addAppMenuItem, _removeAppMenuItem, _propertiesMenuItem });
        _actionMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _applyNowMenuItem, _generateBundleFolderMenuItem });
        _toolsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _optionsMenuItem });
        _mainMenuStrip.Items.AddRange(new ToolStripItem[] { _fileMenuItem, _editMenuItem, _actionMenuItem, _toolsMenuItem, _helpMenuItem });
        _mainToolStrip.Items.AddRange(new ToolStripItem[] { _newToolStripButton, _openToolStripButton, _saveToolStripButton, _addAppToolStripButton, _removeAppToolStripButton, _exportToolStripButton, _applyNowToolStripButton });
        _mainSplitContainer.Panel1.Controls.Add(_packageTreeView);
        _mainSplitContainer.Panel2.Controls.Add(_rightSplitContainer);
        _rightSplitContainer.Panel1.Controls.Add(_gridHostPanel);
        _rightSplitContainer.Panel2.Controls.Add(_consoleControl);
        _gridHostPanel.Controls.Add(_appsWarpDataGridView);
        _gridHostPanel.Controls.Add(_gridButtonPanel);
        _gridButtonPanel.Controls.Add(_addAppButton);
        _gridButtonPanel.Controls.Add(_removeAppButton);
        _gridButtonPanel.Controls.Add(_propertiesButton);
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusLabel });
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).BeginInit();
        _mainSplitContainer.Panel1.SuspendLayout();
        _mainSplitContainer.Panel2.SuspendLayout();
        _mainSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_rightSplitContainer).BeginInit();
        _rightSplitContainer.Panel1.SuspendLayout();
        _rightSplitContainer.Panel2.SuspendLayout();
        _rightSplitContainer.SuspendLayout();
        _gridHostPanel.SuspendLayout();
        _gridButtonPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_appsWarpDataGridView).BeginInit();
        SuspendLayout();
        // 
        // _mainMenuStrip
        // 
        _mainMenuStrip.Location = new Point(0, 0);
        _mainMenuStrip.Name = "_mainMenuStrip";
        _mainMenuStrip.Size = new Size(1182, 28);
        _mainMenuStrip.TabIndex = 0;
        // 
        // menu items
        // 
        _fileMenuItem.Text = "&File";
        _newMenuItem.Text = "&New";
        _openMenuItem.Text = "&Open";
        _saveMenuItem.Text = "&Save";
        _saveAsMenuItem.Text = "Save &As";
        _exportMenuItem.Text = "&Export YAML+Script";
        _quitMenuItem.Text = "&Quit";
        _editMenuItem.Text = "&Edit";
        _addAppMenuItem.Text = "&Add App";
        _removeAppMenuItem.Text = "&Remove App";
        _propertiesMenuItem.Text = "&Properties";
        _actionMenuItem.Text = "&Action";
        _applyNowMenuItem.Text = "&Apply Now";
        _generateBundleFolderMenuItem.Text = "&Generate Bundle Folder";
        _toolsMenuItem.Text = "&Tools";
        _optionsMenuItem.Text = "&Options";
        _helpMenuItem.Text = "&Help";
        // 
        // _mainToolStrip
        // 
        _mainToolStrip.Location = new Point(0, 28);
        _mainToolStrip.Name = "_mainToolStrip";
        _mainToolStrip.Size = new Size(1182, 27);
        _mainToolStrip.TabIndex = 1;
        _newToolStripButton.Text = "New";
        _openToolStripButton.Text = "Open";
        _saveToolStripButton.Text = "Save";
        _addAppToolStripButton.Text = "Add App";
        _removeAppToolStripButton.Text = "Remove App";
        _exportToolStripButton.Text = "Export";
        _applyNowToolStripButton.Text = "Apply";
        // 
        // _mainSplitContainer
        // 
        _mainSplitContainer.Dock = DockStyle.Fill;
        _mainSplitContainer.Location = new Point(0, 55);
        _mainSplitContainer.Name = "_mainSplitContainer";
        _mainSplitContainer.Size = new Size(1182, 678);
        _mainSplitContainer.SplitterDistance = 330;
        _mainSplitContainer.TabIndex = 2;
        // 
        // _packageTreeView
        // 
        _packageTreeView.Dock = DockStyle.Fill;
        _packageTreeView.HideSelection = false;
        _packageTreeView.Name = "_packageTreeView";
        _packageTreeView.TabIndex = 0;
        // 
        // _rightSplitContainer
        // 
        _rightSplitContainer.Dock = DockStyle.Fill;
        _rightSplitContainer.Location = new Point(0, 0);
        _rightSplitContainer.Name = "_rightSplitContainer";
        _rightSplitContainer.Orientation = Orientation.Horizontal;
        _rightSplitContainer.Size = new Size(848, 678);
        _rightSplitContainer.SplitterDistance = 410;
        _rightSplitContainer.TabIndex = 0;
        // 
        // _gridHostPanel
        // 
        _gridHostPanel.Controls.Add(_appsWarpDataGridView);
        _gridHostPanel.Controls.Add(_gridButtonPanel);
        _gridHostPanel.Dock = DockStyle.Fill;
        _gridHostPanel.Name = "_gridHostPanel";
        _gridHostPanel.TabIndex = 0;
        // 
        // _gridButtonPanel
        // 
        _gridButtonPanel.AutoSize = true;
        _gridButtonPanel.Dock = DockStyle.Top;
        _gridButtonPanel.Name = "_gridButtonPanel";
        _gridButtonPanel.Padding = new Padding(6);
        _gridButtonPanel.TabIndex = 0;
        _addAppButton.AutoSize = true;
        _addAppButton.Text = "Add App";
        _removeAppButton.AutoSize = true;
        _removeAppButton.Text = "Remove App";
        _propertiesButton.AutoSize = true;
        _propertiesButton.Text = "Properties";
        // 
        // _appsWarpDataGridView
        // 
        _appsWarpDataGridView.AllowUserToAddRows = false;
        _appsWarpDataGridView.AllowUserToDeleteRows = false;
        _appsWarpDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _appsWarpDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _appsWarpDataGridView.Dock = DockStyle.Fill;
        _appsWarpDataGridView.MultiSelect = false;
        _appsWarpDataGridView.Name = "_appsWarpDataGridView";
        _appsWarpDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _appsWarpDataGridView.TabIndex = 1;
        // 
        // _consoleControl
        // 
        _consoleControl.Dock = DockStyle.Fill;
        _consoleControl.Name = "_consoleControl";
        _consoleControl.ReadOnly = true;
        _consoleControl.TabIndex = 0;
        // 
        // _statusStrip
        // 
        _statusStrip.Location = new Point(0, 733);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Size = new Size(1182, 26);
        _statusStrip.TabIndex = 3;
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Text = "Ready";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1182, 759);
        Controls.Add(_mainSplitContainer);
        Controls.Add(_statusStrip);
        Controls.Add(_mainToolStrip);
        Controls.Add(_mainMenuStrip);
        MainMenuStrip = _mainMenuStrip;
        Name = "MainForm";
        Text = "WinGet Package Editor";
        _mainSplitContainer.Panel1.ResumeLayout(false);
        _mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).EndInit();
        _mainSplitContainer.ResumeLayout(false);
        _rightSplitContainer.Panel1.ResumeLayout(false);
        _rightSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_rightSplitContainer).EndInit();
        _rightSplitContainer.ResumeLayout(false);
        _gridHostPanel.ResumeLayout(false);
        _gridHostPanel.PerformLayout();
        _gridButtonPanel.ResumeLayout(false);
        _gridButtonPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_appsWarpDataGridView).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip _mainMenuStrip = null!;
    private ToolStripMenuItem _fileMenuItem = null!;
    private ToolStripMenuItem _newMenuItem = null!;
    private ToolStripMenuItem _openMenuItem = null!;
    private ToolStripMenuItem _saveMenuItem = null!;
    private ToolStripMenuItem _saveAsMenuItem = null!;
    private ToolStripMenuItem _exportMenuItem = null!;
    private ToolStripMenuItem _quitMenuItem = null!;
    private ToolStripMenuItem _editMenuItem = null!;
    private ToolStripMenuItem _addAppMenuItem = null!;
    private ToolStripMenuItem _removeAppMenuItem = null!;
    private ToolStripMenuItem _propertiesMenuItem = null!;
    private ToolStripMenuItem _actionMenuItem = null!;
    private ToolStripMenuItem _applyNowMenuItem = null!;
    private ToolStripMenuItem _generateBundleFolderMenuItem = null!;
    private ToolStripMenuItem _toolsMenuItem = null!;
    private ToolStripMenuItem _optionsMenuItem = null!;
    private ToolStripMenuItem _helpMenuItem = null!;
    private ToolStrip _mainToolStrip = null!;
    private ToolStripButton _newToolStripButton = null!;
    private ToolStripButton _openToolStripButton = null!;
    private ToolStripButton _saveToolStripButton = null!;
    private ToolStripButton _addAppToolStripButton = null!;
    private ToolStripButton _removeAppToolStripButton = null!;
    private ToolStripButton _exportToolStripButton = null!;
    private ToolStripButton _applyNowToolStripButton = null!;
    private SplitContainer _mainSplitContainer = null!;
    private TreeView _packageTreeView = null!;
    private SplitContainer _rightSplitContainer = null!;
    private Panel _gridHostPanel = null!;
    private FlowLayoutPanel _gridButtonPanel = null!;
    private Button _addAppButton = null!;
    private Button _removeAppButton = null!;
    private Button _propertiesButton = null!;
    private DataGridView _appsWarpDataGridView = null!;
    private ConsoleControl _consoleControl = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _statusLabel = null!;
}
