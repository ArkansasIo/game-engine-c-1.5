# Ethereal Veni Runtime Architecture

## Layers

```text
Application
  -> EngineRuntime
     -> Engine Systems
        -> World / ECS / Input / Events / Assets
           -> Gameplay / RPG / Networking
              -> Persistence adapters
```

## Runtime rules

1. The runtime owns the frame lifecycle.
2. Gameplay systems do not directly depend on database providers.
3. World state is represented independently from rendering.
4. Network identities are separate from ECS entity identities.
5. Stable asset IDs and save identifiers must not be rewritten during refactors.
6. MySQL is an authoritative persistence adapter, not a dependency of the simulation core.
7. JSON persistence remains available for local development and offline tooling.

## Major modules

- `Runtime`: timing and lifecycle.
- `Ecs`: entities and component storage.
- `World`: scenes, transforms, and world state.
- `Events`: typed event dispatch.
- `Input`: platform-neutral action snapshots.
- `Assets`: stable asset identity and registry.
- `Networking`: transport contracts and replication IDs.
- `Persistence`: repository and database contracts.
- `Rpg`: stats, combat, inventory, equipment, skills, and quests.
- `Configuration`: runtime and database settings.
- `Diagnostics`: frame/runtime metrics.

## Migration strategy

Existing Goonzu-oriented classes remain compatible while new Ethereal Veni systems are introduced. New code should depend on the Ethereal Veni contracts first. Legacy classes should migrate behind adapters rather than forcing a repository-wide rewrite.

## Server boundary

The future authoritative server should host:

- authentication/session management
- character state
- inventory and equipment
- quest progression
- combat authority
- world simulation
- replication
- persistence

Clients should submit intents and receive authoritative state changes rather than directly modifying persistent state.
