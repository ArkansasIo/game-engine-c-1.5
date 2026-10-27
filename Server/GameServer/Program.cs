using Microsoft.EntityFrameworkCore;
using GameServer.Database.Data;

namespace GameServer;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("GameServer starting...");
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseSqlite("Data Source=game.db")
            .Options;

        using (var db = new GameDbContext(options))
        {
            db.Database.EnsureCreated();
            Console.WriteLine("Database ready.");
        }

        Console.WriteLine("GameServer running. Press Ctrl+C to stop.");
        using var done = new ManualResetEventSlim(false);
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; done.Set(); };
        done.Wait();
        Console.WriteLine("GameServer stopped.");
    }
}
