namespace CryptoWalletAnalyzer.Core.Interfaces;

public interface IProcessDetector
{
    bool IsProcessRunning(string processName);
    IReadOnlyList<string> GetRunningProcessNames();
}
