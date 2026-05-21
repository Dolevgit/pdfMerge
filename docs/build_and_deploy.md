# Build And Deploy

## 1. Restore

```powershell
dotnet restore .\PdfMerge.sln
```

## 2. Build

Debug build:

```powershell
dotnet build .\PdfMerge.sln
```

Release build:

```powershell
dotnet build .\PdfMerge.sln -c Release
```

## 3. Publish Portable Build

Create the self-contained Windows x64 portable output:

```powershell
.\build\publish-portable.ps1
```

The script publishes the WPF app to:

```text
artifacts/publish/win-x64/
```

It also creates the portable zip package:

```text
artifacts/packages/PdfMerge-win-x64-portable.zip
```

## 4. Prepare For Deployment

Before sharing a build:

```powershell
dotnet build .\PdfMerge.sln -c Release
.\build\publish-portable.ps1
```

Then verify that `PdfMerge.App.exe` starts from `artifacts/publish/win-x64`.

## 5. Runtime State

The app is portable by default. Runtime state is created under the application folder:

```text
data/
  settings.json
  logs/
```

Do not package local developer `data` folders into release artifacts.

## 6. Deployment Notes

- The baseline artifact is a portable folder or zip.
- No installer is required.
- Administrator privileges are not required.
- The published app is self-contained for `win-x64` and does not require a machine-wide .NET runtime.
