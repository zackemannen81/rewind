# Repository Guidelines

## Commit & Pull Request Guidelines
- Run `git config core.hooksPath .githooks`; the pre-commit hook blocks `main` commits and requires a `Task ID:` line.
- Branch names follow `feature/<summary>` or `fix/<summary>`; align them with the relevant file in `tasks/`.
- Write imperative commit titles plus a Task ID (`feat: add bomb meter HUD` + `Task ID: UI-004`).
- PRs need a clear scope, test evidence, linked task/doc updates, and screenshots for UI shifts.

## Work Coordination
- Log progress in `docs/dev-journal.md` from the coordination worktree before pushing feature code.
- Fetch in both worktrees regularly so task status stays current.
- When touching shared assets or configs, flag downstream impacts in the journal and PR notes.
