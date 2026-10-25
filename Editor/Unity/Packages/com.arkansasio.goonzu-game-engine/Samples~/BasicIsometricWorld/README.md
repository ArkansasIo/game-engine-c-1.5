# Basic Isometric World

This sample demonstrates the Unity side of Game Engine C 1.5.

## Setup

1. Install the package through Unity Package Manager as a local package pointing at `Unity/Packages/com.arkansasio.goonzu-game-engine`.
2. Copy or reference `Content/GoonzuGame/Data/goonzu_seed_world.json` from the repository as a Unity `TextAsset`.
3. Create an empty GameObject named `GoonzuWorld`.
4. Add `GoonzuIsometricGrid` and `GoonzuUnityBootstrap`.
5. Assign the JSON file to `Seed World Json`.
6. Add an orthographic camera and assign it to `World Camera`.
7. Press Play. The bootstrap creates Unity scene markers for the seed towns and a starter player placeholder.

The placeholder geometry is intentionally generic. Replace it with original SpriteRenderer, Tilemap, Animator, VFX, UI, and prefab assets as the art pipeline is completed.
