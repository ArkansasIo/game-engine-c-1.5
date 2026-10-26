# Ethereal Veni Runtime Foundation

## Status

Phase 1 establishes a dependency-free runtime foundation inside `src/GoonzuGame.Core/EtherealVeni/`.

### Runtime

- `EngineTime` provides frame timing, clamped delta time, fixed-step accumulation, frame counters, and deterministic fixed ticks.
- `EngineContext` provides typed service lookup for engine systems.
- `IEngineSystem` defines initialization, variable update, fixed update, render, and shutdown phases.
- `EngineRuntime` owns lifecycle and executes the frame pipeline.

### ECS

- `EntityId` is a stable runtime identifier.
- `Ecs.World` provides entity creation/destruction and type-indexed component storage.

### Events

- `EventBus` provides typed publish/subscribe messaging without coupling gameplay systems together.

### Input

- `InputAction` defines engine-level actions independent of keyboard, mouse, controller, or platform APIs.
- `InputSnapshot` stores the current action state.

### Persistence

- `JsonSaveStore<T>` provides asynchronous JSON persistence with temporary-file replacement to reduce partial-write risk.

## Integration rule

The legacy `GoonzuGame` classes remain compatible. New systems should depend on `EtherealVeni.*` abstractions rather than directly coupling gameplay to platform or UI implementations.

## Next phases

1. Add platform input adapters and window lifecycle.
2. Add scene/world ownership on top of ECS.
3. Add asset registry and resource handles.
4. Add renderer abstraction and render graph.
5. Add physics and navigation interfaces.
6. Add authoritative server runtime and replication.
7. Add MySQL persistence adapter behind repository interfaces.
8. Migrate RPG/D&D rules into deterministic domain services.
9. Add editor services and Blueprint/Lua scripting bridges.
10. Add automated unit/integration tests and CI build validation.
