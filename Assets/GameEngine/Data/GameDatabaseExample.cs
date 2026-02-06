using System;
using System.Data;

namespace GameEngine.Data
{
    public static class GameDatabaseExample
    {
        public static void RunExample()
        {
            using var db = new GameDatabase("game.db");

            // Create tables
            db.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS Players (Id TEXT PRIMARY KEY, Name TEXT, Level INT);");
            db.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS Inventory (PlayerId TEXT, ItemId TEXT, Count INT);");

            // Insert data
            db.ExecuteNonQuery("INSERT INTO Players (Id, Name, Level) VALUES ('p1', 'Hero', 1);");

            // Query data
            DataTable table = db.ExecuteQuery("SELECT * FROM Players;");
            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine($"{row["Id"]}: {row["Name"]} (Level {row["Level"]})");
            }
        }
    }
}
