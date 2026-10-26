using EtherealVeni.Serialization;

namespace EtherealVeni.Persistence;

public sealed class JsonRepository<T, TKey> : IRepository<T, TKey> where TKey : notnull
{
    private readonly Func<T, TKey> _keySelector;
    private readonly JsonSaveStore<List<T>> _store = new();
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonRepository(string path, Func<T, TKey> keySelector)
    {
        _path = string.IsNullOrWhiteSpace(path) ? throw new ArgumentException("Path is required.", nameof(path)) : path;
        _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
    }

    public async Task<T?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.FirstOrDefault(x => EqualityComparer<TKey>.Default.Equals(_keySelector(x), key));
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try { return await _store.LoadAsync(_path, cancellationToken) ?? new List<T>(); }
        finally { _gate.Release(); }
    }

    public async Task SaveAsync(T value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var values = await _store.LoadAsync(_path, cancellationToken) ?? new List<T>();
            var key = _keySelector(value);
            var index = values.FindIndex(x => EqualityComparer<TKey>.Default.Equals(_keySelector(x), key));
            if (index >= 0) values[index] = value; else values.Add(value);
            await _store.SaveAsync(_path, values, cancellationToken);
        }
        finally { _gate.Release(); }
    }

    public async Task DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var values = await _store.LoadAsync(_path, cancellationToken) ?? new List<T>();
            values.RemoveAll(x => EqualityComparer<TKey>.Default.Equals(_keySelector(x), key));
            await _store.SaveAsync(_path, values, cancellationToken);
        }
        finally { _gate.Release(); }
    }
}
