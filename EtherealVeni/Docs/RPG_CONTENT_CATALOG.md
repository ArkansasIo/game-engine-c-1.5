# Original RPG Content Catalog

The catalog is an approximate large-MMO content scale for Ethereo Pandora. Counts are explicit project targets, not claims about a particular WoW release, and all names, descriptions, progression data, and loot are generated as original content.

## Catalog size

| Content | Count |
|---|---:|
| Items | 300 |
| Creatures/monsters | 300 |
| Main-story quests | 500 |
| Side quests | 26,000 |
| Daily repeatable quests | 2,000 |
| Weekly repeatable quests | 500 |
| Profession quests | 500 |
| Dungeon/raid/trial quests | 500 |
| **Total quests** | **30,000** |
| Dungeons | 100 |
| Raids | 50 |
| Trials | 100 |

The 300-item catalog contains weapons, armor, shields, consumables, materials, accessories, quest tokens, gems, and profession tools. Creature and instance reward tables reference item definitions through stable item keys.

## Schema and seed

Apply the initial MySQL schema, then [`002_rpg_content_systems.sql`](../../Database/MySQL/DDL/002_rpg_content_systems.sql), then the generated [`003_original_rpg_catalog_seed.sql`](../../Database/MySQL/DDL/003_original_rpg_catalog_seed.sql). The seed is reproducible with [`generate_original_rpg_catalog.py`](../../Database/MySQL/Tools/generate_original_rpg_catalog.py).

Quest definitions support level ranges, zones, repeat intervals, chains, ordered multi-objectives, rewards, and prerequisites. Character state remains in `quest_progress` and `quest_objective_progress`; content definitions do not mix with player state.

Dungeon, raid, and trial definitions include difficulty variants, party sizes, encounters, boss/creature references, weighted loot, and level ranges. Runtime instance state is stored per run, party member, encounter, and character lockout. These definitions are designed for server-authoritative validation; the UE5 prototype does not connect directly to MySQL or grant client authority over persistent rewards.

## Adding gameplay content

Extend the catalog generator's source arrays and generation rules, then regenerate the SQL seed and validate its reported counts. For shipped production content, freeze and version a generated seed rather than modifying an already-applied migration. Keep quest objective and instance target IDs stable so saved progress remains valid.
