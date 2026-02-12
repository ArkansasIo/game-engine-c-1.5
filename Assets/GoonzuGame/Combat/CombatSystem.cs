using System;

namespace GoonzuGame.Combat
{
    public class CombatSystem
    {
        public void StartCombat(Character a, Character b)
        {
            Console.WriteLine($"Combat started between {a.Name} and {b.Name}.");
            while (a.Health > 0 && b.Health > 0)
            {
                a.Attack(b);
                if (b.Health > 0) b.Attack(a);
            }
            string winner = a.Health > 0 ? a.Name : b.Name;
            Console.WriteLine($"Combat ended. Winner: {winner}");
        }
    }
}
