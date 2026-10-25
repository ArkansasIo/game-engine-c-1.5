# Ethereal Veni — Ethereo Pandora

Ethereal Veni is the standalone C#/.NET game-engine platform for **Ethereo Pandora**, a 64-bit RPG/MMORPG project.

## Product identity

- **Engine:** Ethereal Veni
- **Game:** Ethereo Pandora
- **Core:** C#/.NET 10, 64-bit
- **Scripting:** C#, Lua, and native Blueprint-style visual scripting
- **Integration:** Unity and Unreal 5 are optional adapters, not core dependencies
- **Database:** MySQL-backed authoritative server persistence

## Architecture

```text
Ethereo Pandora
      |
      v
Ethereal Veni Runtime
      |
  +---+-----------+
  |   |           |
 C# Blueprint    Lua
  |   |           |
  +---+-----------+
      |
      v
 Engine APIs
      |
 +----+------------------+
 | Native | Unity | UE5  |
 +--------+-------+------+
```

## Editor

Ethereal Veni includes its own editor architecture with a unique dockable workspace rather than reproducing Unity or Unreal's editor implementation. Planned editor surfaces include the world viewport, entity hierarchy, inspector, asset browser, Blueprint graph editor, Lua editor, console, profiler, debugger, and platform build manager.

## Visual scripting

Blueprint is an Ethereal Veni-owned visual scripting layer. It is not Unreal Blueprint and does not require Unreal Engine. Blueprint graphs compile to an engine intermediate representation and can call C# engine APIs and the Lua runtime through explicit bridges.

## Unity and Unreal 5

Unity and Unreal 5 are peer integration targets. Their adapters translate Ethereal Veni entities, assets, scenes, materials, animation, input, and other supported engine data into the host environment. The core engine remains buildable without either SDK installed.

## Legacy reference material

Historical GoonZu-oriented files are treated as reference/legacy material only. They must not define the active product identity. When migrated, such material belongs under `References/GoonZu/` and should be marked `[REFERENCE]` or `[LEGACY]`.

## Documentation

The repository uses companion Markdown documentation for significant source, configuration, schema, asset, and integration files. Documentation includes source links, section anchors, dependencies, API notes, comments, and standard flags.

## Build outputs

Generated binaries belong under the build pipeline's output structure and should not normally be committed:

```text
Output/
├── Engine/
│   ├── Windows-x64/
│   ├── Linux-x64/
│   ├── macOS-x64/
│   ├── macOS-arm64/
│   ├── PS5/
│   ├── Xbox/
│   └── Switch/
└── Game/
    ├── Windows-x64/
    ├── Linux-x64/
    ├── macOS-x64/
    ├── macOS-arm64/
    ├── PS5/
    ├── Xbox/
    └── Switch/
```

Console targets require the appropriate licensed first-party SDKs and build environments; those SDKs are not stored in this public repository.

## Migration rule

Do not mass-rewrite serialized asset GUIDs, save-game identifiers, database IDs, or other stable identifiers merely to change display names. Use explicit compatibility aliases and migration maps when required.
