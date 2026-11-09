namespace CryptoWalletAnalyzer.Infrastructure.KnownWallets;

public record KnownDesktopApp(
    string Name,
    string ExecutableName,
    string ProcessName,
    string Description,
    string[]? RegistrySubstrings = null,
    string[]? KnownRelativePaths = null
);

public static class DesktopAppCatalog
{
    private static readonly List<KnownDesktopApp> Catalog = new()
    {
        new(
            "Exodus",
            "Exodus.exe",
            "Exodus",
            "Multi-asset self-custodial desktop wallet",
            new[] { "Exodus" },
            new[] { @"Exodus\Exodus.exe", @"Programs\Exodus\Exodus.exe" }
        ),
        new(
            "Ledger Live",
            "Ledger Live.exe",
            "Ledger Live",
            "Official companion app for Ledger hardware wallets",
            new[] { "Ledger Live" },
            new[] { @"Ledger Live\Ledger Live.exe", @"Programs\Ledger Live\Ledger Live.exe" }
        ),
        new(
            "Trezor Suite",
            "Trezor Suite.exe",
            "Trezor Suite",
            "Official desktop management suite for Trezor hardware wallets",
            new[] { "Trezor Suite" },
            new[] { @"Trezor Suite\Trezor Suite.exe", @"Programs\Trezor Suite\Trezor Suite.exe" }
        ),
        new(
            "Electrum",
            "electrum.exe",
            "electrum",
            "Lightweight, secure Bitcoin desktop client",
            new[] { "Electrum" },
            new[] { @"Electrum\electrum.exe", @"Programs\Electrum\electrum.exe" }
        ),
        new(
            "Atomic Wallet",
            "Atomic Wallet.exe",
            "Atomic Wallet",
            "Decentralized multi-currency crypto wallet",
            new[] { "Atomic Wallet" },
            new[] { @"atomic\Atomic Wallet.exe", @"Programs\atomic\Atomic Wallet.exe" }
        ),
        new(
            "Wasabi Wallet",
            "wassabee.exe",
            "wassabee",
            "Privacy-focused open-source Bitcoin wallet with CoinJoin",
            new[] { "Wasabi" },
            new[] { @"WasabiWallet\wassabee.exe" }
        ),
        new(
            "SafePal Desktop",
            "SafePal.exe",
            "SafePal",
            "SafePal hardware & software wallet desktop app",
            new[] { "SafePal" },
            new[] { @"SafePal\SafePal.exe", @"Programs\SafePal\SafePal.exe" }
        ),
        new(
            "Trust Wallet Desktop",
            "Trust Wallet.exe",
            "Trust Wallet",
            "Multi-chain decentralized desktop wallet",
            new[] { "Trust Wallet" },
            new[] { @"Trust Wallet\Trust Wallet.exe" }
        ),
        new(
            "Bitcoin Core",
            "bitcoin-qt.exe",
            "bitcoin-qt",
            "Official full-node Bitcoin implementation and GUI client",
            new[] { "Bitcoin Core" },
            new[] { @"Bitcoin\bitcoin-qt.exe" }
        ),
        new(
            "Monero GUI",
            "monero-wallet-gui.exe",
            "monero-wallet-gui",
            "Official open-source GUI client for private Monero cryptocurrency",
            new[] { "Monero GUI" },
            new[] { @"Monero GUI Wallet\monero-wallet-gui.exe" }
        )
    };

    public static IReadOnlyList<KnownDesktopApp> All => Catalog;
}
