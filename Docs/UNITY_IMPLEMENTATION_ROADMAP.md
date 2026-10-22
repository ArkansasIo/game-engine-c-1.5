# Unity Implementation Roadmap

## Product target

Build a Goonzu-inspired, original-content isometric MMORPG vertical slice in Unity, while preserving the repository's existing C++ and .NET work as separate modules.

## Guiding decisions

- Unity owns scene lifecycle, rendering, animation, input, audio, UI, and client presentation.
- C# domain rules should be testable outside Play Mode when possible.
- C++ remains optional and isolated behind a small C ABI; it should not run a second independent game loop inside Unity.
- Server authority is required for player inventory, currencies, crafting outcomes, markets, persistence and multiplayer combat.
- Original visual assets only; references to existing games guide genre-level mechanics, not copying game files or art.

## Milestones

### 1. Stabilize and test the UPM package
Deliver: clean Unity import, JSON schema checks, EditMode tests, validated manifests, deterministic bootstrap lifecycle.

### 2. Isometric world slice
Deliver: tilemap terrain; 5 data-driven towns; camera follow; click-to-move or keyboard movement; colliders; interaction prompts; town services.

### 3. MMORPG core loop
Deliver: character stats, inventory/stack rules, resource gathering, profession skills, crafting recipes, equipment, quests, NPC dialogue and monster encounters.

### 4. Economic loop
Deliver: listings, quantity/price validation, order locking, transaction ledger, taxes and town treasury. Keep only a local mock adapter in the Unity client; real mutation moves to the server.

### 5. Online services
Deliver: authentication, character persistence, chat, party/guild, world-zone instances, authoritative inventory/economy, rate limits, logs and metrics.

### 6. Production and content pipeline
Deliver: reproducible asset imports, Addressables or equivalent content loading, localization, accessibility, graphics settings, build automation, crash/log collection, platform test matrix.

## First sprint task list

- Add a Unity validation test suite and capture a real Editor compile result.
- Validate `goonzu_seed_world.json` before creating the ScriptableObject.
- Put explicit grid coordinates on each town definition.
- Add a generated-object root and replace/clear spawned markers deterministically.
- Add stable IDs and tests for item/recipe definitions.
- Remove UnityEngine-dependent scripts from the .NET app's compile list.
- Label native C++ managers as skeleton until real backends are connected.
