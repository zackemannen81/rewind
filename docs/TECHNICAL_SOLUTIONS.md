
# RE:WIND Technical Solutions

This document summarizes the proposed technical solutions for RE:WIND's core systems, based on the GDD's technical blueprints.

## Game Engine

**Engine:** Unity
**Version:** 2022.3 LTS (or newer)
**Language:** C#

The project will be developed using Unity, leveraging its C# scripting environment, robust physics engine, and extensive asset store for rapid prototyping and development.

## Core Systems Architecture

The game will be built on a modular, event-driven architecture to ensure loose coupling and testability.

- **Event Bus:** A central event bus will handle communication between all major systems. This avoids direct dependencies and allows for flexible extension.

- **TimeManager:** A singleton class that manages the main loop timer. It will broadcast `LoopStartEvent`, `LoopEndEvent`, and `MinutePassedEvent` on the event bus.

- **KnowledgeManager:** A persistent data object that stores all player knowledge. It will be serialized to JSON at the end of each loop. It will listen for events that grant new knowledge (e.g., `CodeDiscoveredEvent`).

- **AnchorManager:** Manages the set of active causality anchors. It listens for `AnchorAppliedEvent` and modifies the `WorldState` accordingly at the beginning of each loop.

- **WorldState:** A data repository that holds the current state of all persistent objects in the world. It is reset at the beginning of each loop, and then anchors are applied.

- **EchoSystem:**
  - `EchoRecorder`: Listens for player action events and records them with a timestamp.
  - `EchoPlayback`: At the start of a loop, it spawns a ghost actor and replays the recorded actions from the previous loop.

## Puzzle Implementation

Puzzles will be implemented as state machines. Each puzzle will have a `PuzzleManager` that listens for relevant game events and advances the puzzle's state. For example, a code-locked door would listen for a `CodeEnteredEvent` with the correct code.

## AI Implementation

AI will also be implemented using state machines.

- **Patrol State:** The default state, where the AI follows a predefined path.
- **Investigate State:** Triggered by a sound or visual stimulus. The AI moves to the source of the stimulus.
- **Alert State:** Triggered when the player is confirmed. The AI will pursue the player.

## Paradox Handling

A `ParadoxValidator` will run before any major action is committed. It will check the action against the current `WorldState` and `KnowledgeManager` to ensure it doesn't violate causality. If a paradox is detected, it will broadcast a `ParadoxEvent`, which the `TimeManager` will listen for to reset the loop.
