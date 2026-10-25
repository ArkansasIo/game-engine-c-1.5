namespace GoonzuGame.DnD5e {
    public class Feat {
        public string Name { get; set; }
        public string Effect { get; set; }
        public Feat(string name, string effect) {
            Name = name;
            Effect = effect;
        }
    }
}
