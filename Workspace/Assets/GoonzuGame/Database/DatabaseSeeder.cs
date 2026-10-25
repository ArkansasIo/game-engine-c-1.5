using System;
using System.IO;
// using System.Data.SQLite;

namespace GoonzuGame.Database
{
    // Stubbed DatabaseSeeder for non-SQLite builds
    public static class DatabaseSeeder
    {
        public static void SeedFromFile(string dbPath, string sqlFilePath)
        {
            throw new NotImplementedException("SQLite is disabled in this build.");
        }
    }
}
