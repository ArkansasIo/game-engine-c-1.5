namespace EtherealVeni.Rules;

public readonly record struct D20Check(int Roll, int Modifier, int DifficultyClass)
{
    public int Total => Roll + Modifier;
    public bool Success => Roll != 1 && (Roll == 20 || Total >= DifficultyClass);
    public bool Critical => Roll == 20;
    public bool CriticalFailure => Roll == 1;
}
