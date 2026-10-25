# MySQL DDL

This directory contains the Data Definition Language (DDL) for the game's persistent MySQL database.

## Purpose

DDL defines the authoritative database structure used by the C#/.NET game server. It is separate from runtime data manipulation and from Entity Framework Core migration code.

## Initial schema

- `001_initial_schema.sql` — creates the `game_engine` database and the initial MMORPG persistence model.

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

## Authority

The .NET server remains authoritative for gameplay state. Clients send intents; the server validates and persists accepted state changes. MySQL is the durable persistence layer.

## Migration policy

Do not edit an already-applied production DDL migration in place. Add a new numbered migration or an Entity Framework Core migration for subsequent schema changes.

Database credentials must be supplied through environment variables or a secrets manager; never commit passwords or connection strings containing credentials.
