using CryptoWalletAnalyzer.Core.Models;

namespace CryptoWalletAnalyzer.Core.Interfaces;

public interface IBrowserExtensionScanner : IWalletScanner
{
    BrowserType TargetBrowser { get; }
}
