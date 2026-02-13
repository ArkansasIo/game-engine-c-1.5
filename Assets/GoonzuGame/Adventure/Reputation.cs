namespace GoonzuGame.Adventure {
    public class Reputation {
        public int FactionId { get; set; }
        public string FactionName { get; set; }
        public int Points { get; set; }
        public Reputation(int id, string name) {
            FactionId = id;
            FactionName = name;
            Points = 0;
        }
        public void AddPoints(int amount) { Points += amount; }
        public void RemovePoints(int amount) { Points -= amount; }
    }
}
