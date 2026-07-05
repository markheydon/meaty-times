#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Preview or build the Meaty Times GitHub Pages documentation site inside Podman.

.DESCRIPTION
    Runs Jekyll in a Linux container so you do not need Ruby on the host. Gems are
    installed into a Podman volume (meaty-times-jekyll-bundle), keeping them separate
    from any native Windows vendor/bundle folder.

    Preview publishes the site at http://127.0.0.1:<Port> (default 4000). The first
    run can take a minute while gems install; wait for "Server running..." in the
    container output before opening the URL.

.PARAMETER BuildOnly
    Generate a static site in docs/_site instead of starting the preview server.

.PARAMETER Port
    Host port for the preview server. Mapped to port 4000 inside the container.

.EXAMPLE
    .\scripts\Start-JekyllDocsSite.ps1

    Start a live preview at http://127.0.0.1:4000

.EXAMPLE
    .\scripts\Start-JekyllDocsSite.ps1 -BuildOnly

    Build docs/_site without starting a server.

.NOTES
    Requires Podman. On Windows, ports are published to 127.0.0.1 explicitly so
    the Podman machine forwards them correctly to the host browser.
#>
[CmdletBinding(DefaultParameterSetName = 'Preview')]
param(
    [Parameter(ParameterSetName = 'Build')]
    [switch]$BuildOnly,

    [Parameter(ParameterSetName = 'Preview')]
    [ValidateRange(1024, 65535)]
    [int]$Port = 4000
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

#region Configuration

$Image = 'docker.io/ruby:3.3-bookworm'
$BundleVolumeName = 'meaty-times-jekyll-bundle'
$PagesRepo = 'markheydon/meaty-times'
$ContainerJekyllPort = 4000

#endregion

#region Private helpers

function Get-JekyllDocsRoot {
    $repoRoot = Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')
    $docsRoot = Join-Path $repoRoot 'docs'
    if (-not (Test-Path -LiteralPath $docsRoot -PathType Container)) {
        throw "Documentation folder not found: $docsRoot"
    }

    return (Resolve-Path -LiteralPath $docsRoot).Path
}

function Assert-PodmanAvailable {
    if (-not (Get-Command podman -ErrorAction SilentlyContinue)) {
        throw 'Podman is not on PATH. Install Podman Desktop from https://podman.io/'
    }
}

function Initialize-JekyllBundleVolume {
    param(
        [Parameter(Mandatory)]
        [string]$VolumeName
    )

    $null = podman volume inspect $VolumeName 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Verbose "Creating Podman volume '$VolumeName'."
        podman volume create $VolumeName | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to create Podman volume '$VolumeName'."
        }
    }
}

function Get-JekyllContainerScript {
    param(
        [Parameter(Mandatory)]
        [ValidateSet('Preview', 'Build')]
        [string]$Mode,

        [Parameter(Mandatory)]
        [int]$JekyllPort
    )

    $common = @'
set -euo pipefail
gem install bundler --no-document
bundle install
'@

    if ($Mode -eq 'Preview') {
        return @"
$common
exec bundle exec jekyll serve --host 0.0.0.0 --port $JekyllPort --baseurl ''
"@
    }

    return @"
$common
bundle exec jekyll build --baseurl ''
echo 'Site generated in ./_site'
"@
}

function Invoke-JekyllPodmanContainer {
    param(
        [Parameter(Mandatory)]
        [string]$DocsRoot,

        [Parameter(Mandatory)]
        [ValidateSet('Preview', 'Build')]
        [string]$Mode,

        [int]$Port = 4000
    )

    Initialize-JekyllBundleVolume -VolumeName $BundleVolumeName

    $containerScript = Get-JekyllContainerScript -Mode $Mode -JekyllPort $ContainerJekyllPort

    # Build argument list explicitly so PowerShell never splits the bash script on semicolons.
    $podmanArgs = @('run', '--rm')

    if ($Mode -eq 'Preview') {
        $podmanArgs += @('-it', '-p', "127.0.0.1:${Port}:${ContainerJekyllPort}")
    }

    $podmanArgs += @(
        '--name', 'meaty-times-jekyll-docs',
        '-v', "${DocsRoot}:/site:Z",
        '-v', "${BundleVolumeName}:/bundle:Z",
        '-e', 'BUNDLE_PATH=/bundle',
        '-e', "PAGES_REPO_NWO=$PagesRepo",
        '-w', '/site',
        $Image,
        'bash',
        '-lc',
        $containerScript
    )

    if ($Mode -eq 'Preview') {
        Write-Output 'Starting Jekyll preview...'
        Write-Output "  URL:  http://127.0.0.1:$Port"
        Write-Output "  Docs: $DocsRoot"
        Write-Output ''
        Write-Output "Wait for 'Server running... press ctrl-c to stop.' before opening the URL."
        Write-Output 'Press Ctrl+C here to stop the container.'
        Write-Output ''
    }
    else {
        Write-Output "Building static site in $DocsRoot\_site ..."
    }

    & podman @podmanArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Podman exited with code $LASTEXITCODE."
    }
}

#endregion

#region Public commands

function Start-JekyllDocsPreview {
    <#
    .SYNOPSIS
        Start a live Jekyll preview server for the docs site in Podman.
    #>
    [CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'Low')]
    param(
        [ValidateRange(1024, 65535)]
        [int]$Port = 4000
    )

    if (-not $PSCmdlet.ShouldProcess('Meaty Times documentation site', 'Start Jekyll preview server in Podman')) {
        return
    }

    Assert-PodmanAvailable
    $docsRoot = Get-JekyllDocsRoot
    Invoke-JekyllPodmanContainer -DocsRoot $docsRoot -Mode Preview -Port $Port
}

function Invoke-JekyllDocsBuild {
    <#
    .SYNOPSIS
        Build the docs site into docs/_site using Podman.
    #>
    [CmdletBinding()]
    param()

    Assert-PodmanAvailable
    $docsRoot = Get-JekyllDocsRoot
    Invoke-JekyllPodmanContainer -DocsRoot $docsRoot -Mode Build
}

#endregion

# Script entry point (skip when dot-sourced to load functions only).
if ($MyInvocation.InvocationName -ne '.') {
    if ($BuildOnly) {
        Invoke-JekyllDocsBuild
    }
    else {
        Start-JekyllDocsPreview -Port $Port
    }
}
