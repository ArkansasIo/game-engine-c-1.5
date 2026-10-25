using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    public enum QuestType
    {
        Kill,
        Collect,
        Deliver,
        Escort,
        Explore,
        Craft,
        Talk,
        Defend,
        TimeLimit,
        Chain
    }

    public enum QuestStatus
    {
        Available,
        Active,
        Completed,
        Failed,
        Abandoned
    }

    public enum QuestDifficulty
    {
        Easy,
        Medium,
        Hard,
        Epic,
        Legendary
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string description;
        public QuestType objectiveType;
        public int targetAmount = 1;
        public int currentAmount = 0;
        public string targetName; // Name of item, creature, location, etc.
        public bool isCompleted = false;

        public QuestObjective(string desc, QuestType type, int target, string targetName = "")
        {
            description = desc;
            objectiveType = type;
            targetAmount = target;
            this.targetName = targetName;
        }

        public void UpdateProgress(int amount = 1)
        {
            currentAmount += amount;
            if (currentAmount >= targetAmount)
            {
                currentAmount = targetAmount;
                isCompleted = true;
            }
        }

        public bool IsCompleted()
        {
            return isCompleted;
        }

        public string GetProgressText()
        {
            return $"{currentAmount}/{targetAmount}";
        }
    }

    [System.Serializable]
    public class QuestReward
    {
        public int experience = 0;
        public int gold = 0;
        public List<GoonzuItem> items = new List<GoonzuItem>();
        public string specialReward = ""; // Special abilities, titles, etc.

        public QuestReward(int exp, int goldReward)
        {
            experience = exp;
            gold = goldReward;
        }

        public void AddItem(GoonzuItem item)
        {
            items.Add(item);
        }

        public void SetSpecialReward(string reward)
        {
            specialReward = reward;
        }
    }

    public class GoonzuQuest
    {
        public string questID;
        public string questName;
        public string description;
        public string giverName;
        public QuestDifficulty difficulty;
        public QuestStatus status = QuestStatus.Available;
        public List<QuestObjective> objectives = new List<QuestObjective>();
        public QuestReward reward;
        public int levelRequirement = 1;
        public float timeLimit = 0; // In minutes, 0 = no limit
        public float timeRemaining = 0;
        public List<string> prerequisites = new List<string>(); // Quest IDs that must be completed first
        public string nextQuestID = ""; // For quest chains
        public bool isRepeatable = false;

        public GoonzuQuest(string id, string name, string desc, string giver, QuestDifficulty diff)
        {
            questID = id;
            questName = name;
            description = desc;
            giverName = giver;
            difficulty = diff;

            reward = new QuestReward(GetBaseExperience(diff), GetBaseGold(diff));
        }

        int GetBaseExperience(QuestDifficulty diff)
        {
            switch (diff)
            {
                case QuestDifficulty.Easy: return 100;
                case QuestDifficulty.Medium: return 250;
                case QuestDifficulty.Hard: return 500;
                case QuestDifficulty.Epic: return 1000;
                case QuestDifficulty.Legendary: return 2500;
                default: return 100;
            }
        }

        int GetBaseGold(QuestDifficulty diff)
        {
            switch (diff)
            {
                case QuestDifficulty.Easy: return 50;
                case QuestDifficulty.Medium: return 150;
                case QuestDifficulty.Hard: return 300;
                case QuestDifficulty.Epic: return 750;
                case QuestDifficulty.Legendary: return 2000;
                default: return 50;
            }
        }

        public void AddObjective(QuestObjective objective)
        {
            objectives.Add(objective);
        }

        public void StartQuest()
        {
            status = QuestStatus.Active;
            if (timeLimit > 0)
            {
                timeRemaining = timeLimit * 60; // Convert to seconds
            }
            Debug.Log($"Started quest: {questName}");
        }

        public void UpdateQuest()
        {
            if (status != QuestStatus.Active) return;

            // Update time limit
            if (timeLimit > 0)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    status = QuestStatus.Failed;
                    Debug.Log($"Quest failed due to time limit: {questName}");
                    return;
                }
            }

            // Check if all objectives are completed
            bool allCompleted = true;
            foreach (var objective in objectives)
            {
                if (!objective.IsCompleted())
                {
                    allCompleted = false;
                    break;
                }
            }

            if (allCompleted)
            {
                CompleteQuest();
            }
        }

        public void CompleteQuest()
        {
            status = QuestStatus.Completed;
            Debug.Log($"Quest completed: {questName}");
        }

        public void FailQuest()
        {
            status = QuestStatus.Failed;
            Debug.Log($"Quest failed: {questName}");
        }

        public void AbandonQuest()
        {
            status = QuestStatus.Abandoned;
            Debug.Log($"Quest abandoned: {questName}");
        }

        public bool IsCompleted()
        {
            return status == QuestStatus.Completed;
        }

        public bool IsActive()
        {
            return status == QuestStatus.Active;
        }

        public bool IsAvailable()
        {
            return status == QuestStatus.Available;
        }

        public float GetCompletionPercentage()
        {
            if (objectives.Count == 0) return 100f;

            int completedObjectives = 0;
            float totalProgress = 0f;

            foreach (var objective in objectives)
            {
                if (objective.IsCompleted())
                {
                    completedObjectives++;
                    totalProgress += 1f;
                }
                else
                {
                    totalProgress += (float)objective.currentAmount / objective.targetAmount;
                }
            }

            return (totalProgress / objectives.Count) * 100f;
        }

        public string GetTimeRemainingText()
        {
            if (timeLimit <= 0) return "";

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            return $"{minutes}:{seconds:D2}";
        }
    }

    public class GoonzuQuestManager : MonoBehaviour
    {
        public static GoonzuQuestManager Instance { get; private set; }

        [Header("Quest Database")]
        public List<GoonzuQuest> questDatabase = new List<GoonzuQuest>();
        public Dictionary<string, GoonzuQuest> activeQuests = new Dictionary<string, GoonzuQuest>();
        public Dictionary<string, GoonzuQuest> completedQuests = new Dictionary<string, GoonzuQuest>();

        [Header("Quest Generation")]
        public int maxActiveQuests = 5;
        public float questUpdateInterval = 1f;
        private float lastUpdateTime = 0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            GenerateQuestDatabase();
        }

        void Update()
        {
            if (Time.time - lastUpdateTime >= questUpdateInterval)
            {
                UpdateActiveQuests();
                lastUpdateTime = Time.time;
            }
        }

        void GenerateQuestDatabase()
        {
            // Generate various quest types
            CreateKillQuests();
            CreateCollectQuests();
            CreateDeliverQuests();
            CreateExploreQuests();
            CreateCraftQuests();
            CreateTalkQuests();
            CreateDefendQuests();
            CreateEscortQuests();
            CreateChainQuests();
        }

        void CreateKillQuests()
        {
            string[] creatures = { "Goblin", "Wolf", "Spider", "Troll", "Orc", "Dragon" };
            string[] difficulties = { "Easy", "Medium", "Hard", "Epic" };

            for (int i = 0; i < 20; i++)
            {
                string creature = creatures[Random.Range(0, creatures.Length)];
                int count = Random.Range(3, 11);
                QuestDifficulty diff = (QuestDifficulty)Random.Range(0, 4);

                GoonzuQuest quest = new GoonzuQuest(
                    $"kill_{i}",
                    $"Slay {count} {creature}s",
                    $"The village is being terrorized by {creature}s. Help us by eliminating {count} of them.",
                    "Village Elder",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Kill {count} {creature}s",
                    QuestType.Kill,
                    count,
                    creature
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateCollectQuests()
        {
            string[] items = { "Herb", "Crystal", "Bone", "Scale", "Tooth", "Feather" };

            for (int i = 0; i < 15; i++)
            {
                string item = items[Random.Range(0, items.Length)];
                int count = Random.Range(5, 21);
                QuestDifficulty diff = (QuestDifficulty)Random.Range(0, 3);

                GoonzuQuest quest = new GoonzuQuest(
                    $"collect_{i}",
                    $"Collect {count} {item}s",
                    $"I need {count} {item}s for my research. Can you gather them for me?",
                    "Alchemist",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Collect {count} {item}s",
                    QuestType.Collect,
                    count,
                    item
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateDeliverQuests()
        {
            string[] items = { "Letter", "Package", "Potion", "Weapon", "Artifact" };
            string[] recipients = { "Merchant", "Guard Captain", "Mayor", "Priest", "Blacksmith" };

            for (int i = 0; i < 10; i++)
            {
                string item = items[Random.Range(0, items.Length)];
                string recipient = recipients[Random.Range(0, recipients.Length)];
                QuestDifficulty diff = QuestDifficulty.Easy;

                GoonzuQuest quest = new GoonzuQuest(
                    $"deliver_{i}",
                    $"Deliver {item} to {recipient}",
                    $"Please deliver this {item} to the {recipient} in the next town.",
                    "Postmaster",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Deliver {item} to {recipient}",
                    QuestType.Deliver,
                    1,
                    recipient
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateExploreQuests()
        {
            string[] locations = { "Ancient Ruins", "Dark Cave", "Mysterious Forest", "Mountain Peak", "Hidden Valley" };

            for (int i = 0; i < 8; i++)
            {
                string location = locations[Random.Range(0, locations.Length)];
                QuestDifficulty diff = (QuestDifficulty)Random.Range(1, 4);

                GoonzuQuest quest = new GoonzuQuest(
                    $"explore_{i}",
                    $"Explore {location}",
                    $"Legends speak of {location}. Go there and report what you find.",
                    "Explorer",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Explore {location}",
                    QuestType.Explore,
                    1,
                    location
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateCraftQuests()
        {
            string[] items = { "Sword", "Armor", "Potion", "Amulet", "Shield" };

            for (int i = 0; i < 6; i++)
            {
                string item = items[Random.Range(0, items.Length)];
                QuestDifficulty diff = (QuestDifficulty)Random.Range(1, 3);

                GoonzuQuest quest = new GoonzuQuest(
                    $"craft_{i}",
                    $"Craft a {item}",
                    $"I need a skilled craftsman to create a {item}. Will you help?",
                    "Master Craftsman",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Craft a {item}",
                    QuestType.Craft,
                    1,
                    item
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateTalkQuests()
        {
            string[] npcs = { "Wise Old Man", "Mysterious Stranger", "Village Gossip", "Royal Messenger" };

            for (int i = 0; i < 5; i++)
            {
                string npc = npcs[Random.Range(0, npcs.Length)];
                QuestDifficulty diff = QuestDifficulty.Easy;

                GoonzuQuest quest = new GoonzuQuest(
                    $"talk_{i}",
                    $"Talk to {npc}",
                    $"I need you to speak with the {npc}. They have important information.",
                    "Information Broker",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Talk to {npc}",
                    QuestType.Talk,
                    1,
                    npc
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateDefendQuests()
        {
            string[] targets = { "Village", "Caravan", "Castle", "Temple", "Mine" };

            for (int i = 0; i < 7; i++)
            {
                string target = targets[Random.Range(0, targets.Length)];
                int waves = Random.Range(2, 5);
                QuestDifficulty diff = (QuestDifficulty)Random.Range(1, 4);

                GoonzuQuest quest = new GoonzuQuest(
                    $"defend_{i}",
                    $"Defend the {target}",
                    $"The {target} is under attack! Help us defend it from {waves} waves of enemies.",
                    "Guard Captain",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Defend against {waves} waves of enemies",
                    QuestType.Defend,
                    waves,
                    target
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateEscortQuests()
        {
            string[] targets = { "Merchant", "Noble", "Priest", "Child", "Scholar" };

            for (int i = 0; i < 6; i++)
            {
                string target = targets[Random.Range(0, targets.Length)];
                QuestDifficulty diff = (QuestDifficulty)Random.Range(1, 3);

                GoonzuQuest quest = new GoonzuQuest(
                    $"escort_{i}",
                    $"Escort {target}",
                    $"Please escort this {target} safely to their destination.",
                    "Guard",
                    diff
                );

                QuestObjective obj = new QuestObjective(
                    $"Escort {target} to safety",
                    QuestType.Escort,
                    1,
                    target
                );
                quest.AddObjective(obj);

                questDatabase.Add(quest);
            }
        }

        void CreateChainQuests()
        {
            // Create a simple quest chain
            GoonzuQuest quest1 = new GoonzuQuest(
                "chain_1",
                "The Lost Amulet",
                "Find the lost amulet in the ancient ruins.",
                "Old Hermit",
                QuestDifficulty.Medium
            );
            quest1.AddObjective(new QuestObjective("Find the lost amulet", QuestType.Collect, 1, "Amulet"));
            quest1.nextQuestID = "chain_2";
            questDatabase.Add(quest1);

            GoonzuQuest quest2 = new GoonzuQuest(
                "chain_2",
                "The Ancient Curse",
                "The amulet is cursed! Find a way to break the curse.",
                "Old Hermit",
                QuestDifficulty.Hard
            );
            quest2.AddObjective(new QuestObjective("Break the curse on the amulet", QuestType.Craft, 1, "Purified Amulet"));
            quest2.prerequisites.Add("chain_1");
            quest2.nextQuestID = "chain_3";
            questDatabase.Add(quest2);

            GoonzuQuest quest3 = new GoonzuQuest(
                "chain_3",
                "Return the Amulet",
                "Return the purified amulet to its rightful owner.",
                "Old Hermit",
                QuestDifficulty.Medium
            );
            quest3.AddObjective(new QuestObjective("Return amulet to the Royal Family", QuestType.Deliver, 1, "Royal Family"));
            quest3.prerequisites.Add("chain_2");
            questDatabase.Add(quest3);
        }

        void UpdateActiveQuests()
        {
            List<string> completedQuestIds = new List<string>();

            foreach (var quest in activeQuests.Values)
            {
                quest.UpdateQuest();
                if (quest.IsCompleted())
                {
                    completedQuestIds.Add(quest.questID);
                }
            }

            foreach (var questId in completedQuestIds)
            {
                CompleteQuest(questId);
            }
        }

        public void AcceptQuest(string questID)
        {
            if (!questDatabase.Exists(q => q.questID == questID)) return;
            if (activeQuests.Count >= maxActiveQuests) return;
            if (activeQuests.ContainsKey(questID)) return;

            GoonzuQuest quest = questDatabase.Find(q => q.questID == questID);
            if (quest == null || !quest.IsAvailable()) return;

            // Check prerequisites
            foreach (string prereq in quest.prerequisites)
            {
                if (!completedQuests.ContainsKey(prereq))
                {
                    Debug.Log($"Cannot accept quest {questID}: prerequisite {prereq} not completed");
                    return;
                }
            }

            activeQuests[questID] = quest;
            quest.StartQuest();

            Debug.Log($"Accepted quest: {quest.questName}");
        }

        public void CompleteQuest(string questID)
        {
            if (!activeQuests.ContainsKey(questID)) return;

            GoonzuQuest quest = activeQuests[questID];
            activeQuests.Remove(questID);
            completedQuests[questID] = quest;

            // Grant rewards
            GoonzuCharacter player = GoonzuGameManager.Instance.GetPlayer();
            if (player != null)
            {
                player.GainExperience(quest.reward.experience);
                player.AddGold(quest.reward.gold);

                foreach (var item in quest.reward.items)
                {
                    player.AddItemToInventory(item);
                }

                if (!string.IsNullOrEmpty(quest.reward.specialReward))
                {
                    // Handle special rewards (titles, abilities, etc.)
                    Debug.Log($"Granted special reward: {quest.reward.specialReward}");
                }
            }

            // Check for next quest in chain
            if (!string.IsNullOrEmpty(quest.nextQuestID))
            {
                // Make next quest available
                Debug.Log($"Next quest in chain available: {quest.nextQuestID}");
            }

            Debug.Log($"Completed quest: {quest.questName}");
        }

        public void AbandonQuest(string questID)
        {
            if (!activeQuests.ContainsKey(questID)) return;

            GoonzuQuest quest = activeQuests[questID];
            activeQuests.Remove(questID);
            quest.AbandonQuest();

            Debug.Log($"Abandoned quest: {quest.questName}");
        }

        public void UpdateQuestProgress(QuestType type, string targetName, int amount = 1)
        {
            foreach (var quest in activeQuests.Values)
            {
                foreach (var objective in quest.objectives)
                {
                    if (objective.objectiveType == type && objective.targetName == targetName)
                    {
                        objective.UpdateProgress(amount);
                        Debug.Log($"Updated quest progress: {quest.questName} - {objective.description} ({objective.GetProgressText()})");
                    }
                }
            }
        }

        public List<GoonzuQuest> GetAvailableQuests()
        {
            List<GoonzuQuest> available = new List<GoonzuQuest>();

            foreach (var quest in questDatabase)
            {
                if (quest.IsAvailable() && !activeQuests.ContainsKey(quest.questID))
                {
                    // Check prerequisites
                    bool prerequisitesMet = true;
                    foreach (string prereq in quest.prerequisites)
                    {
                        if (!completedQuests.ContainsKey(prereq))
                        {
                            prerequisitesMet = false;
                            break;
                        }
                    }

                    if (prerequisitesMet)
                    {
                        available.Add(quest);
                    }
                }
            }

            return available;
        }

        public List<GoonzuQuest> GetActiveQuests()
        {
            return new List<GoonzuQuest>(activeQuests.Values);
        }

        public List<GoonzuQuest> GetCompletedQuests()
        {
            return new List<GoonzuQuest>(completedQuests.Values);
        }

        public GoonzuQuest GetQuestByID(string questID)
        {
            if (activeQuests.ContainsKey(questID)) return activeQuests[questID];
            if (completedQuests.ContainsKey(questID)) return completedQuests[questID];

            return questDatabase.Find(q => q.questID == questID);
        }

        public void GenerateRandomQuest(GoonzuCharacter character)
        {
            if (activeQuests.Count >= maxActiveQuests) return;

            List<GoonzuQuest> suitableQuests = questDatabase.FindAll(q =>
                q.IsAvailable() &&
                q.levelRequirement <= character.stats.level &&
                !activeQuests.ContainsKey(q.questID)
            );

            if (suitableQuests.Count > 0)
            {
                GoonzuQuest randomQuest = suitableQuests[Random.Range(0, suitableQuests.Count)];
                AcceptQuest(randomQuest.questID);
            }
        }
    }
}