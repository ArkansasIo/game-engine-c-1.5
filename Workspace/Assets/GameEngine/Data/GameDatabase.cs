using System;
using System.Data;
using System.Data.SQLite; // For Unity, use Mono.Data.Sqlite or SQLite-net

namespace GameEngine.Data
{
    public sealed class GameDatabase : IDisposable
    {
        private SQLiteConnection connection;

        public GameDatabase(string dbPath)
        {
            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();
        }

        public void ExecuteNonQuery(string sql)
        {
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        public DataTable ExecuteQuery(string sql)
        {
            using var cmd = new SQLiteCommand(sql, connection);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public void Dispose()
        {
            connection?.Close();
            connection = null;
        }
    }
}
