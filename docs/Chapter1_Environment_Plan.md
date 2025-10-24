# Chapter 1 Environment & Tooling Plan

_Last updated: 2025-10-24 (OPS-001)_

## Overview
This plan aligns the tooling and environment work required to deliver a production-ready Chapter 1 experience. It is organised into three workstreams:

1. **Tooling Upgrades** – Procedural asset generation (TOOL-001) and visual feedback interfaces (TOOL-002).
2. **Apartment Entry Sequence** – Apartment 4C, hallway + elevator transition.
3. **District 4 Exterior** – Courtyard, streetfront, and Transit Hub D4.

Each section lists goals, deliverables, owners, and dependencies so agents and human devs can execute independently while staying synchronised.

## 1. Tooling Upgrades
### 1.1 Procedural Asset Generator (TOOL-001)
- **Goal:** Translate high-level object briefs into Unity-ready prefabs that honour Retro Palette constraints.
- **Inputs:** Structured JSON/YAML description (`type`, `dimensions`, `paletteSlot`, `functionalTags`).
- **Outputs:**
  - FBX mesh + prefab with Retro Palette material assignments.
  - Optional collider prefab matching traversal metrics.
  - Metadata JSON for audit (polycount, materials, bounding box).
- **Milestones:**
  1. Schema draft & validation (w/ art director).
  2. Prototype generator supporting common furniture (bench, sofa, desk).
  3. Integration with Unity import pipeline + automated tests.
  4. Documentation & sample library (`docs/tools/procedural-generator.md`).
- **Risks:** Shader/material drift; mitigate via palette validation pass.

### 1.2 Visual Feedback Interface (TOOL-002)
- **Goal:** Provide agents with rapid visual context for scene iterations.
- **Implementation Options:**
  - Unity Editor utility window that queues camera shots (preferred).
  - MCP endpoint to command captures and return image bytes.
  - Fallback: serialized play-mode captures stored under `Artifacts/Captures/`.
- **Milestones:**
  1. Choose capture mechanism + permission model.
  2. Expose CLI tooling (e.g., `pnpm capture --scene Chapter1_Blockout --view apt_entry`).
  3. Tie into PR template (automatic capture diff when environment changes).
- **Risks:** Disk bloat; schedule clean-up job.

## 2. Apartment Entry Sequence
### 2.1 Apartment 4C Rebuild (ENV-APT4C-001)
- **Layout Metrics:**
  - Footprint: 7.4 m × 7.4 m, ceiling 3.0 m.
  - Entry door on north wall centered; bathroom SW corner; bedroom SE alcove.
- **Key Deliverables:**
  - Modular wall prefabs with door/window cut-outs.
  - Built-ins: kitchen run, bathroom fixtures, storage.
  - Furniture set: desk, sofa, lounge chair, bed, lounge ottoman, shelving, hero props.
  - Interactables: radio console, fuse box, TV + plant, keypad, diegetic screens.
  - Lighting: layered point/area lights, emissive accents, external city glow.
  - Backdrop: parallax neon skyline outside windows.
- **Implementation Notes:**
  - Bootstrap (`Chapter1Bootstrap.cs`) remains canonical; scene edits must be backported.
  - Prefabs stored under `Assets/Art/Environment/Apartment4C/` with `_PFB` suffix.
  - Capture setpoints using visual feedback tool (`apt4c_main`, `apt4c_bedroom`, `apt4c_kitchen`).

### 2.2 Transition Hallway & Elevator (ENV-HALL-001)
- **Scope:** Corridor outside 4C, call panel, elevator ride to courtyard level.
- **Deliverables:**
  - Modular hallway segments with grime decals and signage.
  - Elevator interior/exterior, animated doors, call button interactable.
  - Audio cues for arrival/departure, vibration FX.
  - Scene hooks: sets next spawn, triggers loading of courtyard additively.

## 3. District 4 Exterior
### 3.1 Courtyard Rebuild (ENV-COURT-001)
- **Layout:** Courtyard 12 m × 14 m; entry from hallway; exit via electronic gate.
- **Highlights:**
  - Multi-tier lighting (streetlamps, signage, ambient fog).
  - Echo anchor pedestal, generator hero prop, control console.
  - Fence + gate with animated lock state and holographic signage.
  - Cover objects matched to stealth beats.

### 3.2 Streetfront (ENV-STREET-001)
- **Focus:** Connects courtyard to transit hub, introduces patrol routes and signage.
- **Elements:** Building facades, kiosks, neon signage, ambient traffic light loops.

### 3.3 Transit Hub D4 (ENV-TRANSIT-001)
- **Focus:** End-of-chapter staging area with timing puzzle.
- **Elements:** Turnstiles, platform lighting, countdown VO, city vista.

## Timeline (Indicative)
| Week | Focus | Tasks |
|------|-------|-------|
| 1 | Tooling foundations | OPS-001, TOOL-001 (schema), TOOL-002 (capture prototype) |
| 2 | Apartment rebuild (geometry, built-ins) | ENV-APT4C-001 |
| 3 | Apartment dressing & interactables | ENV-APT4C-001, TOOL-001 integration |
| 4 | Hallway/elevator build | ENV-HALL-001 |
| 5 | Courtyard rebuild | ENV-COURT-001 |
| 6 | Streetfront polish | ENV-STREET-001 |
| 7 | Transit hub rebuild | ENV-TRANSIT-001 |
| 8 | Integration pass & testing | All tasks, TOOL-002 automation |

## Dependencies Matrix
| Task | Depends On |
|------|------------|
| TOOL-001 | OPS-001, ART-001 |
| TOOL-002 | OPS-001 |
| ENV-APT4C-001 | OPS-001, TOOL-001, TOOL-002 |
| ENV-HALL-001 | OPS-001, ENV-APT4C-001 |
| ENV-COURT-001 | OPS-001, ENV-HALL-001, TOOL-001 |
| ENV-STREET-001 | OPS-001, ENV-COURT-001 |
| ENV-TRANSIT-001 | OPS-001, ENV-STREET-001 |

## Quality Gates
- **Tooling:** Unit/integration tests for generator output; capture diff automation.
- **Environment:** Play-mode traversal checklist, lighting bake sanity, Retro Palette material audit.
- **Documentation:** Update this plan after every milestone; version tags appended to section headers (e.g., `## 3.1 Courtyard Rebuild (v1)`).

## Risks & Mitigations
- **Tool complexity:** Start with limited archetypes; expand library iteratively.
- **Scene drift:** Enforce bootstrap as single point of truth; run regen before committing.
- **Performance:** Monitor draw calls after each environment milestone; budget table to be added.

## Contacts
- **Art Direction:** ART-001 owners.
- **Systems/Gameplay:** CH1-001 owners.
- **Tooling:** TOOL-001/002 assignees.

Keep this document updated as tasks move through IN_PROGRESS → REVIEW → DONE.
