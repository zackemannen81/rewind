# Task: Procedural Asset Generator

- **ID:** TOOL-001
- **Title:** Build Procedural Asset Generator for Retro Palette Assets
- **Status:** OPEN
- **Description:** Implement a pipeline tool that ingests structured object descriptions and outputs Unity-ready meshes/prefabs conforming to the Retro Palette art direction and technical constraints.
- **Acceptance Criteria:**
  - Tool accepts a schema covering geometric proportions, functional tags, palette choices, and material hints, and outputs FBX/Prefab pairs with correct pivots and collision proxies.
  - Generated assets apply Retro Palette materials/shaders and respect polygon, scale, and naming conventions defined in `GDD_ArtStyleAssets.md` and `ART-001_production_plan.md`.
  - Includes validation to block unsupported requests (e.g., materials outside palette, missing required metadata).
  - Provides documentation and sample prompts covering apartment props and environmental set dressing.
  - Integrates with version control-friendly storage (e.g., deterministic output folder under `Assets/Art/Procedural/`).
- **Dependencies:** OPS-001, ART-001
- **Estimated Effort:** 8 days

## Notes

- Coordinate with art direction stakeholders to codify material/palette limits prior to implementation.
