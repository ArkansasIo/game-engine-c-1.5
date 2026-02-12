using System;
using System.IO;
using System.Data.SQLite;

namespace GoonzuGame.Database
{
    public static class DatabaseSeeder
    {
        public static void SeedFromFile(string dbPath, string sqlFilePath)
        {
            if (!File.Exists(sqlFilePath)) throw new FileNotFoundException(sqlFilePath);
            var sql = File.ReadAllText(sqlFilePath);
            using var conn = new SQLiteConnection($"Data Source={dbPath}");
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
    }
}
