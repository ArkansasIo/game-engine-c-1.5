namespace EtherealVeni.Classes;

public sealed record SubclassDefinition(
    string Id,
    string ClassId,
    string Name,
    IReadOnlyList<string> GrantedSkills);
