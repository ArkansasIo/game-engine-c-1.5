namespace GameEngine.Magic
{
    /// <summary>
    /// Manages magic spells, abilities, and casting logic for players and NPCs.
    /// </summary>
    public class MagicManager
    {
        /// <summary>
        /// Casts a spell by name for a given caster.
        /// </summary>
        public void CastSpell(string casterId, string spellName) { /* ... */ }
        /// <summary>
        /// Learns a new spell for a character.
        /// </summary>
        public void LearnSpell(string characterId, string spellName) { /* ... */ }
        /// <summary>
        /// Forgets a spell for a character.
        /// </summary>
        public void ForgetSpell(string characterId, string spellName) { /* ... */ }
    }
}
