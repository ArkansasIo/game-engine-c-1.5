namespace GoonzuGame.World {
    public class Zone {
        public int ZoneId { get; set; }
        public string Name { get; set; }
        public string Biome { get; set; }
        public List<int> MonsterIds { get; set; }
        public Zone(int id, string name, string biome) {
            ZoneId = id;
            Name = name;
            Biome = biome;
            MonsterIds = new List<int>();
        }
        public void AddMonster(int monsterId) { MonsterIds.Add(monsterId); }
    }
}
