# Run Locally

## 1. Prerequisites

- Windows 10 or Windows 11
- .NET 10 SDK
- PowerShell

Recommended:

- Visual Studio 2026 or newer with the .NET desktop development workload

## 2. Install Dependencies

From the repository root:

```powershell
dotnet restore .\PdfMerge.sln
```

## 3. Configure The Environment

No manual configuration is required for the foundation build.

On first startup, the app creates portable runtime state next to the executable:

```text
data/
  settings.json
  logs/
```

The default language is English. Settings are stored in `data/settings.json`.

## 4. Start The System

Run the WPF desktop app:

```powershell
dotnet run --project .\src\PdfMerge.App\PdfMerge.App.csproj
```

## 5. Verify The System Is Running

Confirm that:

- the PdfMerge window opens
- the native menu bar is visible
- the toolbar and selected PDF list shell are visible
- the status bar shows the ready state
- `data/settings.json` is created after startup
- a log file is written under `data/logs`

## 6. Build Check

Run:

```powershell
dotnet build .\PdfMerge.sln
```

The build must complete with `0 Warning(s)` and `0 Error(s)`.

## 7. Tests

Automated tests are not present yet. Per `Guideline.md`, tests may be added only after explicit approval.
