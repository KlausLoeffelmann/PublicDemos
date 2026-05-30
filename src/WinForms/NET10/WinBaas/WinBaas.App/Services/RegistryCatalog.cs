using Microsoft.Win32;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IRegistryCatalog"/>
public sealed class RegistryCatalog : IRegistryCatalog
{
    private readonly IReadOnlyList<RegistryDescriptor> _entries =
    [
        Dword("Show hidden OS files", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSuperHidden",
            "Explorer toggle for protected operating-system files.",
            "Explorer: show protected operating-system files. Frequently changed during diagnostics or advanced troubleshooting."),
        Dword("Show hidden files", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden",
            "Explorer toggle for hidden files and folders.",
            "Explorer: show hidden files and folders in File Explorer."),
        Dword("Show file extensions", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt",
            "Whether File Explorer hides known file-name extensions.",
            "Explorer: hide known file-name extensions. Power users often flip this off so extensions stay visible."),
        Dword("Explorer launch target", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo",
            "Whether File Explorer opens to Home/Quick Access or This PC.",
            "Explorer: launch target for new File Explorer windows (for example Home/Quick Access versus This PC)."),
        Dword("Item check boxes", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "AutoCheckSelect",
            "Show check boxes for item selection.",
            "Explorer: use item-selection check boxes in File Explorer."),
        Dword("Taskbar alignment", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAl",
            "Windows 11 taskbar alignment setting.",
            "Taskbar: alignment of Start and pinned apps (center versus left)."),
        Dword("Widgets button", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa",
            "Taskbar Widgets button visibility.",
            "Taskbar: show or hide the Widgets button."),
        Dword("Chat / Teams button", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn",
            "Taskbar Chat button visibility.",
            "Taskbar: show or hide the Chat / Microsoft Teams button."),
        Dword("Task View button", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowTaskViewButton",
            "Taskbar Task View button visibility.",
            "Taskbar: show or hide the Task View button."),
        Dword("Search box mode", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Search", "SearchboxTaskbarMode",
            "Taskbar search UI mode.",
            "Search: taskbar search-box mode (hidden, icon, label, or full search box)."),
        Dword("Bing web search", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled",
            "Include Bing/web results in Start or taskbar search.",
            "Search: allow Bing/web results in the Windows search experience."),
        Dword("Cortana consent", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Search", "CortanaConsent",
            "Legacy Cortana / cloud-search consent state.",
            "Search: legacy Cortana consent / cloud-backed search setting."),
        Dword("Telemetry level", RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry",
            "Windows telemetry policy value.",
            "Privacy / telemetry: policy-controlled telemetry level. HKLM policy value; read-only on non-elevated runs is expected.",
            requiresElevation: true),
        Dword("Tailored experiences", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled",
            "Use diagnostic data for tailored experiences.",
            "Privacy: tailored experiences based on diagnostic data."),
        Dword("Track Start suggestions", RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Start_TrackProgs",
            "Track recently used apps for Start recommendations.",
            "Privacy / UX: track frequently used apps so Start and Search can surface them."),
        String("Pause updates until", RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "PauseUpdatesExpiryTime",
            "Windows Update pause-until timestamp.",
            "Windows Update: pause-updates expiry timestamp. HKLM value; reading may require elevation.", requiresElevation: true),
        Dword("Branch readiness", RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "BranchReadinessLevel",
            "Windows Update servicing branch setting.",
            "Windows Update: branch readiness / servicing channel level. HKLM value; reading may require elevation.", requiresElevation: true),
        Dword("Developer mode", RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock", "AllowDevelopmentWithoutDevLicense",
            "Enable sideloading / developer mode without a dev license.",
            "Developer mode: allow development without a developer license. HKLM value; reading or restoring usually requires elevation.", requiresElevation: true),
        Dword("Allow trusted apps", RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock", "AllowAllTrustedApps",
            "Allow sideloading trusted apps.",
            "Developer mode: allow all trusted apps to be sideloaded. HKLM value; reading or restoring usually requires elevation.", requiresElevation: true),
        Dword("Long paths enabled", RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\FileSystem", "LongPathsEnabled",
            "Enable Win32 long-path support.",
            "Developer mode / filesystem: enable long-path support for Win32 APIs. HKLM value; reading or restoring usually requires elevation.", requiresElevation: true),
        String("Classic context menu", RegistryHive.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", string.Empty,
            "Restore the classic Windows 11 context menu.",
            "Windows 11 shell tweak: restore the classic context menu by setting the default value of the shell extension override key.",
            defaultValue: string.Empty),
        Dword("Hibernate enabled", RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled",
            "Hibernate availability.",
            "Power / UX: whether hibernation is enabled on the machine. HKLM value; reading or restoring usually requires elevation.", requiresElevation: true),
        Dword("Verbose boot/status", RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "VerboseStatus",
            "Show verbose startup and shutdown status messages.",
            "Power / UX: show verbose startup, logon, logoff, shutdown and restart status messages. HKLM value; reading or restoring usually requires elevation.", requiresElevation: true),
        Dword("DisableAntiSpyware", RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware",
            "Legacy Defender policy value.",
            "Defender: legacy policy-only value kept for information when present. Modern Windows versions may ignore it. HKLM policy value; reading may require elevation.",
            informationalOnly: true,
            requiresElevation: true),
    ];

    /// <inheritdoc />
    public IReadOnlyList<RegistryDescriptor> GetAll() => _entries;

    private static RegistryDescriptor Dword(
        string name,
        RegistryHive hive,
        string subKeyPath,
        string valueName,
        string shortDescription,
        string fullDescription,
        bool informationalOnly = false,
        bool requiresElevation = false)
        => new()
        {
            Name = name,
            Hive = hive,
            SubKeyPath = subKeyPath,
            ValueName = valueName,
            ValueKind = RegistryValueKind.DWord,
            ShortDescription = shortDescription,
            FullDescription = fullDescription,
            InformationalOnly = informationalOnly,
            RequiresElevation = requiresElevation,
        };

    private static RegistryDescriptor String(
        string name,
        RegistryHive hive,
        string subKeyPath,
        string valueName,
        string shortDescription,
        string fullDescription,
        object? defaultValue = null,
        bool informationalOnly = false,
        bool requiresElevation = false)
        => new()
        {
            Name = name,
            Hive = hive,
            SubKeyPath = subKeyPath,
            ValueName = valueName,
            ValueKind = RegistryValueKind.String,
            ShortDescription = shortDescription,
            FullDescription = fullDescription,
            DefaultValue = defaultValue,
            InformationalOnly = informationalOnly,
            RequiresElevation = requiresElevation,
        };
}
