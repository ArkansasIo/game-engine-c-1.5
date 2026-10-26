namespace EtherealVeni.Rpg.Progression;

public sealed record SkillDefinition(string Id, string Name, int MaximumRank = 10);

public sealed class SkillBook
{
    private readonly Dictionary<string, int> _ranks = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, int> Ranks => _ranks;

    public int GetRank(string skillId) => _ranks.TryGetValue(skillId, out var rank) ? rank : 0;

    public bool Train(string skillId, int amount = 1, int maximumRank = 10)
    {
        if (string.IsNullOrWhiteSpace(skillId) || amount <= 0) return false;
        var next = Math.Min(maximumRank, GetRank(skillId) + amount);
        if (next == GetRank(skillId)) return false;
        _ranks[skillId] = next;
        return true;
    }
}
