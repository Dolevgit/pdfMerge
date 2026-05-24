# Manual Validation Matrix

This document records the current manual validation surface for the PdfMerge release baseline.

## 1. Latest Validation Evidence

Latest automated execution recorded on 2026-05-24:

- Command: `dotnet build .\PdfMerge.sln -c Release`
- Result: Passed
- Command: `.\build\publish-portable.ps1`
- Result: Passed and produced `artifacts\publish\win-x64` plus `artifacts\packages\PdfMerge-win-x64-portable.zip`
- Command: portable package contents check
- Result: Passed; zip includes `PdfMerge.App.exe`, `LICENSE`, `NOTICE`, `README.md`, and `RELEASE_NOTES.md`
- Command: published app startup smoke check from `artifacts\publish\win-x64\PdfMerge.App.exe`
- Result: Passed

Latest human-operated execution recorded on 2026-05-24:

- Source: repository user-reported validation pass
- Result: Passed except for the former dark-theme discoverability gap, which has since been addressed with File > Theme.

## 2. Manual Validation Matrix

| ID | Validation Type | Scenario | Expected Result |
| --- | --- | --- | --- |
| PM11-M01 | MANUAL | Drag and drop PDF files | PDF files are added, non-PDF files are rejected with a clear status message |
| PM11-M02 | MANUAL | File picker PDF selection | Multiple PDF files can be selected and added |
| PM11-M03 | MANUAL | Reordering | Move up and move down actions update the visible merge order |
| PM11-M04 | MANUAL | Merge success | Merging at least two valid PDFs creates a readable output PDF |
| PM11-M05 | MANUAL | Merge failure behavior | Invalid, encrypted, corrupt, or locked files show friendly errors and do not leave a partial final output |
| PM11-M06 | MANUAL | Overwrite protection | Existing output files require confirmation before replacement |
| PM11-M07 | MANUAL | English UI | English strings and left-to-right layout are usable |
| PM11-M08 | MANUAL | Hebrew UI | Hebrew strings and right-to-left layout are usable |
| PM11-M09 | MANUAL | Theme switching | System, Light, and Dark themes are selectable and persist after restart |
| PM11-M10 | MANUAL | Portable publish execution | Published app starts from `artifacts\publish\win-x64` and creates runtime state under that folder's `data\` directory |
| PM11-M11 | MANUAL | About and project links | About dialog opens and GitHub project link launches through the shell |
| PM11-M12 | MANUAL | Window persistence | Window size, position, state, language, and theme persist after restart |

## 3. Test Environment Template

Record this once per manual pass:

| Field | Value |
| --- | --- |
| Date | |
| Tester | |
| Machine | |
| Windows version | |
| Display configuration | |
| PdfMerge build | |
| Theme under test | |
| Language under test | |

Capture for failed or ambiguous scenarios:

- relevant local log lines
- the exact action taken
- a screenshot only when the issue is visual

## 4. Completion Rule

Tasks may be marked complete when:

1. Required build and publish commands pass.
2. Remaining manual-only scenarios have been executed.
3. The evidence above is updated to match the latest verified state.
