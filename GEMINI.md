TOOL‑001 (Procedural Asset Generator) Session Prompt

  You are working on TASK TOOL-001: “Build Procedural Asset Generator for Retro
  Palette Assets” in the RE:WIND repo.

  1. Read before coding:
     • AGENTS.md
     • docs/LLM_Human_Collaboration.md (workflow rules)
     • docs/Chapter1_Environment_Plan.md (section 1.1 and timeline)
     • docs/ART-001_production_plan.md, docs/GDD_ArtStyleAssets.md, docs/
  requiredAssets.md, docs/ASSETS.md, docs/ASSET_REQUESTS.md
     • tasks/task_ops_planning.md, tasks/task_tool_asset_generator.md
     • Latest entries in docs/dev-journal.md concerning OPS-001 / TOOL-001.

  2. Workflow:
     • Administrative updates happen in the rewind-main worktree on branch main;
  configure hooks (`git config core.hooksPath .githooks`), update dev-journal +
  task status, commit & push before leaving main.
     • Implementation occurs in the project worktree on a feature branch named
  `feat/tool-001-asset-generator` (or similar). Publish the branch immediately.
     • Keep output deterministic—no manual scene edits; all deliverables must
  come from scripts/tooling under version control.
     • Reference Retro Palette Pipeline scripts/materials whenever producing
  prefab templates.

  3. Task expectations (from tasks/task_tool_asset_generator.md):
     • Define input schema (JSON/YAML) for object briefs (dimensions, palette
  slot, functional tags).
     • Produce Unity-ready outputs: FBX/prefab with correct pivots, collider
  proxies, Retro Palette materials.
     • Implement validation that blocks unsupported requests (palette
  violations, missing metadata).
     • Create deterministic output structure (e.g., `Assets/Art/
  Procedural/...`), plus editor/runtime import hooks if needed.
     • Add documentation & samples (e.g., `docs/tools/procedural-generator.md`),
  including how to invoke from CLI or Unity menu.
     • Add automated checks (unit/integration) where possible.

  4. While coding:
     • Log intermediate notes in docs/dev-journal.md (main worktree) when
  hitting milestones or blockers.
     • Update docs/Chapter1_Environment_Plan.md (Tooling section) with version
  tags or status changes.
     • Use Retro Palette materials defined in ART-001; confirm compliance via
  script validation.
     • Capture CLI usage examples and expected outputs for other agents.

  5. Finishing:
     • Run validations/tests (note if any unavailable and why).
     • Commit changes with Task ID reference.
     • Prepare PR including schema docs, example outputs, and validation
  evidence.
     • On main worktree, set TOOL-001 task to REVIEW, add closing journal entry,
  commit & push.

  Escalate blockers in the dev journal and environment plan risk section.