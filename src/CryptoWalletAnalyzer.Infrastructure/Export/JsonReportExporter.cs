using System.Text.Json;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;

namespace CryptoWalletAnalyzer.Infrastructure.Export;

public class JsonReportExporter : IReportExporter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string FormatName => "JSON Report";
    public string FileExtension => ".json";

    public async Task ExportAsync(ScanReport report, string destinationFilePath, CancellationToken cancellationToken = default)
    {
        var dir = Path.GetDirectoryName(destinationFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using var stream = File.Create(destinationFilePath);
        await JsonSerializer.SerializeAsync(stream, report, Options, cancellationToken);
    }
}
