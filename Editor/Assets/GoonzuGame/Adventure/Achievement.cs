namespace GoonzuGame.Adventure {
    public class Achievement {
        public int AchievementId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsUnlocked { get; set; }
        public Achievement(int id, string title, string desc) {
            AchievementId = id;
            Title = title;
            Description = desc;
            IsUnlocked = false;
        }
        public void Unlock() { IsUnlocked = true; }
    }
}
