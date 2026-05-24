# Product Manager Overview

Audit date: 2026-05-24

Current snapshot:

- The repository contains the documentation foundation and initial C# WPF solution foundation for a portable Windows desktop PDF merge app.
- Architecture, development guidelines, onboarding handoff, and local run guidance are defined.
- Progress-tracking lifecycle files are established.
- Application source projects now exist with layered boundaries, a WPF shell, portable settings, local file logging, localization resources, and portable publish script.
- Window size, position, and state persist across app launches.
- PDFsharp has been selected, wired behind the infrastructure merge adapter, and validated for PM-03 edge cases.
- File picker, drag and drop, selected-file display, duplicate skipping, removal, select all, and single-item reordering are implemented for PDF inputs.
- The merge workflow is implemented with save dialog output selection, overwrite confirmation, temp-file output safety, busy-state gating, friendly localized messages, and local failure logging.
- English and Hebrew localization is implemented with a File > Language menu and persisted language selection.
- Theme selection is implemented with a File > Theme menu for System, Light, and Dark preferences, with persisted selection.
- Theme-aware menu styling now covers menu bars, submenus, hover states, and separators.
- The selected-file list now includes row-level move up, move down, and remove icon actions, with the main command area simplified outside `ToolBarTray`.
- Row-level actions stay pinned to the selected-file list edge: right in English/LTR and left in Hebrew/RTL.
- Success and error messages are shown in the status bar with transient green/red text emphasis.
- Release metadata is set to version `1.0.0`, the PolyForm Noncommercial license is present, and Help > About opens a branded PdfMerge dialog with Codex attribution.
- Public release preparation is in place with README, release notes, changelog, NOTICE, release checklist, manual validation matrix, GitHub templates, and a verified portable zip.
- The approved product scope is a simple local PDF merger with English and Hebrew support.

Task lifecycle:

- `not_started.md` -> `in_progress.md` -> `completed.md`
- blocked tasks live in `blocked.md` until they can return to `not_started.md` or `in_progress.md`

Maintenance rule:

- After every task, update the task entry in the appropriate lifecycle file.
- If a task is in progress, keep its completion percent current in `in_progress.md`.
- After every task, update the matching status and completion percent in `product_manager.md`.
- Do not mark a task completed unless its acceptance criteria and required document updates are done.

## 1. In Progress Tasks

- None

## 2. Blocked Tasks

- None

## 3. Not Yet Started Tasks

- None

## 4. Completed Tasks

- `PM-01` Documentation And Tracking Foundation
- `PM-02` Solution Skeleton And Project Setup
- `PM-03` PDF Library Selection And Validation
- `PM-04` Main Window Shell
- `PM-05` File Selection, Drag And Drop, And Reordering
- `PM-06` Merge Workflow
- `PM-07` English And Hebrew Localization
- `PM-08` Portable Settings And Window State
- `PM-09` Logging And Error Reporting
- `PM-10` Portable Publish Baseline
- `PM-11` Manual Validation Pass
- `PM-12` File List Row Actions
- `PM-13` Status Bar Message Styling
- `PM-14` Sticky Row Action Layout
- `PM-15` About Window And Release Metadata
- `PM-16` Public Release Preparation

## 5. Current Product Reality

- The project is in foundation stage with a verified runnable WPF shell.
- The architecture uses a local WPF desktop application with layered project boundaries.
- The app is planned as portable by default with settings and logs under a local `data` folder.
- The merge engine remains isolated behind an application-layer interface.
- PDFsharp is selected, wired behind `IPdfMergeService`, and validated for PM-03 edge cases.
- File selection, drag and drop, duplicate skipping, selected-file display, remove, select all, and reordering are implemented for PM-05.
- Merge workflow is implemented for PM-06, including temp-file output safety and overwrite confirmation.
- English and Hebrew localization is implemented with immediate persisted language switching.
- Theme selection is implemented with immediate persisted switching and a System option that follows the Windows app theme preference.
- Window placement is loaded on startup and saved after resize/move/state changes and before close.
- Selected PDF rows include direct move up, move down, and remove actions with localized tooltips and accessible names.
- File row actions remain visible at the trailing physical edge for the current language direction.
- The app has version `1.0.0`, a root `LICENSE`, an About dialog using the PdfMerge logo, and a working GitHub project menu link.
- The repository now has public-release docs and `artifacts/packages/PdfMerge-win-x64-portable.zip` has been produced by the portable publish script.
- Local logging and user-facing error reporting are implemented for current workflows.
- The main window uses status-bar-only user messages with transient success/error color.
- Tests require explicit permission before being added, per `Guideline.md`.
