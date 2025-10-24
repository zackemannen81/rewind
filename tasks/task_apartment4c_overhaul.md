# Task: Apartment 4C Environment Rebuild

- **ID:** ENV-APT4C-001
- **Title:** Rebuild Apartment 4C to Production Quality
- **Status:** OPEN
- **Description:** Replace the Apartment 4C blockout with a production-ready environment using Retro Palette assets, including walls, fixtures, furniture, props, lighting, and scripted interactables aligned with the golden path.
- **Acceptance Criteria:**
  - Apartment scene contains only required floor from bootstrap before rebuild; all walls, doorways, and windows are reauthored with reusable modules and correct metrics from the new plan.
  - Built-in objects (kitchen modules, storage, bathroom fixtures) instantiated via reusable prefabs and registered with Retro Palette materials.
  - Furniture and hero props positioned per the approved layout; no placeholder cubes remain.
  - All interactables (radio console, fuse box, TV, computer, keypad, etc.) function with animation/scripting hooks and collider volumes validated through play mode.
  - Interior lighting and emissive accents leverage Retro Palette Pipeline scripts; materials audited for compliance.
  - Window vistas display a neon city backdrop with ambient lighting loops.
  - Layout documented in `docs/Chapter1_Environment_Plan.md` with coordinates and prefab references.
- **Dependencies:** OPS-001, TOOL-001, TOOL-002
- **Estimated Effort:** 10 days

## Notes

- Coordinate with narrative/system teams to ensure interactable offsets remain compatible with Chapter 1 golden path scripting.
