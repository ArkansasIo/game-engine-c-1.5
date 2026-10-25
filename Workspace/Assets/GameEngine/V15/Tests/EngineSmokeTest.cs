namespace GameEngine.V15.Tests;

public static class TestAssert
{
    public static void True(bool value,string message="Assertion failed"){if(!value)throw new InvalidOperationException(message);}
    public static void Equal<T>(T expected,T actual,string message="Values differ"){if(!EqualityComparer<T>.Default.Equals(expected,actual))throw new InvalidOperationException($"{message}: expected {expected}, got {actual}");}
}
public sealed class EngineSmokeTest
{
    public void Run(){TestAssert.Equal(1500,GameEngine.V15.RPG.RpgFormulas.MaxHealth(new GameEngine.V15.RPG.AttributeSet()));}
}
