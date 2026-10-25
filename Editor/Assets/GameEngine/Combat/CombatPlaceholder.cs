namespace GameEngine.Combat
{
    public class CombatManager
    {
        public void StartCombat(string attacker, string defender)
        {
            System.Console.WriteLine($"Combat started: {attacker} vs {defender}");
        }
        public void CalculateDamage(string attacker, string defender)
        {
            System.Console.WriteLine($"Damage calculated for {attacker} attacking {defender}.");
        }
        public void EndCombat(string winner)
        {
            System.Console.WriteLine($"Combat ended. Winner: {winner}");
        }
    }
}
