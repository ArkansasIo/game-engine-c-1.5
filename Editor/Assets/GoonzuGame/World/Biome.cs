namespace GoonzuGame.World {
    public class Biome {
        public int BiomeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Biome(int id, string name, string desc) {
            BiomeId = id;
            Name = name;
            Description = desc;
        }
    }
}
