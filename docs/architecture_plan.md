# Architecture Plan

## 1. Product Summary

PdfMerge is a simple portable Windows desktop application for merging multiple PDF files into one PDF file.

The application must be easy for non-technical users:

- add PDF files by drag and drop or by Windows file picker
- view the selected PDF file names
- reorder files before merge
- merge the selected files into one output PDF
- use the app in English or Hebrew
- run without an installer

Reliability is more important than advanced features. The first version must focus only on a clear and dependable merge workflow.

## 2. Architecture Goals

- Keep the application local-only and portable by default.
- Keep the user workflow short and predictable.
- Keep PDF processing isolated from the UI.
- Keep all long-running work off the UI thread.
- Keep dependencies minimal and compatible with redistribution.
- Support Hebrew and English from the first implementation.
- Make future maintenance safe for developers and AI agents through clear boundaries.

## 3. Non-Goals

The initial product must not include:

- cloud storage
- user accounts
- telemetry
- online services
- PDF editing
- PDF compression
- PDF splitting
- password removal
- OCR
- installer-only deployment
- database storage

Any feature outside the merge workflow requires explicit approval before implementation.

## 4. Technology Baseline

Use the following baseline unless the project owner explicitly approves a change:

- Language: C#
- Runtime: .NET 10 LTS
- UI framework: WPF
- Target OS: Windows 10 and Windows 11
- Architecture: layered desktop application
- Packaging: self-contained portable `win-x64` publish output
- Configuration format: JSON
- Localization format: JSON resource files or strongly typed `.resx` resources
- Logging: structured local file logging through `Microsoft.Extensions.Logging` with a file sink

WPF is selected because it is mature, stable, native to Windows desktop development, supports drag and drop, supports keyboard navigation, supports menu and status bars, and can be packaged as a self-contained portable app.

## 5. Dependency Policy

PDF merging must be implemented behind an application-layer interface so the selected PDF library can be replaced without changing the UI.

Dependency rules:

- Prefer a maintained, permissively licensed .NET PDF library.
- Do not add AGPL, GPL, LGPL, trial, or commercial-only dependencies without explicit approval.
- Do not add a browser engine, web server, database, or native service for the initial version.
- Do not add dependencies only for convenience if the .NET platform already provides a simple, reliable option.
- Validate the selected PDF library against normal, large, encrypted, corrupt, and mixed-page-size PDFs before closing the PDF engine task.

The selected PDF library is `PDFsharp` 6.2.4, added only to `PdfMerge.Infrastructure` under the MIT license. Application and UI code must continue to depend on `IPdfMergeService`, not on concrete PDFsharp types. Selection details and open validation gaps are tracked in `docs/pdf_library_selection.md`.

## 6. Proposed Solution Structure

```text
PdfMerge.sln
src/
  PdfMerge.App/
  PdfMerge.Application/
  PdfMerge.Domain/
  PdfMerge.Infrastructure/
tests/
  PdfMerge.Application.Tests/
  PdfMerge.Domain.Tests/
  PdfMerge.Infrastructure.Tests/
build/
docs/
progress-tracking/
```

Project responsibilities:

- `PdfMerge.App`: WPF windows, view models, commands, menus, status bar, drag and drop, localization binding, user interaction.
- `PdfMerge.Application`: use cases, workflow coordination, validation orchestration, merge request models, progress reporting contracts.
- `PdfMerge.Domain`: core rules and simple immutable models such as PDF input file, merge job, validation result, and language preference.
- `PdfMerge.Infrastructure`: file-system access, settings persistence, logging setup, PDF library adapter, path resolution.
- `tests`: approved automated tests for main behaviors only.
- `build`: publish scripts and release packaging helpers.

## 7. Runtime Model

The app is a single-process desktop application.

There is no client/server split because the product is a local desktop utility and no server is required. If a server is proposed later, the project must be redesigned with explicit owner approval.

Runtime responsibilities:

1. Start WPF application.
2. Load settings from portable app data.
3. Apply language, theme, and saved window state.
4. Show the main window.
5. Accept files from drag and drop or file picker.
6. Validate selected files.
7. Let the user reorder the list.
8. Ask for an output path.
9. Run merge operation on a background thread.
10. Report success or clear user-facing failure in the status bar and dismissible message area.

## 8. UI Architecture

The main window is the primary experience. Do not add a landing page.

Required main-window areas:

- native menu bar
- toolbar or command area
- selected PDF file list
- reorder controls
- merge command
- status bar
- dismissible message host

Required menu bar:

- File
  - Add PDFs
  - Settings
  - Exit
- Edit
  - Move Up
  - Move Down
  - Remove
  - Select All
- Help
  - Open GitHub Project
  - About

Button rules:

- Every visible button must include text and an icon.
- Use a known icon library.
- Do not use custom SVG icons.
- Icons must match the action.
- Disable buttons while a merge is running.
- Prevent duplicate merge requests.

Status and message rules:

- User-facing information should appear in the status bar.
- Success and error messages must be dismissible.
- Messages should disappear automatically after 5 seconds.
- Technical exception details must not be shown directly to users.

Visual rules:

- Do not use gradients.
- Support light mode and dark mode.
- Follow the operating system theme by default.
- Define a minimum supported window size.
- Keep the UI usable at different DPI and scaling settings.
- Support keyboard navigation.
- Support standard Windows shortcuts where applicable.

## 9. Localization Architecture

The app must support:

- English: `en`
- Hebrew: `he`

Localization requirements:

- All user-facing strings must come from localization resources.
- Do not hard-code user-facing strings in views, view models, services, exceptions, or validation messages.
- Hebrew UI must use right-to-left flow direction where appropriate.
- English UI must use left-to-right flow direction.
- The language preference must persist between app launches.
- If localization loading fails, the app must fall back to English and log the failure.

Recommended structure:

```text
src/PdfMerge.App/Locales/en.json
src/PdfMerge.App/Locales/he.json
```

If `.resx` is selected during implementation, the same rule applies: English and Hebrew resources are required from the first version.

## 10. Configuration And Portable State

The app is portable by default.

Runtime state should be stored under the application folder:

```text
data/
  settings.json
  logs/
```

Persisted settings:

- language
- theme preference
- last window state
- last normal window size
- last normal window position
- last used input folder
- last used output folder

Configuration rules:

- Create default settings when missing.
- Validate settings before applying them.
- Repair invalid individual values when possible.
- Preserve unknown future settings when practical.
- Write settings atomically to avoid partial files.
- If settings cannot be saved, keep the app usable and show a clear status-bar warning.

## 11. Merge Workflow

The merge workflow must be deterministic:

1. User adds PDF files.
2. App filters for `.pdf` files.
3. App validates that each file exists and is readable.
4. App displays file names and full paths.
5. User reorders files.
6. User clicks Merge.
7. App validates that at least two PDF files are selected.
8. App asks for output path using the standard Windows save dialog.
9. App confirms overwrite if the selected output file already exists.
10. App disables merge-related controls.
11. App merges files in the visible order.
12. App writes to a temporary file in the output directory.
13. App replaces or creates the final output file only after merge success.
14. App re-enables controls and shows success or failure.

Overwrite is destructive and must require confirmation.

## 12. Application Services

Required application-layer contracts:

- `IPdfMergeService`: merges validated files into one output PDF.
- `IPdfInputValidator`: validates selected files before merge.
- `ISettingsService`: loads, validates, updates, and saves settings.
- `ILocalizationService`: provides localized strings and culture direction.
- `IFileDialogService`: wraps Windows open/save dialogs for testability.
- `IMessageService`: coordinates status-bar and dismissible messages.

View models may call application services. Views must not call infrastructure directly.

## 13. Threading Model

Threading rules:

- The UI thread owns WPF controls and observable UI state.
- PDF merging must run on a background task.
- UI updates from background work must be marshaled back to the UI thread.
- Only one merge operation may run at a time.
- Cancellation may be added if it is implemented cleanly and does not risk corrupt output.
- Shared state must be immutable or protected by a clear ownership model.

The first version should use a simple busy state instead of a complex job scheduler.

## 14. Error Handling

Expected user-facing failures:

- no files selected
- fewer than two files selected
- file does not exist
- file is not readable
- file is not a supported PDF
- encrypted or password-protected PDF cannot be merged
- output path is invalid
- output file is locked
- disk write fails
- merge library fails on a corrupt PDF

Rules:

- Show clear human-readable messages.
- Do not expose stack traces or internal exception names.
- Log technical details locally.
- Leave original input files untouched.
- Do not leave a partial final output file after failure.
- Delete temporary output files after failure when possible.

## 15. Logging Strategy

Log locally only.

Required log events:

- application start and exit
- settings load/save failures
- files added count
- validation failures with safe metadata
- merge start
- merge success
- merge failure
- publish/build script failures when applicable

Do not log:

- full PDF contents
- personal document text
- unnecessary full paths at information level

Full paths may be logged at debug level only when needed for local troubleshooting.

## 16. Performance Expectations

Performance rules:

- The UI must remain responsive during merge.
- Large files must be streamed or copied through the PDF library without loading avoidable full document content into UI memory.
- The app should handle dozens of PDFs in a single merge.
- The selected file list should remain responsive for typical user workloads.
- Avoid background indexing, previews, or thumbnails in the initial version.
- Do not add PDF preview unless explicitly approved.

## 17. Security And Privacy

The app processes local files only.

Security rules:

- Never upload PDFs.
- Never send telemetry.
- Never execute files selected by the user.
- Treat file paths as user data.
- Validate output path before writing.
- Write output through a temporary file first.
- Do not modify input PDFs.

## 18. Testing And Validation Scope

Automated tests are for main features only and require explicit approval before being added, according to `Guideline.md`.

Recommended validation areas after approval:

- file validation
- merge ordering
- output overwrite protection
- settings persistence
- language selection
- Hebrew right-to-left behavior
- failure cleanup of temporary files

Manual validation is required for:

- drag and drop
- Windows file picker
- save dialog
- DPI scaling
- keyboard navigation
- light and dark theme behavior
- portable publish output

## 19. Deployment Architecture

The primary release artifact is a portable self-contained `win-x64` folder or zip.

Required release output:

```text
artifacts/
  publish/
    win-x64/
  packages/
    PdfMerge-win-x64-portable.zip
```

Release rules:

- The app must run without a machine-wide .NET runtime installation.
- The app must not require administrator privileges.
- The app must not require installation.
- The app must store state in its portable `data` folder.
- Publish output must include all required runtime dependencies.
- Build scripts must be repeatable from a clean checkout after implementation exists.

## 20. Change Control

Architecture changes must update:

- `docs/architecture_plan.md`
- `docs/project_guidelines.md` when standards or process change
- `docs/run_local.md` when commands or prerequisites change
- the relevant progress-tracking files

Any change that adds infrastructure, persistence, external communication, or new product scope requires explicit approval.
