using System.Diagnostics;
using System.Runtime.CompilerServices;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;
using CryptoWalletAnalyzer.Infrastructure.KnownWallets;

namespace CryptoWalletAnalyzer.Infrastructure.Scanners;

public class FilesystemDesktopScanner : IDesktopWalletScanner
{
    private readonly IProcessDetector _processDetector;

    public string ScannerName => "Filesystem Desktop Application Scanner";

    public FilesystemDesktopScanner(IProcessDetector processDetector)
    {
        _processDetector = processDetector;
    }

    public async IAsyncEnumerable<WalletItem> ScanAsync(
        IProgress<string>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        progress?.Report("Checking common installation folders for desktop wallets...");

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        var searchRoots = new[]
        {
            Path.Combine(localAppData, "Programs"),
            localAppData,
            appData,
            programFiles,
            programFilesX86
        }.Where(Directory.Exists).Distinct().ToList();

        var detected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var app in DesktopAppCatalog.All)
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            if (app.KnownRelativePaths == null) continue;

            foreach (var root in searchRoots)
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                foreach (var relPath in app.KnownRelativePaths)
                {
                    var fullExePath = Path.Combine(root, relPath);
                    if (File.Exists(fullExePath) && detected.Add(app.Name))
                    {
                        var version = "Unknown";
                        try
                        {
                            var versionInfo = FileVersionInfo.GetVersionInfo(fullExePath);
                            version = versionInfo.FileVersion ?? versionInfo.ProductVersion ?? "Unknown";
                        }
                        catch { }

                        var installDate = File.GetCreationTime(fullExePath);
                        var isRunning = _processDetector.IsProcessRunning(app.ProcessName);

                        yield return new WalletItem
                        {
                            Name = app.Name,
                            Category = WalletCategory.DesktopApplication,
                            Version = version,
                            InstallDate = installDate,
                            Path = fullExePath,
                            IsRunning = isRunning,
                            ProcessName = app.ProcessName,
                            Notes = app.Description
                        };
                    }
                }
            }
        }

        await Task.CompletedTask;
    }
}
