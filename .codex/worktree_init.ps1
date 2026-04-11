Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$projectRoot = Join-Path $repoRoot 'elixir'

if (-not (Get-Command mise -ErrorAction SilentlyContinue)) {
    [Console]::Error.WriteLine('mise is required. Install it from https://mise.jdx.dev/getting-started.html')
    exit 1
}

Set-Location $projectRoot
mise trust

make setup