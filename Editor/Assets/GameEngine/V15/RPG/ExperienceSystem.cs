namespace GameEngine.V15.RPG;

public sealed class ExperienceSystem
{
    public int Level { get; private set; } = 1;
    public long Experience { get; private set; }
    public event Action<int>? LevelUp;
    public void Add(long amount,int maxLevel=50){if(amount<=0)return; Experience+=amount; while(Level<maxLevel && Experience>=RpgFormulas.RequiredXp(Level)){Experience-=RpgFormulas.RequiredXp(Level);Level++;LevelUp?.Invoke(Level);}}
}
