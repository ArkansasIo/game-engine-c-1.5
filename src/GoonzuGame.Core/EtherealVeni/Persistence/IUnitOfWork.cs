namespace EtherealVeni.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    Task BeginAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
