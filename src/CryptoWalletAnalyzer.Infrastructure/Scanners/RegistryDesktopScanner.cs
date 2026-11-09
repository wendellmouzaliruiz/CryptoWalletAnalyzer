using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;
using CryptoWalletAnalyzer.Infrastructure.KnownWallets;
using Microsoft.Win32;

namespace CryptoWalletAnalyzer.Infrastructure.Scanners;

[SupportedOSPlatform("windows")]
public class RegistryDesktopScanner : IDesktopWalletScanner
{
    private readonly IProcessDetector _processDetector;

    public string ScannerName => "Windows Registry Desktop Application Scanner";

    public RegistryDesktopScanner(IProcessDetector processDetector)
    {
        _processDetector = processDetector;
    }

    public async IAsyncEnumerable<WalletItem> ScanAsync(
        IProgress<string>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        progress?.Report("Scanning Windows Registry for installed desktop wallets...");

        var registryRoots = new[]
        {
            (Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Uninstall"),
            (Registry.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Uninstall"),
            (Registry.LocalMachine, @"Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
        };

        var foundItems = new Dictionary<string, WalletItem>(StringComparer.OrdinalIgnoreCase);

        foreach (var (rootKey, subKeyPath) in registryRoots)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            using var baseKey = rootKey.OpenSubKey(subKeyPath);
            if (baseKey == null) continue;

            string[] subKeyNames;
            try
            {
                subKeyNames = baseKey.GetSubKeyNames();
            }
            catch
            {
                continue;
            }

            foreach (var name in subKeyNames)
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                using var appKey = baseKey.OpenSubKey(name);
                if (appKey == null) continue;

                var displayName = appKey.GetValue("DisplayName") as string;
                if (string.IsNullOrWhiteSpace(displayName)) continue;

                var displayVersion = appKey.GetValue("DisplayVersion") as string ?? "Unknown";
                var installLocation = appKey.GetValue("InstallLocation") as string ?? "";
                var displayIcon = appKey.GetValue("DisplayIcon") as string ?? "";
                var rawInstallDate = appKey.GetValue("InstallDate") as string;

                var installDate = ParseRegistryInstallDate(rawInstallDate);

                // Match against known catalog
                var matched = DesktopAppCatalog.All.FirstOrDefault(k =>
                    k.RegistrySubstrings != null &&
                    k.RegistrySubstrings.Any(sub => displayName.Contains(sub, StringComparison.OrdinalIgnoreCase)));

                if (matched != null)
                {
                    var isRunning = _processDetector.IsProcessRunning(matched.ProcessName);
                    var effectivePath = !string.IsNullOrEmpty(installLocation) ? installLocation : displayIcon;

                    if (!foundItems.ContainsKey(matched.Name))
                    {
                        var item = new WalletItem
                        {
                            Name = matched.Name,
                            Category = WalletCategory.DesktopApplication,
                            Version = displayVersion,
                            InstallDate = installDate,
                            Path = effectivePath,
                            IsRunning = isRunning,
                            ProcessName = matched.ProcessName,
                            Notes = matched.Description
                        };
                        foundItems[matched.Name] = item;
                        yield return item;
                    }
                }
                else if (IsGenericCryptoWallet(displayName))
                {
                    if (!foundItems.ContainsKey(displayName))
                    {
                        var item = new WalletItem
                        {
                            Name = displayName,
                            Category = WalletCategory.DesktopApplication,
                            Version = displayVersion,
                            InstallDate = installDate,
                            Path = !string.IsNullOrEmpty(installLocation) ? installLocation : displayIcon,
                            IsRunning = false,
                            Notes = "Detected via Registry software signatures"
                        };
                        foundItems[displayName] = item;
                        yield return item;
                    }
                }
            }
        }

        await Task.CompletedTask;
    }

    private static bool IsGenericCryptoWallet(string name)
    {
        var lower = name.ToLowerInvariant();
        return lower.Contains("wallet") &&
               (lower.Contains("crypto") || lower.Contains("bitcoin") || lower.Contains("ethereum") || lower.Contains("monero"));
    }

    private static DateTime? ParseRegistryInstallDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        // Registry dates are often YYYYMMDD
        if (raw.Length == 8 &&
            int.TryParse(raw.Substring(0, 4), out var year) &&
            int.TryParse(raw.Substring(4, 2), out var month) &&
            int.TryParse(raw.Substring(6, 2), out var day))
        {
            try { return new DateTime(year, month, day); } catch { return null; }
        }

        if (DateTime.TryParse(raw, out var dt))
            return dt;

        return null;
    }
}
