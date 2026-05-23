# In Progress Tasks

Tasks in this file have meaningful implementation already, but they are not finished or have a material gap that prevents closure.

## PM-07 English And Hebrew Localization

Completed:

- Added English and Hebrew JSON locale files.
- Added JSON localization service with English fallback behavior.
- Applied right-to-left flow direction when Hebrew is selected in settings.
- Localized current foundation UI strings.
- Added localized PM-05 file selection, validation, duplicate, reorder, and remove messages.

Still missing:

- Language selection UI is not implemented yet.
- Future PM-06 user-facing workflow strings must be added as that task begins.

Why this stays open:

- Full first-version localization cannot close until feature workflow strings and language selection are implemented.

Completion:

- 60%

## PM-09 Logging And Error Reporting

Completed:

- Added structured local file logger based on `Microsoft.Extensions.Logging`.
- Added portable log directory under `data/logs`.
- Added app lifecycle logging.
- Added status and dismissible transient message service.

Still missing:

- Merge failure logging cannot be completed until PM-06 implements the merge workflow.

Why this stays open:

- Logging foundation exists, but acceptance criteria tied to merge failures remain future work.

Completion:

- 60%

## Entry Template

Use this structure when a task moves here:

## PM-XX Task Name

Completed:

- completed implementation that already exists

Still missing:

- remaining functional gaps

Why this stays open:

- why the current state is not enough for completion

Completion:

- 0%
