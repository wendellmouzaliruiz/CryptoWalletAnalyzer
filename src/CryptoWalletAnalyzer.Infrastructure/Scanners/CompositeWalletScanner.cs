using System.Diagnostics;
using System.Runtime.CompilerServices;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;

namespace CryptoWalletAnalyzer.Infrastructure.Scanners;

public class CompositeWalletScanner : IWalletScanner
{
    private readonly IEnumerable<IWalletScanner> _scanners;
    private readonly IProcessDetector _processDetector;

    public string ScannerName => "Composite Multi-Vector Wallet Scanner";

    public CompositeWalletScanner(IEnumerable<IWalletScanner> scanners, IProcessDetector processDetector)
    {
        _scanners = scanners;
        _processDetector = processDetector;
    }

    public async IAsyncEnumerable<WalletItem> ScanAsync(
        IProgress<string>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var scanner in _scanners)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            await foreach (var item in scanner.ScanAsync(progress, cancellationToken))
            {
                var dedupeKey = $"{item.Category}_{item.Name}_{item.Browser}_{item.BrowserProfile}_{item.ExtensionId}";
                if (seenKeys.Add(dedupeKey))
                {
                    // Ensure active process state is fresh
                    var isRunning = item.IsRunning;
                    if (!isRunning && !string.IsNullOrEmpty(item.ProcessName))
                    {
                        isRunning = _processDetector.IsProcessRunning(item.ProcessName);
                    }

                    yield return item with { IsRunning = isRunning };
                }
            }
        }
    }

    public async Task<ScanReport> GenerateReportAsync(
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var items = new List<WalletItem>();

        await foreach (var item in ScanAsync(progress, cancellationToken))
        {
            items.Add(item);
        }

        sw.Stop();

        return new ScanReport
        {
            ScannedAt = DateTime.UtcNow,
            ScanDuration = sw.Elapsed,
            Items = items
        };
    }
}
