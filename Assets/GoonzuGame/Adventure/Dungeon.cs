namespace GoonzuGame.Adventure {
    public class Dungeon {
        public int DungeonId { get; set; }
        public string Name { get; set; }
        public List<int> MonsterIds { get; set; }
        public int BossId { get; set; }
        public bool IsCleared { get; set; }
        public Dungeon(int id, string name, int bossId) {
            DungeonId = id;
            Name = name;
            BossId = bossId;
            MonsterIds = new List<int>();
            IsCleared = false;
        }
        public void Enter() { }
        public void Clear() { IsCleared = true; }
    }
}
