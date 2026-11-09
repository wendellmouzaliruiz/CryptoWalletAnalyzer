using System.Diagnostics;
using CryptoWalletAnalyzer.Core.Interfaces;

namespace CryptoWalletAnalyzer.Infrastructure.Scanners;

public class ProcessDetector : IProcessDetector
{
    public bool IsProcessRunning(string processName)
    {
        if (string.IsNullOrWhiteSpace(processName)) return false;
        try
        {
            var processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    public IReadOnlyList<string> GetRunningProcessNames()
    {
        try
        {
            return Process.GetProcesses()
                .Select(p =>
                {
                    try { return p.ProcessName; }
                    catch { return string.Empty; }
                })
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
}
