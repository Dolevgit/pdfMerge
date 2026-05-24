# Completed Tasks

Tasks in this file are complete and verified against the current repository state.

## PM-01 Documentation And Tracking Foundation

Summary:

- Created the architecture plan for the portable C# WPF PDF merge application.
- Created project engineering guidelines covering coding standards, structure, naming, logging, errors, configuration, thread safety, performance, testing, versioning, and deployment.
- Created the onboarding handoff document for future developers and AI agents.
- Created local run and publish guidance for the future implementation.
- Established progress-tracking lifecycle files.

Key decisions:

- The app is a local-only portable Windows desktop application.
- WPF is the baseline UI framework.
- PDF merge behavior must be isolated behind an application-layer interface.
- English and Hebrew support are first-version requirements.
- Tests require explicit permission before being added, per `Guideline.md`.

Verified in:

- `docs/architecture_plan.md`
- `docs/project_guidelines.md`
- `docs/foundation-handoff.md`
- `docs/run_local.md`
- `progress-tracking/product_manager.md`
- `progress-tracking/not_started.md`
- `progress-tracking/in_progress.md`
- `progress-tracking/completed.md`
- `progress-tracking/blocked.md`

## PM-02 Solution Skeleton And Project Setup

Summary:

- Created `PdfMerge.sln` in classic solution format.
- Created `PdfMerge.App`, `PdfMerge.Application`, `PdfMerge.Domain`, and `PdfMerge.Infrastructure`.
- Added `Directory.Build.props` with nullable references, implicit usings, warning enforcement, analysis level, deterministic builds, and version metadata.
- Wired project references in the approved dependency direction.

Verified in:

- `dotnet build .\PdfMerge.sln`
- `dotnet build .\PdfMerge.sln -c Release`

## PM-04 Main Window Shell

Summary:

- Added WPF main window shell with native File, Edit, and Help menus.
- Added toolbar command area, selected PDF list area, and status bar.
- Used Segoe MDL2 Assets icon glyphs with text for visible buttons.
- Defined minimum supported window size and baseline keyboard bindings.

Verified in:

- `dotnet build .\PdfMerge.sln`
- Published app startup check from `artifacts/publish/win-x64/PdfMerge.App.exe`

## PM-08 Portable Settings And Window State

Summary:

- Added portable `data/settings.json` settings persistence.
- Added defaults, validation, repair of invalid values, and atomic save behavior.
- Added required settings fields for schema version, language, theme, window state, size, position, last input folder, and last output folder.
- Wired main window placement load and save.
- Fixed window placement persistence so normal size and position changes are saved before close and restored on next launch.

Verified in:

- Published app startup created `artifacts/publish/win-x64/data/settings.json`
- `dotnet build .\PdfMerge.sln`

## PM-10 Portable Publish Baseline

Summary:

- Added repeatable portable publish script at `build/publish-portable.ps1`.
- Published self-contained `win-x64` output under `artifacts/publish/win-x64`.
- Created portable package under `artifacts/packages/PdfMerge-win-x64-portable.zip`.
- Updated local run and build/deploy documentation.

Verified in:

- `.\build\publish-portable.ps1`
- Published app startup check from `artifacts/publish/win-x64/PdfMerge.App.exe`

## PM-11 Manual Validation Pass

Summary:

- Closed the remaining dark-theme validation gap by adding a File > Theme submenu.
- Added user-selectable System, Light, and Dark theme preferences.
- Persisted theme changes immediately to portable settings.
- Applied the saved theme at startup, with the System option resolving from the Windows app theme setting.
- Refined menu styling so light and dark theme menu surfaces, submenus, hover states, and separators use app theme brushes consistently.
- Removed unnecessary scroll bars from theme-styled menu popups.
- Updated the menu contract documentation to include the Theme submenu.

Verified in:

- User-provided confirmation that all PM-11 validation items except dark theme were already complete.
- `dotnet build .\PdfMerge.sln`

## PM-03 PDF Library Selection And Validation

Summary:

- Selected `PDFsharp` 6.2.4 as the PDF merge library.
- Added the package only to `PdfMerge.Infrastructure` under the MIT license.
- Replaced the deferred merge placeholder with a PDFsharp-backed `IPdfMergeService` adapter.
- Added PDF extension, existence, and readability validation through `IPdfInputValidator`.
- Documented selection reasoning, references, validation, and release-level manual validation boundaries in `docs/pdf_library_selection.md`.

Verified in:

- `dotnet build .\PdfMerge.sln`
- Temporary PM-03 smoke harness for ordinary merge, mixed-page-size merge, corrupt PDF failure, encrypted PDF failure, unsupported extension validation, and locked-file validation.

## PM-05 File Selection, Drag And Drop, And Reordering

Summary:

- Wired file picker selection through the existing WPF file dialog service.
- Added drag and drop support for file paths.
- Added localized validation feedback for non-PDF, missing, unreadable, invalid, and duplicate files.
- Defined duplicate handling as skip-and-notify using case-insensitive full path comparison.
- Displayed selected file names and full paths in the main file list.
- Added multi-select remove, select all, and single-item move up/down behavior.

Verified in:

- `dotnet build .\PdfMerge.sln`
- Temporary PM-05 view-model smoke harness for multiple-file add, non-PDF rejection, duplicate skipping, move up, move down, select all request, and remove.

## PM-06 Merge Workflow

Summary:

- Added the merge command workflow from the selected PDF list to a saved output PDF.
- Required at least two selected valid PDF files before merge.
- Added save dialog output selection and existing-output overwrite confirmation.
- Prevented duplicate merge requests by disabling merge-related commands during the workflow.
- Ran PDF merging through `IPdfMergeService` off the UI thread.
- Wrote merged output to a temporary file in the output directory before replacing or creating the final output.
- Blocked output paths that would overwrite selected input PDFs.
- Added friendly localized success and failure messages.
- Logged merge start, success, known PDF failures, validation failures, and output write failures.

Verified in:

- `dotnet build .\PdfMerge.sln`
- Temporary PM-06 workflow smoke harness for successful merge with overwrite confirmation, final replacement after temp output, cancel without overwrite, failure cleanup of temp output, no final output after failure, and input-output path rejection.

## PM-07 English And Hebrew Localization

Summary:

- Added English and Hebrew JSON locale files.
- Added JSON localization service with English fallback behavior.
- Applied right-to-left flow direction when Hebrew is selected.
- Localized current UI, file-selection workflow, and merge workflow strings.
- Added a File > Language menu with English and Hebrew options.
- Persisted language changes immediately to portable settings.
- Removed the unused Settings menu item because language is currently the only user setting.

Verified in:

- `dotnet build .\PdfMerge.sln`
- Temporary language-command smoke validation for English/Hebrew selection, persisted settings, flow direction, and localized property refresh.

## PM-09 Logging And Error Reporting

Summary:

- Added structured local file logger based on `Microsoft.Extensions.Logging`.
- Added portable log directory under `data/logs`.
- Added app lifecycle logging.
- Added status-bar transient message service with success and error state.
- Added merge workflow logging for start, success, validation failures, known merge failures, and output write failures.
- Kept user-facing errors friendly while technical details stay in local logs.

Verified in:

- `dotnet build .\PdfMerge.sln`
- Temporary PM-06 workflow smoke harness for merge success and failure paths.

## PM-12 File List Row Actions

Summary:

- Replaced the main-window `ToolBarTray` with a simpler command area for adding PDFs and running merge.
- Added row-level icon actions beside each selected PDF for move up, move down, and remove.
- Kept row actions localized through tooltips and accessibility names.
- Preserved existing menu and keyboard commands for selection-based reorder and remove behavior.

Verified in:

- `dotnet build .\PdfMerge.sln`

## PM-13 Status Bar Message Styling

Summary:

- Removed the separate main-window message host and its `HasMessage` binding.
- Simplified message state so user-facing messages are shown through the status bar.
- Added status-bar text coloring for success and error states.
- Kept transient success/error emphasis resetting automatically after 5 seconds.
- Updated project documentation and guidelines to match the status-bar-only message behavior.

Verified in:

- `dotnet build .\PdfMerge.sln`

## PM-14 Sticky Row Action Layout

Summary:

- Replaced the selected-files `GridView` with a stretchable item template.
- Added a custom localized header row for file name, full path, and actions.
- Pinned row action buttons to the right edge in left-to-right layout and to the left edge in right-to-left layout.
- Kept long file names and paths trimmed so they cannot push action buttons offscreen.
- Removed obsolete code-behind header synchronization.

Verified in:

- `dotnet build .\PdfMerge.sln`

## PM-15 About Window And Release Metadata

Summary:

- Added a branded About dialog for PdfMerge based on the ActiveTime About window pattern.
- Wired Help > About to open the modal About dialog instead of showing a status-bar message.
- Added localized About dialog text for English and Hebrew.
- Added the Codex by OpenAI badge to the About dialog.
- Wired Help > Open GitHub Project to open the PdfMerge GitHub repository.
- Added the root `LICENSE` file using PolyForm Noncommercial License 1.0.0.
- Updated release metadata to version `1.0.0` and included `logo.png` as a WPF resource.

Verified in:

- `dotnet build .\PdfMerge.sln`
