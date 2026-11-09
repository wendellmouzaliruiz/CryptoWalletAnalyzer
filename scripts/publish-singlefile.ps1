<#
.SYNOPSIS
    Builds and publishes CryptoWalletAnalyzer as a single-file self-contained win-x64 executable.
#>
[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "$PSScriptRoot\..\publish\win-x64"
)

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  CryptoWalletAnalyzer - Single-File Publish Script" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$projectPath = "$PSScriptRoot\..\src\CryptoWalletAnalyzer.UI\CryptoWalletAnalyzer.UI.csproj"

Write-Host "Building and publishing single-file binary..." -ForegroundColor Yellow
dotnet publish $projectPath `
    -c $Configuration `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $OutputDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[SUCCESS] Single-file executable ready at: $OutputDir\CryptoWalletAnalyzer.UI.exe" -ForegroundColor Green
} else {
    Write-Host "`n[ERROR] Publish failed with exit code $LASTEXITCODE" -ForegroundColor Red
}
