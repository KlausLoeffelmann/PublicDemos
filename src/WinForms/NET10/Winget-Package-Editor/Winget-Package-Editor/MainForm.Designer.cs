using Microsoft.Extensions.DependencyInjection;
using WarpToolkit.ComponentModel;

namespace Winget_Package_Editor;

public partial class MainForm : Form, IServiceProvider
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private readonly IUserSettingsService _userSettingsService = null!;
    private readonly IServiceProvider _serviceProvider = null!;

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
    public MainForm(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        _serviceProvider = serviceProvider;

        _userSettingsService = serviceProvider.GetRequiredService<IUserSettingsService>();

        if (_userSettingsService is null)
        {
            throw new NullReferenceException($"The service '{nameof(IUserSettingsService)}' is not registered.");
        }

        InitializeComponent();
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

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
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
        SuspendLayout();
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1006, 721);
        Name = "MainForm";
        Text = "MainForm";
        ResumeLayout(false);
    }

    #endregion
}
