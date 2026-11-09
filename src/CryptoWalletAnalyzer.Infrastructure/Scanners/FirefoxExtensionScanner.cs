using System.Runtime.CompilerServices;
using System.Text.Json;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;
using CryptoWalletAnalyzer.Core.Security;
using CryptoWalletAnalyzer.Infrastructure.KnownWallets;

namespace CryptoWalletAnalyzer.Infrastructure.Scanners;

public class FirefoxExtensionScanner : IWalletScanner
{
    private readonly string? _customProfilesRoot;

    public string ScannerName => "Mozilla Firefox Extension Scanner";

    public FirefoxExtensionScanner(string? customProfilesRoot = null)
    {
        _customProfilesRoot = customProfilesRoot;
    }

    public async IAsyncEnumerable<WalletItem> ScanAsync(
        IProgress<string>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var profilesRoot = _customProfilesRoot ?? Path.Combine(appData, @"Mozilla\Firefox\Profiles");

        if (!Directory.Exists(profilesRoot))
            yield break;

        progress?.Report("Scanning Mozilla Firefox profiles...");

        IEnumerable<string> profileDirs;
        try
        {
            profileDirs = Directory.GetDirectories(profilesRoot);
        }
        catch
        {
            yield break;
        }

        foreach (var profileDir in profileDirs)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            var profileName = Path.GetFileName(profileDir);
            var extensionsJsonPath = Path.Combine(profileDir, "extensions.json");

            if (!File.Exists(extensionsJsonPath))
                continue;

            SafetyGuard.ValidateSafeToInspect(extensionsJsonPath);

            using var stream = File.OpenRead(extensionsJsonPath);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (!doc.RootElement.TryGetProperty("addons", out var addons) || addons.ValueKind != JsonValueKind.Array)
                continue;

            foreach (var addon in addons.EnumerateArray())
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                var id = addon.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";
                var version = addon.TryGetProperty("version", out var vProp) ? vProp.GetString() ?? "Unknown" : "Unknown";
                
                var name = "";
                if (addon.TryGetProperty("defaultLocale", out var locale) && locale.TryGetProperty("name", out var nProp))
                {
                    name = nProp.GetString() ?? "";
                }

                if (string.IsNullOrWhiteSpace(name)) continue;

                var isKnown = ExtensionCatalog.TryGetKnownExtension(id, out var known);
                var isCryptoName = name.Contains("wallet", StringComparison.OrdinalIgnoreCase) ||
                                   name.Contains("metamask", StringComparison.OrdinalIgnoreCase) ||
                                   name.Contains("phantom", StringComparison.OrdinalIgnoreCase);

                if (isKnown || isCryptoName)
                {
                    yield return new WalletItem
                    {
                        Name = known?.CanonicalName ?? name,
                        Category = WalletCategory.BrowserExtension,
                        Version = version,
                        InstallDate = File.GetCreationTime(extensionsJsonPath),
                        Path = extensionsJsonPath,
                        Browser = BrowserType.Firefox,
                        BrowserProfile = profileName,
                        ExtensionId = id,
                        Notes = known?.Description ?? "Detected via Firefox add-on registry"
                    };
                }
            }
        }
    }
}
