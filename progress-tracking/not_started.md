# Not Started Tasks

These tasks are planned but do not yet have active implementation tracked in the repository state.

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
