# Not Started Tasks

These tasks are planned but do not yet have active implementation tracked in the repository state.

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
