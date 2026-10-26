using EtherealVeni.Authentication;
using EtherealVeni.Persistence;
using EtherealVeni.Rpg.Stats;

namespace EtherealVeni.Characters;

public sealed class CharacterService
{
    private readonly CharacterRepository _repository;

    public CharacterService(CharacterRepository repository) => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<CharacterRecord> CreateAsync(AccountId accountId, string name, AbilityScores abilities, CancellationToken cancellationToken = default)
    {
        var record = new CharacterRecord(CharacterId.New(), accountId, name, 1, 0, 10, 10, DateTimeOffset.UtcNow);
        await _repository.SaveAsync(record, cancellationToken);
        return record;
    }

    public async Task SaveAsync(AccountId accountId, CharacterState character, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(character);
        var record = new CharacterRecord(character.Id, accountId, character.Name, character.Stats.Level, character.Stats.Experience, character.Stats.Health, character.Stats.MaxHealth, DateTimeOffset.UtcNow);
        await _repository.SaveAsync(record, cancellationToken);
    }
}
