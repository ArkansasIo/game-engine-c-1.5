# Game Engine C 1.5 Development Guide

## Architecture
The project combines the existing Unity/Goonzu gameplay layer with an isolated Engine15 C# rules and server-domain layer. Keep authoritative state on the server for multiplayer sessions.

## RPG model
Characters use ability scores, modifiers, proficiency, classes, equipment, conditions, hit points, experience and deterministic combat calculations. The D&D-inspired implementation intentionally avoids reproducing rulebook text.

## Server authority
Clients submit intents. The server validates permissions, resolves game rules, persists important state, and broadcasts replicated state. Never trust client damage, inventory, currency, quest completion, or progression values.

## Database
MySQL is the persistence target. SQL migrations live under `Database/MySQL`. Environment variables should supply credentials; secrets must not be committed.

## Operations
Cron definitions and maintenance scripts live under `ops/cron` and `scripts`. Backups should be encrypted and stored separately from the game server.

## Development workflow
1. Add deterministic domain logic under `Engine15`.
2. Add tests before changing combat/economy formulas.
3. Add or update SQL migration when persistent state changes.
4. Keep configuration environment-safe.
5. Run Unity compilation plus server-side unit tests in CI.
6. Review network authority and persistence implications for multiplayer features.

## Planned modules
Rendering, animation, physics, behavior trees, matchmaking, guilds, parties, trading, auction house, housing, achievements, telemetry, localization, modding, admin tools, and automated load tests.