# GoonZu MMORPG Database Integration

This folder contains C# code for connecting your game to a SQLite database using the schema and seed data in Content/GoonZuInspired/DataTables/.

## Files
- `DatabaseManager.cs`: Simple wrapper for SQLite connection, query, and command execution.
- `DatabaseSeeder.cs`: Utility to seed a database from a .sql file (schema or data).

## Usage Example
```csharp
using GoonzuGame.Database;

string dbPath = "goonzu.db";
string schemaPath = "Content/GoonZuInspired/DataTables/goonzu_mmorpg_schema.sql";
string seedPath = "Content/GoonZuInspired/DataTables/goonzu_mmorpg_seed.sql";

// Create and seed database
DatabaseSeeder.SeedFromFile(dbPath, schemaPath);
DatabaseSeeder.SeedFromFile(dbPath, seedPath);

// Query data
using var db = new DatabaseManager(dbPath);
var items = db.Query("SELECT * FROM items;");
```

## Requirements
- Add `System.Data.SQLite` NuGet package to your project.
