# Task: Courtyard Environment Rebuild

- **ID:** ENV-COURT-001
- **Title:** Rebuild Courtyard to Production Quality
- **Status:** OPEN
- **Description:** Replace the courtyard blockout with a production-ready environment featuring structured walls, fence + electronic gate, hero props, FX, and lighting consistent with the Retro Palette art style.
- **Acceptance Criteria:**
  - Existing courtyard geometry cleared except for floor module; new perimeter buildings, fence, and gate authored with reusable prefabs.
  - Golden path interactables (generator, gate controls, echo anchor, etc.) re-positioned and scripted with final animations and audio.
  - Ambient props (benches, foliage, signage, cover objects) placed to support stealth traversal and narrative beats.
  - Dynamic FX (flickering streetlamps, volumetric fog, sparks, distant city VFX) implemented using production-ready assets.
  - Skybox/backdrop updated to a neon city vista with parallax/lighting sync.
  - Lighting pass ensures performance and readability; Retro Palette shaders applied across all visible surfaces.
  - Updates documented with impact notes in `docs/Chapter1_Environment_Plan.md`.
- **Dependencies:** OPS-001, ENV-HALL-001, TOOL-001
- **Estimated Effort:** 9 days

## Notes

- Coordinate with AI/pathing owners to verify updated cover and traversal volumes.
