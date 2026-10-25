# Gameplay Core

The gameplay folder contains reusable runtime-neutral RPG mechanics. Keep rendering, UI framework, database access and transport adapters outside this layer.

## Current APIs

- `AttributeSet`: attribute values and simple derived-stat helpers.
- `CombatSystem`: combat damage requests/results with injectable random source.
- `Inventory`: registered item definitions, stack-aware inventory and capacity checks.
- `QuestProgress`: data-driven objective tracker.
- `RpgProgression`: XP curve and level lookup.

Callers should treat client-submitted inputs as untrusted. The authoritative server validates requests and persists the accepted resulting state.
