# Ethereal Veni authoritative multiplayer server

## Run locally

From the repository root:

    dotnet run --project EtherealVeni/Server/EtherealVeni.Server.csproj

The server exposes /health, /server/info, and JSON endpoints under /api/v1.

## Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| GET | /api/v1/world/players | List in-memory player snapshots |
| POST | /api/v1/players/join | Create server-owned player identity and starter state |
| GET | /api/v1/players/{playerId} | Read a player snapshot |
| POST | /api/v1/world/move | Validate movement bounds and displacement |
| POST | /api/v1/combat/attack | Apply a server-calculated melee attack against a nearby player |
| POST | /api/v1/progression/experience | Grant XP and calculate level-ups |
| POST | /api/v1/chat | Relay sanitized chat with per-player rate limiting |
| GET | /api/v1/chat/recent?count=50 | Read bounded chat history |

Join request example:

    {"username":"Adventurer_01"}

Movement request example:

    {"playerId":"PLAYER_GUID","position":{"x":1,"y":0,"z":0}}

Use the actual player ID returned by join. Position is a three-number object; server-owned health, position, XP, level and identity must not be accepted from clients as authoritative state.

## Trust boundary and limitations

The server validates usernames, coordinates, movement displacement, attack range, attacker/target state and XP. Melee damage is calculated on the server (10 damage in this initial slice); there is no client-supplied damage amount. Chat is length-limited, control-character filtered and rate-limited.

This is a development baseline, not production-ready MMO infrastructure. Player and chat state are in memory and reset on restart. Authentication, authorization, durable account/session storage, migrations, zone/shard simulation, WebSocket replication, transactional inventory/economy, anti-cheat telemetry, distributed chat, TLS and production request limits remain necessary before public deployment. Integrate the repository's database layer behind a persistence abstraction as the next step.
