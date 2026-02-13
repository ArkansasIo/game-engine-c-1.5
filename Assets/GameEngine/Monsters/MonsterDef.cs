using GoonzuGame.Monsters;
namespace GoonzuGame.Monsters
{
    /// <summary>
    /// Defines a monster, including its type, stats, and properties.
    /// </summary>
    public class MonsterDef
    {
        /// <summary>
        /// The unique name or identifier for this monster.
        /// </summary>
        public string Name;
        /// <summary>
        /// The type of this monster (e.g., Beast, Undead, Dragon).
        /// </summary>
        public MonsterType Type;
        /// <summary>
        /// The base level of this monster.
        /// </summary>
        public int Level;
        /// <summary>
        /// The base health points of this monster.
        /// </summary>
        public int HP;
        /// <summary>
        /// The base attack power of this monster.
        /// </summary>
        public int Attack;
        /// <summary>
        /// The base defense value of this monster.
        /// </summary>
        public int Defense;
        /// <summary>
        /// The experience points awarded for defeating this monster.
        /// </summary>
        public int Exp;
        /// <summary>
        /// Additional description or lore for this monster.
        /// </summary>
        public string Description;
    }
}
