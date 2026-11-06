# Desert Level – Unity Game Design Scene

This package creates a separate, playable desert runner scene. It does not overwrite your existing `GAME Scene.unity`.

## Install
1. Open your existing Unity project.
2. Copy the `Assets/DesertLevel` folder from this package into your project's `Assets` folder.
3. Wait for Unity to compile.
4. In Unity, select **Game Design > Build Desert Level**.
5. The builder creates `Assets/DesertLevel/Scenes/DesertLevel.unity`.
6. Open that scene and press Play.

## Controls
- A / Left Arrow: move one lane left
- D / Right Arrow: move one lane right
- Space / Up Arrow: jump
- S / Down Arrow: slide
- R: restart after a collision / finish

## Included
- Three-lane desert trail
- Dunes, sandstone mesas, cacti, rocks and dry palms
- Pyramid landmarks and ruined temples
- Collectible coins and hazards
- Runner capsule placeholder, follow camera and simple HUD
- Separate generated materials and scene

## Notes
- This is a standalone scene prototype intended for laptop Unity Play Mode.
- The original uploaded scene is not modified.
- The generated player is a placeholder capsule. Replace it with your project's player model and adapt the controller if your original game uses a custom movement / GameManager system.
- Built-in Standard materials are used when available. If your project uses URP, Unity may show shader conversion warnings; use Edit > Rendering > Materials > Convert Selected Built-in Materials to URP if needed.
- The scene is generated locally by Unity Editor so Unity creates valid scene object IDs and references.
