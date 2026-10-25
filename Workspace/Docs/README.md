# [CORE] Workspace Documentation

This folder documents the Ethereal Veni workspace model.

## Documents

- `WorkspaceArchitecture.md` — workspace state, ownership, and persistence boundaries.
- `WorkspaceLayouts.md` — standard editor layouts and panel definitions.
- `WorkspaceConfiguration.md` — input, graphics, video, audio, accessibility, and general settings.

## Persistence policy

Portable project settings and workspace templates belong in Git. User-specific cache, recent-file history, window geometry, local device identifiers, credentials, and generated profiling data do not belong in Git.
