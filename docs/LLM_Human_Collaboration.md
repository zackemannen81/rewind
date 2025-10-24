# LLM + Human Collaboration Best Practices

## Purpose
Establish a repeatable workflow that lets human developers and LLM agents contribute independently while staying aligned on Chapter 1 deliverables.

## Principles
- **Single Source of Truth:** All scope, metrics, and layout references live in shared docs (`docs/Chapter1_Environment_Plan.md`, task files). Avoid ad-hoc notes.
- **Deterministic Generation:** Runtime bootstrap scripts or tools must reproduce a scene exactly. Manual scene edits are prototypes; bake approved positions back into code/assets before committing.
- **Task Ownership:** Every work item must reference an open task ID. Agents and humans signal status updates via `tasks/` and `docs/dev-journal.md` before starting implementation.
- **Artifact Visibility:** Changes to visual elements require paired documentation (screenshots, capture IDs, or coordinate tables) so downstream contributors can validate without reproducing the scene locally.
- **Tooling First:** Prefer extending shared tools (bootstrap, palette scripts, procedural generators) over one-off scene edits. This ensures future agents inherit the same behaviour.

## Workflow
1. **Plan & Discover**
   - Review the relevant sections of `docs/Chapter1_Environment_Plan.md` and task acceptance criteria.
   - Log a journal entry on `main` with start time, intent, and affected files.
2. **Prototype Safely**
   - For geometry/layout, use additive scenes or scratch branches. Capture results with the visual feedback tool (TOOL-002) for others.
   - Record target coordinates/prefabs before leaving Play mode.
3. **Bake & Validate**
   - Translate approved changes into deterministic assets (bootstrap scripts, ScriptableObjects, procedural outputs).
   - Run validation (lint/tests and relevant Unity checks). Note any skipped steps and why.
4. **Document & Handoff**
   - Update the environment plan with diagrams, coordinate tables, and dependency notes.
   - For interactive objects, include animation/state diagrams and hook scripts.
5. **Review & Iterate**
   - Use PR templates to attach captures and evidence of automated checks.
   - Reviewers verify outputs by re-running the bootstrap and comparing to provided captures.

## Communication Cadence
- **Daily sync (async):** Post updates to the dev journal when hitting milestones or blockers.
- **Checklist at Hand-off:**
  - Tasks updated to `REVIEW`/`BLOCKED` as appropriate.
  - Screenshots or capture IDs archived under `Artifacts/` or as PR attachments.
  - Tooling changes accompanied by usage docs under `docs/tools/`.

## Escalation
- Blocking issues affecting shared scripts/assets must be flagged in `docs/dev-journal.md` and pinned in the planning doc’s **Risks** section.
- When a change invalidates existing captures or plans, increment the relevant version tag (see environment plan) and notify #chapter1 channel.

## Version Tags
- Use semantic tags for shared artifacts (e.g., `APT4C-v2`, `COURT-v1`). Update the environment plan each time a tag increments.

Adhering to these practices keeps the multi-agent pipeline predictable and auditable.
