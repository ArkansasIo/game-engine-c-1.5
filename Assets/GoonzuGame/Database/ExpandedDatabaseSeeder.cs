using System;
using System.IO;
using System.Data.SQLite;

namespace GoonzuGame.Database
{
    public static class ExpandedDatabaseSeeder
    {
        public static void SeedExpanded(string dbPath, string schemaPath, string seedPath)
        {
            if (!File.Exists(schemaPath)) throw new FileNotFoundException(schemaPath);
            if (!File.Exists(seedPath)) throw new FileNotFoundException(seedPath);
            var schema = File.ReadAllText(schemaPath);
            var seed = File.ReadAllText(seedPath);
            using var conn = new SQLiteConnection($"Data Source={dbPath}");
            conn.Open();
            using (var cmd = new SQLiteCommand(schema, conn))
                cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand(seed, conn))
                cmd.ExecuteNonQuery();
        }
    }
}
