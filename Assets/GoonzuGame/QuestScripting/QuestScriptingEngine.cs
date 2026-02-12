using System;

namespace GoonzuGame.QuestScripting
{
    using System.Collections.Generic;
    using GoonzuGame.Quests;

    public class QuestEvent
    {
        public string Name { get; set; }
        public string Condition { get; set; }
        public string Outcome { get; set; }
        public QuestEvent(string name, string condition, string outcome)
        {
            Name = name;
            Condition = condition;
            Outcome = outcome;
        }
    }

    public class QuestScriptingEngine
        public void TriggerEvent(string eventName) {
            System.Console.WriteLine($"Event triggered: {eventName}");
        }
    {
        public List<Quest> Quests { get; set; }
        public List<QuestEvent> Events { get; set; }

        public QuestScriptingEngine()
        {
            Quests = new List<Quest>();
            Events = new List<QuestEvent>();
        }

        public void AddQuest(string questName, string description)
        {
            Quests.Add(new Quest { Title = questName, Description = description });
            Console.WriteLine($"Quest '{questName}' added.");
        }

        public void AddEvent(string eventName, string condition, string outcome)
        {
            Events.Add(new QuestEvent(eventName, condition, outcome));
            Console.WriteLine($"Event '{eventName}' added with condition '{condition}'.");
        }

        public void TriggerEvent(string eventName, Character player)
        {
            var evt = Events.Find(e => e.Name == eventName);
            if (evt != null && EvaluateCondition(evt.Condition, player))
            {
                Console.WriteLine($"Event '{eventName}' triggered: {evt.Outcome}");
                ApplyOutcome(evt.Outcome, player);
            }
            else
            {
                Console.WriteLine($"Event '{eventName}' not triggered.");
            }
        }

        public void TrackProgress(string questName, Character player)
        {
            var quest = Quests.Find(q => q.Title == questName);
            if (quest != null && !quest.IsCompleted)
            {
                Console.WriteLine($"Tracking progress for quest: {questName}");
                // Example: check if player has required items
                if (quest.RequiredItems.Count == 0 || quest.RequiredItems.TrueForAll(i => player.Inventory.Contains(i)))
                {
                    quest.Complete(player);
                }
            }
        }

        private bool EvaluateCondition(string condition, Character player)
        {
            // Example: simple condition check
            return condition == "HasItem" && player.Inventory.Count > 0;
        }

        private void ApplyOutcome(string outcome, Character player)
        {
            // Example: apply outcome
            if (outcome == "RewardGold")
                player.Gold += 100;
        }
    }
}
