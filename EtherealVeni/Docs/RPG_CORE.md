# Ethereal Veni RPG Core

This document describes the foundational, engine-independent RPG/MMORPG gameplay layer under `Engine/Gameplay`.

## Systems

- **Attributes** — ten core attributes are already represented by `AttributeSet`.
- **Combat** — deterministic request/result pipeline, pluggable random source, validation, armor mitigation, block reduction, and critical hits.
- **Inventory** — item definitions, rarity/category enums, stack limits, slot/weight capacities, atomic add semantics, and removal/count operations.
- **Quests** — data-driven definitions, validated objectives, progress reporting, and explicit available/active/completed/failed states.
- **Progression** — level XP curve and ability score modifier helper.

## Combat authority

The server should own combat decisions. Clients should submit intent (for example, an attack request) rather than authoritative damage values. Validate entity ownership, cooldowns, range, line of sight, abilities and state on the server before calling `CombatSystem.ApplyDamage`. Persist a combat outcome only after the server accepts it.

The armor formula is intentionally simple and configurable:

`damageAfterArmor = rawDamage * 100 / (100 + armor)`

Block then reduces the remaining damage by `BlockFraction`. Tune these constants through game-balance data rather than treating them as universal rules.

## Inventory consistency

Inventory operations return a result instead of throwing for expected gameplay failures. Item IDs are stable, ordinal identifiers. Register definitions before use. Network commands should execute through a serialized per-character inventory transaction and persist only successful changes.

## Quest lifecycle

A quest definition requires a unique ID, title and at least one objective. Progress only advances for matching objective kinds and target IDs while the quest is active. Objective counts clamp to their configured requirement; the quest completes when every objective is satisfied.

## Production readiness still required

These primitives are not a complete production MMO. Before public deployment, add authenticated identities, authorization, persistence transactions, idempotency keys, server-side rate limits, audit logs, integration tests, anti-cheat controls, telemetry, migration tooling and load testing. MySQL credentials should come from environment variables or a secret manager, never source control.
