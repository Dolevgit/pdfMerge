# Run Local

This document is retained for compatibility with the foundation handoff.
The canonical local run guide is `docs/run_locally.md`.

## 1. Prerequisites

Required:

- Windows 10 or Windows 11
- .NET 10 SDK
- PowerShell

Recommended:

- Visual Studio 2026 or newer with .NET desktop development workload
- Windows Terminal

## 2. Restore

From the repository root:

```powershell
dotnet restore .\PdfMerge.sln
```

## 3. Build

```powershell
dotnet build .\PdfMerge.sln
```

For release configuration:

```powershell
dotnet build .\PdfMerge.sln -c Release
```

## 4. Run

```powershell
dotnet run --project .\src\PdfMerge.App\PdfMerge.App.csproj
```

## 5. Test

Automated tests may be added only after explicit permission, as required by `Guideline.md`.

When approved tests exist, run:

```powershell
dotnet test .\PdfMerge.sln
```

## 6. Publish Portable Build

The baseline release target is a self-contained portable Windows build.

Run:

```powershell
.\build\publish-portable.ps1
```

Expected output:

```text
artifacts/
  publish/
    win-x64/
  packages/
    PdfMerge-win-x64-portable.zip
```

The published app must run without installation and without a machine-wide .NET runtime.

## 7. Manual Validation Checklist

Before a release, manually validate:

- app starts from portable publish folder
- app creates or loads `data/settings.json`
- app writes logs under `data/logs`
- drag and drop accepts PDF files
- file picker accepts multiple PDF files
- non-PDF files are rejected with a clear message
- selected files can be reordered
- Merge is disabled until enough valid PDFs are selected
- overwrite confirmation appears before replacing an output file
- merge creates a readable output PDF
- failed merge does not leave a partial final output file
- English UI is usable
- Hebrew UI is usable and right-to-left where appropriate
- light mode is usable
- dark mode is usable
- window size and position persist between launches
- keyboard navigation works for the main workflow
