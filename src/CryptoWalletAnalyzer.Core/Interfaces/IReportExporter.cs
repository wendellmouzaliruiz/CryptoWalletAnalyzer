using CryptoWalletAnalyzer.Core.Models;

namespace CryptoWalletAnalyzer.Core.Interfaces;

public interface IReportExporter
{
    string FormatName { get; }
    string FileExtension { get; }
    Task ExportAsync(ScanReport report, string destinationFilePath, CancellationToken cancellationToken = default);
}
