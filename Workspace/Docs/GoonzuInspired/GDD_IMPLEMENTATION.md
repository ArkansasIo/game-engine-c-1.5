# GoonZu-Inspired MMORPG Implementation

This document implements the public design characteristics researched from GoonZu Online / Luminary as an original game design layer. It does not copy proprietary art, maps, characters, client files, or extracted data.

## Design pillars

1. Player-driven production and trade
2. Skill-based progression instead of mandatory combat classes
3. Resource -> processing -> crafting -> enchanting -> market loops
4. Town economies and elected local government
5. Server-level elected ruler system
6. Guilds, mentoring, pets, mounts, hunting, quests, and combat
7. 2D isometric anime-fantasy presentation

## Character progression

Core attributes: Strength, Agility, Vitality, Wisdom, Dexterity, Luck.

Weapon specializations and production skills progress through use. A character can specialize in combat, gathering, manufacturing, commerce, politics, or a hybrid role.

## Economy

Every marketable item has a base value, supply, demand, regional multiplier, and tax context. The runtime should never assume that an NPC is the only source of finished equipment.

`MarketPrice = BasePrice * DemandMultiplier * SupplyMultiplier * RegionMultiplier * TaxMultiplier`

The server remains authoritative for balances, ownership, orders, elections, taxes, and transactions.

## Crafting

The production pipeline is:

`Gather -> Process -> Manufacture -> Quality Roll -> Enchant -> Trade`

Crafting success is influenced by the relevant production skill and recipe difficulty. Quality tiers are generated server-side.

## Town government

A town owns a treasury and economic configuration. Shareholders can participate in local elections. The elected town leader can administer approved town settings such as taxes and development priorities.

## Server government

The GoonZu-inspired ruler is an elected player office. Elections are time-boxed and server-authoritative. Administrative powers must be represented as explicit permissions, not arbitrary client commands.

## Social systems

- Guilds with shared progression and storage
- Mentor/mentee relationships
- Party hunting
- Player marketplace
- Town ownership and investment
- Player-run manufacturing

## Content pipeline

`Blender/2D source -> isometric render/export -> sprite atlas -> asset manifest -> C#/C++ runtime`

Existing repository source files remain untouched. This implementation is additive.

## Initial implementation scope

- Data model: `GoonzuGameData.cs`
- C++ runtime contracts: `GoonzuGameSystems.hpp`
- Server seed data: `goonzu_world.json`
- Database schema: `goonzu_economy.sql`
- Asset catalog: `goonzu_asset_catalog.json`
- Validation/setup script: `build_goonzu_content.sh`

## Reference basis

Public descriptions consistently identify GoonZu/Luminary as a 2D fantasy MMORPG with crafting, enchanting, player economics, stocks/real estate, and elected political leadership. The implementation uses those high-level mechanics as inspiration while creating original content.
