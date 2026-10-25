using System.Collections.Generic;

namespace GameEngine.Magic
{
    public class MagicSystem : System
    {
        public List<SpellScroll> KnownSpells { get; private set; } = new();

        public void LearnSpell(SpellScroll spell)
        {
            if (!KnownSpells.Contains(spell))
                KnownSpells.Add(spell);
        }

        public void CastSpell(string spellName)
        {
            var spell = KnownSpells.Find(s => s.Name == spellName);
            if (spell != null)
            {
                // TODO: Implement spell effect logic
            }
        }
    }
}
