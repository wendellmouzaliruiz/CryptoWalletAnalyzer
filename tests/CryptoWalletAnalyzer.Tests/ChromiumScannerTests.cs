using System.Text.Json;
using CryptoWalletAnalyzer.Core.Models;
using CryptoWalletAnalyzer.Infrastructure.Scanners;
using Xunit;

namespace CryptoWalletAnalyzer.Tests;

public class ChromiumScannerTests : IDisposable
{
    private readonly string _tempTestDir;

    public ChromiumScannerTests()
    {
        _tempTestDir = Path.Combine(Path.GetTempPath(), "CWA_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempTestDir);
    }

    [Fact]
    public async Task ScanAsync_ShouldDiscoverMockMetamaskExtension()
    {
        // Arrange: Create fake Chrome User Data structure
        var profileDir = Path.Combine(_tempTestDir, "Default");
        var metamaskId = "nkbihfbeogaeaoehlefnkodbefgpgknn";
        var versionDir = Path.Combine(profileDir, "Extensions", metamaskId, "12.0.0_0");
        Directory.CreateDirectory(versionDir);

        var manifestContent = new
        {
            name = "MetaMask",
            version = "12.0.0",
            description = "Ethereum & Web3 Wallet"
        };
        var manifestPath = Path.Combine(versionDir, "manifest.json");
        await File.WriteAllTextAsync(manifestPath, JsonSerializer.Serialize(manifestContent));

        var target = new ChromiumBrowserTarget(BrowserType.Chrome, "Test Chrome", _tempTestDir);
        var scanner = new ChromiumExtensionScanner(new[] { target });

        // Act
        var results = new List<WalletItem>();
        await foreach (var item in scanner.ScanAsync())
        {
            results.Add(item);
        }

        // Assert
        Assert.Single(results);
        var itemFound = results[0];
        Assert.Equal("MetaMask", itemFound.Name);
        Assert.Equal("12.0.0", itemFound.Version);
        Assert.Equal(BrowserType.Chrome, itemFound.Browser);
        Assert.Equal("Default", itemFound.BrowserProfile);
        Assert.Equal(WalletCategory.BrowserExtension, itemFound.Category);
        Assert.Equal(metamaskId, itemFound.ExtensionId);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempTestDir))
            {
                Directory.Delete(_tempTestDir, true);
            }
        }
        catch { }
    }
}
