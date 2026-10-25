namespace GoonzuGame.Adventure {
    public class Exploration {
        public int ExplorationId { get; set; }
        public string Area { get; set; }
        public string Discovery { get; set; }
        public bool IsComplete { get; set; }
        public Exploration(int id, string area, string discovery) {
            ExplorationId = id;
            Area = area;
            Discovery = discovery;
            IsComplete = false;
        }
        public void Complete() { IsComplete = true; }
    }
}
