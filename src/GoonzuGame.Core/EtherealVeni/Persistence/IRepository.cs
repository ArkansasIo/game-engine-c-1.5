namespace EtherealVeni.Persistence;

public interface IRepository<T, in TKey>
{
    Task<T?> GetAsync(TKey key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(T value, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey key, CancellationToken cancellationToken = default);
}
