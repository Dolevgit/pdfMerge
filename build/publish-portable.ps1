param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot "src/PdfMerge.App/PdfMerge.App.csproj"
$publishDirectory = Join-Path $repositoryRoot "artifacts/publish/win-x64"
$packagesDirectory = Join-Path $repositoryRoot "artifacts/packages"
$packagePath = Join-Path $packagesDirectory "PdfMerge-win-x64-portable.zip"

New-Item -ItemType Directory -Force $publishDirectory | Out-Null
New-Item -ItemType Directory -Force $packagesDirectory | Out-Null

dotnet publish $projectPath `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained true `
    --output $publishDirectory `
    -p:PublishSingleFile=false

if (Test-Path -LiteralPath $packagePath) {
    Remove-Item -LiteralPath $packagePath
}

Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $packagePath

Write-Host "Portable publish output: $publishDirectory"
Write-Host "Portable package: $packagePath"
