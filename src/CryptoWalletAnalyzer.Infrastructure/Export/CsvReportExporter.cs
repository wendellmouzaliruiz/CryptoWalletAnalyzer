using System.Text;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;

namespace CryptoWalletAnalyzer.Infrastructure.Export;

public class CsvReportExporter : IReportExporter
{
    public string FormatName => "Comma-Separated Values (CSV)";
    public string FileExtension => ".csv";

    public async Task ExportAsync(ScanReport report, string destinationFilePath, CancellationToken cancellationToken = default)
    {
        var dir = Path.GetDirectoryName(destinationFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var sb = new StringBuilder();
        sb.AppendLine("Name,Category,Version,InstallDate,Browser,Profile,IsRunning,ExtensionId,Path,Notes");

        foreach (var item in report.Items)
        {
            var line = string.Join(",",
                Escape(item.Name),
                Escape(item.Category.ToString()),
                Escape(item.Version),
                Escape(item.InstallDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"),
                Escape(item.Browser?.ToString() ?? "N/A"),
                Escape(item.BrowserProfile ?? "N/A"),
                item.IsRunning ? "Active" : "Installed",
                Escape(item.ExtensionId ?? ""),
                Escape(item.Path),
                Escape(item.Notes ?? "")
            );
            sb.AppendLine(line);
        }

        await File.WriteAllTextAsync(destinationFilePath, sb.ToString(), Encoding.UTF8, cancellationToken);
    }

    private static string Escape(string text)
    {
        if (string.IsNullOrEmpty(text)) return "\"\"";
        return $"\"{text.Replace("\"", "\"\"")}\"";
    }
}
