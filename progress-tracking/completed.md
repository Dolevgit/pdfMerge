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
- Added toolbar command area, selected PDF list area, status bar, and dismissible message host.
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
