# Retro Palette Pipeline

This folder contains the assets introduced for Task ART-001 to standardise the neon-noir art direction.

## Palette
- `RetroPalette.cs`: ScriptableObject defines the baseline colour swatches (primary, accent, tertiary, loop states).
- `RetroPalette_Default.asset`: Default palette instance referenced by materials, lighting profiles, and post-processing presets.

## Shader & Materials
- `Shaders/RetroPalette.shader`: Surface shader with accent emission and loop-state keyword overrides (`RETRO_ALERT`, `RETRO_LOOPEND`).
- `Materials/Mat_*`: Authorised material presets mapped to palette swatches. Apply these to meshes to keep a consistent colour story.

## Runtime Controller
- `RetroPaletteStateController.cs`: Optional component that pushes palette values into bound materials and flips shader keywords as the loop progresses. By default it:
  - Applies palette colours to each configured material binding.
  - Listens for `LoopStartEvent`, `MinutePassedEvent`, and `LoopEndEvent` to drive alert/loop-end looks.
  - Enables alert mode when the loop timer hits the final minute (via `MinutePassedEvent`).

### Usage Steps
1. Drop `RetroPaletteStateController` onto a scene GameObject (e.g. `ArtPaletteController`).
2. Assign the `RetroPalette_Default` asset to the controller.
3. Add the material bindings you want updated at runtime (each binding selects base/accent swatches and tuning parameters).
4. Ensure level logic publishes `MinutePassedEvent` and `LoopEndEvent` via the existing `EventBus` (already handled by `TimeManager`).

### Creating New Materials
- Duplicate an existing `Mat_*` preset and change the `_BaseColor` or `_AccentColor` using palette getters.
- Keep `_AccentMask` white for full-surface emission or supply a mask texture to localise neon accents.

### Loop State Keywords
- `RETRO_ALERT` intensifies the base colour toward a crimson tint for late-loop tension.
- `RETRO_LOOPEND` desaturates materials for the final rewind flash.
- Toggle these keywords through gameplay by invoking `RetroPaletteStateController.SetLoopState` helpers (automatically handled for basic loop flow).

