using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.V15.Quests
{
    public sealed record ObjectiveDefinition(string Id, string Description, int Required);

    public sealed class ObjectiveProgress
    {
        public ObjectiveDefinition Definition { get; }
        public int Current { get; private set; }
        public bool Complete => Current >= Definition.Required;
        public ObjectiveProgress(ObjectiveDefinition definition) => Definition = definition;
        public void Add(int amount) => Current = Math.Min(Definition.Required, Current + Math.Max(0, amount));
    }

    public sealed class QuestObjectiveSystem
    {
        private readonly Dictionary<string, List<ObjectiveProgress>> quests = new();
        public void Register(string questId, IEnumerable<ObjectiveDefinition> objectives) => quests[questId] = objectives.Select(x => new ObjectiveProgress(x)).ToList();
        public bool Progress(string questId, string objectiveId, int amount)
        {
            if (!quests.TryGetValue(questId, out var list)) return false;
            var objective = list.FirstOrDefault(x => x.Definition.Id == objectiveId);
            if (objective == null) return false;
            objective.Add(amount);
            return true;
        }
        public bool IsComplete(string questId) => quests.TryGetValue(questId, out var list) && list.All(x => x.Complete);
    }
}
