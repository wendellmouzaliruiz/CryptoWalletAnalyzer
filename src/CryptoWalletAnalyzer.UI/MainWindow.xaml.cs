using System.Windows;
using CryptoWalletAnalyzer.UI.ViewModels;

namespace CryptoWalletAnalyzer.UI;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
