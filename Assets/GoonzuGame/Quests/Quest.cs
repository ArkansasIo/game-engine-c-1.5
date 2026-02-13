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
            Debug.Log($"Started quest: {Title}");
        }

        public void Complete()
        {
            if (IsAllObjectivesCompleted())
            {
                IsCompleted = true;
                IsActive = false;
                GiveReward();
                Debug.Log($"Completed quest: {Title}");
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
            // Assuming player character
            var player = GoonzuGame.Characters.CharacterManager.Instance.PlayerCharacter;
            if (player != null)
            {
                player.GainExperience(Reward.Experience);
                // Add gold, items, etc.
                foreach (var item in Reward.Items)
                {
                    player.PickUpItem(item);
                }
            }
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

    public class QuestManager : MonoBehaviour
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
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
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
