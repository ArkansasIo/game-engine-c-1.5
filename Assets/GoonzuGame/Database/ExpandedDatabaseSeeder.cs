using System;
using System.IO;
// using System.Data.SQLite;

namespace GoonzuGame.Database
{
    // Stubbed ExpandedDatabaseSeeder for non-SQLite builds
    public static class ExpandedDatabaseSeeder
    {
        public static void SeedExpanded(string dbPath, string schemaPath, string seedPath)
        {
            throw new NotImplementedException("SQLite is disabled in this build.");
        }
    }
}
