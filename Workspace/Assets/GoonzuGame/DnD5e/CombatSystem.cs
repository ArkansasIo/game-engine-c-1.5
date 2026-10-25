using System;

namespace GoonzuGame.DnD5e
{
    public class CombatSystem
    {
        public void StartCombat()
        {
            Console.WriteLine("Combat started. Initiative order determined.");
        }
        public void EndCombat()
        {
            Console.WriteLine("Combat ended.");
        }
        public void PerformAction(string action)
        {
            Console.WriteLine($"Action performed: {action}");
        }
        public void PerformAttack(string attacker, string target)
        {
            Console.WriteLine($"{attacker} attacks {target}.");
        }
        public void PerformSpell(string caster, string spell, string target)
        {
            Console.WriteLine($"{caster} casts {spell} on {target}.");
        }
    }
}