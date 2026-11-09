using CryptoWalletAnalyzer.Core.Security;
using Xunit;

namespace CryptoWalletAnalyzer.Tests;

public class SafetyGuardTests
{
    [Theory]
    [InlineData(@"C:\Users\User\AppData\Local\Google\Chrome\User Data\Default\Local Storage\leveldb\000005.ldb")]
    [InlineData(@"C:\Users\User\AppData\Local\Google\Chrome\User Data\Default\IndexedDB\meta.sqlite")]
    [InlineData(@"C:\Users\User\AppData\Roaming\Exodus\exodus.wallet\wallet.dat")]
    [InlineData(@"C:\Users\User\AppData\Roaming\Electrum\wallets\default_wallet")]
    [InlineData(@"C:\Users\User\Documents\seed_phrase.txt.key")]
    [InlineData(@"C:\Users\User\AppData\Local\BraveSoftware\Brave-Browser\User Data\Default\databases\Databases.db")]
    public void ValidateSafeToInspect_ShouldThrow_WhenSensitivePathTargeted(string sensitivePath)
    {
        Assert.Throws<InvalidOperationException>(() => SafetyGuard.ValidateSafeToInspect(sensitivePath));
    }

    [Theory]
    [InlineData(@"C:\Users\User\AppData\Local\Google\Chrome\User Data\Default\Extensions\nkbihfbeogaeaoehlefnkodbefgpgknn\10.34.0_0\manifest.json")]
    [InlineData(@"C:\Users\User\AppData\Local\Microsoft\Edge\User Data\Default\Extensions\bfnaelmomeimhlpmgjnjophhpkkoljpa\24.1.0_0\manifest.json")]
    [InlineData(@"C:\Program Files\Exodus\Exodus.exe")]
    [InlineData(@"C:\Users\User\AppData\Local\Programs\ledger-live-desktop\Ledger Live.exe")]
    public void ValidateSafeToInspect_ShouldPass_ForSafeManifestsAndExecutables(string safePath)
    {
        var exception = Record.Exception(() => SafetyGuard.ValidateSafeToInspect(safePath));
        Assert.Null(exception);
    }

    [Fact]
    public void IsSafeDirectory_ShouldReturnFalse_ForIndexedDbOrLevelDbDirectories()
    {
        var unsafeDir = @"C:\AppData\Chrome\Default\IndexedDB\";
        Assert.False(SafetyGuard.IsSafeDirectory(unsafeDir));

        var safeDir = @"C:\AppData\Chrome\Default\Extensions\nkbihfbeogaeaoehlefnkodbefgpgknn\";
        Assert.True(SafetyGuard.IsSafeDirectory(safeDir));
    }
}
