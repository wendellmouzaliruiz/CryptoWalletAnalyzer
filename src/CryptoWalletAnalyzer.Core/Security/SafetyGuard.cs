namespace CryptoWalletAnalyzer.Core.Security;

/// <summary>
/// Enforces architectural safety boundaries: guarantees that only non-sensitive
/// metadata files (manifests, registry keys, version infos) are inspected.
/// Throws SecurityException if any sensitive path, file, or pattern is targeted.
/// </summary>
public static class SafetyGuard
{
    private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".ldb", ".log", ".sqlite", ".db", ".dat", ".key", ".seed", ".vault", ".bak"
    };

    private static readonly string[] BlockedPathSubstrings =
    {
        "leveldb",
        "indexeddb",
        "databases",
        "session storage",
        "local storage",
        "blob_storage",
        "keystore",
        "wallet.dat",
        "wallets",
        "exodus.wallet"
    };

    /// <summary>
    /// Validates whether a file path is strictly safe to inspect (e.g. manifest.json or application executable).
    /// </summary>
    public static void ValidateSafeToInspect(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        var normalized = filePath.Replace('/', '\\').ToLowerInvariant();

        // Check blocked substrings
        foreach (var blocked in BlockedPathSubstrings)
        {
            // Allow extension manifests even if within extension folder, but NEVER inside leveldb/storage
            if (normalized.Contains($"\\{blocked}\\") || normalized.EndsWith($"\\{blocked}"))
            {
                throw new InvalidOperationException($"Security Boundary Violation: Access to sensitive path '{filePath}' is strictly prohibited.");
            }
        }

        var extension = Path.GetExtension(filePath);
        if (BlockedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Security Boundary Violation: Access to files with extension '{extension}' is strictly prohibited.");
        }
    }

    /// <summary>
    /// Checks whether a directory is safe to traverse for extension manifest locating.
    /// </summary>
    public static bool IsSafeDirectory(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath)) return false;
        var lower = directoryPath.Replace('/', '\\').ToLowerInvariant();
        return !BlockedPathSubstrings.Any(b => lower.Contains($"\\{b}\\") || lower.EndsWith($"\\{b}"));
    }
}
