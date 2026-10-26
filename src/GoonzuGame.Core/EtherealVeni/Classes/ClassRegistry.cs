namespace EtherealVeni.Classes;

public sealed class ClassRegistry
{
    private readonly Dictionary<string, ClassDefinition> _classes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, SubclassDefinition> _subclasses = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<ClassDefinition> Classes => _classes.Values;
    public IReadOnlyCollection<SubclassDefinition> Subclasses => _subclasses.Values;

    public void Register(ClassDefinition definition) => _classes[definition.Id] = definition;
    public void Register(SubclassDefinition definition) => _subclasses[definition.Id] = definition;
    public bool TryGetClass(string id, out ClassDefinition? definition) => _classes.TryGetValue(id, out definition);
    public bool TryGetSubclass(string id, out SubclassDefinition? definition) => _subclasses.TryGetValue(id, out definition);
}
