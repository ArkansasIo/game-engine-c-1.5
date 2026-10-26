namespace EtherealVeni.Rpg.Quests;

public enum QuestStatus { Locked, Available, Active, Completed, Failed }

public sealed record QuestObjective(string Id, string Description, int RequiredCount = 1);

public sealed class QuestDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public List<QuestObjective> Objectives { get; init; } = new();
    public long ExperienceReward { get; init; }
}

public sealed class QuestProgress
{
    private readonly Dictionary<string, int> _counts = new();
    public QuestStatus Status { get; private set; } = QuestStatus.Available;
    public IReadOnlyDictionary<string, int> Counts => _counts;

    public void Start() { if (Status == QuestStatus.Available) Status = QuestStatus.Active; }
    public void AddProgress(QuestObjective objective, int amount = 1)
    {
        if (Status != QuestStatus.Active || amount <= 0) return;
        _counts[objective.Id] = Math.Min(objective.RequiredCount, _counts.GetValueOrDefault(objective.Id) + amount);
    }
    public bool IsComplete(IEnumerable<QuestObjective> objectives) => objectives.All(x => _counts.GetValueOrDefault(x.Id) >= x.RequiredCount);
    public void Complete() => Status = QuestStatus.Completed;
    public void Fail() => Status = QuestStatus.Failed;
}
