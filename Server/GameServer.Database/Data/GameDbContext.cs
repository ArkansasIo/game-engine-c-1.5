using Microsoft.EntityFrameworkCore;

namespace GameServer.Database.Data;

public sealed class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<PlayerRecord> Players => Set<PlayerRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerRecord>(entity =>
        {
            entity.ToTable("players");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
        });
    }
}

public sealed class PlayerRecord
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public long Experience { get; set; }
    public long Gold { get; set; }
}
