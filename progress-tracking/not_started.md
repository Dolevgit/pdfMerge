# Not Started Tasks

These tasks are planned but do not yet have active implementation tracked in the repository state.

## PM-02 Solution Skeleton And Project Setup

Summary:

- Create the initial .NET solution and layered project structure.
- Add baseline build properties.
- Wire project references according to the documented architecture.

Acceptance criteria:

- `PdfMerge.sln` exists.
- `src/PdfMerge.App`, `src/PdfMerge.Application`, `src/PdfMerge.Domain`, and `src/PdfMerge.Infrastructure` exist.
- Project references follow the approved dependency direction.
- `dotnet build` succeeds.
- Documentation is updated if commands or structure change.

## PM-03 PDF Library Selection And Validation

Summary:

- Select the PDF library used by the infrastructure merge adapter.
- Validate licensing and merge behavior before implementation depends on it.

Acceptance criteria:

- Selected library has acceptable redistribution terms.
- Library can merge ordinary PDFs in order.
- Known limitations are documented.
- Encrypted, corrupt, locked, and unsupported PDFs produce clear failures.
- No AGPL, GPL, LGPL, trial, or commercial-only dependency is added without explicit approval.

## PM-04 Main Window Shell

Summary:

- Build the WPF main window foundation with menu bar, command area, selected-file list area, status bar, and message host.

Acceptance criteria:

- Native menu bar exists with File, Edit, and Help menus.
- Native status bar exists.
- Buttons include both text and icons.
- No gradients are used.
- Minimum window size is defined.
- Keyboard navigation is usable.

## PM-05 File Selection, Drag And Drop, And Reordering

Summary:

- Allow users to add PDF files by drag and drop and by standard Windows file picker.
- Display selected PDF names and allow ordering changes.

Acceptance criteria:

- Multiple PDFs can be selected from the file picker.
- Drag and drop accepts PDF files.
- Non-PDF files are rejected with a clear localized message.
- Duplicate handling is defined and implemented.
- Files can be moved up and down.
- Files can be removed from the list.

## PM-06 Merge Workflow

Summary:

- Implement the merge use case from selected PDFs to one output PDF.

Acceptance criteria:

- Merge requires at least two valid PDF files.
- Save dialog asks for output path.
- Existing output file overwrite requires confirmation.
- Merge runs off the UI thread.
- Merge controls are disabled while running.
- Output is written through a temporary file first.
- Input files are never modified.
- Failures show friendly localized messages and are logged.

## PM-07 English And Hebrew Localization

Summary:

- Add first-version localization support for English and Hebrew.

Acceptance criteria:

- All user-facing strings are localized.
- English resources exist.
- Hebrew resources exist.
- Hebrew layout uses right-to-left flow direction where appropriate.
- Language preference persists between launches.
- Missing translations fall back safely.

## PM-08 Portable Settings And Window State

Summary:

- Persist portable app settings and window state.

Acceptance criteria:

- `data/settings.json` is created when missing.
- Settings include language, theme, window state, window size, window position, last input folder, and last output folder.
- Settings are validated on load.
- Settings are saved atomically.
- Window state persists between launches.

## PM-09 Logging And Error Reporting

Summary:

- Add local structured logging and consistent user-facing error behavior.

Acceptance criteria:

- Logs are written under `data/logs`.
- Merge failures are logged with technical details.
- User messages do not expose stack traces or internal exception names.
- Transient messages can be dismissed and auto-dismiss after 5 seconds.
- Status bar reflects high-level app state.

## PM-10 Portable Publish Baseline

Summary:

- Add a repeatable portable publish path for Windows.

Acceptance criteria:

- Publish command creates self-contained `win-x64` output.
- Package can run without installation.
- Package can run without machine-wide .NET runtime.
- Output is written under `artifacts/publish` and `artifacts/packages`.
- Publish instructions in `docs/run_local.md` are accurate.

## PM-11 Manual Validation Pass

Summary:

- Validate the first usable version manually on Windows.

Acceptance criteria:

- Drag and drop verified.
- File picker verified.
- Reordering verified.
- Merge success verified.
- Merge failure behavior verified.
- Hebrew UI verified.
- English UI verified.
- Light and dark theme verified.
- Portable publish execution verified.
- Any gaps are recorded in the correct progress-tracking file.
