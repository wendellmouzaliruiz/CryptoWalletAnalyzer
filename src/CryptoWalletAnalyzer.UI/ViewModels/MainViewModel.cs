using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Core.Models;
using CryptoWalletAnalyzer.Infrastructure.Export;
using CryptoWalletAnalyzer.Infrastructure.Scanners;
using Microsoft.Win32;

namespace CryptoWalletAnalyzer.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly CompositeWalletScanner _compositeScanner;
    private readonly JsonReportExporter _jsonExporter;
    private readonly CsvReportExporter _csvExporter;
    private CancellationTokenSource? _scanCts;

    private readonly List<WalletItemViewModel> _allDiscoveredItems = new();

    [ObservableProperty]
    private ObservableCollection<WalletItemViewModel> _filteredItems = new();

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _statusMessage = "Ready to scan. Click 'Start Deep Scan' to inventory local crypto wallets.";

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _selectedCategoryFilter = "All";

    [ObservableProperty]
    private int _totalWalletsCount;

    [ObservableProperty]
    private int _activeProcessesCount;

    [ObservableProperty]
    private int _browserExtensionsCount;

    [ObservableProperty]
    private int _desktopAppsCount;

    [ObservableProperty]
    private string _lastScanDuration = "0.0s";

    [ObservableProperty]
    private string _safetyAuditMessage = "100% Safe: Zero Private Keys, Seed Phrases, or Datastores Accessed";

    public MainViewModel(
        CompositeWalletScanner compositeScanner,
        JsonReportExporter jsonExporter,
        CsvReportExporter csvExporter)
    {
        _compositeScanner = compositeScanner;
        _jsonExporter = jsonExporter;
        _csvExporter = csvExporter;
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilter();
    partial void OnSelectedCategoryFilterChanged(string value) => ApplyFilter();

    [RelayCommand]
    public async Task StartScanAsync()
    {
        if (IsScanning) return;

        IsScanning = true;
        _allDiscoveredItems.Clear();
        FilteredItems.Clear();
        UpdateMetrics();

        _scanCts = new CancellationTokenSource();
        var progress = new Progress<string>(msg => StatusMessage = msg);

        try
        {
            StatusMessage = "Initializing security guards and scanners...";
            var sw = System.Diagnostics.Stopwatch.StartNew();

            await foreach (var item in _compositeScanner.ScanAsync(progress, _scanCts.Token))
            {
                var vm = new WalletItemViewModel(item);
                _allDiscoveredItems.Add(vm);
                
                // Add to view if matches current filter
                if (MatchesFilter(vm))
                {
                    FilteredItems.Add(vm);
                }

                UpdateMetrics();
            }

            sw.Stop();
            LastScanDuration = $"{sw.Elapsed.TotalSeconds:F2}s";
            StatusMessage = $"Scan completed in {LastScanDuration}. Found {_allDiscoveredItems.Count} crypto wallet installation(s).";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Scan was cancelled by user.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Scan error: {ex.Message}";
        }
        finally
        {
            IsScanning = false;
            _scanCts?.Dispose();
            _scanCts = null;
        }
    }

    [RelayCommand]
    public void CancelScan()
    {
        if (IsScanning && _scanCts != null)
        {
            StatusMessage = "Cancelling scan...";
            _scanCts.Cancel();
        }
    }

    [RelayCommand]
    public async Task ExportJsonAsync()
    {
        if (_allDiscoveredItems.Count == 0)
        {
            StatusMessage = "No items to export. Run a scan first.";
            return;
        }

        var saveDialog = new SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            FileName = $"CryptoWalletInventory_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json",
            Title = "Export Inventory Report to JSON"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                var report = BuildScanReport();
                await _jsonExporter.ExportAsync(report, saveDialog.FileName);
                StatusMessage = $"Report exported successfully to {Path.GetFileName(saveDialog.FileName)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export failed: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    public async Task ExportCsvAsync()
    {
        if (_allDiscoveredItems.Count == 0)
        {
            StatusMessage = "No items to export. Run a scan first.";
            return;
        }

        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv",
            FileName = $"CryptoWalletInventory_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv",
            Title = "Export Inventory Report to CSV"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                var report = BuildScanReport();
                await _csvExporter.ExportAsync(report, saveDialog.FileName);
                StatusMessage = $"Report exported successfully to {Path.GetFileName(saveDialog.FileName)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export failed: {ex.Message}";
            }
        }
    }

    private void ApplyFilter()
    {
        FilteredItems.Clear();
        foreach (var item in _allDiscoveredItems)
        {
            if (MatchesFilter(item))
            {
                FilteredItems.Add(item);
            }
        }
    }

    private bool MatchesFilter(WalletItemViewModel item)
    {
        // Category filter
        if (SelectedCategoryFilter == "Browser Extensions" && item.Category != WalletCategory.BrowserExtension)
            return false;
        if (SelectedCategoryFilter == "Desktop Apps" && item.Category != WalletCategory.DesktopApplication)
            return false;

        // Search query
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var q = SearchQuery.Trim().ToLowerInvariant();
            var matches = item.Name.ToLowerInvariant().Contains(q) ||
                          item.LocationDisplay.ToLowerInvariant().Contains(q) ||
                          item.Version.ToLowerInvariant().Contains(q) ||
                          item.Path.ToLowerInvariant().Contains(q) ||
                          item.Notes.ToLowerInvariant().Contains(q);
            if (!matches) return false;
        }

        return true;
    }

    private void UpdateMetrics()
    {
        TotalWalletsCount = _allDiscoveredItems.Count;
        ActiveProcessesCount = _allDiscoveredItems.Count(i => i.IsRunning);
        BrowserExtensionsCount = _allDiscoveredItems.Count(i => i.Category == WalletCategory.BrowserExtension);
        DesktopAppsCount = _allDiscoveredItems.Count(i => i.Category == WalletCategory.DesktopApplication);
    }

    private ScanReport BuildScanReport()
    {
        return new ScanReport
        {
            ScannedAt = DateTime.UtcNow,
            Items = _allDiscoveredItems.Select(vm => vm.Model).ToList()
        };
    }
}
