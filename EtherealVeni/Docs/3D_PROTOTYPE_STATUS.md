# Ethereal Veni 3D Prototype Status

The Windows prototype now uses an OpenGL 3.3 renderer through OpenTK.WinForms.

## Runtime

- Real GPU-backed 3D viewport
- Perspective camera with orbit controls and zoom
- Procedural terrain mesh
- Player actor
- Enemy actors and chase AI
- Melee attack and damage
- HP and mana simulation
- Runtime HUD
- Inventory, quest, and pause panels

## Editor

The Ethereal Veni Editor embeds the same Pandora 3D viewport so the scene being edited is the same runtime world that PLAY launches.

## Build

The Windows CI restores the Editor and Game projects independently, then builds and publishes both x64 executables.
