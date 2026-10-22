# Game Engine C 1.5 — Unity Integration Audit

Date: 2026-10-22

## Scope

This audit covers the public `main` branch's C++ engine skeleton, managed MMORPG code, content manifests, and Unity package at `Unity/Packages/com.arkansasio.goonzu-game-engine`. This document is a source review only; it does not claim a Unity Editor build or runtime test was run.

## Current architecture

- Root C++ API: `GameEngine.h` / `GameEngine.cpp`
- Managed solution: `GameEngine.sln`; its projects target .NET 10.
- Data model: `Content/GoonzuGame/Data/goonzu_seed_world.json`, `goonzu_2d_asset_manifest.json`, and `goonzu_economy.sql`.
- Unity UPM package: `com.arkansasio.goonzu-game-engine`, version 0.3.0, minimum Unity 2022.3.
- Unity components: isometric-grid conversion, JSON world loading into ScriptableObjects, scene bootstrap markers, and a local economy/crafting helper.

## High-priority findings

1. **C++ backend is a skeleton, not a complete renderer.** `Renderer`, `InputManager`, `PhysicsManager`, and `AudioManager` currently log messages or return placeholder values. Keyboard, mouse, and controller checks always return false. Save/load reports activity but does not serialize game state. `GameEngine::Run()` has no exposed stop path and `StartGame()` enters an effectively unbounded loop.
2. **Managed and Unity runtimes are mixed.** The repository contains console/.NET classes and Unity `MonoBehaviour` types in overlapping gameplay namespaces. `src/GameEngineApp/GameEngineApp.csproj` manually includes many files, including Unity-dependent sources, so a normal .NET build may fail unless Unity references are deliberately supplied or those files are excluded.
3. **Project compilation is fragile.** The solution references two projects; the app project manually lists many files and has a `ProjectReference` to a core project. There are also C++, C# and similarly named menu files in the same source tree. A single clearly authoritative build target should be chosen for each runtime.
4. **Unity bootstrap is prototype-level.** It creates primitive quads and a capsule at index-derived coordinates. The assigned camera is not automatically focused, town service/resource data does not drive gameplay yet, and generated markers are not idempotent if the bootstrap is invoked repeatedly.
5. **JSON validation is thin.** `GoonzuWorldLoader.LoadJson()` assumes `JsonUtility.FromJson` returns a non-null DTO and copies its fields without checking required IDs, duplicate town IDs, tax bounds, or cross-references between region town IDs and town definitions.
6. **Economy helper is client-local.** `GoonzuEconomyService` stores listings in a serialized list. That is useful for prototypes but cannot be treated as an authoritative multiplayer marketplace; purchases, inventory changes, currency and listing ownership must be validated by a server.
7. **Assets are a pipeline specification, not proof of finished art.** The asset manifest and prompt CSV/JSON document target content; repository documentation describes placeholder text assets and an AI generation workflow. Verify actual imported PNGs, spritesheets, animation clips and prefabs in Unity before treating the art library as complete.

## Recommended Unity integration pattern

Keep Unity as the presentation/gameplay runtime and use C# for primary managed systems initially. Treat the standalone C++ engine as a separate native library only when a concrete performance-critical module has a stable C ABI. Do not attempt to embed the C++ console loop into Unity's frame loop.

### Runtime layers

- **Domain/data:** item, recipe, resource, town, region, skill, quest and character definitions; deterministic validation and rules.
- **Application services:** inventory, crafting, market, quest, combat and save APIs. These should not depend on UnityEngine where practical.
- **Unity adapter:** MonoBehaviours, ScriptableObjects, Tilemap/SpriteRenderer, Animator, input, camera, audio and UI.
- **Network boundary:** authenticated request/response contracts; server-authoritative economy, inventory, character persistence, PvP and state transitions.
- **Optional native boundary:** C++ exports with fixed-width types, explicit ownership, versioned ABI and error codes; managed wrappers and tests. Keep Unity-specific classes out of this ABI.

## Ordered work plan

### P0 — Make the package verifiably buildable
1. Open the UPM package in a clean Unity 2022.3 LTS or Unity 6 project and capture compile errors.
2. Add EditMode tests for JSON parsing, malformed JSON, isometric round-trips, empty/invalid towns, tax-rate limits and duplicate IDs.
3. Move Unity-dependent files out of .NET console compile includes; make managed domain assemblies independent of UnityEngine.
4. Add CI for JSON validation and .NET builds that do not require a Unity license; run Unity compilation/tests in an available Unity CI runner.

### P1 — Convert the sample into a playable vertical slice
1. Render grid/tilemap from region and biome definitions, not just town markers.
2. Create data-driven town prefabs with labels and services; position towns from explicit coordinates in JSON.
3. Add player selection/movement using Unity Input System, collision/navigation and camera follow.
4. Add one complete loop: gather iron ore → smelt ingot → craft sword → list item in market → inspect market price.
5. Make world bootstrap safely clear/reuse spawned roots on reload.

### P2 — Robust gameplay and content pipeline
1. Add schema/version migration to world manifests and reject invalid content with actionable diagnostics.
2. Add ScriptableObject definitions for items, professions, recipes, enemies, quests, skills and zone encounters.
3. Add original sprite sheets, tile palettes, animator controllers, UI prefabs, audio events and asset import presets.
4. Add persistence versioning and regression tests.

### P3 — Multiplayer readiness
1. Define an authoritative server API for login/session, characters, inventory, crafting, market, chat and movement.
2. Implement idempotent transaction IDs, currency checks, concurrency controls and auditable trade records.
3. Never trust a Unity client to decide money, item ownership, crafting success or combat results.
4. Add load tests and reconnect/replay tests before calling this an MMO backend.

## Acceptance criteria

- Unity package imports with zero compile errors on the declared supported Unity version.
- Seed JSON imports and validation tests pass.
- Isometric conversion round-trips within one tile for supported coordinates.
- World reload does not duplicate town/player objects.
- One end-to-end gather/craft/market loop works without editor-only dependencies.
- Headless .NET build excludes Unity-only code and compiles in CI.
- Server-side economy tests prove invalid or duplicate purchase requests cannot duplicate items or currency.

## Important distinction

The repository currently contains the pieces of a prototype and a Unity integration layer. Do not describe the C++ renderer, physics, input, save/load, network manager, or local Unity marketplace as production-ready until their real implementations and tests exist.
