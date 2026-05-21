# Foundation Handoff

This file is the required starting point for every developer or AI agent joining PdfMerge.

## Files You Must Review Before Writing Code

Read all of the following in full:

1. `Guideline.md`
2. `docs/architecture_plan.md`
3. `docs/project_guidelines.md`
4. `docs/run_local.md`
5. `progress-tracking/product_manager.md`
6. `progress-tracking/not_started.md`
7. `progress-tracking/in_progress.md`
8. `progress-tracking/completed.md`
9. `progress-tracking/blocked.md`

Do not skip sections. These files are the single source of truth for architecture, engineering rules, and task state.

## Working Rules

- Follow `Guideline.md` first.
- Follow `docs/architecture_plan.md` for product scope, architecture, and boundaries.
- Follow `docs/project_guidelines.md` for coding standards, structure, logging, testing, and deployment expectations.
- Follow `docs/run_local.md` for local build, run, and publish commands after implementation exists.
- Keep the product minimal.
- Do not add unnecessary infrastructure.
- Do not add a server.
- Do not add telemetry.
- Do not add an installer unless explicitly approved.
- Do not add tests unless explicit permission is given, as required by `Guideline.md`.
- Stop and clarify if a requirement conflicts with `Guideline.md` or with established project documentation.

## Architecture Summary

PdfMerge is a portable Windows desktop app built with C# and WPF.

The app merges multiple PDF files into one output PDF. Users can add PDF files by drag and drop or by standard Windows file picker, reorder the selected files, and run a merge operation.

The app must support English and Hebrew, including right-to-left layout behavior for Hebrew.

The app must remain local-only and portable by default.

## Task Tracking Requirements

After finishing work, update:

- `progress-tracking/product_manager.md`

And exactly one relevant lifecycle file:

- `progress-tracking/not_started.md`
- `progress-tracking/in_progress.md`
- `progress-tracking/completed.md`
- `progress-tracking/blocked.md`

Rules:

- Every task must appear in exactly one lifecycle file.
- If work starts but is incomplete, move the task to `in_progress.md`.
- If work cannot continue because of a dependency or unresolved decision, move the task to `blocked.md`.
- If work is completed and verified, move it to `completed.md`.
- Keep completion percentages current for active tasks.
- Update documentation whenever behavior, commands, architecture, or standards change.

## Commit Message Requirement

After completing work and updating files, provide a concise commit message.

Use the repository style:

```text
docs(scope): short lowercase summary
feat(scope): short lowercase summary
fix(scope): short lowercase summary
```

Do not perform the commit unless the user explicitly asks.
