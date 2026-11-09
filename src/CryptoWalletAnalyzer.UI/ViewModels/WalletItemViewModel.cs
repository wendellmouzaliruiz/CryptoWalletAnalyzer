using CommunityToolkit.Mvvm.ComponentModel;
using CryptoWalletAnalyzer.Core.Models;

namespace CryptoWalletAnalyzer.UI.ViewModels;

public partial class WalletItemViewModel : ObservableObject
{
    public WalletItem Model { get; }

    public WalletItemViewModel(WalletItem model)
    {
        Model = model;
    }

    public string Name => Model.Name;
    public WalletCategory Category => Model.Category;
    public string CategoryDisplay => Model.Category == WalletCategory.BrowserExtension ? "Browser Extension" : "Desktop Application";
    public string Version => Model.Version;
    public DateTime? InstallDate => Model.InstallDate;
    public string InstallDateDisplay => Model.InstallDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
    public string Path => Model.Path;
    public BrowserType? Browser => Model.Browser;
    public string? BrowserProfile => Model.BrowserProfile;
    public string? ExtensionId => Model.ExtensionId;
    public bool IsRunning => Model.IsRunning;
    public string StatusDisplay => IsRunning ? "RUNNING" : "INSTALLED";
    public string Notes => Model.Notes ?? string.Empty;

    public string LocationDisplay
    {
        get
        {
            if (Model.Category == WalletCategory.BrowserExtension)
            {
                var browserName = Model.Browser?.ToString() ?? "Browser";
                var profile = !string.IsNullOrEmpty(Model.BrowserProfile) ? $" ({Model.BrowserProfile})" : "";
                return $"{browserName}{profile}";
            }
            return "Windows Desktop";
        }
    }
}
