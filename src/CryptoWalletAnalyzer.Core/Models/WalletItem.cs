namespace CryptoWalletAnalyzer.Core.Models;

/// <summary>
/// Represents safe, non-sensitive inventory metadata for a detected crypto wallet.
/// Strictly excludes seed phrases, private keys, vaults, and account balances.
/// </summary>
public record WalletItem
{
    public required string Name { get; init; }
    public required WalletCategory Category { get; init; }
    public string Version { get; init; } = "Unknown";
    public DateTime? InstallDate { get; init; }
    public required string Path { get; init; }
    public BrowserType? Browser { get; init; }
    public string? BrowserProfile { get; init; }
    public string? ExtensionId { get; init; }
    public bool IsRunning { get; init; }
    public string? ProcessName { get; init; }
    public string? Notes { get; init; }
}
