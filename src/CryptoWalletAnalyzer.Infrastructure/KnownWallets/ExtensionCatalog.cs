namespace CryptoWalletAnalyzer.Infrastructure.KnownWallets;

public record KnownExtension(string Id, string CanonicalName, string Description);

public static class ExtensionCatalog
{
    private static readonly Dictionary<string, KnownExtension> Catalog = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nkbihfbeogaeaoehlefnkodbefgpgknn"] = new("nkbihfbeogaeaoehlefnkodbefgpgknn", "MetaMask", "EVM & Web3 Browser Wallet"),
        ["bfnaelmomeimhlpmgjnjophhpkkoljpa"] = new("bfnaelmomeimhlpmgjnjophhpkkoljpa", "Phantom", "Solana, Ethereum & Polygon Multi-Chain Wallet"),
        ["hnfanknocfeofbddgcijnmhnfnkdnaad"] = new("hnfanknocfeofbddgcijnmhnfnkdnaad", "Coinbase Wallet", "Multi-Chain Self-Custody Wallet"),
        ["acmacodkjbdgmoleebolmdjonilkdbch"] = new("acmacodkjbdgmoleebolmdjonilkdbch", "Rabby Wallet", "DeFi & EVM Open-Source Wallet by DeBank"),
        ["egjidjbpglichdcondbcbdnbeeppgdph"] = new("egjidjbpglichdcondbcbdnbeeppgdph", "Trust Wallet", "Multi-Chain Crypto Wallet"),
        ["mcohilncbfahbmgdjkbpemmiiiolobjg"] = new("mcohilncbfahbmgdjkbpemmiiiolobjg", "OKX Wallet", "Universal Web3 & Portal Wallet"),
        ["aholpfdialjgjfhomihkjbmgjidlcdno"] = new("aholpfdialjgjfhomihkjbmgjidlcdno", "Exodus Web3", "Multi-Chain Browser Extension"),
        ["fhbohimaelbohpjbbldcngcnapndodjp"] = new("fhbohimaelbohpjbbldcngcnapndodjp", "Binance Wallet", "BNB Chain & Web3 Extension"),
        ["dmkamcknogkgcdfhhbddcghachkejeap"] = new("dmkamcknogkgcdfhhbddcghachkejeap", "Keplr", "Cosmos Interchain Ecosystem Wallet"),
        ["opcgpfmipidbgpenhmajoajpbobppdil"] = new("opcgpfmipidbgpenhmajoajpbobppdil", "Sui Wallet", "Sui Blockchain Native Wallet"),
        ["phkbamefinggmakgklpkljjmgibohnba"] = new("phkbamefinggmakgklpkljjmgibohnba", "Pontem Aptos", "Aptos Ecosystem Web3 Wallet"),
        ["efbglgflmpkloogamnkepjpbbmngkeaf"] = new("efbglgflmpkloogamnkepjpbbmngkeaf", "Martian Aptos", "Aptos & Sui Ecosystem Wallet"),
        ["ejjladinnckdgjemekebdpeokbikhfci"] = new("ejjladinnckdgjemekebdpeokbikhfci", "Petra Aptos", "Official Aptos Labs Wallet"),
        ["aflkmfhebedbjioipglbdfljkjbhcnmi"] = new("aflkmfhebedbjioipglbdfljkjbhcnmi", "Backpack", "Solana & xNFT Crypto Wallet"),
        ["nnpmikbeeglhmecbhagfcjmfakpigmgb"] = new("nnpmikbeeglhmecbhagfcjmfakpigmgb", "Uniswap Extension", "Uniswap Labs EVM Wallet"),
        ["opfgelmcmbiajamepnmloochgidfdpmmg"] = new("opfgelmcmbiajamepnmloochgidfdpmmg", "Rainbow", "Ethereum & Layer 2 Fun & Colorful Wallet"),
        ["fbgcedjacmlbgleddnoacbnjhgdipnlk"] = new("fbgcedjacmlbgleddnoacbnjhgdipnlk", "Zerion", "Smart Web3 & DeFi Wallet"),
        ["jnlgamecbpmbajjfhmmmlhejkemejdma"] = new("jnlgamecbpmbajjfhmmmlhejkemejdma", "Braavos", "Starknet Layer 2 Smart Contract Wallet"),
        ["dlcobpjiigpikoobohmabehgpchpfhog"] = new("dlcobpjiigpikoobohmabehgpchpfhog", "Argent X", "Starknet First & Leading Wallet"),
        ["fnjhmkhhmkbjkkabndcnnogagogbneec"] = new("fnjhmkhhmkbjkkabndcnnogagogbneec", "Ronin Wallet", "Axie Infinity & Ronin Chain Wallet"),
        ["ibnejdfjmmkpcnlpebklmnkoeoihofec"] = new("ibnejdfjmmkpcnlpebklmnkoeoihofec", "TronLink", "TRON Blockchain Official Extension"),
        ["onhogfjeacnfoofkfgppknbmaenanggl"] = new("onhogfjeacnfoofkfgppknbmaenanggl", "SubWallet", "Polkadot, Kusama & Substrate Wallet"),
        ["mopnmbcafieddcagagflbncmnglgkgja"] = new("mopnmbcafieddcagagflbncmnglgkgja", "Polkadot{.js}", "Polkadot Native Signer & Keyring"),
        ["bhhhlbepdkbapadjdnnojkbgioiodbic"] = new("bhhhlbepdkbapadjdnnojkbgioiodbic", "Solflare", "Solana Non-Custodial Wallet"),
        ["afbcbjpbpfadlkmhmclhkgbfeggmflhn"] = new("afbcbjpbpfadlkmhmclhkgbfeggmflhn", "MathWallet", "Multi-Platform Universal Crypto Wallet"),
        ["jiidiaalihmmhddjgbnbgdfflelocpak"] = new("jiidiaalihmmhddjgbnbgdfflelocpak", "Bitget Wallet", "Formerly BitKeep Multi-Chain Wallet"),
        ["agoakfejjabomempkjlepdfladaobcfb"] = new("agoakfejjabomempkjlepdfladaobcfb", "Core (Avalanche)", "Avalanche & EVM Native Wallet"),
        ["ojggmchlghnjlapmfbnjholfjkiidbch"] = new("ojggmchlghnjlapmfbnjholfjkiidbch", "Venom Wallet", "Venom Blockchain Official Wallet"),
        ["lgmpcpgflpngkonflpodgodpkdfamnlk"] = new("lgmpcpgflpngkonflpodgodpkdfamnlk", "SafePal Wallet", "Hardware & Software Integrated Wallet"),
        ["fcfcfllfndlomdhbehjjcoimbgofdncg"] = new("fcfcfllfndlomdhbehjjcoimbgofdncg", "Leap Cosmos", "Super Wallet for Cosmos Chains")
    };

    public static bool TryGetKnownExtension(string extensionId, out KnownExtension? known)
    {
        return Catalog.TryGetValue(extensionId, out known);
    }

    public static IReadOnlyCollection<KnownExtension> All => Catalog.Values;
}
