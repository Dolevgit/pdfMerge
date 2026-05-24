[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repositoryRoot = (Resolve-Path (Split-Path -Parent $PSScriptRoot)).Path
$projectPath = Join-Path $repositoryRoot "src\PdfMerge.App\PdfMerge.App.csproj"
$publishRoot = Join-Path $repositoryRoot "artifacts\publish"
$publishDirectory = Join-Path $publishRoot $Runtime
$packagesDirectory = Join-Path $repositoryRoot "artifacts\packages"
$packagePath = Join-Path $packagesDirectory "PdfMerge-$Runtime-portable.zip"

function Assert-PathInsideRepository {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    $fullRepositoryRoot = [System.IO.Path]::GetFullPath($repositoryRoot)
    if (-not $fullPath.StartsWith($fullRepositoryRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to operate outside repository root: $fullPath"
    }
}

Assert-PathInsideRepository -Path $publishDirectory
Assert-PathInsideRepository -Path $packagesDirectory
Assert-PathInsideRepository -Path $packagePath

if (Test-Path -LiteralPath $publishDirectory) {
    Remove-Item -LiteralPath $publishDirectory -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $publishDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $packagesDirectory | Out-Null

dotnet publish $projectPath `
    --configuration $Configuration `
    --runtime $Runtime `
    --self-contained true `
    --output $publishDirectory `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false

$releaseFiles = @(
    "LICENSE",
    "NOTICE",
    "README.md",
    "RELEASE_NOTES.md"
)

foreach ($releaseFile in $releaseFiles) {
    $sourcePath = Join-Path $repositoryRoot $releaseFile
    if (Test-Path -LiteralPath $sourcePath) {
        Copy-Item -LiteralPath $sourcePath -Destination $publishDirectory -Force
    }
}

if (Test-Path -LiteralPath $packagePath) {
    Remove-Item -LiteralPath $packagePath -Force
}

Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $packagePath

Write-Host "Portable publish output: $publishDirectory"
Write-Host "Portable package: $packagePath"
