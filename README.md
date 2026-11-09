# Crypto Wallet Analyzer — Local PC Inventory & Safe Auditing Tool

[![.NET 8 LTS](https://img.shields.io/badge/.NET-8.0%20LTS-blue.svg?logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 12](https://img.shields.io/badge/C%23-12-purple.svg?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![WPF](https://img.shields.io/badge/UI-WPF%20Desktop-informational.svg)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Security Guarantee](https://img.shields.io/badge/Security-Zero%20Private%20Data%20Access-emerald.svg)](#-strict-security--safety-guarantees)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20MVVM-success.svg)](#-clean-architecture--design)
[![Unit Tests](https://img.shields.io/badge/Tests-16%20Passed%20(100%25)-brightgreen.svg)](#-unit-testing--verification)
[![License](https://img.shields.io/badge/License-MIT-gray.svg)](LICENSE)

A high-performance, lightweight Windows desktop application engineered in **modern .NET 8 (LTS)** and **C# WPF** designed to perform a non-invasive, safe inventory of crypto wallet software installed on a local machine.

---

## 🛡️ Strict Security & Safety Guarantees

This application is strictly an **inventory and auditing tool**. It is architected with programmatic safety boundaries (*`SafetyGuard`*) ensuring **zero compromise** of user secrets:

* 🚫 **Never accesses private keys, seed phrases, recovery phrases, or account passwords.**
* 🚫 **Never reads wallet storage datastores** (`LevelDB`, `IndexedDB`, `databases/`, `local storage`, `wallet.dat`, `vault.json`, `.sqlite`, `.ldb`, `.key`).
* 🚫 **Never launches or opens wallet software.**
* 🚫 **Never injects into, debugs, or reads process memory** (`PROCESS_VM_READ` is never invoked).
* ✅ **Read-only metadata inspection:** Inspects only public extension manifests (`manifest.json`, `extensions.json`) and Windows Registry software registration keys (`Uninstall` display names, versions, install dates).
* ✅ **Passive process detection:** Checks only active process names via `Process.GetProcessesByName()` to indicate if an application is currently running.

---

## ✨ Key Features

* **Universal Browser Extension Discovery:**
  * Auto-discovers and iterates user profiles across **Google Chrome**, **Microsoft Edge**, **Brave Browser**, **Opera**, **Opera GX**, **Vivaldi**, and **Mozilla Firefox**.
  * Pre-cataloged with **30+ top Web3 wallets** (MetaMask, Phantom, Coinbase Wallet, Rabby, Trust Wallet, OKX, Exodus Web3, Binance Wallet, Keplr, Sui, Uniswap, Rainbow, Zerion, Pontem, Petra, Argent X, Ronin, TronLink, SubWallet, Solflare, etc.).
  * Dynamic heuristic detection for newly emerged Web3 wallet extensions.
* **Desktop Application Detection:**
  * Inspects Windows Registry uninstall hives (`HKLM` and `HKCU`, 64-bit and 32-bit views).
  * Inspects standard `%LOCALAPPDATA%`, `%APPDATA%`, and `%PROGRAMFILES%` directories for standalone/portable installations.
  * Native recognition for **Exodus**, **Ledger Live**, **Trezor Suite**, **Electrum**, **Atomic Wallet**, **Wasabi Wallet**, **SafePal**, **Trust Wallet Desktop**, **Bitcoin Core**, and **Monero GUI**.
* **Modern Dark-Themed WPF Interface:**
  * Clean, reactive dashboard built with `CommunityToolkit.Mvvm` and Microsoft Dependency Injection.
  * Real-time metrics: Total detected wallets, active processes, extension vs desktop breakdown.
  * Live search and category filters (All, Extensions Only, Desktop Only).
* **Corporate Audit Export:**
  * One-click export of complete inventory reports to structured **JSON** and standard **CSV**.
* **Single-File Executable Ready:**
  * Can be packaged as a single self-contained `.exe` (`win-x64`) with zero runtime dependencies.

---

## 🏗️ Clean Architecture & Design

The solution strictly adheres to **Clean Architecture** and **SOLID** principles:

```
CryptoWalletAnalyzer/
├── src/
│   ├── CryptoWalletAnalyzer.Core/           # Domain Models, Contracts & Security
│   │   ├── Models/                           # WalletItem, ScanReport, Enums
│   │   ├── Interfaces/                       # IWalletScanner, IBrowserExtensionScanner, IProcessDetector, IReportExporter
│   │   └── Security/                         # SafetyGuard (enforces strict path & extension whitelists)
│   │
│   ├── CryptoWalletAnalyzer.Infrastructure/ # OS & Filesystem Concrete Implementations
│   │   ├── KnownWallets/                     # ExtensionCatalog (30+ IDs), DesktopAppCatalog
│   │   ├── Scanners/                         # ChromiumExtensionScanner, FirefoxExtensionScanner,
│   │   │                                     # RegistryDesktopScanner, FilesystemDesktopScanner,
│   │   │                                     # CompositeWalletScanner, ProcessDetector
│   │   └── Export/                           # JsonReportExporter, CsvReportExporter
│   │
│   └── CryptoWalletAnalyzer.UI/             # Presentation Layer (WPF / .NET 8)
│       ├── ViewModels/                       # MainViewModel (async commands, reactive filters), WalletItemViewModel
│       ├── Views/                            # MainWindow.xaml (Modern dark aesthetic, DataGrid, Stat Cards)
│       └── App.xaml                          # Microsoft.Extensions.DependencyInjection IoC wiring
│
├── tests/
│   └── CryptoWalletAnalyzer.Tests/          # Unit & Integration Test Suite (xUnit, Moq)
│       ├── SafetyGuardTests.cs               # Asserts security exceptions on forbidden files
│       ├── CatalogTests.cs                   # Verifies signatures and wallet catalogs
│       ├── ChromiumScannerTests.cs           # Mock filesystem extension manifest discovery
│       └── ExporterTests.cs                  # JSON & CSV generation verification
│
├── scripts/
│   └── publish-singlefile.ps1               # Automated win-x64 single-file build script
└── CryptoWalletAnalyzer.sln
```

---

## 🧪 Unit Testing & Verification

All discovery parsers, safety guards, and exporters are backed by automated tests:

```powershell
# Run the entire test suite
dotnet test CryptoWalletAnalyzer.sln -c Release
```

### Test Coverage Highlights:
- **`SafetyGuardTests`**: Injects paths targeting `.ldb`, `.sqlite`, `.dat`, `wallet.dat`, `keystore`, and `vault` to assert that `SafetyGuard.ValidateSafeToInspect()` strictly blocks unauthorized access.
- **`ChromiumScannerTests`**: Spins up isolated temporary directory structures with mocked extension manifests to verify asynchronous discovery and property mapping.
- **`ExporterTests`**: Validates schema compliance of exported JSON and CSV datasets.

---

## 📦 Building & Publishing

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Windows 10/11 (x64)

### Development Build
```powershell
# Restore and build the solution
dotnet build CryptoWalletAnalyzer.sln -c Release
```

### Self-Contained Single-File Executable
To generate a standalone, single-file `.exe` that runs on any Windows machine without requiring .NET pre-installed:

```powershell
# Using the included PowerShell script:
.\scripts\publish-singlefile.ps1

# Or via dotnet CLI:
dotnet publish src/CryptoWalletAnalyzer.UI/CryptoWalletAnalyzer.UI.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -o publish/win-x64
```

The resulting executable will be located at:
`publish\win-x64\CryptoWalletAnalyzer.UI.exe`

---

## 👨‍💻 Author & Engineering Background

**Wendell Mouzali Ruiz**  
*Senior Software Architect & .NET Specialist (15+ years experience)*  
* LinkedIn: [linkedin.com/in/wendell-mouzali-ruiz-60913955](https://www.linkedin.com/in/wendell-mouzali-ruiz-60913955/)  
* GitHub: [github.com/wendellmouzaliruiz](https://github.com/wendellmouzaliruiz)  

*Built with passion, performance, and uncompromised security.*

---

## 📄 License
This project is licensed under the [MIT License](LICENSE).
