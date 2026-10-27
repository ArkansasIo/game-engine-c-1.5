using System.Collections.Generic;
using System;

namespace GoonzuGame.Quests
{
    [System.Serializable]
    public enum QuestType
    {
        Kill,
        Collect,
        Explore,
        Deliver
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string Description;
        public QuestType Type;
        public string Target;
        public int RequiredAmount;
        public int CurrentAmount;

        public bool IsCompleted => CurrentAmount >= RequiredAmount;

        public void UpdateProgress(int amount = 1)
        {
            CurrentAmount += amount;
        }
    }

    [System.Serializable]
    public class QuestReward
    {
        public int Experience;
        public int Gold;
        public List<GoonzuGame.Items.Item> Items = new List<GoonzuGame.Items.Item>();
    }

    public class Quest
    {
        public string Title;
        public string Description;
        public bool IsCompleted;
        public bool IsActive;
        public List<QuestObjective> Objectives = new List<QuestObjective>();
        public QuestReward Reward;

        public Quest(string title, string description)
        {
            Title = title;
            Description = description;
            IsCompleted = false;
            IsActive = false;
            Reward = new QuestReward();
        }

        public void StartQuest()
        {
            IsActive = true;
            System.Console.WriteLine($"Started quest: {Title}");
        }

        public void Complete()
        {
            if (IsAllObjectivesCompleted())
            {
                IsCompleted = true;
                IsActive = false;
                GiveReward();
                System.Console.WriteLine($"Completed quest: {Title}");
            }
        }

        private bool IsAllObjectivesCompleted()
        {
            foreach (var obj in Objectives)
            {
                if (!obj.IsCompleted) return false;
            }
            return true;
        }

        private void GiveReward()
        {
            // Character Manager integration pending; rewards applied when a player is bound.
        }

        public void UpdateObjective(string target, int amount = 1)
        {
            foreach (var obj in Objectives)
            {
                if (obj.Target == target)
                {
                    obj.UpdateProgress(amount);
                    if (IsAllObjectivesCompleted())
                    {
                        Complete();
                    }
                }
            }
        }
    }

    public class QuestManager
    {
        public static QuestManager Instance { get; private set; }

        public List<Quest> AvailableQuests = new List<Quest>();
        public List<Quest> ActiveQuests = new List<Quest>();
        public List<Quest> CompletedQuests = new List<Quest>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void AddQuest(Quest quest)
        {
            AvailableQuests.Add(quest);
        }

        public void AcceptQuest(Quest quest)
        {
            if (AvailableQuests.Contains(quest))
            {
                AvailableQuests.Remove(quest);
                ActiveQuests.Add(quest);
                quest.StartQuest();
            }
        }

        public void UpdateQuestProgress(string target, int amount = 1)
        {
            foreach (var quest in ActiveQuests)
            {
                quest.UpdateObjective(target, amount);
            }
        }
    }
}
