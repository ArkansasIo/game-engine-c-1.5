namespace GoonzuGame.Quests {
    public class Quest {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public void Complete() { IsCompleted = true; }
    }
}
