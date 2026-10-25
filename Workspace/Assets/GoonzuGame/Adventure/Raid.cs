namespace GoonzuGame.Adventure {
    public class Raid {
        public int RaidId { get; set; }
        public string Name { get; set; }
        public List<int> BossIds { get; set; }
        public bool IsActive { get; set; }
        public Raid(int id, string name) {
            RaidId = id;
            Name = name;
            BossIds = new List<int>();
            IsActive = false;
        }
        public void StartRaid() { IsActive = true; }
        public void EndRaid() { IsActive = false; }
    }
}
