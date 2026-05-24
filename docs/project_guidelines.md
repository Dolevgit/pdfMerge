# Project Guidelines

These guidelines are the engineering contract for PdfMerge. Follow them together with `Guideline.md`.

## 1. Core Principles

- Prefer reliability over cleverness.
- Keep the product simple.
- Keep implementation details behind clear interfaces.
- Do not introduce infrastructure that is not needed for local PDF merging.
- Keep the app portable by default.
- Keep the UI responsive and predictable.
- Treat documentation and progress tracking as part of the work.

## 2. Coding Standards

- Use C# with nullable reference types enabled.
- Treat warnings as issues to resolve, not noise to ignore.
- Use clear names over abbreviations.
- Prefer small focused classes.
- Prefer immutable models for domain data.
- Use async APIs for file and merge workflows where they prevent UI blocking.
- Do not block on async work with `.Result`, `.Wait()`, or sync-over-async patterns.
- Keep view code-behind minimal and UI-specific.
- Keep business rules out of WPF views.
- Do not add global mutable state unless there is a clear ownership reason.
- Add comments only when they explain a non-obvious decision.

## 3. File And Folder Structure

Expected repository structure:

```text
PdfMerge.sln
Directory.Build.props
src/
  PdfMerge.App/
    Assets/
    Components/
    Locales/
    Services/
    ViewModels/
    Views/
  PdfMerge.Application/
    Merging/
    Settings/
    Localization/
    Validation/
  PdfMerge.Domain/
    Files/
    Merging/
    Settings/
    Validation/
  PdfMerge.Infrastructure/
    Files/
    Logging/
    Pdf/
    Settings/
build/
docs/
progress-tracking/
tests/
  PdfMerge.Application.Tests/
  PdfMerge.Domain.Tests/
  PdfMerge.Infrastructure.Tests/
```

Rules:

- UI files stay in `PdfMerge.App`.
- Use cases and workflow coordination stay in `PdfMerge.Application`.
- Pure rules and models stay in `PdfMerge.Domain`.
- File system, logging, dialogs, settings persistence, and PDF-library adapters stay in `PdfMerge.Infrastructure` or app-specific service wrappers where WPF requires it.
- Do not let infrastructure reference WPF UI types.
- Do not let domain reference application, infrastructure, or UI projects.
- Do not create folders until they have a clear purpose, except for folders required by `Guideline.md`.

## 4. Naming Conventions

- Projects: `PdfMerge.App`, `PdfMerge.Application`, `PdfMerge.Domain`, `PdfMerge.Infrastructure`.
- Interfaces: prefix with `I`, for example `IPdfMergeService`.
- Classes: PascalCase.
- Methods: PascalCase.
- Properties: PascalCase.
- Fields: camelCase with `_` prefix for private instance fields.
- Constants: PascalCase.
- Async methods: suffix with `Async`.
- View models: suffix with `ViewModel`.
- Services: suffix with `Service`.
- Validators: suffix with `Validator`.
- Options/settings models: suffix with `Options` or `Settings`.
- Test classes: suffix with `Tests`.

File names must match the main type name.

## 5. UI Standards

- Use WPF native controls where practical.
- The desktop client must have a native menu bar.
- The desktop client must have a status bar.
- All buttons must include text and an icon.
- Use a known icon library.
- Do not use custom SVG icons.
- Do not use gradients.
- Use standard Windows dialogs for opening and saving files.
- Support drag and drop for PDF files.
- Disable merge controls while a merge is running.
- Prevent duplicate merge requests.
- Show clear success and error messages.
- Show success and error state through status-bar text color.
- Reset transient success and error emphasis after 5 seconds.
- Confirm destructive actions, including overwriting an output file and clearing a file list.
- Support keyboard navigation and standard shortcuts where applicable.
- Persist window size, position, and state.

## 6. Localization Rules

- Support English and Hebrew from the first implementation.
- Store language resources under `Locales` or equivalent strongly typed resource files.
- Use `en` for English and `he` for Hebrew.
- All user-facing text must be localizable.
- Do not concatenate localized strings in ways that break Hebrew grammar.
- Apply right-to-left flow direction for Hebrew screens.
- Persist the selected language immediately when changed or when settings are confirmed.
- Fall back to English if a translation is missing.

## 7. Logging Strategy

- Use structured logging.
- Write logs to the portable `data/logs` folder.
- Use rolling files to avoid unbounded growth.
- Default production logging should be concise.
- Debug logs may include more detail, but must still avoid PDF contents.
- Log exceptions with technical details locally.
- Show users only friendly messages.
- Do not log telemetry or send logs anywhere automatically.

Recommended log levels:

- `Information`: app lifecycle and successful high-level operations.
- `Warning`: recoverable problems such as skipped invalid files.
- `Error`: failed merge, failed settings save, failed output write.
- `Debug`: local troubleshooting details.

## 8. Error Handling Rules

- Validate inputs before starting a merge.
- Keep original PDFs untouched.
- Write output to a temporary file first.
- Move the temporary file to the final output path only after successful merge.
- Delete temporary files after failure when possible.
- Confirm before overwriting an existing file.
- Catch expected exceptions at workflow boundaries.
- Convert expected exceptions into localized user-facing messages.
- Do not display stack traces, exception type names, or raw internal errors to users.
- Preserve technical details in logs.

## 9. Configuration Management

- Store settings in `data/settings.json`.
- Create settings with defaults when missing.
- Validate settings on load.
- Repair invalid values when safe.
- Save settings atomically.
- Preserve existing settings when updating one value.
- Persist user preferences and window state between launches.
- Do not store machine-specific absolute paths unless they are user preferences such as last used folder.

Required settings:

- app settings schema version
- language
- theme
- window state
- last normal window size
- last normal window position
- last input folder
- last output folder

## 10. Thread Safety Rules

- WPF UI state must be changed only on the UI thread.
- Merge work must run off the UI thread.
- Use a single merge operation gate to prevent concurrent merges.
- Prefer immutable request objects passed into background work.
- Do not mutate the selected file collection from background threads.
- Marshal progress and completion updates back to the UI thread.
- Use cancellation only when the merge adapter can safely stop without corrupting final output.

## 11. Performance Considerations

- Keep startup lightweight.
- Do not load PDF contents just to display the file list.
- Do not generate thumbnails or previews in the initial version.
- Avoid reading large files more than necessary.
- Keep the selected file list responsive for common workloads.
- Use streaming behavior provided by the selected PDF library when available.
- Avoid holding unnecessary large byte arrays in memory.
- Keep logging lightweight during merge.

## 12. Testing Strategy

Per `Guideline.md`, automated tests are for main features only. Before adding tests, ask for explicit permission and explain why the tests are needed.

When approved, prioritize tests for:

- input validation
- merge ordering
- merge failure cleanup
- settings defaults and persistence
- localization fallback
- overwrite protection rules

Manual validation must cover:

- drag and drop
- file picker
- save dialog
- reorder controls
- Hebrew layout
- English layout
- light and dark theme
- high DPI scaling
- keyboard navigation
- portable publish execution

## 13. Versioning Rules

- Use semantic versioning: `MAJOR.MINOR.PATCH`.
- Start the first releasable version at `0.1.0` unless the owner chooses a different release plan.
- Increment `PATCH` for bug fixes.
- Increment `MINOR` for backward-compatible features.
- Increment `MAJOR` only for breaking user-facing or storage changes.
- Record release changes in release notes once release files exist.
- Keep version metadata aligned across project files, documentation, and release artifacts.

## 14. Deployment Expectations

- The primary distribution is a portable self-contained `win-x64` zip.
- No installer is required for the baseline product.
- The app must run without administrator privileges.
- The app must run without a machine-wide .NET runtime.
- Publish scripts must output to `artifacts/publish` and `artifacts/packages`.
- The portable package must include all required runtime files.
- The portable package must preserve the expected `data` folder behavior.
- Do not add auto-update behavior without explicit approval.

## 15. Documentation And Tracking Rules

Every implementation task must update the relevant progress-tracking file.

Task rules:

- A task may appear in exactly one lifecycle file.
- New planned work goes in `not_started.md`.
- Active unfinished work goes in `in_progress.md`.
- Blocked work goes in `blocked.md`.
- Finished verified work goes in `completed.md`.
- `product_manager.md` must reflect the current snapshot after each task.

Documentation must be updated when behavior, architecture, commands, deployment, or project standards change.
