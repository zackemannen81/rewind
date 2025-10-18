# RE:WIND Asset Requests

This document contains detailed specifications for the assets to be created by the asset-generating LLM.

---

## Player Character Model

- **Asset Type:** 3D Model / Character
- **Purpose:** To serve as the main player character in the game. The player will see this character from a third-person perspective.
- **Format:** FBX
- **Dimensions/Resolution:** N/A (Standard humanoid scale for Unity)
- **Color Palette:** Primarily a single, dark, desaturated blue-grey color. No detailed textures, just flat colors.
- **Key Features:**
  - A simple, androgynous, minimalist silhouette.
  - No facial features. The head should be a simple, slightly stylized shape.
  - The body should be slender and geometric, with hard edges. Think low-poly, but with a deliberate, artistic style.
  - The model should be rigged for standard humanoid animations (idle, walk, run, jump, crouch).
- **Technical Constraints:**
  - Polycount: Under 5,000 triangles.
  - Rig: Must be a standard humanoid rig compatible with Unity's Mecanim system.
- **Reference/Inspiration:** The player characters from games like *Inside* or *Another World*. The focus is on a readable silhouette, not detail.
- **Deliverables:** `PlayerCharacter.fbx` file with the rigged model.

---

## Drone Enemy Model

- **Asset Type:** 3D Model / Enemy
- **Purpose:** A patrolling drone enemy for Chapter 1. It should look functional and slightly menacing.
- **Format:** FBX
- **Dimensions/Resolution:** Approx. 1x1x1 meters in Unity.
- **Color Palette:** Dark, industrial grey with a single, glowing cyan/teal emissive "eye" or sensor.
- **Key Features:**
  - A clean, geometric, and functional design. Think of a floating cube or sphere with some simple, articulated panels or antennae.
  - The "eye" should be a distinct, glowing part of the model.
  - The model should have a simple rig allowing for basic animations like hovering, moving, and a detection/alert state (e.g., panels opening up).
- **Technical Constraints:**
  - Polycount: Under 2,000 triangles.
- **Reference/Inspiration:** The drones from *Portal 2* or the security bots from *Inside*. The design should be simple and readable from a distance.
- **Deliverables:** `DroneEnemy.fbx` file with the rigged model.

---

## Wristwatch UI Model

- **Asset Type:** 3D Model / UI
- **Purpose:** The primary diegetic UI element, showing the time remaining in the loop.
- **Format:** FBX
- **Dimensions/Resolution:** Sized to fit the player character's wrist.
- **Color Palette:** Dark grey, matching the player character, with a glowing magenta/pink emissive screen.
- **Key Features:**
  - A simple, chunky, retro-digital design.
  - The screen should be a separate material so its texture can be updated in Unity to display the time.
  - The watch should be a separate object that can be attached to the player character's wrist bone.
- **Technical Constraints:**
  - Polycount: Under 500 triangles.
- **Reference/Inspiration:** The Casio F-91W watch, but with a more brutalist, geometric aesthetic.
- **Deliverables:** `Wristwatch.fbx` file.

---

## Loop Start/End SFX

- **Asset Type:** Audio / SFX
- **Purpose:** To provide clear and impactful audio feedback for the start and end of each time loop.
- **Format:** WAV
- **Dimensions/Resolution:** 44.1kHz, 16-bit, Mono
- **Color Palette:** N/A
- **Key Features:**
  - **Loop Start:** An abrupt, disorienting sound. Should feel like a sharp intake of breath mixed with the sound of a tape rewinding at high speed.
  - **Loop End:** A more drawn-out, tense sound. A low-pass filter should be applied to the game's audio, followed by a rising, whining sound that culminates in a sharp, digital "snap" at the moment of reset.
- **Technical Constraints:** Each file should be under 5 seconds.
- **Reference/Inspiration:** The death sound from *Inside*, the portal sounds from *Portal 2*.
- **Deliverables:**
  - `LoopStart.wav`
  - `LoopEnd.wav`

---

## Ambient Music

- **Asset Type:** Audio / Music
- **Purpose:** To create a melancholic, atmospheric, and slightly tense background track for Chapter 1.
- **Format:** WAV
- **Dimensions/Resolution:** 44.1kHz, 16-bit, Stereo
- **Color Palette:** N/A
- **Key Features:**
  - A dark, minimalist, ambient synthwave style.
  - Should be a looping track, approximately 2-3 minutes in length.
  - The track should be composed of several layers that can be brought in and out to increase or decrease tension (e.g., a simple bass drone, a sparse synth pad, a high-frequency arpeggio).
- **Technical Constraints:** The file should be under 10MB.
- **Reference/Inspiration:** Artists like Lorn, Ben Salisbury & Geoff Barrow. The soundtrack from the movie *Annihilation*.
- **Deliverables:** `Chapter1_Ambient.wav` (and stems for each layer if possible).

---

## Radio Voice Snippet

- **Asset Type:** Audio / Voice
- **Purpose:** A key piece of narrative information delivered via the in-game radio.
- **Format:** WAV
- **Dimensions/Resolution:** 44.1kHz, 16-bit, Mono
- **Color Palette:** N/A
- **Key Features:**
  - The line of dialogue is: "...seven... three... one... two..."
  - The voice should be a calm, male voice, but heavily processed.
  - Apply effects like static, distortion, and a low-pass filter to make it sound like it's coming from a damaged, low-fidelity radio transmitter.
- **Technical Constraints:** The file should be under 5 seconds.
- **Reference/Inspiration:** The radio transmissions from the *Fallout* series or the G-Man's voice from *Half-Life*.
- **Deliverables:** `Radio_Code_7312.wav`

---