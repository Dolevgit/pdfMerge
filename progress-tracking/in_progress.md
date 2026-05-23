# In Progress Tasks

Tasks in this file have meaningful implementation already, but they are not finished or have a material gap that prevents closure.

## PM-03 PDF Library Selection And Validation

Completed:

- Selected `PDFsharp` 6.2.4 as the PDF merge library.
- Added the package only to `PdfMerge.Infrastructure`.
- Implemented a PDFsharp-backed `IPdfMergeService` adapter.
- Added PDF extension, existence, and readability validation through `IPdfInputValidator`.
- Documented license, selection reasoning, and open limitations in `docs/pdf_library_selection.md`.
- Smoke-validated ordinary merge behavior with two generated one-page PDFs.

Still missing:

- Manual validation for encrypted, corrupt, locked, unsupported, large, and mixed-page-size PDFs.
- Full output safety behavior remains part of PM-06, including temporary output files and overwrite protection.

Why this stays open:

- The ordinary merge path is implemented, but the full PM-03 edge-case validation matrix is not complete yet.

Completion:

- 50%

## PM-05 File Selection, Drag And Drop, And Reordering

Completed:

- Wired file picker selection through the existing WPF file dialog service.
- Added drag and drop support for file paths.
- Added localized validation feedback for non-PDF, missing, unreadable, invalid, and duplicate files.
- Defined duplicate handling as skip-and-notify using case-insensitive full path comparison.
- Displayed selected file names and full paths in the main file list.
- Added multi-select remove, select all, and single-item move up/down behavior.

Still missing:

- Manual Windows validation for file picker, drag and drop, keyboard selection, and reorder behavior.
- PM-06 still needs to consume the selected list for the final merge workflow.

Why this stays open:

- The implementation is present, but manual interaction validation is still required before closure.

Completion:

- 75%

## PM-07 English And Hebrew Localization

Completed:

- Added English and Hebrew JSON locale files.
- Added JSON localization service with English fallback behavior.
- Applied right-to-left flow direction when Hebrew is selected in settings.
- Localized current foundation UI strings.
- Added localized PM-05 file selection, validation, duplicate, reorder, and remove messages.

Still missing:

- Language selection UI is not implemented yet.
- Future PM-06 user-facing workflow strings must be added as that task begins.

Why this stays open:

- Full first-version localization cannot close until feature workflow strings and language selection are implemented.

Completion:

- 60%

## PM-09 Logging And Error Reporting

Completed:

- Added structured local file logger based on `Microsoft.Extensions.Logging`.
- Added portable log directory under `data/logs`.
- Added app lifecycle logging.
- Added status and dismissible transient message service.

Still missing:

- Merge failure logging cannot be completed until PM-06 implements the merge workflow.

Why this stays open:

- Logging foundation exists, but acceptance criteria tied to merge failures remain future work.

Completion:

- 60%

## Entry Template

Use this structure when a task moves here:

## PM-XX Task Name

Completed:

- completed implementation that already exists

Still missing:

- remaining functional gaps

Why this stays open:

- why the current state is not enough for completion

Completion:

- 0%
