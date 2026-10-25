namespace EtherealVeni.Engine.Gameplay;

public enum QuestState { Available, Active, Completed, Failed }
public enum QuestObjectiveKind { Kill, Collect, Discover, Interact, ReachLevel }

public sealed record QuestObjectiveDefinition(
    string Id,
    string Description,
    QuestObjectiveKind Kind,
    string TargetId,
    int RequiredCount = 1)
{
    public QuestObjectiveDefinition Validate()
    {
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(TargetId))
            throw new ArgumentException("Objective ID, description and target ID are required.");
        if (RequiredCount < 1) throw new ArgumentOutOfRangeException(nameof(RequiredCount));
        return this;
    }
}

public sealed record QuestDefinition(
    string Id,
    string Title,
    string Description,
    IReadOnlyList<QuestObjectiveDefinition> Objectives,
    long ExperienceReward = 0,
    long CurrencyReward = 0)
{
    public QuestDefinition Validate()
    {
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Quest ID and title are required.");
        if (ExperienceReward < 0 || CurrencyReward < 0) throw new ArgumentOutOfRangeException(nameof(ExperienceReward));
        if (Objectives.Count == 0) throw new ArgumentException("A quest needs at least one objective.");
        if (Objectives.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count() != Objectives.Count)
            throw new ArgumentException("Objective IDs must be unique within a quest.");
        foreach (var objective in Objectives) objective.Validate();
        return this;
    }
}

public sealed class QuestProgress(QuestDefinition definition)
{
    private readonly Dictionary<string, int> _progress = definition.Objectives.ToDictionary(x => x.Id, _ => 0, StringComparer.Ordinal);
    public QuestDefinition Definition { get; } = definition.Validate();
    public QuestState State { get; private set; } = QuestState.Available;
    public IReadOnlyDictionary<string, int> Progress => _progress;

    public bool Start()
    {
        if (State != QuestState.Available) return false;
        State = QuestState.Active;
        return true;
    }

    public bool Report(QuestObjectiveKind kind, string targetId, int amount = 1)
    {
        if (State != QuestState.Active || amount <= 0) return false;
        var objective = Definition.Objectives.FirstOrDefault(x => x.Kind == kind && x.TargetId == targetId);
        if (objective is null) return false;
        _progress[objective.Id] = Math.Min(objective.RequiredCount, _progress[objective.Id] + amount);
        if (Definition.Objectives.All(x => _progress[x.Id] >= x.RequiredCount)) State = QuestState.Completed;
        return true;
    }

    public void Fail()
    {
        if (State == QuestState.Active) State = QuestState.Failed;
    }
}
