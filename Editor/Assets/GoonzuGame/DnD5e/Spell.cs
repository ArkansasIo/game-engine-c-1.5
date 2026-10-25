namespace GoonzuGame.DnD5e {
    public class Spell {
        public string Name { get; set; }
        public int Level { get; set; }
        public string School { get; set; }
        public string Effect { get; set; }
        public Spell(string name, int level, string school, string effect) {
            Name = name;
            Level = level;
            School = school;
            Effect = effect;
        }
    }
}
