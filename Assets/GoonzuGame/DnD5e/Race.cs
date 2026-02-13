namespace GoonzuGame.DnD5e {
    public class Race {
        public string Name { get; set; }
        public string Description { get; set; }
        public Race(string name, string desc) {
            Name = name;
            Description = desc;
        }
    }
}
