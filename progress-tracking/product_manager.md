# Product Manager Overview

Audit date: 2026-05-23

Current snapshot:

- The repository contains the documentation foundation and initial C# WPF solution foundation for a portable Windows desktop PDF merge app.
- Architecture, development guidelines, onboarding handoff, and local run guidance are defined.
- Progress-tracking lifecycle files are established.
- Application source projects now exist with layered boundaries, a WPF shell, portable settings, local file logging, localization resources, and portable publish script.
- PDFsharp has been selected, wired behind the infrastructure merge adapter, and validated for PM-03 edge cases.
- File picker, drag and drop, selected-file display, duplicate skipping, removal, select all, and single-item reordering are implemented for PDF inputs.
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

- `PM-07` English And Hebrew Localization - 60%
- `PM-09` Logging And Error Reporting - 60%

## 2. Blocked Tasks

- None

## 3. Not Yet Started Tasks

- `PM-06` Merge Workflow
- `PM-11` Manual Validation Pass

## 4. Completed Tasks

- `PM-01` Documentation And Tracking Foundation
- `PM-02` Solution Skeleton And Project Setup
- `PM-03` PDF Library Selection And Validation
- `PM-04` Main Window Shell
- `PM-05` File Selection, Drag And Drop, And Reordering
- `PM-08` Portable Settings And Window State
- `PM-10` Portable Publish Baseline

## 5. Current Product Reality

- The project is in foundation stage with a verified runnable WPF shell.
- The architecture uses a local WPF desktop application with layered project boundaries.
- The app is planned as portable by default with settings and logs under a local `data` folder.
- The merge engine remains isolated behind an application-layer interface.
- PDFsharp is selected, wired behind `IPdfMergeService`, and validated for PM-03 edge cases.
- File selection, drag and drop, duplicate skipping, selected-file display, remove, select all, and reordering are implemented for PM-05.
- English and Hebrew resource loading exists, but language selection UI and future PM-06 workflow strings remain open under PM-07.
- Local logging exists, but merge failure logging remains open under PM-09 until PM-06 is implemented.
- Tests require explicit permission before being added, per `Guideline.md`.
