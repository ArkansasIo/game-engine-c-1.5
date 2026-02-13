namespace GoonzuGame.DnD5e {
    public class Background {
        public string Name { get; set; }
        public string Feature { get; set; }
        public Background(string name, string feature) {
            Name = name;
            Feature = feature;
        }
    }
}
