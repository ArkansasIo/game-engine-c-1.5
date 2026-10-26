namespace EtherealVeni.Rules;

public readonly record struct SavingThrow(D20Check Check, string Ability)
{
    public bool Success => Check.Success;
}
