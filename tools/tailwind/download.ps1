#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Target = Join-Path $ScriptDir 'tailwindcss.exe'
$Version = if ($env:TAILWIND_CLI_VERSION) { $env:TAILWIND_CLI_VERSION } else { 'v4.3.3' }
$Asset = 'tailwindcss-windows-x64.exe'
$ExpectedSha256 = 'e0e260ce048014e9268f6237ff18f8ccf02cef521cbd0ae04e82c2cdf7aa3955'

function Test-Checksum {
    param([string]$Path)
    $actual = (Get-FileHash -Path $Path -Algorithm SHA256).Hash.ToLowerInvariant()
    return $actual -eq $ExpectedSha256
}

if ((Test-Path $Target) -and (Test-Checksum $Target)) {
    exit 0
}

if (Test-Path $Target) {
    Write-Host 'Existing Tailwind CLI failed checksum verification; re-downloading...'
    Remove-Item $Target -Force
}

$Url = "https://github.com/tailwindlabs/tailwindcss/releases/download/$Version/$Asset"
Write-Host "Downloading Tailwind CLI $Version ($Asset)..."
Invoke-WebRequest -Uri $Url -OutFile $Target -UseBasicParsing

if (-not (Test-Checksum $Target)) {
    Write-Error 'Downloaded Tailwind CLI failed checksum verification.'
    Remove-Item $Target -Force
    exit 1
}
