namespace Engine15.RPG.Dnd5e;

public enum Condition { Blinded, Charmed, Deafened, Frightened, Grappled, Incapacitated, Invisible, Paralyzed, Petrified, Poisoned, Prone, Restrained, Stunned, Unconscious }

public sealed class ConditionSet
{
    private readonly HashSet<Condition> active = new();
    public IReadOnlyCollection<Condition> Active => active;
    public void Add(Condition condition) => active.Add(condition);
    public void Remove(Condition condition) => active.Remove(condition);
    public bool Has(Condition condition) => active.Contains(condition);
    public void Clear() => active.Clear();
}