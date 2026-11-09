using CryptoWalletAnalyzer.Core.Models;

namespace CryptoWalletAnalyzer.Core.Interfaces;

public interface IWalletScanner
{
    string ScannerName { get; }
    IAsyncEnumerable<WalletItem> ScanAsync(IProgress<string>? progress = null, CancellationToken cancellationToken = default);
}
