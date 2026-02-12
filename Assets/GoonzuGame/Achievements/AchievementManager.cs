using System;

namespace GoonzuGame.Achievements
{
    using System.Collections.Generic;

    public class Achievement
    {
        public string Name { get; set; }
        public bool IsUnlocked { get; set; }
        public int Progress { get; set; }
        public Achievement(string name)
        {
            Name = name;
            IsUnlocked = false;
            Progress = 0;
        }
    }

    public class AchievementManager
    {
        public List<Achievement> Achievements { get; set; }

        public AchievementManager()
        {
            Achievements = new List<Achievement>();
        }

        public void UnlockAchievement(string achievementName)
        {
            var achievement = Achievements.Find(a => a.Name == achievementName);
            if (achievement != null && !achievement.IsUnlocked)
            {
                achievement.IsUnlocked = true;
                Console.WriteLine($"Achievement '{achievementName}' unlocked!");
            }
        }

        public void TrackProgress(string achievementName, int progress)
        {
            var achievement = Achievements.Find(a => a.Name == achievementName);
            if (achievement != null && !achievement.IsUnlocked)
            {
                achievement.Progress += progress;
                Console.WriteLine($"Achievement '{achievementName}' progress: {achievement.Progress}");
                if (achievement.Progress >= 100)
                {
                    UnlockAchievement(achievementName);
                }
            }
        }

        public void ShareAchievement(string achievementName)
        {
            var achievement = Achievements.Find(a => a.Name == achievementName);
            if (achievement != null && achievement.IsUnlocked)
            {
                Console.WriteLine($"Sharing achievement: {achievementName}");
            }
        }
            public void AddAchievement(string achievementName)
            {
                Achievements.Add(new Achievement(achievementName));
            }
            public bool HasAchievement(string achievementName)
            {
                return Achievements.Exists(a => a.Name == achievementName);
            }
            public void DisplayAchievements()
            {
                foreach (var achievement in Achievements)
                {
                    Console.WriteLine($"Achievement: {achievement.Name}, Unlocked: {achievement.IsUnlocked}, Progress: {achievement.Progress}");
                }
            }
    }
}
