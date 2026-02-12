using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace GoonzuGame.Database
{
    public class DatabaseManager : IDisposable
    {
        private SQLiteConnection _connection;
        public DatabaseManager(string dbPath)
        {
            bool create = !File.Exists(dbPath);
            _connection = new SQLiteConnection($"Data Source={dbPath}");
            _connection.Open();
            if (create)
            {
                // Optionally run schema creation here
            }
        }

        public DataTable Query(string sql)
        {
            using var cmd = new SQLiteCommand(sql, _connection);
            using var adapter = new SQLiteDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public int Execute(string sql)
        {
            using var cmd = new SQLiteCommand(sql, _connection);
            return cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
