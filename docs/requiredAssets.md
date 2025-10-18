# Chapter 1 Required Assets (Task CH1-001)

This list captures the art/audio deliverables needed to upgrade the current Chapter 1 blockout into a presentable vertical slice. Items are ordered by priority.

## Environment & Props
- Apartment 4C interior kit: modular walls, door frame, fuse box panel, radio console, scattered props (bed, table, chair, personal items).
- Courtyard set: perimeter walls, gate mesh with animation hooks, generator housing, scaffolding, echo pool surface.
- Street frontage: building facades, environmental signage ("RE:WIND Containment Notice"), street-level props (kiosk, crates, lights).
- Transit hub entry: turnstile structure with moving gates, platform signage, industrial lighting fixtures.
- Connector pieces: walkways, ramps, stairwells linking apartment → courtyard → street → hub (match traversal metrics from player controller).

## Characters & Animations
- Player placeholder mesh rigged for third-person controller (lean, crouch, climb, vault compatible).
- Security guard character with patrol animation set (idle, walk, turn-in-place, alerted).
- Drone enemy with patrol hover cycle, alert animation, light emissive cues.

## VFX & Lighting
- Loop start/end effects (retro rewind flash, desaturation pulse).
- Generator activation spark/lighting cue.
- Gate unlock indicator (light strip, particles).
- Transit turnstile “breathing” lights synced to window timing.

## Audio
- Apartment ambient loop (melancholic, low-poly interior).
- Courtyard ambient loop with distant machinery, faint drones.
- Street ambience: neon buzz, distant announcements.
- Generator activation SFX, gate unlatch, turnstile locked/open cues.
- Radio static + voice fragments for code reveal (binaural processing for diegetic feel).

## UI & Diegetic Elements
- Radio display shader or mesh showing channel cycling.
- Turnstile indicator lights synced to open window.
- Courtyard gate indicator (Insight anchor feedback).
- Apartment diegetic notes (photograph frame, whiteboard assets).

## Integration Notes
- Provide meshes with reasonable pivot placement; include collision proxies where appropriate.
- Supply FBX files with normals/tangents; include LODs for outdoor structures if possible.
- Bundle textures in URP-compatible materials (albedo, normal, emission passes).
- Audio assets: WAV 48kHz mono/stereo, loopable where noted.
- Annotate any moving parts (gate, turnstile arms) in the FBX hierarchy for easy animation rigging.

