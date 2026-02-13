# Expanded GoonZu MMORPG Database Integration

This update adds support for crafting, trading, economy, guilds, and parties.

## Files
- `goonzu_mmorpg_schema_expanded.sql`: Expanded SQL schema for new systems.
- `goonzu_mmorpg_seed_expanded.sql`: Example data for new systems.
- `ExpandedDatabaseSeeder.cs`: Utility to seed the expanded schema and data.

## Usage Example
```csharp
using GoonzuGame.Database;

string dbPath = "goonzu.db";
string schemaPath = "Content/GoonZuInspired/DataTables/goonzu_mmorpg_schema_expanded.sql";
string seedPath = "Content/GoonZuInspired/DataTables/goonzu_mmorpg_seed_expanded.sql";

ExpandedDatabaseSeeder.SeedExpanded(dbPath, schemaPath, seedPath);
```

## Next Steps
- Add C# data access classes for new tables (Crafting, Trading, Market, Guilds, Parties).
- Integrate with your game logic and UI as needed.
