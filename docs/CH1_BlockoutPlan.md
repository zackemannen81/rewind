# Chapter 1 Blockout & Systems Plan (CH1-001)

## Goals
- Establish a playable Chapter 1 scene that hits the golden-path flow described in `docs/GDD_About.md#L225`.
- Integrate placeholder geometry for Apartment 4C, courtyard, and street with traversal volumes.
- Script the core golden-path interactions: radio code discovery, fuse choice, generator mini-puzzle, gate anchor, transit hub timing.

## Scene Structure
- `Assets/Scenes/Chapter1/Chapter1_Blockout.unity`
  - **Root**: `Chapter1Root`
    - `Environment`
      - `Apartment4C`
      - `Courtyard`
      - `Street`
      - `TransitHubD4`
    - `Interactables`
      - `Radio_Channels`
      - `FuseBox`
      - `Generator`
      - `CourtyardGate`
      - `TransitTurnstile`
    - `AI`
      - `Guard_Path`
      - `Drone_Path`
    - `Systems`
      - `LoopEntryPoints`
      - `AnchorTriggers`
      - `KnowledgeMarkers`

## Implementation Phases
1. **Geometry Blockout**
   - Create primitive-based layouts for Apartment 4C, courtyard, street, and transit hub.
   - Apply distinct layer masks for traversal probes (`Traversal`, `Ground`, `Obstacles`).
2. **Interaction Stubs**
   - Add placeholder scripts for FuseBox, Generator, CourtyardGate, TransitTurnstile.
   - Publish events for knowledge updates and anchors.
3. **Radio & Knowledge Flow**
   - Implement radio channel cycling, incremental clarity per loop, and deliver code `7312` to `KnowledgeManager`.
4. **AI & Timing**
   - Stub guard/drone patrol paths using waypoints; integrate `PlayerNoiseEvent` hooks.
   - Implement transit hub “breathing” gate timing with audio/visual cues.
5. **Anchoring & Loop Integration**
   - Register courtyard gate as an anchor candidate and ensure persistence over loops.

## Dependencies & Follow-ups
- Requires finalized input mappings for interact key and channel cycling.
- Needs placeholder audio hook for ambient loop (`Assets/WorkInProgressOrPlaceHolders/Chapter1_Ambient.wav`).
- QA task: build unity test scene to ensure loop resets `TimeManager` properly when chapter loads.

## Risks
- No environment art yet; keep modular so art team can swap meshes later.
- Anchor system currently only stores IDs—ensure we standardize keys for gate unlock state.
- Player traversal relies on accurate collision layers; blockout must respect controller radius (0.5m) and vault/climb thresholds.

