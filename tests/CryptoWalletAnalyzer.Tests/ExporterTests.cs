using System.Text.Json;
using CryptoWalletAnalyzer.Core.Models;
using CryptoWalletAnalyzer.Infrastructure.Export;
using Xunit;

namespace CryptoWalletAnalyzer.Tests;

public class ExporterTests : IDisposable
{
    private readonly string _tempDir;

    public ExporterTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "CWA_ExportTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public async Task JsonReportExporter_ShouldProduceValidJson()
    {
        var exporter = new JsonReportExporter();
        var sampleReport = new ScanReport
        {
            Items = new[]
            {
                new WalletItem
                {
                    Name = "MetaMask",
                    Category = WalletCategory.BrowserExtension,
                    Version = "12.4.0",
                    Path = @"C:\Mock\manifest.json",
                    Browser = BrowserType.Chrome,
                    BrowserProfile = "Default",
                    ExtensionId = "nkbihfbeogaeaoehlefnkodbefgpgknn"
                }
            }
        };

        var filePath = Path.Combine(_tempDir, "report.json");
        await exporter.ExportAsync(sampleReport, filePath);

        Assert.True(File.Exists(filePath));
        var content = await File.ReadAllTextAsync(filePath);
        using var doc = JsonDocument.Parse(content);
        Assert.True(doc.RootElement.TryGetProperty("items", out _));
        Assert.Equal(1, doc.RootElement.GetProperty("totalWallets").GetInt32());
    }

    [Fact]
    public async Task CsvReportExporter_ShouldProduceValidCsvHeaderAndRows()
    {
        var exporter = new CsvReportExporter();
        var sampleReport = new ScanReport
        {
            Items = new[]
            {
                new WalletItem
                {
                    Name = "Exodus",
                    Category = WalletCategory.DesktopApplication,
                    Version = "24.10.1",
                    Path = @"C:\Program Files\Exodus\Exodus.exe",
                    IsRunning = true
                }
            }
        };

        var filePath = Path.Combine(_tempDir, "report.csv");
        await exporter.ExportAsync(sampleReport, filePath);

        Assert.True(File.Exists(filePath));
        var lines = await File.ReadAllLinesAsync(filePath);
        Assert.True(lines.Length >= 2);
        Assert.StartsWith("Name,Category,Version", lines[0]);
        Assert.Contains("Exodus", lines[1]);
        Assert.Contains("Active", lines[1]);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }
        catch { }
    }
}
