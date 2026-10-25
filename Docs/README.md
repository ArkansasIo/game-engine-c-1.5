# Project Documentation Index

This folder is the human-readable documentation hub for Game Engine C 1.5.

## Documentation model

Documentation is organized beside the systems it describes. Each major source/content/configuration area should contain a local `Docs/` folder with purpose, file-by-file explanations, architecture notes, source-code cross-references, diagrams, and links to the exact GitHub source files.

## Source links and flags

Source references use GitHub links to the exact file. Section-level references may use GitHub line anchors when a stable line range is available.

Example:

`[Source: GameDbContext.cs](../Server/GameServer.Database/Data/GameDbContext.cs)`

Use these flags consistently:

- **[CORE]** — engine-authoritative runtime code.
- **[SERVER]** — server-authoritative multiplayer code.
- **[DB]** — persistent database schema or data access.
- **[SCRIPT]** — Lua or scripting-layer code.
- **[CONTENT]** — game-authored content/data.
- **[CONFIG]** — configuration/build/deployment data.
- **[ASSET]** — imported/generated asset pipeline material.
- **[PLATFORM]** — platform abstraction or SDK adapter.
- **[LEGACY]** — compatibility/transition code.
- **[REFERENCE]** — design/reference material only; not product implementation.

## Major areas

- `../Assets/GameEngine/` — existing Unity-facing engine code and transition surface.
- `../Assets/AIAssetTools/` — asset generation, manifests, import, validation, and provenance tooling.
- `../Server/` — C#/.NET authoritative server and persistence layer.
- `../Database/MySQL/` — MySQL DDL, migrations, seeds, and database documentation.
- `../UML/` — architecture diagrams and system relationships.
- `../Content/` — game-authored data/content when present.
- `../Config/` — runtime/build/platform configuration when present.
- `../Engine/` — modern engine implementation and rendering/platform abstractions as introduced.
- `../Scripting/Lua/` — Lua runtime, scripts, APIs, sandbox, and C# bridge as introduced.

## Side-by-side rule

Every major folder should have a local `Docs/` folder explaining the files beside it. Documentation links to exact source files. Source files should add XML documentation or a short header linking back to the appropriate local documentation when practical.

## Database note

The MySQL DDL already defines the authoritative persistence model for accounts, players, characters, quests, items, inventory, loot, parties, guilds, story state, world events, and transactions. See `../Database/MySQL/DDL/001_initial_schema.sql`.
