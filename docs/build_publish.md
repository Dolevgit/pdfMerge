# Build, Compile, And Publish PdfMerge

This guide explains the supported developer workflow for building and publishing PdfMerge.

For the public-release pass, also use `docs/release_checklist.md` after the build and packaging steps below.

## 1. Restore

From the repository root, run:

```powershell
dotnet restore .\PdfMerge.sln
```

## 2. Compile A Debug Build

Run:

```powershell
dotnet build .\PdfMerge.sln -c Debug
```

Use this for normal day-to-day development.

## 3. Compile A Release Build

Run:

```powershell
dotnet build .\PdfMerge.sln -c Release
```

This verifies the optimized build path before publishing.

## 4. Automated Tests

Automated tests are not currently present. Per `Guideline.md`, tests may be added only after explicit approval.

## 5. Publish The Baseline `win-x64` Output

Use the repository script:

```powershell
.\build\publish-portable.ps1
```

Notes:

1. `win-x64` is the first supported release target in the project guidelines.
2. The publish output is self-contained for predictable local deployment.
3. The script publishes to `artifacts\publish\win-x64` and writes `artifacts\packages\PdfMerge-win-x64-portable.zip`.
4. Trimming is intentionally not enabled.
5. The portable package includes `LICENSE`, `NOTICE`, `README.md`, and `RELEASE_NOTES.md`.

Manual fallback:

```powershell
dotnet publish .\src\PdfMerge.App\PdfMerge.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -o .\artifacts\publish\win-x64 `
  -p:PublishSingleFile=false

Compress-Archive `
  -Path .\artifacts\publish\win-x64\* `
  -DestinationPath .\artifacts\packages\PdfMerge-win-x64-portable.zip `
  -Force
```

## 6. Portable Package Behavior

After publishing:

1. Distribute either the full `artifacts\publish\win-x64` directory or `artifacts\packages\PdfMerge-win-x64-portable.zip`.
2. Run `PdfMerge.App.exe` from a writable folder.
3. PdfMerge writes runtime data under the app folder's `data\` directory.

## 7. Recommended Final Validation Before Sharing A Build

Run through this minimum checklist:

1. Start the published app from `artifacts\publish\win-x64\PdfMerge.App.exe`.
2. Confirm `data\settings.json` and `data\logs\` are created under the publish folder.
3. Add multiple PDF files with the file picker.
4. Add PDF files with drag and drop.
5. Reorder files and remove one row.
6. Merge at least two readable PDFs.
7. Confirm overwrite protection appears for an existing output file.
8. Switch between English and Hebrew and confirm Hebrew uses right-to-left layout.
9. Switch between light, dark, and system themes.
10. Restart once and confirm language, theme, and window placement persist.
