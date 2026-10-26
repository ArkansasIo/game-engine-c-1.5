using EtherealVeni.Characters;
using EtherealVeni.Persistence;

namespace EtherealVeni.Characters;

public sealed class CharacterRepository
{
    private readonly IRepository<CharacterRecord, CharacterId> _store;

    public CharacterRepository(IRepository<CharacterRecord, CharacterId> store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public Task<CharacterRecord?> GetAsync(CharacterId id, CancellationToken cancellationToken = default) => _store.GetAsync(id, cancellationToken);
    public Task<IReadOnlyList<CharacterRecord>> GetAllAsync(CancellationToken cancellationToken = default) => _store.GetAllAsync(cancellationToken);
    public Task SaveAsync(CharacterRecord record, CancellationToken cancellationToken = default) => _store.SaveAsync(record, cancellationToken);
    public Task DeleteAsync(CharacterId id, CancellationToken cancellationToken = default) => _store.DeleteAsync(id, cancellationToken);
}
