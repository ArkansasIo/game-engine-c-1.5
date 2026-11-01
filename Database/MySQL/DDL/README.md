# MySQL DDL

This directory contains the Data Definition Language (DDL) for the game's persistent MySQL database.

## Purpose

DDL defines the authoritative database structure used by the C#/.NET game server. It is separate from runtime data manipulation and from Entity Framework Core migration code.

## Initial schema

- `001_initial_schema.sql` — creates the `game_engine` database and the initial MMORPG persistence model.
- `002_rpg_content_systems.sql` — adds quest objectives/rewards/prerequisites, creatures, instance catalogs/difficulties/encounters, and persistent instance-run state. Apply after `001_initial_schema.sql`.
- `003_original_rpg_catalog_seed.sql` — generated original catalog data for 300 items, 300 creatures, 30,000 quests, 100 dungeons, 50 raids, and 100 trials. Apply after the schema migrations.
- `../Tools/generate_original_rpg_catalog.py` — deterministic source generator for the catalog seed SQL.

The initial schema covers:

- accounts and players
- characters and progression
- quests and quest progress
- items and inventory
- loot tables and weighted/guaranteed loot entries
- parties and party membership
- guilds and guild membership
- story/narrative state
- world events
- economy/transaction records

The content catalog is an approximate large-MMO scale target, not a claim to match an exact WoW release or historical count. All generated names and quest descriptions are original. `quest_progress` stores per-character lifecycle state, while `quest_objective_progress` stores objective-level counts. Instance definitions, difficulty modes, encounters, runs, members, encounter state, lockouts, and loot are stored separately.

## Apply

From a MySQL 8 client, apply the initial schema, content migration, and generated catalog in order:

```sql
SOURCE Database/MySQL/DDL/001_initial_schema.sql;
SOURCE Database/MySQL/DDL/002_rpg_content_systems.sql;
SOURCE Database/MySQL/DDL/003_original_rpg_catalog_seed.sql;
```

The migrations and seed are intended for an empty schema. Do not edit an applied migration. To regenerate the original seed, run `python Database/MySQL/Tools/generate_original_rpg_catalog.py`; it overwrites the generated seed file with deterministic catalog data.

## Authority

The .NET server remains authoritative for gameplay state. Clients send intents; the server validates and persists accepted state changes. MySQL is the durable persistence layer.

## Migration policy

Do not edit an already-applied production DDL migration in place. Add a new numbered migration or an Entity Framework Core migration for subsequent schema changes.

Database credentials must be supplied through environment variables or a secrets manager; never commit passwords or connection strings containing credentials.
