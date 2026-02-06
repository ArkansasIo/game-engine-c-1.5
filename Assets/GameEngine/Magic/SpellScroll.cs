namespace GameEngine.Magic
{
    public class SpellScroll
    {
        public string Name { get; set; }
        public MagicSchool School { get; set; }
        public SpellType Type { get; set; }
        public SpellSubType SubType { get; set; }
        public string Description { get; set; }
        public int Power { get; set; }
        public int ManaCost { get; set; }
        public int LevelRequirement { get; set; }
    }
}
