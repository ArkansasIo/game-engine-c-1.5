# GoonZu-Inspired Runtime — Phase 2

This phase turns the research into an additive runtime specification for `game-engine-c-1.5`.

## Design boundary

The implementation uses public high-level MMORPG mechanics as inspiration. It does not copy proprietary GoonZu artwork, maps, source code, extracted data, logos, or game files.

## Runtime loop

```text
Input
  -> Character State
  -> Combat / Gathering / Crafting
  -> Inventory
  -> Marketplace
  -> Town Services
  -> Politics / Guilds
  -> Persistent World State
```

## Skill model

Every skill has two components:

- Basic level: 0–30
- Experience level: 0–70
- Effective level: 0–100

Skill families:

- Combat
- Gathering
- Manufacturing
- Social
- Companion

## Economy

Resources flow through a production chain rather than becoming meaningless loot:

```text
Ore / Wood / Herb / Fish
        |
        v
 Processing
        |
        v
 Intermediate Materials
        |
        v
 Manufacturing
        |
        v
 Finished Equipment / Consumables
        |
        +----> Enchanting
        |
        +----> Player Marketplace
```

## Marketplace

Listings contain seller, town, item, quantity, and unit price. Runtime snapshots expose supply and average price. SQL adds an indexed item/price lookup and aggregate market view.

## Town politics

Each town has a treasury and tax rate. Elections hold weighted votes and resolve a winning candidate. Future phases can connect elected leadership to service pricing, town projects, guards, and guild permissions.

## Asset pipeline

```text
Original Blender scene
        -> Orthographic/isometric camera
        -> Render / sprite export
        -> Sprite sheet
        -> Asset manifest
        -> C# / C++ runtime loader
        -> GameEngine Scene / Entity
```

## Next phase

1. Connect runtime objects to `GameEngine::Scene` and `Entity` without altering the existing engine contract.
2. Add an isometric camera/grid and map chunk format.
3. Add inventory/equipment transactions.
4. Add deterministic crafting transaction tests.
5. Add market buy/sell transactions with server-authoritative validation.
6. Add town election lifecycle and tax collection.
7. Add original character, NPC, building, terrain, item, monster, and UI sprite sets.
