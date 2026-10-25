using System.Collections.Generic;
using System;

namespace GoonzuGame.Achievements
{
    [System.Serializable]
    public class Achievement
    {
        public string Name;
        public bool IsUnlocked;
        public int Progress;
        public int MaxProgress = 100;

        public Achievement(string name, int maxProgress = 100)
        {
            Name = name;
            IsUnlocked = false;
            Progress = 0;
            MaxProgress = maxProgress;
        }

        public void AddProgress(int amount)
        {
            if (!IsUnlocked)
            {
                Progress += amount;
                if (Progress >= MaxProgress)
                {
                    IsUnlocked = true;
                    Console.WriteLine($"Achievement Unlocked: {Name}");
                    // Trigger events, UI updates, etc.
                }
            }
        }
    }

    public class AchievementManager
    {
        public static AchievementManager Instance { get; private set; }

        public List<Achievement> Achievements = new List<Achievement>();
        private static readonly Dictionary<string, (int progress, bool unlocked)> saveData = new Dictionary<string, (int, bool)>();

        static AchievementManager()
        {
            Instance = new AchievementManager();
            Instance.InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            // Add default achievements
            Achievements.Add(new Achievement("First Kill", 1));
            Achievements.Add(new Achievement("Level Up", 1));
            Achievements.Add(new Achievement("Explore 10 Areas", 10));
            // Load from save if needed
            LoadAchievements();
        }

        public void UnlockAchievement(string achievementName)
        {
            var achievement = Achievements.Find(a => a.Name == achievementName);
            if (achievement != null && !achievement.IsUnlocked)
            {
                achievement.IsUnlocked = true;
                achievement.Progress = achievement.MaxProgress;
                Console.WriteLine($"Achievement '{achievementName}' unlocked!");
                // Notify UI, play sound, etc.
            }
        }

        public void TrackProgress(string achievementName, int progress)
        {
            var achievement = Achievements.Find(a => a.Name == achievementName);
            if (achievement != null)
            {
                achievement.AddProgress(progress);
            }
        }

        public void ShareAchievement(string achievementName)
        {
            var achievement = Achievements.Find(a => a.Name == achievementName);
            if (achievement != null && achievement.IsUnlocked)
            {
                Console.WriteLine($"Sharing achievement: {achievementName}");
                // Implement sharing logic, e.g., social media, in-game chat
            }
        }

        public void AddAchievement(string achievementName, int maxProgress = 100)
        {
            Achievements.Add(new Achievement(achievementName, maxProgress));
        }

        public bool HasAchievement(string achievementName)
        {
            return Achievements.Exists(a => a.Name == achievementName && a.IsUnlocked);
        }

        public void DisplayAchievements()
        {
            foreach (var achievement in Achievements)
            {
                Console.WriteLine($"Achievement: {achievement.Name}, Unlocked: {achievement.IsUnlocked}, Progress: {achievement.Progress}/{achievement.MaxProgress}");
            }
        }

        // Save/Load methods
        public void SaveAchievements()
        {
            // Implement saving to file or static storage
            foreach (var achievement in Achievements)
            {
                saveData[achievement.Name] = (achievement.Progress, achievement.IsUnlocked);
            }
            Console.WriteLine("Achievements saved.");
        }

        public void LoadAchievements()
        {
            foreach (var achievement in Achievements)
            {
                if (saveData.ContainsKey(achievement.Name))
                {
                    var data = saveData[achievement.Name];
                    achievement.Progress = data.progress;
                    achievement.IsUnlocked = data.unlocked;
                }
            }
            Console.WriteLine("Achievements loaded.");
        }
    }
}
