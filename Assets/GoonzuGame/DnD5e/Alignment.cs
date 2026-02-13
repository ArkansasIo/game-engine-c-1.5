namespace GoonzuGame.DnD5e {
    public class Alignment {
        public string Name { get; set; }
        public string Description { get; set; }
        public Alignment(string name, string desc) {
            Name = name;
            Description = desc;
        }
    }
}
