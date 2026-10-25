using System;
using System.Collections.Generic;

namespace GameEngine.V15.Progression
{
    public sealed record AchievementDefinition(string Id, string Name, string Description, int Points);

    public sealed class AchievementSystem
    {
        private readonly Dictionary<string, AchievementDefinition> definitions = new();
        private readonly Dictionary<string, HashSet<string>> unlocked = new();

        public void Register(AchievementDefinition achievement) => definitions[achievement.Id] = achievement;

        public bool Unlock(string playerId, string achievementId)
        {
            if (!definitions.ContainsKey(achievementId)) throw new KeyNotFoundException(achievementId);
            if (!unlocked.TryGetValue(playerId, out var set)) unlocked[playerId] = set = new HashSet<string>();
            return set.Add(achievementId);
        }

        public bool IsUnlocked(string playerId, string achievementId) => unlocked.TryGetValue(playerId, out var set) && set.Contains(achievementId);
        public IReadOnlyCollection<string> GetUnlocked(string playerId) => unlocked.TryGetValue(playerId, out var set) ? set : Array.Empty<string>();
    }
}
