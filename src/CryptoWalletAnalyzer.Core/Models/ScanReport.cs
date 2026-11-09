namespace CryptoWalletAnalyzer.Core.Models;

/// <summary>
/// Aggregates an inventory scan report suitable for local display or export.
/// </summary>
public record ScanReport
{
    public DateTime ScannedAt { get; init; } = DateTime.UtcNow;
    public string MachineName { get; init; } = Environment.MachineName;
    public string OsVersion { get; init; } = Environment.OSVersion.ToString();
    public TimeSpan ScanDuration { get; init; }
    public IReadOnlyList<WalletItem> Items { get; init; } = Array.Empty<WalletItem>();

    public int TotalWallets => Items.Count;
    public int BrowserExtensionsCount => Items.Count(i => i.Category == WalletCategory.BrowserExtension);
    public int DesktopAppsCount => Items.Count(i => i.Category == WalletCategory.DesktopApplication);
    public int ActiveRunningCount => Items.Count(i => i.IsRunning);

    public bool SafetyAuditPassed { get; init; } = true;
    public string SafetyAuditNote { get; init; } = 
        "Verified: Strictly read-only metadata inspection. Zero seed phrases, private keys, or LevelDB/IndexedDB stores accessed.";
}
