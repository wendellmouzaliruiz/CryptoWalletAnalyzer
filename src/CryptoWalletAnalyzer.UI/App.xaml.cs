using System.Windows;
using CryptoWalletAnalyzer.Core.Interfaces;
using CryptoWalletAnalyzer.Infrastructure.Export;
using CryptoWalletAnalyzer.Infrastructure.Scanners;
using CryptoWalletAnalyzer.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWalletAnalyzer.UI;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Infrastructure Process & Scanners
        services.AddSingleton<IProcessDetector, ProcessDetector>();
        services.AddTransient<ChromiumExtensionScanner>();
        services.AddTransient<FirefoxExtensionScanner>();
        services.AddTransient<RegistryDesktopScanner>();
        services.AddTransient<FilesystemDesktopScanner>();

        // Composite Scanner with all vector implementations
        services.AddSingleton<CompositeWalletScanner>(sp =>
        {
            var detector = sp.GetRequiredService<IProcessDetector>();
            var scanners = new IWalletScanner[]
            {
                sp.GetRequiredService<ChromiumExtensionScanner>(),
                sp.GetRequiredService<FirefoxExtensionScanner>(),
                sp.GetRequiredService<RegistryDesktopScanner>(),
                sp.GetRequiredService<FilesystemDesktopScanner>()
            };
            return new CompositeWalletScanner(scanners, detector);
        });

        // Exporters
        services.AddSingleton<JsonReportExporter>();
        services.AddSingleton<CsvReportExporter>();

        // Presentation
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
