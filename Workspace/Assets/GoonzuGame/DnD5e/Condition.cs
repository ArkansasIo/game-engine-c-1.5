namespace GoonzuGame.DnD5e {
    public class Condition {
        public string Name { get; set; }
        public string Effect { get; set; }
        public Condition(string name, string effect) {
            Name = name;
            Effect = effect;
        }
    }
}
