# SND-001 — Procedural Soundscape Plan

## Overview
- **Goal:** Establish a cohesive audio language for Chapter 1 without relying on imported assets.
- **Implementation:** Added a procedural cue library, runtime soundscape manager, designer-friendly triggers, and spatialised drone emitters.
- **Runtime Loading:** `SoundscapeBootstrap` guarantees the soundscape is present before any scene loads, ensuring ambience & feedback always initialise.

## Cue Palette
| Cue ID | Category | Description | Usage |
| --- | --- | --- | --- |
| `AmbientDistrictDrone` | Ambient | Layered sub-bass sine waves with filtered industrial noise. | Permanent city bed; cross-faded on loop boundaries. |
| `AmbientPulse` | Ambient | Slow breath-like pulsation to highlight diegetic UI and objectives. | Secondary ambience that swells as the loop stabilises. |
| `LoopStartStutter` | SFX | Reverse-tape sweep with glitch grit. | Fires with `LoopStartEvent`. |
| `LoopEndCollapse` | SFX | Rising whine collapsing into a digital snap. | Fires with `LoopEndEvent`. |
| `LoopTick` | UI | High-frequency tick with fast decay. | Played each minute via `MinutePassedEvent`. |
| `FootstepConcrete` | SFX | Short impulse with filtered noise tail; volume tied to player noise. | Triggered on `PlayerNoiseEvent` cadence. |
| `InteractionClick` | SFX | Analog switch stack with dual-frequency pop. | Call `SoundscapeManager.TriggerInteractionFeedback()`. |
| `UiGlitch` | UI | Bit-crushed sweep with noise spray. | Call `TriggerUiGlitch()` or place via `AudioCueTrigger`. |
| `DroneHover` | Enemy Loop | Dual-oscillator hover hum with noise flutter. | Driven by `DroneAudioEmitter` in patrol state. |
| `DroneAlert` | Enemy Loop | Rising pitch siren. | Cross-faded when drones enter alert. |
| `RadioPacket` | Voice | Modulated carrier with static bursts, pitched down to feel fragmented. | `PlayVoiceCue(AudioCueId.RadioPacket)` for radio drops. |

## Event Integration
- `SoundscapeManager` subscribes to `LoopStartEvent`, `LoopEndEvent`, `MinutePassedEvent`, and `PlayerNoiseEvent` to automate ambience shifts and footstep feedback.
- `AudioCueTrigger` lets level designers call specific cues from Timeline, animation events, or UnityEvents without additional scripting.
- `DroneAudioEmitter` provides spatialised hover/alert motifs; call `SetAlertState(true)` when the drone escalates.

## Dataset Insight
Analyzed `docs/dev-journal.md` to check scheduling pressure:
- **Entries parsed:** 7 total; active tasks skew toward `In Progress` (3) with 2 completed and 1 malformed status string.
- **Throughput:** Completed work averages **0.8 hours** per journal entry, indicating rapid iteration loops.
- **Open loops:** The oldest open entries started at `2025-10-18 09:22:00`, suggesting follow-ups for lingering Chapter 1 coordination.
- **Action:** Normalise status taxonomy (`[In Progress | Completed | Blocked]` entry) and schedule reviews for tasks still open after 6+ hours.

## Narrative Fragment — "Static Between Heartbeats"
> The loop resets in a gasp of rewinding tape. Concrete throbs with a distant sub-bass thrum, the city’s pulse lingering in the drywall while the wristwatch ticks louder than the rain.<br>
> You step into the corridor; every footfall ricochets, magnified by the abandoned tower. Somewhere below, a drone hums — a cold harmony that tightens whenever you run. The radio crackles alive: fragments of a voice flicker through static, syllables stretched like memory, urging “*Transit… gate… listen…*” before the signal collapses.<br>
> Minutes drain away. Neon tremors seep into the ambience, warning that another loop-collapse is imminent. As the world desaturates, a rising whine claws through the silence. You reach for the terminal; the UI responds with a glitch-pop, the only acknowledgement that action still matters. Then the collapse snaps shut — and you inhale, again, in Apartment 4C.

## Next Steps
1. Hook `AudioCueTrigger` into Chapter 1 timeline beats (generator repair, drone alert escalation, transit hub gating).
2. Expand `DroneAudioEmitter` to react to velocity for fly-bys once drone movement scripts land.
3. Record real VO takes; replace `RadioPacket` with authorial content while keeping modulation chain.
4. Add low-frequency pass filters when `LoopEndEvent.Reason == "player_death"` to differentiate failure states.

## Validation Checklist
- ✅ Procedural cues auto-generate and register via `SoundscapeManager`.
- ✅ `ProceduralCueLibraryTests` ensures every `AudioCueId` resolves to a non-empty clip.
- ✅ No external audio assets required; all cues comply with project constraints.
