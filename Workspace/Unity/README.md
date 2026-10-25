# Unity Integration — Game Engine C 1.5

Game Engine C 1.5 now has a Unity Package Manager-compatible integration layer. The existing C++, C#, Shell, data, and asset files remain intact; Unity is an additional presentation/runtime target.

## Architecture

```text
Game Engine C 1.5
├── C++ core / economy / simulation
├── C# gameplay data and runtime helpers
├── Content/GoonzuGame/Data
│   └── JSON world, economy, and asset manifests
└── Unity/Packages/com.arkansasio.goonzu-game-engine
    ├── Runtime
    │   ├── Isometric grid conversion
    │   ├── World JSON -> ScriptableObject loader
    │   ├── World bootstrap / GameObject bridge
    │   └── Economy + crafting service adapter
    └── Editor
        └── JSON -> GoonzuWorldDefinition asset importer
```

## Install in Unity

Open **Window → Package Manager → + → Add package from disk...** and select:

`Unity/Packages/com.arkansasio.goonzu-game-engine/package.json`

Or add the package as a local package in a Unity project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.arkansasio.goonzu-game-engine": "file:../game-engine-c-1.5/Unity/Packages/com.arkansasio.goonzu-game-engine"
  }
}
```

Use Unity 2022.3 LTS or newer. Unity 6 is also supported by the package design.

## First playable test

1. Add an empty GameObject called `GoonzuWorld`.
2. Add `GoonzuIsometricGrid`.
3. Add `GoonzuUnityBootstrap`.
4. Drag `Content/GoonzuGame/Data/goonzu_seed_world.json` into the `Seed World Json` field after the repository is inside or linked into the Unity project.
5. Assign `Camera.main` to `World Camera`, or leave it empty and let the bootstrap find the main camera.
6. Press Play.

The bootstrap creates placeholder town markers and a starter player object. These are deliberately generic so the repository does not copy proprietary game art.

## Asset pipeline

The intended 2D workflow is:

`Original Blender/2D art → orthographic/isometric render → PNG/Sprite Sheet → Unity Sprite/Tilemap/Animator → GoonzuWorldDefinition + data manifests`

The Unity layer is additive. It does **not** replace or modify `GameEngine.cpp`, `GameEngine.h`, `TextureManager.cs`, or the existing C# / C++ engine contracts.

## Next integration step

For a production build, the Unity presentation layer can be connected to a compiled native C++ library through Unity's native plug-in ABI (`DllImport`) or kept fully managed for WebGL and portable builds. Server-authoritative economy and persistence should remain outside client-only Unity objects.
