# ART-001 Production Plan — Establish Core Art Style

## Objective
Build the Chapter 1 vertical-slice art foundation that matches the retro-futuristic, neon-noir vision in `GDD_ArtStyleAssets.md`. Deliver stylized hero assets, palette/shader infrastructure, and post-processing/lighting presets that the level team can drop into `Chapter1_Blockout` without rework.

## Deliverables
- **Character Set:** Low-poly, silhouette-driven meshes for the Player, Guard, and Drone with clean pivots and FBX export. Include placeholder animation hooks (idle root motion transform + emissive markers on the drone).
- **Hero Prop Kit:** Generator, wristwatch UI, terminal, and echo anchor props sized for the existing blockout metrics. Provide OBJ + material assignments, with optional texture masks for emission.
- **Environment Module:** Brutalist wall/floor/door kitbash pieces and 3 marquee props (courtyard bench, neon signage, transit turnstile) to replace high-poly placeholders.
- **Palette System:** Scriptable palette asset with defined hex values, `RetroPalette.shader` (built-in render pipeline) supporting base/albedo tint, accent emission, and glitch rim.
- **Lighting Templates:** Volume profile asset encoding fog, bloom, chromatic aberration, color grading curves for the neon-noir look; reusable directional/area light prefabs tagged for Chapter 1 scenes.
- **Post-Processing Stack:** Global volume prefab referencing the palette accent colors, depth-of-field, bloom, film grain, and vignette tuned to the 7-minute loop pacing (normal, alert, loop-end overrides).
- **Documentation:** Drop-in instructions for art integration, asset naming conventions, and palette usage guidelines.
- **Editor Automation:** `Art ▸ Generate ▸ Build Core Art Baseline` menu builds hero prefabs and lighting profiles from palette definitions to accelerate blockouts.

## Working Approach
1. **Audit & Cleanup (Day 1):** Catalogue existing placeholder meshes in `Assets/WorkInProgressOrPlaceHolders`. Identify reusable items vs. replacements. Normalize scale references (1 unit = 1 meter).
2. **Palette & Shader (Day 1-2):** Implement `RetroPalette.asset` (ScriptableObject) containing base/secondary/accent colors. Author `RetroPalette.shader` + materials (`Mat_PrimaryConcrete`, `Mat_AccentNeon`, `Mat_TertiaryOxide`). Validate via Unity Scene view.
3. **Character Blockouts (Day 2-3):** Model Player and Guard silhouettes (12–24 tris per limb) in Blender, export to FBX. Drone uses octagonal body + twin rotor discs. Assign palette materials and emissive accent.
4. **Prop Kit (Day 3-5):** Generator, wristwatch, terminal, neon signage, and notice board. Focus on strong silhouettes and pivot placement. Include simple normal map placeholders if needed.
5. **Environment Module (Day 4-6):** Modular walls (4m x 4m), floors, door frames, stair segment. Provide LOD0/LOD1 where polygon count warrants.
6. **Post Stack & Lighting (Day 6-7):** Create `Art/Lighting/Chapter1_GlobalVolume.prefab` with bloom (intensity 12), chromatic aberration (0.2), color adjustments (lift/gamma/gain to palette accent). Author `Art/Lighting/LoopEndOverride.asset` for desaturation/glitch.
7. **Integration Pass (Day 8-9):** Replace placeholders in `Chapter1_Blockout` scene, validate readability under stealth lighting, adjust palette values as needed.
8. **Final Polish & Hand-off (Day 10):** Document pipelines in this file and `docs/dependencies`. Update `dev-journal` with testing notes.

## Asset Naming & Folder Structure
- `Assets/Art/Characters/{Player,Guard,Drone}/` for FBX + materials.
- `Assets/Art/Props/{Generator,Wristwatch,Terminal,...}/` with OBJ source + prefabs (prefab suffix `_PFB`).
- `Assets/Art/Environment/BrutalistKit/` for modular pieces.
- `Assets/Art/Shaders/RetroPalette.shader`
- `Assets/Art/Materials/Mat_*.mat`
- `Assets/Art/Lighting/{Volumes,Presets}/`
- `Assets/Art/Documentation/Readme.md`

## Technical Notes
- Target poly density: <2k tris per hero asset, <500 tris per prop module.
- Texture usage: primarily flat color (albedo) with emission masks; rely on lighting for variation.
- Shader supports palette override via global keywords (`_RETRO_ALERT`, `_RETRO_LOOPEND`) toggled by gameplay scripts.
- Post-processing uses Unity Post Processing stack v3 (add `com.unity.postprocessing` to manifest). No third-party dependencies.

## Risks & Mitigations
- **Package Footprint:** Adding Post Processing & potential URP needs QA; mitigate by isolating new assets under `Assets/Art` and verifying standalone scene loads.
- **Timeline Overlap:** Coordinate with Chapter 1 mission scripting to avoid merge conflicts when replacing props. Stage prefabs in additive scene `Chapter1_ArtDrop` first.
- **Performance:** High bloom/emission may blow out in low-end PCs. Provide low-spec profile variant without volumetric fog.

## Next Steps
1. Create folder skeleton under `Assets/Art/` and stub palette/shader assets.
2. Implement `RetroPalette` ScriptableObject & shader.
3. Produce first-pass player/guard/drone blockout meshes and wire them into prefabs for iteration.

## Scope Update — 2025-10-22
- Brutalist environment kit will be split into a follow-up art task once Chapter 1 layout locking is complete; current ART-001 scope focuses on character, shader, and lighting systems.
- Scheduled Chapter 1 art integration window for next layout sync (targeting 2025-10-24) to swap player prefab, hero props, and palette-driven volumes into `Chapter1_Blockout`.
- `PlayerCharacterBuilder` + `PlayerProceduralAnimator` replace the previous `Player_LowPoly` placeholder to align with the neon-noir silhouette style and animation requirements.
- `ThirdPersonCamera` rig automation included in the builder so drop-in prefabs arrive with tuned orbit camera + audio listener.

