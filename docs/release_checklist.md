# Public Release Checklist

This checklist is the final release-preparation pass for the first public PdfMerge release. It complements `docs/build_publish.md` by focusing on public-facing release readiness instead of only local build mechanics.

## 1. Required Decisions Before Publishing

Chosen for the first public release:

1. Version tag: `v1.0.0`
2. Repository license: `PolyForm Noncommercial License 1.0.0`
3. Repository URL: `https://github.com/Dolevgit/pdfMerge`
4. Release format: portable `win-x64` zip only

## 2. Preflight Verification

From a clean and verified working tree:

```powershell
dotnet build .\PdfMerge.sln -c Release
.\build\publish-portable.ps1
```

Manual spot checks:

1. Launch the portable build and confirm runtime files appear under `<publish root>\data\`.
2. Confirm the main window, menu bar, file list, row actions, and status bar render correctly.
3. Confirm File > Language switches between English and Hebrew.
4. Confirm File > Theme switches between System, Light, and Dark.
5. Confirm Help > About and Help > Open GitHub Project behave correctly.

## 3. Release Asset

Attach this asset to the GitHub release:

- `artifacts\packages\PdfMerge-win-x64-portable.zip`

Release text should make the portable behavior explicit:

- Portable zip: no installation required, settings and logs stay beside the executable under `data\`.

## 4. GitHub Release Metadata

Recommended release title pattern:

- `PdfMerge v1.0.0`

Recommended short release summary:

- Local Windows PDF merge utility
- File picker and drag-and-drop input
- English and Hebrew support
- Portable zip distribution

Recommended release body sections:

1. Overview
2. What is included
3. Portable usage
4. Privacy and storage notes
5. Known limitations

## 5. Repository Hygiene

Confirm the following files are present and up to date:

- `README.md`
- `RELEASE_NOTES.md`
- `CHANGELOG.md`
- `LICENSE`
- `NOTICE`
- issue templates under `.github\ISSUE_TEMPLATE\`
- `.github\PULL_REQUEST_TEMPLATE.md`
- `docs\build_publish.md`
- `docs\manual_validation.md`
- `docs\release_checklist.md`

## 6. Post-Publish Check

After the release goes live:

1. Download the portable zip from the public release page and launch it once.
2. Verify the release body clearly states that PdfMerge is local-only and portable.
3. Verify the attached asset name matches the documented package name exactly.
4. Verify the zip contains the app executable plus `LICENSE`, `NOTICE`, `README.md`, and `RELEASE_NOTES.md`.
