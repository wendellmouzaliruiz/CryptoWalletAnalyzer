using System.Runtime.CompilerServices;
using System.Text.Json;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;
using CryptoWalletAnalyzer.Core.Security;
using CryptoWalletAnalyzer.Infrastructure.KnownWallets;

namespace CryptoWalletAnalyzer.Infrastructure.Scanners;

public record ChromiumBrowserTarget(BrowserType Type, string Name, string UserDataPath);

public class ChromiumExtensionScanner : IWalletScanner
{
    private readonly IReadOnlyList<ChromiumBrowserTarget> _targets;

    public string ScannerName => "Chromium Browser Extension Scanner";

    public ChromiumExtensionScanner(IReadOnlyList<ChromiumBrowserTarget>? customTargets = null)
    {
        _targets = customTargets ?? GetDefaultChromiumTargets();
    }

    public static List<ChromiumBrowserTarget> GetDefaultChromiumTargets()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        return new List<ChromiumBrowserTarget>
        {
            new(BrowserType.Chrome, "Google Chrome", Path.Combine(localAppData, @"Google\Chrome\User Data")),
            new(BrowserType.Edge, "Microsoft Edge", Path.Combine(localAppData, @"Microsoft\Edge\User Data")),
            new(BrowserType.Brave, "Brave Browser", Path.Combine(localAppData, @"BraveSoftware\Brave-Browser\User Data")),
            new(BrowserType.Opera, "Opera", Path.Combine(appData, @"Opera Software\Opera Stable")),
            new(BrowserType.OperaGX, "Opera GX", Path.Combine(appData, @"Opera Software\Opera GX Stable")),
            new(BrowserType.Vivaldi, "Vivaldi", Path.Combine(localAppData, @"Vivaldi\User Data"))
        };
    }

    public async IAsyncEnumerable<WalletItem> ScanAsync(
        IProgress<string>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var browser in _targets)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            if (!Directory.Exists(browser.UserDataPath))
                continue;

            progress?.Report($"Scanning {browser.Name} profiles...");

            var profileDirectories = GetProfileDirectories(browser.UserDataPath);

            foreach (var profileDir in profileDirectories)
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                var profileName = Path.GetFileName(profileDir);
                var extensionsRoot = Path.Combine(profileDir, "Extensions");

                if (!Directory.Exists(extensionsRoot))
                    continue;

                IEnumerable<string> extensionDirs;
                try
                {
                    extensionDirs = Directory.GetDirectories(extensionsRoot);
                }
                catch
                {
                    continue;
                }

                foreach (var extDir in extensionDirs)
                {
                    if (cancellationToken.IsCancellationRequested) yield break;

                    var extensionId = Path.GetFileName(extDir);

                    // Check if it's a known wallet extension or contains web3 metadata
                    var isKnown = ExtensionCatalog.TryGetKnownExtension(extensionId, out var knownExt);

                    var item = await InspectExtensionFolderAsync(
                        browser.Type,
                        profileName,
                        extDir,
                        extensionId,
                        knownExt,
                        isKnown,
                        cancellationToken);

                    if (item != null)
                    {
                        yield return item;
                    }
                }
            }
        }
    }

    private static List<string> GetProfileDirectories(string userDataPath)
    {
        var result = new List<string>();

        // Check root if extensions are directly under it (Opera style)
        if (Directory.Exists(Path.Combine(userDataPath, "Extensions")))
        {
            result.Add(userDataPath);
        }

        // Check "Default" and "Profile *"
        try
        {
            var subDirs = Directory.GetDirectories(userDataPath);
            foreach (var dir in subDirs)
            {
                var dirName = Path.GetFileName(dir);
                if (dirName.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
                    dirName.StartsWith("Profile ", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(dir);
                }
            }
        }
        catch
        {
            // Suppress IO permission errors
        }

        return result;
    }

    private static async Task<WalletItem?> InspectExtensionFolderAsync(
        BrowserType browserType,
        string profileName,
        string extDir,
        string extensionId,
        KnownExtension? knownExt,
        bool isKnown,
        CancellationToken cancellationToken)
    {
        try
        {
            // Extension directory has subdirectories named by version (e.g. 10.34.0_0)
            var versionDirs = Directory.GetDirectories(extDir);
            if (versionDirs.Length == 0) return null;

            // Pick latest version folder by creation time
            var latestVersionDir = versionDirs
                .OrderByDescending(d => Directory.GetCreationTime(d))
                .First();

            var manifestPath = Path.Combine(latestVersionDir, "manifest.json");
            if (!File.Exists(manifestPath)) return null;

            // Strict Safety check before reading!
            SafetyGuard.ValidateSafeToInspect(manifestPath);

            using var stream = File.OpenRead(manifestPath);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = doc.RootElement;

            var rawName = root.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
            var version = root.TryGetProperty("version", out var v) ? v.GetString() ?? "Unknown" : "Unknown";
            var description = root.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";

            var installDate = Directory.GetCreationTime(latestVersionDir);

            // If it's in our known catalog, we definitely want it
            if (isKnown && knownExt != null)
            {
                return new WalletItem
                {
                    Name = knownExt.CanonicalName,
                    Category = WalletCategory.BrowserExtension,
                    Version = version,
                    InstallDate = installDate,
                    Path = manifestPath,
                    Browser = browserType,
                    BrowserProfile = profileName,
                    ExtensionId = extensionId,
                    Notes = knownExt.Description
                };
            }

            // If not in catalog, check if manifest name/description contains crypto wallet indicators
            var combinedText = $"{rawName} {description}".ToLowerInvariant();
            if (IsWeb3WalletIndicator(combinedText))
            {
                var cleanName = rawName.StartsWith("__MSG_") ? "Generic Web3 Wallet" : rawName;
                return new WalletItem
                {
                    Name = cleanName,
                    Category = WalletCategory.BrowserExtension,
                    Version = version,
                    InstallDate = installDate,
                    Path = manifestPath,
                    Browser = browserType,
                    BrowserProfile = profileName,
                    ExtensionId = extensionId,
                    Notes = "Detected via Web3 wallet manifest signatures"
                };
            }
        }
        catch
        {
            // Ignore parse errors or safety rejections
        }

        return null;
    }

    private static bool IsWeb3WalletIndicator(string text)
    {
        return text.Contains("wallet") &&
               (text.Contains("crypto") || text.Contains("ethereum") || text.Contains("solana") ||
                text.Contains("bitcoin") || text.Contains("web3") || text.Contains("blockchain") ||
                text.Contains("polygon") || text.Contains("defi"));
    }
}
