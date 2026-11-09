using CryptoWalletAnalyzer.Infrastructure.KnownWallets;
using Xunit;

namespace CryptoWalletAnalyzer.Tests;

public class CatalogTests
{
    [Fact]
    public void ExtensionCatalog_ShouldContainMajorWallets()
    {
        Assert.True(ExtensionCatalog.TryGetKnownExtension("nkbihfbeogaeaoehlefnkodbefgpgknn", out var metamask));
        Assert.NotNull(metamask);
        Assert.Equal("MetaMask", metamask.CanonicalName);

        Assert.True(ExtensionCatalog.TryGetKnownExtension("bfnaelmomeimhlpmgjnjophhpkkoljpa", out var phantom));
        Assert.NotNull(phantom);
        Assert.Equal("Phantom", phantom.CanonicalName);

        Assert.True(ExtensionCatalog.TryGetKnownExtension("hnfanknocfeofbddgcijnmhnfnkdnaad", out var coinbase));
        Assert.NotNull(coinbase);
        Assert.Equal("Coinbase Wallet", coinbase.CanonicalName);

        Assert.True(ExtensionCatalog.All.Count >= 25);
    }

    [Fact]
    public void DesktopAppCatalog_ShouldContainMajorDesktopWallets()
    {
        var names = DesktopAppCatalog.All.Select(a => a.Name).ToList();

        Assert.Contains("Exodus", names);
        Assert.Contains("Ledger Live", names);
        Assert.Contains("Trezor Suite", names);
        Assert.Contains("Electrum", names);
        Assert.Contains("Atomic Wallet", names);
        Assert.Contains("Bitcoin Core", names);
    }
}
